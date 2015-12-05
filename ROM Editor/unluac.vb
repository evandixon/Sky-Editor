Imports SkyEditorBase

Public Class unluac
    ''' <summary>
    ''' Creates a new instance of the lua decompiler.
    ''' </summary>
    ''' <param name="Filename">Filename of the compiled lua script.</param>
    Public Sub New(Filename As String)
        Dim args As New Text.StringBuilder

        Instance = New Process
        Instance.StartInfo.FileName = "java"
        Instance.StartInfo.Arguments = $"-jar ""{PluginHelper.GetResourceName("unluac.jar")}"" ""{Filename}"""
        Instance.StartInfo.RedirectStandardOutput = True
        Instance.StartInfo.UseShellExecute = False
        Instance.StartInfo.CreateNoWindow = True
    End Sub

    Private Sub Start()
        Instance.Start()
        Instance.BeginOutputReadLine()
    End Sub

    Private Async Function WaitForExit() As Task
        Await Task.Run(New Action(Sub()
                                      Instance.WaitForExit()
                                  End Sub))
    End Function

    Public Async Function Decompile() As Task(Of String)
        Start()
        Await WaitForExit()
        Return Output.ToString
    End Function

    Private Sub Instance_OutputDataReceived(sender As Object, e As DataReceivedEventArgs) Handles Instance.OutputDataReceived
        Output.AppendLine(e.Data)
    End Sub

    Private WithEvents Instance As Process
    Private Property Output As New Text.StringBuilder
End Class
