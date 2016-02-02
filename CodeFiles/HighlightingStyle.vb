Public Class HighlightingStyle
    Public Property Name As String
    Public Property Foreground As String
    Public Property Background As String
    Public Property FontWeight As String
    Public Property FontStyle As String
    Public Property Underline As Boolean
    Public Sub New()
        Foreground = "Transparent"
        Background = "Transparent"
        FontStyle = "Normal"
        FontWeight = "Normal"
        Underline = False
    End Sub
End Class
