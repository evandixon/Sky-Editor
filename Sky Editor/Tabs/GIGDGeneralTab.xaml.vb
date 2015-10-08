Imports SkyEditor.Saves
Imports SkyEditorBase

Namespace Tabs
    Public Class GIGDGeneralTab
        Inherits ObjectTab(Of GatesGameData)
        Public Overrides Sub RefreshDisplay()
            With editingitem
                numGeneral_HeldMoney.Value = .HeldMoney
            End With
        End Sub

        Public Overrides ReadOnly Property SupportedTypes As Type()
            Get
                Return {GetType(Saves.GatesGameData)}
            End Get
        End Property

        Public Overrides Sub UpdateObject()
            With EditingItem
                .HeldMoney = numGeneral_HeldMoney.Value
            End With
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