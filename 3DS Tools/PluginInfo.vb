Imports SkyEditorBase

Public Class PluginInfo
    Implements SkyEditorBase.iSkyEditorPlugin
    Public Function DetectSaveType(File As GenericFile) As String
        If File.OriginalFilename.ToLower.EndsWith("00000ba8\game_data") Then
            Return GameConstants.MDGatesData
        Else
            Return Nothing
        End If
    End Function
    Public ReadOnly Property Credits As String Implements SkyEditorBase.iSkyEditorPlugin.Credits
        Get
            Return SkyEditorBase.PluginHelper.GetLanguageItem("3DSToolsCredits", "-Developer: evandixon")
        End Get
    End Property

    Public Sub Load(ByRef Manager As SkyEditorBase.PluginManager) Implements SkyEditorBase.iSkyEditorPlugin.Load
        Manager.RegisterSaveGameFormat(GameConstants.ThreeDSRom, GameConstants.ThreeDSRom, GetType(ThreeDSRom))
        Manager.RegisterSaveGameFormat(GameConstants.MDGatesData, GameConstants.MDGatesData, GetType(Saves.GatesGameData))
        'Manager.RegisterEditorTab(GetType(Tabs.GIGDGeneralTab))
        Manager.RegisterSaveTypeDetector(AddressOf DetectSaveType)
    End Sub

    Public ReadOnly Property PluginAuthor As String Implements SkyEditorBase.iSkyEditorPlugin.PluginAuthor
        Get
            Return "evandixon"
        End Get
    End Property

    Public ReadOnly Property PluginName As String Implements SkyEditorBase.iSkyEditorPlugin.PluginName
        Get
            Return "3DS Tools"
        End Get
    End Property

    Public Sub PrepareForDistribution() Implements SkyEditorBase.iSkyEditorPlugin.PrepareForDistribution

    End Sub

    Public Sub UnLoad(ByRef Manager As SkyEditorBase.PluginManager) Implements SkyEditorBase.iSkyEditorPlugin.UnLoad

    End Sub
End Class