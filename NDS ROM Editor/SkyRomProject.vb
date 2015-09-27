Imports ROMEditor
Imports ROMEditor.FileFormats
Imports ROMEditor.Roms
Imports SkyEditorBase

Public Class SkyRomProject
    Inherits GenericNDSModProject

    Private Async Function LoadKaomadoFixMod(modDirectory As String, romDirectory As String) As Task
        Dim portraitDir = IO.Path.Combine(modDirectory, "Pokemon", "Portraits")
        If Not IO.Directory.Exists(portraitDir) Then IO.Directory.CreateDirectory(portraitDir)
        Dim k As New Kaomado(IO.Path.Combine(romDirectory, "Data", "FONT", "kaomado.kao"))
        Await Kaomado.RunUnpack(IO.Path.Combine(romDirectory, "Data", "FONT", "kaomado.kao"), portraitDir)
        Await k.ApplyMissingPortraitFix(portraitDir)
    End Function

    Private Async Function LoadGeneralMod(modDirectory As String, romDirectory As String, modFilename As String, internalPath As String) As Task
        'Convert BACK
        Dim BACKdir As String = IO.Path.Combine(modDirectory, "Backgrounds")
        CreateDirectory("Mods/" & IO.Path.GetFileNameWithoutExtension(modFilename) & "/Backgrounds/")
        Dim backFiles = IO.Directory.GetFiles(IO.Path.Combine(romDirectory, "Data", "BACK"), "*.bgp")
        For count = 0 To backFiles.Count - 1
            PluginHelper.StartLoading("Converting backgrounds...", count / backFiles.Count)
            Dim item = backFiles(count)
            Using b As New FileFormats.BGP(item)
                Dim image = Await b.GetImage
                Dim newFilename = IO.Path.Combine(BACKdir, IO.Path.GetFileNameWithoutExtension(item) & ".bmp")
                If Not IO.Directory.Exists(IO.Path.GetDirectoryName(newFilename)) Then
                    IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(newFilename))
                End If
                image.Save(newFilename, Drawing.Imaging.ImageFormat.Bmp)
                IO.File.Copy(newFilename, newFilename & ".original")
                OpenFile(newFilename, "Mods/" & IO.Path.GetFileNameWithoutExtension(modFilename) & "/Backgrounds/" & IO.Path.GetFileName(newFilename), False)
            End Using
        Next

        ''Open Language
        'Dim englishLanguage = New FileFormats.LanguageString(IO.Path.Combine(romDirectory, "Data", "MESSAGE", "text_en.str"))
        'Dim itemNames(1351) As String
        'englishLanguage.Items.CopyTo(6775, itemNames, 0, 1351)

        'Convert Languages
        PluginHelper.StartLoading("Converting languages...")
        CreateDirectory("Mods/" & IO.Path.GetFileNameWithoutExtension(modFilename) & "/Languages/")
        Dim languageDictionary As New Dictionary(Of String, String)
        languageDictionary.Add("text_e.str", "English")
        languageDictionary.Add("text_f.str", "Frensh")
        languageDictionary.Add("text_s.str", "Spanish")
        languageDictionary.Add("text_i.str", "Italian")
        languageDictionary.Add("text_g.str", "German")
        languageDictionary.Add("text_j.str", "Japanese")
        For Each item In languageDictionary
            If IO.File.Exists(IO.Path.Combine(romDirectory, "Data", "MESSAGE", item.Key)) Then
                Using langString = New FileFormats.LanguageString(IO.Path.Combine(romDirectory, "Data", "MESSAGE", item.Key))
                    Using langList As New ObjectFile(Of List(Of String))
                        langList.ContainedObject = langString.Items
                        langList.Save(IO.Path.Combine(modDirectory, "Languages", item.Value))

                        OpenFile(IO.Path.Combine(modDirectory, "Languages", item.Value), IO.Path.Combine(internalPath, "Languages", item.Value), False)
                    End Using
                End Using
            End If
        Next

        'Copy Items
        PluginHelper.StartLoading("Converting item definitions...")
        Dim item_p_path As String = IO.Path.Combine(romDirectory, "Data", "BALANCE", "item_p.bin")
        Dim item_s_p_path As String = IO.Path.Combine(romDirectory, "Data", "BALANCE", "item_s_p.bin")

        CreateDirectory("Mods/" & IO.Path.GetFileNameWithoutExtension(modFilename) & "/Items/")
        IO.File.Copy(item_p_path, IO.Path.Combine(modDirectory, "Items", "Item Definitions"))
        IO.File.Copy(item_s_p_path, IO.Path.Combine(modDirectory, "Items", "Exclusive Item Rarity"))

        OpenFile(IO.Path.Combine(modDirectory, "Items", "Item Definitions"), IO.Path.Combine(internalPath, "Items", "Item Definitions"), False)
        OpenFile(IO.Path.Combine(modDirectory, "Items", "Exclusive Item Rarity"), IO.Path.Combine(internalPath, "Items", "Exclusive Item Rarity"), False)

        'Copy Swap Shop Rewards
        PluginHelper.StartLoading("Copying swap shop rewards...")
        Dim tableDat_00_path As String = IO.Path.Combine(romDirectory, "Data", "TABLEDAT", "item00.dat") 'Unknown ticket, rewards are rare and useful items (Sitrus Berry, Reviver Seed, Life Seed, Ginseng, Joy Seed, Protein, Calcium, Iron, Zinc, Gold Ticket, Prism Ticket, Link Box)
        Dim tableDat_01_path As String = IO.Path.Combine(romDirectory, "Data", "TABLEDAT", "item01.dat") 'Unknown ticket, rewards are orbs
        Dim tableDat_02_path As String = IO.Path.Combine(romDirectory, "Data", "TABLEDAT", "item02.dat") 'Unknown ticket, rewards are tickets + TMs
        Dim tableDat_03_path As String = IO.Path.Combine(romDirectory, "Data", "TABLEDAT", "item03.dat") 'Unknown ticket, rewards are dungeon staples (Oran Berry, Reviver Seed, Max Elixir, Apple, Big Apple, Escape Orb)
        Dim tableDat_04_path As String = IO.Path.Combine(romDirectory, "Data", "TABLEDAT", "item04.dat") 'Prize Ticket - Loss
        Dim tableDat_05_path As String = IO.Path.Combine(romDirectory, "Data", "TABLEDAT", "item05.dat") 'Prize Ticket - Win
        Dim tableDat_06_path As String = IO.Path.Combine(romDirectory, "Data", "TABLEDAT", "item06.dat") 'Prize Ticket - Big Win
        Dim tableDat_07_path As String = IO.Path.Combine(romDirectory, "Data", "TABLEDAT", "item07.dat") 'Silver Ticket - Loss
        Dim tableDat_08_path As String = IO.Path.Combine(romDirectory, "Data", "TABLEDAT", "item08.dat") 'Silver Ticket - Win
        Dim tableDat_09_path As String = IO.Path.Combine(romDirectory, "Data", "TABLEDAT", "item09.dat") 'Silver Ticket - Big Win
        Dim tableDat_10_path As String = IO.Path.Combine(romDirectory, "Data", "TABLEDAT", "item10.dat") 'Gold Ticket - Loss
        Dim tableDat_11_path As String = IO.Path.Combine(romDirectory, "Data", "TABLEDAT", "item11.dat") 'Gold Ticket - Win
        Dim tableDat_12_path As String = IO.Path.Combine(romDirectory, "Data", "TABLEDAT", "item12.dat") 'Gold Ticket - Big Win
        Dim tableDat_13_path As String = IO.Path.Combine(romDirectory, "Data", "TABLEDAT", "item13.dat") 'Prism Ticket - Loss
        Dim tableDat_14_path As String = IO.Path.Combine(romDirectory, "Data", "TABLEDAT", "item14.dat") 'Prism Ticket - Win
        Dim tableDat_15_path As String = IO.Path.Combine(romDirectory, "Data", "TABLEDAT", "item15.dat") 'Prism Ticket - Big Win

        CreateDirectory("Mods/" & IO.Path.GetFileNameWithoutExtension(modFilename) & "/Items/Swap Shop Rewards/")
        'IO.File.Copy(tableDat_00_path, IO.Path.Combine(modDirectory, "Items", "Swap Shop Rewards", "item00.bin"))
        'IO.File.Copy(tableDat_00_path, IO.Path.Combine(modDirectory, "Items", "Swap Shop Rewards", "item00.bin"))
        'IO.File.Copy(tableDat_00_path, IO.Path.Combine(modDirectory, "Items", "Swap Shop Rewards", "item00.bin"))
        'IO.File.Copy(tableDat_00_path, IO.Path.Combine(modDirectory, "Items", "Swap Shop Rewards", "item00.bin"))
        IO.File.Copy(tableDat_04_path, IO.Path.Combine(modDirectory, "Items", "Swap Shop Rewards", "Prize Ticket - Loss"))
        IO.File.Copy(tableDat_05_path, IO.Path.Combine(modDirectory, "Items", "Swap Shop Rewards", "Prize Ticket - Win"))
        IO.File.Copy(tableDat_06_path, IO.Path.Combine(modDirectory, "Items", "Swap Shop Rewards", "Prize Ticket - Big Win"))
        IO.File.Copy(tableDat_07_path, IO.Path.Combine(modDirectory, "Items", "Swap Shop Rewards", "Silver Ticket - Loss"))
        IO.File.Copy(tableDat_08_path, IO.Path.Combine(modDirectory, "Items", "Swap Shop Rewards", "Silver Ticket - Win"))
        IO.File.Copy(tableDat_09_path, IO.Path.Combine(modDirectory, "Items", "Swap Shop Rewards", "Silver Ticket - Big Win"))
        IO.File.Copy(tableDat_10_path, IO.Path.Combine(modDirectory, "Items", "Swap Shop Rewards", "Gold Ticket - Loss"))
        IO.File.Copy(tableDat_11_path, IO.Path.Combine(modDirectory, "Items", "Swap Shop Rewards", "Gold Ticket - Win"))
        IO.File.Copy(tableDat_12_path, IO.Path.Combine(modDirectory, "Items", "Swap Shop Rewards", "Gold Ticket - Big Win"))
        IO.File.Copy(tableDat_13_path, IO.Path.Combine(modDirectory, "Items", "Swap Shop Rewards", "Prism Ticket - Loss"))
        IO.File.Copy(tableDat_14_path, IO.Path.Combine(modDirectory, "Items", "Swap Shop Rewards", "Prism Ticket - Win"))
        IO.File.Copy(tableDat_15_path, IO.Path.Combine(modDirectory, "Items", "Swap Shop Rewards", "Prism Ticket - Big Win"))

        OpenFile(IO.Path.Combine(modDirectory, "Items", "Swap Shop Rewards", "Prize Ticket - Loss"), IO.Path.Combine(internalPath, "Items", "Swap Shop Rewards", "Prize Ticket - Loss"), False)
        OpenFile(IO.Path.Combine(modDirectory, "Items", "Swap Shop Rewards", "Prize Ticket - Win"), IO.Path.Combine(internalPath, "Items", "Swap Shop Rewards", "Prize Ticket - Win"), False)
        OpenFile(IO.Path.Combine(modDirectory, "Items", "Swap Shop Rewards", "Prize Ticket - Big Win"), IO.Path.Combine(internalPath, "Items", "Swap Shop Rewards", "Prize Ticket - Big Win"), False)
        OpenFile(IO.Path.Combine(modDirectory, "Items", "Swap Shop Rewards", "Silver Ticket - Loss"), IO.Path.Combine(internalPath, "Items", "Swap Shop Rewards", "Silver Ticket - Loss"), False)
        OpenFile(IO.Path.Combine(modDirectory, "Items", "Swap Shop Rewards", "Silver Ticket - Win"), IO.Path.Combine(internalPath, "Items", "Swap Shop Rewards", "Silver Ticket - Win"), False)
        OpenFile(IO.Path.Combine(modDirectory, "Items", "Swap Shop Rewards", "Silver Ticket - Big Win"), IO.Path.Combine(internalPath, "Items", "Swap Shop Rewards", "Silver Ticket - Big Win"), False)
        OpenFile(IO.Path.Combine(modDirectory, "Items", "Swap Shop Rewards", "Gold Ticket - Loss"), IO.Path.Combine(internalPath, "Items", "Swap Shop Rewards", "Gold Ticket - Loss"), False)
        OpenFile(IO.Path.Combine(modDirectory, "Items", "Swap Shop Rewards", "Gold Ticket - Win"), IO.Path.Combine(internalPath, "Items", "Swap Shop Rewards", "Gold Ticket - Win"), False)
        OpenFile(IO.Path.Combine(modDirectory, "Items", "Swap Shop Rewards", "Gold Ticket - Big Win"), IO.Path.Combine(internalPath, "Items", "Swap Shop Rewards", "Gold Ticket - Big Win"), False)
        OpenFile(IO.Path.Combine(modDirectory, "Items", "Swap Shop Rewards", "Prism Ticket - Loss"), IO.Path.Combine(internalPath, "Items", "Swap Shop Rewards", "Prism Ticket - Loss"), False)
        OpenFile(IO.Path.Combine(modDirectory, "Items", "Swap Shop Rewards", "Prism Ticket - Win"), IO.Path.Combine(internalPath, "Items", "Swap Shop Rewards", "Prism Ticket - Win"), False)
        OpenFile(IO.Path.Combine(modDirectory, "Items", "Swap Shop Rewards", "Prism Ticket - Big Win"), IO.Path.Combine(internalPath, "Items", "Swap Shop Rewards", "Prism Ticket - Big Win"), False)

        'Convert Personality Test
        PluginHelper.StartLoading("Converting Personality Test")
        Dim overlay13 As New FileFormats.Overlay13(IO.Path.Combine(romDirectory, "Overlay", "overlay_0013.bin"))
        Using personalityTest As New ObjectFile(Of FileFormats.PersonalityTestContainer)
            personalityTest.ContainedObject = New FileFormats.PersonalityTestContainer(overlay13)
            personalityTest.Save(IO.Path.Combine(modDirectory, "Starter Pokemon"))
            OpenFile(IO.Path.Combine(modDirectory, "Starter Pokemon"), IO.Path.Combine(internalPath, "Starter Pokemon"), False)
        End Using

        'Convert Portraits
        'PluginHelper.StartLoading("Unpacking portraits...")
        'If Not IO.Directory.Exists(IO.Path.Combine(modDirectory, "Pokemon", "Portraits")) Then IO.Directory.CreateDirectory(IO.Path.Combine(modDirectory, "Pokemon", "Portraits"))
        'Await Kaomado.RunUnpack(IO.Path.Combine(romDirectory, "Data", "FONT", "kaomado.kao"), IO.Path.Combine(modDirectory, "Pokemon", "Portraits"))

        PluginHelper.StopLoading()
    End Function

    Private Async Sub SkyRomProject_NDSModAdded(sender As Object, e As NDSModAddedEventArgs) Handles Me.NDSModAdded
        Dim romDirectory = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "Mods", IO.Path.GetFileNameWithoutExtension(e.InternalName), "RawFiles")
        Dim modDirectory = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "Mods", IO.Path.GetFileNameWithoutExtension(e.InternalName))
        Dim internalPath = "Mods/" & IO.Path.GetFileNameWithoutExtension(e.InternalName)
        Dim sky = DirectCast(Files("BaseRom.nds"), SkyNDSRom)

        If TypeOf e.File Is KaomadoFixNDSMod Then
            Await LoadKaomadoFixMod(modDirectory, romDirectory)
        Else
            Await LoadGeneralMod(modDirectory, romDirectory, e.InternalName, internalPath)
        End If
    End Sub
    Private Async Function BuildModAsync(e As NDSModBuildingEventArgs) As Task
        Dim romDirectory = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "Mods", IO.Path.GetFileNameWithoutExtension(IO.Path.GetFileNameWithoutExtension(e.NDSModSourceFilename)), "RawFiles")
        Dim modDirectory = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "Mods", IO.Path.GetFileNameWithoutExtension(IO.Path.GetFileNameWithoutExtension(e.NDSModSourceFilename)))
        'Convert BACK
        If IO.Directory.Exists(IO.Path.Combine(IO.Path.GetDirectoryName(e.NDSModSourceFilename), IO.Path.GetFileNameWithoutExtension(e.NDSModSourceFilename), "Backgrounds")) Then
            For Each background In IO.Directory.GetFiles(IO.Path.Combine(IO.Path.GetDirectoryName(e.NDSModSourceFilename), IO.Path.GetFileNameWithoutExtension(e.NDSModSourceFilename), "Backgrounds"), "*.bmp")
                Dim includeInPack As Boolean

                If IO.File.Exists(background & ".original") Then
                    Using bmp As New IO.FileStream(background, IO.FileMode.Open)
                        Using orig As New IO.FileStream(background & ".original", IO.FileMode.Open)
                            Dim equal As Boolean = (bmp.Length = orig.Length)
                            While equal
                                Dim b = bmp.ReadByte
                                Dim o = orig.ReadByte
                                equal = (b = o)
                                If b = -1 OrElse o = -1 Then
                                    Exit While
                                End If
                            End While
                            includeInPack = Not equal
                        End Using
                    End Using
                Else
                    includeInPack = True
                End If

                If includeInPack Then
                    Dim bgp = FileFormats.BGP.ConvertFromBitmap(Drawing.Bitmap.FromFile(background))
                    bgp.Save(IO.Path.Combine(IO.Path.GetDirectoryName(e.NDSModSourceFilename), IO.Path.GetFileNameWithoutExtension(e.NDSModSourceFilename), "RawFiles", "Data", "BACK", IO.Path.GetFileNameWithoutExtension(background) & ".bgp"))
                    bgp.Dispose()
                End If

            Next
        End If

        'Convert Personality Test
        Dim personalityTest As ObjectFile(Of PersonalityTestContainer) = Nothing
        If IO.File.Exists(IO.Path.Combine(modDirectory, "Starter Pokemon")) Then
            Dim overlay13 As New FileFormats.Overlay13(IO.Path.Combine(romDirectory, "Overlay", "overlay_0013.bin"))
            personalityTest = New ObjectFile(Of PersonalityTestContainer)(IO.Path.Combine(modDirectory, "Starter Pokemon"))
            personalityTest.ContainedObject.UpdateOverlay(overlay13)
            overlay13.Save()
        End If


        'Convert Languages
        Dim languageDictionary As New Dictionary(Of String, String)
        languageDictionary.Add("text_e.str", "English")
        languageDictionary.Add("text_f.str", "Frensh")
        languageDictionary.Add("text_s.str", "Spanish")
        languageDictionary.Add("text_i.str", "Italian")
        languageDictionary.Add("text_g.str", "German")
        languageDictionary.Add("text_j.str", "Japanese")
        For Each item In languageDictionary
            If IO.File.Exists(IO.Path.Combine(modDirectory, "Languages", item.Value)) Then
                Using langFile As New ObjectFile(Of List(Of String))(IO.Path.Combine(modDirectory, "Languages", item.Value))
                    Using langString As New FileFormats.LanguageString
                        langString.Items = langFile.ContainedObject

                        If personalityTest IsNot Nothing Then
                            langString.UpdatePersonalityTestResult(personalityTest.ContainedObject)
                        End If

                        langString.Save(IO.Path.Combine(romDirectory, "Data", "MESSAGE", item.Key))
                    End Using
                End Using
            End If
        Next

        If personalityTest IsNot Nothing Then personalityTest.Dispose()

        'Copy Items
        Dim item_p_path As String = IO.Path.Combine(romDirectory, "Data", "BALANCE", "item_p.bin")
        Dim item_s_p_path As String = IO.Path.Combine(romDirectory, "Data", "BALANCE", "item_s_p.bin")

        If IO.File.Exists(IO.Path.Combine(modDirectory, "Items", "Item Definitions")) Then
            IO.File.Copy(IO.Path.Combine(modDirectory, "Items", "Item Definitions"), item_p_path, True)
        End If
        If IO.File.Exists(IO.Path.Combine(modDirectory, "Items", "Exclusive Item Rarity")) Then
            IO.File.Copy(IO.Path.Combine(modDirectory, "Items", "Exclusive Item Rarity"), item_s_p_path, True)
        End If




        'Convert portraits
        If IO.Directory.Exists(IO.Path.Combine(modDirectory, "Pokemon", "Portraits")) Then
            Await Kaomado.RunPack(IO.Path.Combine(romDirectory, "Data", "FONT", "kaomado.kao"), IO.Path.Combine(modDirectory, "Pokemon", "Portraits"))
        End If

        'Cleanup
        '-Data/Back/Decompressed
        If IO.Directory.Exists(IO.Path.Combine(IO.Path.GetDirectoryName(e.NDSModSourceFilename), IO.Path.GetFileNameWithoutExtension(e.NDSModSourceFilename), "RawFiles", "Data", "BACK", "Decompressed")) Then
            IO.Directory.Delete(IO.Path.Combine(IO.Path.GetDirectoryName(e.NDSModSourceFilename), IO.Path.GetFileNameWithoutExtension(e.NDSModSourceFilename), "RawFiles", "Data", "BACK", "Decompressed"), True)
        End If
    End Function
    Protected Overrides Async Function BuildMod(e As NDSModBuildingEventArgs) As Task
        Await MyBase.BuildMod(e)
        Await BuildModAsync(e)
    End Function

    Public Overrides Function CustomFilePatchers() As List(Of FilePatcher)
        Dim patchers = MyBase.CustomFilePatchers
        If patchers Is Nothing Then
            patchers = New List(Of FilePatcher)
        End If
        Dim LSPatcher As New FilePatcher()
        With LSPatcher
            .CreatePatchProgram = "LanguageStringPatcher.exe"
            .CreatePatchArguments = "-c ""{0}"" ""{1}"" ""{2}"""
            .ApplyPatchProgram = "LanguageStringPatcher.exe"
            .ApplyPatchArguments = "-a ""{0}"" ""{1}"" ""{2}"""
            .MergeSafe = True
            .PatchExtension = "textstrlsp"
            .FilePath = ".*text_.\.str"
        End With
        patchers.Add(LSPatcher)
        Return patchers
    End Function
    Public Overrides Function CreatableFiles(InternalPath As String, Manager As PluginManager) As IList(Of Type)
        Dim l = MyBase.CreatableFiles(InternalPath, Manager)
        If InternalPath.ToLower = "mods/" Then
            l.Add(GetType(KaomadoFixNDSMod))
        End If
        Return l
    End Function
End Class