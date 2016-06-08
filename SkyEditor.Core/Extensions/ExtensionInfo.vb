Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Utilities

Namespace Extensions
    ''' <summary>
    ''' Contains the metadata of an extension
    ''' </summary>
    Public Class ExtensionInfo
        Public Property ExtensionTypeName As String
        ''' <summary>
        ''' Unique ID of the extension
        ''' </summary>
        ''' <returns></returns>
        Public Property ID As Guid
        Public Property Name As String
        Public Property Description As String
        Public Property Author As String
        Public Property Version As String
        Public Property IsEnabled As Boolean
        Public Property ExtensionFiles As List(Of String)
        Public Property IsInstalled As Boolean
        Private Property Filename As String
        Public Function GetFilename() As String
            Return Filename
        End Function
        Public Sub Save(filename As String, provider As IOProvider)
            Json.SerializeToFile(filename, Me, provider)
        End Sub
        Public Shared Function Deserialize(serialized As String) As ExtensionInfo
            Return Json.Deserialize(Of ExtensionInfo)(serialized)
        End Function
        Public Shared Function OpenFromFile(filename As String, provider As IOProvider) As ExtensionInfo
            Dim out = Json.DeserializeFromFile(Of ExtensionInfo)(filename, provider)
            out.Filename = filename
            Return out
        End Function
        Public Sub New()
            ID = Guid.NewGuid
            ExtensionFiles = New List(Of String)
            Name = ""
            Description = ""
            Author = ""
            Version = ""
            IsEnabled = True
        End Sub
    End Class

End Namespace
