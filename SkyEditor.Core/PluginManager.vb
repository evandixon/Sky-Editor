Imports System.Reflection
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI
Imports SkyEditor.Core.Utilities
Imports SkyEditor.Core.Settings
Imports SkyEditor.Core.Extensions
Imports SkyEditor.Core.ConsoleCommands

Public Class PluginManager
    Implements IDisposable

#Region "Constructors"
    Public Sub New()
        Me.TypeRegistery = New Dictionary(Of TypeInfo, List(Of TypeInfo))
        Me.TypeInstances = New Dictionary(Of TypeInfo, Object)
        Me.FailedPluginLoads = New List(Of String)
        Me.Assemblies = New List(Of Assembly)
        Me.DependantPlugins = New Dictionary(Of Assembly, List(Of Assembly))
    End Sub
#End Region

#Region "Properties"

    ''' <summary>
    ''' Caches instances of types, so they are not constantly recreated to read metadata (such supported types on object controls)
    ''' </summary>
    ''' <returns></returns>
    Protected Property TypeInstances As Dictionary(Of TypeInfo, Object)

    ''' <summary>
    ''' The core of the plugin manager: matches base types or interfaces to types that inherit or implement these
    ''' </summary>
    ''' <returns></returns>
    Protected Property TypeRegistery As Dictionary(Of TypeInfo, List(Of TypeInfo))
    Protected Property CoreModAssembly As Assembly
    Public Property ExtensionDirectory As String

    ''' <summary>
    ''' Matches plugin assemblies (key) to assemblies that depend on that assembly (value).
    ''' If an assembly is a key, it is manually loaded by each of the assemblies in the value.
    ''' </summary>
    ''' <returns></returns>
    Protected Property DependantPlugins As Dictionary(Of Assembly, List(Of Assembly))

    ''' <summary>
    ''' Contains the assemblies that contain plugin information.
    ''' </summary>
    ''' <returns></returns>
    Public Property Assemblies As List(Of Assembly)

    ''' <summary>
    ''' List of all loaded iSkyEditorPlugins that are loaded.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Plugins As New List(Of SkyEditorPlugin)

    ''' <summary>
    ''' Gets a list of assemblies that failed to be loaded as plugins, while being registered as such.
    ''' </summary>
    ''' <returns></returns>
    Protected Property FailedPluginLoads As List(Of String)

    ''' <summary>
    ''' The current IO Provider for the application.
    ''' </summary>
    ''' <returns></returns>
    Public Property CurrentIOProvider As IOProvider
        Get
            Return _currentIOProvider
        End Get
        Protected Set(value As IOProvider)
            _currentIOProvider = value
        End Set
    End Property
    Dim _currentIOProvider As IOProvider

    ''' <summary>
    ''' The current Settings Provider for the applicaiton.
    ''' </summary>
    ''' <returns></returns>
    Public Property CurrentSettingsProvider As ISettingsProvider
        Get
            Return _currentSettingsProvider
        End Get
        Protected Set(value As ISettingsProvider)
            _currentSettingsProvider = value
        End Set
    End Property
    Dim _currentSettingsProvider As ISettingsProvider

    ''' <summary>
    ''' The current Console Provider for the application.
    ''' </summary>
    ''' <returns></returns>
    Public Property CurrentConsoleProvider As IConsoleProvider
        Get
            Return _currentConsoleProvider
        End Get
        Protected Set(value As IConsoleProvider)
            _currentConsoleProvider = value
        End Set
    End Property
    Dim _currentConsoleProvider As IConsoleProvider

    ''' <summary>
    ''' The current instance of the IO/UI Manager, helping manage open files and their associated UI.
    ''' </summary>
    ''' <returns></returns>
    Public Property CurrentIOUIManager As IOUIManager
        Get
            Return _currentIOmanager
        End Get
        Protected Set(value As IOUIManager)
            _currentIOmanager = value
        End Set
    End Property
    Dim _currentIOmanager As IOUIManager

#End Region

#Region "Events"
    ''' <summary>
    ''' Raised when a type is added into the type registry.
    ''' </summary>
    ''' <param name="sender">Instance of the PluginManager</param>
    Public Event TypeRegistered(sender As Object, e As TypeRegisteredEventArgs)

    ''' <summary>
    ''' Raised before plugins' Load methods are called.
    ''' </summary>
    Public Event PluginsLoading(sender As Object, e As EventArgs)

    ''' <summary>
    ''' Raised after all plugins have been loaded.
    ''' </summary>
    Public Event PluginLoadComplete(sender As Object, e As EventArgs)
#End Region

#Region "Plugin Loading"

    ''' <summary>
    ''' Loads the given Core plugin, and any other available plugins, if supported by the platform.
    ''' </summary>
    ''' <param name="Core">Core to load</param>
    Public Overridable Async Function LoadCore(Core As CoreSkyEditorPlugin) As Task
        'Load providers
        CurrentIOProvider = Core.GetIOProvider
        CurrentSettingsProvider = Core.GetSettingsProvider(Me)
        CurrentConsoleProvider = Core.GetConsoleProvider
        CurrentIOUIManager = Core.GetIOUIManager(Me)

        'Delete files and directories scheduled for deletion
        Dim deleteTasks As New List(Of Task)
        '-Files
        For Each item In CurrentSettingsProvider.GetFilesScheduledForDeletion
            'This doesn't really need to be asynchronous, but since the directory deletes are async, why not?
            deleteTasks.Add(Task.Run(New Action(Sub()
                                                    FileSystem.DeleteFile(item, CurrentIOProvider)
                                                    CurrentSettingsProvider.UncheduleFileForDeletion(item)
                                                End Sub)))
        Next
        '-Directories
        For Each item In CurrentSettingsProvider.GetDirectoriesScheduledForDeletion
            deleteTasks.Add(Task.Run(Async Function() As Task
                                         Await FileSystem.DeleteDirectory(item, CurrentIOProvider)
                                         CurrentSettingsProvider.UncheduleDirectoryForDeletion(item)
                                     End Function))
        Next

        Await Task.WhenAll(deleteTasks)

        'Load the provided core
        Me.CoreModAssembly = Core.GetType.GetTypeInfo.Assembly
        Core.Load(Me)

        ExtensionDirectory = Core.GetExtensionDirectory

        'Load type registers
        RegisterTypeRegister(GetType(ExtensionType).GetTypeInfo)
        RegisterTypeRegister(GetType(Solution).GetTypeInfo)
        RegisterTypeRegister(GetType(Project).GetTypeInfo)
        RegisterTypeRegister(GetType(ICreatableFile).GetTypeInfo)
        RegisterTypeRegister(GetType(IOpenableFile).GetTypeInfo)
        RegisterTypeRegister(GetType(IDetectableFileType).GetTypeInfo)
        RegisterTypeRegister(GetType(IDirectoryTypeDetector).GetTypeInfo)
        RegisterTypeRegister(GetType(IFileTypeDetector).GetTypeInfo)
        RegisterTypeRegister(GetType(MenuAction).GetTypeInfo)
        RegisterTypeRegister(GetType(AnchorableViewModel).GetTypeInfo)


        'Load types
        RegisterType(GetType(IFileTypeDetector).GetTypeInfo, GetType(DetectableFileTypeDetector).GetTypeInfo)
        RegisterType(GetType(IFileTypeDetector).GetTypeInfo, GetType(ObjectFileDetector).GetTypeInfo)
        RegisterType(GetType(ExtensionType).GetTypeInfo, GetType(PluginExtensionType).GetTypeInfo)
        RegisterType(GetType(Solution).GetTypeInfo, GetType(Solution).GetTypeInfo)
        RegisterType(GetType(Project).GetTypeInfo, GetType(Project).GetTypeInfo)

        'Load plugins, if enabled
        Dim enablePluginLoading = Core.IsPluginLoadingEnabled
        If enablePluginLoading Then
            'Get the paths of all plugins to be loaded
            Dim supportedPlugins = GetPluginPaths(ExtensionDirectory)

            'Load the plugin assemblies
            For Each item In supportedPlugins
                Try
                    Dim assemblyActual = Core.LoadAssembly(item)
                    If assemblyActual IsNot Nothing Then
                        Assemblies.Add(assemblyActual)
                        For Each plg In From t In assemblyActual.DefinedTypes Where ReflectionHelpers.IsOfType(t, GetType(SkyEditorPlugin).GetTypeInfo) AndAlso ReflectionHelpers.CanCreateInstance(t)
                            Plugins.Add(ReflectionHelpers.CreateInstance(plg))
                        Next
                    End If

                Catch ex As BadImageFormatException
                    'The assembly we just tried to load is a bad assembly.  We can continue, but not with this assembly.
                    FailedPluginLoads.Add(item)
                Catch ex As NotSupportedException
                    'The current platform mod does not support loading assemblies this way.
                    'Abort dynamic assembly loading.
                    enablePluginLoading = False
                    Exit For
                End Try
            Next
        End If

        'Check again to see if plugin loading is enabled.
        'It's possible the core mod does not support this, yet still enabled it anyway.
        If enablePluginLoading Then

            'Load the logical plugins
            RaiseEvent PluginsLoading(Me, New EventArgs)

            For Each item In Plugins
                item.Load(Me)
            Next

            'Use reflection to fill the type registry
            LoadTypes(Core.GetType.GetTypeInfo.Assembly)
            For Each item In Assemblies
                LoadTypes(item)
            Next

            RaiseEvent PluginLoadComplete(Me, New EventArgs)
        End If

    End Function

    ''' <summary>
    ''' Gets the paths of available plugin assemblies.
    ''' </summary>
    ''' <param name="extensionDirectory">Directory in which extensions are stored.</param>
    ''' <returns></returns>
    Protected Function GetPluginPaths(extensionDirectory As String) As List(Of String)
        'Start loading plugins
        Dim supportedPlugins As New List(Of String)

        'Look at the plugin extensions to find plugins.
        Dim pluginExtType As New PluginExtensionType
        pluginExtType.CurrentPluginManager = Me
        pluginExtType.RootExtensionDirectory = extensionDirectory
        For Each item In pluginExtType.GetInstalledExtensions(Me)
            Dim extAssemblies As New List(Of String)
            For Each file In item.ExtensionFiles
                extAssemblies.Add(Path.Combine(pluginExtType.GetExtensionDirectory(item.ID), file))
            Next
            ''Todo: somehow verify the assemblies.
            ''It's probably OK to not do so, relying only on the extension's manifest of plugin entrypoints, but it would be better to check them.
            'supportedPlugins.AddRange(GetSupportedPlugins(extAssemblies, CoreAssemblyName))
            supportedPlugins.AddRange(extAssemblies)
        Next
        Return supportedPlugins
    End Function

    ''' <summary>
    ''' Loads a plugin that's referenced by another.
    ''' </summary>
    ''' <param name="targetPlugin">The plugin to load.</param>
    ''' <param name="dependantPlugin">The plugin that requires the load.</param>
    Public Overridable Sub LoadRequiredPlugin(targetPlugin As SkyEditorPlugin, dependantPlugin As SkyEditorPlugin)
        Dim pluginType = targetPlugin.GetType
        Dim pluginTypeInfo = pluginType.GetTypeInfo
        Dim pluginAssembly = pluginTypeInfo.Assembly

        For Each item In Plugins
            If item.GetType.Equals(pluginType) Then
                'Then we already have this plugin loaded and should do nothing
            Else
                targetPlugin.Load(Me)

                If Not Assemblies.Contains(pluginAssembly) Then
                    Assemblies.Add(pluginAssembly)
                End If
            End If
        Next

        'Mark this plugin as a dependant
        If Not DependantPlugins.ContainsKey(pluginAssembly) Then
            DependantPlugins.Add(pluginAssembly, New List(Of Assembly))
        End If
        Dim caller = dependantPlugin.GetType.GetTypeInfo.Assembly
        If Not DependantPlugins(pluginAssembly).Contains(caller) Then
            DependantPlugins(pluginAssembly).Add(caller)
        End If
    End Sub

    ''' <summary>
    ''' Looks at the given assembly and loads supported types into the type registry.
    ''' </summary>
    ''' <param name="Item"></param>
    Protected Overridable Sub LoadTypes(Item As Assembly)
        'Load types
        For Each actualType In Item.DefinedTypes
            'Check to see if this type inherits from one we're looking for
            For Each registeredType In TypeRegistery.Keys
                If ReflectionHelpers.IsOfType(actualType, registeredType, True) Then
                    RegisterType(registeredType, actualType)
                End If
            Next

            'Do the same for each interface
            For Each i In actualType.ImplementedInterfaces
                For Each registeredType In TypeRegistery.Keys
                    If ReflectionHelpers.IsOfType(i, registeredType, True) Then
                        RegisterType(registeredType, actualType)
                    End If
                Next
            Next
        Next
    End Sub

    ''' <summary>
    ''' Returns a boolean indicating whether or not the given assembly is a plugin assembly that is directly loaded by another plugin assembly.
    ''' </summary>
    ''' <param name="Assembly">Assembly in question</param>
    ''' <returns></returns>
    Public Function IsAssemblyDependant(Assembly As Assembly) As Boolean
        Return DependantPlugins.ContainsKey(Assembly)
    End Function

