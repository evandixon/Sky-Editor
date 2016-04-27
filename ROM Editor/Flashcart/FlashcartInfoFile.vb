Imports SkyEditor.Core.Utilities

Namespace Flashcart
    Public Class FlashcartInfoFile
        Public Property FlashcartAssemblyQualifiedTypeName As String
        Public Property Name As String
        Public Property ID As String
        Public Property Collections As List(Of String)

        Public Sub Save(Filename As String)
            Json.SerializeToFile(Filename, Me, New SkyEditor.Core.Windows.IOProvider)
        End Sub
        Public Shared Function Open(Filename As String) As FlashcartInfoFile
            Return Json.DeserializeFromFile(Of FlashcartInfoFile)(Filename, New SkyEditor.Core.Windows.IOProvider)
        End Function
        Public Sub New()
            Collections = New List(Of String)
        End Sub
    End Class
End Namespace