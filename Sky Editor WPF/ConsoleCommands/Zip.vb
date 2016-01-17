Namespace ConsoleCommands
    Public Class Zip
        Inherits ConsoleCommand

        Public Overrides Sub Main(Arguments() As String)
            Redistribution.RedistributionHelpers.PackProgram(PluginManager.GetInstance, Arguments(0))
        End Sub
    End Class

End Namespace
