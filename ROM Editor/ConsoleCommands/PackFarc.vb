Imports ROMEditor.FileFormats.PSMD
Imports SkyEditor.Core.ConsoleCommands
Imports SkyEditorBase

Namespace ConsoleCommands
    Public Class PackFarc
        Inherits ConsoleCommandAsync

        Public Overrides Async Function MainAsync(Arguments() As String) As Task
            If Arguments.Count > 1 Then
                If IO.Directory.Exists(Arguments(0)) Then
                    Await FarcF5.Pack(Arguments(0), Arguments(1), CurrentPluginManager.CurrentIOProvider)
                Else
                    Console.WriteLine("Directory does not exist: " & Arguments(0))
                End If
            Else
                Console.WriteLine("Usage: PackFarc <Input Directory> <Output Filename>")
            End If
        End Function
    End Class

End Namespace
