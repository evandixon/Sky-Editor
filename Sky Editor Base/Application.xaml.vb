Imports System.Deployment
Imports System.Deployment.Application
Imports System.IO
Imports System.Threading.Tasks
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

        SettingsManager.Instance.Save()

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

    Private Async Sub Application_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup
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
            Internal.ConsoleManager.Show()
            ConsoleModule.ConsoleMain()
            Application.Current.Shutdown()
        Else
            Dim _manager As PluginManager = PluginManager.GetInstance

            Dim l As SkyEditorWindows.BackgroundTaskWait = Nothing
            If SettingsManager.Instance.Settings.UpdatePlugins Then
                l = New SkyEditorWindows.BackgroundTaskWait
                l.ChangeMessage(PluginHelper.GetLanguageItem("Updating plugins..."))
                l.Show()

                Try
                    PluginHelper.StartLoading(PluginHelper.GetLanguageItem("Updating plugins..."))
                    If Await Task.Run(Function() As Boolean
                                          Return RedistributionHelpers.DownloadAllPlugins(_manager, SettingsManager.Instance.Settings.PluginUpdateUrl)
                                      End Function) Then
                        PluginHelper.StopLoading()
                        RedistributionHelpers.RestartProgram()
                        l.Close()
                        Exit Sub
                    End If
                    PluginHelper.StopLoading()
                Catch ex As Exception
                    PluginHelper.StopLoading()
                    PluginHelper.Writeline("Unable to update plugins.  Error: " & ex.ToString, PluginHelper.LineType.Error)
                End Try
                l.Visibility = Visibility.Collapsed
            End If

            Dim m As New MainWindow2(_manager)
            m.Show()
            If l IsNot Nothing Then
                l.Close()
            End If
        End If
    End Sub
End Class