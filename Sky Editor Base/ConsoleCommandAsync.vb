Imports System.Threading.Tasks

''' <summary>
''' Represents a console command that returns a Task.
''' </summary>
Public MustInherit Class ConsoleCommandAsync
    Public MustOverride Function MainAsync(Arguments As String()) As Task
    Public Overridable ReadOnly Property CommandName As String
        Get
            Return Me.GetType.Name
        End Get
    End Property

End Class
