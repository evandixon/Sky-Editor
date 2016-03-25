Public Class FriendCodeManager

    'Credit to the following people:
    'http://www.caitsith2.com/ds/fc.php
    'SCV
    'damio

    Private InputVar As ULong
    Private magic As UInt32
    Private CRC_Table(255) As Byte
    Private game_code() As UInt32 = New UInt32() {&H41504100, &H41444100, &H43505500}
    Private lang() As Byte = New Byte() {&H44, &H45, &H46, &H49, &H4A, &H4B}

    Public Function ValidateFC(ByVal FriendCode As ULong) As Boolean
        InputVar = FriendCode
        If validate_code(game_code(1) Or lang(4)) Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Function validate_code(ByVal magic) As Boolean
        Dim crc_data(7) As UShort
        crc_data(0) = InputVar >> 0 And &HFF
        crc_data(1) = InputVar >> 8 And &HFF
        crc_data(2) = InputVar >> 16 And &HFF
        crc_data(3) = InputVar >> 24 And &HFF
        crc_data(4) = magic >> 0 And &HFF
        crc_data(5) = magic >> 8 And &HFF
        crc_data(6) = magic >> 16 And &HFF
        crc_data(7) = magic >> 24 And &HFF

        Return (InputVar >> 32 = (calc_crc(0, crc_data, 8)) And &H7F)

    End Function

    Private Function calc_crc(ByVal initial As UShort, ByVal data() As UShort, ByVal length As UShort) As UShort
        Dim crc As UShort = initial
        For i As UInt32 = 0 To length - 1
            crc = crc Xor data(i)
            crc = crc And &HFF
            crc = CRC_Table(crc)
        Next
        Return crc
    End Function

    Private Sub gen_crc()
        Dim crctab As UInt32
        For i As UInt32 = 0 To 255
            crctab = i
            For j As UInt32 = 0 To 7
                If crctab And &H80 Then
                    crctab = crctab << 1
                    crctab = crctab Xor 7
                Else
                    crctab = crctab << 1
                End If
            Next
            CRC_Table(i) = crctab And &HFF
        Next
    End Sub

    Public Sub New()
        gen_crc()
        magic = game_code(1) Or lang(4)
    End Sub

    Public Function FC_Checksum(ByVal data() As Byte) As Byte
        Dim crc_data(7) As UShort
        Dim _IPV(7) As Byte
        Array.Copy(data, 0, _IPV, 0, 4)
        InputVar = BitConverter.ToUInt64(_IPV, 0)
        crc_data(0) = InputVar >> 0 And &HFF
        crc_data(1) = InputVar >> 8 And &HFF
        crc_data(2) = InputVar >> 16 And &HFF
        crc_data(3) = InputVar >> 24 And &HFF
        crc_data(4) = magic >> 0 And &HFF
        crc_data(5) = magic >> 8 And &HFF
        crc_data(6) = magic >> 16 And &HFF
        crc_data(7) = magic >> 24 And &HFF
        Return (calc_crc(0, crc_data, 8)) And &H7F
    End Function

    Public Function FC_Checksum(ByVal data As UInt32) As Byte
        Return FC_Checksum(BitConverter.GetBytes(data))
    End Function

    Public Function GetFC(ByVal data As UInt32) As UInt64
        If data = 0UI Then Return 0UI
        Dim fcdata() As Byte = BitConverter.GetBytes(data)
        Dim fcOut(7) As Byte
        Array.Copy(fcdata, 0, fcOut, 0, 4)
        Dim FCM As New FriendCodeManager
        fcOut(4) = FCM.FC_Checksum(fcdata)
        Return BitConverter.ToUInt64(fcOut, 0)
    End Function

    Public Function GetFC(ByVal data() As Byte) As UInt64
        Return GetFC(BitConverter.ToUInt64(data, 0))
    End Function

End Class