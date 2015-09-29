Imports SkyEditorBase

Namespace Mods
    Public Class GenericSkyMod
        Inherits GenericNDSMod
        Public Overrides Async Function InitializeAsync(CurrentProject As Project) As Task
            Dim internalPath = "Mods/" & IO.Path.GetFileNameWithoutExtension(OriginalFilename)
            'Convert BACK
            Dim BACKdir As String = IO.Path.Combine(ModOutputDirectory, "Backgrounds")
            CurrentProject.CreateDirectory("Mods/" & IO.Path.GetFileNameWithoutExtension(OriginalFilename) & "/Backgrounds/")
            Dim backFiles = IO.Directory.GetFiles(IO.Path.Combine(ROMDirectory, "Data", "BACK"), "*.bgp")
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
                    CurrentProject.OpenFile(newFilename, "Mods/" & IO.Path.GetFileNameWithoutExtension(OriginalFilename) & "/Backgrounds/" & IO.Path.GetFileName(newFilename), False)
                End Using
            Next

            ''Open Language
            'Dim englishLanguage = New FileFormats.LanguageString(IO.Path.Combine(romDirectory, "Data", "MESSAGE", "text_en.str"))
            'Dim itemNames(1351) As String
            'englishLanguage.Items.CopyTo(6775, itemNames, 0, 1351)

            'Convert Languages
            PluginHelper.StartLoading("Converting languages...")
            CurrentProject.CreateDirectory("Mods/" & IO.Path.GetFileNameWithoutExtension(OriginalFilename) & "/Languages/")
            Dim languageDictionary As New Dictionary(Of String, String)
            languageDictionary.Add("text_e.str", "English")
            languageDictionary.Add("text_f.str", "Frensh")
            languageDictionary.Add("text_s.str", "Spanish")
            languageDictionary.Add("text_i.str", "Italian")
            languageDictionary.Add("text_g.str", "German")
            languageDictionary.Add("text_j.str", "Japanese")
            For Each item In languageDictionary
                If IO.File.Exists(IO.Path.Combine(ROMDirectory, "Data", "MESSAGE", item.Key)) Then
                    Using langString = New FileFormats.LanguageString(IO.Path.Combine(ROMDirectory, "Data", "MESSAGE", item.Key))
                        Using langList As New ObjectFile(Of List(Of String))
                            langList.ContainedObject = langString.Items
                            langList.Save(IO.Path.Combine(ModOutputDirectory, "Languages", item.Value))

                            CurrentProject.OpenFile(IO.Path.Combine(ModOutputDirectory, "Languages", item.Value), IO.Path.Combine(internalPath, "Languages", item.Value), False)
                        End Using
                    End Using
                End If
            Next

            'Copy Items
            PluginHelper.StartLoading("Converting item definitions...")
            Dim item_p_path As String = IO.Path.Combine(ROMDirectory, "Data", "BALANCE", "item_p.bin")
            Dim item_s_p_path As String = IO.Path.Combine(ROMDirectory, "Data", "BALANCE", "item_s_p.bin")

            CurrentProject.CreateDirectory("Mods/" & IO.Path.GetFileNameWithoutExtension(OriginalFilename) & "/Items/")
            IO.File.Copy(item_p_path, IO.Path.Combine(ModOutputDirectory, "Items", "Item Definitions"))
            IO.File.Copy(item_s_p_path, IO.Path.Combine(ModOutputDirectory, "Items", "Exclusive Item Rarity"))

            CurrentProject.OpenFile(IO.Path.Combine(ModOutputDirectory, "Items", "Item Definitions"), IO.Path.Combine(internalPath, "Items", "Item Definitions"), False)
            CurrentProject.OpenFile(IO.Path.Combine(ModOutputDirectory, "Items", "Exclusive Item Rarity"), IO.Path.Combine(internalPath, "Items", "Exclusive Item Rarity"), False)

            'Copy Swap Shop Rewards
            PluginHelper.StartLoading("Copying swap shop rewards...")
            Dim tableDat_00_path As String = IO.Path.Combine(ROMDirectory, "Data", "TABLEDAT", "item00.dat") 'Unknown ticket, rewards are rare and useful items (Sitrus Berry, Reviver Seed, Life Seed, Ginseng, Joy Seed, Protein, Calcium, Iron, Zinc, Gold Ticket, Prism Ticket, Link Box)
            Dim tableDat_01_path As String = IO.Path.Combine(ROMDirectory, "Data", "TABLEDAT", "item01.dat") 'Unknown ticket, rewards are orbs
            Dim tableDat_02_path As String = IO.Path.Combine(ROMDirectory, "Data", "TABLEDAT", "item02.dat") 'Unknown ticket, rewards are tickets + TMs
            Dim tableDat_03_path As String = IO.Path.Combine(ROMDirectory, "Data", "TABLEDAT", "item03.dat") 'Unknown ticket, rewards are dungeon staples (Oran Berry, Reviver Seed, Max Elixir, Apple, Big Apple, Escape Orb)
            Dim tableDat_04_path As String = IO.Path.Combine(ROMDirectory, "Data", "TABLEDAT", "item04.dat") 'Prize Ticket - Loss
            Dim tableDat_05_path As String = IO.Path.Combine(ROMDirectory, "Data", "TABLEDAT", "item05.dat") 'Prize Ticket - Win
            Dim tableDat_06_path As String = IO.Path.Combine(ROMDirectory, "Data", "TABLEDAT", "item06.dat") 'Prize Ticket - Big Win
            Dim tableDat_07_path As String = IO.Path.Combine(ROMDirectory, "Data", "TABLEDAT", "item07.dat") 'Silver Ticket - Loss
            Dim tableDat_08_path As String = IO.Path.Combine(ROMDirectory, "Data", "TABLEDAT", "item08.dat") 'Silver Ticket - Win
            Dim tableDat_09_path As String = IO.Path.Combine(ROMDirectory, "Data", "TABLEDAT", "item09.dat") 'Silver Ticket - Big Win
            Dim tableDat_10_path As String = IO.Path.Combine(ROMDirectory, "Data", "TABLEDAT", "item10.dat") 'Gold Ticket - Loss
            Dim tableDat_11_path As String = IO.Path.Combine(ROMDirectory, "Data", "TABLEDAT", "item11.dat") 'Gold Ticket - Win
            Dim tableDat_12_path As String = IO.Path.Combine(ROMDirectory, "Data", "TABLEDAT", "item12.dat") 'Gold Ticket - Big Win
            Dim tableDat_13_path As String = IO.Path.Combine(ROMDirectory, "Data", "TABLEDAT", "item13.dat") 'Prism Ticket - Loss
            Dim tableDat_14_path As String = IO.Path.Combine(ROMDirectory, "Data", "TABLEDAT", "item14.dat") 'Prism Ticket - Win
            Dim tableDat_15_path As String = IO.Path.Combine(ROMDirectory, "Data", "TABLEDAT", "item15.dat") 'Prism Ticket - Big Win

            CurrentProject.CreateDirectory("Mods/" & IO.Path.GetFileNameWithoutExtension(OriginalFilename) & "/Items/Swap Shop Rewards/")
            'IO.File.Copy(tableDat_00_path, IO.Path.Combine(modDirectory, "Items", "Swap Shop Rewards", "item00.bin"))
            'IO.File.Copy(tableDat_00_path, IO.Path.Combine(modDirectory, "Items", "Swap Shop Rewards", "item00.bin"))
            'IO.File.Copy(tableDat_00_path, IO.Path.Combine(modDirectory, "Items", "Swap Shop Rewards", "item00.bin"))
            'IO.File.Copy(tableDat_00_path, IO.Path.Combine(modDirectory, "Items", "Swap Shop Rewards", "item00.bin"))
            IO.File.Copy(tableDat_04_path, IO.Path.Combine(ModOutputDirectory, "Items", "Swap Shop Rewards", "Prize Ticket - Loss"))
            IO.File.Copy(tableDat_05_path, IO.Path.Combine(ModOutputDirectory, "Items", "Swap Shop Rewards", "Prize Ticket - Win"))
            IO.File.Copy(tableDat_06_path, IO.Path.Combine(ModOutputDirectory, "Items", "Swap Shop Rewards", "Prize Ticket - Big Win"))
            IO.File.Copy(tableDat_07_path, IO.Path.Combine(ModOutputDirectory, "Items", "Swap Shop Rewards", "Silver Ticket - Loss"))
            IO.File.Copy(tableDat_08_path, IO.Path.Combine(ModOutputDirectory, "Items", "Swap Shop Rewards", "Silver Ticket - Win"))
            IO.File.Copy(tableDat_09_path, IO.Path.Combine(ModOutputDirectory, "Items", "Swap Shop Rewards", "Silver Ticket - Big Win"))
            IO.File.Copy(tableDat_10_path, IO.Path.Combine(ModOutputDirectory, "Items", "Swap Shop Rewards", "Gold Ticket - Loss"))
            IO.File.Copy(tableDat_11_path, IO.Path.Combine(ModOutputDirectory, "Items", "Swap Shop Rewards", "Gold Ticket - Win"))
            IO.File.Copy(tableDat_12_path, IO.Path.Combine(ModOutputDirectory, "Items", "Swap Shop Rewards", "Gold Ticket - Big Win"))
            IO.File.Copy(tableDat_13_path, IO.Path.Combine(ModOutputDirectory, "Items", "Swap Shop Rewards", "Prism Ticket - Loss"))
            IO.File.Copy(tableDat_14_path, IO.Path.Combine(ModOutputDirectory, "Items", "Swap Shop Rewards", "Prism Ticket - Win"))
            IO.File.Copy(tableDat_15_path, IO.Path.Combine(ModOutputDirectory, "Items", "Swap Shop Rewards", "Prism Ticket - Big Win"))

            CurrentProject.OpenFile(IO.Path.Combine(ModOutputDirectory, "Items", "Swap Shop Rewards", "Prize Ticket - Loss"), IO.Path.Combine(internalPath, "Items", "Swap Shop Rewards", "Prize Ticket - Loss"), False)
            CurrentProject.OpenFile(IO.Path.Combine(ModOutputDirectory, "Items", "Swap Shop Rewards", "Prize Ticket - Win"), IO.Path.Combine(internalPath, "Items", "Swap Shop Rewards", "Prize Ticket - Win"), False)
            CurrentProject.OpenFile(IO.Path.Combine(ModOutputDirectory, "Items", "Swap Shop Rewards", "Prize Ticket - Big Win"), IO.Path.Combine(internalPath, "Items", "Swap Shop Rewards", "Prize Ticket - Big Win"), False)
            CurrentProject.OpenFile(IO.Path.Combine(ModOutputDirectory, "Items", "Swap Shop Rewards", "Silver Ticket - Loss"), IO.Path.Combine(internalPath, "Items", "Swap Shop Rewards", "Silver Ticket - Loss"), False)
            CurrentProject.OpenFile(IO.Path.Combine(ModOutputDirectory, "Items", "Swap Shop Rewards", "Silver Ticket - Win"), IO.Path.Combine(internalPath, "Items", "Swap Shop Rewards", "Silver Ticket - Win"), False)
            CurrentProject.OpenFile(IO.Path.Combine(ModOutputDirectory, "Items", "Swap Shop Rewards", "Silver Ticket - Big Win"), IO.Path.Combine(internalPath, "Items", "Swap Shop Rewards", "Silver Ticket - Big Win"), False)
            CurrentProject.OpenFile(IO.Path.Combine(ModOutputDirectory, "Items", "Swap Shop Rewards", "Gold Ticket - Loss"), IO.Path.Combine(internalPath, "Items", "Swap Shop Rewards", "Gold Ticket - Loss"), False)
            CurrentProject.OpenFile(IO.Path.Combine(ModOutputDirectory, "Items", "Swap Shop Rewards", "Gold Ticket - Win"), IO.Path.Combine(internalPath, "Items", "Swap Shop Rewards", "Gold Ticket - Win"), False)
            CurrentProject.OpenFile(IO.Path.Combine(ModOutputDirectory, "Items", "Swap Shop Rewards", "Gold Ticket - Big Win"), IO.Path.Combine(internalPath, "Items", "Swap Shop Rewards", "Gold Ticket - Big Win"), False)
            CurrentProject.OpenFile(IO.Path.Combine(ModOutputDirectory, "Items", "Swap Shop Rewards", "Prism Ticket - Loss"), IO.Path.Combine(internalPath, "Items", "Swap Shop Rewards", "Prism Ticket - Loss"), False)
            CurrentProject.OpenFile(IO.Path.Combine(ModOutputDirectory, "Items", "Swap Shop Rewards", "Prism Ticket - Win"), IO.Path.Combine(internalPath, "Items", "Swap Shop Rewards", "Prism Ticket - Win"), False)
            CurrentProject.OpenFile(IO.Path.Combine(ModOutputDirectory, "Items", "Swap Shop Rewards", "Prism Ticket - Big Win"), IO.Path.Combine(internalPath, "Items", "Swap Shop Rewards", "Prism Ticket - Big Win"), False)

            'Convert Personality Test
            PluginHelper.StartLoading("Converting Personality Test")
            Dim overlay13 As New FileFormats.Overlay13(IO.Path.Combine(ROMDirectory, "Overlay", "overlay_0013.bin"))
            Using personalityTest As New ObjectFile(Of FileFormats.PersonalityTestContainer)
                personalityTest.ContainedObject = New FileFormats.PersonalityTestContainer(overlay13)
                personalityTest.Save(IO.Path.Combine(ModOutputDirectory, "Starter Pokemon"))
                CurrentProject.OpenFile(IO.Path.Combine(ModOutputDirectory, "Starter Pokemon"), IO.Path.Combine(internalPath, "Starter Pokemon"), False)
            End Using

            'Convert Portraits
            'PluginHelper.StartLoading("Unpacking portraits...")
            'If Not IO.Directory.Exists(IO.Path.Combine(modDirectory, "Pokemon", "Portraits")) Then IO.Directory.CreateDirectory(IO.Path.Combine(modDirectory, "Pokemon", "Portraits"))
            'Await Kaomado.RunUnpack(IO.Path.Combine(romDirectory, "Data", "FONT", "kaomado.kao"), IO.Path.Combine(modDirectory, "Pokemon", "Portraits"))

            PluginHelper.StopLoading()
        End Function
    End Class

End Namespace
