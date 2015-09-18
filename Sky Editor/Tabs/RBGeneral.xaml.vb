Imports SkyEditorBase

Namespace Tabs
    Public Class RBGeneral
        Inherits ObjectTab
        Public Overrides Sub RefreshDisplay()
            Dim Save = DirectCast(Me.EditingObject, GenericSave)
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

        Public Overrides Sub UpdateObject()
            Dim Save = DirectCast(Me.EditingObject, GenericSave)
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
        End Sub
        Public Overrides ReadOnly Property SupportedTypes As Type()
            Get
                Return {GetType(Saves.RBSave)}
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
        Private Sub OnModified(sender As Object, e As EventArgs) Handles txtGeneral_TeamName.TextChanged,
                                                                        numGeneral_HeldMoney.ValueChanged,
                                                                        numGeneral_StoredMoney.ValueChanged,
                                                                        numGeneral_RescuePoints.ValueChanged,
                                                                        cbGeneral_Base.SelectionChanged
            RaiseModified()
        End Sub
    End Class
End Namespace