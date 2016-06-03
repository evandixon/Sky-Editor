Imports System.Reflection

Namespace UI
    Public MustInherit Class AnchorableViewModel
        Protected Event CurrentSolutionChanged(sender As Object, e As EventArgs)
        Protected Event CurrentIOUIManagerChanged(sender As Object, e As EventArgs)

        Public Property CurrentIOUIManager As IOUIManager
            Get
                Return _iouiManager
            End Get
            Set(value As IOUIManager)
                _iouiManager = value
                RaiseEvent CurrentIOUIManagerChanged(Me, New EventArgs)
            End Set
        End Property
        Protected WithEvents _iouiManager As IOUIManager

        Public ReadOnly Property ModelID As String
            Get
                Return Me.GetType.GetTypeInfo.AssemblyQualifiedName
            End Get
        End Property

        Public Property Header As String

        Private Sub _iouiManager_SolutionChanged(sender As Object, e As EventArgs) Handles _iouiManager.SolutionChanged
            RaiseEvent CurrentSolutionChanged(sender, e)
        End Sub

    End Class
End Namespace

