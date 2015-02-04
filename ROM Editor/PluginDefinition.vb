Imports SkyEditorBase
Imports System.Windows.Controls
Imports ROMEditor.PMD_Explorers

Public Class PluginDefinition
    Implements iSkyEditorPlugin
    Public Function AutoDetectSaveType(File As GenericFile) As String
        Dim out As String = ""
        If New GenericNDSRom(File.RawData).ROMHeader = Constants.SkyROMHeader Then out = Constants.SkyNDSRom
        Return out
    End Function

    Public ReadOnly Property Credits As String Implements iSkyEditorPlugin.Credits
        Get
            Return ""
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
        DeveloperConsole.Writeline(SkyEditorBase.PluginHelper.GetResourceName("Root"))
        Manager.RegisterConsoleCommand("header", AddressOf ConsoleCommands.ROMHeader)
        Manager.RegisterConsoleCommand("unpack", AddressOf ConsoleCommands.UnPack)
        Manager.RegisterConsoleCommand("repack", AddressOf ConsoleCommands.RePack)
        Manager.RegisterConsoleCommand("explorersextractbgp", AddressOf ConsoleCommands.ExplorersExtractBGP)
        'Window.RegisterConsoleCommand("kaomadopatch", AddressOf ConsoleCommands.KaomadoPatch)
        Manager.RegisterConsoleCommand("pmdlanguage", AddressOf ConsoleCommands.PmdLanguage)
        Manager.RegisterConsoleCommand("eostestmusic", AddressOf ConsoleCommands.EoSTestMusic)

        Manager.RegisterIOFilter("*.nds", "Nintendo DS ROM")

        'Window.RegisterMenuItem(New RomEditorMenuItem(Window))

        Manager.RegisterSaveType(Constants.GenericNDSRom, GetType(GenericNDSRom))
        Manager.RegisterSaveType(Constants.SkyNDSRom, GetType(SkyNDSRom))

        Manager.RegisterGameType(Constants.GenericNDSRom, Constants.GenericNDSRom)
        Manager.RegisterGameType(Constants.SkyNDSRom, Constants.SkyNDSRom)

        Manager.RegisterEditorTab(GetType(PortraitTab))
        Manager.RegisterEditorTab(GetType(PersonalityTest))
        Manager.RegisterEditorTab(GetType(FilesTab))

        Manager.RegisterSaveTypeDetector(AddressOf AutoDetectSaveType)
    End Sub

    Public Sub UnLoad(ByRef Manager As PluginManager) Implements iSkyEditorPlugin.UnLoad
        DeveloperConsole.Writeline("Deleting ROM Editor's temp directory")
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
