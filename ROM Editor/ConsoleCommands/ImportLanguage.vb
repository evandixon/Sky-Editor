Imports ROMEditor.FileFormats.Explorers
Imports SkyEditorBase

Namespace ConsoleCommands
    Public Class ImportLanguage
        Inherits ConsoleCommandAsync

        Public Overrides Async Function MainAsync(Arguments() As String) As Task
            Dim LanguageStringPath = Arguments(0)
            Dim formatRegex As New Text.RegularExpressions.Regex("\[.+\]")
            Dim ls As New LanguageString
            Await ls.OpenFile(LanguageStringPath, New SkyEditor.Core.Windows.IOProvider)
            Dim languagechar As String = IO.Path.GetFileNameWithoutExtension(LanguageStringPath).Replace("text_", "")
            Dim language As String
            Select Case languagechar
                Case "e"
                    language = "English"
                Case "f"
                    language = "Français"
                Case "s"
                    language = "Español"
                Case "g"
                    language = "Deutsche"
                Case "i"
                    language = "Italiano"
                Case "j"
                    language = "日本語"
                Case Else
                    Console.WriteLine("Unrecognized language character :" & languagechar)
                    Console.WriteLine("Please type the name of the language this file corresponds to:")
                    language = Console.ReadLine
            End Select

            'Import Pokemon
            Dim PokemonLines As New List(Of String)
            For count = 0 To LanguageString.PokemonNameLength - 1
                PokemonLines.Add(count.ToString & "=" & formatRegex.Replace(ls.GetPokemonName(count), ""))
            Next
            Dim pkmFile = PluginHelper.GetResourceName(language & "/SkyPokemon.txt", "SkyEditor")
            If Not IO.Directory.Exists(IO.Path.GetDirectoryName(pkmFile)) Then
                IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(pkmFile))
            End If
            IO.File.WriteAllLines(pkmFile, PokemonLines.ToList)
            Console.WriteLine("Saved Pokemon.")

            'Import Items
            Dim ItemLines As New List(Of String)
            For count = 0 To LanguageString.ItemLength - 1
                ItemLines.Add(count.ToString & "=" & formatRegex.Replace(ls.GetItemName(count), ""))
            Next
            Dim itemFile = PluginHelper.GetResourceName(language & "/SkyItems.txt", "SkyEditor")
            If Not IO.Directory.Exists(IO.Path.GetDirectoryName(itemFile)) Then
                IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(itemFile))
            End If
            IO.File.WriteAllLines(itemFile, ItemLines.ToList)
            Console.WriteLine("Saved Items.")

            'Import Moves
            Dim MoveLines As New List(Of String)
            For count = 0 To LanguageString.MoveLength - 1
                MoveLines.Add(count.ToString & "=" & formatRegex.Replace(ls.GetMoveName(count), ""))
            Next
            Dim moveFile = PluginHelper.GetResourceName(language & "/SkyMoves.txt", "SkyEditor")
            If Not IO.Directory.Exists(IO.Path.GetDirectoryName(moveFile)) Then
                IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(moveFile))
            End If
            IO.File.WriteAllLines(moveFile, MoveLines.ToList)
            Console.WriteLine("Saved Moves.")

            'Import Locations
            Dim LocationLines As New List(Of String)
            For count = 0 To LanguageString.LocationLength - 1
                LocationLines.Add(count.ToString & "=" & formatRegex.Replace(ls.GetLocationName(count), ""))
            Next
            Dim locFile = PluginHelper.GetResourceName(language & "/SkyLocations.txt", "SkyEditor")
            If Not IO.Directory.Exists(IO.Path.GetDirectoryName(locFile)) Then
                IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(locFile))
            End If
            IO.File.WriteAllLines(locFile, LocationLines.ToList)
            Console.WriteLine("Saved Locations.")

            Console.WriteLine("Done!")
        End Function
    End Class

End Namespace
