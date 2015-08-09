'Imports System.Threading.Tasks
'Imports System.Text
'Imports System.Reflection

'Public Class PluginManagerOld
'    Implements IDisposable

'    Delegate Sub ConsoleCommand(ByVal Manager As PluginManager, ByVal Argument As String)
'    Delegate Function SaveTypeDetector(SaveBytes As GenericFile) As String

'    Public Function GetAssemblyVersion(Assembly As Assembly) As Version
'        Return Assembly.GetName.Version
'    End Function

'    Public Function GetAssemblyFileName(Assembly As Assembly) As String
'        Dim n = Assembly.GetName.Name
'        If IO.File.Exists(IO.Path.Combine(PluginFolder, n & ".dll")) Then
'            Return n & ".dll"
'        ElseIf IO.File.Exists(IO.Path.Combine(PluginFolder, n & ".exe")) Then
'            Return n & ".exe"
'        Else
'            Return n & ".dll"
'        End If
'    End Function

'#Region "Events"
'    Public Event CurrentSaveChanged(sender As Object, e As SaveChangedEventArgs)
'    Public Class SaveChangedEventArgs
'        Inherits EventArgs
'        Public Property OldSaveName As String
'        Public Property NewSaveName As String
'        Public Sub New(OldSaveName As String, NewSaveName As String)
'            Me.OldSaveName = OldSaveName
'            Me.NewSaveName = NewSaveName
'        End Sub
'    End Class

'    Public Event SaveAdded(sender As Object, e As SaveAddedEventArgs)
'    Public Class SaveAddedEventArgs
'        Inherits EventArgs
'        Public Property SaveName As String
'        Public Property Save As GenericSave
'        Public Sub New(SaveName As String, Save As GenericSave)
'            Me.SaveName = SaveName
'            Me.Save = Save
'        End Sub
'    End Class
'#End Region

'#Region "Properties"
'    ''' <summary>
'    ''' Gets or sets whether or not to show the loading window when applicable.
'    ''' Defaults to True, useful when using PluginManager to load plugins without a GUI.
'    ''' </summary>
'    ''' <value></value>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    Public Property ShowLoadingWindow As Boolean = True

'    ''' <summary>
'    ''' Matches the save ID using the given game name.
'    '''
'    ''' Key: Game Type
'    ''' Value: Save Type
'    ''' </summary>
'    ''' <value></value>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    Public ReadOnly Property GameTypes As Dictionary(Of String, String)
'        Get
'            Return _gameTypes
'        End Get
'    End Property
'    Dim _gameTypes As New Dictionary(Of String, String)

'    ''' <summary>
'    ''' Dicitonary matching console commands to the relevant PluginManager.ConsoleCommand delegate.
'    ''' </summary>
'    ''' <value></value>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    Public ReadOnly Property ConsoleCommandList As Dictionary(Of String, ConsoleCommand)
'        Get
'            Return _consoleCommands
'        End Get
'    End Property
'    Private _consoleCommands As Dictionary(Of String, ConsoleCommand)

'#Region "IO Filters"
'    ''' <summary>
'    ''' Dictionary of (Extension, Friendly Name) used in the Open and Save file dialogs.
'    ''' </summary>
'    ''' <value></value>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    Public Property IOFilters As Dictionary(Of String, String)
'        Get
'            Return _IOFilters
'        End Get
'        Set(value As Dictionary(Of String, String))
'            _IOFilters = value
'        End Set
'    End Property
'    Private _IOFilters As Dictionary(Of String, String)

'    ''' <summary>
'    ''' Gets the IO filters in a form Open and Save file dialogs can use.
'    ''' </summary>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    Public Function IOFiltersString(Optional Filters As Dictionary(Of String, String) = Nothing, Optional IsSaveAs As Boolean = False) As String
'        If Filters Is Nothing Then
'            Filters = IOFilters
'        End If
'        Dim listFilter As New StringBuilder
'        Dim supportedFilterName As String = ""
'        Dim supportedFilterExt As String = ""
'        If Filters IsNot Nothing Then
'            For Each item In Filters
'                listFilter.Append(String.Format("{0} ({1})|{1}|", item.Value, item.Key))
'                supportedFilterName &= item.Value & ", "
'                supportedFilterExt &= "" & item.Key & ";"
'            Next
'            Dim out = ""
'            If Not IsSaveAs Then
'                out &= String.Format("{0} ({1})|{1}", supportedFilterName.Trim(";"), supportedFilterExt.Trim(";"))
'            End If
'            out &= "|" & listFilter.ToString & "All Files (*.*)|*.*"
'            Return out.Trim("|")
'        Else
'            Return "All Files (*.*)|*.*"
'        End If
'    End Function

