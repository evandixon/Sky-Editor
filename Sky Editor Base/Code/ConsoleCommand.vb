Imports System.Threading.Tasks

''' <summary>
''' Represents a console command subroutine.
''' </summary>
Public MustInherit Class ConsoleCommand
    Inherits ConsoleCommandAsync

    Public Overrides Async Function MainAsync(Arguments() As String) As Task
        Await Task.Run(New Action(Sub()
                                      Main(Arguments)
                                  End Sub))
    End Function

    Public MustOverride Sub Main(Arguments As String())
End Class
