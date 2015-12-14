Imports SkyEditorBase
Imports SkyEditorBase.Interfaces
Imports SkyEditorBase.Redistribution

Namespace Internal
    ''' <summary>
    ''' Most plugins need to call registration methods on load.  Sky Editor Base is no exception.  This class contains methods like the ones found in plugin definitions, without actually being its own plugin.
    ''' </summary>
    Friend Class PluginInfo
        Public Shared Sub Load(Manager As PluginManager)
            Manager.OpenableFiles.Add(GetType(ExecutableFile))

            Manager.RegisterFileTypeDetector(AddressOf Manager.DetectFileType)
            Manager.RegisterFileTypeDetector(AddressOf PluginManager.TryGetObjectFileType)

            Manager.RegisterConsoleCommand("distprep", AddressOf RedistributionHelpers.PrepareForDistribution)
            Manager.RegisterConsoleCommand("zip", AddressOf RedistributionHelpers.PackProgram)
            Manager.RegisterConsoleCommand("packplug", AddressOf RedistributionHelpers.PackPlugins)
            Manager.RegisterConsoleCommand("delplug", AddressOf RedistributionHelpers.DeletePlugin)
            Manager.RegisterConsoleCommand("generateinfo", AddressOf RedistributionHelpers.GeneratePluginDownloadDir)
            Manager.RegisterConsoleCommand("updateall", AddressOf RedistributionHelpers.DownloadAllPlugins)
            Manager.RegisterConsoleCommand("packall", AddressOf RedistributionHelpers.PackageAll)

            Manager.RegisterObjectControl(GetType(Language.LanguageEditor))
            Manager.RegisterObjectControl(GetType(SettingsEditor))

            Manager.RegisterMenuAction(New MenuActions.FileNewFile)
            Manager.RegisterMenuAction(New MenuActions.FileNewProject)
            Manager.RegisterMenuAction(New MenuActions.FileOpenAuto)
            Manager.RegisterMenuAction(New MenuActions.FileOpenManual)
            Manager.RegisterMenuAction(New MenuActions.FileSave)
            Manager.RegisterMenuAction(New MenuActions.FileSaveAs)
            Manager.RegisterMenuAction(New MenuActions.FileSaveProject)
            Manager.RegisterMenuAction(New MenuActions.FileSaveAll)
            Manager.RegisterMenuAction(New MenuActions.ToolsSettings)
            Manager.RegisterMenuAction(New MenuActions.ToolsLanguage)
            Manager.RegisterMenuAction(New MenuActions.ProjectBuild)
            Manager.RegisterMenuAction(New MenuActions.ProjectRun)
            Manager.RegisterMenuAction(New MenuActions.ProjectArchive)
            Manager.RegisterMenuAction(New MenuActions.DevConsole)
        End Sub
        Public Shared Sub UnLoad(Manager As PluginManager)

        End Sub
    End Class

End Namespace
