Imports System.Threading.Tasks
Imports System.Threading

Public NotInheritable Class AsyncHelpers
    Private Sub New()
    End Sub
    ''' <summary>
    ''' Execute's an async Task(Of T) method which has a void return value synchronously
    ''' </summary>
    ''' <param name="task">Task(Of T) method to execute</param>


    Public Shared Sub RunSync(task As Func(Of Task))
        Dim oldContext = SynchronizationContext.Current
        Dim synch = New ExclusiveSynchronizationContext()
        SynchronizationContext.SetSynchronizationContext(synch)
        synch.Post(Async Sub()
                       Try
                           Await task()
                       Catch e As Exception
                           synch.InnerException = e
                           Throw
                       Finally
                           synch.EndMessageLoop()
                       End Try

                   End Sub, Nothing)
        synch.BeginMessageLoop()

        SynchronizationContext.SetSynchronizationContext(oldContext)
	End Sub

    ''' <summary>
    ''' Execute's an async Task(Of T) method which has a T return type synchronously
    ''' </summary>
    ''' <typeparam name="T">Return Type</typeparam>
    ''' <param name="task">Task(Of T) method to execute</param>
    ''' <returns></returns>
    Public Shared Function RunSync(Of T)(task As Func(Of Task(Of T))) As T
        Dim oldContext = SynchronizationContext.Current
        Dim synch = New ExclusiveSynchronizationContext()
        SynchronizationContext.SetSynchronizationContext(synch)
        Dim ret As T = Nothing
        synch.Post(Async Sub()
                       Try
                           ret = Await task()
                       Catch e As Exception
                           synch.InnerException = e
                           Throw
                       Finally
                           synch.EndMessageLoop()
                       End Try

                   End Sub, Nothing)
        synch.BeginMessageLoop()
        SynchronizationContext.SetSynchronizationContext(oldContext)
        Return ret
	End Function
    Private Class ExclusiveSynchronizationContext
        Inherits SynchronizationContext
        Private done As Boolean
        Public Property InnerException() As Exception
            Get
                Return m_InnerException
            End Get
            Set(value As Exception)
                m_InnerException = value
            End Set
        End Property
        Private m_InnerException As Exception
        ReadOnly workItemsWaiting As New AutoResetEvent(False)
        ReadOnly items As New Queue(Of Tuple(Of SendOrPostCallback, Object))()

        Public Overrides Sub Send(d As SendOrPostCallback, state As Object)
            Throw New NotSupportedException("We cannot send to our same thread")
        End Sub

        Public Overrides Sub Post(d As SendOrPostCallback, state As Object)
            SyncLock items
                items.Enqueue(Tuple.Create(d, state))
            End SyncLock
            workItemsWaiting.[Set]()
        End Sub

        Public Sub EndMessageLoop()
            Post(Function() InlineAssignHelper(done, True), Nothing)
		End Sub

        Public Sub BeginMessageLoop()
            While Not done
                Dim task As Tuple(Of SendOrPostCallback, Object) = Nothing
                SyncLock items
                    If items.Count > 0 Then
                        task = items.Dequeue()
                    End If
                End SyncLock
                If task IsNot Nothing Then
                    task.Item1.Invoke(task.Item2)
                    If InnerException IsNot Nothing Then
                        ' the method threw an exeption
                        Throw New AggregateException("AsyncHelpers.Run method threw an exception.", InnerException)
                    End If
                Else
                    workItemsWaiting.WaitOne()
                End If
            End While
        End Sub

        Public Overrides Function CreateCopy() As SynchronizationContext
            Return Me
        End Function
        Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
            target = value
            Return value
        End Function
    End Class
End Class

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================
