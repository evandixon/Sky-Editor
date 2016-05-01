Imports System.Reflection
Imports System.Text
Imports System.Threading.Tasks
Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.Extensions.Plugins
Imports SkyEditor.Core.Windows
Imports SkyEditorBase.EventArguments
Imports SkyEditorBase.Interfaces

Public Class PluginManager
    Inherits SkyEditor.Core.Extensions.Plugins.PluginManager
    Implements IDisposable
    Implements iNamed

#Region "Constructors"
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
    Private Shared _instance As PluginManager

    ''' <summary>
    ''' Creates a new PluginManager, using the default storage location for plugins, which is Resources/Plugins, stored in the current working directory.
    ''' Plugins should end in _plg.dll or _plg.exe,  Ex. MyPlugin_plg.dll
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
        MyBase.New

        Me.CurrentSolution = Nothing
        Assemblies = New List(Of Assembly)
        Me.DirectoryTypeDetectors = New List(Of DirectoryTypeDetector)
        Me.TypeRegistery = New Dictionary(Of TypeInfo, List(Of TypeInfo))
        Me.FailedPluginLoads = New List(Of String)
        Me.OpenedFiles = New Dictionary(Of Object, ProjectOld)
        Me.DependantPlugins = New Dictionary(Of Assembly, List(Of Assembly))

        AddHandler PluginHelper.FileOpenRequested, AddressOf _pluginHelper_FileOpened
        AddHandler PluginHelper.FileClosed, AddressOf _pluginHelper_FileClosed
    End Sub
#End Region

#Region "Plugin Loading"
    ''' <summary>
    ''' Returns a list of the plugin paths that are valid .Net assemblies that contain an iPlugin.
    ''' </summary>
    ''' <param name="PluginPaths">Full paths of the plugin assemblies to analyse.</param>
    ''' <param name="CoreAssemblyName">Name of the core assembly, usually the Entry assembly.  Assemblies with this name are not supported, to avoid loading duplicates.</param>
    ''' <returns></returns>
    Public Overrides Function GetSupportedPlugins(PluginPaths As IEnumerable(Of String), Optional CoreAssemblyName As String = Nothing) As List(Of String)
        Dim supportedList As New List(Of String)
        'We're going to load these assemblies into another appdomain, so we don't accidentally create duplicates, and so we don't keep any unneeded assemblies loaded for the life of the application.
        Using reflectionManager As New Utilities.AssemblyReflectionManager
            For Each item In PluginPaths
                reflectionManager.LoadAssembly(item, "PluginManagerAnalysis")

                Dim pluginInfoNames As New List(Of String)

                Try
                    pluginInfoNames =
                            reflectionManager.Reflect(item,
                                                      Function(a As Assembly, Args() As Object) As List(Of String)
                                                          Dim out As New List(Of String)

                                                          If a IsNot Nothing AndAlso
                                                            Not (a.FullName = Assembly.GetCallingAssembly.FullName OrElse
                                                                    (Assembly.GetEntryAssembly IsNot Nothing AndAlso a.FullName = Assembly.GetEntryAssembly.FullName) OrElse
                                                                    a.FullName = Assembly.GetExecutingAssembly.FullName OrElse
                                                                    (Args(0) IsNot Nothing AndAlso a.FullName = Args(0))
                                                                    ) Then
                                                              For Each t As Type In a.GetTypes
                                                                  If ReflectionHelpers.IsOfType(t, GetType(SkyEditorPlugin)) Then
                                                                      out.Add(t.FullName)
                                                                  End If
                                                              Next
                                                          End If

                                                          Return out
                                                      End Function, CoreAssemblyName)
                Catch ex As Reflection.ReflectionTypeLoadException
                    'If we fail here, then the assembly is NOT a valid plugin, so we won't load it.
                    Console.WriteLine(ex.ToString)
                Catch ex As IO.FileNotFoundException
                    'If we fail here, then the assembly is missing some of its references, meaning it's not a valid plugin.
                    Console.WriteLine(ex.ToString)
                End Try

                If pluginInfoNames.Count > 0 Then
                    'Then we want to keep this assembly
                    supportedList.Add(item)
                End If
            Next
        End Using 'The reflection appdomain will be unloaded on dispose
        Return supportedList
    End Function



    ''' <summary>
    ''' Loads all available plugins using the given CoreMod.
    ''' </summary>
    ''' <param name="CoreMod"></param>
    Public Overrides Sub LoadPlugins(CoreMod As SkyEditorPlugin)
        'Me.PluginFolder = FromFolder
        Dim devAssemblyPaths As New List(Of String)
        Dim saveAssemblies As Boolean = False

        ''Load plugins from settings
        'For Each item In SettingsManager.Instance.Settings.Plugins
        '    assemblyPaths.Add(IO.Path.Combine(FromFolder, item))
        'Next

        'Register plugin extension type, since we're about to use it to load more plugins
        Me.RegisterTypeRegister(GetType(Extensions.ExtensionType))
        Me.RegisterType(GetType(Extensions.ExtensionType), GetType(Extensions.PluginExtensionType))

        'Note the core assembly name, so we don't accidentally try to load it again (seeing that it's already in the AppDomain).
        CoreAssemblyName = CoreMod.GetType.Assembly.FullName

        Dim supportedPlugins As New List(Of String)

        'Look at the plugin extensions to find plugins.
        Dim pluginExtType As New Extensions.PluginExtensionType
        For Each item In pluginExtType.GetInstalledExtensions
            Dim extAssemblies As New List(Of String)
            For Each file In item.ExtensionFiles
                extAssemblies.Add(IO.Path.Combine(pluginExtType.GetExtensionDirectory(item), file))
            Next
            'extAssemblies.AddRange(IO.Directory.GetFiles(pluginExtType.GetExtensionDirectory(item), "*.dll"))
            'extAssemblies.AddRange(IO.Directory.GetFiles(pluginExtType.GetExtensionDirectory(item), "*.exe"))
            supportedPlugins.AddRange(GetSupportedPlugins(extAssemblies, CoreAssemblyName))
        Next

        'If we're in dev mode, or if we couldn't find any plugin extensions, then load plugins from the dev directory
        If SettingsManager.Instance.Settings.DevelopmentMode OrElse supportedPlugins.Count = 0 Then
            saveAssemblies = True
            Dim available = PluginHelper.GetPluginAssemblies
            For Each item In available
                If Not devAssemblyPaths.Contains(item) Then
                    devAssemblyPaths.Add(item)
                End If
            Next
            supportedPlugins.AddRange(GetSupportedPlugins(devAssemblyPaths, CoreAssemblyName))
        End If

        For Each item In supportedPlugins
            Dim assemblyActual = Assembly.LoadFrom(item)
            Assemblies.Add(assemblyActual)
            For Each plg In From t In assemblyActual.GetTypes Where ReflectionHelpers.IsOfType(t, GetType(SkyEditorPlugin)) AndAlso t.GetConstructor({}) IsNot Nothing
                Plugins.Add(plg.GetConstructor({}).Invoke({}))
            Next
        Next

        ''If we found searched for plugins, then save the paths to the settings
        'If saveAssemblies Then
        '    SettingsManager.Instance.Settings.Plugins.Clear()
        '    For Each item In supportedPlugins
        '        SettingsManager.Instance.Settings.Plugins.Add(item.Replace(FromFolder, "").TrimStart("\"))
        '        SettingsManager.Instance.Save()
        '    Next
        'End If

        CoreMod.Load(Me)

        RaiseEvent PluginsLoading(Me, New PluginLoadingEventArgs)

        For Each item In Plugins
            item.Load(Me)
        Next

        LoadTypes(CoreMod.GetType.Assembly)
        LoadTypes(Assembly.GetCallingAssembly)

        For Each item In Assemblies
            LoadTypes(item)
        Next
        RaiseEvent PluginLoadComplete(Me, New EventArgs)
    End Sub

#End Region

#Region "Properties"

    Public Property PluginFolder As String

    ''' <summary>
    ''' Gets a list of assemblies that failed to be loaded as plugins, while being registered as such.
    ''' </summary>
    ''' <returns></returns>
    Private Property FailedPluginLoads As List(Of String)

    Private Property MenuItems As List(Of MenuItemInfo)

    ''' <summary>
    ''' Matches opened files to their parent projects
    ''' </summary>
    ''' <returns></returns>
    Private Property OpenedFiles As Dictionary(Of Object, ProjectOld)



    Public Property CurrentSolution As SolutionOld
        Get
            Return _currentSolutoin
        End Get
        Set(value As SolutionOld)
            If _currentSolutoin IsNot Nothing Then _currentSolutoin.Dispose()
            _currentSolutoin = value
            RaiseEvent SolutionChanged(Me, New EventArgs)
        End Set
    End Property
    Private WithEvents _currentSolutoin As SolutionOld

    Public Property CurrentProject As ProjectOld
        Get
            Return _currentProject
        End Get
        Set(value As ProjectOld)
            _currentProject = value
            RaiseEvent CurrentProjectChanged(Me, New EventArgs)
        End Set
    End Property
    Private WithEvents _currentProject As ProjectOld

    Private ReadOnly Property Name As String Implements iNamed.Name
        Get
            Return My.Resources.Language.PluginManager
        End Get
    End Property

#End Region

#Region "Delegates"
    Delegate Sub TypeSearchFound(TypeFound As Type)
#End Region

#Region "Registration"

    ''' <summary>
    ''' Registers a Menu Action for use with creating custom menu items.
    ''' </summary>
    ''' <param name="ActionType">Type of the menu action to be registered.</param>
    Protected Sub RegisterMenuActionType(ActionType As Type)
        If ActionType Is Nothing Then
            Throw New ArgumentNullException(NameOf(ActionType))
        End If
        If Not ReflectionHelpers.IsOfType(ActionType, GetType(MenuAction)) Then
            Throw New ArgumentException("Given type must inherit from MenuAction.", NameOf(ActionType))
        End If

        'While we're registering the type, we need an instance to get extra information, like where to put it
        Dim ActionInstance As MenuAction = ActionType.GetConstructor({}).Invoke({})

        If ActionInstance.DevOnly AndAlso Not SettingsManager.Instance.Settings.DevelopmentMode Then
            'Then this menu item is not supported.
            Exit Sub
        End If

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
                If current.ActionTypes.Count = 0 Then
                    current.SortOrder = Math.Min(current.SortOrder, ActionInstance.SortOrder)
                End If
            Else
                Dim m As New MenuItemInfo
                m.Header = ActionInstance.ActionPath(0)
                m.Children = New List(Of MenuItemInfo)
                m.ActionTypes = New List(Of Type)
                m.SortOrder = ActionInstance.SortOrder
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
                    If current.ActionTypes.Count = 0 Then
                        current.SortOrder = Math.Min(current.SortOrder, ActionInstance.SortOrder)
                    End If
                Else
                    Dim m As New MenuItemInfo
                    m.Header = ActionInstance.ActionPath(count)
                    m.Children = New List(Of MenuItemInfo)
                    m.SortOrder = ActionInstance.SortOrder
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
                    m.SortOrder = ActionInstance.SortOrder
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

    Public Overrides Sub RegisterDefaultFileTypeDetectors()
        RegisterFileTypeDetector(AddressOf Me.DetectFileType)
        RegisterFileTypeDetector(AddressOf Me.TryGetObjectFileType)
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
                out.Add(item.CommandName, item)
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
        Return GetRegisteredObjects(Of iObjectControl)()
    End Function

    ''' <summary>
    ''' Returns the file's parent project, if it exists.
    ''' </summary>
    ''' <param name="File">File of which to get the parent project.  Must be an open file, otherwise the function will return Nothing.</param>
    ''' <returns></returns>
    Public Function GetOpenedFileProject(File As Object) As ProjectOld
        If Me.OpenedFiles.ContainsKey(File) Then
            Return Me.OpenedFiles(File)
        Else
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' Returns a boolean indicating whether or not the given assembly is a plugin assembly that is directly loaded by another plugin assembly.
    ''' </summary>
    ''' <param name="Assembly">Assembly in question</param>
    ''' <returns></returns>
    Public Function IsAssemblyDependant(Assembly As Assembly) As Boolean
        Return DependantPlugins.ContainsKey(Assembly)
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
    Public Event ProjectDirectoryCreated(sender As Object, e As ProjectDirectoryCreatedEventArgs)
    Public Event ProjectModified(sender As Object, e As EventArgs)
    Public Event MenuActionAdded(sender As Object, e As MenuActionAddedEventArgs)
    Public Event SolutionChanged(sender As Object, e As EventArgs)
    Public Event CurrentProjectChanged(sender As Object, e As EventArgs)
    Public Event PluginLoadComplete(sender As Object, e As EventArgs)
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

    Private Sub _pluginHelper_FileOpened(sender As Object, e As EventArguments.FileOpenedEventArguments)
        If Not Me.OpenedFiles.ContainsKey(e.File) Then
            Me.OpenedFiles.Add(e.File, e.ParentProject)
        End If
    End Sub

    Private Sub _pluginHelper_FileClosed(sender As Object, e As EventArguments.FileClosedEventArgs)
        If Me.OpenedFiles.ContainsKey(e.File) Then
            Me.OpenedFiles.Remove(e.File)
        End If
    End Sub

    Private Sub PluginManager_TypeRegistered(sender As Object, e As TypeRegisteredEventArgs) Handles Me.TypeRegistered
        If e.BaseType.IsEquivalentTo(GetType(MenuAction)) Then
            RegisterMenuActionType(e.RegisteredType)
        End If
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
            Dim g As New GenericFile
            g.IsReadOnly = True
            g.OpenFile(Filename)
            Return OpenFile(g)
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
        If type Is Nothing OrElse Not ReflectionHelpers.IsOfType(type, GetType(iOpenableFile)) Then
            'Reopen the file without being readonly
            Dim filename = File.OriginalFilename
            File.Dispose()
            Dim g As New GenericFile
            g.OpenFile(File.OriginalFilename)
            Return g
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
        If type Is Nothing OrElse Not ReflectionHelpers.IsOfType(type, GetType(iOpenableFile)) Then
            'Let's not return nothing.  Maybe something wants to use the directory info.
            Return Directory
        Else
            Dim out As iOpenableFile = type.GetConstructor({}).Invoke({})
            out.OpenFile(Directory.FullName)
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
    Public Function GetObjectControl(ObjectToEdit As Object, RequestedTabTypes As IEnumerable(Of Type)) As iObjectControl
        Dim out As iObjectControl = Nothing
        If ObjectToEdit IsNot Nothing Then
            'Look for a supported Object Control
            For Each item In (From o In GetObjectControls() Order By o.GetSortOrder(ObjectToEdit.GetType, False) Descending)
                'We're only looking for the first non-backup control
                If out Is Nothing OrElse out.IsBackupControl(ObjectToEdit) Then
                    'Check to see if the control supports what we want to edit
                    For Each t In item.GetSupportedTypes
                        If ReflectionHelpers.IsOfType(ObjectToEdit, t) Then

                            'If the control supports our object, we also want to make sure it's supported in the environment.
                            'It must be one of the types in RequestedTabTypes
                            Dim isSupported As Boolean = False
                            For Each r In RequestedTabTypes
                                If ReflectionHelpers.IsOfType(item, r) Then
                                    isSupported = True
                                    Exit For
                                End If
                            Next

                            If isSupported Then
                                out = item '.GetType.GetConstructor({}).Invoke({})
                                Exit For
                            End If
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
    ''' <param name="ObjectToEdit">Object the iObjectControl should edit.</param>
    ''' <param name="RequestedTabTypes">Limits what types of iObjectControl should be returned.  If the iObjectControl is not of any type in this IEnumerable, it will not be used.  If empty or nothing, no constraints will be applied, which is not recommended because the iObjectControl could be made for a different environment (for example, a Windows Forms user control being used in a WPF environment).</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetRefreshedTabs(ObjectToEdit As Object, RequestedTabTypes As IEnumerable(Of Type)) As IEnumerable(Of iObjectControl)
        If ObjectToEdit Is Nothing Then
            Throw New ArgumentNullException(NameOf(ObjectToEdit))
        End If
        Dim objType = ObjectToEdit.GetType
        Dim allTabs As New List(Of iObjectControl)

        'This is our cache of reference-only object controls.
        'We use this to find out which object controls support the given object.
        'It's a static variable because we're likely going to be calling GetRefreshedTabs multiple times,
        'So we'll only have to take a little more time the first time we run this
        Static objControls As List(Of iObjectControl) = Nothing
        If objControls Is Nothing Then
            objControls = GetObjectControls()
        End If

        For Each etab In (From e In objControls Order By e.GetSortOrder(objType, True) Ascending)
            Dim isMatch = False
            'Check to see if the tab itself is supported
            'It must be one of the types in RequestedTabTypes
            For Each t In RequestedTabTypes
                If ReflectionHelpers.IsOfType(etab, t) Then
                    isMatch = True
                    Exit For
                End If
            Next
            'Check to see if the tab support the type of the given object
            Dim supportedTypes = etab.GetSupportedTypes
            If isMatch Then
                isMatch = supportedTypes.Count > 0
            End If
            If isMatch Then
                For Each t In supportedTypes
                    If ObjectToEdit Is Nothing OrElse Not ReflectionHelpers.IsOfType(ObjectToEdit, t) Then
                        isMatch = False
                        Exit For
                    End If
                Next
            End If
            'Check to see if the tab support the object itself
            If isMatch Then
                isMatch = etab.SupportsObject(ObjectToEdit)
            End If
            'This is a supported tab.  We're adding it!
            If isMatch Then
                'etab.EditingObject = ObjectToEdit
                'allTabs.Add(etab)
                'Create another instance of etab, since etab is our cached, search-only instance.
                Dim t As iObjectControl = etab.GetType.GetConstructor({}).Invoke({})
                t.EditingObject = ObjectToEdit
                allTabs.Add(t)
            End If
        Next

        Dim backupTabs As New List(Of iObjectControl)
        Dim notBackup As New List(Of iObjectControl)

        'Sort the backup vs non-backup tabs
        For Each item In allTabs
            If item.IsBackupControl(ObjectToEdit) Then
                backupTabs.Add(item)
            Else
                notBackup.Add(item)
            End If
        Next

        'And use the non-backup ones if available
        If notBackup.Count > 0 Then
            Return notBackup
        Else
            Dim toUse = (From b In backupTabs Order By b.GetSortOrder(objType, True)).FirstOrDefault
            If toUse Is Nothing Then
                Return {}
            Else
                Return {toUse}
            End If
        End If
    End Function

    Public Function GetFileType(File As GenericFile) As TypeInfo
        Dim matches As New List(Of TypeInfo)
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

    Public Function GetDirectoryType(Directory As IO.DirectoryInfo) As TypeInfo
        Dim matches As New List(Of TypeInfo)
        For Each item In DirectoryTypeDetectors
            Dim t = item.Invoke(Directory.FullName)
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

    Public Function DetectFileType(File As GenericFile) As IEnumerable(Of TypeInfo)
        Dim matches As New List(Of TypeInfo)

        If ExecutableFile.IsExeFile(File.OriginalFilename) Then
            matches.Add(GetType(ExecutableFile))
        End If

        If matches.Count = 0 Then
            For Each item In GetDetectableFileTypes()
                Dim instance As iDetectableFileType = item.GetConstructor({})?.Invoke({})
                If instance IsNot Nothing AndAlso instance.IsOfType(File) Then
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
    Public Shared Function TryGetObjectFileType(Filename As String) As TypeInfo
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

                For Each item In Plugins
                    item.UnLoad(Me)
                Next
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
