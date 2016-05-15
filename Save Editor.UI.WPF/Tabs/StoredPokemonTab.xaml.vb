Imports SkyEditorBase
Imports SkyEditorWPF.UI

Namespace Tabs
    Public Class StoredPokemonTab
        Inherits ObjectControl
        Dim storage As iPokemonStorage
        Dim pokemon As iMDPkm()
        Dim slots As StoredPokemonSlotDefinition()
        Private Sub gbPokemon_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs) Handles lbPokemon.MouseDoubleClick
            ShowPkmEditDialog()
        End Sub
        Private Sub btnEditPokemon_Click(sender As Object, e As RoutedEventArgs) Handles btnEditPokemon.Click
            ShowPkmEditDialog()
        End Sub
        Sub ShowPkmEditDialog()
            If lbPokemon.SelectedIndex > -1 Then
                Dim w As New ObjectWindow
                w.ObjectToEdit = lbPokemon.SelectedItem
                w.ShowDialog()
                lbPokemon.SelectedItem = w.ObjectToEdit
                IsModified = True
                RefreshPKMDisplay()
            End If
        End Sub
        Sub RefreshPKMDisplay()
            Dim pkms As New List(Of Object)
            For Each p In lbPokemon.Items
                pkms.Add(p)
            Next
            lbPokemon.Items.Clear()
            For count As Integer = 0 To pkms.Count - 1
                'If pkms(count).isvalid Then
                lbPokemon.Items.Add(pkms(count))
                'End If
            Next
            'ChangeHeader()
        End Sub

        Public Overrides Sub RefreshDisplay()
            storage = GetEditingObject(Of iPokemonStorage)()
            pokemon = storage.GetPokemon
            slots = storage.GetStoredPokemonOffsets
            RefreshSlotDisplay()
            ShowSlot(-1)
            isRefresh = False
        End Sub
        Private Sub ShowSlot(Index As Integer)
            lbPokemon.Items.Clear()
            If Index > -1 Then
                For count As Integer = slots(Index).Index To slots(Index).Index + slots(Index).Length - 1
                    lbPokemon.Items.Add(pokemon(count))
                Next
            Else
                For count As Integer = 0 To pokemon.Length - 1
                    lbPokemon.Items.Add(pokemon(count))
                Next
            End If
        End Sub
        Private Sub SaveSlot(Index As Integer)
            If Index > -1 Then
                For count As Integer = 0 To slots(Index).Length - 1
                    pokemon(slots(Index).Index + count) = lbPokemon.Items(count)
                Next
            Else
                For count As Integer = 0 To lbPokemon.Items.Count - 1
                    pokemon(count) = lbPokemon.Items(count)
                Next
            End If
        End Sub
        Private Sub RefreshSlotDisplay()
            Dim index As Integer = lbFriendArea.SelectedIndex
            isRefresh = True
            lbFriendArea.Items.Clear()
            For Each item In slots
                Dim number As Integer = 0
                For count As Integer = item.Index To item.Index + item.Length - 1
                    If pokemon(count).IsValid Then
                        number += 1
                    End If
                Next
                lbFriendArea.Items.Add(item.Name & String.Format(" ({0}/{1})", number, item.Length))
            Next
            isRefresh = True
            lbFriendArea.SelectedIndex = index
        End Sub
        Dim oldSlot As Integer = -1
        Dim isRefresh As Boolean = False
        Private Sub lbFriendArea_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lbFriendArea.SelectionChanged
            If Not isRefresh Then
                If oldSlot > -1 Then
                    SaveSlot(oldSlot)
                End If
                ShowSlot(lbFriendArea.SelectedIndex)
                oldSlot = lbFriendArea.SelectedIndex
                isRefresh = True
                RefreshSlotDisplay()
            Else
                isRefresh = False
            End If
        End Sub

        Public Overrides Sub UpdateObject()
            SaveSlot(lbFriendArea.SelectedIndex)
            GetEditingObject(Of iPokemonStorage).SetPokemon(pokemon)
        End Sub

        Private Sub StoredPokemonTab_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Header = My.Resources.Language.StoredPokemonTab
        End Sub

        Private Sub btnShowAll_Click(sender As Object, e As RoutedEventArgs) Handles btnShowAll.Click
            lbFriendArea.SelectedIndex = -1
        End Sub

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of Type)
            Return {GetType(iPokemonStorage)}
        End Function

        Public Overrides Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer
            Return 4
        End Function

    End Class

End Namespace