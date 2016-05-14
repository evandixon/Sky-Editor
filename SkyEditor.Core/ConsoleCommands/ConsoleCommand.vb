Imports System.Threading.Tasks

Namespace ConsoleCommands
    ''' <summary>
    ''' Represents a console command subroutine.
    ''' </summary>
    Public MustInherit Class ConsoleCommand
        Inherits ConsoleCommandAsync

        Public Overrides Function MainAsync(Arguments() As String) As Task
            Main(Arguments)
            Return Task.FromResult(0)
        End Function

        Public MustOverride Sub Main(Arguments As String())
    End Class
End Namespace

