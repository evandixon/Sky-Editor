Namespace ConsoleCommands
    Public Class PackAll
        Inherits ConsoleCommandAsync

        Public Overrides Async Function MainAsync(Arguments() As String) As Task
            Await Redistribution.RedistributionHelpers.PackageAll(PluginManager.GetInstance)
        End Function
    End Class
End Namespace

