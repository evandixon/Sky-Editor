Namespace UI
    Public Class HeaderUpdatedEventArgs
        Inherits EventArgs
        Public Property OldValue As String
        Public Property NewValue As String
        Public Sub New()
            MyBase.New
        End Sub
        Public Sub New(OldValue As String, NewValue As String)
            MyBase.New
            Me.OldValue = OldValue
            Me.NewValue = NewValue
        End Sub
    End Class
End Namespace

