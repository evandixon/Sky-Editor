Imports SkyEditorBase.Interfaces
''' <summary>
''' Most plugins need to call registration methods on load.  Sky Editor Base is no exception.  This class contains methods like the ones found in plugin definitions, without actually being its own plugin.
''' </summary>
Friend Class WpfCoreMod
    Inherits WpfCoreModBase

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
            Return PluginHelper.GetLanguageItem("Sky Editor WPF")
        End Get
    End Property

    Public Overrides Sub Load(Manager As PluginManager)
        MyBase.Load(Manager)
        'CoreMod stuff
        Manager.RegisterTypeRegister(GetType(iObjectControl))
        Manager.RegisterTypeRegister(GetType(Solution))
        Manager.RegisterTypeRegister(GetType(Project))
        Manager.RegisterTypeRegister(GetType(iCreatableFile))
        Manager.RegisterTypeRegister(GetType(iOpenableFile))
        Manager.RegisterTypeRegister(GetType(iDetectableFileType))
        Manager.RegisterTypeRegister(GetType(ConsoleCommandAsync))
        Manager.RegisterTypeRegister(GetType(ITargetedControl))

        Manager.RegisterType(GetType(Solution), GetType(Solution))
        Manager.RegisterType(GetType(Project), GetType(Project))

        Manager.RegisterDefaultFileTypeDetectors()
        'End CoreMod stuff

        'Menu Items
        Manager.RegisterMenuActionType(GetType(MenuActions.FileNewFile))
        Manager.RegisterMenuActionType(GetType(MenuActions.FileNewSolution))
        Manager.RegisterMenuActionType(GetType(MenuActions.FileOpenAuto))
        Manager.RegisterMenuActionType(GetType(MenuActions.FileOpenManual))
        Manager.RegisterMenuActionType(GetType(MenuActions.FileSave))
        Manager.RegisterMenuActionType(GetType(MenuActions.FileSaveAs))
        Manager.RegisterMenuActionType(GetType(MenuActions.FileSaveSolution))
        Manager.RegisterMenuActionType(GetType(MenuActions.FileSaveAll))
        Manager.RegisterMenuActionType(GetType(MenuActions.ToolsSettings))
        Manager.RegisterMenuActionType(GetType(MenuActions.ToolsLanguage))
        Manager.RegisterMenuActionType(GetType(MenuActions.SolutionBuild))
        Manager.RegisterMenuActionType(GetType(MenuActions.DevConsole))

        'Manager.OpenableFiles.Add(GetType(ExecutableFile))

        Manager.RegisterIOFilter("*.skysln", PluginHelper.GetLanguageItem("Sky Editor Solution"))

        'Console Commands
        Manager.RegisterType(GetType(ConsoleCommandAsync), GetType(ConsoleCommands.DistPrep))
        Manager.RegisterType(GetType(ConsoleCommandAsync), GetType(ConsoleCommands.Zip))
        Manager.RegisterType(GetType(ConsoleCommandAsync), GetType(ConsoleCommands.UpdateAll))
        Manager.RegisterType(GetType(ConsoleCommandAsync), GetType(ConsoleCommands.PackAll))

        'Register provided types
        Manager.RegisterType(GetType(ITargetedControl), GetType(SolutionExplorer))
        Manager.RegisterType(GetType(iObjectControl), GetType(LanguageEditor))
        Manager.RegisterType(GetType(iObjectControl), GetType(SettingsEditor))
    End Sub

End Class
