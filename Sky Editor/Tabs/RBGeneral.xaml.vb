Imports SkyEditorBase

Namespace Tabs
    Public Class RBGeneral
        Inherits EditorTab
        Public Overrides Sub RefreshDisplay(Save As GenericSave)
            If Save.IsOfType(GameStrings.RBSave) Then
                With Save.Convert(Of Saves.RBSave)()
                    txtGeneral_TeamName.Text = .TeamName
                    numGeneral_HeldMoney.Value = .HeldMoney
                    numGeneral_StoredMoney.Value = .StoredMoney
                    numGeneral_RescuePoints.Value = .RescuePoints
                    cbGeneral_Base.SelectedItem = Lists.RBBaseTypesInverse(.BaseType)
                End With
            End If
        End Sub

        Public Overrides Function UpdateSave(Save As GenericSave) As GenericSave
            If Save.IsOfType(GameStrings.RBSave) Then
                Dim rb = Save.Convert(Of Saves.RBSave)()
                With rb
                    .TeamName = txtGeneral_TeamName.Text
                    .HeldMoney = numGeneral_HeldMoney.Value
                    .StoredMoney = numGeneral_StoredMoney.Value
                    .RescuePoints = numGeneral_RescuePoints.Value
                    .BaseType = Lists.RBBaseTypes(cbGeneral_Base.SelectedItem)
                End With
                Save = Save.Convert(Of Saves.RBSave)(rb)
            End If
            Return Save
        End Function
        Public Overrides ReadOnly Property SupportedGames As String()
            Get
                Return {GameStrings.RBSave}
            End Get
        End Property

        Private Sub RBGeneral_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Header = PluginHelper.GetLanguageItem("General")
        End Sub
        Public Overrides ReadOnly Property SortOrder As Integer
            Get
                Return 26
            End Get
        End Property
    End Class
End Namespace