Imports System.Windows.Threading

Namespace UI
    Public Class BackgroundTaskWait
        Public Overloads Sub Show(Message As String)
            Me.lblMessage.Content = Message
            Me.Show()
        End Sub
        Public Sub ChangeMessage(Message As String)
            lblMessage.Dispatcher.Invoke(DispatcherPriority.Background, New Action(Sub()
                                                                                       lblMessage.Content = Message
                                                                                   End Sub))
        End Sub
        Public Sub DoClose()
            Me.Dispatcher.Invoke(DispatcherPriority.Background, New Action(Sub()
                                                                               Me.Close()
                                                                           End Sub))
        End Sub
        Private Sub BackgroundTaskWait_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Title = My.Resources.Language.BackgroundTask
        End Sub
    End Class
End Namespace