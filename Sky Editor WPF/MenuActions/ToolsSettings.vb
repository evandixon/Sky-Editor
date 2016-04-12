Imports System.Threading.Tasks

Namespace MenuActions
    Public Class ToolsSettings
        Inherits MenuAction

        Public Overrides Function SupportsObject(Obj As Object) As Boolean
            Return True
        End Function

        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            PluginHelper.RequestFileOpen(SettingsManager.Instance, False)
            Return Task.CompletedTask
        End Function

        Public Sub New()
            MyBase.New({My.Resources.Language.MenuTools, My.Resources.Language.menutoolsSettings})
            AlwaysVisible = True
            SortOrder = 3.1
        End Sub
    End Class
End Namespace

