Namespace EventArguments
    Public Class FileOpenedEventArguments
        Inherits EventArgs
        Public Property File As Object
        Public Property ProjectPath As String
        Public Property DisposeOnExit As Boolean
        Public Sub New()
            DisposeOnExit = False
        End Sub
    End Class

End Namespace