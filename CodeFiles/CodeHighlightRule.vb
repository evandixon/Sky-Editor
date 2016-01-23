Public Class CodeHighlightRule
    Public Property Name As String
    Public Property Expression As String
    Public Property IgnoreCase As Boolean
    Public Property Foreground As String
    Public Property FontWeight As String
    Public Property FontStyle As String
    Public Overridable Function Clone() As CodeHighlightRule
        Dim output As New CodeHighlightRule
        output.Name = Me.Name
        output.Expression = Me.Expression
        output.IgnoreCase = Me.IgnoreCase
        output.Foreground = Me.Foreground
        output.FontWeight = Me.FontWeight
        output.FontStyle = Me.FontStyle
        Return output
    End Function
End Class
