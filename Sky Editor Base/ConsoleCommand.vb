Imports System.Threading.Tasks

''' <summary>
''' Represents a console command subroutine.
''' </summary>
Public MustInherit Class ConsoleCommand
    Inherits ConsoleCommandAsync

    Public Overrides Function MainAsync(Arguments() As String) As Task
        Main(Arguments)
        Return Task.CompletedTask
    End Function

    Public MustOverride Sub Main(Arguments As String())
End Class
