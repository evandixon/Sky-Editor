﻿Imports SaveEditor.Saves
Imports SkyEditorBase
Imports SkyEditorBase.Interfaces
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
            Me.Header = PluginHelper.GetLanguageItem("General")
            lblGeneral_Adventures.Content = PluginHelper.GetLanguageItem("Adventures")
            lblGeneral_Base.Content = PluginHelper.GetLanguageItem("Base Type")
            lblGeneral_HeldMoney.Content = PluginHelper.GetLanguageItem("Held Money")
            lblGeneral_RescuePoints.Content = PluginHelper.GetLanguageItem("Rescue Points")
            lblGeneral_StoredMoney.Content = PluginHelper.GetLanguageItem("Stored Money")
            lblGeneral_TeamName.Content = PluginHelper.GetLanguageItem("Team Name")

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
            Return {GetType(Saves.RBSave)}
        End Function

        Public Overrides Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer
            Return 0
        End Function

    End Class
End Namespace