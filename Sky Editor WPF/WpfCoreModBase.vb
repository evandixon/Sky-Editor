Imports SkyEditor.Core.Extensions.Plugins

''' <summary>
''' Core of a WPF Core Mod that handles environment specific things, like how to restart the application.
''' </summary>
Public MustInherit Class WpfCoreModBase
    Inherits SkyEditorPlugin
    Public MustOverride Overrides ReadOnly Property Credits As String

    Public MustOverride Overrides ReadOnly Property PluginAuthor As String

    Public MustOverride Overrides ReadOnly Property PluginName As String

    Public Overrides Sub Load(Manager As PluginManager)
        AddHandler Redistribution.RedistributionHelpers.ApplicationRestartRequested, AddressOf WpfCoreModBase.RestartApplication
    End Sub

    Public Overrides Sub PrepareForDistribution()

    End Sub

    Public Overrides Sub UnLoad(Manager As PluginManager)

    End Sub

    Private Shared Sub RestartApplication()
        Windows.Forms.Application.Restart()
        Application.Current.Shutdown(1)
    End Sub
End Class
