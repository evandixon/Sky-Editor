Imports System.Threading.Tasks
Namespace Utilities
    ''' <summary>
    ''' Runs each item in a For statement asynchronously.
    ''' </summary>
    Public Class AsyncFor

        Public Delegate Sub ForItem(i As Integer)
        Public Delegate Sub ForEachItem(Of T)(i As T)

        Public Sub New(ProgressMessage As String)
            SetLoadingStatus = True
            SetLoadingStatusOnFinish = True
            Me.ProgressMessage = ProgressMessage
        End Sub
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

        Public Async Function RunFor(DelegateSub As ForItem, StartValue As Integer, EndValue As Integer, StepCount As Integer) As Task
            Dim tasks As New List(Of Task)
            _opMax = (EndValue - StartValue) / StepCount
            For i As Integer = StartValue To EndValue Step StepCount
                Dim i2 = i 'Needed because we're running a lambda in a for statement.
                Dim t As New Task(New Action(Sub()
                                                 DelegateSub(i2)
                                                 OperationsCompleted += 1
                                             End Sub))
                t.Start()
                tasks.Add(t)
            Next
            Await Task.Run(New Action(Sub()
                                          Task.WaitAll(tasks.ToArray)
                                      End Sub))
        End Function

        Public Async Function RunForEach(Of T)(DelegateSub As ForEachItem(Of T), Collection As IEnumerable(Of T)) As Task
            Dim tasks As New List(Of Task)
            _opMax = Collection.Count
            For Each item In Collection
                Dim item2 = item 'Needed because we're running a lambda in a for statement.
                Dim tTask As New Task(New Action(Sub()
                                                     DelegateSub(item2)
                                                     OperationsCompleted += 1
                                                 End Sub))
                tTask.Start()
                tasks.Add(tTask)
            Next
            Await Task.Run(New Action(Sub()
                                          Task.WaitAll(tasks.ToArray)
                                      End Sub))
        End Function

        Private Property OperationsCompleted As Integer
            Get
                Return _opCompleted
            End Get
            Set(value As Integer)
                _opCompleted = value
                ReportProgress(value, _opMax)
            End Set
        End Property
        Private _opCompleted As Integer
        Private _opMax As Integer
        Private Sub ReportProgress(Completed As Integer, Max As Integer)
            If Completed < Max Then
                If SetLoadingStatus Then PluginHelper.SetLoadingStatus(String.Format(PluginHelper.GetLanguageItem("CopyingFilesStatus", "{0} ({1} of {2})"), ProgressMessage, Completed, Max), Completed / Max)
            Else
                If SetLoadingStatusOnFinish Then PluginHelper.SetLoadingStatusFinished()
            End If
        End Sub
    End Class
End Namespace

