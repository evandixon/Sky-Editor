Imports System.IO
Imports SkyEditorBase.Redistribution

Class Application

    ' Application-level events, such as Startup, Exit, and DispatcherUnhandledException
    ' can be handled in this file.
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
        Dim languageManager = SkyEditorBase.Language.LanguageManager.Instance
        If languageManager.AdditionsMade Then
            languageManager.SaveAll()
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

    Private Async Sub Application_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup
        Try
            RedistributionHelpers.DeleteScheduledFiles()
            RedistributionHelpers.InstallPendingPlugins()
        Catch ex As Exception
            MessageBox.Show("An error has occurred during pre-startup: " & ex.ToString)
            Application.Current.Shutdown()
        End Try


        'Run the program
        Dim args = Environment.GetCommandLineArgs
        If args.Contains("-console") Then
            PluginHelper.ShowConsole()
            ConsoleModule.ConsoleMain()
            Application.Current.Shutdown()
        Else
            Dim manager As PluginManager = PluginManager.GetInstance
            'Add a handler so we can register certain things as if this application was a plugin
            AddHandler manager.PluginsLoading, AddressOf OnPluginsLoading
            manager.LoadPlugins()

            'Remove the handler.
            'Since we've already done what was needed at the particular point in LoadPlugins(), having the event handler registered is pointless at best
            RemoveHandler manager.PluginsLoading, AddressOf OnPluginsLoading

            Dim checkForUpdates As Boolean = SettingsManager.Instance.Settings.UpdatePlugins
            If args.Contains("-disableupdates") Then
                checkForUpdates = False
            End If

            Dim l As SkyEditorWindows.BackgroundTaskWait = Nothing
            If checkForUpdates Then
                l = New SkyEditorWindows.BackgroundTaskWait
                l.ChangeMessage(PluginHelper.GetLanguageItem("Updating plugins..."))
                l.Show()

                Try
                    PluginHelper.StartLoading(PluginHelper.GetLanguageItem("Updating plugins..."))
                    If Await Task.Run(Function() As Boolean
                                          Return RedistributionHelpers.DownloadAllPlugins(manager, SettingsManager.Instance.Settings.PluginUpdateUrl)
                                      End Function) Then
                        PluginHelper.StopLoading()
                        manager.Dispose()
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

            Dim m As New MainWindow2(manager)
            m.Show()
            If l IsNot Nothing Then
                l.Close()
            End If
        End If
    End Sub

    Private Shared Sub OnPluginsLoading(sender As Object, e As EventArguments.PluginLoadingEventArgs)
        PluginInfo.Load(sender)
    End Sub
End Class