'    ''' <summary>
'    ''' Gets the IO filters in a form the Open and Save file dialogs can use, optimized for the Save file dialog.
'    ''' </summary>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    Public Function IOFiltersStringSaveAs() As String
'        If Save IsNot Nothing Then
'            Dim ext As String = Save.DefaultExtension
'            If IOFilters.ContainsKey(ext) Then
'                Dim filters As New Dictionary(Of String, String)
'                filters.Add(ext, IOFilters(ext))
'                For Each item In IOFilters
'                    If Not filters.ContainsKey(item.Key) Then
'                        filters.Add(item.Key, item.Value)
'                    End If
'                Next
'                Return IOFiltersString(filters, True)
'            Else
'                Return IOFiltersString()
'            End If
'        Else
'            Return IOFiltersString()
'        End If
'    End Function
'#End Region

'#Region "Saves"
'    Public Property Saves As List(Of GenericSave)

'    Public Property Saves(Name As String) As GenericSave
'        Get
'            Dim output As GenericSave = Nothing
'            For Each item In Me.Saves
'                If item.Name.ToLower = Name.ToLower Then
'                    output = item
'                    Exit For
'                End If
'            Next
'            Return output
'        End Get
'        Set(value As GenericSave)
'            For count As Integer = 0 To Me.Saves.Count - 1
'                If Me.Saves()(count).Name.ToLower = Name.ToLower Then
'                    Me.Saves()(count) = value
'                    Exit For
'                End If
'            Next
'        End Set
'    End Property

'    Dim _currentSave As String
'    Public Property CurrentSaveName As String
'        Get
'            Return _currentSave
'        End Get
'        Set(value As String)
'            Dim old = _currentSave
'            _currentSave = value
'            If Not String.IsNullOrEmpty(value) Then
'                RaiseEvent CurrentSaveChanged(Me, New SaveChangedEventArgs(old, value))
'            End If
'        End Set
'    End Property

'    ''' <summary>
'    ''' The currently loaded save.
'    ''' </summary>
'    ''' <value></value>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    Public Property Save As GenericSave
'        Get
'            Return Saves(CurrentSaveName)
'        End Get
'        Set(value As GenericSave)
'            Saves(CurrentSaveName) = value
'        End Set
'    End Property
'#End Region

'    ''' <summary>
'    ''' List of all the plugins' assembly names.
'    ''' </summary>
'    ''' <value></value>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    Public Property Assemblies As New List(Of Assembly)

'    ''' <summary>
'    ''' List of all loaded iSkyEditorPlugins that are loaded.
'    ''' </summary>
'    ''' <value></value>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    Public Property Plugins As New List(Of iSkyEditorPlugin)

'    ''' <summary>
'    ''' Dictionary containing all files needed by each plugin.
'    ''' Excludes Assembly_plg.dll and /Assembly/, as these can be inferred by the assembly name of the plugin.
'    ''' </summary>
'    ''' <value></value>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    Public Property PluginFiles As New Dictionary(Of String, List(Of String))
'    Public Property CheatManager As ARDS.Manager
'    Public Property SaveTypeDetectors As New List(Of SaveTypeDetector)
'    Public Property SaveTypes As New Dictionary(Of String, Type)
'    Public Property EditorTabs As New List(Of ObjectTab)
'    Public Property Window As iMainWindow
'    Public Property PluginFolder As String
'    Public Property ObjectControls As New List(Of ObjectControl)
'#End Region

