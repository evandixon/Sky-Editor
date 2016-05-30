Imports System.Collections.ObjectModel
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI

Namespace ViewModels
    Public Class SolutionExplorerViewModel
        Inherits AnchorableViewModel

        Public ReadOnly Property Solutions As ObservableCollection(Of Solution)
            Get
                Return _solutions
            End Get
        End Property
        Dim _solutions As ObservableCollection(Of Solution)

        Public Sub New()
            Me.Header = My.Resources.Language.SolutionExplorerToolWindowTitle
            _solutions = New ObservableCollection(Of Solution)
        End Sub

        Private Sub SolutionExplorerViewModel_CurrentSolutionChanged(sender As Object, e As EventArgs) Handles Me.CurrentSolutionChanged
            _solutions.Clear()
            _solutions.Add(CurrentIOUIManager.CurrentSolution)
        End Sub
    End Class

End Namespace
