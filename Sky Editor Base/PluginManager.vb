Imports System.Reflection
Imports System.Text
Imports System.Threading.Tasks
Imports SkyEditor.Core.Interfaces
Imports SkyEditorBase.EventArguments
Imports SkyEditorBase.Interfaces
Imports SkyEditor.Core.UI
Imports SkyEditor.Core.Utilities
Imports SkyEditor.Core
Imports SkyEditor.Core.IO
Imports System.IO

Public Class PluginManager
    Inherits SkyEditor.Core.PluginManager
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
    Public Function GetSupportedPlugins(PluginPaths As IEnumerable(Of String), Optional CoreAssemblyName As String = Nothing) As List(Of String)
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
                                                                  If ReflectionHelpers.IsOfType(t, GetType(SkyEditorPlugin).GetTypeInfo) Then
                                                                      out.Add(t.FullName)
                                                                  End If
                                                              Next
                                                          End If

                                                          Return out
                                                      End Function, CoreAssemblyName)
                Catch ex As Reflection.ReflectionTypeLoadException
                    'If we fail here, then the assembly is NOT a valid plugin, so we won't load it.
                    Console.WriteLine(ex.ToString)
                Catch ex As FileNotFoundException
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
    ''' <param name="Core"></param>
    Public Overrides Sub LoadCore(Core As CoreSkyEditorPlugin)
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
        CoreAssemblyName = Core.GetType.Assembly.FullName

        Dim supportedPlugins As New List(Of String)

        'Look at the plugin extensions to find plugins.
        Dim pluginExtType As New Extensions.PluginExtensionType
        For Each item In pluginExtType.GetInstalledExtensions
            Dim extAssemblies As New List(Of String)
            For Each file In item.ExtensionFiles
                extAssemblies.Add(Path.Combine(pluginExtType.GetExtensionDirectory(item), file))
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

        MyBase.LoadCore(Core)

        RaiseEvent PluginsLoading(Me, New PluginLoadingEventArgs)

        For Each item In Plugins
            item.Load(Me)
        Next

        LoadTypes(Core.GetType.Assembly)
        LoadTypes(Assembly.GetCallingAssembly)

        For Each item In Assemblies
            LoadTypes(item)
        Next
        RaiseEvent PluginLoadComplete(Me, New EventArgs)
    End Sub

#End Region

#Region "Properties"

    ''' <summary>
    ''' Gets a list of assemblies that failed to be loaded as plugins, while being registered as such.
    ''' </summary>
    ''' <returns></returns>
    Private Property FailedPluginLoads As List(Of String)

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

#End Region

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
