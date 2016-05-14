Imports SkyEditorBase
Imports SkyEditorBase.Interfaces
Imports SkyEditorWPF.UI

Namespace Controls
    Public Class iAttack
        Inherits ObjectControl
        Dim _attack As SkyEditor.SaveEditor.Interfaces.iAttack
        Public Property Attack As SkyEditor.SaveEditor.Interfaces.iAttack
            Get
                _attack.ID = SelectedMoveID
                Return _attack
            End Get
            Set(value As SkyEditor.SaveEditor.Interfaces.iAttack)
                For Each item In (From m In value.GetAttackDictionary Select m Order By m.Value)
                    cbMove.Items.Add(New SkyEditorBase.Utilities.GenericListItem(Of Integer)(item.Value, item.Key))
                Next
                SelectedMoveID = value.ID
                _attack = value
            End Set
        End Property

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
            Attack = GetEditingObject(Of iAttack)()
        End Sub

        Public Overrides Sub UpdateObject()
            Me.SetEditingObject(Attack)
        End Sub

        Private Sub OnModified(sender As Object, e As EventArgs) Handles cbMove.SelectionChanged
            IsModified = True
        End Sub

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of Type)
            Return {GetType(SkyEditor.SaveEditor.Interfaces.iAttack)}
        End Function

        Public Overrides Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer
            Return 0
        End Function

    End Class
End Namespace