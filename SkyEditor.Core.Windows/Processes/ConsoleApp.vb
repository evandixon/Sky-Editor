Imports System.Collections.Concurrent
Imports System.IO
Imports System.Text

Namespace Processes
    Public Class ConsoleApp
        Implements IDisposable

        ''' <summary>
        ''' Runs the specified program without waiting for it to complete.
        ''' </summary>
        ''' <param name="Filename">Filename of the executable to run.</param>
        ''' <param name="Arguments">Arguments to pass to the process.</param>
        ''' <returns>The ConsoleApp that was started.</returns>
        ''' <remarks></remarks>
        Public Shared Function RunProgramInBackground(Filename As String, Arguments As String) As ConsoleApp
            Dim p As New ConsoleApp(Filename, Arguments)
            p.StartConsole()
            Return p
        End Function

        Public Shared Async Function RunProgram(Filename As String, Arguments As String) As Task
            ''Todo: figure out why this style isn't properly waiting for exit
            'Using program As New ConsoleApp(Filename, Arguments)
            '    program.WorkingDirectory = Path.GetDirectoryName(Filename)
            '    program.StartConsole()
            '    Await program.WaitForExit
            'End Using

            'Set up the process
            Dim p As New Process()
            p.StartInfo.FileName = Filename
            p.StartInfo.Arguments = Arguments
            p.StartInfo.RedirectStandardOutput = True
            p.StartInfo.UseShellExecute = False
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
            p.StartInfo.CreateNoWindow = True
            p.StartInfo.WorkingDirectory = Path.GetDirectoryName(Filename)

            'Start the process
            p.Start()
            p.BeginOutputReadLine()

            'Wait for the process to close
            Await WaitForProcess(p)

            p.Dispose()
        End Function

        ''' <summary>
        ''' Waits for the given process to exit.
        ''' </summary>
        ''' <param name="p">The process for which to wait.</param>
        ''' <returns></returns>
        Private Shared Async Function WaitForProcess(p As Process) As Task
            Await Task.Run(Sub()
                               p.WaitForExit()
                           End Sub)
        End Function

        Public Sub New(Filename As String, Arguments As String)
            Me.Status = ProcessStatus.Stopped
            ConsoleHistory = New ConcurrentQueue(Of String)
            MaxHistoryLines = -1
            Me.Filename = Filename
            Me.Arguments = Arguments
            InitializeProcess()
        End Sub

#Region "Event Args Classes"
        Public Class InputDataSentEventArgs
            Inherits EventArgs
            Public Property Line As String
            Public Sub New(Line As String)
                Me.Line = Line
            End Sub
        End Class
        Public Class StatusChangedEventArgs
            Inherits EventArgs
            Public Property OldStatus As String
            Public Property NewStatus As String
            Public Sub New(OldStatus As String, NewStatus As String)
                Me.OldStatus = OldStatus
                Me.NewStatus = NewStatus
            End Sub
        End Class
#End Region

#Region "Events"

        ''' <summary>
        ''' Raised when the process writes data to the error output.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Public Event ErrorDataReceived(sender As Object, e As ConsoleDataRecievedEventArgs)

        ''' <summary>
        ''' Raised when data is written to the standard input.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Public Event InputDataSent(sender As Object, e As InputDataSentEventArgs)

        ''' <summary>
        ''' Raised when the status of the process is changed.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Public Event StatusChanged(sender As Object, e As StatusChangedEventArgs)

        Public Event Started(sender As Object, e As EventArgs)
        Public Event Stopped(sender As Object, e As EventArgs)
        Public Event ConsoleDataRecieved(sender As Object, e As ConsoleDataRecievedEventArgs)

        Protected Sub RaiseConsoleDataRecieved(sender As Object, e As ConsoleDataRecievedEventArgs)
            RaiseEvent ConsoleDataRecieved(sender, e)
        End Sub
#End Region

#Region "Data"

        ''' <summary>
        ''' Path of the process's file.
        ''' </summary>
        ''' <returns></returns>
        Public Overridable Property Filename As String
        ''' <summary>
        ''' Start arguments for the process.
        ''' </summary>
        ''' <returns></returns>
        Public Overridable Property Arguments As String

        Public Property WorkingDirectory As String
            Get
                Return _p.StartInfo.WorkingDirectory
            End Get
            Set(value As String)
                _p.StartInfo.WorkingDirectory = value
            End Set
        End Property

        Public Property Process As Process
            Get
                Return _p
            End Get
            Set(value As Process)
                _p = value
            End Set
        End Property
        Private WithEvents _p As Process

        ''' <summary>
        ''' Status of the process.
        ''' </summary>
        ''' <returns></returns>
        Public Property Status As ProcessStatus
            Get
                Return _status
            End Get
            Set(value As ProcessStatus)
                Dim old As String = _status
                _status = value
                RaiseEvent StatusChanged(Me, New StatusChangedEventArgs(old, value))
            End Set
        End Property
        Private _status As ProcessStatus

        Protected Property ConsoleHistory As ConcurrentQueue(Of String)

        Public Property MaxHistoryLines As Integer

