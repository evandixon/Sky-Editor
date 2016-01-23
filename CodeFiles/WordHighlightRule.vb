Public Class WordHighlightRule
    Inherits CodeHighlightRule
    Public Property Words As List(Of String)
    Public Sub New()
        Words = New List(Of String)
    End Sub
    Public Overrides Function Clone() As CodeHighlightRule
        Dim output As New WordHighlightRule
        output.Name = Me.Name
        output.Expression = Me.Expression
        output.IgnoreCase = Me.IgnoreCase
        output.Foreground = Me.Foreground
        output.FontWeight = Me.FontWeight
        output.FontStyle = Me.FontStyle
        output.Words = New List(Of String)
        For Each item In Me.Words
            output.Words.Add(item.Clone)
        Next
        Return output
    End Function
End Class
