Imports SkyEditor.Core.Utilities

Namespace Extensions
    Public Class ExtensionInfo
        Public Property ExtensionTypeName As String
        ''' <summary>
        ''' Unique ID of the extension
        ''' </summary>
        ''' <returns></returns>
        Public Property ID As String
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
        Public Sub Save(Filename As String)
            Json.SerializeToFile(Filename, Me, New SkyEditor.Core.Windows.IOProvider)
        End Sub
        Public Shared Function Open(Filename) As ExtensionInfo
            Dim out = Json.DeserializeFromFile(Of ExtensionInfo)(Filename, New SkyEditor.Core.Windows.IOProvider)
            out.Filename = Filename
            Return out
        End Function
        Public Sub New()
            ID = Guid.NewGuid.ToString
            ExtensionFiles = New List(Of String)
            Name = ""
            Description = ""
            Author = ""
            Version = ""
            IsEnabled = True
        End Sub
    End Class

End Namespace
