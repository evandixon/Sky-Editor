Imports SkyEditor.skyjed.save
Imports SkyEditorBase
Public Class RBStoredPokemonTab
    Inherits EditorTab
    Private Property Storage As New List(Of skyjed.save.iPkmn)
    Dim isSkySave As Boolean = False
    Dim isTDSave As Boolean = False
    Dim isRBSave As Boolean = False
    Private Sub gbPokemon_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs) Handles lbPokemon.MouseDoubleClick
        ShowPkmEditDialog()
    End Sub
    Private Sub btnEditPokemon_Click(sender As Object, e As RoutedEventArgs) Handles btnEditPokemon.Click
        ShowPkmEditDialog()
    End Sub
    Sub ShowPkmEditDialog()
        If lbPokemon.SelectedIndex > -1 Then
            'If save.IsSkySave Then
            Dim x As New PkmWindow
            x.JSkyPokemon = lbPokemon.SelectedItem
            x.ShowDialog()
            lbPokemon.SelectedItem = x.JSkyPokemon
            RefreshPKMDisplay()
            'End If
        End If
    End Sub
    Sub RefreshPKMDisplay()
            Dim pkms As New List(Of skyjed.save.iPkmn)
            For Each p In lbPokemon.Items
                pkms.Add(p)
            Next
            lbPokemon.Items.Clear()
            For count As Integer = 0 To pkms.Count - 1
                'If pkms(count).isvalid Then
                lbPokemon.Items.Add(pkms(count))
                'End If
            Next
            ChangeHeader()
    End Sub

    Public Overrides Sub RefreshDisplay(Save As GenericSave)
        If TypeOf Save Is RBSave Then
            isRBSave = True
            isTDSave = False
            isSkySave = False
            'Load Pokemon
            lbPokemon.Items.Clear()
            Dim pkmns = DirectCast(Save, RBSave).StoredPokemon.pkmns
            For count As Integer = 0 To pkmns.Length - 1
                'If count < 5 OrElse pkmns(count).isvalid Then
                lbPokemon.Items.Add(pkmns(count))
                Storage.Add(pkmns(count))
                'End If
            Next
        ElseIf TypeOf Save Is RBSaveEU Then
            isRBSave = True
            isTDSave = False
            isSkySave = False
            'Load Pokemon
            lbPokemon.Items.Clear()
            Dim pkmns = RBSaveEU.FromBase(Save).StoredPokemon.pkmns
            For count As Integer = 0 To pkmns.Length - 1
                'If count < 5 OrElse pkmns(count).isvalid Then
                lbPokemon.Items.Add(pkmns(count))
                Storage.Add(pkmns(count))
                'End If
            Next
        ElseIf TypeOf Save Is TDSave Then
            isRBSave = False
            isTDSave = True
            isSkySave = False
            'Load Pokemon
            lbPokemon.Items.Clear()
            Dim pkmns = DirectCast(Save, TDSave).StoredPokemon.pkmns
            For count As Integer = 0 To pkmns.Length - 1
                'If count < 5 OrElse pkmns(count).isvalid Then
                lbPokemon.Items.Add(pkmns(count))
                Storage.Add(pkmns(count))
                'End If
            Next
        ElseIf TypeOf Save Is SkySave Then
            isRBSave = False
            isTDSave = False
            isSkySave = True
            'Load Pokemon
            lbPokemon.Items.Clear()
            Dim pkmns = DirectCast(Save, SkySave).JSave.pkmnStorage.pkmns
            For count As Integer = 0 To pkmns.Length - 1
                'If count < 5 OrElse pkmns(count).isvalid Then
                lbPokemon.Items.Add(pkmns(count))
                Storage.Add(pkmns(count))
                'End If
            Next
        End If
        ChangeHeader()
    End Sub

    Public Overrides ReadOnly Property SupportedGames As String()
        Get
            Return {GameConstants.SkySave, GameConstants.TDSave, GameConstants.RBSave, GameConstants.RBSaveEU}
        End Get
    End Property

    Public Overrides Function UpdateSave(Save As GenericSave) As GenericSave
        Dim out As GenericSave = Nothing
        If TypeOf Save Is RBSave Then
            Dim rb = DirectCast(Save, RBSave)
            Dim pkms As New List(Of skyjed.save.RBPkmn)
            For Each p In Storage
                pkms.Add(p)
            Next
            Dim temppkm = rb.StoredPokemon
            temppkm.pkmns = pkms.ToArray
            rb.StoredPokemon = temppkm
            out = rb.ToBase
        ElseIf TypeOf Save Is RBSaveEU Then
            Dim rb = RBSaveEU.FromBase(Save)
            Dim pkms As New List(Of skyjed.save.RBPkmn)
            For Each p In Storage
                pkms.Add(p)
            Next
            Dim temppkm = rb.StoredPokemon
            temppkm.pkmns = pkms.ToArray
            rb.StoredPokemon = temppkm
            out = rb.ToBase
        ElseIf TypeOf Save Is TDSave Then
            Dim td = DirectCast(Save, TDSave)
            Dim pkms As New List(Of skyjed.save.TDPkmn)
            For Each p In Storage
                pkms.Add(p)
            Next
            Dim temppkm = td.StoredPokemon
            temppkm.pkmns = pkms.ToArray
            td.StoredPokemon = temppkm
            out = td
        ElseIf TypeOf Save Is SkySave Then
            Dim sky = DirectCast(Save, SkySave)
            'Update things using code from Sky JEditor
            Dim JSave = sky.JSave
            'update Pokemon
            Dim pkms As New List(Of skyjed.save.SkyPkmn)
            For Each p In Storage
                pkms.Add(p)
            Next
            JSave.pkmnStorage.pkmns = pkms.ToArray
            'Update JSave
            sky.JSave = JSave
            out = sky
        End If
        Return out
    End Function

    Private Sub StoredPokemonTab_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded

    End Sub
    Sub ChangeHeader(Optional RefreshFriendArea As Boolean = True)
        'load language
        btnEditPokemon.Content = Lists.SkyEditorLanguageText("Edit")
        btnShowAll.Content = Lists.SkyEditorLanguageText("ShowAll")

        If RefreshFriendArea Then
            Dim selectedFriendArea As Integer = lbFriendArea.SelectedIndex
            If selectedFriendArea > -1 Then
                Dim olditem = DirectCast(lbFriendArea.SelectedItem, RBSave.RBFriendAreaOffsetDefinition)
                For count As Integer = olditem.Index To (olditem.Index + olditem.Length) - 1
                    Storage(count) = lbPokemon.Items(count - olditem.Index)
                Next
            End If
            lbFriendArea.Items.Clear()
            Dim friendAreas As List(Of RBSave.RBFriendAreaOffsetDefinition)
            If isSkySave Then
                friendAreas = RBSave.RBFriendAreaOffsetDefinition.FromLines(IO.File.ReadAllText(IO.Path.Combine(Environment.CurrentDirectory, "Resources\" & Lists.CurrentLanguage & "\" & Lists.SubDirectory & "\SkyFriendAreaOffsets.txt")))
            ElseIf isTDSave Then
                friendAreas = RBSave.RBFriendAreaOffsetDefinition.FromLines(IO.File.ReadAllText(IO.Path.Combine(Environment.CurrentDirectory, "Resources\" & Lists.CurrentLanguage & "\" & Lists.SubDirectory & "\TDFriendAreaOffsets.txt")))
            ElseIf isRBSave Then
                friendAreas = RBSave.RBFriendAreaOffsetDefinition.FromLines(IO.File.ReadAllText(IO.Path.Combine(Environment.CurrentDirectory, "Resources\" & Lists.CurrentLanguage & "\" & Lists.SubDirectory & "\RBFriendAreaOffsets.txt")))
            Else
                friendAreas = RBSave.RBFriendAreaOffsetDefinition.FromLines(IO.File.ReadAllText(IO.Path.Combine(Environment.CurrentDirectory, "Resources\" & Lists.CurrentLanguage & "\" & Lists.SubDirectory & "\TDFriendAreaOffsets.txt")))
            End If
            For Each item In friendAreas
                Dim pkm As Integer = 0
                For count As Integer = item.Index To item.Index + item.Length - 1
                    If Storage.Count > count AndAlso Storage(count).GetIsValid Then pkm += 1
                Next
                item.CurrentPokemon = pkm
                lbFriendArea.Items.Add(item)
            Next
            lbFriendArea.SelectedIndex = selectedFriendArea
        End If
        If lbFriendArea.SelectedIndex > -1 Then
            Me.Header = String.Format(Lists.SkyEditorLanguageText("RBStoredPokemon"), lbFriendArea.SelectedItem)
        Else
            Me.Header = String.Format(Lists.SkyEditorLanguageText("RBStoredPokemon"), "All")
        End If
    End Sub

    Private Sub lbFriendArea_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lbFriendArea.SelectionChanged
        'save the old ones
        If e.RemovedItems.Count > 0 Then
            Dim olditem = DirectCast(e.RemovedItems(0), RBSave.RBFriendAreaOffsetDefinition)
            For count As Integer = olditem.Index To (olditem.Index + olditem.Length) - 1
                Storage(count) = lbPokemon.Items(count - olditem.Index)
            Next
        Else
            For count As Integer = 0 To Storage.Count - 1
                Storage(count) = lbPokemon.Items(count)
            Next
        End If
        'update the new ones
        lbPokemon.Items.Clear()
        If e.AddedItems.Count > 0 Then
            Dim newitem = DirectCast(e.AddedItems(0), RBSave.RBFriendAreaOffsetDefinition)
            For count As Integer = newitem.Index To (newitem.Index + newitem.Length) - 1
                lbPokemon.Items.Add(Storage(count))
            Next
        Else
            For count As Integer = 0 To Storage.Count - 1
                lbPokemon.Items.Add(Storage(count))
            Next
        End If
        ChangeHeader(False)
    End Sub

    Private Sub btnShowAll_Click(sender As Object, e As RoutedEventArgs) Handles btnShowAll.Click
        lbFriendArea.SelectedIndex = -1
    End Sub
End Class
