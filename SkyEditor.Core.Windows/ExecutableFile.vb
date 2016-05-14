Imports System.Threading.Tasks
Imports SkyEditor.Core.IO

Public Class ExecutableFile
    Implements IOpenableFile
    Implements iOnDisk

    Private WithEvents _process As Process
    Dim _started As Boolean
    Public Property Filename As String Implements iOnDisk.Filename

    Public ReadOnly Property Started As Boolean
        Get
            Return _started
        End Get
    End Property

    Public Function OpenFile(Filename As String, Provider As Core.IO.IOProvider) As Task Implements IOpenableFile.OpenFile
        Me.Filename = Filename
        InitProcess()
        Return Task.CompletedTask
    End Function

    Private Sub _process_Exited(sender As Object, e As EventArgs) Handles _process.Exited
        _process = Nothing
        _started = False
    End Sub

    Private Sub InitProcess()
        _process = New Process
        _process.StartInfo.FileName = Filename
        _process.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(Filename)
        _process.EnableRaisingEvents = True
        _started = False
    End Sub

    Public Sub Start(RestartIfOpen As Boolean)
        If _process Is Nothing Then
            InitProcess()
        End If

        If _started Then
            If RestartIfOpen Then
                _process.Kill()
                _process.Dispose()
                InitProcess()

                _process.Start()
                _started = True
            End If
        Else
            _process.Start()
            _started = True
        End If
    End Sub

    Public Shared Function IsExeFile(Filename As String) As Boolean
        Return Filename.ToLower.EndsWith(".exe")
    End Function

End Class
