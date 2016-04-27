Imports System.Reflection

Namespace Extensions.Plugins
    Public MustInherit Class PluginManager

        Protected Sub New()
            Me.DirectoryTypeDetectors = New List(Of DirectoryTypeDetector)
            Me.TypeRegistery = New Dictionary(Of TypeInfo, List(Of TypeInfo))
        End Sub
        Protected Property TypeRegistery As Dictionary(Of TypeInfo, List(Of TypeInfo))
        Public Property CoreAssemblyName As String 'Todo: make readonly to the public
        Protected Property FileTypeDetectors As New List(Of FileTypeDetector)
        Protected Property DirectoryTypeDetectors As New List(Of DirectoryTypeDetector)

        ''' <summary>
        ''' Dictionary of (Extension, Friendly Name) used in the Open and Save file dialogs.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IOFilters As New Dictionary(Of String, String)

#Region "Delegates"
        ''' <summary>
        ''' A function that can determine what Type can model the given file.
        ''' </summary>
        ''' <param name="File"></param>
        ''' <returns></returns>
        Delegate Function FileTypeDetector(File As SkyEditor.Core.GenericFile) As IEnumerable(Of TypeInfo)
        Delegate Function DirectoryTypeDetector(Directory As String) As IEnumerable(Of TypeInfo)
#End Region

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
        ''' Loads all available plugins using the given CoreMod.
        ''' </summary>
        ''' <param name="CoreMod"></param>
        Public MustOverride Sub LoadPlugins(CoreMod As ISkyEditorPlugin)

        ''' <summary>
        ''' Loads the given plugin.
        ''' </summary>
        ''' <param name="Plugin"></param>
        Public MustOverride Sub LoadPlugin(Plugin As ISkyEditorPlugin)

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
        ''' Returns a list of the paths to the supported plugins in PluginPaths.
        ''' </summary>
        ''' <param name="PluginPaths">Full paths of the plugin assemblies to analyse.</param>
        ''' <param name="CoreAssemblyName">Name of the core assembly, usually the Entry assembly.  Assemblies with this name are not supported, to avoid loading duplicates.</param>
        ''' <returns>The full paths of the supported plugins.</returns>
        Public MustOverride Function GetSupportedPlugins(PluginPaths As IEnumerable(Of String), Optional CoreAssemblyName As String = Nothing) As List(Of String)
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

        ''' <summary>
        ''' Registers the given delegate function so that it can be used to detect file types.
        ''' </summary>
        ''' <param name="Detector"></param>
        Public Sub RegisterFileTypeDetector(Detector As FileTypeDetector)
            If Not FileTypeDetectors.Contains(Detector) Then
                FileTypeDetectors.Add(Detector)
            End If
        End Sub

        ''' <summary>
        ''' Registers SkyEditorBase's file type detectors.
        ''' </summary>
        Public MustOverride Sub RegisterDefaultFileTypeDetectors()

        Public Sub RegisterDirectoryTypeDetector(Detector As DirectoryTypeDetector)
            DirectoryTypeDetectors.Add(Detector)
        End Sub
#End Region


    End Class

End Namespace
