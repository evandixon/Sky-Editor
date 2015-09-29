'Imports ROMEditor
'Imports ROMEditor.FileFormats
'Imports ROMEditor.Mods
'Imports ROMEditor.Roms
'Imports SkyEditorBase

'Public Class SkyRomProject
'    Inherits GenericNDSModProject

'    Private Async Function LoadKaomadoFixMod(modDirectory As String, romDirectory As String) As Task

'    End Function

'    Private Async Function LoadGeneralMod(modDirectory As String, romDirectory As String, modFilename As String, internalPath As String) As Task

'    End Function

'    Private Async Sub SkyRomProject_NDSModAdded(sender As Object, e As NDSModAddedEventArgs) Handles Me.NDSModAdded
'        Dim romDirectory = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "Mods", IO.Path.GetFileNameWithoutExtension(e.InternalName), "RawFiles")
'        Dim modDirectory = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "Mods", IO.Path.GetFileNameWithoutExtension(e.InternalName))
'        Dim internalPath = "Mods/" & IO.Path.GetFileNameWithoutExtension(e.InternalName)
'        Dim sky = DirectCast(Files("BaseRom.nds"), SkyNDSRom)

'        If TypeOf e.File Is KaomadoNDSMod Then
'            Await LoadKaomadoFixMod(modDirectory, romDirectory)
'        Else
'            Await LoadGeneralMod(modDirectory, romDirectory, e.InternalName, internalPath)
'        End If
'    End Sub
'    Private Async Function BuildModAsync(e As NDSModBuildingEventArgs) As Task
'        Dim romDirectory = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "Mods", IO.Path.GetFileNameWithoutExtension(IO.Path.GetFileNameWithoutExtension(e.NDSModSourceFilename)), "RawFiles")
'        Dim modDirectory = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "Mods", IO.Path.GetFileNameWithoutExtension(IO.Path.GetFileNameWithoutExtension(e.NDSModSourceFilename)))




'        'Copy Items
'        Dim item_p_path As String = IO.Path.Combine(romDirectory, "Data", "BALANCE", "item_p.bin")
'        Dim item_s_p_path As String = IO.Path.Combine(romDirectory, "Data", "BALANCE", "item_s_p.bin")

'        If IO.File.Exists(IO.Path.Combine(modDirectory, "Items", "Item Definitions")) Then
'            IO.File.Copy(IO.Path.Combine(modDirectory, "Items", "Item Definitions"), item_p_path, True)
'        End If
'        If IO.File.Exists(IO.Path.Combine(modDirectory, "Items", "Exclusive Item Rarity")) Then
'            IO.File.Copy(IO.Path.Combine(modDirectory, "Items", "Exclusive Item Rarity"), item_s_p_path, True)
'        End If







'    End Function

'End Class