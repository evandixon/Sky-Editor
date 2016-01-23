Public Class HighlightDefinition
    Public Property Name As String
    Public Property NamedHighlightColors As List(Of HighlightingStyle)
    Public Property Rules As List(Of HighlightRule)
    Public Sub New()
        NamedHighlightColors = New List(Of HighlightingStyle)
        Rules = New List(Of HighlightRule)
    End Sub
End Class
