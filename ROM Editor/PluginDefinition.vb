Imports SkyEditorBase
Imports ROMEditor.Roms
Imports SkyEditorBase.Interfaces

Public Class PluginDefinition
    Implements iSkyEditorPlugin

    'Public Function AutoDetectFileType(File As GenericFile) As Type
    '    Dim out As Type = Nothing
    '    If SkyNDSRom.IsFileOfType(File) Then
    '        out = GetType(SkyNDSRom)
    '    ElseIf File.OriginalFilename.ToLower.EndsWith(".bgp") Then
    '        out = GetType(FileFormats.BGP)
    '    ElseIf File.OriginalFilename.ToLower.EndsWith(".ndsmodsrc")
    '        out = GetType(FileFormats.GenericNDSMod)
    '    ElseIf IO.Path.GetFileName(File.OriginalFilename).ToLower = "item_p.bin" OrElse IO.Path.GetFileName(File.OriginalFilename).ToLower = "Item Definitions".ToLower
    '        out = GetType(FileFormats.item_p)
    '    ElseIf IO.Path.GetFileName(File.OriginalFilename).ToLower = "item_s_p.bin" OrElse IO.Path.GetFileName(File.OriginalFilename).ToLower = "Exclusive Item Rarity".ToLower
    '        out = GetType(FileFormats.item_s_p)
    '    ElseIf IO.Path.GetFileName(File.OriginalFilename).ToLower = "starter pokemon"
    '        out = GetType(ObjectFile(Of FileFormats.PersonalityTestContainer))
    '    End If
    '    Return out
    'End Function

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

        'Manager.RegisterIOFilter("*.nds", PluginHelper.GetLanguageItem("Nintendo DS ROM"))

        Manager.RegisterTypeSearcher(GetType(Mods.GenericMod), AddressOf NDSModRegistry.AddNDSMod)

        Manager.RegisterConsoleCommand("import-language", AddressOf ConsoleCommands.ImportSkyLanguageString)
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
        EnsureDirDeleted(PluginHelper.GetResourceName("temp"))
        EnsureDirDeleted(PluginHelper.GetResourceName("desmume-0.9.11-win32"))
        EnsureDirDeleted(PluginHelper.GetResourceName("desmume-0.9.11-win64"))
        EnsureFileDeleted(PluginHelper.GetResourceName("Current.nds"))
        EnsureFileDeleted(PluginHelper.GetResourceName("3DS Builder.exe.config"))
        EnsureFileDeleted(PluginHelper.GetResourceName("3DS Builder.pdb"))
        EnsureFileDeleted(PluginHelper.GetResourceName("3DS Builder.vshost.exe"))
        EnsureFileDeleted(PluginHelper.GetResourceName("3DS Builder.vshost.exe.config"))
        EnsureFileDeleted(PluginHelper.GetResourceName("3DS Builder.vshost.exe.manifest"))
        EnsureFileDeleted(PluginHelper.GetResourceName("DSPatcher.exe.config"))
        EnsureFileDeleted(PluginHelper.GetResourceName("DSPatcher.pdb"))
        EnsureFileDeleted(PluginHelper.GetResourceName("DSPatcher.vshost.exe"))
        EnsureFileDeleted(PluginHelper.GetResourceName("DSPatcher.vshost.exe.config"))
        EnsureFileDeleted(PluginHelper.GetResourceName("DSPatcher.vshost.exe.manifest"))
        EnsureFileDeleted(PluginHelper.GetResourceName("DSPatcher.xml"))
        EnsureFileDeleted(PluginHelper.GetResourceName("itemppatcher.exe.config"))
        EnsureFileDeleted(PluginHelper.GetResourceName("itemppatcher.pdb"))
        EnsureFileDeleted(PluginHelper.GetResourceName("LanguageStringPatcher.exe.config"))
        EnsureFileDeleted(PluginHelper.GetResourceName("LanguageStringPatcher.pdb"))
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