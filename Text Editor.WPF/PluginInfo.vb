Imports SkyEditor.Core.Extensions.Plugins

Public Class PluginInfo
    Implements ISkyEditorPlugin

    Public ReadOnly Property Credits As String Implements ISkyEditorPlugin.Credits
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property PluginAuthor As String Implements ISkyEditorPlugin.PluginAuthor
        Get
            Return "evandixon"
        End Get
    End Property

    Public ReadOnly Property PluginName As String Implements ISkyEditorPlugin.PluginName
        Get
            Return My.Resources.Language.TextEditor
        End Get
    End Property

    Public Sub Load(Manager As PluginManager) Implements ISkyEditorPlugin.Load
        Manager.LoadPlugin(New CodeFiles.PluginDefinition)
    End Sub

    Public Sub PrepareForDistribution() Implements ISkyEditorPlugin.PrepareForDistribution

    End Sub

    Public Sub UnLoad(Manager As PluginManager) Implements ISkyEditorPlugin.UnLoad

    End Sub
End Class
