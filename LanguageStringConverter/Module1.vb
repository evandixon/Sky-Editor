Module Module1

    Sub Main()
        Dim filename As String = "C:\Users\Evan\OneDrive\Documents\Saves\Save File Research\Mystery Dungeon Sky items list\text_f.str"
        Dim text As String = ""
        Using l As New ROMEditor.FileFormats.LanguageString(filename)
            For count As Integer = 6775 To 8126
                text &= (count - 6775).ToString & "=" & l(count) & vbCrLf
            Next
        End Using
        IO.File.WriteAllText("Items.txt", text.Trim)
    End Sub

End Module
