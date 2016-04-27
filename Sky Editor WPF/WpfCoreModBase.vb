Imports SkyEditor.Core.Extensions.Plugins

''' <summary>
''' Core of a WPF Core Mod that handles environment specific things, like how to restart the application.
''' </summary>
Public MustInherit Class WpfCoreModBase
    Implements ISkyEditorPlugin
    Public MustOverride ReadOnly Property Credits As String Implements ISkyEditorPlugin.Credits

    Public MustOverride ReadOnly Property PluginAuthor As String Implements ISkyEditorPlugin.PluginAuthor

    Public MustOverride ReadOnly Property PluginName As String Implements ISkyEditorPlugin.PluginName

    Public Overridable Sub Load(Manager As PluginManager) Implements ISkyEditorPlugin.Load
        AddHandler Redistribution.RedistributionHelpers.ApplicationRestartRequested, AddressOf WpfCoreModBase.RestartApplication
    End Sub

    Public Overridable Sub PrepareForDistribution() Implements ISkyEditorPlugin.PrepareForDistribution

    End Sub

    Public Overridable Sub UnLoad(Manager As PluginManager) Implements ISkyEditorPlugin.UnLoad

    End Sub

    Private Shared Sub RestartApplication()
        Windows.Forms.Application.Restart()
        Application.Current.Shutdown(1)
    End Sub
End Class