'#Region "Constructors and Plugin Loading"
'    Public Sub New()
'        Me.New(Nothing, IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins"))
'    End Sub
'    ''' <summary>
'    ''' Creates a new PluginManager, using the default storage location for plugins, which is Resources/Plugins, stored in the current working directory.
'    ''' Plugins should end in _plg.dll or _plg.exe,  Ex. MyPlugin_plg.dll
'    ''' </summary>
'    ''' <remarks></remarks>
'    Public Sub New(Window As iMainWindow)
'        Me.New(Window, IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins"))
'    End Sub

'    ''' <summary>
'    ''' Creates a new PluginManager given the folder plugin files are stored in.
'    ''' Plugins should end in _plg.dll or _plg.exe,  Ex. MyPlugin_plg.dll
'    ''' </summary>
'    ''' <param name="PluginFolder"></param>
'    ''' <remarks></remarks>
'    Public Sub New(Window As iMainWindow, PluginFolder As String)
'        Me.Window = Window
'        Me.CheatManager = New ARDS.Manager
'        Me.Saves = New List(Of GenericSave)
'        LoadPlugins(PluginFolder)
'        'PluginHelper.PluginManagerInstance = Me
'    End Sub
'    Protected Sub LoadPlugins(FromFolder As String)
'        Me.PluginFolder = FromFolder
'        If IO.Directory.Exists(FromFolder) Then
'            Dim assemblyPaths As New List(Of String)
'            assemblyPaths.AddRange(IO.Directory.GetFiles(FromFolder, "*_plg.dll"))
'            assemblyPaths.AddRange(IO.Directory.GetFiles(FromFolder, "*_plg.exe"))
'            For Each plugin In assemblyPaths
'                PluginHelper.Writeline("Opening plugin " & IO.Path.GetFileName(plugin))
'                Dim a As Assembly = Assembly.LoadFrom(plugin)
'                Try
'                    Dim types As Type() = a.GetTypes
'                    For Each item In types
'                        Dim IsPlugin As Boolean = False
'                        For Each intface As Type In item.GetInterfaces
'                            If intface Is GetType(iSkyEditorPlugin) Then
'                                IsPlugin = True
'                            End If
'                        Next
'                        If IsPlugin Then
'                            Dim Plg As iSkyEditorPlugin = a.CreateInstance(item.ToString)
'                            Plugins.Add(Plg)
'                            Assemblies.Add(a)
'                        End If
'                    Next
'                Catch ex As System.Reflection.ReflectionTypeLoadException
'                    PluginHelper.Writeline("Fatal error: System.Reflection.ReflectionTypeLoadException.", PluginHelper.LineType.Error)
'                    PluginHelper.Writeline("Unable to load.  Deleting plugin and restarting program.", PluginHelper.LineType.Error)
'                    PluginHelper.Writeline("Details: " & ex.ToString, PluginHelper.LineType.Error)
'                    ' Redistribution.RedistributionHelpers.DeletePlugin(Me, IO.Path.GetFileName(plugin))
'                End Try
'            Next
'            For Each item In Assemblies
'                Dim types As Type() = item.GetTypes
'                For Each type In types
'                    If IsOfType(type, GetType(ObjectTab)) OrElse IsOfType(type, GetType(EditorTab)) Then
'                        RegisterEditorTab(type)
'                    ElseIf IsOfType(type, GetType(ObjectControl)) Then
'                        RegisterObjectControl(type)
'                    End If
'                Next
'            Next
'            If Window IsNot Nothing Then
'                For Each item In Plugins
'                    'item.Load(Me)
'                Next
'            End If
'            CheatManager.GameIDs = GameTypes
'        End If
'    End Sub
'    Public Sub ReloadPlugins()
'        LoadPlugins(Me.PluginFolder)
'    End Sub
'    Public Sub UnloadPlugins()
'        For Each item In Plugins
'            'item.UnLoad(Me)
'        Next
'        If Window IsNot Nothing Then Window.ClearTabItems()
'        Me.CheatManager.GameIDs = New Dictionary(Of String, String)
'        _consoleCommands = New Dictionary(Of String, ConsoleCommand)
'        EditorTabs = New List(Of ObjectTab)
'        IOFilters = New Dictionary(Of String, String)
'        Dim toRemove As New List(Of MenuItem)
'        If Window IsNot Nothing Then
'            For Each item In Window.GetMenuItems
'                If item.Tag = True Then
'                    toRemove.Add(item)
'                End If
'            Next
'            For Each item In toRemove
'                Window.RemoveMenuItem(item)
'            Next
'        End If
'        SaveTypes = New Dictionary(Of String, Type)
'        SaveTypeDetectors = New List(Of SaveTypeDetector)
'        _gameTypes = New Dictionary(Of String, String)
'        Me.CheatManager.CodeDefinitions = New List(Of ARDS.CodeDefinition)
'        PluginFiles = New Dictionary(Of String, List(Of String))
'        Plugins = New List(Of iSkyEditorPlugin)
'        'AppDomain.Unload(PluginDomain)
'        'PluginDomain = Nothing
'    End Sub
'#End Region

