Imports System.Reflection
Imports System.Text
Imports System.Threading.Tasks
Imports SkyEditorBase.Interfaces
Imports SkyEditorBase.Redistribution
Imports SkyEditorBase.Utilities

Public Class PluginManager
    Implements IDisposable

#Region "Constructors"
    Private Shared _instance As PluginManager
    ''' <summary>
    ''' Returns an instance of PluginManager, or returns nothing if an instance has not been created.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As PluginManager
        If _instance Is Nothing Then
            _instance = New PluginManager
            _instance.LoadPlugins(_instance.PluginFolder)
        End If
        Return _instance
    End Function
    ''' <summary>
    ''' Creates a new PluginManager, using the default storage location for plugins, which is Resources/Plugins, stored in the current working directory.
    ''' Plugins should end in _plg.dll or _plg.exe,  Ex. MyPlugin_plg.dll
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
        Me.New(IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins"))
        Me.CurrentProject = Nothing ' New Project(Me)
    End Sub

    ''' <summary>
    ''' Creates a new PluginManager given the folder plugin files are stored in.
    ''' Plugins should end in _plg.dll or _plg.exe,  Ex. MyPlugin_plg.dll
    ''' </summary>
    ''' <param name="PluginFolder"></param>
    ''' <remarks></remarks>
    Private Sub New(PluginFolder As String)
        'Me.Window = Window
        'Me.CheatManager = New ARDS.Manager
        'Me.Saves = New List(Of GenericSave)
        'LoadPlugins(PluginFolder)
        Me.PluginFolder = PluginFolder
        PluginHelper.PluginManagerInstance = Me
    End Sub
    Public Sub LoadPlugins(FromFolder As String)
        'Me.PluginFolder = FromFolder
        If IO.Directory.Exists(FromFolder) Then
            Dim assemblyPaths As New List(Of String)
            assemblyPaths.AddRange(IO.Directory.GetFiles(FromFolder, "*_plg.dll"))
            assemblyPaths.AddRange(IO.Directory.GetFiles(FromFolder, "*_plg.exe"))
            For Each plugin In assemblyPaths
                PluginHelper.Writeline("Opening plugin " & IO.Path.GetFileName(plugin))
                Dim a As Assembly = Assembly.LoadFrom(plugin)
                Try
                    Dim types As Type() = a.GetTypes
                    For Each item In types
                        Dim IsPlugin As Boolean = False
                        For Each intface As Type In item.GetInterfaces
                            If intface Is GetType(iSkyEditorPlugin) Then
                                IsPlugin = True
                            End If
                        Next
                        If IsPlugin Then
                            Dim Plg As iSkyEditorPlugin = a.CreateInstance(item.ToString)
                            Plugins.Add(Plg)
                            Assemblies.Add(a)
                        End If
                    Next
                Catch ex As System.Reflection.ReflectionTypeLoadException
                    PluginHelper.Writeline("Fatal error: System.Reflection.ReflectionTypeLoadException.", PluginHelper.LineType.Error)
                    PluginHelper.Writeline("Unable to load.  Deleting plugin and restarting program.", PluginHelper.LineType.Error)
                    PluginHelper.Writeline("Details: " & ex.ToString, PluginHelper.LineType.Error)
                    Redistribution.RedistributionHelpers.DeletePlugin(Me, IO.Path.GetFileName(plugin))
                End Try
            Next
            For Each item In Plugins
                item.Load(Me)
            Next
            For Each item In Assemblies
                'Load languages
                If IO.Directory.Exists(IO.Path.Combine(PluginHelper.GetResourceDirectory(item.GetName.Name), "Language")) Then
                    For Each lang In IO.Directory.GetFiles(IO.Path.Combine(PluginHelper.GetResourceDirectory(item.GetName.Name), "Language"), "*.txt")
                        Language.LanguageManager.ImportLanguageFile(lang, item.GetName.Name.Replace("_plg", ""))
                        'IO.File.Delete(lang)
                    Next
                End If
                'Load types
                Dim types As Type() = item.GetTypes
                For Each type In types
                    If ReflectionHelpers.IsOfType(type, GetType(ObjectTab)) Then
                        RegisterObjectTab(type)
                    ElseIf ReflectionHelpers.IsOfType(type, GetType(ObjectControl)) Then
                        RegisterObjectControl(type)
                    ElseIf ReflectionHelpers.IsOfType(type, GetType(Project))
                        RegisterProjectType(PluginHelper.GetLanguageItem(type.Name, CallingAssembly:=item.GetName.Name), type)
                    ElseIf ReflectionHelpers.IsOfType(type, GetType(GenericFile))
                        If type.GetMethod("IsFileOfType") IsNot Nothing Then
                            GenericFilesWithTypeValidator.Add(type)
                        End If
                    End If
                    For Each searcher In TypeSearcher
                        If ReflectionHelpers.IsOfType(type, searcher.Key) Then
                            searcher.Value.Invoke(type)
                        End If
                    Next
                    For Each i In type.GetInterfaces
                        If i Is GetType(Interfaces.iCreatableFile) AndAlso type.GetConstructor({}) IsNot Nothing Then
                            CreatableFiles.Add(type)
                        ElseIf i Is GetType(Interfaces.iOpenableFile) AndAlso type.GetConstructor({GetType(String)}) IsNot Nothing
                            OpenableFiles.Add(type)
                        End If
                    Next
                Next
            Next
            RegisterFileTypeDetector(AddressOf Me.DetectFileType)
            RegisterFileTypeDetector(AddressOf PluginManager.TryGetObjectFileType)

            RegisterConsoleCommand("distprep", AddressOf RedistributionHelpers.PrepareForDistribution)
            RegisterConsoleCommand("zip", AddressOf RedistributionHelpers.PackProgram)
            RegisterConsoleCommand("packplug", AddressOf RedistributionHelpers.PackPlugins)
            RegisterConsoleCommand("delplug", AddressOf RedistributionHelpers.DeletePlugin)
            RegisterConsoleCommand("generateinfo", AddressOf RedistributionHelpers.GeneratePluginDownloadDir)
            RegisterConsoleCommand("updateall", AddressOf RedistributionHelpers.DownloadAllPlugins)
            RegisterConsoleCommand("packall", AddressOf RedistributionHelpers.PackageAll)

            RegisterObjectControl(GetType(Language.LanguageEditor))
            RegisterObjectControl(GetType(SettingsEditor))
            'CheatManager.GameIDs = GameTypes
        End If
    End Sub
#End Region

#Region "Properties"
    ''' <summary>
    ''' Dictionary of (Extension, Friendly Name) used in the Open and Save file dialogs.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IOFilters As New Dictionary(Of String, String)

    ''' <summary>
    ''' Matches the save ID using the given game name.
    '''
    ''' Key: Game Type
    ''' Value: Save Type
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property GameTypes As New Dictionary(Of String, String)

    ''' <summary>
    ''' Dictionary matching console commands to the relevant PluginManager.ConsoleCommand delegate.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ConsoleCommandList As New Dictionary(Of String, ConsoleCommand)

    ''' <summary>
    ''' List of all the plugins' assembly names.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Assemblies As New List(Of Assembly)

    ''' <summary>
    ''' List of all loaded iSkyEditorPlugins that are loaded.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Plugins As New List(Of iSkyEditorPlugin)

    ''' <summary>
    ''' Dictionary containing all files needed by each plugin.
    ''' Excludes Assembly_plg.dll and /Assembly/, as these can be inferred by the assembly name of the plugin.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PluginFiles As New Dictionary(Of String, List(Of String))

    ''' <summary>
    ''' List of types of files that support creation (ie. a default constructor for the context of creating a new file).
    ''' </summary>
    ''' <returns></returns>
    Public Property CreatableFiles As New List(Of Type)

    ''' <summary>
    ''' List of types of files that support opening (ie. a constructor with a filename).
    ''' </summary>
    ''' <returns></returns>
    Public Property OpenableFiles As New List(Of Type)

    ''' <summary>
    ''' List of all GenericFile classes that have a shared method with signature: IsFileOfType(GenericFile) as Boolean
    ''' </summary>
    ''' <returns></returns>
    Public Property GenericFilesWithTypeValidator As New List(Of Type)

    Public Property CheatManager As New ARDS.Manager
    <Obsolete("Depricated.  Use FileTypeDetectors instead.")> Public Property SaveTypeDetectors As New List(Of SaveTypeDetector)
    Public Property FileTypeDetectors As New List(Of FileTypeDetector)
    Public Property SaveTypes As New Dictionary(Of String, Type)
    Public Property ProjectTypes As New Dictionary(Of String, Type)
    Public Property ObjectTabs As New List(Of ObjectTab)
    Public Property PluginFolder As String
    Public Property ObjectControls As New List(Of ObjectControl)
    Public Property TypeSearcher As New Dictionary(Of Type, TypeSearchFound)
    Private WithEvents _currentProject As Project
    Public Property CurrentProject As Project
        Get
            Return _currentProject
        End Get
        Set(value As Project)
            If _currentProject IsNot Nothing Then _currentProject.Dispose()
            _currentProject = value
            RaiseEvent ProjectChanged(Me, value)
        End Set
    End Property
#End Region

#Region "Delegates"
    Delegate Sub ConsoleCommand(ByVal Manager As PluginManager, ByVal Argument As String)
    <Obsolete("Depricated.  Use FileTypeDetector instead.")> Delegate Function SaveTypeDetector(SaveBytes As GenericFile) As String
    Delegate Function FileTypeDetector(File As GenericFile) As IEnumerable(Of Type)
    Delegate Sub TypeSearchFound(TypeFound As Type)
#End Region

#Region "Registration"
    Public Sub RegisterConsoleCommand(CommandName As String, Command As ConsoleCommand)
        If ConsoleCommandList Is Nothing Then
            ConsoleCommandList = New Dictionary(Of String, ConsoleCommand)
        End If
        ConsoleCommandList.Add(CommandName, Command)
        PluginHelper.Writeline("Registered console command """ & CommandName & """.")
    End Sub

    Private Function IsObjectTab(T As Type) As Boolean
        If T.BaseType Is GetType(ObjectTab) Then
            Return True
        Else
            If T.BaseType Is GetType(Object) Then
                Return False
            Else
                Return IsObjectTab(T.BaseType)
            End If
        End If
    End Function

    Private Sub RegisterObjectTab(Tab As Type)
        If IsObjectTab(Tab) Then
            ObjectTabs.Add(Tab.GetConstructor({}).Invoke({}))
        End If
    End Sub

    Public Sub RegisterObjectControl(T As Type)
        ObjectControls.Add(T.GetConstructor({}).Invoke({}))
    End Sub

    ''' <summary>
    ''' Registers a filter for use in open and save file dialogs.
    ''' </summary>
    ''' <param name="FileExtension">Filter for the dialog.  If this is by extension, should be *.extension</param>
    ''' <param name="FileFormatName">Name of the file format</param>
    Public Sub RegisterIOFilter(FileExtension As String, FileFormatName As String)
        Dim TempIOFilters As Dictionary(Of String, String) = IOFilters
        If TempIOFilters Is Nothing Then
            TempIOFilters = New Dictionary(Of String, String)
        End If
        TempIOFilters.Add(FileExtension, FileFormatName)
        IOFilters = TempIOFilters
    End Sub

    Public Sub RegisterMenuItem(Item As MenuItem)
        If Item IsNot Nothing Then
            Item.Tag = True
            'Window.AddMenuItem(Item)
            RaiseEvent MenuItemRegistered(Me, Item)
        End If
    End Sub
    ''' <summary>
    ''' Registers a save game format using the given information.
    ''' </summary>
    ''' <param name="GameName">Name of the specific game this format is for.  Include a relevant extension if applicable (Ex: "Pokemon X.nds", "My Game.gba", "Something Else.exe", etc).  Should be human readable, in English.</param>
    ''' <param name="SaveName">Human readable English identifier for the kind of save format this game uses.  If the given SaveFormat is used for another game, this should be the same for both games.  Do not include an extension.  (Ex: "Pokemon X/Y", "My Game", "Something Else")</param>
    ''' <param name="ContainerType">Type that represents the save file format.  Given Type should inherit SkyEditorBase.GenericSave</param>
    ''' <remarks></remarks>
    Public Sub RegisterSaveGameFormat(GameName As String, SaveName As String, ContainerType As Type)
        If Not GameTypes.ContainsKey(GameName) Then
            GameTypes.Add(GameName, SaveName)
        End If
        If Not SaveTypes.ContainsKey(SaveName) Then
            SaveTypes.Add(SaveName, ContainerType)
        End If
    End Sub

    Private Sub RegisterProjectType(ProjectName As String, ProjectType As Type)
        ProjectTypes.Add(ProjectName, ProjectType)
    End Sub

    '''' <summary>
    '''' Registers a file format using the given information.
    ''''
    '''' This is intended for generic files.  If this is a game save, use RegisterSaveGameFormat.
    '''' </summary>
    '''' <param name="FormatName">Human readable English identifier for the kind of save format this game uses.  Do not include an extension.</param>
    '''' <param name="ContainerType">Type that represents the save file format.  Given Type should inherit SkyEditorBase.GenericFile</param>
    '''' <remarks></remarks>
    'Public Sub RegisterFileFormat(FormatName As String, ContainerType As Type)
    '    If Not SaveTypes.ContainsKey(FormatName) Then
    '        SaveTypes.Add(FormatName, ContainerType)
    '    End If
    'End Sub

    <Obsolete("Depricated.  Use RegisterFileTypeDetector instead.")> Public Sub RegisterSaveTypeDetector(Detector As SaveTypeDetector)
        SaveTypeDetectors.Add(Detector)
    End Sub

    Public Sub RegisterFileTypeDetector(Detector As FileTypeDetector)
        FileTypeDetectors.Add(Detector)
    End Sub

    Public Sub RegisterCodeGenerator(Generator As ARDS.CodeDefinition)
        Me.CheatManager.CodeDefinitions.Add(Generator)
    End Sub
    ''' <summary>
    ''' Registers the given File Path as being a resource used by the calling plugin.
    ''' This should be used for files in the same directory as the plugin, that are a strict requirement of functionality for your plugin.
    ''' Example: IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins", "xceed.wpf.toolkit.dll")
    ''' </summary>
    ''' <param name="FilePath">If the file is in the same directory as your plugin, use something like IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins", "xceed.wpf.toolkit.dll")</param>
    ''' <remarks></remarks>
    Public Sub RegisterResourceFile(FilePath As String)
        Dim plugin As String = Assembly.GetCallingAssembly.GetName.Name
        If Not PluginFiles.ContainsKey(plugin) Then
            PluginFiles.Add(plugin, New List(Of String))
        End If
        PluginFiles(plugin).Add(FilePath)
    End Sub

    Public Sub RegisterTypeSearcher(TypeToSearch As Type, OnFound As TypeSearchFound)
        If Not TypeSearcher.ContainsKey(TypeToSearch) Then
            TypeSearcher.Add(TypeToSearch, OnFound)
        End If
    End Sub
