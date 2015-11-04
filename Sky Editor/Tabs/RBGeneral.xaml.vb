Imports SkyEditor.Saves
Imports SkyEditorBase

Namespace Tabs
    Public Class RBGeneral
        Inherits ObjectTab(Of RBSave)
        Public Overrides Sub RefreshDisplay()
            With EditingItem
                txtGeneral_TeamName.Text = .TeamName
                numGeneral_HeldMoney.Value = .HeldMoney
                numGeneral_StoredMoney.Value = .StoredMoney
                numGeneral_RescuePoints.Value = .RescuePoints
                'cbGeneral_Base.SelectedItem = cbGeneral_Base.Items.IndexOf(Lists.RBBaseTypesInverse(.BaseType))
            End With
        End Sub

        Public Overrides Sub UpdateObject()
            With EditingItem
                .TeamName = txtGeneral_TeamName.Text
                .HeldMoney = numGeneral_HeldMoney.Value
                .StoredMoney = numGeneral_StoredMoney.Value
                .RescuePoints = numGeneral_RescuePoints.Value
                '.BaseType = Lists.RBBaseTypes(cbGeneral_Base.SelectedItem)
            End With
        End Sub
        Public Overrides ReadOnly Property SupportedTypes As Type()
            Get
                Return {GetType(Saves.RBSave)}
            End Get
        End Property

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
        Public Overrides ReadOnly Property SortOrder As Integer
            Get
                Return 26
            End Get
        End Property
        Private Sub OnModified(sender As Object, e As EventArgs) Handles txtGeneral_TeamName.TextChanged,
                                                                        numGeneral_HeldMoney.ValueChanged,
                                                                        numGeneral_StoredMoney.ValueChanged,
                                                                        numGeneral_RescuePoints.ValueChanged,
                                                                        cbGeneral_Base.SelectionChanged
            RaiseModified()
        End Sub
    End Class
End Namespace