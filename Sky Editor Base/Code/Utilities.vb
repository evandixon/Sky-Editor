Imports System.Runtime.CompilerServices
Public Class Bits8
    Public Property Value As Byte
#Region "Bits 1-8"
    Public Property Bit1 As Boolean
        Get
            Return ((Value Or 254) - 254) > 0
        End Get
        Set(value As Boolean)
            Dim bit As Byte = 0
            If value Then bit = 1
            Me.Value = ((Me.Value Or 1) - 1) + (bit * 1)
        End Set
    End Property
    Public Property Bit2 As Boolean
        Get
            Return ((Value Or 253) - 253) > 0
        End Get
        Set(value As Boolean)
            Dim bit As Byte = 0
            If value Then bit = 1
            Me.Value = ((Me.Value Or 2) - 2) + (bit * 2)
        End Set
    End Property
    ''' <summary>
    ''' Bit that adds 4 to the value
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Bit3 As Boolean
        Get
            Return ((Value Or 251) - 251) > 0
        End Get
        Set(value As Boolean)
            Dim bit As Byte = 0
            If value Then bit = 1
            Me.Value = ((Me.Value Or 4) - 4) + (bit * 4)
        End Set
    End Property
    ''' <summary>
    ''' Bit that adds 8 to the value
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Bit4 As Boolean
        Get
            Return ((Value Or 247) - 247) > 0
        End Get
        Set(value As Boolean)
            Dim bit As Byte = 0
            If value Then bit = 1
            Me.Value = ((Me.Value Or 8) - 8) + (bit * 8)
        End Set
    End Property
    ''' <summary>
    ''' Bit that adds 16 to the value
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Bit5 As Boolean
        Get
            Return ((Value Or 239) - 239) > 0
        End Get
        Set(value As Boolean)
            Dim bit As Byte = 0
            If value Then bit = 1
            Me.Value = ((Me.Value Or 16) - 16) + (bit * 16)
        End Set
    End Property
    ''' <summary>
    ''' Bit that adds 32 to the value
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Bit6 As Boolean
        Get
            Return ((Value Or 223) - 223) > 0
        End Get
        Set(value As Boolean)
            Dim bit As Byte = 0
            If value Then bit = 1
            Me.Value = ((Me.Value Or 32) - 32) + (bit * 32)
        End Set
    End Property
    ''' <summary>
    ''' Bit that adds 64 to the value
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Bit7 As Boolean
        Get
            Return ((Value Or 191) - 191) > 0
        End Get
        Set(value As Boolean)
            Dim bit As Byte = 0
            If value Then bit = 1
            Me.Value = ((Me.Value Or 64) - 64) + (bit * 64)
        End Set
    End Property
    ''' <summary>
    ''' Bit that adds 128 to the value
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Bit8 As Boolean
        Get
            Return ((Value Or 127) - 127) > 0
        End Get
        Set(value As Boolean)
            Dim bit As Byte = 0
            If value Then bit = 1
            Me.Value = ((Me.Value Or 128) - 128) + (bit * 128)
        End Set
    End Property
