Namespace Utilities
    Public Class GenericArrayOperations(Of T)
        Public Shared Function CopyOfRange(ByteArr As T(), Index As Integer, EndPoint As Integer) As T()
            Dim output(Math.Max(Math.Min(EndPoint, ByteArr.Length) - Index, 0)) As T
            For x As Integer = 0 To output.Length - 1
                output(x) = ByteArr(x + Index)
            Next
            Return output
        End Function
    End Class
End Namespace