Imports SkyEditor.skyjed.util
Imports SkyEditor.skyjed.buffer
Imports SkyEditorBase
Imports SkyEditor.skyjed.save
Imports SkyEditorBase.Utilities

Public Class ActivePkmWindow
    Public Property JSkyPokemonEx As skyjed.save.iPkmnEx
    Private WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Private WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog

    Private Sub ActivePkmWindow_Closing(sender As Object, e As ComponentModel.CancelEventArgs) Handles Me.Closing
        UpdatePKM()
    End Sub
    Private Sub PkmWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        'Load Language
        PluginHelper.TranslateForm(Me)

        'Initialize Dialogs
        OpenFileDialog1 = New Forms.OpenFileDialog
        OpenFileDialog1.Filter = "Pokemon Files (*.skypkm)|*.skypkm|All Files (*.*)|*.*"
        SaveFileDialog1 = New Forms.SaveFileDialog
        SaveFileDialog1.Filter = "Pokemon Files (*.skypkm)|*.skypkm|All Files (*.*)|*.*"
        If TypeOf JSkyPokemonEx Is SkyPkmnEx Then
            Dim keys As New Generic.List(Of GenericListItem(Of Integer))
            For Each item In Lists.SkyPokemon.Keys
                keys.Add(New GenericListItem(Of Integer)(Lists.SkyPokemon(item), item))
            Next
            keys.Sort()
            For Each item In keys
                cbPokemon.Items.Add(item)
            Next
            Dim keys2 As New Generic.List(Of GenericListItem(Of Integer))
            For Each item In Lists.SkyMoves.Keys
                keys2.Add(New GenericListItem(Of Integer)(Lists.SkyMoves(item), item))
            Next
            keys2.Sort()
            For Each item In keys2
                cbMove1.Items.Add(item)
                cbMove2.Items.Add(item)
                cbMove3.Items.Add(item)
                cbMove4.Items.Add(item)
            Next
            Dim keys3 As New Generic.List(Of GenericListItem(Of Integer))
            For Each item In Lists.SkyLocations.Keys
                keys3.Add(New GenericListItem(Of Integer)(Lists.SkyLocations(item), item))
            Next
            keys3.Sort()
            For Each item In keys3
                cbMetAt.Items.Add(item)
            Next
            cbPokemon.SelectedIndex = cbPokemon.Items.IndexOf(New GenericListItem(Of Integer)(Lists.SkyPokemon(JSkyPokemonEx.no), JSkyPokemonEx.no))
            cbMove1.SelectedIndex = cbMove1.Items.IndexOf(New GenericListItem(Of Integer)(Lists.SkyMoves(JSkyPokemonEx.attacks(0).no), JSkyPokemonEx.attacks(0).no))
            cbMove2.SelectedIndex = cbMove2.Items.IndexOf(New GenericListItem(Of Integer)(Lists.SkyMoves(JSkyPokemonEx.attacks(1).no), JSkyPokemonEx.attacks(1).no))
            cbMove3.SelectedIndex = cbMove3.Items.IndexOf(New GenericListItem(Of Integer)(Lists.SkyMoves(JSkyPokemonEx.attacks(2).no), JSkyPokemonEx.attacks(2).no))
            cbMove4.SelectedIndex = cbMove4.Items.IndexOf(New GenericListItem(Of Integer)(Lists.SkyMoves(JSkyPokemonEx.attacks(3).no), JSkyPokemonEx.attacks(3).no))
            cbMetAt.SelectedIndex = cbMetAt.Items.IndexOf(New GenericListItem(Of Integer)(Lists.SkyLocations(JSkyPokemonEx.metat), JSkyPokemonEx.metat))
        ElseIf TypeOf JSkyPokemonEx Is TDPkmnEx Then
            Dim keys As New Generic.List(Of GenericListItem(Of Integer))
            For Each item In Lists.SkyPokemon.Keys
                keys.Add(New GenericListItem(Of Integer)(Lists.SkyPokemon(item), item))
            Next
            keys.Sort()
            For Each item In keys
                cbPokemon.Items.Add(item)
            Next
            Dim keys2 As New Generic.List(Of GenericListItem(Of Integer))
            For Each item In Lists.SkyMoves.Keys
                keys2.Add(New GenericListItem(Of Integer)(Lists.SkyMoves(item), item))
            Next
            keys2.Sort()
            For Each item In keys2
                cbMove1.Items.Add(item)
                cbMove2.Items.Add(item)
                cbMove3.Items.Add(item)
                cbMove4.Items.Add(item)
            Next
            Dim keys3 As New Generic.List(Of GenericListItem(Of Integer))
            For Each item In Lists.SkyLocations.Keys
                keys3.Add(New GenericListItem(Of Integer)(Lists.SkyLocations(item), item))
            Next
            keys3.Sort()
            For Each item In keys3
                cbMetAt.Items.Add(item)
            Next
            cbPokemon.SelectedIndex = cbPokemon.Items.IndexOf(New GenericListItem(Of Integer)(Lists.SkyPokemon(JSkyPokemonEx.no), JSkyPokemonEx.no))
            cbMove1.SelectedIndex = cbMove1.Items.IndexOf(New GenericListItem(Of Integer)(Lists.SkyMoves(JSkyPokemonEx.attacks(0).no), JSkyPokemonEx.attacks(0).no))
            cbMove2.SelectedIndex = cbMove2.Items.IndexOf(New GenericListItem(Of Integer)(Lists.SkyMoves(JSkyPokemonEx.attacks(1).no), JSkyPokemonEx.attacks(1).no))
            cbMove3.SelectedIndex = cbMove3.Items.IndexOf(New GenericListItem(Of Integer)(Lists.SkyMoves(JSkyPokemonEx.attacks(2).no), JSkyPokemonEx.attacks(2).no))
            cbMove4.SelectedIndex = cbMove4.Items.IndexOf(New GenericListItem(Of Integer)(Lists.SkyMoves(JSkyPokemonEx.attacks(3).no), JSkyPokemonEx.attacks(3).no))
            cbMetAt.SelectedIndex = cbMetAt.Items.IndexOf(New GenericListItem(Of Integer)(Lists.TDLocations(JSkyPokemonEx.metat), JSkyPokemonEx.metat))
        End If

        'Load Values
        'numPkmID.Value = JSkyPokemonEx.no
        chbIsFemale.IsChecked = JSkyPokemonEx.isfemale
        txtName.Text = JSkyPokemonEx.name
        numLevel.Value = JSkyPokemonEx.lvl
        numMetFloor.Value = JSkyPokemonEx.metfl
        numIQ.Value = JSkyPokemonEx.iq
        numExp.Value = JSkyPokemonEx.exp
        numHP1.Value = JSkyPokemonEx.hp1
        numHP2.Value = JSkyPokemonEx.hp2
        numAttack.Value = JSkyPokemonEx.stats(0)
        numSpAttack.Value = JSkyPokemonEx.stats(1)
        numDefense.Value = JSkyPokemonEx.stats(2)
        numSpDefence.Value = JSkyPokemonEx.stats(3)
        numGinseng1.Value = JSkyPokemonEx.attacks(0).ginseng
        chbLinked1.IsChecked = JSkyPokemonEx.attacks(0).islinked
        chbSet1.IsChecked = JSkyPokemonEx.attacks(0).isset
        chbSwitched1.IsChecked = JSkyPokemonEx.attacks(0).isswitched
        chbSealed1.IsChecked = JSkyPokemonEx.attacks(0).unkflag
        numGinseng2.Value = JSkyPokemonEx.attacks(1).ginseng
        chbLinked2.IsChecked = JSkyPokemonEx.attacks(1).islinked
        chbSet2.IsChecked = JSkyPokemonEx.attacks(1).isset
        chbSwitched2.IsChecked = JSkyPokemonEx.attacks(1).isswitched
        chbSealed2.IsChecked = JSkyPokemonEx.attacks(1).unkflag
        numGinseng3.Value = JSkyPokemonEx.attacks(2).ginseng
        chbLinked3.IsChecked = JSkyPokemonEx.attacks(2).islinked
        chbSet3.IsChecked = JSkyPokemonEx.attacks(2).isset
        chbSwitched3.IsChecked = JSkyPokemonEx.attacks(2).isswitched
        chbSealed3.IsChecked = JSkyPokemonEx.attacks(2).unkflag
        numGinseng4.Value = JSkyPokemonEx.attacks(3).ginseng
        chbLinked4.IsChecked = JSkyPokemonEx.attacks(3).islinked
        chbSet4.IsChecked = JSkyPokemonEx.attacks(3).isset
        chbSwitched4.IsChecked = JSkyPokemonEx.attacks(3).isswitched
        chbSealed4.IsChecked = JSkyPokemonEx.attacks(3).unkflag
    End Sub
    Sub UpdatePKM()
        JSkyPokemonEx.no = cbPokemon.LastSafeValue.Value
        JSkyPokemonEx.isfemale = chbIsFemale.IsChecked
        JSkyPokemonEx.name = txtName.Text
        JSkyPokemonEx.lvl = numLevel.Value
        JSkyPokemonEx.metat = cbMetAt.LastSafeValue.Value
        JSkyPokemonEx.metfl = numMetFloor.Value
        JSkyPokemonEx.iq = numIQ.Value
        JSkyPokemonEx.exp = numExp.Value
        JSkyPokemonEx.hp1 = numHP1.Value
        JSkyPokemonEx.hp2 = numHP2.Value
        JSkyPokemonEx.stats(0) = numAttack.Value
        JSkyPokemonEx.stats(1) = numSpAttack.Value
        JSkyPokemonEx.stats(2) = numDefense.Value
        JSkyPokemonEx.stats(3) = numSpDefence.Value
        With JSkyPokemonEx.attacks(0)
            .no = cbMove1.LastSafeValue.Value
            .ginseng = numGinseng1.Value
            .islinked = chbLinked1.IsChecked
            .isset = chbSet1.IsChecked
            .isswitched = chbSwitched1.IsChecked
            .isvalid = (.no > 0)
            .unkflag = chbSealed1.IsChecked
        End With

        With JSkyPokemonEx.attacks(1)
            .no = cbMove2.LastSafeValue.Value
            .ginseng = numGinseng2.Value
            .islinked = chbLinked2.IsChecked
            .isset = chbSet2.IsChecked
            .isswitched = chbSwitched2.IsChecked
            .isvalid = (.no > 0)
            .unkflag = chbSealed2.IsChecked
        End With

        With JSkyPokemonEx.attacks(2)
            .no = cbMove3.LastSafeValue.Value
            .ginseng = numGinseng3.Value
            .islinked = chbLinked3.IsChecked
            .isset = chbSet3.IsChecked
            .isswitched = chbSwitched3.IsChecked
            .isvalid = (.no > 0)
            .unkflag = chbSealed3.IsChecked
        End With

        With JSkyPokemonEx.attacks(3)
            .no = cbMove4.LastSafeValue.Value
            .ginseng = numGinseng4.Value
            .islinked = chbLinked4.IsChecked
            .isset = chbSet4.IsChecked
            .isswitched = chbSwitched4.IsChecked
            .isvalid = (.no > 0)
            .unkflag = chbSealed4.IsChecked
        End With
    End Sub
    'Private Sub menuFileSaveAs_Click(sender As Object, e As RoutedEventArgs) Handles menuFileSaveAs.Click
    '    If SaveFileDialog1.ShowDialog = Forms.DialogResult.OK Then

    '        ' IO.File.WriteAllBytes(SaveFileDialog1.FileName, JSkyPokemonEx.)
    '    End If
    'End Sub
End Class