Imports System.Text

Public Class Binary
    Public Property Bits As List(Of Boolean)
    Public Property Position As Integer
    Public Sub New()
        Bits = New List(Of Boolean)
        Position = 0
    End Sub
    Public Sub New(Length As Integer)
        Bits = New List(Of Boolean)
        For i = 0 To Length - 1
            Bits.Add(0)
        Next
    End Sub
    Public Sub New(Bits As Binary)
        Me.Bits = New List(Of Boolean)
        For Each item In Bits.Bits
            Me.Bits.Add(item)
        Next
    End Sub
    Public Sub New(Bits As Boolean())
        Me.Bits = New List(Of Boolean)(Bits)
    End Sub
    Public Sub New(RawData As IEnumerable(Of Byte))
        Bits = New List(Of Boolean)
        Position = 0
        For Each item In RawData
            For j As Integer = 0 To 7
                Bits.Add(((item >> j) And 1) <> 0)
            Next
        Next
    End Sub
    Public ReadOnly Property Count As Integer
        Get
            Return Bits.Count
        End Get
    End Property
    Default Public Property Bit(Index As Integer) As Boolean
        Get
            Return Bits(Index)
        End Get
        Set(value As Boolean)
            Bits(Index) = value
        End Set
    End Property
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
            For i = 0 To BitLength - 1
                Bits(ByteIndex * 8 + BitIndex + i) = bin.Bits(i)
            Next
        End Set
    End Property
    Public Property UInt(ByteIndex As Integer, BitIndex As Integer, BitLength As Integer) As UInteger
        Get
            Dim output As UInteger = 0
            For j As UInteger = 0 To BitLength - 1
                output = output Or (If(Bits(ByteIndex * 8 + BitIndex + j), CUInt(1), CUInt(0))) << j
            Next j
            Return output
        End Get
        Set(value As UInteger)
            Dim bin As New Binary(BitConverter.GetBytes(value))
            For i = 0 To BitLength - 1
                Bits(ByteIndex * 8 + BitIndex + i) = bin.Bits(i)
            Next
        End Set
    End Property
    Public Property NextInt(BitLength As Integer) As Integer
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
            For i = 0 To BitLength - 1
                Me.Bits(Position + i) = bin.Bits(i)
            Next
            Position += BitLength
        End Set
    End Property
    Public Property StringPMD(ByteIndex As Integer, BitIndex As Integer, ByteLength As Integer) As String
        Get
            Dim s As New StringBuilder
            For i = 0 To ByteLength - 1
                Dim c = Lists.StringEncoding(Int(ByteIndex + i, BitIndex, 8))
                If Not c = vbNullChar Then
                    s.Append(c)
                Else
                    Exit For
                End If
            Next
            Return s.ToString
        End Get
        Set(value As String)
            For i = 0 To ByteLength - 1
                If value.Length > i Then
                    Int(ByteIndex + i, BitIndex, 8) = Lists.StringEncodingInverse(value(i))
                Else
                    Int(ByteIndex + i, BitIndex, 8) = 0
                End If
            Next
        End Set
    End Property
    Public Property Range(Index As Integer, Length As Integer) As Binary
        Get
            Dim buffer(Length - 1) As Boolean
            Me.Bits.CopyTo(Index, buffer, 0, Length)
            Return New Binary(buffer)
        End Get
        Set(value As Binary)
            For i = 0 To Length - 1
                Me.Bits(i + Index) = value.Bits(i)
            Next
        End Set
    End Property
    Public Sub AppendByte(ByteToAppend As Byte)
        For j As Integer = 0 To 7
            Bits.Add(((ByteToAppend >> j) And 1) <> 0)
        Next
    End Sub
    Public Function ToByteArray() As Byte()
        Dim output As New List(Of Byte)
        For i = 0 To Bits.Count - 1 Step 8
            If Bits.Count - 1 - i >= 8 Then
                output.Add(Int(0, i, 8))
            Else
                Exit For
            End If
        Next
        Return output.ToArray
    End Function
End Class