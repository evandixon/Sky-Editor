Imports System.Reflection
Imports SkyEditor.Core
Imports SkyEditor.Core.UI
Imports SkyEditor.Core.Windows
Imports SkyEditor.UI.WPF.MenuActions
Imports SkyEditor.UI.WPF.MenuActions.Context
Imports SkyEditor.UI.WPF.ObjectControls

Public MustInherit Class WPFCoreSkyEditorPlugin
    Inherits WindowsCoreSkyEditorPlugin

    Public Overrides Sub Load(manager As PluginManager)
        MyBase.Load(manager)

        manager.RegisterType(GetType(IObjectControl).GetTypeInfo, GetType(GenericIList).GetTypeInfo)
        manager.RegisterType(GetType(IObjectControl).GetTypeInfo, GetType(SolutionExplorer).GetTypeInfo)

        manager.RegisterType(GetType(MenuAction).GetTypeInfo, GetType(DevConsole).GetTypeInfo)
        manager.RegisterType(GetType(MenuAction).GetTypeInfo, GetType(DevPlugins).GetTypeInfo)
        manager.RegisterType(GetType(MenuAction).GetTypeInfo, GetType(FileNewFile).GetTypeInfo)
        manager.RegisterType(GetType(MenuAction).GetTypeInfo, GetType(FileNewSolution).GetTypeInfo)
        manager.RegisterType(GetType(MenuAction).GetTypeInfo, GetType(FileOpenAuto).GetTypeInfo)
        manager.RegisterType(GetType(MenuAction).GetTypeInfo, GetType(FileOpenManual).GetTypeInfo)
        manager.RegisterType(GetType(MenuAction).GetTypeInfo, GetType(FileSave).GetTypeInfo)
        manager.RegisterType(GetType(MenuAction).GetTypeInfo, GetType(FileSaveAll).GetTypeInfo)
        manager.RegisterType(GetType(MenuAction).GetTypeInfo, GetType(FileSaveAs).GetTypeInfo)
        manager.RegisterType(GetType(MenuAction).GetTypeInfo, GetType(FileSaveSolution).GetTypeInfo)
        manager.RegisterType(GetType(MenuAction).GetTypeInfo, GetType(SolutionBuild).GetTypeInfo)
        manager.RegisterType(GetType(MenuAction).GetTypeInfo, GetType(ToolsExtensions).GetTypeInfo)
        manager.RegisterType(GetType(MenuAction).GetTypeInfo, GetType(ToolsSettings).GetTypeInfo)

        manager.RegisterType(GetType(MenuAction).GetTypeInfo, GetType(SolutionProjectAddFolder).GetTypeInfo)
    End Sub

End Class