#End Region

#Region "Registration"
    ''' <summary>
    ''' Adds the given type to the type registry.
    ''' After plugins are loaded, any type that inherits or implements the given Type can be easily found.
    ''' 
    ''' If the type is already in the type registry, nothing will be done.
    ''' </summary>
    ''' <param name="Type"></param>
    Public Sub RegisterTypeRegister(Type As TypeInfo)
        If Type Is Nothing Then
            Throw New ArgumentNullException(NameOf(Type))
        End If

        If Not TypeRegistery.ContainsKey(Type) Then
            TypeRegistery.Add(Type, New List(Of TypeInfo))
        End If
    End Sub


    ''' <summary>
    ''' Registers the given Type in the type registry.
    ''' </summary>
    ''' <param name="Register">The base type or interface that the given Type inherits or implements.</param>
    ''' <param name="Type">The type to register.</param>
    Public Sub RegisterType(Register As TypeInfo, Type As TypeInfo)
        If Register Is Nothing Then
            Throw New ArgumentNullException(NameOf(Register))
        End If
        If Type Is Nothing Then
            Throw New ArgumentNullException(NameOf(Type))
        End If
        Dim x = From c In Type.DeclaredConstructors Where c.GetParameters.Length = 1


        If Not ReflectionHelpers.CanCreateInstance(Type) Then
            'We only want types with default constructors.
            'This also helps weed out Generic Types, MustInherit Classes, and Interfaces.
            Exit Sub
        End If

        'Ensure that TypeRegistry contains the key.
        RegisterTypeRegister(Register)

        'Duplicates make can cause minor issues
        If Not TypeRegistery(Register).Contains(Type) Then
            TypeRegistery(Register).Add(Type)
        End If

        RaiseEvent TypeRegistered(Me, New TypeRegisteredEventArgs With {.BaseType = Register, .RegisteredType = Type})
    End Sub

