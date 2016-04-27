Imports SkyEditor.Core.Interfaces
Imports SkyEditorBase
Imports SkyEditorBase.Interfaces
''' <summary>
''' Most plugins need to call registration methods on load.  Sky Editor Base is no exception.  This class contains methods like the ones found in plugin definitions, without actually being its own plugin.
''' </summary>
Friend Class ConsoleCoreMod
    Implements iSkyEditorPlugin

    Public ReadOnly Property Credits As String Implements iSkyEditorPlugin.Credits
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property PluginAuthor As String Implements iSkyEditorPlugin.PluginAuthor
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property PluginName As String Implements iSkyEditorPlugin.PluginName
        Get
            Return My.Resources.Language.ConsolePluginName
        End Get
    End Property

    Public Sub Load(Manager As PluginManager) Implements iSkyEditorPlugin.Load
        'CoreMod stuff
        Manager.RegisterTypeRegister(GetType(SolutionOld))
        Manager.RegisterTypeRegister(GetType(ProjectOld))
        Manager.RegisterTypeRegister(GetType(iCreatableFile))
        Manager.RegisterTypeRegister(GetType(iOpenableFile))
        Manager.RegisterTypeRegister(GetType(iDetectableFileType))
        Manager.RegisterTypeRegister(GetType(ConsoleCommandAsync))

        Manager.RegisterType(GetType(SolutionOld), GetType(SolutionOld))
        Manager.RegisterType(GetType(ProjectOld), GetType(ProjectOld))

        Manager.RegisterType(GetType(ConsoleCommandAsync), GetType(SkyEditorBase.ConsoleCommands.InstallExtension))
        Manager.RegisterType(GetType(ConsoleCommandAsync), GetType(SkyEditorBase.ConsoleCommands.GeneratePluginExtensions))

        Manager.RegisterDefaultFileTypeDetectors()
    End Sub
    Public Sub UnLoad(Manager As PluginManager) Implements iSkyEditorPlugin.UnLoad

    End Sub

    Public Sub PrepareForDistribution() Implements iSkyEditorPlugin.PrepareForDistribution

    End Sub
End Class
