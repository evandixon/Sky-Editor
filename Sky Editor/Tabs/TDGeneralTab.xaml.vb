Imports SkyEditorBase

Namespace Tabs
    Public Class TDGeneralTab
        Inherits EditorTab
        Public Overrides Sub RefreshDisplay(Save As GenericSave)
            If Save.IsOfType(GameStrings.TDSave) Then
                With Save.Convert(Of Saves.TDSave)()
                    txtGeneral_TeamName.Text = .TeamName
                End With
            End If
        End Sub

        Public Overrides ReadOnly Property SupportedGames As String()
            Get
                Return {GameStrings.TDSave}
            End Get
        End Property

        Public Overrides Function UpdateSave(Save As GenericSave) As GenericSave
            If Save.IsOfType(GameStrings.TDSave) Then
                Dim td = Save.Convert(Of Saves.TDSave)()
                With td
                    .TeamName = txtGeneral_TeamName.Text
                End With
                Save = Save.Convert(Of Saves.TDSave)(td)
            End If
            Return Save
        End Function

        Private Sub GeneralTab_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Header = PluginHelper.GetLanguageItem("General", "General")
            lblGeneral_TeamName.Content = PluginHelper.GetLanguageItem("TeamName", "Team Name:")
        End Sub
        Public Overrides ReadOnly Property SortOrder As Integer
            Get
                Return 26
            End Get
        End Property
    End Class
End Namespace