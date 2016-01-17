Imports SkyEditorBase
Imports SaveEditor.Saves
Namespace Tabs
    Public Class SkyGeneralTab
        Inherits ObjectTab(Of SkySave)
        Public Overrides Sub RefreshDisplay()
            With EditingItem
                numGeneral_StoredMoney.Value = .StoredMoney
                numGeneral_HeldMoney.Value = .HeldMoney
                numGeneral_SpEpisodeHeldMoney.Value = .SpEpisode_HeldMoney
                numGeneral_Adventures.Value = .Adventures
                txtGeneral_TeamName.Text = .TeamName
                numGeneral_RankPoints.Value = .ExplorerRank
            End With
        End Sub

        'Public Overrides ReadOnly Property SupportedTypes As Type()
        '    Get
        '        Return {GetType(Saves.SkySave)}
        '    End Get
        'End Property

        Public Overrides Sub UpdateObject()
            With EditingItem
                .StoredMoney = numGeneral_StoredMoney.Value
                .HeldMoney = numGeneral_HeldMoney.Value
                .SpEpisode_HeldMoney = numGeneral_SpEpisodeHeldMoney.Value
                .Adventures = numGeneral_Adventures.Value
                .TeamName = txtGeneral_TeamName.Text
                .ExplorerRank = numGeneral_RankPoints.Value
            End With
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

        Private Sub OnModified(sender As Object, e As EventArgs) Handles txtGeneral_TeamName.TextChanged,
                                                                         numGeneral_StoredMoney.ValueChanged,
                                                                         numGeneral_HeldMoney.ValueChanged,
                                                                         numGeneral_SpEpisodeHeldMoney.ValueChanged,
                                                                         numGeneral_Adventures.ValueChanged,
                                                                         numGeneral_RankPoints.ValueChanged
            RaiseModified()
        End Sub

        Public Overrides ReadOnly Property SortOrder As Integer
            Get
                Return 26
            End Get
        End Property
    End Class
End Namespace