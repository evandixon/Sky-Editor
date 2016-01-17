Imports SkyEditorBase

Public Class SkyGeneralTab
    Inherits EditorTab
    Public Overrides Sub RefreshDisplay(Save As GenericSave)
        If TypeOf Save Is SkySave Then
            With DirectCast(Save, SkySave)
                numGeneral_StoredMoney.Value = .JSave.storedMoney
                numGeneral_HeldMoney.Value = .HeldMoney
                numGeneral_SpEpisodeHeldMoney.Value = .SpEpisode_HeldMoney
                numGeneral_Adventures.Value = .AdventuresHad
                txtGeneral_TeamName.Text = .TeamName
                numGeneral_RankPoints.Value = .RankPoints
            End With
        End If
    End Sub

    Public Overrides ReadOnly Property SupportedGames As String()
        Get
            Return {GameStrings.SkySave}
        End Get
    End Property

    Public Overrides Function UpdateSave(Save As GenericSave) As GenericSave
        Dim out As GenericSave = Nothing
        If TypeOf Save Is SkySave Then
            Dim sky = DirectCast(Save, SkySave)
            With sky
                '.StoredMoney = numGeneral_StoredMoney.Value
                .HeldMoney = numGeneral_HeldMoney.Value
                .SpEpisode_HeldMoney = numGeneral_SpEpisodeHeldMoney.Value
                .AdventuresHad = numGeneral_Adventures.Value
                .TeamName = txtGeneral_TeamName.Text
                .RankPoints = numGeneral_RankPoints.Value

                Dim j = .JSave
                j.storedMoney = numGeneral_StoredMoney.Value
                .JSave = j
            End With
            out = sky
        End If
        Return out
    End Function

    Private Sub GeneralTab_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Me.Header = PluginHelper.GetLanguageItem("Category_General", "General")
        lblGeneral_Adventures.Content = PluginHelper.GetLanguageItem("Adventures", "Adventures:")
        lblGeneral_HeldMoney.Content = PluginHelper.GetLanguageItem("General_HeldMoney", "Held Money:")
        lblGeneral_SpEpisodeHeldMoney.Content = PluginHelper.GetLanguageItem("General_SpEpisodeHeldMoney", "Sp. Episode Held Money:")
        lblGeneral_StoredMoney.Content = PluginHelper.GetLanguageItem("General_StoredMoney", "Stored Money:")
        lblGeneral_TeamName.Content = PluginHelper.GetLanguageItem("General_TeamName", "Team Name:")
        lblGeneral_RankPoints.Content = PluginHelper.GetLanguageItem("Rank Points", "Rank Points:")

        numGeneral_Adventures.Maximum = Integer.MaxValue
        numGeneral_Adventures.Minimum = Integer.MinValue
        numGeneral_StoredMoney.Maximum = 9999999 'Integer.MaxValue '16580607
        numGeneral_StoredMoney.Minimum = 0
    End Sub
End Class