Imports SkyEditor.SaveEditor.MysteryDungeon.Rescue
Imports SkyEditor.SaveEditor.Saves
Imports SkyEditorBase
Imports SkyEditorWPF.UI

Namespace Tabs
    Public Class RBGeneral
        Inherits ObjectControl
        Public Overrides Sub RefreshDisplay()
            With GetEditingObject(Of RBSave)()
                txtGeneral_TeamName.Text = .TeamName
                numGeneral_HeldMoney.Value = .HeldMoney
                numGeneral_StoredMoney.Value = .StoredMoney
                numGeneral_RescuePoints.Value = .RescuePoints
                'cbGeneral_Base.SelectedItem = cbGeneral_Base.Items.IndexOf(Lists.RBBaseTypesInverse(.BaseType))
            End With
            IsModified = False
        End Sub

        Public Overrides Sub UpdateObject()
            With GetEditingObject(Of RBSave)()
                .TeamName = txtGeneral_TeamName.Text
                .HeldMoney = numGeneral_HeldMoney.Value
                .StoredMoney = numGeneral_StoredMoney.Value
                .RescuePoints = numGeneral_RescuePoints.Value
                '.BaseType = Lists.RBBaseTypes(cbGeneral_Base.SelectedItem)
            End With
        End Sub

        Private Sub RBGeneral_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Header = My.Resources.Language.General

            'For Each item In Lists.RBBaseTypes
            '    cbGeneral_Base.Items.Add(item)
            'Next
            'cbGeneral_Base.DisplayMemberPath = "Key"
        End Sub
        Private Sub OnModified(sender As Object, e As EventArgs) Handles txtGeneral_TeamName.TextChanged,
                                                                        numGeneral_HeldMoney.ValueChanged,
                                                                        numGeneral_StoredMoney.ValueChanged,
                                                                        numGeneral_RescuePoints.ValueChanged,
                                                                        cbGeneral_Base.SelectionChanged
            IsModified = True
        End Sub

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of Type)
            Return {GetType(RBSave)}
        End Function

        Public Overrides Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer
            Return 0
        End Function

    End Class
End Namespace