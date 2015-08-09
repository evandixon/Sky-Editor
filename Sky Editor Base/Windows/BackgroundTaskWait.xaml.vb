Imports System.Windows.Threading

Namespace SkyEditorWindows
    Public Class BackgroundTaskWait
        Private WithEvents _timer As Timers.Timer
        Private tempMessage As String
        Private shouldClose As Boolean
        Public Overloads Sub Show(Message As String)
            Me.lblMessage.Content = Message
            Me.Show()
        End Sub
        Public Sub ChangeMessage(Message As String)
            lblMessage.Dispatcher.Invoke(Dispatcherpriority.Background, New Action(Sub()
                                                                                       lblMessage.Content = Message
                                                                                   End Sub))
        End Sub
        Public Sub DoClose()
            Me.Dispatcher.Invoke(DispatcherPriority.Background, New Action(Sub()
                                                                               Me.Close()
                                                                           End Sub))
            shouldClose = True
        End Sub
        Private Sub BackgroundTaskWait_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Title = PluginHelper.GetLanguageItem("BackgroundTask", "Background Task")
            '_timer = New Timers.Timer(1000)
            'shouldClose = False
            '_timer.Start()
        End Sub

        'Private Sub _timer_Elapsed(sender As Object, e As Timers.ElapsedEventArgs) Handles _timer.Elapsed
        '    _timer.Stop()
        '    If tempMessage IsNot Nothing Then
        '        Me.lblMessage.Content = tempMessage
        '        tempMessage = Nothing
        '    End If
        '    If shouldClose Then
        '        Me.Close()
        '    End If
        '    _timer.Start()
        'End Sub
    End Class
End Namespace