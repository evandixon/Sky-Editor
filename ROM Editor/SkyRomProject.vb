﻿Imports ROMEditor.Roms

Public Class SkyRomProject
    Inherits GenericNDSModProject

    Private Async Sub SkyRomProject_NDSModAdded(sender As Object, e As NDSModAddedEventArgs) Handles Me.NDSModAdded
        Dim romDirectory = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "Mods", IO.Path.GetFileNameWithoutExtension(e.InternalName), "RawFiles")
        Dim modDirectory = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "Mods", IO.Path.GetFileNameWithoutExtension(e.InternalName))
        Dim internalPath = "Mods/" & IO.Path.GetFileNameWithoutExtension(e.InternalName)
        Dim sky = DirectCast(Files("BaseRom.nds"), SkyNDSRom)

        'Convert BACK
        Dim BACKdir As String = IO.Path.Combine(modDirectory, "Backgrounds")
        CreateDirectory("Mods/" & IO.Path.GetFileNameWithoutExtension(e.InternalName) & "/Backgrounds/")
        For Each item In IO.Directory.GetFiles(IO.Path.Combine(romDirectory, "Data", "BACK"), "*.bgp")
            Dim b As New FileFormats.BGP(item)
            Dim image = Await b.GetImage
            Dim newFilename = IO.Path.Combine(BACKdir, IO.Path.GetFileNameWithoutExtension(item) & ".bmp")
            If Not IO.Directory.Exists(IO.Path.GetDirectoryName(newFilename)) Then
                IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(newFilename))
            End If
            image.Save(newFilename, Drawing.Imaging.ImageFormat.Bmp)
            IO.File.Copy(newFilename, newFilename & ".original")
            OpenFile(newFilename, "Mods/" & IO.Path.GetFileNameWithoutExtension(e.InternalName) & "/Backgrounds/" & IO.Path.GetFileName(newFilename), False)
        Next

        ''Open Language
        'Dim englishLanguage = New FileFormats.LanguageString(IO.Path.Combine(romDirectory, "Data", "MESSAGE", "text_en.str"))
        'Dim itemNames(1351) As String
        'englishLanguage.Items.CopyTo(6775, itemNames, 0, 1351)

        'Copy Language
        CreateDirectory("Mods/" & IO.Path.GetFileNameWithoutExtension(e.InternalName) & "/Languages/")
        IO.File.Copy(IO.Path.Combine(romDirectory, "Data", "MESSAGE", "text_e.str"), IO.Path.Combine(modDirectory, "Languages", "English"))
        OpenFile(IO.Path.Combine(modDirectory, "Languages", "English"), IO.Path.Combine(internalPath, "Languages", "English"), False)

        'Copy Items
        Dim item_p_path As String = IO.Path.Combine(romDirectory, "Data", "BALANCE", "item_p.bin")
        Dim item_s_p_path As String = IO.Path.Combine(romDirectory, "Data", "BALANCE", "item_s_p.bin")

        CreateDirectory("Mods/" & IO.Path.GetFileNameWithoutExtension(e.InternalName) & "/Items/")
        IO.File.Copy(item_p_path, IO.Path.Combine(modDirectory, "Items", "Item Definitions"))
        IO.File.Copy(item_s_p_path, IO.Path.Combine(modDirectory, "Items", "Exclusive Item Rarity"))

        OpenFile(IO.Path.Combine(modDirectory, "Items", "Item Definitions"), IO.Path.Combine(internalPath, "Items", "Item Definitions"), False)
        OpenFile(IO.Path.Combine(modDirectory, "Items", "Exclusive Item Rarity"), IO.Path.Combine(internalPath, "Items", "Exclusive Item Rarity"), False)

        'Copy Swap Shop Rewards
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

        CreateDirectory("Mods/" & IO.Path.GetFileNameWithoutExtension(e.InternalName) & "/Items/Swap Shop Rewards/")
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











    End Sub

    Private Sub SkyRomProject_NDSModBuilding(sender As Object, e As NDSModBuildingEventArgs) Handles Me.NDSModBuilding
        Dim romDirectory = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "Mods", IO.Path.GetFileNameWithoutExtension(IO.Path.GetFileNameWithoutExtension(e.NDSModSourceFilename)), "RawFiles")
        Dim modDirectory = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "Mods", IO.Path.GetFileNameWithoutExtension(IO.Path.GetFileNameWithoutExtension(e.NDSModSourceFilename)))
        'Convert BACK
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
            End If

        Next

        'Copy Language
        IO.File.Copy(IO.Path.Combine(modDirectory, "Languages", "English"), IO.Path.Combine(romDirectory, "Data", "MESSAGE", "text_e.str"), True)

        'Copy Items
        Dim item_p_path As String = IO.Path.Combine(romDirectory, "Data", "BALANCE", "item_p.bin")
        Dim item_s_p_path As String = IO.Path.Combine(romDirectory, "Data", "BALANCE", "item_s_p.bin")

        IO.File.Copy(IO.Path.Combine(modDirectory, "Items", "Item Definitions"), item_p_path, True)
        IO.File.Copy(IO.Path.Combine(modDirectory, "Items", "Exclusive Item Rarity"), item_s_p_path, True)

        'Cleanup
        '-Data/Back/Decompressed
        If IO.Directory.Exists(IO.Path.Combine(IO.Path.GetDirectoryName(e.NDSModSourceFilename), IO.Path.GetFileNameWithoutExtension(e.NDSModSourceFilename), "RawFiles", "Data", "BACK", "Decompressed")) Then
            IO.Directory.Delete(IO.Path.Combine(IO.Path.GetDirectoryName(e.NDSModSourceFilename), IO.Path.GetFileNameWithoutExtension(e.NDSModSourceFilename), "RawFiles", "Data", "BACK", "Decompressed"), True)
        End If
    End Sub
End Class
