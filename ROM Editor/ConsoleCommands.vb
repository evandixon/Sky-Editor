Imports SkyEditorBase

Public Class ConsoleCommands
    Public Shared Sub ImportSkyLanguageString(Manager As PluginManager, LanguageStringPath As String)
        Dim formatRegex As New Text.RegularExpressions.Regex("\[.+\]")
        Dim ls As New FileFormats.LanguageString(LanguageStringPath)
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
        For count = 0 To FileFormats.LanguageString.PokemonNameLength - 1
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
        For count = 0 To FileFormats.LanguageString.ItemLength - 1
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
        For count = 0 To FileFormats.LanguageString.MoveLength - 1
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
        For count = 0 To FileFormats.LanguageString.LocationLength - 1
            LocationLines.Add(count.ToString & "=" & formatRegex.Replace(ls.GetLocationName(count), ""))
        Next
        Dim locFile = PluginHelper.GetResourceName(language & "/SkyLocations.txt", "SkyEditor")
        If Not IO.Directory.Exists(IO.Path.GetDirectoryName(locFile)) Then
            IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(locFile))
        End If
        IO.File.WriteAllLines(locFile, LocationLines.ToList)
        Console.WriteLine("Saved Locations.")

        Console.WriteLine("Done!")
    End Sub

    Public Shared Async Function CreatePSMDSoundtrack(Manager As PluginManager, Arguments As String) As Task
        'If current BaseRom is a PSMD Rom
        If (Manager.CurrentProject IsNot Nothing AndAlso TypeOf Manager.CurrentProject Is Generic3DSModProject AndAlso Manager.CurrentProject.Files.ContainsKey("BaseRom.3ds") AndAlso TypeOf Manager.CurrentProject.Files("BaseRom.3ds") Is Roms.PSMDRom) OrElse Arguments.ToLower = "-override" Then
            Console.WriteLine("Starting conversion...")

            Dim sourceDir As String = IO.Path.Combine(IO.Path.GetDirectoryName(Manager.CurrentProject.Filename), "BaseRom RawFiles", "romfs", "sound", "stream")
            Dim destDir As String = IO.Path.Combine(IO.Path.GetDirectoryName(Manager.CurrentProject.Filename), "Soundtrack")

            'Todo: do error checks on input file
            Dim trackNames As New Dictionary(Of String, String)
            If IO.File.Exists(PluginHelper.GetResourceName("PSMD English Soundtrack.txt")) Then
                Dim lines = IO.File.ReadAllLines(PluginHelper.GetResourceName("PSMD English Soundtrack.txt"))
                For Each item In lines
                    Dim parts = item.Split("=".ToCharArray, 2)
                    If parts.Count = 2 Then
                        trackNames.Add(parts(0), parts(1))
                    End If
                Next
            End If
            Console.WriteLine("Track names: " & trackNames.Count)
            If trackNames.Count = 0 Then
                For Each item In IO.File.ReadAllLines(PluginHelper.GetResourceName("PSMD English Soundtrack.txt"))
                    Console.WriteLine(item)
                Next
            End If

            If Not IO.Directory.Exists(destDir) Then
                IO.Directory.CreateDirectory(destDir)
            End If

            For Each item In IO.Directory.GetFiles(destDir)
                IO.File.Delete(item)
            Next

            PluginHelper.SetLoadingStatus(PluginHelper.GetLanguageItem("Converting streams..."))

            Dim f As New SkyEditorBase.Utilities.AsyncFor(PluginHelper.GetLanguageItem("Converting streams..."))
            Await f.RunForEach(Async Function(Item As String) As Task
                                   Console.WriteLine("Converting " & Item & " to wav.")
                                   Dim source = IO.Path.Combine(sourceDir, Item) & ".dspadpcm.bcstm"

                                   'Create the wav
                                   Dim destinationWav = source.Replace(sourceDir, destDir).Replace("dspadpcm.bcstm", "wav")

                                   Dim filename = IO.Path.GetFileNameWithoutExtension(destinationWav)

                                   If trackNames.ContainsKey(filename) Then
                                       destinationWav = destinationWav.Replace(filename, trackNames(filename).Replace(":", "").Replace("é", "e"))
                                   End If

                                   For Each c In "!?,".ToCharArray
                                       destinationWav = destinationWav.Replace(c, "")
                                   Next

                                   Dim destinationMp3 = destinationWav.Replace(".wav", ".mp3")

                                   Await vgmstream.RunVGMStream(source, destinationWav)

                                   'Convert to mp3
                                   Console.WriteLine("Converting " & Item & " to mp3.")
                                   Await ffmpeg.ConvertToMp3(destinationWav, destinationMp3)

                                   IO.File.Delete(destinationWav)

                                   Console.WriteLine("Tagging " & Item)

                                   ' Dim m = IdSharp.AudioInfo.AudioFile.Create(destinationMp3, True)
                                   Dim t As New IdSharp.Tagging.ID3v2.ID3v2Tag(destinationMp3)
                                   t.Album = "Pokémon Super Mystery Dungeon"
                                   t.Artist = "Chunsoft"
                                   t.Year = 2015
                                   Dim filenameParts = trackNames(filename).Split(" ".ToCharArray, 2)
                                   If filenameParts.Count = 2 Then
                                       If IsNumeric(filenameParts(0)) Then
                                           t.TrackNumber = CInt(filenameParts(0))
                                       End If

                                       t.Title = filenameParts(1)
                                   End If
                                   t.Save(destinationMp3)
                               End Function, trackNames.Keys)

            Console.WriteLine("Conversion complete!")
        Else
            Console.WriteLine("Unable To create the soundtrack.  Please load a 3DS Mod Project For Pokemon Super Mystery Dungeon, And try again.  If you are using a romfs.bin instead of a .3DS file, use ""psmd-soundtrack -override""")
        End If
    End Function
End Class