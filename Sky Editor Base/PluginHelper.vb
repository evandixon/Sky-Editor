Imports System.Reflection
Imports System.Threading.Tasks
Imports System.Runtime.CompilerServices
Imports System.Deployment.Application
Imports System.Resources
Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.IO

''' <summary>
''' A collection of methods that are useful to Sky Editor plugins.
''' </summary>
''' <remarks></remarks>
Public Class PluginHelper

#Region "Program Running"
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sendingProcess"></param>
    ''' <param name="outLine"></param>
    ''' <remarks>Posted by brendan at http://stackoverflow.com/questions/9996709/read-console-process-output</remarks>
    Private Shared Sub OutputHandler(sendingProcess As Object, outLine As DataReceivedEventArgs)
        ' Collect the sort command output.
        If Not String.IsNullOrEmpty(outLine.Data) Then
            PluginHelper.Writeline(outLine.Data, LineType.ConsoleOutput)
        End If
    End Sub

    ''' <summary>
    ''' Runs the specified program, capturing console output, and waits for it to complete.
    ''' </summary>
    ''' <param name="Filename">Filename of the executable to run.</param>
    ''' <param name="Arguments">Arguments to pass to the process.</param>
    ''' <remarks></remarks>
    Public Shared Async Function RunProgram(Filename As String, Arguments As String, Optional ShowLoadingWindow As Boolean = True) As Task
        Writeline(String.Format(My.Resources.Language.ExecutingProgram, Filename, Arguments))
        'Set up the process
        Dim p As New Process()
        p.StartInfo.FileName = Filename
        p.StartInfo.Arguments = Arguments
        p.StartInfo.RedirectStandardOutput = True
        p.StartInfo.UseShellExecute = False
        p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
        p.StartInfo.CreateNoWindow = True
        p.StartInfo.WorkingDirectory = IO.Path.GetDirectoryName(Filename)
        AddHandler p.OutputDataReceived, AddressOf OutputHandler

        'Start the process
        p.Start()
        p.BeginOutputReadLine()

        'Wait for the process to close
        If ShowLoadingWindow Then
            SetLoadingStatus(String.Format(My.Resources.Language.WaitingOnProgram, IO.Path.GetFileName(Filename)))
            Await WaitForProcess(p)
            SetLoadingStatusFinished()
        Else
            Await WaitForProcess(p)
        End If

        'Clean up
        RemoveHandler p.OutputDataReceived, AddressOf OutputHandler
        p.Dispose()
        Writeline(String.Format(My.Resources.Language.ProgramFinished, p.StartInfo.FileName))
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

    ''' <summary>
    ''' Runs the specified program without waiting for it to complete.
    ''' </summary>
    ''' <param name="Filename">Filename of the executable to run.</param>
    ''' <param name="Arguments">Arguments to pass to the process.</param>
    ''' <remarks></remarks>
    Public Shared Sub RunProgramInBackground(Filename As String, Arguments As String)
        Writeline(String.Format(My.Resources.Language.ExecutingProgramAsync, Filename, Arguments))
        Dim p As New Process()
        p.StartInfo.FileName = Filename
        p.StartInfo.Arguments = Arguments
        p.Start()
    End Sub
#End Region

#Region "Loading"
    Public Class LoadingMessageChangedEventArgs
        Inherits EventArgs
        Public Property NewMessage As String
        Public Property Progress As Single
        Public Property IsIndeterminate As Boolean
        Public Sub New()
            MyBase.New()
            Progress = 1
            IsIndeterminate = True
        End Sub
        Public Sub New(Message As String)
            Me.New()
            NewMessage = Message
        End Sub
        Public Sub New(Message As String, Progress As Single)
            Me.New(Message)
            Me.Progress = Progress
            Me.IsIndeterminate = False
        End Sub
    End Class
    Private Shared _loadingShown As Boolean = False
    Private Shared _loadingDefinitions As New Dictionary(Of String, LoadingMessageChangedEventArgs)

    Public Shared Event LoadingMessageChanged(sender As Object, e As LoadingMessageChangedEventArgs)
    Public Shared Sub SetLoadingStatus(Message As String, Progress As Single)
        RaiseEvent LoadingMessageChanged(Nothing, New LoadingMessageChangedEventArgs(Message, Progress))
    End Sub
    Public Shared Sub SetLoadingStatus(Message As String)
        RaiseEvent LoadingMessageChanged(Nothing, New LoadingMessageChangedEventArgs(Message))
    End Sub
    Public Shared Sub SetLoadingStatusFinished()
        RaiseEvent LoadingMessageChanged(Nothing, New LoadingMessageChangedEventArgs(My.Resources.Language.Ready, 1))
    End Sub
    Public Shared Sub SetLoadingStatusFailed()
        SetLoadingStatus(My.Resources.Language.Failed, 0)
    End Sub
#End Region

#Region "Console Writeline"
    Public Class ConsoleLineWrittenEventArgs
        Inherits EventArgs
        Public Property Line As String
        Public Property Type As LineType
        Public Property CallerName As String
    End Class
    Public Enum LineType
        Message = 1
        Warning = 2
        [Error] = 3
        ConsoleOutput = 4
    End Enum
    Public Shared Sub Writeline(Line As String, Optional type As LineType = LineType.Message, <CallerMemberName> Optional CallerName As String = Nothing)
        Console.WriteLine(Line) 'String.Format("{1}", CallerName, Line)) 'Old format: "{0}: {1}"

        Dim e As New ConsoleLineWrittenEventArgs
        e.Line = Line
        e.Type = type
        e.CallerName = CallerName
        RaiseEvent ConsoleLineWritten(Nothing, e)
    End Sub
    Public Shared Event ConsoleLineWritten(sender As Object, e As ConsoleLineWrittenEventArgs)
#End Region

    ''' <summary>
    ''' Shows the application's underlying console to the user.
    ''' </summary>
    Public Shared Sub ShowConsole()
        Internal.ConsoleManager.Show()
    End Sub

    ''' <summary>
    ''' Casts the given object to type T, or returns its contained item if it implements the interface iContainer(Of T).
    ''' </summary>
    ''' <typeparam name="T">Type to cast to.</typeparam>
    ''' <param name="ObjectToCast">Object to cast.</param>
    ''' <returns></returns>
    Public Shared Function Cast(Of T)(ObjectToCast As Object) As T
        If TypeOf ObjectToCast Is T Then
            Return DirectCast(ObjectToCast, T)
        ElseIf TypeOf ObjectToCast Is IContainer(Of T) Then
            Return DirectCast(ObjectToCast, IContainer(Of T)).Item
        Else
            'I should probably throw my own exception here, since I'm casting EditingObject to T even though I just found that EditingObject is NOT T, but there will be an exception anyway
            Return DirectCast(ObjectToCast, T)
        End If
    End Function

    ''' <summary>
    ''' If ObjectToCast implements iContainer(Of T), sets iContainer(Of T).Item to NewValue.
    ''' Otherwise, sets ObjectToCast to NewValue.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="ObjectToCast"></param>
    ''' <param name="NewValue"></param>
    Public Shared Sub CastUpdate(Of T)(ByRef ObjectToCast As Object, ByVal NewValue As T)
        If TypeOf ObjectToCast Is T Then
            ObjectToCast = NewValue
        ElseIf TypeOf ObjectToCast Is IContainer(Of T) Then
            DirectCast(ObjectToCast, IContainer(Of T)).Item = NewValue
        Else
            ObjectToCast = NewValue
        End If
    End Sub
End Class