#End Region
    ''' <summary>
    ''' Bit order: 87654321
    ''' So, FromBits(False,False,True,False,False,False,False,False)=4
    ''' </summary>
    ''' <param name="Bit1"></param>
    ''' <param name="Bit2"></param>
    ''' <param name="Bit3"></param>
    ''' <param name="Bit4"></param>
    ''' <param name="Bit5"></param>
    ''' <param name="Bit6"></param>
    ''' <param name="Bit7"></param>
    ''' <param name="Bit8"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function FromBits(Bit1 As Boolean, Bit2 As Boolean, Bit3 As Boolean, Bit4 As Boolean, Bit5 As Boolean, Bit6 As Boolean, Bit7 As Boolean, Bit8 As Boolean) As Bits8
        Dim out As New Bits8(0)
        If Bit1 Then out += 1
        If Bit2 Then out += 2
        If Bit3 Then out += 4
        If Bit4 Then out += 8
        If Bit5 Then out += 16
        If Bit6 Then out += 32
        If Bit7 Then out += 64
        If Bit8 Then out += 128
        Return out
    End Function
    ''' <summary>
    ''' Returns a Bit8 from an array of byte
    ''' </summary>
    ''' <param name="Binary">Byte array to get value from</param>
    ''' <param name="ByteIndex">Index of the byte to get the value from</param>
    ''' <param name="BitIndex">Index of the bit (0 to 7) to start getting the value from.  If the binary is 11110000 00001111, a BitIndex of 2 would return 11000000.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function FromBinary(Binary As Byte(), ByteIndex As Long, BitIndex As Long) As Bits8
        Dim output As Bits8 = Nothing
        If BitIndex > 7 Then
            ByteIndex += Math.Floor(BitIndex \ 8)
            BitIndex = BitIndex Mod 8
        End If
        Dim byte1 As Bits8 = Binary(ByteIndex)
        Dim byte2 As Bits8 = 0
        If Binary.Length > ByteIndex + 1 Then
            byte2 = Binary(ByteIndex + 1)
        End If
        Select Case BitIndex
            Case 0
                Dim out As Bits8 = 0
                If byte1.Bit1 Then out += 1
                If byte1.Bit2 Then out += 2
                If byte1.Bit3 Then out += 4
                If byte1.Bit4 Then out += 8
                If byte1.Bit5 Then out += 16
                If byte1.Bit6 Then out += 32
                If byte1.Bit7 Then out += 64
                If byte1.Bit8 Then out += 128
                output = out
            Case 1
                Dim out As Bits8 = 0
                If byte2.Bit8 Then out += 1
                If byte1.Bit1 Then out += 2
                If byte1.Bit2 Then out += 4
                If byte1.Bit3 Then out += 8
                If byte1.Bit4 Then out += 16
                If byte1.Bit5 Then out += 32
                If byte1.Bit6 Then out += 64
                If byte1.Bit7 Then out += 128
                output = out
            Case 2
                Dim out As Bits8 = 0
                If byte2.Bit7 Then out += 1
                If byte2.Bit8 Then out += 2
                If byte1.Bit1 Then out += 4
                If byte1.Bit2 Then out += 8
                If byte1.Bit3 Then out += 16
                If byte1.Bit4 Then out += 32
                If byte1.Bit5 Then out += 64
                If byte1.Bit6 Then out += 128
                output = out
            Case 3
                Dim out As Bits8 = 0
                If byte2.Bit6 Then out += 1
                If byte2.Bit7 Then out += 2
                If byte2.Bit8 Then out += 4
                If byte1.Bit1 Then out += 8
                If byte1.Bit2 Then out += 16
                If byte1.Bit3 Then out += 32
                If byte1.Bit4 Then out += 64
                If byte1.Bit5 Then out += 128
                output = out
            Case 4
                Dim out As Bits8 = 0
                If byte2.Bit5 Then out += 1
                If byte2.Bit6 Then out += 2
                If byte2.Bit7 Then out += 4
                If byte2.Bit8 Then out += 8
                If byte1.Bit1 Then out += 16
                If byte1.Bit2 Then out += 32
                If byte1.Bit3 Then out += 64
                If byte1.Bit4 Then out += 128
                output = out
            Case 5
                Dim out As Bits8 = 0
                If byte2.Bit4 Then out += 1
                If byte2.Bit5 Then out += 2
                If byte2.Bit6 Then out += 4
                If byte2.Bit7 Then out += 8
                If byte2.Bit8 Then out += 16
                If byte1.Bit1 Then out += 32
                If byte1.Bit2 Then out += 64
                If byte1.Bit3 Then out += 128
                output = out
            Case 6
                Dim out As Bits8 = 0
                If byte2.Bit3 Then out += 1
                If byte2.Bit4 Then out += 2
                If byte2.Bit5 Then out += 4
                If byte2.Bit6 Then out += 8
                If byte2.Bit7 Then out += 16
                If byte2.Bit8 Then out += 32
                If byte1.Bit1 Then out += 64
                If byte1.Bit2 Then out += 128
                output = out
            Case 7
                Dim out As Bits8 = 0
                If byte2.Bit2 Then out += 1
                If byte2.Bit3 Then out += 2
                If byte2.Bit4 Then out += 4
                If byte2.Bit5 Then out += 8
                If byte2.Bit6 Then out += 16
                If byte2.Bit7 Then out += 32
                If byte2.Bit8 Then out += 64
                If byte1.Bit1 Then out += 128
                output = out
                'Case Else
                '    Throw New ArgumentException("BitIndex must be a value from 0 to 7.")
        End Select
        Return output
    End Function
    Public Shared Function FromBinary(Binary As Byte(), ByteIndex As Long, BitIndex As Long, ByteCount As Long) As Byte()
        Dim out(ByteCount - 1) As Byte
        For count As Integer = 0 To ByteCount - 1
            out(count) = FromBinary(Binary, ByteIndex + count, BitIndex)
        Next
        Return out
    End Function

    Public Sub New(Value As Byte)
        Me.Value = Value
    End Sub
