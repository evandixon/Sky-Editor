Module Module1

    Sub Main()
        Try
            'For converting text from the Exploreres language files
            Dim args() As String = Environment.GetCommandLineArgs
            If args.Length > 1 Then
                Dim input() As String = IO.File.ReadAllLines(args(1))
                Dim out As String = ""
                Dim StartInt As Integer = CInt(input(0).Split("|")(0)) ' - 1 'If 1st item should be 0, don't subtract 1
                For Each line In input
                    Dim parts() As String = line.Split("|")
                    out = out & (CInt(parts(0)) - StartInt) & "=" & parts(1) & vbCrLf
                Next
                IO.File.WriteAllText(IO.Path.Combine(Environment.CurrentDirectory, "Converted.txt"), out)
            End If

            'for converting text ripped directly from a ROM, specifically Blue rescue team.
            'Dim items = IO.File.ReadAllLines(IO.Path.Combine(Environment.CurrentDirectory, "TextFile3.txt"))
            'Dim out As String = ""
            'For count As Integer = 0 To items.Length - 1 Step 1
            '    out &= (count) & "=" & items(count) & vbCrLf
            'Next
            'IO.File.WriteAllText(IO.Path.Combine(Environment.CurrentDirectory, "RBMoves.txt"), out.Trim)
        Catch ex As Exception
            Console.WriteLine(ex.ToString)
        End Try
    End Sub

End Module
