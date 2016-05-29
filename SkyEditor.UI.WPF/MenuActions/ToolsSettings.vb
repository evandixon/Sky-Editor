Imports System.Threading.Tasks
Imports SkyEditor.Core.UI

Namespace MenuActions
    Public Class ToolsSettings
        Inherits MenuAction

        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            CurrentPluginManager.CurrentIOUIManager.OpenFile(CurrentPluginManager.CurrentSettingsProvider, False)
            Return Task.CompletedTask
        End Function

        Public Sub New()
            MyBase.New({My.Resources.Language.MenuTools, My.Resources.Language.MenuToolsSettings})
            AlwaysVisible = True
            SortOrder = 3.1
        End Sub
    End Class
End Namespace

