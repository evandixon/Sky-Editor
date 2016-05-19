Imports SkyEditor.Core
Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Windows
''' <summary>
''' Most plugins need to call registration methods on load.  Sky Editor Base is no exception.  This class contains methods like the ones found in plugin definitions, without actually being its own plugin.
''' </summary>
Friend Class ConsoleCoreMod
    Inherits WindowsCoreSkyEditorPlugin

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
            Return My.Resources.Language.SkyEditorConsole
        End Get
    End Property

    Public Overrides Sub Load(Manager As PluginManager)
        MyBase.Load(Manager)

        Manager.CurrentIOUIManager.RegisterIOFilter("*.skysln", My.Resources.Language.SkyEditorSolution)
    End Sub
    Public Overrides Sub UnLoad(Manager As PluginManager)

    End Sub

    Public Overrides Sub PrepareForDistribution()

    End Sub

    Public Overrides Function GetSettingsProvider(manager As SkyEditor.Core.PluginManager) As ISettingsProvider
        Return SettingsProvider.Open(System.IO.Path.Combine(PluginHelper.RootResourceDirectory, "settings.json"), manager)
    End Function

    Public Overrides Function GetExtensionDirectory() As String
        Return PluginHelper.GetExtensionDirectory
    End Function
End Class
