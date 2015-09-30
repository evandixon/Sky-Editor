'Imports SkyEditorBase

'Namespace Mods
'    Public Class GenericSkyMod
'        Inherits GenericMod
'        Public Overrides Sub Initialize(CurrentProject As Project)
'            Dim internalPath = "Mods/" & IO.Path.GetFileNameWithoutExtension(OriginalFilename)




'            'Copy Items
'            PluginHelper.StartLoading("Converting item definitions...")
'            Dim item_p_path As String = IO.Path.Combine(ROMDirectory, "Data", "BALANCE", "item_p.bin")
'            Dim item_s_p_path As String = IO.Path.Combine(ROMDirectory, "Data", "BALANCE", "item_s_p.bin")

'            CurrentProject.CreateDirectory("Mods/" & IO.Path.GetFileNameWithoutExtension(OriginalFilename) & "/Items/")
'            IO.File.Copy(item_p_path, IO.Path.Combine(ModDirectory, "Items", "Item Definitions"))
'            IO.File.Copy(item_s_p_path, IO.Path.Combine(ModDirectory, "Items", "Exclusive Item Rarity"))

'            CurrentProject.OpenFile(IO.Path.Combine(ModDirectory, "Items", "Item Definitions"), IO.Path.Combine(internalPath, "Items", "Item Definitions"), False)
'            CurrentProject.OpenFile(IO.Path.Combine(ModDirectory, "Items", "Exclusive Item Rarity"), IO.Path.Combine(internalPath, "Items", "Exclusive Item Rarity"), False)

'            'Copy Swap Shop Rewards
'            PluginHelper.StartLoading("Copying swap shop rewards...")
'            Dim tableDat_00_path As String = IO.Path.Combine(ROMDirectory, "Data", "TABLEDAT", "item00.dat") 'Unknown ticket, rewards are rare and useful items (Sitrus Berry, Reviver Seed, Life Seed, Ginseng, Joy Seed, Protein, Calcium, Iron, Zinc, Gold Ticket, Prism Ticket, Link Box)
'            Dim tableDat_01_path As String = IO.Path.Combine(ROMDirectory, "Data", "TABLEDAT", "item01.dat") 'Unknown ticket, rewards are orbs
'            Dim tableDat_02_path As String = IO.Path.Combine(ROMDirectory, "Data", "TABLEDAT", "item02.dat") 'Unknown ticket, rewards are tickets + TMs
'            Dim tableDat_03_path As String = IO.Path.Combine(ROMDirectory, "Data", "TABLEDAT", "item03.dat") 'Unknown ticket, rewards are dungeon staples (Oran Berry, Reviver Seed, Max Elixir, Apple, Big Apple, Escape Orb)
'            Dim tableDat_04_path As String = IO.Path.Combine(ROMDirectory, "Data", "TABLEDAT", "item04.dat") 'Prize Ticket - Loss
'            Dim tableDat_05_path As String = IO.Path.Combine(ROMDirectory, "Data", "TABLEDAT", "item05.dat") 'Prize Ticket - Win
'            Dim tableDat_06_path As String = IO.Path.Combine(ROMDirectory, "Data", "TABLEDAT", "item06.dat") 'Prize Ticket - Big Win
'            Dim tableDat_07_path As String = IO.Path.Combine(ROMDirectory, "Data", "TABLEDAT", "item07.dat") 'Silver Ticket - Loss
'            Dim tableDat_08_path As String = IO.Path.Combine(ROMDirectory, "Data", "TABLEDAT", "item08.dat") 'Silver Ticket - Win
'            Dim tableDat_09_path As String = IO.Path.Combine(ROMDirectory, "Data", "TABLEDAT", "item09.dat") 'Silver Ticket - Big Win
'            Dim tableDat_10_path As String = IO.Path.Combine(ROMDirectory, "Data", "TABLEDAT", "item10.dat") 'Gold Ticket - Loss
'            Dim tableDat_11_path As String = IO.Path.Combine(ROMDirectory, "Data", "TABLEDAT", "item11.dat") 'Gold Ticket - Win
'            Dim tableDat_12_path As String = IO.Path.Combine(ROMDirectory, "Data", "TABLEDAT", "item12.dat") 'Gold Ticket - Big Win
'            Dim tableDat_13_path As String = IO.Path.Combine(ROMDirectory, "Data", "TABLEDAT", "item13.dat") 'Prism Ticket - Loss
'            Dim tableDat_14_path As String = IO.Path.Combine(ROMDirectory, "Data", "TABLEDAT", "item14.dat") 'Prism Ticket - Win
'            Dim tableDat_15_path As String = IO.Path.Combine(ROMDirectory, "Data", "TABLEDAT", "item15.dat") 'Prism Ticket - Big Win

