Public Class ExternalProgramManager
    Implements IDisposable

    Private Function GetToolsDir() As String
        Dim path = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SkyEditor.ROMEditor")
        If Not IO.Directory.Exists(path) Then
            IO.Directory.CreateDirectory(path)
        End If
        Return path
    End Function

    ''' <summary>
    ''' Gets the path for ctrtool.exe
    ''' </summary>
    ''' <returns></returns>
    Public Function GetCtrToolPath() As String
        'Ensure ctrtool exists
        Dim path = IO.Path.Combine(GetToolsDir, "ctrtool.exe")
        If Not IO.File.Exists(path) Then
            IO.File.WriteAllBytes(path, My.Resources.ctrtool)
        End If
        Return path
    End Function

    ''' <summary>
    ''' Runs ctrtool.exe with the given arguments
    ''' </summary>
    ''' <param name="arguments"></param>
    ''' <returns></returns>
    Public Async Function RunCtrTool(arguments As String) As Task
        Await SkyEditor.Core.Windows.Processes.ConsoleApp.RunProgram(GetCtrToolPath, arguments)
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class
