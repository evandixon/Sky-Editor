Public Class Binary
    Public Property Bits As List(Of Boolean)
    Public Property Position As Integer
    Public Sub New()
        Bits = New List(Of Boolean)
        Position = 0
    End Sub
    Public Sub New(RawData As Byte())
        Bits = New List(Of Boolean)
        Position = 0
        For Each item In RawData
            For j As Integer = 0 To 7
                Bits.Add(((item >> j) And 1) <> 0)
            Next j
        Next
    End Sub
    Public Property Int(ByteIndex As Integer, BitIndex As Integer, BitLength As Integer) As Integer
        Get
            Dim output As Integer = 0
            For j As Integer = 0 To BitLength - 1
                output = output Or (If(Bits(ByteIndex * 8 + BitIndex + j), 1, 0)) << j
            Next j
            Return output
        End Get
        Set(value As Integer)
            Dim bin As New Binary(BitConverter.GetBytes(value))
            For count As Integer = 0 To bin.Bits.Count - 1
                Me.Bits(ByteIndex * 8 + BitIndex + count) = bin.Bits(count)
            Next
        End Set
    End Property
    Public Property Int(BitLength As Integer) As Integer
        Get
            Dim output As Integer = 0
            For j As Integer = 0 To BitLength - 1
                output = output Or (If(Bits(Position + j), 1, 0)) << j
            Next j
            Position += BitLength
            Return output
        End Get
        Set(value As Integer)
            Dim bin As New Binary(BitConverter.GetBytes(value))
            For count As Integer = 0 To bin.Bits.Count - 1
                Me.Bits(Position + count) = bin.Bits(count)
            Next
            Position += BitLength
        End Set
    End Property
    Public Function ToByteArray() As Byte()
        Dim output As New List(Of Byte)
        For count As Integer = 0 To Bits.Count - 1 Step 8
            If Bits.Count - 1 - count >= 8 Then
                output.Add(Int(0, count, 8))
            Else
                Exit For
            End If
        Next
        Return output.ToArray
    End Function
End Class
