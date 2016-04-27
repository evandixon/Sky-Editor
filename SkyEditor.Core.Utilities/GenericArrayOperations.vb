Namespace Utilities
    Public Class GenericArrayOperations(Of T)
        Public Shared Function CopyOfRange(Array As IEnumerable(Of T), Index As Integer, EndPoint As Integer) As T()
            Dim output(Math.Max(Math.Min(EndPoint, Array.Count) - Index, 0)) As T
            For x As Integer = 0 To output.Length - 1
                output(x) = Array(x + Index)
            Next
            Return output
        End Function
        Public Shared Function ArraysEqual(Array1 As T(), Array2 As T())
            If Array1.Length = Array2.Length Then
                Dim equal = True
                For count As Integer = 0 To Array1.Length - 1
                    equal = (Array1(count).Equals(Array2(count)))
                    If Not equal Then
                        Exit For
                    End If
                Next
                Return equal
            Else
                Return False
            End If
        End Function
    End Class
End Namespace