#End Region

#Region "Functions"

#Region "IO Filters"
    ''' <summary>
    ''' Gets the IO filters in a form Open and Save file dialogs can use.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IOFiltersString(Optional Filters As Dictionary(Of String, String) = Nothing, Optional IsSaveAs As Boolean = False) As String
        If Filters Is Nothing Then
            Filters = IOFilters
        End If
        Dim listFilter As New StringBuilder
        Dim supportedFilterName As String = ""
        Dim supportedFilterExt As String = ""
        If Filters IsNot Nothing Then
            For Each item In Filters
                listFilter.Append(String.Format("{0} ({1})|{1}|", item.Value, item.Key))
                supportedFilterName &= item.Value & ", "
                supportedFilterExt &= "" & item.Key & ";"
            Next
            Dim out = ""
            If Not IsSaveAs Then
                out &= String.Format("{0} ({1})|{1}", supportedFilterName.Trim(";"), supportedFilterExt.Trim(";"))
            End If
            out &= "|" & listFilter.ToString & "All Files (*.*)|*.*"
            Return out.Trim("|")
        Else
            Return "All Files (*.*)|*.*"
        End If
    End Function

    ''' <summary>
    ''' Gets the IO filters in a form the Open and Save file dialogs can use, optimized for the Save file dialog.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IOFiltersStringSaveAs(DefaultExtension As String) As String
        Dim ext As String = DefaultExtension
        If IOFilters.ContainsKey(ext) Then
            Dim filters As New Dictionary(Of String, String)
            filters.Add(ext, IOFilters(ext))
            For Each item In IOFilters
                If Not filters.ContainsKey(item.Key) Then
                    filters.Add(item.Key, item.Value)
                End If
            Next
            Return IOFiltersString(filters, True)
        Else
            Return IOFiltersString()
        End If
    End Function
