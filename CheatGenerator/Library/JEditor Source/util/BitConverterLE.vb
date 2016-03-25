Imports System

Namespace skyjed.util

    Public Class BitConverterLE

        Public Shared Function readInt32(ByVal buf() As Byte, ByVal [off] As Integer) As Integer
            Dim b0 As Integer = buf([off] + 0) And &HFF
            Dim b1 As Integer = buf([off] + 1) And &HFF
            Dim b2 As Integer = buf([off] + 2) And &HFF
            Dim b3 As Integer = buf([off] + 3) And &HFF
            Return b0 << 0 Or b1 << 8 Or b2 << 16 Or b3 << 24
        End Function

        Public Shared Sub writeInt32(ByVal val As Integer, ByVal buf() As Byte, ByVal [off] As Integer)
            buf([off] + 0) = CByte(val >> 0)
            buf([off] + 1) = CByte(val >> 8)
            buf([off] + 2) = CByte(val >> 16)
            buf([off] + 3) = CByte(val >> 24)
        End Sub

        Public Shared Function packBits(ByVal src() As Boolean) As Byte()
            Dim dest((src.Length \ 8) - 1) As Byte
            For i As Integer = 0 To dest.Length - 1
                dest(i) = 0
                For j As Integer = 0 To 7
                    dest(i) = dest(i) Or (If(src(i * 8 + j), 1, 0)) << j
                Next j
            Next i
            Return dest
        End Function

        Public Shared Function splitBits(ByVal src() As Byte) As Boolean()
            Dim dest((src.Length * 8) - 1) As Boolean
            For i As Integer = 0 To src.Length - 1
                For j As Integer = 0 To 7
                    dest(i * 8 + j) = ((src(i) >> j) And 1) <> 0
                Next j
            Next i
            Return dest
        End Function

        Public Shared Function packBitsInt(ByVal src() As Boolean, ByVal n As Integer) As Integer()
            Dim dest((src.Length \ n) - 1) As Integer
            For i As Integer = 0 To dest.Length - 1
                dest(i) = 0
                For j As Integer = 0 To n - 1
                    dest(i) = dest(i) Or (If(src(i * n + j), 1, 0)) << j
                Next j
            Next i
            Return dest
        End Function

        Public Shared Function packBitsInt(ByVal src() As Boolean) As Integer
            Return packBitsInt(src, src.Length)(0)
        End Function

        Public Shared Function splitBitsInt(ByVal src() As Integer, ByVal n As Integer) As Boolean()
            Dim dest((src.Length * n) - 1) As Boolean
            For i As Integer = 0 To src.Length - 1
                For j As Integer = 0 To n - 1
                    dest(i * n + j) = ((src(i) >> j) And 1) <> 0
                Next j
            Next i
            Return dest
        End Function

        Public Shared Function splitBitsInt(ByVal src As Integer, ByVal n As Integer) As Boolean()
            Return splitBitsInt(New Integer() {src}, n)
        End Function

        Public Shared Function packBitsLong(ByVal src() As Boolean, ByVal n As Integer) As Long()
            Dim dest((src.Length \ n) - 1) As Long
            For i As Integer = 0 To dest.Length - 1
                dest(i) = 0
                For j As Integer = 0 To n - 1
                    dest(i) = dest(i) Or CLng(Math.Truncate(If(src(i * n + j), 1, 0))) << j
                Next j
            Next i
            Return dest
        End Function

        Public Shared Function packBitsLong(ByVal src() As Boolean) As Long
            Return packBitsLong(src, src.Length)(0)
        End Function

        Public Shared Function splitBitsLong(ByVal src() As Long, ByVal n As Integer) As Boolean()
            Dim dest((src.Length * n) - 1) As Boolean
            For i As Integer = 0 To src.Length - 1
                For j As Integer = 0 To n - 1
                    dest(i * n + j) = ((src(i) >> j) And 1) <> 0
                Next j
            Next i
            Return dest
        End Function

        Public Shared Function splitBitsLong(ByVal src As Long, ByVal n As Integer) As Boolean()
            Return splitBitsLong(New Long() {src}, n)
        End Function

    End Class

End Namespace