#End Region

#Region "Functions"
#Region "Read Type Registry"
    Protected Function GetCachedInstance(type As TypeInfo) As Object
        If TypeInstances.ContainsKey(type) Then
            Return TypeInstances(type)
        Else
            Dim instance = ReflectionHelpers.CreateInstance(type)
            TypeInstances.Add(type, instance)
            Return instance
        End If
    End Function
    ''' <summary>
    ''' Returns an IEnumerable of all the registered types that inherit or implement the given BaseType.
    ''' </summary>
    ''' <param name="BaseType">Type to get children or implementors of.</param>
    ''' <returns></returns>
    Public Function GetRegisteredTypes(BaseType As TypeInfo) As IEnumerable(Of TypeInfo)
        If BaseType Is Nothing Then
            Throw New ArgumentNullException(NameOf(BaseType))
        End If

        If TypeRegistery.ContainsKey(BaseType) Then
            Return TypeRegistery(BaseType)
        Else
            Return {}
        End If
    End Function

    Public Function GetRegisteredTypes(Of T)() As IEnumerable(Of TypeInfo)
        Return GetRegisteredTypes(GetType(T).GetTypeInfo)
    End Function

    ''' <summary>
    ''' Returns an IEnumerable of instances of all the registered types that inherit or implement the given type.
    ''' These instances are not new instances, so create new ones if needed.
    ''' </summary>
    ''' <param name="BaseType">Type to get children or implementors of.</param>
    ''' <returns></returns>
    Public Function GetRegisteredObjects(BaseType As TypeInfo) As IEnumerable(Of Object)
        Dim output As New List(Of Object)

        For Each item In GetRegisteredTypes(BaseType)
            If ReflectionHelpers.CanCreateInstance(item) AndAlso Not item.IsGenericType Then
                output.Add(GetCachedInstance(item))
            End If
        Next

        Return output
    End Function

    ''' <summary>
    ''' Returns an IEnumerable of instances of all the registered types that inherit or implement the given type.
    ''' These instances are not new instances, so create new ones if needed.
    ''' </summary>
    ''' <typeparam name="T">Type to get children or implementors of.</typeparam>
    ''' <returns></returns>
    Public Function GetRegisteredObjects(Of T)() As IEnumerable(Of T)
        Dim out As New List(Of T)
        For Each item In GetRegisteredObjects(GetType(T).GetTypeInfo)
            out.Add(item)
        Next
        Return out
    End Function
#End Region

    Public Function GetLoadedAssemblies() As List(Of Assembly)
        Dim out As New List(Of Assembly)

        For Each item In Me.Assemblies
            out.Add(item)
        Next

        Dim currentAssembly As Assembly = GetType(PluginManager).GetTypeInfo.Assembly

        If Not out.Contains(currentAssembly) Then
            out.Add(currentAssembly)
        End If

        If Not out.Contains(CoreModAssembly) Then
            out.Add(CoreModAssembly)
        End If

        Return out
    End Function

#End Region

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).

                For Each item In Plugins
                    item.UnLoad(Me)
                Next

                For Each item In TypeInstances
                    If item.Value IsNot Nothing AndAlso TypeOf item.Value Is IDisposable Then
                        DirectCast(item.Value, IDisposable).Dispose()
                    End If
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