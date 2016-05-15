Imports System.Reflection
Imports SkyEditor.Core
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Utilities

Public Class Library
    ''' <summary>
    ''' Represents the serialized form of a Library.
    ''' </summary>
    Protected Class LibraryFile
        Public Property ID As Guid
        Public Property Name As String
        Public Property RelativePath As String
        Public Property LibraryTypeName As String
        Public Property ExtraDataJson As String
        Public Property ExtraDataTypeName As String
    End Class

    ''' <summary>
    ''' Opens a library using the serialized data.
    ''' </summary>
    ''' <param name="serializedLibrary">Serialized data created from Library.Save</param>
    ''' <param name="rootPath">Root path of the library.  The library itself may look at a child directory.</param>
    ''' <param name="manager">Instance of the current plugin manager.</param>
    ''' <returns></returns>
    Public Shared Async Function OpenLibrary(serializedLibrary As String, rootPath As String, manager As PluginManager) As Task(Of Library)
        'Open the file
        Dim file = Json.Deserialize(Of LibraryFile)(serializedLibrary)

        'Get the type
        Dim libraryType = ReflectionHelpers.GetTypeByName(file.LibraryTypeName, manager)

        'Default to Library if the type isn't available
        If libraryType Is Nothing Then
            libraryType = GetType(Library).GetTypeInfo
        End If

        'Create an instance of the library
        Dim instance As Library = ReflectionHelpers.CreateInstance(libraryType)

        instance.ID = file.ID
        instance.Name = file.Name
        instance.RelativePath = file.RelativePath
        instance.RootPath = rootPath

        'Parse the extra data
        Dim extraData As Object = Nothing
        If Not String.IsNullOrEmpty(file.ExtraDataJson) AndAlso Not String.IsNullOrEmpty(file.ExtraDataTypeName) Then
            Dim extraDataType = ReflectionHelpers.GetTypeByName(file.ExtraDataTypeName, manager)
            extraData = Json.Deserialize(extraDataType.AsType, file.ExtraDataJson)
        End If

        'Initialize the library instance using the extra data
        Await instance.Load(extraData, manager)

        Return instance
    End Function

    Public Sub New(name As String, relativePath As String, rootPath As String)
        Me.id = Guid.NewGuid
        Me.Name = name
        Me.RelativePath = relativePath
        Me.RootPath = rootPath
    End Sub
    Public Sub New()

    End Sub

    Public Property ID As Guid
    Public Property Name As String
    Public Property RelativePath As String
    Protected Property RootPath As String

    ''' <summary>
    ''' Initializes the library.
    ''' </summary>
    ''' <param name="extraData">Extra data object stored in the library file.</param>
    ''' <param name="manager">Instance of the current plugin manager.</param>
    ''' <returns></returns>
    Protected Overridable Function Load(extraData As Object, manager As PluginManager) As Task
        Return Task.FromResult(0)
    End Function

    ''' <summary>
    ''' Gets a list of the logical contents of the library.
    ''' </summary>
    ''' <returns></returns>
    Public Overridable Function GetContents(manager As PluginManager) As Task(Of IEnumerable(Of Object))
        Return Task.FromResult(DirectCast({}, IEnumerable(Of Object)))
    End Function

    ''' <summary>
    ''' Gets a list of the supported content types for AddContent.
    ''' </summary>
    ''' <returns></returns>
    Public Overridable Function GetSupportedContentTypes() As IEnumerable(Of Type)
        Return New List(Of Type)
    End Function

    ''' <summary>
    ''' Adds a content item to the library.
    ''' </summary>
    ''' <param name="content">Item to add.</param>
    ''' <exception cref="InvalidDataException">Thrown when the type of content is not supported.</exception>
    ''' <returns></returns>
    Public Overridable Function AddContent(content As Object, manager As PluginManager) As Task
        If GetSupportedContentTypes.Contains(content.GetType) Then
            'Do nothing, let child classes process things.
            Return Task.FromResult(0)
        Else
            Throw New NotImplementedException
        End If
    End Function

    ''' <summary>
    ''' Gets the serializable object storing extra data to be used on load.
    ''' Returns null if the library type does not have extra data.
    ''' </summary>
    ''' <returns></returns>
    Protected Overridable Function GetExtraData() As Task(Of Object)
        Return Task.FromResult(Of Object)(Nothing)
    End Function

    Public Async Function Save() As Task(Of String)
        Dim file As New LibraryFile
        file.ID = Me.ID
        file.Name = Me.Name
        file.RelativePath = Me.RelativePath
        file.LibraryTypeName = Me.GetType.AssemblyQualifiedName

        Dim extraData = Await GetExtraData()
        file.ExtraDataJson = Json.Serialize(extraData)
        file.ExtraDataTypeName = extraData?.GetType.AssemblyQualifiedName

        Return Json.Serialize(file)
    End Function

End Class
