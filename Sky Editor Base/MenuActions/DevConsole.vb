Imports System.Threading.Tasks

Namespace MenuActions
    Public Class DevConsole
        Inherits MenuAction

        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            Internal.ConsoleManager.Show()
            ConsoleMain(PluginManager.GetInstance)
            Return Task.CompletedTask
        End Function

        Public Sub New()
            MyBase.New("Console", "/", True)
            AlwaysVisible = True
        End Sub
    End Class
End Namespace

