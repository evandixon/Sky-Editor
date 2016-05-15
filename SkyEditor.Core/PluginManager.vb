Imports System.Reflection
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI
Imports SkyEditor.Core.Utilities

Public Class PluginManager

#Region "Constructors"
    Protected Sub New()
        Me.TypeRegistery = New Dictionary(Of TypeInfo, List(Of TypeInfo))
    End Sub
#End Region

    Protected Property TypeRegistery As Dictionary(Of TypeInfo, List(Of TypeInfo))
    Public Property CoreAssemblyName As String 'Todo: make readonly to the public

    ''' <summary>
    ''' Matches plugin assemblies (key) to assemblies that depend on that assembly (value).
    ''' If an assembly is a key, it is manually loaded by each of the assemblies in the value.
    ''' </summary>
    ''' <returns></returns>
    Protected Property DependantPlugins As Dictionary(Of Assembly, List(Of Assembly))

    ''' <summary>
    ''' Dictionary of (Extension, Friendly Name) used in the Open and Save file dialogs.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IOFilters As New Dictionary(Of String, String)

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

#Region "Events"
    ''' <summary>
    ''' Raised when a type is added into the type registry.
    ''' </summary>
    ''' <param name="sender">Instance of the PluginManager</param>
    ''' <param name="e"></param>
    Public Event TypeRegistered(sender As Object, e As TypeRegisteredEventArgs)
#End Region

#Region "Plugin Loading"

    ''' <summary>
    ''' Loads the given Core plugin, and any other available plugins, if supported by the platform.
    ''' </summary>
    ''' <param name="Core">Core to load</param>
    Public Overridable Sub LoadCore(Core As CoreSkyEditorPlugin)
        'Load the provided core
        CurrentIOProvider = Core.GetIOProvider
        Core.Load(Me)

        'Load type registers
        RegisterTypeRegister(GetType(IOpenableFile).GetTypeInfo)
        RegisterTypeRegister(GetType(IDetectableFileType).GetTypeInfo)
        RegisterTypeRegister(GetType(IDirectoryTypeDetector).GetTypeInfo)
        RegisterTypeRegister(GetType(IFileTypeDetector).GetTypeInfo)
        RegisterTypeRegister(GetType(MenuAction).GetTypeInfo)

        'Load types
        RegisterType(GetType(IFileTypeDetector).GetTypeInfo, GetType(DetectableFileTypeDetector).GetTypeInfo)
    End Sub

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


        If Not ReflectionHelpers.HasDefaultConstructor(Type) Then
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
        If Not TempIOFilters.ContainsKey(FileExtension) Then
            TempIOFilters.Add(FileExtension, FileFormatName)
        End If
        IOFilters = TempIOFilters
    End Sub

#End Region

#Region "Functions"
#Region "Read Type Registry"
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
    ''' Returns an IEnumerable of new instances of all the registered types that inherit or implement the given BaseType.
    ''' </summary>
    ''' <param name="BaseType">Type to get children or implementors of.</param>
    ''' <returns></returns>
    Public Function GetRegisteredObjects(BaseType As TypeInfo) As IEnumerable(Of Object)
        Dim output As New List(Of Object)

        For Each item In GetRegisteredTypes(BaseType)
            If ReflectionHelpers.HasDefaultConstructor(item) AndAlso Not item.IsGenericType Then
                output.Add(ReflectionHelpers.CreateInstance(item))
            End If
        Next

        Return output
    End Function

    ''' <summary>
    ''' eturns an IEnumerable of new instances of all the registered types that inherit or implement the given type.
    ''' </summary>
    ''' <typeparam name="T">Type to get children or implementors of.</typeparam>
    ''' <returns></returns>
    Public Function GetRegisteredObjects(Of T)() As IEnumerable(Of T)
        Dim output As New List(Of T)
        Dim targetType = GetType(T).GetTypeInfo

        For Each item In GetRegisteredTypes(targetType)
            If ReflectionHelpers.HasDefaultConstructor(item) AndAlso Not item.IsGenericType Then
                output.Add(ReflectionHelpers.CreateInstance(item))
            End If
        Next

        Return output
    End Function
#End Region

#End Region

End Class