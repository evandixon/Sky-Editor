Imports System.Threading.Tasks
Imports SkyEditor.Core.UI

Namespace MenuActions
    Public Class DevConsole
        Inherits MenuAction

        Public Overrides Async Function DoAction(Targets As IEnumerable(Of Object)) As Task
            PluginHelper.ShowConsole()
            Await ConsoleMain(PluginManager.GetInstance)
        End Function

        Public Sub New()
            MyBase.New({My.Resources.Language.MenuDev, My.Resources.Language.MenuDevConsole})
            AlwaysVisible = True
            SortOrder = 10.1
            DevOnly = True
        End Sub
    End Class
End Namespace

