Imports System.IO
Imports System.Threading
Imports SkyEditor.Core
Imports SkyEditorBase.Interfaces
Imports SkyEditorBase.Redistribution

Public Class StartupHelpers

    ''' <summary>
    ''' Starts the console core.
    ''' Will shut down the current application when complete.
    ''' </summary>
    Public Shared Async Function StartConsole() As Task
        Dim manager As PluginManager = SkyEditorBase.PluginManager.GetInstance
        manager.LoadCore(New ConsoleCoreMod)

        PluginHelper.ShowConsole()
        Await ConsoleModule.ConsoleMain(manager)

        Application.Current.Shutdown()
    End Function
    Public Shared Async Function RunWPFStartupSequence() As Task
        Await RunWPFStartupSequence(New WpfCoreMod)
    End Function

    Public Shared Async Function RunWPFStartupSequence(CoreMod As CoreSkyEditorPlugin) As Task
        Try
            Await RedistributionHelpers.DeleteScheduledFiles()
        Catch ex As Exception
            MessageBox.Show("An error has occurred during pre-startup: " & ex.ToString)
            Application.Current.Shutdown()
        End Try

        'Run the program
        Dim args As New List(Of String)
        args.AddRange(Environment.GetCommandLineArgs())
        If args.Contains("-culture") Then
            Dim index = args.IndexOf("-culture")
            If args.Count > index + 1 Then
                Dim culture = args(index + 1)
                Thread.CurrentThread.CurrentCulture = New Globalization.CultureInfo(culture)
                Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(culture)
            End If
        End If
        If args.Contains("-console") Then
            Await StartupHelpers.StartConsole()
        Else
            Dim manager As PluginManager = SkyEditorBase.PluginManager.GetInstance
            manager.LoadCore(CoreMod)

            Dim checkForUpdates As Boolean = SettingsManager.Instance.Settings.UpdatePlugins
            If args.Contains("-disableupdates") Then
                checkForUpdates = False
            End If

            Dim l As UI.BackgroundTaskWait = Nothing
            'If checkForUpdates Then
            '    l = New UI.BackgroundTaskWait
            '    l.ChangeMessage(PluginHelper.GetLanguageItem("Updating plugins..."))
            '    l.Show()

            '    Try
            '        PluginHelper.SetLoadingStatus(PluginHelper.GetLanguageItem("Updating plugins..."))
            '        If Await Task.Run(Function() As Boolean
            '                              Return RedistributionHelpers.DownloadAllPlugins(manager, SettingsManager.Instance.Settings.PluginUpdateUrl)
            '                          End Function) Then
            '            PluginHelper.SetLoadingStatusFailed()
            '            manager.Dispose()
            '            RedistributionHelpers.RequestRestartProgram()
            '            Application.Current.Shutdown()
            '        End If
            '        PluginHelper.SetLoadingStatusFinished()
            '    Catch ex As Net.WebException
            '        'Do nothing, we simply won't update the plugins
            '    Catch ex As Exception
            '        PluginHelper.SetLoadingStatusFinished()
            '        PluginHelper.Writeline("Unable to update plugins.  Error: " & ex.ToString, PluginHelper.LineType.Error)
            '    End Try
            '    l.Visibility = Visibility.Collapsed
            'End If

            Dim m As New UI.MainWindow(manager)
            m.Show()
            If l IsNot Nothing Then
                l.Close()
            End If
        End If
    End Function

    Public Shared Sub RunExitSequence()
        'Delete .tmp files
        For Each item In Directory.GetFiles(PluginHelper.RootResourceDirectory, "*.tmp", SearchOption.AllDirectories)
            Try
                File.Delete(item)
            Catch ex As IOException
                'Something's keeping the file from being deleted.  It's probably still open.  It will get deleted the next time the program exits.
            End Try
        Next

        SettingsManager.Instance.Save()

        ''Restart program if exit code warrants it
        'If e.ApplicationExitCode = 1 Then
        '    Dim args = Environment.GetCommandLineArgs()
        '    If args.Length = 1 Then
        '        Process.Start(args(0))
        '    Else
        '        Dim params As New Text.StringBuilder
        '        For count As Integer = 1 To args.Length - 1
        '            params.Append(String.Format("""{0}"" ", params(count)))
        '        Next
        '        Process.Start(args(0), params.ToString.Trim())
        '    End If
        'End If
    End Sub
End Class