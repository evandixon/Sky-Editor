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

    Public Sub Load(ByRef Window As iMainWindow) Implements iSkyEditorPlugin.Load
        DeveloperConsole.Writeline(SkyEditorBase.PluginHelper.GetResourceName("Root"))

        Window.RegisterEditorTab(GetType(SkyGeneralTab))
        Window.RegisterEditorTab(GetType(TDGeneralTab))
        Window.RegisterEditorTab(GetType(RBGeneral))
        Window.RegisterEditorTab(GetType(HeldItemsTab))
        Window.RegisterEditorTab(GetType(EpisodeHeldItems))
        Window.RegisterEditorTab(GetType(TDSStoredItems))
        Window.RegisterEditorTab(GetType(RBStoredItemsTab))
        Window.RegisterEditorTab(GetType(ActivePokemonTab))
        Window.RegisterEditorTab(GetType(EpisodeActivePokemon))
        Window.RegisterEditorTab(GetType(RBStoredPokemonTab))
        Window.RegisterEditorTab(GetType(QuicksavePokemonTab))

        Window.RegisterSaveTypeDetector(AddressOf DetectSaveType)

        Window.RegisterIOFilter("sav", "Raw Save File")
        Window.RegisterIOFilter("dsv", "DeSmuMe Save File")

        Window.RegisterSaveType(GameConstants.RBSave, GetType(RBSave))
        Window.RegisterSaveType(GameConstants.TDSave, GetType(TDSave))
        Window.RegisterSaveType(GameConstants.SkySave, GetType(SkySave))
        Window.RegisterSaveType(GameConstants.RBSaveEU, GetType(RBSaveEU))

        Window.RegisterGameType(GameConstants.RedGame, GameConstants.RBSave)
        Window.RegisterGameType(GameConstants.BlueGame, GameConstants.RBSave)
        Window.RegisterGameType(GameConstants.TimeGame, GameConstants.TDSave)
        Window.RegisterGameType(GameConstants.DarknessGame, GameConstants.TDSave)
        Window.RegisterGameType(GameConstants.SkyGame, GameConstants.SkySave)
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

    Public Sub UnLoad(ByRef Window As iMainWindow) Implements iSkyEditorPlugin.UnLoad

    End Sub

    Public Sub PrepareForDistribution() Implements iSkyEditorPlugin.PrepareForDistribution

    End Sub
End Class
