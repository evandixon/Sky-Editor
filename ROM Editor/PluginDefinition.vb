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

    Public Function AutoDetect3dsRom(File As GenericFile) As TypeInfo()
        If File.Length > &H115A Then
            Dim e As New System.Text.ASCIIEncoding
            If e.GetString(File.RawData(&H100, 4)) = "NCSD" Then 'This is a 3DS ROM
                Return {GetType(Generic3DSRom).GetTypeInfo}
            Else
                Return {}
            End If
        Else
            Return {}
        End If
    End Function

    Public Function FileFormatDetector(File As GenericFile) As TypeInfo()
        If File.Length > &H4 Then
            If File.RawData(0) = 0 AndAlso File.RawData(1) = &H63 AndAlso File.RawData(2) = &H74 AndAlso File.RawData(3) = &H65 Then
                Return {GetType(CteImage).GetTypeInfo}
            Else
                Return {}
            End If
        Else
            Return {}
        End If
    End Function

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
        PluginHelper.Writeline(SkyEditorBase.PluginHelper.GetResourceName("Root"))
        'Manager.RegisterIOFilter("*.nds", PluginHelper.GetLanguageItem("Nintendo DS ROM"))
        Manager.RegisterIOFilter("*.img", My.Resources.Language.CTEImageFiles)

        Manager.RegisterFileTypeDetector(AddressOf AutoDetect3dsRom)
        Manager.RegisterFileTypeDetector(AddressOf FileFormatDetector)

        Manager.RegisterTypeRegister(GetType(GenericModProject))
        Manager.RegisterTypeRegister(GetType(Flashcart.GenericFlashcart))
        Manager.RegisterTypeRegister(GetType(Flashcart.FileCollection))

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