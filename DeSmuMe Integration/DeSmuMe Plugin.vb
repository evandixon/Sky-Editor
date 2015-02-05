Imports SkyEditorBase
Public Class DeSmuMePlugin
    Implements iSkyEditorPlugin
    Public ReadOnly Property Credits As String Implements iSkyEditorPlugin.Credits
        Get
            Return PluginHelper.GetLanguageItem("DeSmuMeIntegrationCredits", "DeSmuMe Integration Credits:\n     Cracker (DS Icon Tool)\n     The DeSmuMe Developers\n     evandixon (Integration)")
        End Get
    End Property

    Public Sub Load(ByRef Manager As PluginManager) Implements iSkyEditorPlugin.Load
        Manager.RegisterMenuItem(New DesmumeMenuItem(Manager))
    End Sub

    Public ReadOnly Property PluginAuthor As String Implements iSkyEditorPlugin.PluginAuthor
        Get
            Return "evandixon"
        End Get
    End Property

    Public ReadOnly Property PluginName As String Implements iSkyEditorPlugin.PluginName
        Get
            Return "DeSmuMe Integration"
        End Get
    End Property

    Public Sub UnLoad(ByRef Manager As PluginManager) Implements iSkyEditorPlugin.UnLoad

    End Sub

    Public Sub PrepareForDistribution() Implements iSkyEditorPlugin.PrepareForDistribution
        Try
            Dim folder = IO.Path.Combine(Environment.CurrentDirectory, "Resources/Plugins/DeSmuMe/")
            For Each d In IO.Directory.GetDirectories(folder)
                DeveloperConsole.Writeline("Deleting directory " & d)
                IO.Directory.Delete(d, True)
            Next
        Catch ex As Exception
            DeveloperConsole.Writeline(ex.ToString)
        End Try
    End Sub
End Class
