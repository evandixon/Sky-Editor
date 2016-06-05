Namespace Processes
    Public Class StatusChangedEventArgs
        Inherits EventArgs
        Public Property OldStatus As ProcessStatus
        Public Property NewStatus As ProcessStatus
        Public Sub New(OldStatus As ProcessStatus, NewStatus As ProcessStatus)
            Me.OldStatus = OldStatus
            Me.NewStatus = NewStatus
        End Sub
    End Class
End Namespace
