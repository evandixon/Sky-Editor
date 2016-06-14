Imports System.IO
Imports System.Threading
Imports System.Windows.Controls
Imports SkyEditor.Core
Imports SkyEditor.Core.Windows
Imports SkyEditor.UI.WPF

Public Class StartupHelpers

    '''' <summary>
    '''' Starts the console core.
    '''' Will shut down the current application when complete.
    '''' </summary>
    'Public Shared Async Function StartConsole() As Task
    '    Dim manager As New PluginManager
    '    Await manager.LoadCore(New ConsoleCoreMod)

    '    PluginHelper.ShowConsole()
    '    Await ConsoleHelper.RunConsole(manager)

    '    Application.Current.Shutdown()
    'End Function
    Public Shared Sub RunWPFStartupSequence()
        RunWPFStartupSequence(New WPFCoreSkyEditorPlugin)
    End Sub

    Public Shared Sub RunWPFStartupSequence(CoreMod As CoreSkyEditorPlugin)
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
        'If args.Contains("-console") Then
        '    Await StartupHelpers.StartConsole()
        'Else
        Dim manager As New PluginManager
        manager.LoadCore(CoreMod).Wait()


        manager.CurrentIOUIManager.SupportedToolWindowTypes = {GetType(UserControl)}
        Dim m As New MainWindow3 'UI.MainWindow(manager)
        m.CurrentPluginManager = manager
        m.DataContext = manager.CurrentIOUIManager
        m.Show()
    End Sub

    Public Shared Sub RunExitSequence()
        'Delete .tmp files
        For Each item In Directory.GetFiles(EnvironmentPaths.GetRootResourceDirectory, "*.tmp", SearchOption.AllDirectories)
            Try
                File.Delete(item)
            Catch ex As IOException
                'Something's keeping the file from being deleted.  It's probably still open.  It will get deleted the next time the program exits.
            End Try
        Next
    End Sub
End Class