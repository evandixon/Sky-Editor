Imports ROMEditor.Projects
Imports SkyEditor.Core
Imports System.IO
Imports SkyEditor.Core.Windows

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

    Public Overrides Sub UnLoad(Manager As PluginManager)
        Dim dir As String = EnvironmentPaths.GetResourceName("Temp")
        If Directory.Exists(dir) Then
            On Error Resume Next
            Directory.Delete(dir, True)
            Directory.CreateDirectory(dir)
        End If
    End Sub

    Public Overrides Sub PrepareForDistribution()
        EnsureDirDeleted(EnvironmentPaths.GetResourceName("Current"))
        EnsureDirDeleted(EnvironmentPaths.GetResourceName("temp"))
        EnsureDirDeleted(EnvironmentPaths.GetResourceName("desmume-0.9.11-win32"))
        EnsureDirDeleted(EnvironmentPaths.GetResourceName("desmume-0.9.11-win64"))
        EnsureFileDeleted(EnvironmentPaths.GetResourceName("Current.nds"))
        EnsureFileDeleted(EnvironmentPaths.GetResourceName("3DS Builder.exe.config"))
        EnsureFileDeleted(EnvironmentPaths.GetResourceName("3DS Builder.pdb"))
        EnsureFileDeleted(EnvironmentPaths.GetResourceName("3DS Builder.vshost.exe"))
        EnsureFileDeleted(EnvironmentPaths.GetResourceName("3DS Builder.vshost.exe.config"))
        EnsureFileDeleted(EnvironmentPaths.GetResourceName("3DS Builder.vshost.exe.manifest"))
        EnsureFileDeleted(EnvironmentPaths.GetResourceName("DSPatcher.exe.config"))
        EnsureFileDeleted(EnvironmentPaths.GetResourceName("DSPatcher.pdb"))
        EnsureFileDeleted(EnvironmentPaths.GetResourceName("DSPatcher.vshost.exe"))
        EnsureFileDeleted(EnvironmentPaths.GetResourceName("DSPatcher.vshost.exe.config"))
        EnsureFileDeleted(EnvironmentPaths.GetResourceName("DSPatcher.vshost.exe.manifest"))
        EnsureFileDeleted(EnvironmentPaths.GetResourceName("DSPatcher.xml"))
        EnsureFileDeleted(EnvironmentPaths.GetResourceName("itemppatcher.exe.config"))
        EnsureFileDeleted(EnvironmentPaths.GetResourceName("itemppatcher.pdb"))
        EnsureFileDeleted(EnvironmentPaths.GetResourceName("LanguageStringPatcher.exe.config"))
        EnsureFileDeleted(EnvironmentPaths.GetResourceName("LanguageStringPatcher.pdb"))
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