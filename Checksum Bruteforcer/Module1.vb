Module Module1
    'Public SeedTable As Integer()

    'Public Sub GetSeeds()
    '    SeedTable = New Integer(255) {}

    '    Dim index As Integer = 0
    '    Dim result As Integer = 0

    '    Do
    '        result = index << 8
    '        Dim index2 As Integer = 0
    '        Do
    '            If (CByte(result >> 8) And &H80) <> 0 Then
    '                result = (2 * result) Xor &H1021
    '            Else
    '                result *= 2
    '            End If
    '            index2 += 1
    '        Loop While index2 < 8 'System.Threading.Interlocked.Increment(index2) < 8

    '        SeedTable(index) = CUShort(result)
    '        index += 1
    '    Loop While index <= &HFF
    'End Sub


    ''The Diamond/Pearl way
    'Public Function GetCheckSum(data As Byte()) As UShort
    '    Dim sum As Integer = &HFFFF
    '    GetSeeds()

    '    For i As Integer = 0 To data.Length - 1
    '        sum = (sum << 8) Xor SeedTable(CByte(data(i) Xor CByte(sum >> 8)))
    '    Next

    '    Return CUShort(sum)
    'End Function

    'Function bsdChecksum(Data As Byte()) As UInteger
    '    Dim checksum As UInteger = 0
    '    For Each b In Data
    '        checksum = checksum >> 1 + ((checksum And 1) << 15)
    '        checksum += b
    '        checksum = checksum And &HFFFF
    '    Next
    '    Return checksum
    'End Function
    Delegate Function Checksum(Data As Byte(), StartIndex As Integer, EndIndex As Integer, Variable As Integer) As UInt16
    Function SumChecksum(Data As Byte(), StartIndex As Integer, EndIndex As Integer, Variable As Integer) As UInt16
        Dim sum As UInteger = 0
        For i As Integer = StartIndex To EndIndex - 1 Step 2
            sum += BitConverter.ToUInt16(Data, i)
            sum = sum And &HFFFF
        Next
        Return CUShort(sum)
    End Function
    Function SumChecksum32Xor(Data As Byte(), StartIndex As Integer, EndIndex As Integer, Variable As Integer) As UInt16
        Dim sum As ULong = 0
        For i As Integer = StartIndex To EndIndex - 3 Step 4
            sum += BitConverter.ToUInt32(Data, i)
            sum = sum And &HFFFFFFFF
        Next
        Return (sum And &HFFFF) 'Xor (sum >> 16 And &HFFFF)
    End Function
    Function SumSubtract(Data As Byte(), StartIndex As Integer, EndIndex As Integer, Variable As Integer) As UInt16
        Dim sum As UInteger = 0
        For i As Integer = StartIndex To EndIndex - 1 Step 2
            sum += Data(i)
            sum -= Data(i + 1)
        Next
        Return CUShort(sum)
    End Function
    Function BitSum(Data As Byte(), StartIndex As Integer, EndIndex As Integer, variable As Integer)
        Dim sum As UInteger = 0
        For i = StartIndex To EndIndex
            Dim b8 As New SkyEditorBase.Utilities.Bits8(Data(i))
            If b8.Bit1 Then
                sum += 1
            End If
            If b8.Bit2 Then
                sum += 1
            End If
            If b8.Bit3 Then
                sum += 1
            End If
            If b8.Bit4 Then
                sum += 1
            End If
            If b8.Bit5 Then
                sum += 1
            End If
            If b8.Bit6 Then
                sum += 1
            End If
            If b8.Bit7 Then
                sum += 1
            End If
            If b8.Bit8 Then
                sum += 1
            End If
        Next
        Return sum
    End Function
    Sub Main()
        Dim data As Byte() = IO.File.ReadAllBytes("1.game_data")
        Dim methods As Checksum() = {AddressOf BitSum}
        Console.WriteLine(Conversion.Hex(BitSum(data, 0, data.Length - 2, 0)).ToUpper)
        Console.ReadLine()
        'For Each method In methods
        '    For start As Integer = 0 To 11
        '        For last As Integer = data.Length - 1 To &H6986 Step -1
        '            Dim sum = method.Invoke(data, start, last, 0)
        '            Dim sbytes1 = BitConverter.GetBytes(sum)
        '            If sbytes1(0) = 222 Then
        '                Console.WriteLine(Match found)
        '            Else
        '                Console.WriteLine(No match:  & start &  to  & last & , using method & method.ToString)
        '            End If
        '            'Dim sbytes1 = BitConverter.GetBytes(sum)
        '            'If sbytes1(0) = &H78 AndAlso sbytes1(1) = 3 Then
        '            '    Console.WriteLine(Match found)
        '            'Else
        '            '    Console.WriteLine(No match:  & start &  to  & last & , using method & method.ToString)
        '            'End If
        '        Next
        '    Next
        'Next










        'For start As Integer = 0 To data.Length - 1
        '    For last As Integer = data.Length - 1 To start Step -1
        '        Dim bytes = SkyEditorBase.Utilities.genericarrayoperations(Of Byte).CopyOfRange(data, start, last)
        '        If start < last Then
        '            Dim c As New CRC16
        '            Dim sum1 As Integer = 0
        '            Dim sum2 As Long = 0
        '            Dim t As Boolean = True
        '            For i As Integer = start To last - 3 Step 2
        '                ' sum1 += BitConverter.ToUInt16(bytes, i)
        '                'sum1 = sum1 Xor BitConverter.ToUInt16(bytes, i)
        '                'If t Then
        '                '    On Error Resume Next
        '                '    sum2 += BitConverter.ToUInt32(bytes, i)
        '                '    sum2 = sum2 And &HFFFF
        '                'End If
        '                't = Not t
        '            Next
        '            Dim sbytes1 = BitConverter.GetBytes(sum1)
        '            'Dim sbytes2 = BitConverter.GetBytes(sum2)
        '            Dim sbytes2 = c.ComputeHash(bytes, start, last - start)
        '            'Dim sbytes2 = BitConverter.GetBytes(GetCheckSum(bytes))
        '            If sbytes1(0) = &H68 AndAlso sbytes1(1) = 3 Then
        '                Console.WriteLine(Match found!)
        '                Console.WriteLine(start &  to  & last)
        '                Console.WriteLine()
        '            ElseIf sbytes2(0) = &H78 AndAlso sbytes2(1) = 3 Then
        '                Console.WriteLine(Match found!)
        '                Console.WriteLine(start &  to  & last)
        '                Console.WriteLine()
        '            Else
        '                Console.WriteLine(No match:  & start &  to  & last)
        '            End If
        '        End If
        '    Next
        'Next

    End Sub

End Module