'            CurrentProject.CreateDirectory("Mods/" & IO.Path.GetFileNameWithoutExtension(OriginalFilename) & "/Items/Swap Shop Rewards/")
'            'IO.File.Copy(tableDat_00_path, IO.Path.Combine(modDirectory, "Items", "Swap Shop Rewards", "item00.bin"))
'            'IO.File.Copy(tableDat_00_path, IO.Path.Combine(modDirectory, "Items", "Swap Shop Rewards", "item00.bin"))
'            'IO.File.Copy(tableDat_00_path, IO.Path.Combine(modDirectory, "Items", "Swap Shop Rewards", "item00.bin"))
'            'IO.File.Copy(tableDat_00_path, IO.Path.Combine(modDirectory, "Items", "Swap Shop Rewards", "item00.bin"))
'            IO.File.Copy(tableDat_04_path, IO.Path.Combine(ModDirectory, "Items", "Swap Shop Rewards", "Prize Ticket - Loss"))
'            IO.File.Copy(tableDat_05_path, IO.Path.Combine(ModDirectory, "Items", "Swap Shop Rewards", "Prize Ticket - Win"))
'            IO.File.Copy(tableDat_06_path, IO.Path.Combine(ModDirectory, "Items", "Swap Shop Rewards", "Prize Ticket - Big Win"))
'            IO.File.Copy(tableDat_07_path, IO.Path.Combine(ModDirectory, "Items", "Swap Shop Rewards", "Silver Ticket - Loss"))
'            IO.File.Copy(tableDat_08_path, IO.Path.Combine(ModDirectory, "Items", "Swap Shop Rewards", "Silver Ticket - Win"))
'            IO.File.Copy(tableDat_09_path, IO.Path.Combine(ModDirectory, "Items", "Swap Shop Rewards", "Silver Ticket - Big Win"))
'            IO.File.Copy(tableDat_10_path, IO.Path.Combine(ModDirectory, "Items", "Swap Shop Rewards", "Gold Ticket - Loss"))
'            IO.File.Copy(tableDat_11_path, IO.Path.Combine(ModDirectory, "Items", "Swap Shop Rewards", "Gold Ticket - Win"))
'            IO.File.Copy(tableDat_12_path, IO.Path.Combine(ModDirectory, "Items", "Swap Shop Rewards", "Gold Ticket - Big Win"))
'            IO.File.Copy(tableDat_13_path, IO.Path.Combine(ModDirectory, "Items", "Swap Shop Rewards", "Prism Ticket - Loss"))
'            IO.File.Copy(tableDat_14_path, IO.Path.Combine(ModDirectory, "Items", "Swap Shop Rewards", "Prism Ticket - Win"))
'            IO.File.Copy(tableDat_15_path, IO.Path.Combine(ModDirectory, "Items", "Swap Shop Rewards", "Prism Ticket - Big Win"))

