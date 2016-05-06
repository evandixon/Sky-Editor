Imports SkyEditor.Core

Public Class PluginInfo
    Inherits SkyEditorPlugin

    Public Overrides ReadOnly Property Credits As String
        Get
            Return ""
        End Get
    End Property

    Public Overrides ReadOnly Property PluginAuthor As String
        Get
            Return "evandixon"
        End Get
    End Property

    Public Overrides ReadOnly Property PluginName As String
        Get
            Return My.Resources.Language.TextEditor
        End Get
    End Property

    Public Overrides Sub Load(Manager As PluginManager)
        Manager.LoadRequiredPlugin(New CodeFiles.PluginDefinition, Me)
    End Sub

    Public Overrides Sub PrepareForDistribution()

    End Sub

    Public Overrides Sub UnLoad(Manager As PluginManager)

    End Sub
End Class
