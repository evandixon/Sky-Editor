Namespace skyjed.buffer

    Public Class BooleanBufferArray
        Inherits BooleanBuffer

        Private __array() As Boolean

        Public Sub New(ByVal array() As Boolean)
            MyBase.New(array.Length)
            __array = array
        End Sub

        Protected Friend Overrides Function aget(ByVal index As Integer) As Boolean
            Return __array(index)
        End Function

        Protected Friend Overrides Sub aput(ByVal index As Integer, ByVal b As Boolean)
            __array(index) = b
        End Sub

        Public Function GetSplitBytes() As Boolean()
            Return __array
        End Function

    End Class

End Namespace