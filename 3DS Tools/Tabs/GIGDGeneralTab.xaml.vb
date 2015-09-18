Imports SkyEditorBase
Imports System.Windows

Namespace Tabs
    Public Class GIGDGeneralTab
        Inherits ObjectTab
        Public Overrides Sub RefreshDisplay()
            Dim Save = DirectCast(Me.EditingObject, GenericSave)
            If Save.IsOfType(GameConstants.MDGatesData) Then
                With Save.Convert(Of Saves.GatesGameData)()
                    numGeneral_HeldMoney.Value = .HeldMoney
                End With
            End If
        End Sub

        Public Overrides ReadOnly Property SupportedTypes As Type()
            Get
                Return {GetType(Saves.GatesGameData)}
            End Get
        End Property

        Public Overrides Sub UpdateObject()
            If Me.EditingObject.IsOfType(GameConstants.MDGatesData) Then
                Dim td = DirectCast(Me.EditingObject, GenericSave).Convert(Of Saves.GatesGameData)()
                With td
                    .HeldMoney = numGeneral_HeldMoney.Value
                End With
                Me.EditingObject = DirectCast(Me.EditingObject, GenericSave).Convert(td)
            End If
        End Sub

        Private Sub GeneralTab_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Header = PluginHelper.GetLanguageItem("General", "General")
            'lblGeneral_TeamName.Content = PluginHelper.GetLanguageItem("TeamName", "Team Name:")
        End Sub

        Private Sub OnModified(sender As Object, e As EventArgs) Handles numGeneral_HeldMoney.ValueChanged
            RaiseModified()
        End Sub
    End Class
End Namespace