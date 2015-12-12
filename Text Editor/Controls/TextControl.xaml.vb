Imports System.Windows.Controls

Namespace Controls
    Public Class TextControl
        Inherits SkyEditorBase.ObjectControl(Of iTextFile)
        Public Overrides Sub RefreshDisplay()
            txtText.Text = EditingItem.Text
        End Sub
        Public Overrides Sub UpdateObject()
            EditingItem.Text = txtText.Text
        End Sub

        Private Sub txtText_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtText.TextChanged
            RaiseModified()
        End Sub

        Public Overrides Function UsagePriority(Type As Type) As Integer
            Return 0
        End Function
    End Class
End Namespace