'#Region "Registration"
'    Public Sub RegisterConsoleCommand(CommandName As String, Command As ConsoleCommand)
'        If _consoleCommands Is Nothing Then
'            _consoleCommands = New Dictionary(Of String, ConsoleCommand)
'        End If
'        _consoleCommands.Add(CommandName, Command)
'    End Sub

'    Private Function IsObjectTab(T As Type) As Boolean
'        If T.BaseType Is GetType(ObjectTab) Then
'            Return True
'        Else
'            If T.BaseType Is GetType(Object) Then
'                Return False
'            Else
'                Return IsObjectTab(T.BaseType)
'            End If
'        End If
'    End Function

'    Public Sub RegisterEditorTab(Tab As Type)
'        If IsObjectTab(Tab) Then
'            EditorTabs.Add(Tab.GetConstructor({}).Invoke({}))
'        End If
'    End Sub

'    Public Sub RegisterObjectControl(T As Type)
'        ObjectControls.Add(T.GetConstructor({}).Invoke({}))
'    End Sub

'    Public Sub RegisterIOFilter(FileExtension As String, FileFormatName As String)
'        Dim TempIOFilters As Dictionary(Of String, String) = IOFilters
'        If TempIOFilters Is Nothing Then
'            TempIOFilters = New Dictionary(Of String, String)
'        End If
'        TempIOFilters.Add(FileExtension, FileFormatName)
'        IOFilters = TempIOFilters
'    End Sub

'    Public Sub RegisterMenuItem(Item As MenuItem)
'        If Item IsNot Nothing Then
'            Item.Tag = True
'            Window.AddMenuItem(Item)
'        End If
'    End Sub
'    ''' <summary>
'    ''' Registers a save game format using the given information.
'    ''' </summary>
'    ''' <param name="GameName">Name of the specific game this format is for.  Include a relevant extension if applicable (Ex: "Pokemon X.nds", "My Game.gba", "Something Else.exe", etc).  Should be human readable, in English.</param>
'    ''' <param name="SaveName">Human readable English identifier for the kind of save format this game uses.  If the given SaveFormat is used for another game, this should be the same for both games.  Do not include an extension.  (Ex: "Pokemon X/Y", "My Game", "Something Else")</param>
'    ''' <param name="ContainerType">Type that represents the save file format.  Given Type should inherit SkyEditorBase.GenericSave</param>
'    ''' <remarks></remarks>
'    Public Sub RegisterSaveGameFormat(GameName As String, SaveName As String, ContainerType As Type)
'        If Not GameTypes.ContainsKey(GameName) Then
'            GameTypes.Add(GameName, SaveName)
'        End If
'        If Not SaveTypes.ContainsKey(SaveName) Then
'            SaveTypes.Add(SaveName, ContainerType)
'        End If
'    End Sub

'    ''' <summary>
'    ''' Registers a file format using the given information.
'    '''
'    ''' This is intended for generic files.  If this is a game save, use RegisterSaveGameFormat.
'    ''' </summary>
'    ''' <param name="FormatName">Human readable English identifier for the kind of save format this game uses.  Do not include an extension.</param>
'    ''' <param name="ContainerType">Type that represents the save file format.  Given Type should inherit SkyEditorBase.GenericFile</param>
'    ''' <remarks></remarks>
'    Public Sub RegisterFileFormat(FormatName As String, ContainerType As Type)
'        If Not SaveTypes.ContainsKey(FormatName) Then
'            SaveTypes.Add(FormatName, ContainerType)
'        End If
'    End Sub

