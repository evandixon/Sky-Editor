Imports System.Threading.Tasks

''' <summary>
''' Represents a console command that returns a Task.
''' </summary>
Public MustInherit Class ConsoleCommandAsync
    Public MustOverride Function MainAsync(Arguments As String()) As Task

End Class
