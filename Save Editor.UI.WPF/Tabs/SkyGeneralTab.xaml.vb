Imports SkyEditorBase
Imports SkyEditorWPF.UI

Namespace Tabs
    Public Class SkyGeneralTab
        Inherits ObjectControl

        ''' <summary>
        ''' Updates UI elements to display certain properties.
        ''' </summary>
        Public Overrides Sub RefreshDisplay()
            With GetEditingObject(Of SkySave)()
                numGeneral_StoredMoney.Value = .StoredMoney
                numGeneral_HeldMoney.Value = .HeldMoney
                numGeneral_SpEpisodeHeldMoney.Value = .SpEpisode_HeldMoney
                numGeneral_Adventures.Value = .Adventures
                txtGeneral_TeamName.Text = .TeamName
                numGeneral_RankPoints.Value = .ExplorerRank
            End With
            IsModified = False
        End Sub

        ''' <summary>
        ''' Updates the EditingObject using data in UI elements.
        ''' </summary>
        Public Overrides Sub UpdateObject()
            With GetEditingObject(Of SkySave)()
                .StoredMoney = numGeneral_StoredMoney.Value
                .HeldMoney = numGeneral_HeldMoney.Value
                .SpEpisode_HeldMoney = numGeneral_SpEpisodeHeldMoney.Value
                .Adventures = numGeneral_Adventures.Value
                .TeamName = txtGeneral_TeamName.Text
                .ExplorerRank = numGeneral_RankPoints.Value
            End With
        End Sub

        Private Sub GeneralTab_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Header = My.Resources.Language.General

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
            IsModified = True
        End Sub

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of Type)
            Return {GetType(Saves.SkySave)}
        End Function

        Public Overrides Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer
            Return 0
        End Function
    End Class
End Namespace