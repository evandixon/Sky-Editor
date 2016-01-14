Namespace ConsoleCommands
    Public Class UpdateAll
        Inherits ConsoleCommand

        Public Overrides Sub Main(Arguments() As String)
            Redistribution.RedistributionHelpers.DownloadAllPlugins(PluginManager.GetInstance, Arguments(0))
        End Sub
    End Class
End Namespace

