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
            End With
        End If
    End Sub

    Public Overrides ReadOnly Property SupportedGames As String()
        Get
            Return {GameConstants.SkySave}
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
                Dim j = .JSave
                j.storedMoney = numGeneral_StoredMoney.Value
                .JSave = j
            End With
            out = sky
        End If
        Return out
    End Function

    Private Sub GeneralTab_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Me.Header = Lists.SkyEditorLanguageText("Category_General")
        lblGeneral_Adventures.Content = Lists.SkyEditorLanguageText("Adventures")
        lblGeneral_HeldMoney.Content = Lists.SkyEditorLanguageText("General_HeldMoney")
        lblGeneral_SpEpisodeHeldMoney.Content = Lists.SkyEditorLanguageText("General_SpEpisodeHeldMoney")
        lblGeneral_StoredMoney.Content = Lists.SkyEditorLanguageText("General_StoredMoney")
        lblGeneral_TeamName.Content = Lists.SkyEditorLanguageText("General_TeamName")

        numGeneral_Adventures.Maximum = Integer.MaxValue
        numGeneral_Adventures.Minimum = Integer.MinValue
        numGeneral_StoredMoney.Maximum = 9999999 'Integer.MaxValue '16580607
        numGeneral_StoredMoney.Minimum = 0
    End Sub
End Class
