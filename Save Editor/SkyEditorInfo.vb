Imports SkyEditorBase
Imports SkyEditorBase.Interfaces
Imports SaveEditor.Saves
Public Class SkyEditorInfo
    Implements iSkyEditorPlugin
    Public ReadOnly Property PluginAuthor As String Implements iSkyEditorPlugin.PluginAuthor
        Get
            Return "evandixon"
        End Get
    End Property

    Public ReadOnly Property PluginName As String Implements iSkyEditorPlugin.PluginName
        Get
            Return "Sky Editor"
        End Get
    End Property

    Public ReadOnly Property Credits As String Implements iSkyEditorPlugin.Credits
        Get
            Return PluginHelper.GetLanguageItem("SkyEditorPlgCredits", "Sky Editor Credits:\n     evandixon (General Research)\n     matix2267 (Pokemon Structure, code for interacting with bits)\n     Grovyle91 (Item Structure, IDs of Pokemon/Items/etc)\n     Prof. 9 (Team Name character encoding)\n     Demonic722 (Misc RAM and save addresses)")
        End Get
    End Property

    Public Sub Load(Manager As PluginManager) Implements iSkyEditorPlugin.Load
        'Manager.RegisterEditorTab(GetType(Tabs.SkyGeneralTab))
        'Manager.RegisterEditorTab(GetType(Tabs.TDGeneralTab))
        'Manager.RegisterEditorTab(GetType(Tabs.RBGeneral))
        'Manager.RegisterEditorTab(GetType(Tabs.HeldItemsTab))
        'Manager.RegisterEditorTab(GetType(Tabs.RBStoredItemsTab))
        'Manager.RegisterEditorTab(GetType(Tabs.ActivePokemonTab))
        'Manager.RegisterEditorTab(GetType(Tabs.EpisodeActivePokemonTab))
        'Manager.RegisterEditorTab(GetType(Tabs.StoredPokemonTab))
        'Manager.RegisterEditorTab(GetType(Tabs.QuicksavePokemonTab))

        'Manager.RegisterEditorTab(GetType(Tabs.PKMGeneralTab))
        'Manager.RegisterEditorTab(GetType(Tabs.PkmMovesTab))

        'Manager.RegisterObjectControl(New Controls.MDAttack)
        'Manager.RegisterObjectControl(New Controls.iAttack)

        'Manager.RegisterSaveTypeDetector(AddressOf DetectSaveType)
        Manager.RegisterIOFilter("*.sav", PluginHelper.GetLanguageItem("Raw Save File"))
        Manager.RegisterIOFilter("*.dsv", PluginHelper.GetLanguageItem("DeSmuMe Save File"))

        'Manager.RegisterSaveGameFormat(GameStrings.BlueGame, GameStrings.RBSave, GetType(RBSave))
        'Manager.RegisterSaveGameFormat(GameStrings.RedGame, GameStrings.RBSave, GetType(RBSave))
        'Manager.RegisterSaveGameFormat(GameStrings.BlueGameEU, GameStrings.RBSaveEU, GetType(RBSaveEU))
        'Manager.RegisterSaveGameFormat(GameStrings.RedGameEU, GameStrings.RBSave, GetType(RBSaveEU))
        'Manager.RegisterSaveGameFormat(GameStrings.TimeGame, GameStrings.TDSave, GetType(TDSave))
        'Manager.RegisterSaveGameFormat(GameStrings.DarknessGame, GameStrings.TDSave, GetType(TDSave))
        'Manager.RegisterSaveGameFormat(GameStrings.SkyGame, GameStrings.SkySave, GetType(SkySave))
        'Manager.RegisterSaveGameFormat(GameStrings.MDGatesData, GameStrings.MDGatesData, GetType(GatesGameData))

        'Manager.RegisterCodeGenerator(New BlueBaseType)
        'Manager.RegisterCodeGenerator(New BlueHeldMoney)
        'Manager.RegisterCodeGenerator(New BlueRescuePoints)
        'Manager.RegisterCodeGenerator(New BlueStoredMoney)
        'Manager.RegisterCodeGenerator(New BlueTeamName)

        'Manager.RegisterCodeGenerator(New RedBaseType)
        'Manager.RegisterCodeGenerator(New RedHeldMoney)
        'Manager.RegisterCodeGenerator(New RedRescuePoints)
        'Manager.RegisterCodeGenerator(New RedStoredMoney)
        'Manager.RegisterCodeGenerator(New RedTeamName)

        'Manager.RegisterResourceFile(IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins", "xceed.wpf.toolkit.dll"))

        Manager.RegisterMenuActionType(GetType(MenuActions.ImportSdf))
        Manager.RegisterMenuActionType(GetType(MenuActions.OpenSdfSave))

        Manager.RegisterDirectoryTypeDetector(AddressOf Me.DirectoryDetector)
    End Sub

    Public Function DetectSaveType(File As GenericFile) As String
        Try
            If File.Length > &HD Then
                Select Case File.RawData(&HD)
                    Case &H54
                        Return GameStrings.TDSave
                    Case &H53
                        Return GameStrings.SkySave
                    Case Else
                        If (File.RawData(&H404) = &H50 AndAlso File.RawData(&H405) = &H4F AndAlso File.RawData(&H406) = &H4B AndAlso File.RawData(&H407) = &H45) Then
                            Return GameStrings.RBSave
                        ElseIf File.OriginalFilename.ToLower.EndsWith("00000ba8\game_data") Then
                            Return GameStrings.MDGatesData
                        Else
                            Return Nothing
                        End If
                End Select
            Else
                Return Nothing
            End If
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function DirectoryDetector(Directory As IO.DirectoryInfo) As IEnumerable(Of Type)
        Dim s As New SdfSave(Directory.FullName)
        If s.IsValid Then
            Select Case s.MiniTitleId.ToLower
                Case GameStrings.GTIMiniTitleID
                    Return {GetType(GTISave)}
                Case Else
                    Return {GetType(SdfSave)}
            End Select
        Else
            Return {}
        End If
    End Function

    Public Sub UnLoad(Manager As PluginManager) Implements iSkyEditorPlugin.UnLoad

    End Sub

    Public Sub PrepareForDistribution() Implements iSkyEditorPlugin.PrepareForDistribution

    End Sub
End Class