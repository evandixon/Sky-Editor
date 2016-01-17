Namespace skyjed.buffer

    Public Class BooleanBufferView
        Inherits BooleanBuffer

        Private __buf As BooleanBuffer
        Private __offset As Integer

        Public Sub New(ByVal buf As BooleanBuffer, ByVal offset As Integer, ByVal length As Integer)
            MyBase.New(length)
            __buf = buf
            __offset = offset
        End Sub

        Protected Friend Overrides Function aget(ByVal index As Integer) As Boolean
            Return __buf.aget(__offset + index)
        End Function

        Protected Friend Overrides Sub aput(ByVal index As Integer, ByVal b As Boolean)
            __buf.aput(__offset + index, b)
        End Sub

    End Class

End Namespace