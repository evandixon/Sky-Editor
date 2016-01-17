Imports SkyEditorBase
Namespace Tabs
    Public Class EpisodeActivePokemonTab
        Inherits ObjectTab(Of Saves.SkySave)
        Public Overrides Sub RefreshDisplay()
            lbActivePokemon.Items.Clear()
            For Each apkm In EditingItem.SpEpisodeActivePokemon
                If apkm.IsValid Then
                    lbActivePokemon.Items.Add(apkm)
                End If
            Next
        End Sub
        Public Overrides Sub UpdateObject()
            Dim apkms As New List(Of Interfaces.iMDPkm)
            For Each item In lbActivePokemon.Items
                apkms.Add(item)
            Next
            EditingItem.SpEpisodeActivePokemon = apkms.ToArray
        End Sub
        Private Sub ActivePokemonTab_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Header = PluginHelper.GetLanguageItem("Sp. Episode Party")
            btnEditActivePokemon.Content = PluginHelper.GetLanguageItem("Edit")
        End Sub
        Sub RefreshActivePKMDisplay()
            Dim pkms As New List(Of Interfaces.iMDPkm)
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
                Dim w = Me.GetPluginManager.GetObjectWindow
                w.ObjectToEdit = lbActivePokemon.SelectedItem
                w.ShowDialog()
                lbActivePokemon.SelectedItem = w.ObjectToEdit
                RaiseModified()
                RefreshActivePKMDisplay()
            End If
        End Sub
        Private Sub btnEditActivePokemon_Click(sender As Object, e As RoutedEventArgs) Handles btnEditActivePokemon.Click
            ShowActivePkmEditDialog()
        End Sub
        Private Sub lbActivePokemon_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs) Handles lbActivePokemon.MouseDoubleClick
            ShowActivePkmEditDialog()
        End Sub
        Public Overrides ReadOnly Property SupportedTypes As Type()
            Get
                Return {GetType(Saves.SkySave)}
            End Get
        End Property
        Public Overrides ReadOnly Property SortOrder As Integer
            Get
                Return 3
            End Get
        End Property
    End Class

End Namespace