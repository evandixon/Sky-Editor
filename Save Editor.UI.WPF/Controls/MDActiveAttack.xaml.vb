Imports System.Windows
Imports System.Windows.Controls
Imports SkyEditor.SaveEditor
Imports SkyEditor.UI.WPF
Imports SkyEditorBase
Imports SkyEditorWPF.UI

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
                    cbMove.Items.Add(New GenericListItem(Of Integer)(item.Value, item.Key))
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
            numGinseng.ToolTip = My.Resources.Language.Ginseng
            lblGinseng.Content = My.Resources.Language.Ginseng

            numPP.ToolTip = My.Resources.Language.PP
            lblPP.Content = My.Resources.Language.PP

            chbLinked.Content = My.Resources.Language.Linked
            chbSet.Content = My.Resources.Language.IsSet
            chbSealed.Content = My.Resources.Language.Sealed
            chbSwitched.ToolTip = My.Resources.Language.Switched
        End Sub
        Private Property SelectedMoveID As Integer
            Get
                If cbMove.LastSafeValue Is Nothing Then
                    Return Nothing
                Else
                    Return DirectCast(cbMove.LastSafeValue, GenericListItem(Of Integer)).Value
                End If
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
            Attack = GetEditingObject(Of iMDActiveAttack)()
            IsModified = False
        End Sub

        Public Overrides Sub UpdateObject()
            Me.SetEditingObject(Attack)
        End Sub

        Private Sub OnModified(sender As Object, e As EventArgs) Handles cbMove.SelectionChanged,
                                                                            numPP.ValueChanged,
                                                                            numGinseng.ValueChanged,
                                                                            chbLinked.Checked,
                                                                            chbLinked.Unchecked,
                                                                            chbSet.Checked,
                                                                            chbSet.Unchecked,
                                                                            chbSealed.Checked,
                                                                            chbSealed.Unchecked,
                                                                            chbSwitched.Checked,
                                                                            chbSwitched.Unchecked
            IsModified = True
        End Sub

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of Type)
            Return {GetType(Interfaces.iMDActiveAttack)}
        End Function

        Public Overrides Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer
            Return 2
        End Function

    End Class

End Namespace