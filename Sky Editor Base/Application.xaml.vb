Imports System.Deployment
Imports System.Deployment.Application
Imports SkyEditorBase.Redistribution

Class Application

    Private Sub Application_Exit(sender As Object, e As ExitEventArgs) Handles Me.Exit
        If e.ApplicationExitCode = 1 Then
            Dim args = Environment.GetCommandLineArgs()
            If args.Length = 1 Then
                Process.Start(args(0))
            Else
                Dim params As New Text.StringBuilder
                For count As Integer = 1 To args.Length - 1
                    params.Append(String.Format("""{0}"" ", params(count)))
                Next
                Process.Start(args(0), params.ToString.Trim())
            End If
        End If
    End Sub

    ' Application-level events, such as Startup, Exit, and DispatcherUnhandledException
    ' can be handled in this file.

    Private Sub Application_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup
        RedistributionHelpers.DeleteScheduledFiles()
        RedistributionHelpers.InstallPendingPlugins()

    End Sub
End Class