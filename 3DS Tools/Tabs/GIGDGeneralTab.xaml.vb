Imports SkyEditorBase
Imports System.Windows

Namespace Tabs
    Public Class GIGDGeneralTab
        Inherits EditorTab
        Public Overrides Sub RefreshDisplay(Save As GenericSave)
            If Save.IsOfType(GameConstants.MDGatesData) Then
                With Save.Convert(Of Saves.GatesGameData)()
                    numGeneral_HeldMoney.Value = .HeldMoney
                End With
            End If
        End Sub

        Public Overrides ReadOnly Property SupportedGames As String()
            Get
                Return {GameConstants.MDGatesData}
            End Get
        End Property

        Public Overrides Function UpdateSave(Save As GenericSave) As GenericSave
            If Save.IsOfType(GameConstants.MDGatesData) Then
                Dim td = Save.Convert(Of Saves.GatesGameData)()
                With td
                    .HeldMoney = numGeneral_HeldMoney.Value
                End With
                Save = Save.Convert(Of Saves.GatesGameData)(td)
            End If
            Return Save
        End Function

        Private Sub GeneralTab_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Header = PluginHelper.GetLanguageItem("General", "General")
            'lblGeneral_TeamName.Content = PluginHelper.GetLanguageItem("TeamName", "Team Name:")
        End Sub
    End Class
End Namespace