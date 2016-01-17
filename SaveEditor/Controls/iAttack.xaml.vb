Imports SkyEditorBase

Namespace Controls
    Public Class iAttack
        Inherits ObjectControl
        Dim _attack As Interfaces.iAttack
        Public Property Attack As Interfaces.iAttack
            Get
                _attack.ID = SelectedMoveID
                Return _attack
            End Get
            Set(value As Interfaces.iAttack)
                For Each item In (From m In value.GetAttackDictionary Select m Order By m.Value)
                    cbMove.Items.Add(New SkyEditorBase.Utilities.GenericListItem(Of Integer)(item.Value, item.Key))
                Next
                SelectedMoveID = value.ID
                _attack = value
            End Set
        End Property

        Private Sub SkyAttack_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded

        End Sub
        Private Property SelectedMoveID As Integer
            Get
                Return DirectCast(cbMove.SelectedItem, Utilities.GenericListItem(Of Integer)).Value
            End Get
            Set(value As Integer)
                For Each item In cbMove.Items
                    If DirectCast(item, Utilities.GenericListItem(Of Integer)).Value = value Then
                        cbMove.SelectedItem = item
                    End If
                Next
            End Set
        End Property

        Public Overrides Sub RefreshDisplay()
            Attack = EditingObject
        End Sub

        Public Overrides Sub UpdateObject()
            EditingObject = Attack
        End Sub
        Public Overrides ReadOnly Property SupportedTypes As Type()
            Get
                Return {GetType(Interfaces.iAttack)}
            End Get
        End Property

        Private Sub OnModified(sender As Object, e As EventArgs) Handles cbMove.SelectionChanged
            RaiseModified()
        End Sub
    End Class
End Namespace