#End Region

#End Region

#Region "Events"
    'Public Event ConsoleCommandRegistered(sender As Object, ConsoleCommand As ConsoleCommand)
    'Public Event EditorTabRegistered(sender As Object, EditorTab As Type)
    'Public Event ObjectControlRegistered(sender As Object, ObjectControl As Type)
    Public Event IOFilterRegistered(sender As Object, IOFilter As Object)
    Public Event MenuItemRegistered(sender As Object, Item As MenuItem)
    'Public Event SaveGameFormatRegisterd(sender As Object, SaveGameFormat As Object)
    Public Event CodeGeneratorRegistered(sender As Object, CodeGenerator As ARDS.CodeDefinition)
    'Public Event ResourceFileRegistered(sender As Object, ResourceFile As Object)
    Public Event ProjectFileAdded(sender As Object, e As EventArguments.FileAddedEventArguments)
    Public Event ProjectFileRemoved(sender As Object, File As String)
    Public Event ProjectChanged(sender As Object, NewProject As Project)
    Public Event ProjectDirectoryCreated(sender As Object, File As String)
#End Region

    Public Function CreateNewFile(NewFileName As String, FileType As Type) As iCreatableFile
        If Not ReflectionHelpers.IsOfType(FileType, GetType(iOpenableFile)) Then
            Throw New ArgumentException("The given type must implement iCreatableFile.")
        End If
        Dim c = FileType.GetConstructor({})
        If c Is Nothing Then
            Throw New ArgumentException("The given type must provide a default constructor.")
        End If
        Dim file As iCreatableFile = c.Invoke({})
        file.CreateFile(NewFileName)
        Return file
    End Function

