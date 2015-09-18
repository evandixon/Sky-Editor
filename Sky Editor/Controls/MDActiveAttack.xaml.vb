Imports SkyEditorBase

Namespace Controls
    Public Class MDActiveAttack
        Inherits ObjectControl
        Dim _attack As Interfaces.iMDActiveAttack
        Public Property Attack As Interfaces.iMDActiveAttack
            Get
                With _attack
                    .ID = SelectedMoveID
                    .Ginseng = numGinseng.Value
                    .IsSwitched = chbSwitched.IsChecked
                    .IsLinked = chbLinked.IsChecked
                    .IsSet = chbSet.IsChecked
                    .IsSealed = chbSealed.IsChecked
                    .PP = numPP.Value
                End With
                Return _attack
            End Get
            Set(value As Interfaces.iMDActiveAttack)
                For Each item In (From m In value.GetAttackDictionary Select m Order By m.Value)
                    cbMove.Items.Add(New SkyEditorBase.Utilities.GenericListItem(Of Integer)(item.Value, item.Key))
                Next
                SelectedMoveID = value.ID
                numGinseng.Value = value.Ginseng
                chbSwitched.IsChecked = value.IsSwitched
                chbLinked.IsChecked = value.IsLinked
                chbSet.IsChecked = value.IsSet
                chbSealed.IsChecked = value.IsSet
                numPP.Value = value.PP
                _attack = value
            End Set
        End Property

        Private Sub SkyAttack_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded

        End Sub
        Private Property SelectedMoveID As Integer
            Get
                Return DirectCast(cbMove.LastSafeValue, Utilities.GenericListItem(Of Integer)).Value
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
            Attack = ContainedObject
        End Sub

        Public Overrides Sub UpdateObject()
            ContainedObject = Attack
        End Sub
        Public Overrides ReadOnly Property SupportedTypes As Type()
            Get
                Return {GetType(Interfaces.iMDActiveAttack)}
            End Get
        End Property
        Public Overrides Function UsagePriority(Type As Type) As Integer
            Return 2
        End Function
    End Class

End Namespace