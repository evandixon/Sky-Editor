Module Module1

    Sub Main()
        Dim _rawdata = IO.File.ReadAllBytes("C:\Users\Evan\Desktop\Pokemon Mystery Dungeon - Red Rescue Team.sav")
        Dim success As Integer = 0
        'Dim _rawdata2 = IO.File.ReadAllBytes("C:\Users\Evan\Desktop\Pokemon Mystery Dungeon - Blue Rescue Team.dsv")
        For first As Integer = 4 To &H6000 Step 4
            For last As Integer = 24549 To 4 Step -4
                Console.Clear()
                Console.WriteLine("Testing range {0} to {1}", first, last)
                'Console.WriteLine("{0} percent.", ((24549 - last) / 24594) * 100)
                Console.WriteLine()
                Console.WriteLine("{0} successes.", success)
                Dim words As New List(Of UInt32)
                For count As Integer = first To last Step 4
                    words.Add(BitConverter.ToUInt32(_rawdata, count))
                Next
                Dim sum As UInt64 = 0
                For Each item In words
                    sum += item
                Next
                'Dim words2 As New List(Of UInt32)
                'For count As Integer = first To last Step 4
                '    words2.Add(BitConverter.ToUInt32(_rawdata, count))
                'Next
                'Dim sum2 As UInt64 = 0
                'For Each item In words
                '    sum2 += item
                'Next
                Dim buffer() As Byte = BitConverter.GetBytes(sum)
                If buffer(0) = _rawdata(&H420) AndAlso buffer(0) = _rawdata(&H421) AndAlso buffer(0) = _rawdata(&H422) AndAlso buffer(0) = _rawdata(&H423) Then
                    IO.File.WriteAllText(IO.Path.Combine(Environment.CurrentDirectory, "redChecksum2-" & first & "-" & last), "")
                    Console.WriteLine("Found range {0} to {1}", first, last)
                    success += 1
                End If
            Next
        Next
        Console.ReadKey()
        'Dim b() As Byte = {0, 255, 0, 0, 0} '{45, 67, 23}
        ''Array.Reverse(b)
        ''Console.WriteLine(Convert.ToString(b(2), 2).PadLeft(8, "0") & " " & Convert.ToString(b(1), 2).PadLeft(8, "0") & " " & Convert.ToString(b(0), 2).PadLeft(8, "0"))
        ''Array.Reverse(b)
        ''Console.WriteLine(Convert.ToString(b(2), 2).PadLeft(8, "0") & " " & Convert.ToString(b(1), 2).PadLeft(8, "0") & " " & Convert.ToString(b(0), 2).PadLeft(8, "0"))
        ''Array.Reverse(b)
        'For x As Integer = 1 To 10
        '    Dim t() As Byte = b.Clone
        '    Array.Reverse(t)
        '    Array.Resize(t, 4)
        '    Array.Reverse(t)
        '    t = SkyEditor.BitOperations.EncodeBytes(t, x, 24)
        '    Console.WriteLine("Orig:" & Convert.ToString(t(0), 2).PadLeft(8, "0") & " " & Convert.ToString(t(1), 2).PadLeft(8, "0"))
        '    t = b.Clone
        '    t = SkyEditor.BitOperations.ShiftLeftPMD(t, x, 0, 4)
        '    Console.WriteLine("New :" & Convert.ToString(t(0), 2).PadLeft(8, "0") & " " & Convert.ToString(t(1), 2).PadLeft(8, "0"))
        'Next
        'Console.ReadKey()
    End Sub

End Module