#End Region

#Region "Operations"
        Public Overridable Sub StartConsole()
            If Not Status = ProcessStatus.Configuring Then
                If _p Is Nothing Then
                    InitializeProcess()
                End If
                Status = ProcessStatus.Starting
                _p.Start()
                If _p.StartInfo.RedirectStandardOutput Then _p.BeginOutputReadLine()
                If _p.StartInfo.RedirectStandardError Then _p.BeginErrorReadLine()
                Status = ProcessStatus.Started
                RaiseEvent Started(Me, New EventArgs)
            End If
        End Sub

        Public Overridable Sub StopConsole()
            If Not Status = ProcessStatus.Configuring Then
                Status = ProcessStatus.Stopping
                _p.Close()
            End If
        End Sub
        Public Overridable Sub KillConsole()
            If Not Status = ProcessStatus.Configuring Then
                Status = ProcessStatus.Killing
                Try
                    _p.Kill()
                Catch ex As InvalidOperationException
                    If Not ex.ToString.Contains("No process is associated with this object.") Then
                        Throw ex
                    End If
                End Try
                Status = ProcessStatus.Killed
            End If
        End Sub
        Public Overridable Sub SendInput(ConsoleLine As String)
            If Status = ProcessStatus.Started Then
                _p.StandardInput.WriteLine(ConsoleLine)
                RaiseEvent InputDataSent(Me, New InputDataSentEventArgs(ConsoleLine))
            Else
                Console.WriteLine("Unable to send input to console that is not running.")
            End If
        End Sub

        Public Async Function WaitForExit() As Task
            Await Task.Run(New Action(Sub()
                                          'If the status is Stopped, that means the process exited (and was reinitialized with the event handler) before we got here
                                          If Not Status = ProcessStatus.Stopped Then
                                              _p.WaitForExit()
                                          End If
                                      End Sub))
        End Function
#End Region

#Region "Event Handlers"
        Private Sub _p_Exited(sender As Object, e As EventArgs) Handles _p.Exited
            Status = ProcessStatus.Stopped
            RaiseEvent Stopped(Me, e)
            InitializeProcess()
        End Sub
        Private Sub _p_OutputDataReceived(sender As Object, e As DataReceivedEventArgs) Handles _p.OutputDataReceived
            RaiseEvent ConsoleDataRecieved(Me, New ConsoleDataRecievedEventArgs(e.Data))
            ConsoleHistory.Enqueue(e.Data)
            While ConsoleHistory.Count > MaxHistoryLines
                Dim line As Object = Nothing 'Only here so that TryDequeue works.  We're not actually doing anything with the result
                ConsoleHistory.TryDequeue(line)
            End While
        End Sub
        Private Sub _p_ErrorDataReceived(sender As Object, e As DataReceivedEventArgs) Handles _p.ErrorDataReceived
            RaiseEvent ErrorDataReceived(Me, New ConsoleDataRecievedEventArgs(e.Data))
            ConsoleHistory.Enqueue(e.Data)
            'Remove history lines if the MaxHistoryLines is positive and if we have more lines than MaxHistoryLines
            While ConsoleHistory.Count > MaxHistoryLines AndAlso MaxHistoryLines > -1
                Dim line As Object = Nothing 'Only here so that TryDequeue works.  We're not actually doing anything with the result
                ConsoleHistory.TryDequeue(line)
            End While
        End Sub
        Private Sub _p_Disposed(sender As Object, e As EventArgs) Handles _p.Disposed
            Status = ProcessStatus.Stopped
            _p = Nothing
        End Sub
#End Region

        Protected Overridable Sub InitializeProcess()
            If _p IsNot Nothing Then
                _p.Dispose()
            End If
            _p = New Process
            With _p
                .StartInfo.FileName = Filename
                .StartInfo.Arguments = Arguments
                .StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                .StartInfo.RedirectStandardOutput = True
                .StartInfo.RedirectStandardInput = True
                .StartInfo.RedirectStandardError = True
                .StartInfo.UseShellExecute = False
                .StartInfo.CreateNoWindow = True
                .StartInfo.WorkingDirectory = Path.GetDirectoryName(Filename)
                .EnableRaisingEvents = True
            End With
        End Sub

        Public Function GetStatus() As ProcessStatus
            Return Status
        End Function

        Public Function GetConsoleDataHistory() As String
            Dim output As New StringBuilder()
            For Each item In ConsoleHistory
                output.AppendLine(item)
            Next
            Return output.ToString
        End Function

        ''' <summary>
        ''' Adds the given text to the console log.
        ''' </summary>
        ''' <param name="Line"></param>
        Protected Sub LogInConsole(Line As String)
            RaiseEvent ConsoleDataRecieved(Me, New ConsoleDataRecievedEventArgs(Line))
            ConsoleHistory.Enqueue(Line)
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    If _p IsNot Nothing Then
                        _p.Dispose()
                    End If
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
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
End Namespace
