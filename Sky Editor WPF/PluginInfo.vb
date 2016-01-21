Imports SkyEditorBase.Interfaces
''' <summary>
''' Most plugins need to call registration methods on load.  Sky Editor Base is no exception.  This class contains methods like the ones found in plugin definitions, without actually being its own plugin.
''' </summary>
Friend Class PluginInfo
    Public Shared Sub Load(Manager As PluginManager)
        Manager.ObjectWindowType = GetType(ObjectWindow)

        'Manager.OpenableFiles.Add(GetType(ExecutableFile))

        'Manager.RegisterFileTypeDetector(AddressOf Manager.DetectFileType)
        'Manager.RegisterFileTypeDetector(AddressOf PluginManager.TryGetObjectFileType)

        Manager.RegisterType(GetType(ConsoleCommandAsync), GetType(ConsoleCommands.DistPrep))
        Manager.RegisterType(GetType(ConsoleCommandAsync), GetType(ConsoleCommands.Zip))
        Manager.RegisterType(GetType(ConsoleCommandAsync), GetType(ConsoleCommands.UpdateAll))
        Manager.RegisterType(GetType(ConsoleCommandAsync), GetType(ConsoleCommands.PackAll))

        Manager.RegisterType(GetType(iObjectControl), GetType(LanguageEditor))
        Manager.RegisterType(GetType(iObjectControl), GetType(SettingsEditor))

        Manager.RegisterMenuActionType(GetType(MenuActions.FileNewFile))
        Manager.RegisterMenuActionType(GetType(MenuActions.FileNewProject))
        Manager.RegisterMenuActionType(GetType(MenuActions.FileOpenAuto))
        Manager.RegisterMenuActionType(GetType(MenuActions.FileOpenManual))
        Manager.RegisterMenuActionType(GetType(MenuActions.FileSave))
        Manager.RegisterMenuActionType(GetType(MenuActions.FileSaveAs))
        Manager.RegisterMenuActionType(GetType(MenuActions.FileSaveProject))
        Manager.RegisterMenuActionType(GetType(MenuActions.FileSaveAll))
        Manager.RegisterMenuActionType(GetType(MenuActions.ToolsSettings))
        Manager.RegisterMenuActionType(GetType(MenuActions.ToolsLanguage))
        Manager.RegisterMenuActionType(GetType(MenuActions.ProjectBuild))
        Manager.RegisterMenuActionType(GetType(MenuActions.ProjectRun))
        Manager.RegisterMenuActionType(GetType(MenuActions.ProjectArchive))
        Manager.RegisterMenuActionType(GetType(MenuActions.DevConsole))
    End Sub
    Public Shared Sub UnLoad(Manager As PluginManager)

    End Sub
End Class
