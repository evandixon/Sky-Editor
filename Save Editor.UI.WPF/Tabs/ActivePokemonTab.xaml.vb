Imports SkyEditorBase
Imports SaveEditor.Interfaces
Imports SkyEditorBase.Interfaces
Imports SkyEditorWPF.UI

Namespace Tabs
    Public Class ActivePokemonTab
        Inherits ObjectControl

        Public Overrides Sub RefreshDisplay()
            lbActivePokemon.Items.Clear()
            For Each apkm In GetEditingObject(Of iParty).GetPokemon
                If apkm.IsValid Then
                    lbActivePokemon.Items.Add(apkm)
                End If
            Next
        End Sub

        Public Overrides Sub UpdateObject()
            Dim apkms As New List(Of iMDPkm)
            For Each item In lbActivePokemon.Items
                apkms.Add(item)
            Next
            GetEditingObject(Of iParty).SetPokemon(apkms.ToArray)
        End Sub

        Private Sub ActivePokemonTab_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Header = PluginHelper.GetLanguageItem("Party")
            btnEditActivePokemon.Content = PluginHelper.GetLanguageItem("Edit")
        End Sub
        Sub RefreshActivePKMDisplay()
            Dim pkms As New List(Of iMDPkm)
            For Each p In lbActivePokemon.Items
                pkms.Add(p)
            Next
            lbActivePokemon.Items.Clear()
            For count As Integer = 0 To pkms.Count - 1
                If pkms(count).ID > 0 Then
                    lbActivePokemon.Items.Add(pkms(count))
                End If
            Next
        End Sub
        Sub ShowActivePkmEditDialog()
            If lbActivePokemon.SelectedIndex > -1 Then
                Dim w As New ObjectWindow
                w.ObjectToEdit = lbActivePokemon.SelectedItem
                w.ShowDialog()
                lbActivePokemon.SelectedItem = w.ObjectToEdit
                IsModified = True
                RefreshActivePKMDisplay()
            End If
        End Sub
        Private Sub btnEditActivePokemon_Click(sender As Object, e As RoutedEventArgs) Handles btnEditActivePokemon.Click
            ShowActivePkmEditDialog()
        End Sub

        Private Sub lbActivePokemon_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs) Handles lbActivePokemon.MouseDoubleClick
            ShowActivePkmEditDialog()
        End Sub

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of Type)
            Return {GetType(Interfaces.iParty)}
        End Function

        Public Overrides Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer
            Return 2
        End Function

    End Class

End Namespace