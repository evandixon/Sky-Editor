Namespace Flashcart
    Public Class FlashcartInfoFile
        Public Property FlashcartAssemblyQualifiedTypeName As String
        Public Property Name As String
        Public Property ID As String
        Public Property Collections As List(Of String)

        Public Sub Save(Filename As String)
            SkyEditorBase.Utilities.Json.SerializeToFile(Filename, Me)
        End Sub
        Public Shared Function Open(Filename As String) As FlashcartInfoFile
            Return SkyEditorBase.Utilities.Json.DeserializeFromFile(Of FlashcartInfoFile)(Filename)
        End Function
        Public Sub New()
            Collections = New List(Of String)
        End Sub
    End Class
End Namespace