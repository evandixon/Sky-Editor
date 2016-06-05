Imports SkyEditor.Core.ConsoleCommands
Imports SkyEditor.Core.Windows.Redistribution

Namespace ConsoleCommands
    Public Class DistPrep
        Inherits ConsoleCommand

        Public Overrides Sub Main(Arguments() As String)
            RedistributionHelpers.PrepareForDistribution(CurrentPluginManager)
        End Sub
    End Class
End Namespace

