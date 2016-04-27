Namespace MenuActions
    Public Class FileNewSolution
        Inherits MenuAction

        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            Dim m = PluginManager.GetInstance
            Dim newSol As New UI.NewSolutionWindow(m)
            If newSol.ShowDialog Then
                m.CurrentSolution = SolutionOld.CreateSolution(newSol.SelectedLocation, newSol.SelectedName, newSol.SelectedSolution.GetType)
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