'            CurrentProject.OpenFile(IO.Path.Combine(ModDirectory, "Items", "Swap Shop Rewards", "Prize Ticket - Loss"), IO.Path.Combine(internalPath, "Items", "Swap Shop Rewards", "Prize Ticket - Loss"), False)
'            CurrentProject.OpenFile(IO.Path.Combine(ModDirectory, "Items", "Swap Shop Rewards", "Prize Ticket - Win"), IO.Path.Combine(internalPath, "Items", "Swap Shop Rewards", "Prize Ticket - Win"), False)
'            CurrentProject.OpenFile(IO.Path.Combine(ModDirectory, "Items", "Swap Shop Rewards", "Prize Ticket - Big Win"), IO.Path.Combine(internalPath, "Items", "Swap Shop Rewards", "Prize Ticket - Big Win"), False)
'            CurrentProject.OpenFile(IO.Path.Combine(ModDirectory, "Items", "Swap Shop Rewards", "Silver Ticket - Loss"), IO.Path.Combine(internalPath, "Items", "Swap Shop Rewards", "Silver Ticket - Loss"), False)
'            CurrentProject.OpenFile(IO.Path.Combine(ModDirectory, "Items", "Swap Shop Rewards", "Silver Ticket - Win"), IO.Path.Combine(internalPath, "Items", "Swap Shop Rewards", "Silver Ticket - Win"), False)
'            CurrentProject.OpenFile(IO.Path.Combine(ModDirectory, "Items", "Swap Shop Rewards", "Silver Ticket - Big Win"), IO.Path.Combine(internalPath, "Items", "Swap Shop Rewards", "Silver Ticket - Big Win"), False)
'            CurrentProject.OpenFile(IO.Path.Combine(ModDirectory, "Items", "Swap Shop Rewards", "Gold Ticket - Loss"), IO.Path.Combine(internalPath, "Items", "Swap Shop Rewards", "Gold Ticket - Loss"), False)
'            CurrentProject.OpenFile(IO.Path.Combine(ModDirectory, "Items", "Swap Shop Rewards", "Gold Ticket - Win"), IO.Path.Combine(internalPath, "Items", "Swap Shop Rewards", "Gold Ticket - Win"), False)
'            CurrentProject.OpenFile(IO.Path.Combine(ModDirectory, "Items", "Swap Shop Rewards", "Gold Ticket - Big Win"), IO.Path.Combine(internalPath, "Items", "Swap Shop Rewards", "Gold Ticket - Big Win"), False)
'            CurrentProject.OpenFile(IO.Path.Combine(ModDirectory, "Items", "Swap Shop Rewards", "Prism Ticket - Loss"), IO.Path.Combine(internalPath, "Items", "Swap Shop Rewards", "Prism Ticket - Loss"), False)
'            CurrentProject.OpenFile(IO.Path.Combine(ModDirectory, "Items", "Swap Shop Rewards", "Prism Ticket - Win"), IO.Path.Combine(internalPath, "Items", "Swap Shop Rewards", "Prism Ticket - Win"), False)
'            CurrentProject.OpenFile(IO.Path.Combine(ModDirectory, "Items", "Swap Shop Rewards", "Prism Ticket - Big Win"), IO.Path.Combine(internalPath, "Items", "Swap Shop Rewards", "Prism Ticket - Big Win"), False)



'            'Convert Portraits
'            'PluginHelper.StartLoading("Unpacking portraits...")
'            'If Not IO.Directory.Exists(IO.Path.Combine(modDirectory, "Pokemon", "Portraits")) Then IO.Directory.CreateDirectory(IO.Path.Combine(modDirectory, "Pokemon", "Portraits"))
'            'Await Kaomado.RunUnpack(IO.Path.Combine(romDirectory, "Data", "FONT", "kaomado.kao"), IO.Path.Combine(modDirectory, "Pokemon", "Portraits"))

'            PluginHelper.StopLoading()
'        End Sub

'        Public Overrides Function SupportedGameCodes() As IEnumerable(Of Type)
'            Return {GetType(Roms.SkyNDSRom)}
'        End Function

'    End Class

'End Namespace
