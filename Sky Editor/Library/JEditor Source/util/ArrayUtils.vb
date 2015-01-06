Imports SkyEditorBase
Imports SkyEditorBase.Utilities
Namespace skyjed.util


    Public Class ArrayUtils

        Public Shared Sub copyInto(ByVal dest() As Integer, ByVal src() As Integer, ByVal [off] As Integer)
            For i As Integer = 0 To src.Length - 1
                dest([off] + i) = src(i)
            Next i
        End Sub

        Public Shared Sub copyInto(ByVal dest() As Char, ByVal src() As Char, ByVal [off] As Integer)
            For i As Integer = 0 To src.Length - 1
                dest([off] + i) = src(i)
            Next i
        End Sub

        Public Shared Sub copyInto(ByVal dest() As Byte, ByVal src() As Byte, ByVal [off] As Integer)
            For i As Integer = 0 To src.Length - 1
                dest([off] + i) = src(i)
            Next i
        End Sub

        Public Shared Sub copyInto(ByVal dest() As Boolean, ByVal src() As Boolean, ByVal [off] As Integer)
            For i As Integer = 0 To src.Length - 1
                dest([off] + i) = src(i)
            Next i
        End Sub

        Public Shared Function copyOfRange(ByVal src() As Integer, ByVal [off] As Integer, ByVal len As Integer) As Integer()
            'Return Arrays.copyOfRange(src, [off], [off] + len)
            Return GenericArrayOperations(Of Integer).CopyOfRange(src, [off], [off] + len - 1)
            ' Return BitOperations.SubByteArr(src, [off], [off] + len)
        End Function

        Public Shared Function copyOfRange(ByVal src() As Char, ByVal [off] As Integer, ByVal len As Integer) As Char()
            'Return Arrays.copyOfRange(src, [off], [off]+len)
            Return GenericArrayOperations(Of Char).CopyOfRange(src, [off], [off] + len - 1)
        End Function

        Public Shared Function copyOfRange(ByVal src() As Byte, ByVal [off] As Integer, ByVal len As Integer) As Byte()
            'Return Arrays.copyOfRange(src, [off], [off] + len)
            Return GenericArrayOperations(Of Byte).CopyOfRange(src, [off], [off] + len - 1)
        End Function

        Public Shared Function copyOfRange(ByVal src() As Boolean, ByVal [off] As Integer, ByVal len As Integer) As Boolean()
            'Return Arrays.copyOfRange(src, [off], [off]+len)
            Return GenericArrayOperations(Of Boolean).CopyOfRange(src, [off], [off] + len - 1)
        End Function

    End Class

End Namespace