Imports SkyEditorBase.Interfaces
Imports SkyEditorWPF.UI
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
        Manager.RegisterTypeRegister(GetType(MenuAction))
        Manager.RegisterDefaultFileTypeDetectors()
        'End CoreMod stuff

        Manager.RegisterIOFilter("*.skysln", PluginHelper.GetLanguageItem("Sky Editor Solution"))
    End Sub

End Class
