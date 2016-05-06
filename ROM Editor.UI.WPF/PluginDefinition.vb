Imports SkyEditor.Core

Public Class PluginDefinition
    Inherits SkyEditorPlugin

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

    Public Overrides Sub Load(Manager As PluginManager)
        'Load the plugin this one depends on
        Manager.LoadRequiredPlugin(New ROMEditor.PluginDefinition, Me)
    End Sub

    Public Overrides Sub UnLoad(Manager As PluginManager)

    End Sub

    Public Overrides Sub PrepareForDistribution()

    End Sub
End Class