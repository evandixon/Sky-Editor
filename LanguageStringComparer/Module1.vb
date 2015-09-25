Module Module1

    Sub Main()
        Console.WriteLine("Loading...")
        Dim us As New ROMEditor.FileFormats.LanguageString("text_e US.str")
        Dim eu As New ROMEditor.FileFormats.LanguageString("text_e.str")

        Dim removals As New List(Of Integer)
        Dim changes As New List(Of Integer)

        Dim i As Integer = 0
        Dim j As Integer = 0

        While i < us.Items.Count AndAlso j < eu.Items.Count
            If us.Item(i) = eu.Item(j) Then
                'Then do nothing, go to the next one.
                Console.WriteLine("Match: " & us.Item(i))
                i += 1
                j += 1
            ElseIf j = 3938 OrElse j = 3939 'Removal
                Console.WriteLine("Known Removal")
                removals.Add(j)
                j += 1

            ElseIf i < 17598 'Change
                Console.WriteLine("Change: " & us(i))
                changes.Add(j)
                i += 1
                j += 1
            ElseIf 17598 < i AndAlso i < 17781 'Staff credits, which are ignored for now.
                Console.WriteLine("Ignoring staff credits")
                i += 1
                j += 1
            ElseIf i = 17781 'First item outside staff credits
                Console.WriteLine("Exit staff credtis")
                j = 17812
                'We know it's a match.
            Else
                Console.WriteLine()
                Console.WriteLine()
                Console.WriteLine("At " & i)
                Console.WriteLine("US Text: " & us.Item(i))
                Console.WriteLine()
                Console.WriteLine("At " & j)
                Console.WriteLine("EU Text: " & eu.Item(j))
                Console.WriteLine()
                Console.WriteLine("Is this a regional change, or a removal? (Type: c or r)")
                Dim action = Console.ReadKey
                If action.Key = ConsoleKey.C Then
                    changes.Add(j)
                    i += 1
                    j += 1
                Else
                    removals.Add(j)
                    j += 1
                End If

            End If

        End While

    End Sub

End Module
