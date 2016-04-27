Imports SkyEditorBase.Interfaces
Namespace UI
    Public Class SolutionBuildProgress
        Implements Interfaces.ITargetedControl

        Private Property CurrentSolution As SolutionOld

        Public Property Header As String Implements ITargetedControl.Header
            Get
                Return _header
            End Get
            Set(value As String)
                Dim old = _header
                _header = value
                RaiseEvent HeaderChanged(Me, New EventArguments.HeaderUpdatedEventArgs With {.NewValue = value, .OldValue = old})
            End Set
        End Property
        Dim _header As String

        Protected Property ITargetedControl_IsVisible As Boolean Implements ITargetedControl.IsVisible
            Get
                Return _isVisible
            End Get
            Set(value As Boolean)
                _isVisible = value
                RaiseEvent VisibilityChanged(Me, New EventArguments.VisibilityUpdatedEventArgs With {.IsVisible = value})
            End Set
        End Property
        Dim _isVisible As Boolean

        Public Event HeaderChanged As ITargetedControl.HeaderChangedEventHandler Implements ITargetedControl.HeaderChanged
        Public Event VisibilityChanged As ITargetedControl.VisibilityChangedEventHandler Implements ITargetedControl.VisibilityChanged

        Public Sub UpdateTargets(Targets As IEnumerable(Of Object)) Implements ITargetedControl.UpdateTargets
            Dim supported As Integer = 0

            If CurrentSolution IsNot Nothing Then
                RemoveHandler CurrentSolution.SolutionBuildStarted, AddressOf Solution_BuildStarted
                RemoveHandler CurrentSolution.SolutionBuildCompleted, AddressOf Solution_BuildCompleted
            End If

            dataGrid.Items.Clear()

            For Each item In Targets
                If TypeOf item Is SolutionOld Then
                    CurrentSolution = item
                    AddHandler CurrentSolution.SolutionBuildStarted, AddressOf Solution_BuildStarted
                    AddHandler CurrentSolution.SolutionBuildCompleted, AddressOf Solution_BuildCompleted
                    supported += 1
                End If
            Next

            Me.ITargetedControl_IsVisible = (supported > 0)
        End Sub

        Public Function GetDefaultPane() As ITargetedControl.Pane Implements ITargetedControl.GetDefaultPane
            Return ITargetedControl.Pane.Bottom
        End Function

        Public Function StartCollapsed() As Boolean Implements ITargetedControl.StartCollapsed
            Return True
        End Function
        Private Sub SolutionExplorer_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Header = My.Resources.Language.BuildStatus
        End Sub
        Private Sub Solution_BuildStarted(sender As Object, e As EventArgs)
            dataGrid.Items.Clear()
            For Each item In DirectCast(sender, SolutionOld).GetAllProjects
                AddHandler item.BuildStatusChanged, AddressOf Project_BuildStatusChanged
            Next
        End Sub

        Private Sub Solution_BuildCompleted(sender As Object, e As EventArgs)
            For Each item In DirectCast(sender, SolutionOld).GetAllProjects
                RemoveHandler item.BuildStatusChanged, AddressOf Project_BuildStatusChanged
            Next
        End Sub

        Private Sub Project_BuildStatusChanged(sender As Object, e As EventArguments.ProjectBuildStatusChanged)
            Dispatcher.Invoke(New Action(Sub()
                                             If Not dataGrid.Items.Contains(sender) Then
                                                 dataGrid.Items.Add(sender)
                                             End If
                                         End Sub))
        End Sub
    End Class

End Namespace
