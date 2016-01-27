Public Class ProcessHelper
    ''' <summary>
    ''' Runs the specified program, capturing console output.
    ''' Returns true when the program exits.
    ''' </summary>
    ''' <param name="Filename"></param>
    ''' <param name="Arguments"></param>
    ''' <remarks></remarks>
    Public Shared Async Function RunProgram(Filename As String, Arguments As String, Optional IsVisible As Boolean = False) As Task
        'WriteLine(String.Format("Executing {0} {1}", Filename, Arguments))
        Dim p As New Process()
        p.StartInfo.FileName = Filename
        p.StartInfo.Arguments = Arguments
        p.StartInfo.RedirectStandardOutput = True
        p.StartInfo.UseShellExecute = False
        If IsVisible Then
            p.StartInfo.WindowStyle = ProcessWindowStyle.Normal
        Else
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
        End If
        p.StartInfo.CreateNoWindow = True
        p.StartInfo.WorkingDirectory = IO.Path.GetDirectoryName(Filename)
        p.Start()
        p.BeginOutputReadLine()
        Await WaitForProcess(p)
        p.Dispose()
        ' WriteLine(String.Format("""{0}"" finished running.", p.StartInfo.FileName))
    End Function
    Public Shared Async Function RunCTRTool(Arguments) As Task
        Await RunProgram("Tools/ctrtool.exe", Arguments)
    End Function
    Private Shared Async Function WaitForProcess(p As Process) As Task
        Await Task.Run(Sub()
                           p.WaitForExit()
                       End Sub)
    End Function
End Class
