Imports SkyEditor.Core
Imports SkyEditor.Core.Windows

''' <summary>
''' Most plugins need to call registration methods on load.  Sky Editor Base is no exception.  This class contains methods like the ones found in plugin definitions, without actually being its own plugin.
''' </summary>
Friend Class ServiceCoreMod
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
            Return My.Resources.Language.ServicePluginName
        End Get
    End Property

    Public Overrides Sub Load(Manager As SkyEditor.Core.PluginManager)
        MyBase.Load(Manager)
        'In the service environment, there's not much that can be done outside of just loading the plugins.
        'So, we're going to do nothing.  It's up to the plugins to define things.
    End Sub
    Public Overrides Sub UnLoad(Manager As SkyEditor.Core.PluginManager)

    End Sub

    Public Overrides Sub PrepareForDistribution()

    End Sub

    Public Overrides Function GetSettingsProvider(manager As SkyEditor.Core.PluginManager) As ISettingsProvider
        Return SettingsProvider.Open(EnvironmentPaths.GetSettingsFilename, manager)
    End Function

    Public Overrides Function GetExtensionDirectory() As String
        Return EnvironmentPaths.GetExtensionDirectory
    End Function
End Class
