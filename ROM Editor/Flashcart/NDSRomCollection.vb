Namespace Flashcart
    Public Class NDSRomCollection
        Inherits FileCollection

        Public Overrides Function GetFiles(CurrentFlashcart As GenericFlashcart) As IEnumerable(Of Object)
            Dim out As New List(Of FlashcartNDSRom)
            For Each item In IO.Directory.GetFiles(IO.Path.Combine(CurrentFlashcart.Drive.RootDirectory.FullName, Me.Directory), "*.nds", IO.SearchOption.TopDirectoryOnly)
                out.Add(New FlashcartNDSRom(item))
            Next
            Return out
        End Function
    End Class

End Namespace
