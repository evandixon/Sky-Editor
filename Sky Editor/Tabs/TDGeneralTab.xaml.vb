Imports SkyEditorBase

Namespace Tabs
    Public Class TDGeneralTab
        Inherits ObjectTab
        Public Overrides Sub RefreshDisplay()
            Dim Save = DirectCast(Me.EditingObject, GenericSave)
            If Save.IsOfType(GameStrings.TDSave) Then
                With Save.Convert(Of Saves.TDSave)()
                    txtGeneral_TeamName.Text = .TeamName
                End With
            End If
        End Sub

        Public Overrides ReadOnly Property SupportedTypes As Type()
            Get
                Return {GetType(Saves.TDSave)}
            End Get
        End Property

        Public Overrides Sub UpdateObject()
            Dim Save = DirectCast(Me.EditingObject, GenericSave)
            If Save.IsOfType(GameStrings.TDSave) Then
                Dim td = Save.Convert(Of Saves.TDSave)()
                With td
                    .TeamName = txtGeneral_TeamName.Text
                End With
                Me.EditingObject = Save.Convert(Of Saves.TDSave)(td)
            End If
        End Sub

        Private Sub GeneralTab_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Header = PluginHelper.GetLanguageItem("General", "General")
            lblGeneral_TeamName.Content = PluginHelper.GetLanguageItem("TeamName", "Team Name:")
        End Sub
        Public Overrides ReadOnly Property SortOrder As Integer
            Get
                Return 26
            End Get
        End Property

        Private Sub OnModified(sender As Object, e As EventArgs) Handles txtGeneral_TeamName.TextChanged
            RaiseModified()
        End Sub
    End Class
End Namespace