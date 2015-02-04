Imports SkyEditorBase
Public Class SkyEditorInfo
    Implements iSkyEditorPlugin
    Public ReadOnly Property PluginAuthor As String Implements SkyEditorBase.iSkyEditorPlugin.PluginAuthor
        Get
            Return "evandixon"
        End Get
    End Property

    Public ReadOnly Property PluginName As String Implements SkyEditorBase.iSkyEditorPlugin.PluginName
        Get
            Return "Sky Editor"
        End Get
    End Property

    Public ReadOnly Property Credits As String Implements iSkyEditorPlugin.Credits
        Get
            Return PluginHelper.GetLanguageItem("SkyEditorPlgCredits", "Sky Editor Credits:\n     evandixon (General Research)\n     matix2267(Pokemon Stucture, code for interacting with bits)\n     Grovyle91 (Item Structure, IDs of Pokemon/Items/etc)\n     Prof. 9 (Team Name character encoding)\n     Demonic722 (Misc RAM and save addresses)")
        End Get
    End Property

    Public Sub Load(ByRef Manager As PluginManager) Implements iSkyEditorPlugin.Load
        DeveloperConsole.Writeline(SkyEditorBase.PluginHelper.GetResourceName("Root"))

        Manager.RegisterEditorTab(GetType(SkyGeneralTab))
        Manager.RegisterEditorTab(GetType(TDGeneralTab))
        Manager.RegisterEditorTab(GetType(RBGeneral))
        Manager.RegisterEditorTab(GetType(HeldItemsTab))
        Manager.RegisterEditorTab(GetType(EpisodeHeldItems))
        Manager.RegisterEditorTab(GetType(TDSStoredItems))
        Manager.RegisterEditorTab(GetType(RBStoredItemsTab))
        Manager.RegisterEditorTab(GetType(ActivePokemonTab))
        Manager.RegisterEditorTab(GetType(EpisodeActivePokemon))
        Manager.RegisterEditorTab(GetType(RBStoredPokemonTab))
        Manager.RegisterEditorTab(GetType(QuicksavePokemonTab))

        Manager.RegisterSaveTypeDetector(AddressOf DetectSaveType)
        Manager.RegisterIOFilter("*.sav", "Raw Save File")
        Manager.RegisterIOFilter("*.dsv", "DeSmuMe Save File")

        Manager.RegisterSaveType(GameConstants.RBSave, GetType(RBSave))
        Manager.RegisterSaveType(GameConstants.TDSave, GetType(TDSave))
        Manager.RegisterSaveType(GameConstants.SkySave, GetType(SkySave))
        Manager.RegisterSaveType(GameConstants.RBSaveEU, GetType(RBSaveEU))
        Manager.RegisterGameType(GameConstants.RedGame, GameConstants.RBSave)
        Manager.RegisterGameType(GameConstants.BlueGame, GameConstants.RBSave)
        Manager.RegisterGameType(GameConstants.TimeGame, GameConstants.TDSave)
        Manager.RegisterGameType(GameConstants.DarknessGame, GameConstants.TDSave)
        Manager.RegisterGameType(GameConstants.SkyGame, GameConstants.SkySave)
    End Sub

    Public Function DetectSaveType(File As GenericFile) As String
        Select Case File.RawData(&HD)
            Case &H54
                Return GameConstants.TDSave
            Case &H53
                Return GameConstants.SkySave
            Case Else
                If (File.RawData(&H404) = &H50 AndAlso File.RawData(&H405) = &H4F AndAlso File.RawData(&H406) = &H4B AndAlso File.RawData(&H407) = &H45) Then
                    Return GameConstants.RBSave
                Else
                    Return Nothing
                End If
        End Select
    End Function

    Public Sub UnLoad(ByRef Manager As PluginManager) Implements iSkyEditorPlugin.UnLoad

    End Sub

    Public Sub PrepareForDistribution() Implements iSkyEditorPlugin.PrepareForDistribution

    End Sub
End Class
