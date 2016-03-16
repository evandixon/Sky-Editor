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
        Private Property Filename As String
        Public Function GetFilename() As String
            Return Filename
        End Function
        Public Sub Save(Filename As String)
            Utilities.Json.SerializeToFile(Filename, Me)
        End Sub
        Public Shared Function Open(Filename) As ExtensionInfo
            Dim out = Utilities.Json.DeserializeFromFile(Of ExtensionInfo)(Filename)
            out.Filename = Filename
            Return out
        End Function
        Public Sub New()
            ID = Guid.NewGuid.ToString
            ExtensionFiles = New List(Of String)
        End Sub
    End Class

End Namespace
