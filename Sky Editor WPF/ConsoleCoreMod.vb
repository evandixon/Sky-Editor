Imports SkyEditor.Core.Extensions.Plugins
Imports SkyEditor.Core.Interfaces
''' <summary>
''' Most plugins need to call registration methods on load.  Sky Editor Base is no exception.  This class contains methods like the ones found in plugin definitions, without actually being its own plugin.
''' </summary>
Friend Class ConsoleCoreMod
    Inherits SkyEditorPlugin

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
        'CoreMod stuff
        Manager.RegisterTypeRegister(GetType(SolutionOld))
        Manager.RegisterTypeRegister(GetType(ProjectOld))
        Manager.RegisterTypeRegister(GetType(iCreatableFile))
        Manager.RegisterTypeRegister(GetType(iOpenableFile))
        Manager.RegisterTypeRegister(GetType(iDetectableFileType))
        Manager.RegisterTypeRegister(GetType(ConsoleCommandAsync))

        Manager.RegisterType(GetType(SolutionOld), GetType(SolutionOld))
        Manager.RegisterType(GetType(ProjectOld), GetType(ProjectOld))

        Manager.RegisterDefaultFileTypeDetectors()
        'End CoreMod stuff

        'Manager.OpenableFiles.Add(GetType(ExecutableFile))

        Manager.RegisterIOFilter("*.skysln", My.Resources.Language.SkyEditorSolution)

        'Console Commands
        Manager.RegisterType(GetType(ConsoleCommandAsync), GetType(SkyEditorBase.ConsoleCommands.InstallExtension))
    End Sub
    Public Overrides Sub UnLoad(Manager As PluginManager)

    End Sub

    Public Overrides Sub PrepareForDistribution()

    End Sub
End Class
