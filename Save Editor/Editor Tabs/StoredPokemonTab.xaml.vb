Public Class StoredPokemonTab
    Inherits EditorTab
    Dim IsRBSave As Boolean
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
        If IsRBSave Then
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
        Else
            Dim pkms As New List(Of skyjed.save.iPkmn)
            For Each p In lbPokemon.Items
                pkms.Add(p)
            Next
            lbPokemon.Items.Clear()
            For count As Integer = 0 To pkms.Count - 1
                If count < 5 OrElse pkms(count).isvalid Then
                    lbPokemon.Items.Add(pkms(count))
                End If
            Next
        End If
    End Sub

    Public Overrides Sub RefreshDisplay(Save As GenericSave)
        If Save.IsRBSave Then
            'Load Pokemon
            lbPokemon.Items.Clear()
            Dim pkmns = RBSave.FromBase(Save).StoredPokemon.pkmns
            For count As Integer = 0 To pkmns.Length - 1
                'If count < 5 OrElse pkmns(count).isvalid Then
                lbPokemon.Items.Add(pkmns(count))
                'End If
            Next
            IsRBSave = True
        ElseIf Save.IsTDSave Then
            'Load Pokemon
            lbPokemon.Items.Clear()
            Dim pkmns = TDSave.FromBase(Save).StoredPokemon.pkmns
            For count As Integer = 0 To pkmns.Length - 1
                'If count < 5 OrElse pkmns(count).isvalid Then
                lbPokemon.Items.Add(pkmns(count))
                'End If
            Next
            IsRBSave = False
        ElseIf Save.IsSkySave Then
            'Load Pokemon
            lbPokemon.Items.Clear()
            Dim pkmns = SkySave.FromBase(Save).JSave.pkmnStorage.pkmns
            For count As Integer = 0 To pkmns.Length - 1
                'If count < 5 OrElse pkmns(count).isvalid Then
                lbPokemon.Items.Add(pkmns(count))
                'End If
            Next
            IsRBSave = False
        End If
    End Sub

    Public Overrides ReadOnly Property SupportedGames As GameType
        Get
            Return GameType.TD Or GameType.Sky
        End Get
    End Property

    Public Overrides Function UpdateSave(Save As GenericSave) As GenericSave
        If Save.IsRBSave Then
            Dim pkms As New List(Of skyjed.save.RBPkmn)
            For Each p In lbPokemon.Items
                pkms.Add(p)
            Next
            Dim temppkm = DirectCast(Save, RBSave).StoredPokemon
            temppkm.pkmns = pkms.ToArray
            RBSave.FromBase(Save).StoredPokemon = temppkm
        ElseIf Save.IsTDSave Then
            'update Pokemon
            Dim pkms As New List(Of skyjed.save.TDPkmn)
            For Each p In lbPokemon.Items
                pkms.Add(p)
            Next
            Dim temppkm = DirectCast(Save, TDSave).StoredPokemon
            temppkm.pkmns = pkms.ToArray
            TDSave.FromBase(Save).StoredPokemon = temppkm
        ElseIf Save.IsSkySave Then
            'Update things using code from Sky JEditor
            Dim JSave = DirectCast(Save, SkySave).JSave
            'update Pokemon
            Dim pkms As New List(Of skyjed.save.SkyPkmn)
            For Each p In lbPokemon.Items
                pkms.Add(p)
            Next
            JSave.pkmnStorage.pkmns = pkms.ToArray
            'Update JSave
            SkySave.FromBase(Save).JSave = JSave
        End If
        Return Save
    End Function

    Private Sub StoredPokemonTab_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        ChangeHeader()
    End Sub
    Sub ChangeHeader()
        Me.Header = String.Format(Lists.LanguageText("Category_Pokemon"), lbPokemon.Items.Count)
    End Sub
End Class
