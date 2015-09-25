﻿Imports SkyEditorBase
Imports ROMEditor.Roms

Public Class PluginDefinition
    Implements iSkyEditorPlugin
    Public Function AutoDetectSaveType(File As GenericFile) As String
        Dim out As String = Nothing
        If SkyNDSRom.IsSkyNDSRom(File) Then
            out = GameStrings.SkyNDSRom
        ElseIf File.OriginalFilename.ToLower.EndsWith(".bgp") Then
            out = GameStrings.BGPFile
        ElseIf File.OriginalFilename.ToLower.EndsWith(".ndsmodsrc")
            out = GameStrings.NDSModSourceFile
        ElseIf IO.Path.GetFileName(File.OriginalFilename).ToLower = "item_p.bin" OrElse IO.Path.GetFileName(File.OriginalFilename).ToLower = "Item Definitions".ToLower
            out = GameStrings.item_p_File
        ElseIf IO.Path.GetFileName(File.OriginalFilename).ToLower = "item_s_p.bin" OrElse IO.Path.GetFileName(File.OriginalFilename).ToLower = "Exclusive Item Rarity".ToLower
            out = GameStrings.item_s_p_File
        ElseIf IO.Path.GetFileName(File.OriginalFilename).ToLower = "starter pokemon"
            out = GameStrings.PersonalityTest
        End If
        Return out
    End Function

    Public ReadOnly Property Credits As String Implements iSkyEditorPlugin.Credits
        Get
            Return PluginHelper.GetLanguageItem("RomEditorCredits", "Rom Editor Credits:\n     psy_commando (Pokemon portraits, most of the research)\n     Grovyle91 (Language strings)\n     evandixon (Personality test, bgp files)")
        End Get
    End Property

    Public ReadOnly Property PluginAuthor As String Implements iSkyEditorPlugin.PluginAuthor
        Get
            Return "evandixon"
        End Get
    End Property

    Public ReadOnly Property PluginName As String Implements iSkyEditorPlugin.PluginName
        Get
            Return "Generic ROM Editor"
        End Get
    End Property

    'Public Property ROMFileTypes As Dictionary(Of String, FileFormatControl)

    'Public Sub New()
    '    ROMFileTypes = New Dictionary(Of String, FileFormatControl)
    '    ROMFileTypes.Add("bgp", New BGPControl)
    'End Sub

    Public Sub Load(ByRef Manager As PluginManager) Implements iSkyEditorPlugin.Load
        PluginHelper.Writeline(SkyEditorBase.PluginHelper.GetResourceName("Root"))
        'Manager.RegisterConsoleCommand("header", AddressOf ConsoleCommands.ROMHeader)
        'Manager.RegisterConsoleCommand("unpack", AddressOf ConsoleCommands.UnPack)
        'Manager.RegisterConsoleCommand("repack", AddressOf ConsoleCommands.RePack)
        'Manager.RegisterConsoleCommand("explorersextractbgp", AddressOf ConsoleCommands.ExplorersExtractBGP)
        'Manager.RegisterConsoleCommand("pmdlanguage", AddressOf ConsoleCommands.PmdLanguage)
        'Manager.RegisterConsoleCommand("eostestmusic", AddressOf ConsoleCommands.EoSTestMusic)

        Manager.RegisterIOFilter("*.nds", PluginHelper.GetLanguageItem("Nintendo DS ROM"))

        Manager.RegisterSaveGameFormat(GameStrings.GenericNDSRom, GameStrings.GenericNDSRom, GetType(GenericNDSRom))
        Manager.RegisterSaveGameFormat(GameStrings.SkyNDSRom, GameStrings.SkyNDSRom, GetType(SkyNDSRom))

        Manager.RegisterFileFormat(GameStrings.BGPFile, GetType(FileFormats.BGP))
        Manager.RegisterFileFormat(GameStrings.NDSModSourceFile, GetType(FileFormats.NDSModSource))
        Manager.RegisterFileFormat(GameStrings.KaomadoFixNDSModSourceFile, GetType(FileFormats.KaomadoFixNDSMod))
        Manager.RegisterFileFormat(GameStrings.item_p_File, GetType(FileFormats.item_p))
        Manager.RegisterFileFormat(GameStrings.item_s_p_File, GetType(FileFormats.item_s_p))
        Manager.RegisterFileFormat(GameStrings.PersonalityTest, GetType(ObjectFile(Of FileFormats.PersonalityTestContainer)))

        'Manager.RegisterEditorTab(GetType(PortraitTab))
        'Manager.RegisterEditorTab(GetType(PersonalityTest))
        'Manager.RegisterEditorTab(GetType(FilesTab))

        Manager.RegisterSaveTypeDetector(AddressOf AutoDetectSaveType)

        Manager.RegisterProjectType("Explorers of Sky Rom Mod", GetType(SkyRomProject))
    End Sub

    Public Sub UnLoad(ByRef Manager As PluginManager) Implements iSkyEditorPlugin.UnLoad
        PluginHelper.Writeline("Deleting ROM Editor's temp directory")
        Dim directory As String = PluginHelper.GetResourceName("Temp")
        If IO.Directory.Exists(directory) Then
            On Error Resume Next
            IO.Directory.Delete(directory, True)
            IO.Directory.CreateDirectory(directory)
        End If
    End Sub

    Public Sub PrepareForDistribution() Implements iSkyEditorPlugin.PrepareForDistribution
        EnsureDirDeleted(PluginHelper.GetResourceName("Current"))
        EnsureFileDeleted(PluginHelper.GetResourceName("Current.nds"))
    End Sub

    Private Sub EnsureDirDeleted(Dir As String)
        If IO.Directory.Exists(Dir) Then
            IO.Directory.Delete(Dir, True)
        End If
    End Sub
    Private Sub EnsureFileDeleted(Dir As String)
        If IO.File.Exists(Dir) Then
            IO.File.Delete(Dir)
        End If
    End Sub
End Class