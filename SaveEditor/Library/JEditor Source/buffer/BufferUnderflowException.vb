Imports System

Namespace skyjed.buffer
    <Serializable> _
    Public Class BufferUnderflowException
        Inherits Exception

        Public Sub New()
            MyBase.New("Buffer Underflow")
        End Sub

        Public Sub New(ByVal msg As String)
            MyBase.New(msg)
        End Sub

    End Class

End Namespace