﻿Imports AurelienRibon.Ui.SyntaxHighlightBox
Imports SkyEditorBase

Namespace Controls
    Public Class SyntaxHighlightBox
        Inherits ObjectControl(Of CodeFile)
        Public Overrides Sub RefreshDisplay()
            MyBase.RefreshDisplay()
            txtCode.CurrentHighlighter = EditingItem.CodeHighlighter
            txtCode.Text = EditingItem.Text
        End Sub
        Public Overrides Sub UpdateObject()
            EditingItem.Text = txtCode.Text
        End Sub
        Public Overrides Function UsagePriority(Type As Type) As Integer
            Return 2
        End Function
    End Class
End Namespace
