Imports SkyEditor.Core
Imports SkyEditor.Core.Windows

''' <summary>
''' Core of a WPF Core Mod that handles environment specific things, like how to restart the application.
''' </summary>
Public MustInherit Class WpfCoreModBase
    Inherits WindowsCoreSkyEditorPlugin
    Public MustOverride Overrides ReadOnly Property Credits As String

    Public MustOverride Overrides ReadOnly Property PluginAuthor As String

    Public MustOverride Overrides ReadOnly Property PluginName As String

    Public Overrides Sub Load(Manager As PluginManager)
        MyBase.Load(Manager)
        AddHandler Redistribution.RedistributionHelpers.ApplicationRestartRequested, AddressOf WpfCoreModBase.RestartApplication
    End Sub

    Public Overrides Sub PrepareForDistribution()

    End Sub

    Public Overrides Sub UnLoad(Manager As PluginManager)

    End Sub

    Private Shared Sub RestartApplication()
        System.Windows.Forms.Application.Restart()
        Application.Current.Shutdown(1)
    End Sub
End Class
