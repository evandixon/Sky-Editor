Imports SkyEditorBase.Interfaces

Public Class ExecutableFile
    Implements iOpenableFile
    Implements iOnDisk

    Private WithEvents _process As Process
    Dim _started As Boolean
    Public Property Filename As String Implements iOnDisk.Filename

    Public Sub OpenFile(Filename As String) Implements iOpenableFile.OpenFile
        Me.Filename = Filename
        InitProcess()
    End Sub

    Private Sub _process_Exited(sender As Object, e As EventArgs) Handles _process.Exited
        _process = Nothing
        _started = False
    End Sub

    Private Sub InitProcess()
        _process = New Process
        _process.StartInfo.FileName = Filename
        _process.StartInfo.WorkingDirectory = IO.Path.GetDirectoryName(Filename)
        _process.EnableRaisingEvents = True
        _started = False
    End Sub

    Public Sub Start()
        If _process Is Nothing Then
            InitProcess()
        End If

        If _started Then
            If MessageBox.Show(PluginHelper.GetLanguageItem("This program is alredy open.  Would you like to stop the current instance?  Any unsaved data may be lost."), "Sky Editor", MessageBoxButton.YesNo) = MessageBoxResult.Yes Then
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
