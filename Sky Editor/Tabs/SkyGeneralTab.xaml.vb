Imports SkyEditorBase
Imports SkyEditor.Saves
Namespace Tabs
    Public Class SkyGeneralTab
        Inherits ObjectTab
        Public Overrides Sub RefreshDisplay()
            Dim Save = DirectCast(Me.ContainedObject, GenericSave)
            If Save.IsOfType(GameStrings.SkySave) Then
                With Save.Convert(Of SkySave)()
                    numGeneral_StoredMoney.Value = .StoredMoney
                    numGeneral_HeldMoney.Value = .HeldMoney
                    numGeneral_SpEpisodeHeldMoney.Value = .SpEpisode_HeldMoney
                    numGeneral_Adventures.Value = .Adventures
                    txtGeneral_TeamName.Text = .TeamName
                    numGeneral_RankPoints.Value = .ExplorerRank
                End With
            End If
        End Sub

        Public Overrides ReadOnly Property SupportedTypes As Type()
            Get
                Return {GetType(Saves.SkySave)}
            End Get
        End Property

        Public Overrides Sub UpdateObject()
            Dim Save = DirectCast(Me.ContainedObject, GenericSave)
            Dim out As GenericSave = Nothing
            If Save.IsOfType(GameStrings.SkySave) Then
                Dim sky = Save.Convert(Of SkySave)()
                With sky
                    .StoredMoney = numGeneral_StoredMoney.Value
                    .HeldMoney = numGeneral_HeldMoney.Value
                    .SpEpisode_HeldMoney = numGeneral_SpEpisodeHeldMoney.Value
                    .Adventures = numGeneral_Adventures.Value
                    .TeamName = txtGeneral_TeamName.Text
                    .ExplorerRank = numGeneral_RankPoints.Value
                End With
                Me.ContainedObject = Save.Convert(sky)
            End If
        End Sub

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
        Public Overrides ReadOnly Property SortOrder As Integer
            Get
                Return 26
            End Get
        End Property
    End Class
End Namespace