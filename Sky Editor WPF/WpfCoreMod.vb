Imports SkyEditor.Core
Imports SkyEditor.Core.UI
Imports SkyEditor.Core.Windows
''' <summary>
''' Most plugins need to call registration methods on load.  Sky Editor Base is no exception.  This class contains methods like the ones found in plugin definitions, without actually being its own plugin.
''' </summary>
Friend Class WpfCoreMod
    Inherits WpfCoreModBase

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
        MyBase.Load(Manager)
        'CoreMod stuff
        Manager.RegisterTypeRegister(GetType(IObjectControl))
        Manager.RegisterTypeRegister(GetType(ITargetedControl))
        'End CoreMod stuff

        Manager.CurrentIOUIManager.RegisterIOFilter("*.skysln", My.Resources.Language.SkyEditorSolution)
    End Sub

    Public Overrides Function GetSettingsProvider(manager As PluginManager) As ISettingsProvider
        Return SettingsProvider.Open(System.IO.Path.Combine(EnvironmentPaths.GetRootResourceDirectory, "settings.json"), manager)
    End Function

    Public Overrides Function GetExtensionDirectory() As String
        Return EnvironmentPaths.GetExtensionDirectory
    End Function

End Class
