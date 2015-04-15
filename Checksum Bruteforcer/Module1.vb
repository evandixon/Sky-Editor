Module Module1

    Sub Main()
        Dim bytes As Byte() = IO.File.ReadAllBytes("1.game_data")
        For start As Integer = 0 To bytes.Length - 1
            For last As Integer = bytes.Length - 1 To 0 Step -1
                If start < last Then
                    Dim c As New CRC16
                    Dim sum1 As Integer = 0
                    Dim sum2 As Long = 0
                    Dim t As Boolean = True
                    For i As Integer = start To last - 3 Step 2
                        ' sum1 += BitConverter.ToUInt16(bytes, i)
                        sum1 = sum1 Xor BitConverter.ToUInt16(bytes, i)
                        If t Then sum2 += BitConverter.ToUInt32(bytes, i)
                        t = Not t
                    Next
                    Dim sbytes1 = BitConverter.GetBytes(sum1)
                    'Dim sbytes2 = BitConverter.GetBytes(sum2)
                    Dim sbytes2 = c.ComputeHash(bytes, start, last - start)
                    If sbytes1(0) = &H68 AndAlso sbytes1(1) = 3 Then
                        Console.WriteLine("Match found!")
                        Console.WriteLine(start & " to " & last)
                        Console.WriteLine()
                    ElseIf sbytes2(0) = &H68 AndAlso sbytes2(1) = 3 Then
                        Console.WriteLine("Match found!")
                        Console.WriteLine(start & " to " & last)
                        Console.WriteLine()
                    Else
                        Console.WriteLine("No match: " & start & " to " & last)
                    End If
                End If
            Next
        Next
    End Sub

End Module