'    Public Sub RegisterSaveTypeDetector(Detector As SaveTypeDetector)
'        SaveTypeDetectors.Add(Detector)
'    End Sub

'    Public Sub RegisterCodeGenerator(Generator As ARDS.CodeDefinition)
'        Me.CheatManager.CodeDefinitions.Add(Generator)
'    End Sub
'    ''' <summary>
'    ''' Registers the given File Path as being a resource used by the calling plugin.
'    ''' This should be used for files in the same directory as the plugin, that are a strict requirement of functionality for your plugin.
'    ''' Example: IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins", "xceed.wpf.toolkit.dll")
'    ''' </summary>
'    ''' <param name="FilePath">If the file is in the same directory as your plugin, use something like IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins", "xceed.wpf.toolkit.dll")</param>
'    ''' <remarks></remarks>
'    Public Sub RegisterResourceFile(FilePath As String)
'        Dim plugin As String = Assembly.GetCallingAssembly.GetName.Name
'        If Not PluginFiles.ContainsKey(plugin) Then
'            PluginFiles.Add(plugin, New List(Of String))
'        End If
'        PluginFiles(plugin).Add(FilePath)
'    End Sub
'#End Region

'#Region "Refresh and Update"
'    Private Sub RefreshTabs(SaveName As String)
'        If ShowLoadingWindow Then PluginHelper.StartLoading(PluginHelper.GetLanguageItem("Refreshing tabs..."))
'        'If Window IsNot Nothing Then Window.ClearTabItems()
'        Dim tabs = GetRefreshedTabs(Saves(SaveName))
'        For Each item In tabs
'            If Window IsNot Nothing Then Window.AddTabItem(SaveName, item)
'        Next
'        PluginHelper.StopLoading()
'    End Sub
'    Private Shared Function IsOfType(Obj As Object, TypeToCheck As Type) As Boolean
'        Dim match = False
'        Dim g As Type = Nothing
'        If TypeOf Obj Is Type Then
'            If TypeToCheck.IsEquivalentTo(GetType(Type)) Then
'                match = True
'            Else
'                g = Obj
'            End If
'        Else
'            g = Obj.GetType
'        End If
'        If Not match Then
'            match = g.IsEquivalentTo(TypeToCheck) OrElse (g.BaseType IsNot Nothing AndAlso g.BaseType.IsEquivalentTo(TypeToCheck))
'        End If
'        If Not match Then
'            For Each item In g.GetInterfaces
'                If item.IsEquivalentTo(TypeToCheck) Then
'                    match = True
'                    Exit For
'                End If
'            Next
'        End If
'        Return match
'    End Function
'    ''' <summary>
'    ''' Gets EditorTabs for the given save.
'    ''' </summary>
'    ''' <param name="SaveName"></param>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    Public Function GetRefreshedTabs(Save As Object) As List(Of TabItem)
'        Dim tabs As New List(Of ObjectTab)
'        For Each etab In (From e In EditorTabs Order By e.SortOrder Descending)
'            Dim isMatch = False
'            If TypeOf etab Is EditorTab Then
'                For Each game In DirectCast(etab, EditorTab).SupportedGames
'                    If game IsNot Nothing AndAlso Save IsNot Nothing AndAlso (TypeOf Save Is GenericSave AndAlso Save.IsOfType(game)) Then
'                        'add the tab because this save is one of the supported games
'                        isMatch = True
'                        Exit For
'                    End If
'                Next
'            End If
'            If Not isMatch Then
'                For Each t In etab.SupportedTypes
'                    If Save IsNot Nothing AndAlso ((TypeOf Save Is GenericSave AndAlso Save.IsOfType(t)) OrElse IsOfType(Save, t)) Then
'                        isMatch = True
'                        Exit For
'                    End If
'                Next
'            End If
'            If isMatch Then
'                'If the tab is an EditorTab, only proceed if the save is a GenericSave
'                If TypeOf etab Is EditorTab Then
'                    isMatch = (TypeOf Save Is GenericSave)
'                End If
'            End If
'            If isMatch Then
'                Dim t As ObjectTab = etab.GetType.GetConstructor({}).Invoke({})
'                Dim a As New action(Sub()
'                                        t.RefreshDisplay(Save)
'                                    End Sub)
'                'Await Task.Run(x)
'                Dim x As New Task(a)
'                x.RunSynchronously()
'                tabs.Add(t)
'            End If
'        Next
'        Dim out As New List(Of TabItem)
'        For Each item In tabs
'            Dim t As New TabItem
'            'item.VerticalAlignment = VerticalAlignment.Top
'            'item.HorizontalAlignment = HorizontalAlignment.Left
'            item.Margin = New Thickness(0, 0, 0, 0)
'            t.Content = item
'            item.ParentTabItem = t
'            out.Add(t)
'        Next
'        Return out
'    End Function
'    Public Function GetObjectControl(ObjectToEdit As Object) As ObjectControl
'        Dim out As ObjectControl = Nothing
'        For Each item In (From o In ObjectControls Order By o.UsagePriority(ObjectToEdit.GetType) Descending)
'            If out Is Nothing Then
'                For Each t In item.SupportedTypes
'                    If IsOfType(ObjectToEdit, t) Then
'                        out = item.GetType.GetConstructor({}).Invoke({})
'                        Exit For
'                    End If
'                Next
'            Else
'                Exit For
'            End If
'        Next
'        Return out
'    End Function
'    Public Sub RefreshDisplay(SaveName As String)
'        RefreshTabs(SaveName)
'        If Settings.DebugMode Then Saves(SaveName).DebugInfo()
'    End Sub
'    Public Sub UpdateSave()
'        UpdateFromTabs()
'    End Sub

