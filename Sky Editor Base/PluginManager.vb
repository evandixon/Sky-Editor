Imports System.Reflection
Imports System.Text
Imports System.Threading.Tasks
Imports SkyEditorBase.EventArguments
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
    End Sub

    ''' <summary>
    ''' Creates a new PluginManager given the folder plugin files are stored in.
    ''' Plugins should end in _plg.dll or _plg.exe,  Ex. MyPlugin_plg.dll
    ''' </summary>
    ''' <param name="PluginFolder"></param>
    ''' <remarks></remarks>
    Private Sub New(PluginFolder As String)
        Me.CurrentSolution = Nothing
        Me.DirectoryTypeDetectors = New List(Of DirectoryTypeDetector)
        Me.PluginFolder = PluginFolder
        Me.TypeRegistery = New Dictionary(Of Type, List(Of Type))
        PluginHelper.PluginManagerInstance = Me
    End Sub
    Public Sub LoadPlugins()
        LoadPlugins(PluginFolder)
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

            RegisterTypeRegister(GetType(iObjectControl))
            RegisterTypeRegister(GetType(Solution))
            RegisterTypeRegister(GetType(Project))
            RegisterTypeRegister(GetType(iCreatableFile))
            RegisterTypeRegister(GetType(iOpenableFile))
            RegisterTypeRegister(GetType(iDetectableFileType))
            RegisterTypeRegister(GetType(ConsoleCommandAsync))
            RegisterTypeRegister(GetType(ITargetedControl))
            RegisterFileTypeDetector(AddressOf Me.DetectFileType)
            RegisterFileTypeDetector(AddressOf Me.TryGetObjectFileType)
            RegisterType(GetType(Solution), GetType(Solution))
            RegisterType(GetType(Project), GetType(Project))


            'Internal.PluginInfo.Load(Me)
            RaiseEvent PluginsLoading(Me, New PluginLoadingEventArgs)

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
                For Each actualType In types
                    'Check to see if this type inherits from one we're looking for
                    For Each registeredType In TypeRegistery.Keys
                        If ReflectionHelpers.IsOfType(actualType, registeredType) Then
                            If TypeRegistery(registeredType) Is Nothing Then
                                TypeRegistery(registeredType) = New List(Of Type)
                            End If
                            If Not TypeRegistery(registeredType).Contains(actualType) Then
                                TypeRegistery(registeredType).Add(actualType)
                            End If
                        End If
                    Next

                    'Do the same for each interface
                    For Each i In actualType.GetInterfaces
                        For Each registeredType In TypeRegistery.Keys
                            If ReflectionHelpers.IsOfType(i, registeredType) Then
                                If TypeRegistery(registeredType) Is Nothing Then
                                    TypeRegistery(registeredType) = New List(Of Type)
                                End If
                                If Not TypeRegistery(registeredType).Contains(actualType) Then
                                    TypeRegistery(registeredType).Add(actualType)
                                End If
                            End If
                        Next
                    Next
                Next
            Next
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
    Public Property FileTypeDetectors As New List(Of FileTypeDetector)
    Public Property DirectoryTypeDetectors As New List(Of directoryTypeDetector)
    Public Property SaveTypes As New Dictionary(Of String, Type)
    Public Property PluginFolder As String
    Public Property ObjectWindowType As Type
    Private Property MenuItems As List(Of MenuItemInfo)
    Private Property TypeRegistery As Dictionary(Of Type, List(Of Type))
    Private WithEvents _currentSolutoin As Solution
    Public Property CurrentSolution As Solution
        Get
            Return _currentSolutoin
        End Get
        Set(value As Solution)
            If _currentSolutoin IsNot Nothing Then _currentSolutoin.Dispose()
            _currentSolutoin = value
            RaiseEvent SolutionChanged(Me, New EventArgs)
        End Set
    End Property
#End Region

#Region "Delegates"
    Delegate Function FileTypeDetector(File As GenericFile) As IEnumerable(Of Type)
    Delegate Function DirectoryTypeDetector(Directory As IO.DirectoryInfo) As IEnumerable(Of Type)
    Delegate Sub TypeSearchFound(TypeFound As Type)
