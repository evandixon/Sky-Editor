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
        'CoreMod stuff
        Manager.RegisterTypeRegister(GetType(SolutionOld))
        Manager.RegisterTypeRegister(GetType(ProjectOld))
        Manager.RegisterTypeRegister(GetType(ICreatableFile))

        Manager.RegisterType(GetType(SolutionOld), GetType(SolutionOld))
        Manager.RegisterType(GetType(ProjectOld), GetType(ProjectOld))
        'End CoreMod stuff

        Manager.RegisterIOFilter("*.skysln", My.Resources.Language.SkyEditorSolution)
    End Sub
    Public Overrides Sub UnLoad(Manager As PluginManager)

    End Sub

    Public Overrides Sub PrepareForDistribution()

    End Sub
End Class