#Region "Open File"
    ''' <summary>
    ''' Creates a new instance of the given iOpenableFile type using the given filename.
    ''' </summary>
    ''' <param name="Filename">Filename of the file to open.</param>
    ''' <param name="FileType">Type of the class to create an instance of.  Must have a default constructor and implement iOpenableFile.</param>
    ''' <returns></returns>
    Public Function OpenFile(Filename As String, FileType As Type) As Object
        If String.IsNullOrEmpty(Filename) Then
            Throw New ArgumentNullException(NameOf(Filename))
        End If
        If Not ReflectionHelpers.IsOfType(FileType, GetType(iOpenableFile)) Then
            Throw New ArgumentException("The given type must implement iOpenableFile.")
        End If
        Dim c = FileType.GetConstructor({})
        If c Is Nothing Then
            Throw New ArgumentException("The given type must provide a default constructor.")
        Else
            Dim f As iOpenableFile = c.Invoke({})
            f.OpenFile(Filename)
            Return f
        End If
    End Function
    ''' <summary>
    ''' Auto-detects the file type and creates an instance of an appropriate class to model it.
    ''' </summary>
    ''' <param name="Filename"></param>
    ''' <returns></returns>
    Public Function OpenFile(Filename As String) As Object
        Return OpenFile(New GenericFile(Filename))
    End Function
    ''' <summary>
    ''' Using the given file, auto-detects the file type and creates an instance of an appropriate class.
    ''' If no appropriate file can be found, will return the given File.
    ''' </summary>
    ''' <param name="File"></param>
    ''' <returns></returns>
    Public Function OpenFile(File As GenericFile) As Object
        Dim type = GetFileType(File)
        If type Is Nothing OrElse Not ReflectionHelpers.IsOfType(type, GetType(Interfaces.iOpenableFile)) Then
            Return File
        Else
            Dim out As iOpenableFile = type.GetConstructor({}).Invoke({})
            out.OpenFile(File.OriginalFilename)
            File.Dispose()
            Return out
        End If
    End Function

#End Region

    ''' <summary>
    ''' Gets an object control that can edit the given object.
    ''' </summary>
    ''' <param name="ObjectToEdit"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetObjectControl(ObjectToEdit As Object) As ObjectControl
        Dim out As ObjectControl = Nothing
        If ObjectToEdit IsNot Nothing Then
            For Each item In (From o In ObjectControls Order By o.UsagePriority(ObjectToEdit.GetType) Descending)
                If out Is Nothing Then
                    For Each t In item.SupportedTypes
                        If ReflectionHelpers.IsOfType(ObjectToEdit, t) Then
                            out = item.GetType.GetConstructor({}).Invoke({})
                            Exit For
                        End If
                    Next
                Else
                    Exit For
                End If
            Next
        End If
        Return out
    End Function
    ''' <summary>
    ''' Gets tabs that can edit the specified object.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetRefreshedTabs(Save As Object) As List(Of TabItem)
        Dim tabs As New List(Of ObjectTab)
        For Each etab In (From e In ObjectTabs Order By e.SortOrder Descending)
            Dim isMatch = False
            If Not isMatch Then
                For Each t In etab.SupportedTypes
                    If Save IsNot Nothing AndAlso ReflectionHelpers.IsOfType(Save, t) Then 'If Save IsNot Nothing AndAlso ((TypeOf Save Is GenericSave AndAlso Save.IsOfType(t)) OrElse ReflectionHelpers.IsOfType(Save, t)) Then
                        isMatch = True
                        Exit For
                    End If
                Next
            End If
            If isMatch Then
                Dim t As ObjectTab = etab.GetType.GetConstructor({}).Invoke({})
                t.EditingObject = Save
                t.RefreshDisplay()
                tabs.Add(t)
            End If
        Next
        Dim out As New List(Of TabItem)
        For Each item In tabs
            Dim t As New TabItem
            item.Margin = New Thickness(0, 0, 0, 0)
            t.Content = item
            item.ParentTabItem = t
            out.Add(t)
        Next
        Return out
    End Function
    Private Class TypeComparer
        Implements IComparer(Of Type)

        Public Function Compare(x As Type, y As Type) As Integer Implements IComparer(Of Type).Compare
            Return GetInheritanceDepth(x).CompareTo(GetInheritanceDepth(y))
        End Function
        Public Function GetInheritanceDepth(T As Type) As Integer
            If Not T = GetType(Object) Then
                Return 1 + GetInheritanceDepth(T.BaseType)
            Else
                Return 0
            End If
        End Function
    End Class
    Public Function GetFileType(File As GenericFile) As Type
        Dim matches As New List(Of Type)
        For Each item In FileTypeDetectors
            Dim t = item.Invoke(File)
            If t IsNot Nothing Then
                For Each match In t
                    matches.Add(match)
                Next
            End If
        Next
        'If matches.Count > 0 Then
        '    Dim saveID As String = Nothing
        '    For Each item In SaveTypeDetectors
        '        saveID = item.Invoke(File)
        '        If Not String.IsNullOrEmpty(saveID) Then
        '            matches.Add(SaveTypes(saveID))
        '        End If
        '    Next
        'End If
        If matches.Count = 0 Then
            Return Nothing
        ElseIf matches.Count = 1
            Return matches(0)
        Else
            matches.Sort(New TypeComparer)
            matches.Reverse()
            Return matches(0)
            'Dim w As New SkyEditorWindows.GameTypeSelector()
            'Dim games As New Dictionary(Of String, Type)
            'For Each item In matches
            '    games.Add(PluginHelper.GetLanguageItem(item.Name), item)
            'Next
            'w.AddGames(games.Keys)
            'If w.ShowDialog Then
            '    Return games(w.SelectedGame)
            'Else
            '    Return Nothing
            'End If
        End If
    End Function

    Private Sub _currentProject_FileAdded(sender As Object, e As EventArguments.FileAddedEventArguments) Handles _currentProject.FileAdded
        RaiseEvent ProjectFileAdded(sender, e)
    End Sub

    Private Sub _currentProject_FileRemoved(sender As Object, File As String) Handles _currentProject.FileRemoved
        RaiseEvent ProjectFileRemoved(sender, File)
    End Sub

    Private Sub _currentProject_DirectoryCreated(sender As Object, Directory As String) Handles _currentProject.DirectoryCreated
        RaiseEvent ProjectDirectoryCreated(sender, Directory)
    End Sub

    Public Function DetectFileType(File As GenericFile) As IEnumerable(Of Type)
        Dim matches As New List(Of Type)
        For Each item In GenericFilesWithTypeValidator
            If item.GetMethod("IsFileOfType").Invoke(Nothing, {File}) Then
                matches.Add(item)
            End If
        Next
        If matches.Count = 0 Then
            Return Nothing
        Else
            Return matches
        End If
    End Function

    ''' <summary>
    ''' If the given file is of type ObjectFile, returns the contained Type.
    ''' Otherwise, returns Nothing.
    ''' </summary>
    ''' <param name="Filename"></param>
    ''' <returns></returns>
    Public Shared Function TryGetObjectFileType(Filename As String) As Type
        Try
            Dim f As New ObjectFile(Of Object)(Filename)
            'Doesn't work for ObjectFiles
            Return Utilities.ReflectionHelpers.GetTypeFromName(f.ContainedTypeName) 'GetType(ObjectFile(Of Object)).GetGenericTypeDefinition.MakeGenericType({Type.GetType(f.ContainedTypeName, AddressOf AssemblyResolver, AddressOf TypeResolver, False)})
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    ''' <summary>
    ''' If the given file is of type ObjectFile, returns the contained Type.
    ''' Otherwise, returns Nothing.
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function TryGetObjectFileType(File As GenericFile) As IEnumerable(Of Type)
        If File.RawData(0) = &H7B Then 'Check to see if the first character is "{".  Otherwise, we could try to open a 500+ MB file which takes much more RAM than we need.
            Dim result = TryGetObjectFileType(File.OriginalFilename)
            If result Is Nothing Then
                Return Nothing
            Else
                Return {result}
            End If
        Else
            Return Nothing
        End If
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                If CurrentProject IsNot Nothing Then
                    CurrentProject.Dispose()
                End If
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class
