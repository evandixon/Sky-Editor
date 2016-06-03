Imports SkyEditor.Core.UI
Imports SkyEditor.UI.WPF.ViewModels

Namespace MenuActions.View
    Public Class MenuViewSolutionBuildProgress
        Inherits MenuAction

        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            CurrentPluginManager.CurrentIOUIManager.ShowAnchorable(New SolutionBuildProgressViewModel)
            Return Task.CompletedTask
        End Function

        Public Sub New()
            MyBase.New({My.Resources.Language.MenuView, My.Resources.Language.MenuViewSolutionBuildProgress})
            AlwaysVisible = True
            SortOrder = 3.2
        End Sub
    End Class
End Namespace