'    Private Sub UpdateFromTabs()
'        If Window IsNot Nothing Then
'            For Each item In (From t In Window.GetTabItems Order By DirectCast(t.Content, ObjectTab).UpdatePriority Descending)
'                Dim content = DirectCast(item, TabItem).Content
'                If TypeOf content Is EditorTab Then
'                    Save = DirectCast(content, EditorTab).UpdateSave(Save)
'                ElseIf TypeOf content Is ObjectTab Then
'                    Save = DirectCast(content, ObjectTab).UpdateObject(Save)
'                End If
'            Next
'        End If
'    End Sub
'#End Region

'#Region "Load Save"
'    ''' <summary>
'    ''' Loads the save from the given Filename.  If the save format cannot be determined, the built-in save type selector form will be shown.
'    ''' </summary>
'    ''' <param name="Filename"></param>
'    ''' <remarks></remarks>
'    Public Async Function LoadSave(Filename As String) As Task
'        Await LoadSave(Filename, New SkyEditorWindows.GameTypeSelector)
'    End Function
'    Public Function GetSaveType(File As GenericFile) As Type
'        Dim saveID As String = ""
'        Dim found As Boolean = False
'        For Each item In SaveTypeDetectors
'            saveID = item.Invoke(File)
'            If Not String.IsNullOrEmpty(saveID) Then
'                found = True
'                Exit For
'            End If
'        Next
'        If found Then
'            Return SaveTypes(saveID)
'        Else
'            Return Nothing
'        End If
'    End Function
'    Public Function OpenSave(File As GenericFile) As GenericFile
'        Dim type = GetSaveType(File)
'        If type Is Nothing Then
'            Return Nothing
'        Else
'            Return type.GetConstructor({GetType(String)}).Invoke({File.OriginalFilename})
'        End If
'    End Function
'    ''' <summary>
'    ''' Loads the save from the given Filename.  If the save format cannot be determined, the given Detector will be used to determine the save format.
'    ''' </summary>
'    ''' <param name="Filename"></param>
'    ''' <param name="Detector"></param>
'    ''' <remarks></remarks>
'    Public Async Function LoadSave(Filename As String, Detector As iGameTypeSelector) As Task
'        Dim loadingSave As GenericSave
'        Using d As New GenericFile(Filename)
'            Dim file = OpenSave(d)
'            If TypeOf file Is GenericSave Then
'                loadingSave = file
'            Else
'                loadingSave = Nothing
'            End If
'        End Using
'        If loadingSave IsNot Nothing Then
'            Dim name As String = IO.Path.GetFileName(Filename)
'            Dim count = 1
'            While Saves(name) IsNot Nothing
'                count += 1
'                name = String.Format("{0} ({1})", IO.Path.GetFileName(Filename), count)
'            End While
'            loadingSave.Name = name
'            Saves.Add(loadingSave)
'            If CurrentSaveName Is Nothing Then
'                CurrentSaveName = name
'            End If
'            Await RefreshDisplay(name)
'            AddHandler loadingSave.SaveIDUpdated, AddressOf OnSaveIDUpdated
'            RaiseEvent SaveAdded(Me, New SaveAddedEventArgs(name, loadingSave))
'        Else
'            Await LoadSaveNoAutodetect(Filename, Detector)
'        End If
'    End Function
'    ''' <summary>
'    ''' Loads the save from the given Filename, showing the built-in save type selector to determine the save format.
'    ''' </summary>
'    ''' <param name="Filename"></param>
'    ''' <remarks></remarks>
'    Public Async Function LoadSaveNoAutodetect(Filename As String) As Task
'        Await LoadSaveNoAutodetect(Filename, New SkyEditorWindows.GameTypeSelector)
'    End Function
'    ''' <summary>
'    ''' Loads the save from the given Filename, showing the given Detector to determine the save format.
'    ''' </summary>
'    ''' <param name="Filename"></param>
'    ''' <param name="Detector"></param>
'    ''' <remarks></remarks>
'    Public Async Function LoadSaveNoAutoDetect(Filename As String, Detector As iGameTypeSelector) As Task
'        Detector.AddGames(SaveTypes.Keys)
'        If Detector.ShowDialog() Then
'            Dim loadingSave As GenericSave
'            Dim d() As Byte = IO.File.ReadAllBytes(Filename)
'            Dim gameID As String = Detector.SelectedGame
'            If Not String.IsNullOrEmpty(gameID) Then
'                Dim constructor = SaveTypes(gameID).GetConstructor({GetType(String)}) 'Sub New (Filename as String)
'                If constructor IsNot Nothing Then
'                    loadingSave = constructor.Invoke({Filename})
'                Else
'                    constructor = SaveTypes(gameID).GetConstructor({GetType(Byte())}) 'Sub New (Bytes as Byte())
'                    loadingSave = constructor.Invoke({d})
'                End If
'                Dim name As String = IO.Path.GetFileName(Filename)
'                Dim count As Integer = 1
'                While Saves(name) IsNot Nothing
'                    count += 1
'                    name = String.Format("{0} ({1})", IO.Path.GetFileName(Filename), count)
'                End While
'                loadingSave.Name = name
'                Saves.Add(loadingSave)
'                If CurrentSaveName Is Nothing Then
'                    CurrentSaveName = name
'                End If
'                Await RefreshDisplay(name)
'                AddHandler loadingSave.SaveIDUpdated, AddressOf OnSaveIDUpdated
'                RaiseEvent SaveAdded(Me, New SaveAddedEventArgs(name, loadingSave))
'            End If
'        End If
'    End Function
'    Public Async Sub OnSaveIDUpdated(sender As Object, NewSaveID As String)
'        Await RefreshDisplay(DirectCast(sender, GenericSave).Name)
'    End Sub
'#End Region

'#Region "IDisposable Support"
'    Private disposedValue As Boolean ' To detect redundant calls

'    ' IDisposable
'    Protected Overridable Sub Dispose(disposing As Boolean)
'        If Not Me.disposedValue Then
'            If disposing Then
'                ' TODO: dispose managed state (managed objects).
'                For Each item In Plugins
'                    'item.UnLoad(Me)
'                Next
'            End If

'            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
'            ' TODO: set large fields to null.
'        End If
'        Me.disposedValue = True
'    End Sub

'    ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
'    'Protected Overrides Sub Finalize()
'    '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
'    '    Dispose(False)
'    '    MyBase.Finalize()
'    'End Sub

'    ' This code added by Visual Basic to correctly implement the disposable pattern.
'    Public Sub Dispose() Implements IDisposable.Dispose
'        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
'        Dispose(True)
'        GC.SuppressFinalize(Me)
'    End Sub
'#End Region

'End Class