Imports System.Threading.Tasks
Imports System.Runtime.CompilerServices
Imports SkyEditorBase.Windows

Public Class DeveloperConsole
    Public Shared Event OnWriteLine(sender As Object, e As WritelineEventArgs)
    Public Class WritelineEventArgs
        Inherits EventArgs
        Dim _line As String
        Public ReadOnly Property Line As String
            Get
                Return _line
            End Get
        End Property
        Public Sub New(Line As String)
            _line = Line
        End Sub
    End Class

    Public Shared Sub DoCommands(Manager As PluginManager, ConsoleCommands As Dictionary(Of String, PluginManager.ConsoleCommand))
        DeveloperConsole.Writeline("Type ""exit"" to return to Sky Editor.")
        While True
            Console.Write("> ")
            Dim line = Console.ReadLine()
            Dim cmdParts = line.Split(" ".ToCharArray, 2)
            Dim cmd = cmdParts(0).ToLower
            Dim arg = ""
            If cmdParts.Length > 1 Then
                arg = cmdParts(1)
            End If
            If cmd = "exit" Then
                DeveloperConsole.Writeline("You may now use Sky Editor again.")
                Exit While
            ElseIf ConsoleCommands.Keys.Contains(cmd) Then
                ConsoleCommands(cmd).Invoke(Manager, arg)
            Else
                DeveloperConsole.Writeline(String.Format("""{0}"" is not a recognisable command.", cmd))
            End If
        End While
    End Sub
    Public Shared Sub Writeline(Line As String, <CallerMemberName> Optional CallerName As String = Nothing)
        RaiseEvent OnWriteLine(CallerName, New WritelineEventArgs(Line))
        Console.WriteLine(String.Format("{0}: {1}", CallerName, Line))
    End Sub

    Private Shared loadingWindow As BackgroundTaskWait
    ''' <summary>
    ''' Posted by brendan at http://stackoverflow.com/questions/9996709/read-console-process-output
    ''' </summary>
    ''' <param name="sendingProcess"></param>
    ''' <param name="outLine"></param>
    ''' <remarks></remarks>
    Private Shared Sub OutputHandler(sendingProcess As Object, outLine As DataReceivedEventArgs)
        ' Collect the sort command output.
        If Not String.IsNullOrEmpty(outLine.Data) Then
            'Add the text to the collected output.
            'Don't write to dev console, this is a different thread.
            'The only difference is that this output will be shown in the Real console, while DebugConsole.Writeline may be shown in the window
            Console.WriteLine(outLine.Data)
        End If
    End Sub
    Private Shared Sub ProgramFinished(ByVal sender As Object, ByVal e As System.EventArgs)

    End Sub
    ''' <summary>
    ''' Runs the specified program synchronously, capturing console output
    ''' </summary>
    ''' <param name="Filename"></param>
    ''' <param name="Arguments"></param>
    ''' <remarks></remarks>
    Public Shared Async Function RunProgram(Filename As String, Arguments As String) As Task(Of Boolean)
        Writeline(String.Format("Executing {0} {1}", Filename, Arguments))
        Dim p As New Process()
        p.StartInfo.FileName = Filename
        p.StartInfo.Arguments = Arguments
        p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
        p.StartInfo.RedirectStandardOutput = True
        p.StartInfo.UseShellExecute = False
        AddHandler p.OutputDataReceived, AddressOf OutputHandler
        p.Start()
        p.BeginOutputReadLine()
        loadingWindow = New BackgroundTaskWait
        loadingWindow.Show("Waiting on " & IO.Path.GetFileName(Filename))

        'p.WaitForExit()
        Await WaitForProcess(p)

        'If x Then
        loadingWindow.Close()
        RemoveHandler p.OutputDataReceived, AddressOf OutputHandler
        p.Dispose()
        Writeline(String.Format("""{0}"" finished running.", p.StartInfo.FileName))
        'End If
        Return True
    End Function
    Private Shared Async Function WaitForProcess(p As Process) As Task(Of Boolean)
        Return Await Task.Run(Function()
                                  p.WaitForExit()
                                  Return True
                              End Function)
    End Function
    ''' <summary>                    
    ''' 
    ''' Runs the specified program asyncrynously
    ''' </summary>
    ''' <param name="Filename"></param>
    ''' <param name="Arguments"></param>
    ''' <remarks></remarks>
    Public Shared Sub RunProgramAsync(Filename As String, Arguments As String)
        Writeline(String.Format("(Async) Executing ""{0}"" ""{1}""", Filename, Arguments))
        Dim p As New Process()
        p.StartInfo.FileName = Filename
        p.StartInfo.Arguments = Arguments
        'p.StartInfo.WindowStyle = ProcessWindowStyle.Normal
        'p.StartInfo.RedirectStandardOutput = True
        'p.StartInfo.UseShellExecute = False
        'AddHandler p.OutputDataReceived, AddressOf OutputHandler
        p.Start()
        'p.BeginOutputReadLine()
    End Sub
End Class