#End Region

#Region "Registration"
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

    ''' <summary>
    ''' Registers a Menu Action for use with creating custom menu items.
    ''' </summary>
    ''' <param name="ActionType">Type of the menu action to be registered.</param>
    Public Sub RegisterMenuActionType(ActionType As Type)
        If ActionType Is Nothing Then
            Throw New ArgumentNullException(NameOf(ActionType))
        End If
        If Not ReflectionHelpers.IsOfType(ActionType, GetType(MenuAction)) Then
            Throw New ArgumentException("Given type must inherit from MenuAction.", NameOf(ActionType))
        End If

        'While we're registering the type, we need an instance to get extra information, like where to put it
        Dim ActionInstance As MenuAction = ActionType.GetConstructor({}).Invoke({})


        'Generate the MenuItem
        If MenuItems Is Nothing Then
            MenuItems = New List(Of MenuItemInfo)
        End If

        If ActionInstance.ActionPath.Count >= 1 Then
            'Create parent menu items
            Dim parent = From m In MenuItems Where m.Header = ActionInstance.ActionPath(0)

            Dim current As MenuItemInfo
            If parent.Any Then
                current = parent.First
            Else
                Dim m As New MenuItemInfo
                m.Header = ActionInstance.ActionPath(0)
                m.Children = New List(Of MenuItemInfo)
                m.ActionTypes = New List(Of Type)
                If ActionInstance.ActionPath.Count = 1 Then
                    m.ActionTypes.Add(ActionType)
                End If
                MenuItems.Add(m)
                current = m
            End If


            For count = 1 To ActionInstance.ActionPath.Count - 2
                Dim index = count 'To avoid potential issues with using the below linq expression.  Might not be needed, but it's probably best to avoid potential issues.
                parent = From m As MenuItemInfo In current.Children Where m.Header = ActionInstance.ActionPath(index)
                If parent.Any Then
                    current = parent.First
                Else
                    Dim m As New MenuItemInfo
                    m.Header = ActionInstance.ActionPath(count)
                    m.Children = New List(Of MenuItemInfo)
                    If count = 0 Then
                        MenuItems.Add(m)
                    Else
                        current.Children.Add(m)
                    End If
                    current = m
                End If
            Next


            If ActionInstance.ActionPath.Count > 1 Then
                'Check to see if the menu item exists
                parent = From m As MenuItemInfo In current.Children Where m.Header = ActionInstance.ActionPath.Last

                If parent.Any Then
                    Dim m = DirectCast(parent.First, MenuItemInfo)
                    m.ActionTypes = New List(Of Type)
                    m.ActionTypes.Add(ActionType)
                Else
                    'Add the menu item, and give it a proper tag
                    Dim m As New MenuItemInfo
                    m.Children = New List(Of MenuItemInfo)
                    m.Header = ActionInstance.ActionPath.Last
                    m.ActionTypes = New List(Of Type)
                    m.ActionTypes.Add(ActionType)
                    current.Children.Add(m)
                End If
            End If

        Else 'Count=0
            Throw New ArgumentException("The action's ActionPath needs to contain at least 1 item.")
        End If

        'Call the event
        RaiseEvent MenuActionAdded(Me, New EventArguments.MenuActionAddedEventArgs With {.ActionType = ActionType})
    End Sub

    Public Sub RegisterFileTypeDetector(Detector As FileTypeDetector)
        FileTypeDetectors.Add(Detector)
    End Sub

    Public Sub RegisterDirectoryTypeDetector(Detector As DirectoryTypeDetector)
        DirectoryTypeDetectors.Add(Detector)
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

    ''' <summary>
    ''' Adds the given type to the type registry.
    ''' After plugins are loaded, any type that inherits or implements the given Type can be easily found.
    ''' 
    ''' If the type is already in the type registry, nothing will be done.
    ''' </summary>
    ''' <param name="Type"></param>
    Public Sub RegisterTypeRegister(Type As Type)
        If Type Is Nothing Then
            Throw New ArgumentNullException(NameOf(Type))
        End If

        If Not TypeRegistery.ContainsKey(Type) Then
            TypeRegistery.Add(Type, New List(Of Type))
        End If
    End Sub

    ''' <summary>
    ''' Registers the given Type in the type registry.
    ''' </summary>
    ''' <param name="Register">The base type or interface that the given Type inherits or implements.</param>
    ''' <param name="Type">The type to register.</param>
    Public Sub RegisterType(Register As Type, Type As Type)
        If Register Is Nothing Then
            Throw New ArgumentNullException(NameOf(Register))
        End If
        If Type Is Nothing Then
            Throw New ArgumentNullException(NameOf(Type))
        End If

        RegisterTypeRegister(Register)

        TypeRegistery(Register).Add(Type)
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

    ''' <summary>
    ''' Returns an IEnumerable of all the registered types that inherit or implement the given BaseType.
    ''' </summary>
    ''' <param name="BaseType">Type to get children or implementors of.</param>
    ''' <returns></returns>
    Public Function GetRegisteredTypes(BaseType As Type) As IEnumerable(Of Type)
        If BaseType Is Nothing Then
            Throw New ArgumentNullException(NameOf(BaseType))
        End If

        If TypeRegistery.ContainsKey(BaseType) Then
            Return TypeRegistery(BaseType)
        Else
            Return {}
        End If
    End Function

    Public Function GetRegisteredObjects(BaseType As Type) As IEnumerable(Of Object)
        Dim output As New List(Of Object)

        For Each item In GetRegisteredTypes(BaseType)
            output.Add(item.GetConstructor({}).Invoke({}))
        Next

        Return output
    End Function

    ''' <summary>
    ''' Returns an IEnumerable of all the registered types that implement iCreatableFile.
    ''' </summary>
    ''' <returns></returns>
    Public Function GetCreatableFiles() As IEnumerable(Of Type)
        Return GetRegisteredTypes(GetType(iCreatableFile))
    End Function

    ''' <summary>
    ''' Returns an IEnumerable of all the registered types that implement iOpenableFile.
    ''' </summary>
    ''' <returns></returns>
    Public Function GetOpenableFiles() As IEnumerable(Of Type)
        Return GetRegisteredTypes(GetType(iOpenableFile))
    End Function

    ''' <summary>
    ''' Returns an IEnumerable of all the registered types that implement iDetectableFileType.
    ''' </summary>
    ''' <returns></returns>
    Public Function GetDetectableFileTypes() As IEnumerable(Of Type)
        Return GetRegisteredTypes(GetType(iDetectableFileType))
    End Function

    Public Function GetConsoleCommands() As Dictionary(Of String, SkyEditorBase.ConsoleCommandAsync)
        Dim out As New Dictionary(Of String, ConsoleCommandAsync)
        For Each item As ConsoleCommandAsync In GetRegisteredObjects(GetType(ConsoleCommandAsync))
            If Not out.ContainsKey(item.CommandName) Then
                out.Add(item.commandname, item)
            End If
        Next
        Return out
    End Function

    ''' <summary>
    ''' Returns data that can be used to make MenuItems that run MenuActions.
    ''' </summary>
    ''' <returns></returns>
    Public Function GetMenuItemInfo() As IEnumerable(Of MenuItemInfo)
        Return MenuItems
    End Function

    ''' <summary>
    ''' Returns a new instance of each registered ObjectControl.
    ''' </summary>
    ''' <returns></returns>
    Public Function GetObjectControls() As IEnumerable(Of iObjectControl)
        Dim output As New List(Of iObjectControl)

        For Each item In GetRegisteredTypes(GetType(iObjectControl))
            output.Add(item.GetConstructor({}).Invoke({}))
        Next

        Return output
    End Function

    ''' <summary>
    ''' Returns a new instance of each registered Project.
    ''' </summary>
    ''' <returns></returns>
    Public Function GetProjects() As IEnumerable(Of ProjectOld)
        Dim output As New List(Of ProjectOld)

        For Each item In GetRegisteredTypes(GetType(ProjectOld))
            output.Add(item.GetConstructor({}).Invoke({}))
        Next

        Return output
    End Function

    ''' <summary>
    ''' Returns the type of the project with the given Project Type Name
    ''' </summary>
    ''' <param name="ProjectTypeName">Name of the Project Type.</param>
    ''' <returns></returns>
    Public Function GetProjectType(ProjectTypeName As String) As Type
        Dim q = From p In GetProjects() Where p.GetProjectTypeName = ProjectTypeName Select p

        If q.Any Then
            Return q.First.GetType
        Else
            Return Nothing
        End If
    End Function

