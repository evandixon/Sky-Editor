Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI

Namespace MenuActions
    Public Class FileNewSolution
        Inherits MenuAction

        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            Dim newSol As New NewSolutionWindow(CurrentPluginManager)
            If newSol.ShowDialog Then
                CurrentPluginManager.CurrentIOUIManager.CurrentSolution = Solution.CreateSolution(newSol.SelectedLocation, newSol.SelectedName, newSol.SelectedSolution.GetType, CurrentPluginManager)
            End If
            Return Task.CompletedTask
        End Function

        Public Sub New()
            MyBase.New({My.Resources.Language.MenuFile, My.Resources.Language.MenuFileNew, My.Resources.Language.MenuFileNewSolution})
            Me.AlwaysVisible = True
            SortOrder = 1.12
        End Sub
    End Class
End Namespace