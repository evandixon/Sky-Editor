Imports SkyEditorBase.Interfaces

''' <summary>
''' Core of a WPF Core Mod that handles environment specific things, like how to restart the application.
''' </summary>
Public MustInherit Class WpfCoreModBase
    Implements iSkyEditorPlugin
    Public MustOverride ReadOnly Property Credits As String Implements iSkyEditorPlugin.Credits

    Public MustOverride ReadOnly Property PluginAuthor As String Implements iSkyEditorPlugin.PluginAuthor

    Public MustOverride ReadOnly Property PluginName As String Implements iSkyEditorPlugin.PluginName

    Public Overridable Sub Load(Manager As PluginManager) Implements iSkyEditorPlugin.Load
        AddHandler Redistribution.RedistributionHelpers.ApplicationRestartRequested, AddressOf WpfCoreModBase.RestartApplication
    End Sub

    Public Overridable Sub PrepareForDistribution() Implements iSkyEditorPlugin.PrepareForDistribution

    End Sub

    Public Overridable Sub UnLoad(Manager As PluginManager) Implements iSkyEditorPlugin.UnLoad

    End Sub

    Private Shared Sub RestartApplication()
        Windows.Forms.Application.Restart()
        Application.Current.Shutdown(1)
    End Sub
End Class
