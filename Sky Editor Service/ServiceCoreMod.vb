Imports SkyEditor.Core.Extensions.Plugins

''' <summary>
''' Most plugins need to call registration methods on load.  Sky Editor Base is no exception.  This class contains methods like the ones found in plugin definitions, without actually being its own plugin.
''' </summary>
Friend Class ServiceCoreMod
    Implements ISkyEditorPlugin

    Public ReadOnly Property Credits As String Implements ISkyEditorPlugin.Credits
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property PluginAuthor As String Implements ISkyEditorPlugin.PluginAuthor
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property PluginName As String Implements ISkyEditorPlugin.PluginName
        Get
            Return My.Resources.Language.ServicePluginName
        End Get
    End Property

    Public Sub Load(Manager As PluginManager) Implements ISkyEditorPlugin.Load
        'In the service environment, there's not much that can be done outside of just loading the plugins.
        'So, we're going to do nothing.  It's up to the plugins to define things.
    End Sub
    Public Sub UnLoad(Manager As PluginManager) Implements ISkyEditorPlugin.UnLoad

    End Sub

    Public Sub PrepareForDistribution() Implements ISkyEditorPlugin.PrepareForDistribution

    End Sub
End Class
