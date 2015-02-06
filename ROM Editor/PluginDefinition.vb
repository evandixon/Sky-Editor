Imports SkyEditorBase
Imports System.Windows.Controls
Imports ROMEditor.PMD_Explorers

Public Class PluginDefinition
    Implements iSkyEditorPlugin
    Public Function AutoDetectSaveType(File As GenericFile) As String
        Dim out As String = ""
        If New GenericNDSRom(File.RawData).ROMHeader = GameStrings.SkyROMHeader Then out = GameStrings.SkyNDSRom
        Return out
    End Function

    Public ReadOnly Property Credits As String Implements iSkyEditorPlugin.Credits
        Get
            Return PluginHelper.GetLanguageItem("RomEditorCredits", "Rom Editor Credits:\n     psy_commando (Pokemon portraits, misc file formats)\n     Grovyle91 (Language strings)\n     evandixon (Personality test, bgp files)")
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

    Public Property ROMFileTypes As Dictionary(Of String, FileFormatControl)

    Public Sub New()
        ROMFileTypes = New Dictionary(Of String, FileFormatControl)
        ROMFileTypes.Add("bgp", New BGPControl)
    End Sub

    Public Sub Load(ByRef Manager As PluginManager) Implements iSkyEditorPlugin.Load
        PluginHelper.Writeline(SkyEditorBase.PluginHelper.GetResourceName("Root"))
        Manager.RegisterConsoleCommand("header", AddressOf ConsoleCommands.ROMHeader)
        Manager.RegisterConsoleCommand("unpack", AddressOf ConsoleCommands.UnPack)
        Manager.RegisterConsoleCommand("repack", AddressOf ConsoleCommands.RePack)
        Manager.RegisterConsoleCommand("explorersextractbgp", AddressOf ConsoleCommands.ExplorersExtractBGP)
        'Window.RegisterConsoleCommand("kaomadopatch", AddressOf ConsoleCommands.KaomadoPatch)
        Manager.RegisterConsoleCommand("pmdlanguage", AddressOf ConsoleCommands.PmdLanguage)
        Manager.RegisterConsoleCommand("eostestmusic", AddressOf ConsoleCommands.EoSTestMusic)

        Manager.RegisterIOFilter("*.nds", PluginHelper.GetLanguageItem("Nintendo DS ROM"))

        Manager.RegisterSaveType(GameStrings.GenericNDSRom, GetType(GenericNDSRom))
        Manager.RegisterSaveType(GameStrings.SkyNDSRom, GetType(SkyNDSRom))

        Manager.RegisterGameType(GameStrings.GenericNDSRom, GameStrings.GenericNDSRom)
        Manager.RegisterGameType(GameStrings.SkyNDSRom, GameStrings.SkyNDSRom)

        Manager.RegisterEditorTab(GetType(PortraitTab))
        Manager.RegisterEditorTab(GetType(PersonalityTest))
        Manager.RegisterEditorTab(GetType(FilesTab))

        Manager.RegisterSaveTypeDetector(AddressOf AutoDetectSaveType)
    End Sub

    Public Sub UnLoad(ByRef Manager As PluginManager) Implements iSkyEditorPlugin.UnLoad
        PluginHelper.Writeline("Deleting ROM Editor's temp directory")
        Dim directory As String = IO.Path.Combine(Environment.CurrentDirectory, "Resources\Plugins\ROMEditor\Temp")
        If IO.Directory.Exists(directory) Then
            IO.Directory.Delete(directory, True)
            IO.Directory.CreateDirectory(directory)
        End If
    End Sub

    Public Sub PrepareForDistribution() Implements iSkyEditorPlugin.PrepareForDistribution
        EnsureDirDeleted(IO.Path.Combine(Environment.CurrentDirectory, "Resources\Plugins\ROMEditor\Current"))
        EnsureFileDeleted(IO.Path.Combine(Environment.CurrentDirectory, "Resources\Plugins\ROMEditor\Current.nds"))
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
    Public Shared Function GetResourceDirectory() As String
        Return IO.Path.Combine(Environment.CurrentDirectory, "Resources/Plugins/ROMEditor/")
    End Function
End Class
