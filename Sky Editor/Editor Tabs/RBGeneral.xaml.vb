Imports SkyEditorBase

Public Class RBGeneral
    Inherits EditorTab
    Public Overrides Sub RefreshDisplay(Save As GenericSave)
        If TypeOf Save Is RBSave Then
            With DirectCast(Save, RBSave)
                txtGeneral_TeamName.Text = .TeamName
                numGeneral_HeldMoney.Value = .HeldMoney
                numGeneral_StoredMoney.Value = .StoredMoney
                numGeneral_RescuePoints.Value = .RescuePoints
                cbGeneral_Base.SelectedItem = Lists.RBBaseTypesInverse(.BaseType)
            End With
        End If
    End Sub

    Public Overrides ReadOnly Property SupportedGames As String()
        Get
            Return {GameConstants.RBSave}
        End Get
    End Property

    Public Overrides Function UpdateSave(Save As GenericSave) As GenericSave
        Dim out As GenericSave = Nothing
        If TypeOf Save Is RBSave Then
            Dim rb = DirectCast(Save, RBSave)
            With rb
                .TeamName = txtGeneral_TeamName.Text
                .HeldMoney = numGeneral_HeldMoney.Value
                .StoredMoney = numGeneral_StoredMoney.Value
                .RescuePoints = numGeneral_RescuePoints.Value
                .BaseType = Lists.RBBaseTypes(cbGeneral_Base.SelectedItem)
            End With
            out = rb.ToBase
        End If
        Return out
    End Function

    Private Sub GeneralTab_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Me.Header = PluginHelper.GetLanguageItem("Category_General", "General")
        lblGeneral_Adventures.Content = PluginHelper.GetLanguageItem("Adventures")
        lblGeneral_HeldMoney.Content = PluginHelper.GetLanguageItem("General_HeldMoney", "Held Money:")
        lblGeneral_RescuePoints.Content = PluginHelper.GetLanguageItem("RescuePoints", "Rescue Points:")
        lblGeneral_StoredMoney.Content = PluginHelper.GetLanguageItem("General_StoredMoney", "Stored Money:")
        lblGeneral_TeamName.Content = PluginHelper.GetLanguageItem("General_TeamName", "Team Name:")

        numGeneral_Adventures.Maximum = Integer.MaxValue
        numGeneral_Adventures.Minimum = Integer.MinValue
        numGeneral_StoredMoney.Maximum = 9999999 'Integer.MaxValue '16580607
        numGeneral_StoredMoney.Minimum = 0

        'Load Base Type dropdown
        For Each item In Lists.RBBaseTypes.Keys
            cbGeneral_Base.Items.Add(item)
        Next
    End Sub
End Class
