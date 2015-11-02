Namespace Language
    Public Class LanguageEditor
        Inherits ObjectControl(Of Language.LanguageManager)
        Public Overrides Sub RefreshDisplay()
            MyBase.RefreshDisplay()
            For Each item In EditingItem.Languages("English").ContainedObject
                listBox1.Items.Add(item)
            Next
        End Sub
    End Class
End Namespace