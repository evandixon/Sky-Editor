Namespace SkyEditorWindows
    Public Class BackgroundTaskWait
        Public Overloads Sub Show(Message As String)
            Me.lblMessage.Content = Message
            Me.Show()
        End Sub

        Private Sub BackgroundTaskWait_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Title = PluginHelper.GetLanguageItem("BackgroundTask", "Background Task")
        End Sub
    End Class
End Namespace

