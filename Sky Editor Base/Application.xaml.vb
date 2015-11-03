Imports System.Deployment
Imports System.Deployment.Application
Imports System.IO
Imports SkyEditorBase.Redistribution

Class Application

    Private Sub Application_Exit(sender As Object, e As ExitEventArgs) Handles Me.Exit
        'Delete .tmp files
        For Each item In IO.Directory.GetFiles(PluginHelper.RootResourceDirectory, "*.tmp", IO.SearchOption.AllDirectories)
            Try
                IO.File.Delete(item)
            Catch ex As IOException
                'Something's keeping the file from being deleted.  It's probably still open.  It will get deleted the next time the program exits.
            End Try
        Next

        'Save pending language changes
        Dim languageManager = Language.LanguageManager.Instance
        If languageManager.AdditionsMade Then
            languageManager.SaveAll
        End If

        'Restart program if exit code warrants it
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
        Try
            'Delete and install things
            RedistributionHelpers.DeleteScheduledFiles()
            RedistributionHelpers.InstallPendingPlugins()
        Catch ex As Exception
            MessageBox.Show("An error has occurred during pre-startup: " & ex.ToString)
            Application.Current.Shutdown()
        End Try

        'Run the program
        Dim args = Environment.GetCommandLineArgs
        If args.Contains("-console") Then
            Language.ConsoleManager.Show()
            ConsoleModule.ConsoleMain()
            Application.Current.Shutdown()
        Else
            Dim m As New MainWindow2
            m.Show()
        End If
    End Sub
End Class