Imports SkyEditor.skyjed.util
Imports SkyEditor.skyjed.buffer
Imports SkyEditorBase
Imports SkyEditor.skyjed.save
Imports SkyEditorBase.Utilities

Public Class QPkmWindow
    Public Property JSkyPokemonQ As skyjed.save.iPkmnQ
    Private WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Private WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog

    Private Sub ActivePkmWindow_Closing(sender As Object, e As ComponentModel.CancelEventArgs) Handles Me.Closing
        UpdatePKM()
    End Sub
    Private Sub PkmWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        'Load Language
        lblPkm.Content = PluginHelper.GetLanguageItem("Pokemon")
        lblIsFemale.Content = PluginHelper.GetLanguageItem("Is Female")
        'lblName.Content = Lists.SkyEditorLanguageText("Name")
        lblLevel.Content = PluginHelper.GetLanguageItem("Level")
        lblExp.Content = PluginHelper.GetLanguageItem("Exp")
        'lblMetAt.Content = Lists.SkyEditorLanguageText("MetAt")
        'lblMetFloor.Content = Lists.SkyEditorLanguageText("MetFloor")
        'lblIQ.Content = Lists.SkyEditorLanguageText("IQ")
        lblHP1.Content = PluginHelper.GetLanguageItem("Current HP")
        lblHP2.Content = PluginHelper.GetLanguageItem("Max HP")
        lblAttack.Content = PluginHelper.GetLanguageItem("Attack")
        lblSpAttack.Content = PluginHelper.GetLanguageItem("Sp. Attack")
        lblDefense.Content = PluginHelper.GetLanguageItem("Defense")
        lblSpDefense.Content = PluginHelper.GetLanguageItem("Sp. Defense")

        lblMove.Content = PluginHelper.GetLanguageItem("Move")
        lblGinseng.Content = PluginHelper.GetLanguageItem("Ginseng")
        lblSet.Content = PluginHelper.GetLanguageItem("Set")
        lblSwitched.Content = PluginHelper.GetLanguageItem("Switched")
        lblLinked.Content = PluginHelper.GetLanguageItem("Linked")
        lblSealed.Content = PluginHelper.GetLanguageItem("Sealed")

        lblMove1.Content = PluginHelper.GetLanguageItem("Move1")
        lblMove2.Content = PluginHelper.GetLanguageItem("Move2")
        lblMove3.Content = PluginHelper.GetLanguageItem("Move3")
        lblMove4.Content = PluginHelper.GetLanguageItem("Move4")


        'Initialize Dialogs
        OpenFileDialog1 = New Forms.OpenFileDialog
        OpenFileDialog1.Filter = "Pokemon Files (*.skypkm)|*.skypkm|All Files (*.*)|*.*"
        SaveFileDialog1 = New Forms.SaveFileDialog
        SaveFileDialog1.Filter = "Pokemon Files (*.skypkm)|*.skypkm|All Files (*.*)|*.*"
        If TypeOf JSkyPokemonQ Is SkyPkmnQ Then
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
            'keys3.Sort()
            'For Each item In keys3
            '    cbMetAt.Items.Add(item)
            'Next
            cbPokemon.SelectedIndex = cbPokemon.Items.IndexOf(New GenericListItem(Of Integer)(Lists.SkyPokemon(JSkyPokemonQ.no1), JSkyPokemonQ.no1))
            cbMove1.SelectedIndex = cbMove1.Items.IndexOf(New GenericListItem(Of Integer)(Lists.SkyMoves(JSkyPokemonQ.attacks(0).no), JSkyPokemonQ.attacks(0).no))
            cbMove2.SelectedIndex = cbMove2.Items.IndexOf(New GenericListItem(Of Integer)(Lists.SkyMoves(JSkyPokemonQ.attacks(1).no), JSkyPokemonQ.attacks(1).no))
            cbMove3.SelectedIndex = cbMove3.Items.IndexOf(New GenericListItem(Of Integer)(Lists.SkyMoves(JSkyPokemonQ.attacks(2).no), JSkyPokemonQ.attacks(2).no))
            cbMove4.SelectedIndex = cbMove4.Items.IndexOf(New GenericListItem(Of Integer)(Lists.SkyMoves(JSkyPokemonQ.attacks(3).no), JSkyPokemonQ.attacks(3).no))
            'cbMetAt.SelectedIndex = cbMetAt.Items.IndexOf(New GenericListItem(Of Integer)(Lists.SkyLocations(JSkyPokemonQ.metat), JSkyPokemonQ.metat))
        ElseIf TypeOf JSkyPokemonQ Is TDPkmnEx Then
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
            'keys3.Sort()
            'For Each item In keys3
            '    cbMetAt.Items.Add(item)
            'Next
            cbPokemon.SelectedIndex = cbPokemon.Items.IndexOf(New GenericListItem(Of Integer)(Lists.SkyPokemon(JSkyPokemonQ.no1), JSkyPokemonQ.no1))
            cbMove1.SelectedIndex = cbMove1.Items.IndexOf(New GenericListItem(Of Integer)(Lists.SkyMoves(JSkyPokemonQ.attacks(0).no), JSkyPokemonQ.attacks(0).no))
            cbMove2.SelectedIndex = cbMove2.Items.IndexOf(New GenericListItem(Of Integer)(Lists.SkyMoves(JSkyPokemonQ.attacks(1).no), JSkyPokemonQ.attacks(1).no))
            cbMove3.SelectedIndex = cbMove3.Items.IndexOf(New GenericListItem(Of Integer)(Lists.SkyMoves(JSkyPokemonQ.attacks(2).no), JSkyPokemonQ.attacks(2).no))
            cbMove4.SelectedIndex = cbMove4.Items.IndexOf(New GenericListItem(Of Integer)(Lists.SkyMoves(JSkyPokemonQ.attacks(3).no), JSkyPokemonQ.attacks(3).no))
            'cbMetAt.SelectedIndex = cbMetAt.Items.IndexOf(New GenericListItem(Of Integer)(Lists.TDLocations(JSkyPokemonEx.metat), JSkyPokemonEx.metat))
        End If

        'Load Values
        'numPkmID.Value = JSkyPokemonEx.no
        chbIsFemale.IsChecked = JSkyPokemonQ.isfemale1
        'txtName.Text = JSkyPokemonEx.name
        numLevel.Value = JSkyPokemonQ.lvl
        'numMetFloor.Value = JSkyPokemonEx.metfl
        'numIQ.Value = JSkyPokemonEx.iq
        numExp.Value = JSkyPokemonQ.exp
        numHP1.Value = JSkyPokemonQ.hp1
        numHP2.Value = JSkyPokemonQ.hp2
        numAttack.Value = JSkyPokemonQ.stats(0)
        numSpAttack.Value = JSkyPokemonQ.stats(1)
        numDefense.Value = JSkyPokemonQ.stats(2)
        numSpDefence.Value = JSkyPokemonQ.stats(3)
        numGinseng1.Value = JSkyPokemonQ.attacks(0).ginseng
        chbLinked1.IsChecked = JSkyPokemonQ.attacks(0).islinked
        chbSet1.IsChecked = JSkyPokemonQ.attacks(0).isset
        chbSwitched1.IsChecked = JSkyPokemonQ.attacks(0).isswitched
        chbSealed1.IsChecked = JSkyPokemonQ.attacks(0).issealed
        numGinseng2.Value = JSkyPokemonQ.attacks(1).ginseng
        chbLinked2.IsChecked = JSkyPokemonQ.attacks(1).islinked
        chbSet2.IsChecked = JSkyPokemonQ.attacks(1).isset
        chbSwitched2.IsChecked = JSkyPokemonQ.attacks(1).isswitched
        chbSealed2.IsChecked = JSkyPokemonQ.attacks(1).issealed
        numGinseng3.Value = JSkyPokemonQ.attacks(2).ginseng
        chbLinked3.IsChecked = JSkyPokemonQ.attacks(2).islinked
        chbSet3.IsChecked = JSkyPokemonQ.attacks(2).isset
        chbSwitched3.IsChecked = JSkyPokemonQ.attacks(2).isswitched
        chbSealed3.IsChecked = JSkyPokemonQ.attacks(2).issealed
        numGinseng4.Value = JSkyPokemonQ.attacks(3).ginseng
        chbLinked4.IsChecked = JSkyPokemonQ.attacks(3).islinked
        chbSet4.IsChecked = JSkyPokemonQ.attacks(3).isset
        chbSwitched4.IsChecked = JSkyPokemonQ.attacks(3).isswitched
        chbSealed4.IsChecked = JSkyPokemonQ.attacks(3).issealed
    End Sub
    Sub UpdatePKM()
        JSkyPokemonQ.no1 = DirectCast(cbPokemon.SelectedItem, GenericListItem(Of Integer)).Value 'numPkmID.Value
        JSkyPokemonQ.isfemale1 = chbIsFemale.IsChecked
        'JSkyPokemonEx.name = txtName.Text
        JSkyPokemonQ.lvl = numLevel.Value
        'JSkyPokemonEx.metat = DirectCast(cbMetAt.SelectedValue, GenericListItem(Of Integer)).Value
        'JSkyPokemonEx.metfl = numMetFloor.Value
        'JSkyPokemonEx.iq = numIQ.Value
        JSkyPokemonQ.exp = numExp.Value
        JSkyPokemonQ.hp1 = numHP1.Value
        JSkyPokemonQ.hp2 = numHP2.Value
        JSkyPokemonQ.stats(0) = numAttack.Value
        JSkyPokemonQ.stats(1) = numSpAttack.Value
        JSkyPokemonQ.stats(2) = numDefense.Value
        JSkyPokemonQ.stats(3) = numSpDefence.Value
        With JSkyPokemonQ.attacks(0)
            .no = cbMove1.SelectedItem.Value
            .ginseng = numGinseng1.Value
            .islinked = chbLinked1.IsChecked
            .isset = chbSet1.IsChecked
            .isswitched = chbSwitched1.IsChecked
            .isvalid = (.no > 0)
            .issealed = chbSealed1.IsChecked
        End With

        With JSkyPokemonQ.attacks(1)
            .no = cbMove2.SelectedItem.Value
            .ginseng = numGinseng2.Value
            .islinked = chbLinked2.IsChecked
            .isset = chbSet2.IsChecked
            .isswitched = chbSwitched2.IsChecked
            .isvalid = (.no > 0)
            .issealed = chbSealed2.IsChecked
        End With

        With JSkyPokemonQ.attacks(2)
            .no = cbMove3.SelectedItem.Value
            .ginseng = numGinseng3.Value
            .islinked = chbLinked3.IsChecked
            .isset = chbSet3.IsChecked
            .isswitched = chbSwitched3.IsChecked
            .isvalid = (.no > 0)
            .issealed = chbSealed3.IsChecked
        End With

        With JSkyPokemonQ.attacks(3)
            .no = cbMove4.SelectedItem.Value
            .ginseng = numGinseng4.Value
            .islinked = chbLinked4.IsChecked
            .isset = chbSet4.IsChecked
            .isswitched = chbSwitched4.IsChecked
            .isvalid = (.no > 0)
            .issealed = chbSealed4.IsChecked
        End With
    End Sub
    'Private Sub menuFileSaveAs_Click(sender As Object, e As RoutedEventArgs) Handles menuFileSaveAs.Click
    '    If SaveFileDialog1.ShowDialog = Forms.DialogResult.OK Then

    '        ' IO.File.WriteAllBytes(SaveFileDialog1.FileName, JSkyPokemonEx.)
    '    End If
    'End Sub
End Class