#Region "Operators"
    Public Shared Narrowing Operator CType(ByVal x As Byte) As Bits8
        Return New Bits8(x)
    End Operator
    Public Shared Widening Operator CType(ByVal x As Bits8) As Byte
        Return x.Value
    End Operator
    Public Shared Operator +(ByVal x As Bits8, ByVal y As Bits8) As Bits8
        Return x.Value + y.Value
    End Operator
    Public Shared Operator =(ByVal x As Bits8, ByVal y As Byte) As Boolean
        Return x.Value = y
    End Operator
    Public Shared Operator <>(ByVal x As Bits8, ByVal y As Byte) As Boolean
        Return x.Value <> y
    End Operator
#End Region

    Public Overrides Function ToString() As String
        Return Value.ToString
    End Function
End Class

Module ByteExtensions
    <Extension()> _
    Public Function Bit1(ByVal aByte As Byte) As Boolean
        Return CType(aByte, Bits8).Bit1
    End Function
    <Extension()> _
    Public Function Bit2(ByVal aByte As Byte) As Boolean
        Return CType(aByte, Bits8).Bit2
    End Function
    <Extension()> _
    Public Function Bit3(ByVal aByte As Byte) As Boolean
        Return CType(aByte, Bits8).Bit3
    End Function
    <Extension()> _
    Public Function Bit4(ByVal aByte As Byte) As Boolean
        Return CType(aByte, Bits8).Bit4
    End Function
    <Extension()> _
    Public Function Bit5(ByVal aByte As Byte) As Boolean
        Return CType(aByte, Bits8).Bit5
    End Function
    <Extension()> _
    Public Function Bit6(ByVal aByte As Byte) As Boolean
        Return CType(aByte, Bits8).Bit6
    End Function
    <Extension()> _
    Public Function Bit7(ByVal aByte As Byte) As Boolean
        Return CType(aByte, Bits8).Bit7
    End Function
    <Extension()> _
    Public Function Bit8(ByVal aByte As Byte) As Boolean
        Return CType(aByte, Bits8).Bit8
    End Function
End Module
Public Class GenericArrayOperations(Of T)
    Public Shared Function CopyOfRange(ByteArr As T(), Index As Integer, EndPoint As Integer) As T()
        Dim output(Math.Max(Math.Min(EndPoint, ByteArr.Length) - Index, 0)) As T
        For x As Integer = 0 To output.Length - 1
            output(x) = ByteArr(x + Index)
        Next
        Return output
    End Function
