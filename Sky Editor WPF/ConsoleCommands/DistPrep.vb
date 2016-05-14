Imports SkyEditor.Core.ConsoleCommands

Namespace ConsoleCommands
    Public Class DistPrep
        Inherits ConsoleCommand

        Public Overrides Sub Main(Arguments() As String)
            Redistribution.RedistributionHelpers.PrepareForDistribution(PluginManager.GetInstance)
        End Sub
    End Class
End Namespace

