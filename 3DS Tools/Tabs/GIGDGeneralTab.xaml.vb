Imports SkyEditorBase
Imports System.Windows

Namespace Tabs
    Public Class GIGDGeneralTab
        Inherits ObjectTab
        Public Overrides Sub RefreshDisplay()
            Dim Save = DirectCast(Me.ContainedObject, GenericSave)
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
            If Me.ContainedObject.IsOfType(GameConstants.MDGatesData) Then
                Dim td = DirectCast(Me.ContainedObject, GenericSave).Convert(Of Saves.GatesGameData)()
                With td
                    .HeldMoney = numGeneral_HeldMoney.Value
                End With
                Me.ContainedObject = DirectCast(Me.ContainedObject, GenericSave).Convert(td)
            End If
        End Sub

        Private Sub GeneralTab_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Header = PluginHelper.GetLanguageItem("General", "General")
            'lblGeneral_TeamName.Content = PluginHelper.GetLanguageItem("TeamName", "Team Name:")
        End Sub
    End Class
End Namespace