#End Region

#Region "Events"
    ''' <summary>
    ''' Raised before each plugin's Load method is called.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Public Event PluginsLoading(sender As Object, e As PluginLoadingEventArgs)
    Public Event ProjectFileAdded(sender As Object, e As FileAddedEventArguments)
    Public Event ProjectFileRemoved(sender As Object, e As FileRemovedEventArgs)
    Public Event ProjectChanged(sender As Object, e As ProjectChangedEventArgs)
    Public Event ProjectDirectoryCreated(sender As Object, e As ProjectDirectoryCreatedEventArgs)
    Public Event ProjectModified(sender As Object, e As EventArgs)
    Public Event MenuActionAdded(sender As Object, e As MenuActionAddedEventArgs)
    Public Event SolutionChanged(sender As Object, e As EventArgs)
#End Region

#Region "Event Handlers"
    Private Sub _currentProject_FileAdded(sender As Object, e As EventArguments.FileAddedEventArguments) ' Handles _currentProject.FileAdded
        RaiseEvent ProjectFileAdded(sender, e)
    End Sub

    Private Sub _currentProject_FileRemoved(sender As Object, File As String) ' Handles _currentProject.FileRemoved
        RaiseEvent ProjectFileRemoved(sender, New FileRemovedEventArgs With {.File = File})
    End Sub

    Private Sub _currentProject_DirectoryCreated(sender As Object, Directory As String) ' Handles _currentProject.DirectoryCreated
        RaiseEvent ProjectDirectoryCreated(sender, New EventArguments.ProjectDirectoryCreatedEventArgs With {.Directory = Directory})
    End Sub

    Private Sub _currentProject_Modified(sender As Object, e As EventArgs) ' Handles _currentProject.Modified
        RaiseEvent ProjectModified(sender, e)
    End Sub

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
    ''' Auto-detects the file/directory type and creates an instance of an appropriate class to model it.
    ''' </summary>
    ''' <param name="Filename"></param>
    ''' <returns></returns>
    Public Function OpenObject(Filename As String) As Object
        If IO.File.Exists(Filename) Then
            Return OpenFile(New GenericFile(Filename, True))
        Else
            If IO.Directory.Exists(Filename) Then
                Return OpenDirectory(New IO.DirectoryInfo(Filename))
            Else
                Return Nothing
            End If
        End If
    End Function
    ''' <summary>
    ''' Using the given file, auto-detects the file type and creates an instance of an appropriate class.
    ''' If no appropriate file can be found, will return the given File.
    ''' </summary>
    ''' <returns></returns>
    Public Function OpenFile(File As GenericFile) As Object
        Dim type = GetFileType(File)
        If type Is Nothing OrElse Not ReflectionHelpers.IsOfType(type, GetType(Interfaces.iOpenableFile)) Then
            'Reopen the file without being readonly
            Dim filename = File.OriginalFilename
            File.Dispose()
            Return New GenericFile(File.OriginalFilename, False)
        Else
            Dim out As iOpenableFile = type.GetConstructor({}).Invoke({})
            out.OpenFile(File.OriginalFilename)
            File.Dispose()
            Return out
        End If
    End Function

    ''' <summary>
    ''' Sometimes a "file" actually exists as multiple files in a directory.  This method will open a "file" using the given directory.
    ''' </summary>
    ''' <returns></returns>
    Public Function OpenDirectory(Directory As IO.DirectoryInfo) As Object
        Dim type = GetDirectoryType(Directory)
        If type Is Nothing OrElse Not ReflectionHelpers.IsOfType(type, GetType(Interfaces.iOpenableFile)) Then
            'Let's not return nothing.  Maybe something wants to use the directory info.
            Return Directory
        Else
            Dim out As iOpenableFile = type.GetConstructor({}).Invoke({})
            out.OpenFile(Directory.FullName)
            Return out
        End If
    End Function

#End Region

    Public Function GetObjectWindow() As Interfaces.iObjectWindow
        Return ObjectWindowType.GetConstructor({}).Invoke({})
    End Function

    ''' <summary>
    ''' Gets an object control that can edit the given object.
    ''' </summary>
    ''' <param name="ObjectToEdit"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetObjectControl(ObjectToEdit As Object) As iObjectControl
        Dim out As iObjectControl = Nothing
        If ObjectToEdit IsNot Nothing Then
            For Each item In (From o In GetObjectControls() Order By o.GetSortOrder(ObjectToEdit.GetType, False) Descending)
                If out Is Nothing Then
                    For Each t In item.GetSupportedTypes
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
    ''' Returns a list of iObjectControl that edit the given ObjectToEdit.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetRefreshedTabs(ObjectToEdit As Object) As List(Of iObjectControl)
        If ObjectToEdit Is Nothing Then
            Throw New ArgumentNullException(NameOf(ObjectToEdit))
        End If

        Dim tabs As New List(Of iObjectControl)
        For Each etab In (From e In GetObjectControls() Order By e.GetSortOrder(ObjectToEdit.GetType, True) Ascending)
            Dim isMatch = False
            If Not isMatch Then
                For Each t In etab.GetSupportedTypes
                    If ObjectToEdit IsNot Nothing AndAlso ReflectionHelpers.IsOfType(ObjectToEdit, t) Then
                        isMatch = True
                        Exit For
                    End If
                Next
            End If
            If isMatch Then
                Dim t As iObjectControl = etab.GetType.GetConstructor({}).Invoke({})
                t.EditingObject = ObjectToEdit
                tabs.Add(t)
            End If
        Next
        'Dim out As New List(Of TabItem)
        'For Each item In tabs
        '    Dim t As New TabItem
        '    item.Margin = New Thickness(0, 0, 0, 0)
        '    t.Content = item
        '    item.ParentTabItem = t
        '    out.Add(t)
        'Next
        Return tabs
    End Function

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

        If matches.Count = 0 Then
            Return Nothing
        ElseIf matches.Count = 1 Then
            Return matches(0)
        Else
            matches.Sort(New Utilities.ReflectionHelpers.TypeInheritanceDepthComparer)
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

    Public Function GetDirectoryType(Directory As IO.DirectoryInfo) As Type
        Dim matches As New List(Of Type)
        For Each item In DirectoryTypeDetectors
            Dim t = item.Invoke(Directory)
            If t IsNot Nothing Then
                For Each match In t
                    matches.Add(match)
                Next
            End If
        Next

        If matches.Count = 0 Then
            Return Nothing
        ElseIf matches.Count = 1 Then
            Return matches(0)
        Else
            matches.Sort(New Utilities.ReflectionHelpers.TypeInheritanceDepthComparer)
            matches.Reverse()
            Return matches(0)
        End If
    End Function

    Public Function DetectFileType(File As GenericFile) As IEnumerable(Of Type)
        Dim matches As New List(Of Type)

        If ExecutableFile.IsExeFile(File.OriginalFilename) Then
            matches.Add(GetType(ExecutableFile))
        End If

        If matches.Count = 0 Then
            For Each item In GetDetectableFileTypes()
                Dim instance As iDetectableFileType = item.GetConstructor({}).Invoke({})
                If instance.isoftype(File) Then
                    matches.Add(item)
                End If
            Next
        End If

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
    Public Function TryGetObjectFileType(File As GenericFile) As IEnumerable(Of Type)
        If File.Length > 0 AndAlso File.RawData(0) = &H7B Then 'Check to see if the first character is "{".  Otherwise, we could try to open a 500+ MB file which takes much more RAM than we need.
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
                If CurrentSolution IsNot Nothing Then
                    CurrentSolution.Dispose()
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
