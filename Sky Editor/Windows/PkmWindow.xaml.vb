Imports SkyEditor.skyjed.util
Imports SkyEditor.skyjed.buffer
Imports SkyEditorBase
Imports SkyEditor.skyjed.save
Imports SkyEditorBase.Utilities

Public Class PkmWindow
    Public Property JSkyPokemon As skyjed.save.iPkmn
    Private WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Private WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog

    Private Sub PkmWindow_Closing(sender As Object, e As ComponentModel.CancelEventArgs) Handles Me.Closing
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

        RefreshDisplay()
    End Sub
    Sub RefreshDisplay()
        'Initialize DropDowns and load values
        If TypeOf JSkyPokemon Is SkyPkmn Then
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
            cbPokemon.SelectedIndex = cbPokemon.Items.IndexOf(New GenericListItem(Of Integer)(Lists.SkyPokemon(JSkyPokemon.no), JSkyPokemon.no))
            cbMetAt.SelectedIndex = cbMetAt.Items.IndexOf(New GenericListItem(Of Integer)(Lists.SkyLocations(JSkyPokemon.metat), JSkyPokemon.metat))
            cbMove1.SelectedIndex = cbMove1.Items.IndexOf(New GenericListItem(Of Integer)(Lists.SkyMoves(JSkyPokemon.attacks(0).no), JSkyPokemon.attacks(0).no))
            cbMove2.SelectedIndex = cbMove2.Items.IndexOf(New GenericListItem(Of Integer)(Lists.SkyMoves(JSkyPokemon.attacks(1).no), JSkyPokemon.attacks(1).no))
            cbMove3.SelectedIndex = cbMove3.Items.IndexOf(New GenericListItem(Of Integer)(Lists.SkyMoves(JSkyPokemon.attacks(2).no), JSkyPokemon.attacks(2).no))
            cbMove4.SelectedIndex = cbMove4.Items.IndexOf(New GenericListItem(Of Integer)(Lists.SkyMoves(JSkyPokemon.attacks(3).no), JSkyPokemon.attacks(3).no))
        ElseIf TypeOf JSkyPokemon Is TDPkmn Then
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
            For Each item In Lists.TDLocations.Keys
                keys3.Add(New GenericListItem(Of Integer)(Lists.TDLocations(item), item))
            Next
            keys3.Sort()
            For Each item In keys3
                cbMetAt.Items.Add(item)
            Next
            cbPokemon.SelectedIndex = cbPokemon.Items.IndexOf(New GenericListItem(Of Integer)(Lists.SkyPokemon(JSkyPokemon.no), JSkyPokemon.no))
            cbMetAt.SelectedIndex = cbMetAt.Items.IndexOf(New GenericListItem(Of Integer)(Lists.TDLocations(JSkyPokemon.metat), JSkyPokemon.metat))
            cbMove1.SelectedIndex = cbMove1.Items.IndexOf(New GenericListItem(Of Integer)(Lists.SkyMoves(JSkyPokemon.attacks(0).no), JSkyPokemon.attacks(0).no))
            cbMove2.SelectedIndex = cbMove2.Items.IndexOf(New GenericListItem(Of Integer)(Lists.SkyMoves(JSkyPokemon.attacks(1).no), JSkyPokemon.attacks(1).no))
            cbMove3.SelectedIndex = cbMove3.Items.IndexOf(New GenericListItem(Of Integer)(Lists.SkyMoves(JSkyPokemon.attacks(2).no), JSkyPokemon.attacks(2).no))
            cbMove4.SelectedIndex = cbMove4.Items.IndexOf(New GenericListItem(Of Integer)(Lists.SkyMoves(JSkyPokemon.attacks(3).no), JSkyPokemon.attacks(3).no))
        ElseIf TypeOf JSkyPokemon Is RBPkmn Then
            Dim keys As New Generic.List(Of GenericListItem(Of Integer))
            For Each item In Lists.RBPokemon.Keys
                keys.Add(New GenericListItem(Of Integer)(Lists.RBPokemon(item), item))
            Next
            keys.Sort()
            For Each item In keys
                cbPokemon.Items.Add(item)
            Next
            Dim keys2 As New Generic.List(Of GenericListItem(Of Integer))
            For Each item In Lists.RBMoves.Keys
                keys2.Add(New GenericListItem(Of Integer)(Lists.RBMoves(item), item))
            Next
            keys2.Sort()
            For Each item In keys2
                cbMove1.Items.Add(item)
                cbMove2.Items.Add(item)
                cbMove3.Items.Add(item)
                cbMove4.Items.Add(item)
            Next
            Dim keys3 As New Generic.List(Of GenericListItem(Of Integer))
            'For Each item In Lists.SkyLocations.Keys
            '    keys3.Add(New GenericListItem(Of Integer)(Lists.SkyLocations(item), item))
            'Next
            For count As Integer = 0 To 255
                If Lists.RBLocations().ContainsKey(count) Then
                    keys3.Add(New GenericListItem(Of Integer)(Lists.RBLocations(count), count))
                Else
                    keys3.Add(New GenericListItem(Of Integer)(count, count))
                End If
            Next
            keys3.Sort()
            For Each item In keys3
                cbMetAt.Items.Add(item)
            Next
            cbPokemon.SelectedIndex = cbPokemon.Items.IndexOf(New GenericListItem(Of Integer)(Lists.RBPokemon(JSkyPokemon.no), JSkyPokemon.no))
            'cbMetAt.SelectedIndex = cbMetAt.Items.IndexOf(New GenericListItem(Of Integer)(Lists.SkyLocations(JSkyPokemon.metat), JSkyPokemon.metat))
            If Lists.RBLocations().ContainsKey(JSkyPokemon.metat) Then
                cbMetAt.SelectedIndex = cbMetAt.Items.IndexOf(New GenericListItem(Of Integer)(Lists.RBLocations(JSkyPokemon.metat), JSkyPokemon.metat))
            Else
                cbMetAt.SelectedIndex = cbMetAt.Items.IndexOf(New GenericListItem(Of Integer)((JSkyPokemon.metat), JSkyPokemon.metat))
            End If
            cbMove1.SelectedIndex = cbMove1.Items.IndexOf(New GenericListItem(Of Integer)(Lists.RBMoves(JSkyPokemon.attacks(0).no), JSkyPokemon.attacks(0).no))
            cbMove2.SelectedIndex = cbMove2.Items.IndexOf(New GenericListItem(Of Integer)(Lists.RBMoves(JSkyPokemon.attacks(1).no), JSkyPokemon.attacks(1).no))
            cbMove3.SelectedIndex = cbMove3.Items.IndexOf(New GenericListItem(Of Integer)(Lists.RBMoves(JSkyPokemon.attacks(2).no), JSkyPokemon.attacks(2).no))
            cbMove4.SelectedIndex = cbMove4.Items.IndexOf(New GenericListItem(Of Integer)(Lists.RBMoves(JSkyPokemon.attacks(3).no), JSkyPokemon.attacks(3).no))
        End If

        'Load Values

        chbIsFemale.IsChecked = JSkyPokemon.isfemale
        txtName.Text = JSkyPokemon.name
        numLevel.Value = JSkyPokemon.lvl

        numMetFloor.Value = JSkyPokemon.metfl
        numIQ.Value = JSkyPokemon.iq
        numExp.Value = JSkyPokemon.exp
        numHP.Value = JSkyPokemon.hp
        numAttack.Value = JSkyPokemon.stats(0)
        numSpAttack.Value = JSkyPokemon.stats(1)
        numDefense.Value = JSkyPokemon.stats(2)
        numSpDefence.Value = JSkyPokemon.stats(3)

        numGinseng1.Value = JSkyPokemon.attacks(0).ginseng
        chbLinked1.IsChecked = JSkyPokemon.attacks(0).islinked
        chbSet1.IsChecked = JSkyPokemon.attacks(0).isset
        chbSwitched1.IsChecked = JSkyPokemon.attacks(0).isswitched

        numGinseng2.Value = JSkyPokemon.attacks(1).ginseng
        chbLinked2.IsChecked = JSkyPokemon.attacks(1).islinked
        chbSet2.IsChecked = JSkyPokemon.attacks(1).isset
        chbSwitched2.IsChecked = JSkyPokemon.attacks(1).isswitched

        numGinseng3.Value = JSkyPokemon.attacks(2).ginseng
        chbLinked3.IsChecked = JSkyPokemon.attacks(2).islinked
        chbSet3.IsChecked = JSkyPokemon.attacks(2).isset
        chbSwitched3.IsChecked = JSkyPokemon.attacks(2).isswitched

        numGinseng4.Value = JSkyPokemon.attacks(3).ginseng
        chbLinked4.IsChecked = JSkyPokemon.attacks(3).islinked
        chbSet4.IsChecked = JSkyPokemon.attacks(3).isset
        chbSwitched4.IsChecked = JSkyPokemon.attacks(3).isswitched
    End Sub
    Sub UpdatePKM()
        JSkyPokemon.no = cbPokemon.LastSafeValue.Value
        JSkyPokemon.isfemale = chbIsFemale.IsChecked
        JSkyPokemon.name = txtName.Text
        JSkyPokemon.lvl = numLevel.Value
        JSkyPokemon.metat = cbMetAt.LastSafeValue.Value
        JSkyPokemon.metfl = numMetFloor.Value
        JSkyPokemon.iq = numIQ.Value
        JSkyPokemon.exp = numExp.Value
        JSkyPokemon.hp = numHP.Value
        JSkyPokemon.stats(0) = numAttack.Value
        JSkyPokemon.stats(1) = numSpAttack.Value
        JSkyPokemon.stats(2) = numDefense.Value
        JSkyPokemon.stats(3) = numSpDefence.Value
        With JSkyPokemon.attacks(0)
            .no = cbMove1.LastSafeValue.Value
            .ginseng = numGinseng1.Value
            .islinked = chbLinked1.IsChecked
            .isset = chbSet1.IsChecked
            .isswitched = chbSwitched1.IsChecked
            .isvalid = (.no > 0)
        End With

        With JSkyPokemon.attacks(1)
            .no = cbMove2.LastSafeValue.Value
            .ginseng = numGinseng2.Value
            .islinked = chbLinked2.IsChecked
            .isset = chbSet2.IsChecked
            .isswitched = chbSwitched2.IsChecked
            .isvalid = (.no > 0)
        End With

        With JSkyPokemon.attacks(2)
            .no = cbMove3.LastSafeValue.Value
            .ginseng = numGinseng3.Value
            .islinked = chbLinked3.IsChecked
            .isset = chbSet3.IsChecked
            .isswitched = chbSwitched3.IsChecked
            .isvalid = (.no > 0)
        End With

        With JSkyPokemon.attacks(3)
            .no = cbMove4.LastSafeValue.Value
            .ginseng = numGinseng4.Value
            .islinked = chbLinked4.IsChecked
            .isset = chbSet4.IsChecked
            .isswitched = chbSwitched4.IsChecked
            .isvalid = (.no > 0)
        End With
    End Sub
    Private Sub menuFileSaveAs_Click(sender As Object, e As RoutedEventArgs) Handles menuFileSaveAs.Click
        If SaveFileDialog1.ShowDialog = Forms.DialogResult.OK Then
            IO.File.WriteAllBytes(SaveFileDialog1.FileName, JSkyPokemon.GetBytes)
        End If
    End Sub

    Private Sub menuFileOpen_Click(sender As Object, e As RoutedEventArgs) Handles menuFileOpen.Click
        If OpenFileDialog1.ShowDialog = Forms.DialogResult.OK Then
            Dim d As Byte() = IO.File.ReadAllBytes(OpenFileDialog1.FileName)
            Select Case d.Length * 8
                Case SkyPkmn.LENGTH + 6
                    JSkyPokemon = SkyPkmn.FromBytes(d)
                    RefreshDisplay()
                Case TDPkmn.LENGTH + 6
                    JSkyPokemon = TDPkmn.FromBytes(d)
                    RefreshDisplay()
                Case RBPkmn.LENGTH + 6
                    JSkyPokemon = RBPkmn.FromBytes(d)
                    RefreshDisplay()
                Case Else
                    MessageBox.Show("The Pokemon you tried to load is not a supported size.")
            End Select
        End If
    End Sub
End Class