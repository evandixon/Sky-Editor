Namespace ConsoleCommands
    Public Class PackAll
        Inherits ConsoleCommand

        Public Overrides Sub Main(Arguments() As String)
            Redistribution.RedistributionHelpers.PackageAll(PluginManager.GetInstance)
        End Sub
    End Class
End Namespace

