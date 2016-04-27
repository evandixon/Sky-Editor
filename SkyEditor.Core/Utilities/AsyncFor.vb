Imports SkyEditor.Core.EventArguments

Namespace Utilities
    ''' <summary>
    ''' Runs a provided delegate function or sub repeatedly and asynchronously in the style of a For statement.
    ''' </summary>
    Public Class AsyncFor

        Public Delegate Sub ForItem(i As Integer)
        Public Delegate Sub ForEachItem(Of T)(i As T)
        Public Delegate Function ForEachItemAsync(Of T)(i As T) As Task
        Public Delegate Function ForItemAsync(i As Integer) As Task
        Public Event LoadingStatusChanged(sender As Object, e As LoadingStatusChangedEventArgs)

#Region "Constructors"
        Public Sub New()
            BatchSize = Integer.MaxValue
            SetLoadingStatus = False
            SetLoadingStatusOnFinish = False
            RunningTasks = New List(Of Task)
        End Sub
        Public Sub New(ProgressMessage As String)
            Me.New
            SetLoadingStatus = True
            SetLoadingStatusOnFinish = True
            Me.ProgressMessage = ProgressMessage
        End Sub
#End Region

#Region "Properties"
        ''' <summary>
        ''' Gets or sets whether or not progress will be reported to PluginHelper.SetLoadingStatus after each operation.
        ''' </summary>
        ''' <returns></returns>
        Public Property SetLoadingStatus As Boolean

        ''' <summary>
        ''' Gets or sets whether or not progress will be set to "Ready" once all operations are complete.
        ''' </summary>
        ''' <returns></returns>
        Public Property SetLoadingStatusOnFinish As Boolean

        ''' <summary>
        ''' Message template to set the loading status with.
        ''' Ex. Value of "Copying Files..." will be displayed as "Copying Files... (7 of 100)"
        ''' </summary>
        ''' <returns></returns>
        Public Property ProgressMessage As String

        ''' <summary>
        ''' Whether or not to run each task sequentially.
        ''' </summary>
        ''' <returns></returns>
        Public Property RunSynchronously As Boolean

        ''' <summary>
        ''' The number of tasks to run at once.
        ''' </summary>
        ''' <returns></returns>
        Public Property BatchSize As Integer

        ''' <summary>
        ''' The currently running tasks.
        ''' </summary>
        ''' <returns></returns>
        Private Property RunningTasks As List(Of Task)

        ''' <summary>
        ''' The total number of tasks to run.
        ''' </summary>
        ''' <returns></returns>
        Private Property TotalTasks As Integer

        ''' <summary>
        ''' The number of tasks that have been completed.
        ''' </summary>
        ''' <returns></returns>
        Private Property CompletedTasks As Integer
            Get
                Return _completedTasks
            End Get
            Set(value As Integer)
                _completedTasks = value
                RaiseEvent LoadingStatusChanged(Me, New LoadingStatusChangedEventArgs With {.Complete = (value = TotalTasks),
                                                .Completed = value,
                                                .Message = ProgressMessage,
                                                .Progress = If((TotalTasks > 0), (value / TotalTasks), 1),
                                                .Total = TotalTasks})
            End Set
        End Property
        Dim _completedTasks As Integer

#End Region

#Region "Core Functions"
        Public Async Function RunForEach(Of T)(DelegateFunction As ForEachItemAsync(Of T), Collection As IEnumerable(Of T)) As Task
            'Todo: throw exception if there's already tasks running
            Dim taskItemQueue As New Queue(Of T)
            For Each item In Collection
                taskItemQueue.Enqueue(item)
            Next

            TotalTasks = RunningTasks.Count

            'While there's either more tasks to start or while there's still tasks running
            While (taskItemQueue.Count > 0 OrElse (taskItemQueue.Count = 0 AndAlso RunningTasks.Count > 0))
                If RunningTasks.Count < BatchSize AndAlso taskItemQueue.Count > 0 Then
                    'We can run more tasks

                    'Get the next task item to run
                    Dim item = taskItemQueue.Dequeue 'The item in Collection to process

                    'Start the task
                    Dim tTask = DelegateFunction(item)

                    'Either wait for it or move on
                    If RunSynchronously Then
                        Await tTask
                        CompletedTasks += 1
                    Else
                        RunningTasks.Add(tTask)
                    End If
                Else
                    If RunningTasks.Count > 0 Then
                        'We can't start any more tasks, so we have to wait on one.
                        Await Task.WhenAny(RunningTasks)

                        'Remove completed tasks
                        For count = RunningTasks.Count - 1 To 0 Step -1
                            If RunningTasks(count).IsCompleted Then
                                CompletedTasks += 1
                                RunningTasks.RemoveAt(count)
                            End If
                        Next
                    Else
                        'We're finished.  Nothing else to do.
                        Exit While
                    End If
                End If
            End While
        End Function

        Public Async Function RunFor(DelegateFunction As ForItemAsync, StartValue As Integer, EndValue As Integer, Optional StepCount As Integer = 1) As Task
            'Todo: throw exception if there's already tasks running

            If StepCount = 0 Then
                Throw New ArgumentException(My.Resources.Language.ErrorAsyncForInfiniteLoop, NameOf(StepCount))
            End If

            'Find how many tasks there are to run
            'Ex. For i = 0 to 10 gives us 11 tasks
            TotalTasks = Math.Ceiling((EndValue - StartValue + 1) / StepCount)

            If TotalTasks < 0 Then
                'Then in a normal For statement, the body would never be called
                TotalTasks = 0
                CompletedTasks = 0
                Exit Function
            End If

            Dim i As Integer = StartValue

            Dim tasksRemaining As Integer = TotalTasks

            'While there's either more tasks to start or while there's still tasks running
            While (tasksRemaining > 0 OrElse (tasksRemaining = 0 AndAlso RunningTasks.Count > 0))
                If RunningTasks.Count < BatchSize AndAlso tasksRemaining > 0 Then
                    'We can run more tasks

                    'Get the next task item to run
                    Dim item = i 'The item in Collection to process
                    i += StepCount

                    'Start the task
                    Dim tTask = DelegateFunction(item)

                    'Either wait for it or move on
                    If RunSynchronously Then
                        Await tTask
                        CompletedTasks += 1
                        tasksRemaining -= 1
                    Else
                        RunningTasks.Add(tTask)
                    End If
                Else
                    If tasksRemaining > 0 Then
                        'We can't start any more tasks, so we have to wait on one.
                        Await Task.WhenAny(RunningTasks)

                        'Remove completed tasks
                        For count = RunningTasks.Count - 1 To 0 Step -1
                            If RunningTasks(count).IsCompleted Then
                                CompletedTasks += 1
                                tasksRemaining -= 1
                                RunningTasks.RemoveAt(count)
                            End If
                        Next
                    Else
                        'We're finished.  Nothing else to do.
                        Exit While
                    End If
                End If
            End While
        End Function

        Public Async Function RunFor(DelegateSub As ForItem, StartValue As Integer, EndValue As Integer, Optional StepCount As Integer = 1) As Task
            Await RunFor(Function(Count As Integer) As Task
                             DelegateSub(Count)
                             Return Task.FromResult(0)
                         End Function, StartValue, EndValue, StepCount)
        End Function

        Public Async Function RunForEach(Of T)(DelegateSub As ForEachItem(Of T), Collection As IEnumerable(Of T)) As Task
            Await RunForEach(Function(Item As T) As Task
                                 DelegateSub(Item)
                                 Return Task.FromResult(0)
                             End Function, Collection)
        End Function
#End Region

    End Class
End Namespace

