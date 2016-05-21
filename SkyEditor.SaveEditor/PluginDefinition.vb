Imports SkyEditor.Core

Public Class PluginDefinition
    Inherits SkyEditor.Core.SkyEditorPlugin

    Public Overrides ReadOnly Property Credits As String
        Get
            Return ""
        End Get
    End Property

    Public Overrides ReadOnly Property PluginAuthor As String
        Get
            Return ""
        End Get
    End Property

    Public Overrides ReadOnly Property PluginName As String
        Get
            Return ""
        End Get
    End Property

    Public Overrides Sub Load(manager As PluginManager)
        MyBase.Load(manager)
        manager.CurrentIOUIManager.RegisterIOFilter("*.sav", My.Resources.Language.RawSaveFile)
    End Sub
End Class
