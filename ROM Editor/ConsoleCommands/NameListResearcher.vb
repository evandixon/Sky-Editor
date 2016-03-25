Imports SkyEditorBase

Namespace ConsoleCommands
    Public Class NameListResearcher
        Inherits ConsoleCommandAsync

        Private Function GetHash(Search As String, File As FileFormats.MessageBin) As FileFormats.MessageBinStringEntry
            Return (From s In File.Strings Where s.Entry = Search).First
        End Function

        Public Overrides Function MainAsync(Arguments() As String) As Task
            If Arguments.Length > 0 Then
                If IO.File.Exists(Arguments(0)) Then
                    Dim output As New Text.StringBuilder
                    Dim msg As New GenericFile
                    Dim msg2 As New FileFormats.MessageBin
                    msg.OpenFile(Arguments(0))
                    msg2.OpenFile(Arguments(0))
                    Dim position = &H4D2A
                    For count = 0 To 980
                        Dim s = msg.ReadNullTerminatedString(position, Text.Encoding.Unicode)
                        Console.WriteLine(s)
                        Dim entry = (From ent In msg2.Strings Where ent.Pointer = position).First
                        output.AppendLine(entry.HashSigned)
                        position += s.Length * 2 + 2
                    Next
                    IO.File.WriteAllText("PSMD Pokemon Name Hashes.txt", output.ToString)
                    Console.Write("Done.")
                Else
                    Console.WriteLine("File doesn't exist")
                End If
            Else
                Console.WriteLine("Usage: NameListResearcher <filename>")
            End If
            Return Task.CompletedTask
        End Function
    End Class
End Namespace

