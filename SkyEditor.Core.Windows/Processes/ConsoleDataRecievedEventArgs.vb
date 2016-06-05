Namespace Processes
    Public Class ConsoleDataRecievedEventArgs
        Inherits EventArgs
        Public Property Line As String
        Public Sub New(Line As String)
            Me.Line = Line
        End Sub
    End Class
End Namespace
