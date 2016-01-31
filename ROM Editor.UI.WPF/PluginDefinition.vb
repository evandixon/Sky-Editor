Imports SkyEditorBase
Imports ROMEditor.Roms
Imports SkyEditorBase.Interfaces

Public Class PluginDefinition
    Implements iSkyEditorPlugin

    Public ReadOnly Property Credits As String Implements iSkyEditorPlugin.Credits
        Get
            Return PluginHelper.GetLanguageItem("RomEditorCredits", "Rom Editor Credits:\n     psy_commando (Pokemon portraits, most of the research)\n     Grovyle91 (Language strings)\n     evandixon (Personality test, bgp files)")
        End Get
    End Property

    Public ReadOnly Property PluginAuthor As String Implements iSkyEditorPlugin.PluginAuthor
        Get
            Return "evandixon"
        End Get
    End Property

    Public ReadOnly Property PluginName As String Implements iSkyEditorPlugin.PluginName
        Get
            Return "Generic ROM Editor"
        End Get
    End Property

    Public Sub Load(Manager As PluginManager) Implements iSkyEditorPlugin.Load
        Manager.RegisterResourceFile(IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins", "Xceed.Wpf.AvalonDock.dll"))
        Manager.RegisterResourceFile(IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins", "Xceed.Wpf.AvalonDock.Aero.dll"))
        Manager.RegisterResourceFile(IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins", "Xceed.Wpf.AvalonDock.Metro.dll"))
        Manager.RegisterResourceFile(IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins", "Xceed.Wpf.AvalonDock.VS2010.dll"))
        Manager.RegisterResourceFile(IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins", "Xceed.Wpf.DataGrid.dll"))
        Manager.RegisterResourceFile(IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins", "Xceed.Wpf.Toolkit.dll"))
    End Sub

    Public Sub UnLoad(Manager As PluginManager) Implements iSkyEditorPlugin.UnLoad

    End Sub

    Public Sub PrepareForDistribution() Implements iSkyEditorPlugin.PrepareForDistribution

    End Sub
End Class