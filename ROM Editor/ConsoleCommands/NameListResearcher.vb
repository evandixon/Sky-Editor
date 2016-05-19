Imports ROMEditor.FileFormats.PSMD
Imports SkyEditor.Core.ConsoleCommands
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Windows
Imports SkyEditorBase

Namespace ConsoleCommands
    Public Class NameListResearcher
        Inherits ConsoleCommandAsync

        Private Function GetHash(Search As String, File As MessageBin) As MessageBinStringEntry
            Return (From s In File.Strings Where s.Entry = Search).First
        End Function

        Public Overrides Async Function MainAsync(Arguments() As String) As Task
            If Arguments.Length > 0 Then
                If IO.File.Exists(Arguments(0)) Then
                    Dim output As New Text.StringBuilder
                    Dim nameOutput As New Text.StringBuilder
                    Dim msg As New GenericFile()
                    Dim msg2 As New MessageBin
                    Await msg.OpenFile(Arguments(0), CurrentPluginManager.CurrentIOProvider)
                    Await msg2.OpenFile(Arguments(0), CurrentPluginManager.CurrentIOProvider)
                    Dim position = &HD0BA
                    For count = 0 To 2000
                        Dim s = msg.ReadNullTerminatedString(position, Text.Encoding.Unicode)
                        Console.WriteLine(s)
                        Dim entry = (From ent In msg2.Strings Where ent.Pointer = position).First
                        output.AppendLine(entry.HashSigned)
                        nameOutput.AppendLine(s)
                        position += s.Length * 2 + 2
                    Next
                    'IO.File.WriteAllText("PSMD Dungeon Name Hashes.txt", output.ToString)
                    'IO.File.WriteAllText("PSMD Dungeon BGM Names.txt", nameOutput.ToString)
                    IO.File.WriteAllText("PSMD Item Name Hashes.txt", output.ToString)
                    IO.File.WriteAllText("PSMD Item Names.txt", nameOutput.ToString)
                    Console.Write("Done.")
                Else
                    Console.WriteLine("File doesn't exist")
                End If
            Else
                Console.WriteLine("Usage: NameListResearcher <filename>")
            End If
        End Function
    End Class
End Namespace

