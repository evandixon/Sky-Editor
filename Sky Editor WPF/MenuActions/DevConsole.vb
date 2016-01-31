Imports System.Threading.Tasks

Namespace MenuActions
    Public Class DevConsole
        Inherits MenuAction

        Public Overrides Async Function DoAction(Targets As IEnumerable(Of Object)) As Task
            PluginHelper.ShowConsole()
            Await ConsoleMain(PluginManager.GetInstance)
        End Function

        Public Sub New()
            MyBase.New("Console", "/", True)
            AlwaysVisible = True
        End Sub
    End Class
End Namespace

