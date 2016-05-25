Imports System.Windows
Imports SkyEditor.SaveEditor
Imports SkyEditor.UI.WPF
Imports SkyEditorBase
Imports SkyEditorWPF.UI

Namespace Controls
    Public Class MDAttack
        Inherits ObjectControl
        Dim _attack As Interfaces.iMDAttack
        Public Property Attack As Interfaces.iMDAttack
            Get
                With _attack
                    .ID = SelectedMoveID
                    .Ginseng = numGinseng.Value
                    .IsSwitched = chbSwitched.IsChecked
                    .IsLinked = chbLinked.IsChecked
                    .IsSet = chbSet.IsChecked
                End With
                Return _attack
            End Get
            Set(value As Interfaces.iMDAttack)
                For Each item In (From m In value.GetAttackDictionary Select m Order By m.Value)
                    cbMove.Items.Add(New GenericListItem(Of Integer)(item.Value, item.Key))
                Next
                SelectedMoveID = value.ID
                numGinseng.Value = value.Ginseng
                chbSwitched.IsChecked = value.IsSwitched
                chbLinked.IsChecked = value.IsLinked
                chbSet.IsChecked = value.IsSet
                _attack = value
            End Set
        End Property

        Private Sub SkyAttack_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            numGinseng.ToolTip = My.Resources.Language.Ginseng
            chbLinked.Content = My.Resources.Language.Linked
            chbSet.Content = My.Resources.Language.IsSet
            chbSwitched.ToolTip = My.Resources.Language.Switched
        End Sub
        Private Property SelectedMoveID As Integer
            Get
                Return DirectCast(cbMove.LastSafeValue, GenericListItem(Of Integer)).Value
            End Get
            Set(value As Integer)
                For Each item In cbMove.Items
                    If DirectCast(item, GenericListItem(Of Integer)).Value = value Then
                        cbMove.SelectedItem = item
                    End If
                Next
            End Set
        End Property

        Public Overrides Sub RefreshDisplay()
            Attack = GetEditingObject(Of iMDAttack)()
            IsModified = False
        End Sub

        Public Overrides Sub UpdateObject()
            Me.SetEditingObject(Attack)
        End Sub

        Private Sub OnModified(sender As Object, e As EventArgs) Handles cbMove.SelectionChanged,
                                                                    numGinseng.ValueChanged,
                                                                    chbLinked.Checked,
                                                                    chbLinked.Unchecked,
                                                                    chbSet.Checked,
                                                                    chbSet.Unchecked,
                                                                    chbSwitched.Checked,
                                                                    chbSwitched.Unchecked
            IsModified = True
        End Sub

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of Type)
            Return {GetType(Interfaces.iMDAttack)}
        End Function

        Public Overrides Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer
            Return 1
        End Function

    End Class

End Namespace