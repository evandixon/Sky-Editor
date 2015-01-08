Imports SkyEditorBase

Public Class TDGeneralTab
    Inherits EditorTab
    Public Overrides Sub RefreshDisplay(Save As GenericSave)
        If TypeOf Save Is TDSave Then
            With DirectCast(Save, TDSave)
                txtGeneral_TeamName.Text = .TeamName
            End With
        End If
    End Sub

    Public Overrides ReadOnly Property SupportedGames As String()
        Get
            Return {GameConstants.TDSave}
        End Get
    End Property

    Public Overrides Function UpdateSave(Save As GenericSave) As GenericSave
        Dim out As GenericSave = Nothing
        If TypeOf Save Is TDSave Then
            Dim td = DirectCast(Save, TDSave)
            With td
                .TeamName = txtGeneral_TeamName.Text
            End With
            out = td
        End If
        Return Save
    End Function

    Private Sub GeneralTab_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Me.Header = PluginHelper.GetLanguageItem("Category_General", "General")
        lblGeneral_TeamName.Content = PluginHelper.GetLanguageItem("General_TeamName", "Team Name:")
        'numGeneral_Adventures.Maximum = Integer.MaxValue
        'numGeneral_Adventures.Minimum = Integer.MinValue
        'numGeneral_StoredMoney.Maximum = 9999999 'Integer.MaxValue '16580607
        'numGeneral_StoredMoney.Minimum = 0
    End Sub
End Class
