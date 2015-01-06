Namespace Utilities
    Public Class BitOperations
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
End Namespace