End Class
Public Class BitOperations
    ''' <summary>
    ''' flakey.  Use ShiftLeftPMD()
    ''' </summary>
    ''' <param name="Binary"></param>
    ''' <param name="startbit"></param>
    ''' <param name="bits"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Obsolete("Use ShiftLeftPMD() instead.")>
    Public Shared Function EncodeBytes(ByVal Binary As Byte(), startbit As Integer, bits As Integer) As Byte()
        Dim input As Byte() = Binary
        Array.Reverse(input)

        Dim src As New IO.MemoryStream(input)
        Dim res As New IO.MemoryStream(bits / 8)
        Dim srcbit As Integer = startbit
        Dim resbit As Integer = 0
        Dim a As Byte = If(srcbit = 0, CByte(0), CByte(src.ReadByte() >> srcbit))
        Dim b As Byte = 0
        For count As Integer = bits To 0 Step -1
            b <<= 1
            'Check to see if Bit8 of a is 1
            If (a And &H80) = &H80 Then
                'Set Bit1 of b to 1
                b = b Or 1 '&H80
            End If
            a <<= 1
            srcbit += 1
            'If current bit position is along byte lines
            If srcbit = 8 Then
                'Load next byte into a
                a = CByte(src.ReadByte())
                'Reset bit counter
                srcbit = 0
            End If
            resbit += 1
            'If b contains one byte worth of data, output it
            If resbit = 8 Then
                resbit = 0
                res.WriteByte(b)
            End If
        Next
        Dim out = res.ToArray

        Array.Reverse(out)
        Return out
    End Function
    ''' <summary>
    ''' Shifts the bits in an array of bytes to the left.
    ''' </summary>
    ''' <param name="bytes">The byte array to shift.</param>
    Public Shared Function ShiftLeft(ByRef bytes As Byte()) As Boolean
        Dim leftMostCarryFlag As Boolean = False

        ' Iterate through the elements of the array from left to right.
        For index As Integer = 0 To bytes.Length - 1
            ' If the leftmost bit of the current byte is 1 then we have a carry.
            Dim carryFlag As Boolean = (bytes(index) And &H80) > 0

            If index > 0 Then
                If carryFlag = True Then
                    ' Apply the carry to the rightmost bit of the current bytes neighbor to the left.
                    bytes(index - 1) = CByte(bytes(index - 1) Or &H1)
                End If
            Else
                leftMostCarryFlag = carryFlag
            End If

            bytes(index) = CByte(bytes(index) << 1)
        Next

        Return leftMostCarryFlag
    End Function

    ''' <summary>
    ''' Shifts the bits in an array of bytes to the right.
    ''' </summary>
    ''' <param name="bytes">The byte array to shift.</param>
    Public Shared Function ShiftRight(ByRef bytes As Byte()) As Boolean
        Dim rightMostCarryFlag As Boolean = False
        Dim rightEnd As Integer = bytes.Length - 1

        ' Iterate through the elements of the array right to left.
        For index As Integer = rightEnd To 0 Step -1
            ' If the rightmost bit of the current byte is 1 then we have a carry.
            Dim carryFlag As Boolean = (bytes(index) And &H1) > 0

            If index < rightEnd Then
                If carryFlag = True Then
                    ' Apply the carry to the leftmost bit of the current bytes neighbor to the right.
                    bytes(index + 1) = CByte(bytes(index + 1) Or &H80)
                End If
            Else
                rightMostCarryFlag = carryFlag
            End If

            bytes(index) = CByte(bytes(index) >> 1)
        Next

        Return rightMostCarryFlag
    End Function
    Public Shared Function ShiftRightPMD(ByVal bytes As Byte(), bits As Integer, StartIndex As Integer, ByteCount As Integer) As Byte()
        If bits = 0 Then
            Return GenericArrayOperations(Of Byte).CopyOfRange(bytes, StartIndex, StartIndex + ByteCount - 1)
        Else
            Dim out As Byte() = GenericArrayOperations(Of Byte).CopyOfRange(bytes, StartIndex, StartIndex + ByteCount - 1)
            For count As Integer = 0 To ByteCount - 1
                Dim nextByte As Byte = 0
                If count + StartIndex < bytes.Length - 1 Then
                    nextByte = bytes(StartIndex + count + 1)
                End If
                Dim t As Byte() = {nextByte, out(count)}
                For x As Integer = 1 To bits
                    ShiftRight(t)
                Next
                out(count) = t(1)
            Next
            Return out
        End If
    End Function
    Public Shared Function ShiftLeftPMD(ByVal bytes As Byte(), bits As Integer, StartIndex As Integer, ByteCount As Integer) As Byte()
        If bits = 0 Then Return bytes
        Dim out As Byte() = GenericArrayOperations(Of Byte).CopyOfRange(bytes, StartIndex, StartIndex + ByteCount - 1)
        Dim carry As Byte = out(0)
        For count As Integer = 1 To ByteCount - 1
            Dim t As Byte() = {out(count), carry}
            For x As Integer = 1 To bits
                ShiftLeft(t)
            Next
            carry = out(count)
            out(count) = t(0)
        Next
        Return out
    End Function
End Class
