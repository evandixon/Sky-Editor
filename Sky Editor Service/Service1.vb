Imports System.Timers
Imports SkyEditorBase

Public Class Service1

    Private WithEvents workTimer As Timer
    Private WithEvents manager As PluginManager

    Protected Overrides Sub OnStart(ByVal args() As String)
        ' Add code here to start your service. This method should set things
        ' in motion so your service can do its work.
        workTimer = New Timer(1000)
        workTimer.Start()
    End Sub

    Protected Overrides Sub OnStop()
        ' Add code here to perform any tear-down necessary to stop your service.
        Dim stopTask = Task.Run(New Action(AddressOf DoStop))
        Dim waitTask As Task

        Dim retriesLeft = 5
WaitForTask:
        'Wait until either the stopTask or 5 seconds has elapsed
        waitTask = Task.Delay(5000)
        Task.WaitAny(stopTask, waitTask)

        'If stopTask hasn't finished, request more time, unless we've already done so a few times already
        If Not stopTask.IsCompleted AndAlso retriesLeft > 0 Then
            'We're requesting more time than we're waiting to avoid abruptly stopping OnStop without our consent.

            RequestAdditionalTime(10000)
            retriesLeft -= 1
            GoTo WaitForTask 'Wait for the stop task again.
        End If
    End Sub

    Private Sub DoStop()
        If manager IsNot Nothing Then
            manager.Dispose() 'This also unloads plugins
        End If
    End Sub

    Private Sub InitPluginManager()
        manager = PluginManager.GetInstance
        manager.LoadPlugins(New ServiceCoreMod)
    End Sub

    Private Sub workTimer_Elapsed(sender As Object, e As ElapsedEventArgs) Handles workTimer.Elapsed
        workTimer.Stop()

        If manager Is Nothing Then
            InitPluginManager()
        End If

        'Todo: run any logic here

        'If we need to do anything repeatedly, uncomment workTimer.Start()
        'workTimer.Start()
    End Sub
End Class
