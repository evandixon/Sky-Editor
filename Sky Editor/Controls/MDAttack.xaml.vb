Imports SkyEditorBase

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
                    cbMove.Items.Add(New SkyEditorBase.Utilities.GenericListItem(Of Integer)(item.Value, item.Key))
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
            numGinseng.ToolTip = PluginHelper.GetLanguageItem("Ginseng")
            chbLinked.Content = PluginHelper.GetLanguageItem("Linked")
            chbSet.Content = PluginHelper.GetLanguageItem("Set")
            chbSwitched.ToolTip = PluginHelper.GetLanguageItem("Switched")
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
            Attack = EditingObject
        End Sub

        Public Overrides Sub UpdateObject()
            EditingObject = Attack
        End Sub
        Public Overrides ReadOnly Property SupportedTypes As Type()
            Get
                Return {GetType(Interfaces.iMDAttack)}
            End Get
        End Property
        Public Overrides Function UsagePriority(Type As Type) As Integer
            Return 1
        End Function
        Private Sub OnModified(sender As Object, e As EventArgs) Handles cbMove.SelectionChanged,
                                                                    numGinseng.ValueChanged,
                                                                    chbLinked.Checked,
                                                                    chbLinked.Unchecked,
                                                                    chbSet.Checked,
                                                                    chbSet.Unchecked,
                                                                    chbSwitched.Checked,
                                                                    chbSwitched.Unchecked
            RaiseModified()
        End Sub
    End Class

End Namespace