Imports SkyEditor.Core
Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI
Imports SkyEditorBase.Interfaces
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
            Return My.Resources.Language.Author
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
        Manager.RegisterTypeRegister(GetType(iObjectControl))
        Manager.RegisterTypeRegister(GetType(SolutionOld))
        Manager.RegisterTypeRegister(GetType(ProjectOld))
        Manager.RegisterTypeRegister(GetType(ICreatableFile))
        Manager.RegisterTypeRegister(GetType(ITargetedControl))
        'End CoreMod stuff

        Manager.RegisterIOFilter("*.skysln", My.Resources.Language.SkyEditorSolution)
    End Sub

End Class
