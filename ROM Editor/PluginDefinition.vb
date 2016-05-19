Imports ROMEditor.Roms
Imports ROMEditor.Projects
Imports ROMEditor.FileFormats.PSMD
Imports SkyEditor.Core
Imports SkyEditorBase
Imports System.Reflection
Imports System.IO
Imports SkyEditor.Core.IO

Public Class PluginDefinition
    Inherits SkyEditorPlugin

    Public Overrides ReadOnly Property Credits As String
        Get
            Return My.Resources.Language.PluginCredits
        End Get
    End Property

    Public Overrides ReadOnly Property PluginAuthor As String
        Get
            Return My.Resources.Language.PluginAuthor
        End Get
    End Property

    Public Overrides ReadOnly Property PluginName As String
        Get
            Return My.Resources.Language.PluginName
        End Get
    End Property

    'Public Property ROMFileTypes As Dictionary(Of String, FileFormatControl)

    'Public Sub New()
    '    ROMFileTypes = New Dictionary(Of String, FileFormatControl)
    '    ROMFileTypes.Add("bgp", New BGPControl)
    'End Sub

    Public Overrides Sub Load(Manager As SkyEditor.Core.PluginManager)
        'Manager.RegisterIOFilter("*.nds", PluginHelper.GetLanguageItem("Nintendo DS ROM"))
        Manager.CurrentIOUIManager.RegisterIOFilter("*.img", My.Resources.Language.CTEImageFiles)

        Manager.RegisterTypeRegister(GetType(GenericModProject))

        'Manager.RegisterConsoleCommand("import-language", New ConsoleCommands.ImportLanguage)
        'Manager.RegisterConsoleCommand("cteconvert", New ConsoleCommands.BatchCteConvert)
        'Manager.RegisterConsoleCommand("gzip", New ConsoleCommands.Gzip)

        GameCodeRegistry.RegisterGameCode(My.Resources.Language.Game_OR, GameStrings.ORCode)
        GameCodeRegistry.RegisterGameCode(My.Resources.Language.Game_AS, GameStrings.ASCode)
        GameCodeRegistry.RegisterGameCode(My.Resources.Language.Game_X, GameStrings.PokemonXCode)
        GameCodeRegistry.RegisterGameCode(My.Resources.Language.Game_Y, GameStrings.PokemonYCode)
        GameCodeRegistry.RegisterGameCode(My.Resources.Language.Game_GTI, GameStrings.GTICode)
        GameCodeRegistry.RegisterGameCode(My.Resources.Language.Game_PSMD, GameStrings.PSMDCode)
    End Sub

    Public Overrides Sub UnLoad(Manager As SkyEditor.Core.PluginManager)
        PluginHelper.Writeline("Deleting ROM Editor's temp directory")
        Dim dir As String = PluginHelper.GetResourceName("Temp")
        If Directory.Exists(dir) Then
            On Error Resume Next
            Directory.Delete(dir, True)
            Directory.CreateDirectory(dir)
        End If
    End Sub

    Public Overrides Sub PrepareForDistribution()
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
        If Directory.Exists(Dir) Then
            Directory.Delete(Dir, True)
        End If
    End Sub
    Private Sub EnsureFileDeleted(Dir As String)
        If File.Exists(Dir) Then
            File.Delete(Dir)
        End If
    End Sub

End Class