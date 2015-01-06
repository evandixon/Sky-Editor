Namespace Windows
    Public Class BackgroundTaskWait
        Public Overloads Sub Show(Message As String)
            Me.lblMessage.Content = Message
            Me.Show()
        End Sub
    End Class
End Namespace

