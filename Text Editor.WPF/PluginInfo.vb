Imports CodeFiles
Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Public Class PluginInfo
    Implements SkyEditorBase.Interfaces.iSkyEditorPlugin

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
            Return PluginHelper.GetLanguageItem("Text Editor")
        End Get
    End Property

    Public Sub Load(Manager As PluginManager) Implements iSkyEditorPlugin.Load
        Manager.LoadPlugin(New CodeFiles.PluginDefinition)
    End Sub

    Public Sub PrepareForDistribution() Implements iSkyEditorPlugin.PrepareForDistribution

    End Sub

    Public Sub UnLoad(Manager As PluginManager) Implements iSkyEditorPlugin.UnLoad

    End Sub
End Class
