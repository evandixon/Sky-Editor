Imports System.Collections.ObjectModel
Imports System.Windows
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI

Namespace ViewModels
    Public Class SolutionBuildProgressViewModel
        Inherits AnchorableViewModel

        Public Sub New()
            Me.Header = My.Resources.Language.BuildProgress
            BuildingProjects = New ObservableCollection(Of Project)
        End Sub

        Public Property BuildingProjects As ObservableCollection(Of Project)

        Private WithEvents CurrentSolution As Solution

        Private Sub SolutionBuildProgressViewModel_CurrentSolutionChanged(sender As Object, e As EventArgs) Handles Me.CurrentSolutionChanged, Me.CurrentIOUIManagerChanged
            CurrentSolution = CurrentIOUIManager.CurrentSolution
        End Sub

        Private Sub Solution_BuildStarted(sender As Object, e As EventArgs) Handles CurrentSolution.SolutionBuildStarted
            Application.Current.Dispatcher.Invoke(Sub()
                                                      BuildingProjects.Clear()
                                                  End Sub)

            For Each item In DirectCast(sender, Solution).GetAllProjects
                AddHandler item.BuildStatusChanged, AddressOf Project_BuildStatusChanged
            Next
        End Sub

        Private Sub Solution_BuildCompleted(sender As Object, e As EventArgs) Handles CurrentSolution.SolutionBuildCompleted
            For Each item In DirectCast(sender, Solution).GetAllProjects
                RemoveHandler item.BuildStatusChanged, AddressOf Project_BuildStatusChanged
            Next
        End Sub

        Private Sub Project_BuildStatusChanged(sender As Object, e As ProjectBuildStatusChanged)
            If Not BuildingProjects.Contains(sender) Then
                Application.Current.Dispatcher.Invoke(Sub()
                                                          BuildingProjects.Add(sender)
                                                      End Sub)
            End If
        End Sub
    End Class
End Namespace

