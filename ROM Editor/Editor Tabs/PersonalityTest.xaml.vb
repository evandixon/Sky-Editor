Imports SkyEditorBase
Imports ROMEditor.FileFormats
Imports ROMEditor.PMD_Explorers

Public Class PersonalityTest
    Inherits EditorTab

    Public Overrides Async Sub RefreshDisplay(Save As GenericSave)
        If TypeOf Save Is PMD_Explorers.SkyNDSRom Then
            Dim overlay = Await (DirectCast(Save, PMD_Explorers.SkyNDSRom)).GetPersonalityTestOverlay
            cbPartner01.SelectedIndex = Overlay13.GetPokemonID(overlay.Partner01)
            cbPartner02.SelectedIndex = Overlay13.GetPokemonID(overlay.Partner02)
            cbPartner03.SelectedIndex = Overlay13.GetPokemonID(overlay.Partner03)
            cbPartner04.SelectedIndex = Overlay13.GetPokemonID(overlay.Partner04)
            cbPartner05.SelectedIndex = Overlay13.GetPokemonID(overlay.Partner05)
            cbPartner06.SelectedIndex = Overlay13.GetPokemonID(overlay.Partner06)
            cbPartner07.SelectedIndex = Overlay13.GetPokemonID(overlay.Partner07)
            cbPartner08.SelectedIndex = Overlay13.GetPokemonID(overlay.Partner08)
            cbPartner09.SelectedIndex = Overlay13.GetPokemonID(overlay.Partner09)
            cbPartner10.SelectedIndex = Overlay13.GetPokemonID(overlay.Partner10)
            cbPartner11.SelectedIndex = Overlay13.GetPokemonID(overlay.Partner11)
            cbPartner12.SelectedIndex = Overlay13.GetPokemonID(overlay.Partner12)
            cbPartner13.SelectedIndex = Overlay13.GetPokemonID(overlay.Partner13)
            cbPartner14.SelectedIndex = Overlay13.GetPokemonID(overlay.Partner14)
            cbPartner15.SelectedIndex = Overlay13.GetPokemonID(overlay.Partner15)
            cbPartner16.SelectedIndex = Overlay13.GetPokemonID(overlay.Partner16)
            cbPartner17.SelectedIndex = Overlay13.GetPokemonID(overlay.Partner17)
            cbPartner18.SelectedIndex = Overlay13.GetPokemonID(overlay.Partner18)
            cbPartner19.SelectedIndex = Overlay13.GetPokemonID(overlay.Partner19)
            cbPartner20.SelectedIndex = Overlay13.GetPokemonID(overlay.Partner20)
            cbPartner21.SelectedIndex = Overlay13.GetPokemonID(overlay.Partner21)

            cbHardyMale.SelectedIndex = Overlay13.GetPokemonID(overlay.HardyMale)
            cbHardyFemale.SelectedIndex = Overlay13.GetPokemonID(overlay.HardyFemale)
            cbDocileMale.SelectedIndex = Overlay13.GetPokemonID(overlay.DocileMale)
            cbDocileFemale.SelectedIndex = Overlay13.GetPokemonID(overlay.DocileFemale)
            cbBraveMale.SelectedIndex = Overlay13.GetPokemonID(overlay.BraveMale)
            cbBraveFemale.SelectedIndex = Overlay13.GetPokemonID(overlay.BraveFemale)
            cbJollyMale.SelectedIndex = Overlay13.GetPokemonID(overlay.JollyMale)
            cbJollyFemale.SelectedIndex = Overlay13.GetPokemonID(overlay.JollyFemale)
            cbImpishMale.SelectedIndex = Overlay13.GetPokemonID(overlay.ImpishMale)
            cbImpishFemale.SelectedIndex = Overlay13.GetPokemonID(overlay.ImpishFemale)
            cbNaiveMale.SelectedIndex = Overlay13.GetPokemonID(overlay.NaiveMale)
            cbNaiveFemale.SelectedIndex = Overlay13.GetPokemonID(overlay.NaiveFemale)
            cbTimidMale.SelectedIndex = Overlay13.GetPokemonID(overlay.TimidMale)
            cbTimidFemale.SelectedIndex = Overlay13.GetPokemonID(overlay.TimidFemale)
            cbHastyMale.SelectedIndex = Overlay13.GetPokemonID(overlay.HastyMale)
            cbHastyFemale.SelectedIndex = Overlay13.GetPokemonID(overlay.HastyFemale)
            cbSassyMale.SelectedIndex = Overlay13.GetPokemonID(overlay.SassyMale)
            cbSassyFemale.SelectedIndex = Overlay13.GetPokemonID(overlay.SassyFemale)
            cbCalmMale.SelectedIndex = Overlay13.GetPokemonID(overlay.CalmMale)
            cbCalmFemale.SelectedIndex = Overlay13.GetPokemonID(overlay.CalmFemale)
            cbRelaxedMale.SelectedIndex = Overlay13.GetPokemonID(overlay.RelaxedMale)
            cbRelaxedFemale.SelectedIndex = Overlay13.GetPokemonID(overlay.RelaxedFemale)
            cbLonelyMale.SelectedIndex = Overlay13.GetPokemonID(overlay.LonelyMale)
            cbLonelyFemale.SelectedIndex = Overlay13.GetPokemonID(overlay.LonelyFemale)
            cbQuirkyMale.SelectedIndex = Overlay13.GetPokemonID(overlay.QuirkyMale)
            cbQuirkyFemale.SelectedIndex = Overlay13.GetPokemonID(overlay.QuirkyFemale)
            cbQuietMale.SelectedIndex = Overlay13.GetPokemonID(overlay.QuietMale)
            cbQuietFemale.SelectedIndex = Overlay13.GetPokemonID(overlay.QuietFemale)
            cbRashMale.SelectedIndex = Overlay13.GetPokemonID(overlay.RashMale)
            cbRashFemale.SelectedIndex = Overlay13.GetPokemonID(overlay.RashFemale)
            cbBoldMale.SelectedIndex = Overlay13.GetPokemonID(overlay.BoldMale)
            cbBoldFemale.SelectedIndex = Overlay13.GetPokemonID(overlay.BoldFemale)

            chbPartner01.IsChecked = Overlay13.GetPokemonGender(overlay.Partner01)
            chbPartner02.IsChecked = Overlay13.GetPokemonGender(overlay.Partner02)
            chbPartner03.IsChecked = Overlay13.GetPokemonGender(overlay.Partner03)
            chbPartner04.IsChecked = Overlay13.GetPokemonGender(overlay.Partner04)
            chbPartner05.IsChecked = Overlay13.GetPokemonGender(overlay.Partner05)
            chbPartner06.IsChecked = Overlay13.GetPokemonGender(overlay.Partner06)
            chbPartner07.IsChecked = Overlay13.GetPokemonGender(overlay.Partner07)
            chbPartner08.IsChecked = Overlay13.GetPokemonGender(overlay.Partner08)
            chbPartner09.IsChecked = Overlay13.GetPokemonGender(overlay.Partner09)
            chbPartner10.IsChecked = Overlay13.GetPokemonGender(overlay.Partner10)
            chbPartner11.IsChecked = Overlay13.GetPokemonGender(overlay.Partner11)
            chbPartner12.IsChecked = Overlay13.GetPokemonGender(overlay.Partner12)
            chbPartner13.IsChecked = Overlay13.GetPokemonGender(overlay.Partner13)
            chbPartner14.IsChecked = Overlay13.GetPokemonGender(overlay.Partner14)
            chbPartner15.IsChecked = Overlay13.GetPokemonGender(overlay.Partner15)
            chbPartner16.IsChecked = Overlay13.GetPokemonGender(overlay.Partner16)
            chbPartner17.IsChecked = Overlay13.GetPokemonGender(overlay.Partner17)
            chbPartner18.IsChecked = Overlay13.GetPokemonGender(overlay.Partner18)
            chbPartner19.IsChecked = Overlay13.GetPokemonGender(overlay.Partner19)
            chbPartner20.IsChecked = Overlay13.GetPokemonGender(overlay.Partner20)
            chbPartner21.IsChecked = Overlay13.GetPokemonGender(overlay.Partner21)

            chbHardyMale.IsChecked = Overlay13.GetPokemonGender(overlay.HardyMale)
            chbHardyFemale.IsChecked = Overlay13.GetPokemonGender(overlay.HardyFemale)
            chbDocileMale.IsChecked = Overlay13.GetPokemonGender(overlay.DocileMale)
            chbDocileFemale.IsChecked = Overlay13.GetPokemonGender(overlay.DocileFemale)
            chbBraveMale.IsChecked = Overlay13.GetPokemonGender(overlay.BraveMale)
            chbBraveFemale.IsChecked = Overlay13.GetPokemonGender(overlay.BraveFemale)
            chbJollyMale.IsChecked = Overlay13.GetPokemonGender(overlay.JollyMale)
            chbJollyFemale.IsChecked = Overlay13.GetPokemonGender(overlay.JollyFemale)
            chbImpishMale.IsChecked = Overlay13.GetPokemonGender(overlay.ImpishMale)
            chbImpishFemale.IsChecked = Overlay13.GetPokemonGender(overlay.ImpishFemale)
            chbNaiveMale.IsChecked = Overlay13.GetPokemonGender(overlay.NaiveMale)
            chbNaiveFemale.IsChecked = Overlay13.GetPokemonGender(overlay.NaiveFemale)
            chbTimidMale.IsChecked = Overlay13.GetPokemonGender(overlay.TimidMale)
            chbTimidFemale.IsChecked = Overlay13.GetPokemonGender(overlay.TimidFemale)
            chbHastyMale.IsChecked = Overlay13.GetPokemonGender(overlay.HastyMale)
            chbHastyFemale.IsChecked = Overlay13.GetPokemonGender(overlay.HastyFemale)
            chbSassyMale.IsChecked = Overlay13.GetPokemonGender(overlay.SassyMale)
            chbSassyFemale.IsChecked = Overlay13.GetPokemonGender(overlay.SassyFemale)
            chbCalmMale.IsChecked = Overlay13.GetPokemonGender(overlay.CalmMale)
            chbCalmFemale.IsChecked = Overlay13.GetPokemonGender(overlay.CalmFemale)
            chbRelaxedMale.IsChecked = Overlay13.GetPokemonGender(overlay.RelaxedMale)
            chbRelaxedFemale.IsChecked = Overlay13.GetPokemonGender(overlay.RelaxedFemale)
            chbLonelyMale.IsChecked = Overlay13.GetPokemonGender(overlay.LonelyMale)
            chbLonelyFemale.IsChecked = Overlay13.GetPokemonGender(overlay.LonelyFemale)
            chbQuirkyMale.IsChecked = Overlay13.GetPokemonGender(overlay.QuirkyMale)
            chbQuirkyFemale.IsChecked = Overlay13.GetPokemonGender(overlay.QuirkyFemale)
            chbQuietMale.IsChecked = Overlay13.GetPokemonGender(overlay.QuietMale)
            chbQuietFemale.IsChecked = Overlay13.GetPokemonGender(overlay.QuietFemale)
            chbRashMale.IsChecked = Overlay13.GetPokemonGender(overlay.RashMale)
            chbRashFemale.IsChecked = Overlay13.GetPokemonGender(overlay.RashFemale)
            chbBoldMale.IsChecked = Overlay13.GetPokemonGender(overlay.BoldMale)
            chbBoldFemale.IsChecked = Overlay13.GetPokemonGender(overlay.BoldFemale)

            Dim lang = Await (DirectCast(Save, SkyNDSRom).GetLanguageString)
            Dim offset As Integer = 1660

            txtHardyDe.Text = lang(offset + 0)
            txtHardyMP.Text = lang(offset + 1)
            txtHardyFP.Text = lang(offset + 2)
            txtDocileDe.Text = lang(offset + 3)
            txtDocileMP.Text = lang(offset + 4)
            txtDocileFP.Text = lang(offset + 5)
            txtBraveDe.Text = lang(offset + 6)
            txtBraveMP.Text = lang(offset + 7)
            txtBraveFP.Text = lang(offset + 8)
            txtJollyDe.Text = lang(offset + 9)
            txtJollyMP.Text = lang(offset + 10)
            txtJollyFP.Text = lang(offset + 11)
            txtImpishDe.Text = lang(offset + 12)
            txtImpishMP.Text = lang(offset + 13)
            txtImpishFP.Text = lang(offset + 14)
            txtNaiveDe.Text = lang(offset + 15)
            txtNaiveMP.Text = lang(offset + 16)
            txtNaiveFP.Text = lang(offset + 17)
            txtTimidDe.Text = lang(offset + 18)
            txtTimidMP.Text = lang(offset + 19)
            txtTimidFP.Text = lang(offset + 20)
            txtHastyDe.Text = lang(offset + 21)
            txtHastyMP.Text = lang(offset + 22)
            txtHastyFP.Text = lang(offset + 23)
            txtSassyDe.Text = lang(offset + 24)
            txtSassyMP.Text = lang(offset + 25)
            txtSassyFP.Text = lang(offset + 26)
            txtCalmDe.Text = lang(offset + 27)
            txtCalmMP.Text = lang(offset + 28)
            txtCalmFP.Text = lang(offset + 29)
            txtRelaxedDe.Text = lang(offset + 30)
            txtRelaxedMP.Text = lang(offset + 31)
            txtRelaxedFP.Text = lang(offset + 32)
            txtLonelyDe.Text = lang(offset + 33)
            txtLonelyMP.Text = lang(offset + 34)
            txtLonelyFP.Text = lang(offset + 35)
            txtQuirkyDe.Text = lang(offset + 36)
            txtQuirkyMP.Text = lang(offset + 37)
            txtQuirkyFP.Text = lang(offset + 38)
            txtQuietDe.Text = lang(offset + 39)
            txtQuietMP.Text = lang(offset + 40)
            txtQuietFP.Text = lang(offset + 41)
            txtRashDe.Text = lang(offset + 42)
            txtRashMP.Text = lang(offset + 43)
            txtRashFP.Text = lang(offset + 44)
            txtBoldDe.Text = lang(offset + 45)
            txtBoldMP.Text = lang(offset + 46)
            txtBoldFP.Text = lang(offset + 47)
        End If
    End Sub

    Public Overrides ReadOnly Property SupportedGames As String()
        Get
            Return {Constants.SkyNDSRom}
        End Get
    End Property

    Public Overrides Function UpdateSave(Save As GenericSave) As GenericSave
        If TypeOf Save Is PMD_Explorers.SkyNDSRom Then
            Dim o = (DirectCast(Save, PMD_Explorers.SkyNDSRom)).GetPersonalityTestOverlay.Result
            o.Partner01 = Overlay13.SetPokemonIDGender(cbPartner01.SelectedIndex, chbPartner01.IsChecked)
            o.Partner02 = Overlay13.SetPokemonIDGender(cbPartner02.SelectedIndex, chbPartner02.IsChecked)
            o.Partner03 = Overlay13.SetPokemonIDGender(cbPartner03.SelectedIndex, chbPartner03.IsChecked)
            o.Partner04 = Overlay13.SetPokemonIDGender(cbPartner04.SelectedIndex, chbPartner04.IsChecked)
            o.Partner05 = Overlay13.SetPokemonIDGender(cbPartner05.SelectedIndex, chbPartner05.IsChecked)
            o.Partner06 = Overlay13.SetPokemonIDGender(cbPartner06.SelectedIndex, chbPartner06.IsChecked)
            o.Partner07 = Overlay13.SetPokemonIDGender(cbPartner07.SelectedIndex, chbPartner07.IsChecked)
            o.Partner08 = Overlay13.SetPokemonIDGender(cbPartner08.SelectedIndex, chbPartner08.IsChecked)
            o.Partner09 = Overlay13.SetPokemonIDGender(cbPartner09.SelectedIndex, chbPartner09.IsChecked)
            o.Partner10 = Overlay13.SetPokemonIDGender(cbPartner10.SelectedIndex, chbPartner10.IsChecked)
            o.Partner11 = Overlay13.SetPokemonIDGender(cbPartner11.SelectedIndex, chbPartner11.IsChecked)
            o.Partner12 = Overlay13.SetPokemonIDGender(cbPartner12.SelectedIndex, chbPartner12.IsChecked)
            o.Partner13 = Overlay13.SetPokemonIDGender(cbPartner13.SelectedIndex, chbPartner13.IsChecked)
            o.Partner14 = Overlay13.SetPokemonIDGender(cbPartner14.SelectedIndex, chbPartner14.IsChecked)
            o.Partner15 = Overlay13.SetPokemonIDGender(cbPartner15.SelectedIndex, chbPartner15.IsChecked)
            o.Partner16 = Overlay13.SetPokemonIDGender(cbPartner16.SelectedIndex, chbPartner16.IsChecked)
            o.Partner17 = Overlay13.SetPokemonIDGender(cbPartner17.SelectedIndex, chbPartner17.IsChecked)
            o.Partner18 = Overlay13.SetPokemonIDGender(cbPartner18.SelectedIndex, chbPartner18.IsChecked)
            o.Partner19 = Overlay13.SetPokemonIDGender(cbPartner19.SelectedIndex, chbPartner19.IsChecked)
            o.Partner20 = Overlay13.SetPokemonIDGender(cbPartner20.SelectedIndex, chbPartner20.IsChecked)
            o.Partner21 = Overlay13.SetPokemonIDGender(cbPartner21.SelectedIndex, chbPartner21.IsChecked)

            o.HardyMale = Overlay13.SetPokemonIDGender(cbHardyMale.SelectedIndex, chbHardyMale.IsChecked)
            o.HardyFemale = Overlay13.SetPokemonIDGender(cbHardyFemale.SelectedIndex, chbHardyFemale.IsChecked)
            o.DocileMale = Overlay13.SetPokemonIDGender(cbDocileMale.SelectedIndex, chbDocileMale.IsChecked)
            o.DocileFemale = Overlay13.SetPokemonIDGender(cbDocileFemale.SelectedIndex, chbDocileFemale.IsChecked)
            o.BraveMale = Overlay13.SetPokemonIDGender(cbBraveMale.SelectedIndex, chbBraveMale.IsChecked)
            o.BraveFemale = Overlay13.SetPokemonIDGender(cbBraveFemale.SelectedIndex, chbBraveFemale.IsChecked)
            o.JollyMale = Overlay13.SetPokemonIDGender(cbJollyMale.SelectedIndex, chbJollyMale.IsChecked)
            o.JollyFemale = Overlay13.SetPokemonIDGender(cbJollyFemale.SelectedIndex, chbJollyFemale.IsChecked)
            o.ImpishMale = Overlay13.SetPokemonIDGender(cbImpishMale.SelectedIndex, chbImpishMale.IsChecked)
            o.ImpishFemale = Overlay13.SetPokemonIDGender(cbImpishFemale.SelectedIndex, chbImpishFemale.IsChecked)
            o.NaiveMale = Overlay13.SetPokemonIDGender(cbNaiveMale.SelectedIndex, chbNaiveMale.IsChecked)
            o.NaiveFemale = Overlay13.SetPokemonIDGender(cbNaiveFemale.SelectedIndex, chbNaiveFemale.IsChecked)
            o.TimidMale = Overlay13.SetPokemonIDGender(cbTimidMale.SelectedIndex, chbTimidMale.IsChecked)
            o.TimidFemale = Overlay13.SetPokemonIDGender(cbTimidFemale.SelectedIndex, chbTimidFemale.IsChecked)
            o.HastyMale = Overlay13.SetPokemonIDGender(cbHastyMale.SelectedIndex, chbHastyMale.IsChecked)
            o.HastyFemale = Overlay13.SetPokemonIDGender(cbHastyFemale.SelectedIndex, chbHastyFemale.IsChecked)
            o.SassyMale = Overlay13.SetPokemonIDGender(cbSassyMale.SelectedIndex, chbSassyMale.IsChecked)
            o.SassyFemale = Overlay13.SetPokemonIDGender(cbSassyFemale.SelectedIndex, chbSassyFemale.IsChecked)
            o.CalmMale = Overlay13.SetPokemonIDGender(cbCalmMale.SelectedIndex, chbCalmMale.IsChecked)
            o.CalmFemale = Overlay13.SetPokemonIDGender(cbCalmFemale.SelectedIndex, chbCalmFemale.IsChecked)
            o.RelaxedMale = Overlay13.SetPokemonIDGender(cbRelaxedMale.SelectedIndex, chbRelaxedMale.IsChecked)
            o.RelaxedFemale = Overlay13.SetPokemonIDGender(cbRelaxedFemale.SelectedIndex, chbRelaxedFemale.IsChecked)
            o.LonelyMale = Overlay13.SetPokemonIDGender(cbLonelyMale.SelectedIndex, chbLonelyMale.IsChecked)
            o.LonelyFemale = Overlay13.SetPokemonIDGender(cbLonelyFemale.SelectedIndex, chbLonelyFemale.IsChecked)
            o.QuirkyMale = Overlay13.SetPokemonIDGender(cbQuirkyMale.SelectedIndex, chbQuirkyMale.IsChecked)
            o.QuirkyFemale = Overlay13.SetPokemonIDGender(cbQuirkyFemale.SelectedIndex, chbQuirkyFemale.IsChecked)
            o.QuietMale = Overlay13.SetPokemonIDGender(cbQuietMale.SelectedIndex, chbQuietMale.IsChecked)
            o.QuietFemale = Overlay13.SetPokemonIDGender(cbQuietFemale.SelectedIndex, chbQuietFemale.IsChecked)
            o.RashMale = Overlay13.SetPokemonIDGender(cbRashMale.SelectedIndex, chbRashMale.IsChecked)
            o.RashFemale = Overlay13.SetPokemonIDGender(cbRashFemale.SelectedIndex, chbRashFemale.IsChecked)
            o.BoldMale = Overlay13.SetPokemonIDGender(cbBoldMale.SelectedIndex, chbBoldMale.IsChecked)
            o.BoldFemale = Overlay13.SetPokemonIDGender(cbBoldFemale.SelectedIndex, chbBoldFemale.IsChecked)

            Dim lang = (DirectCast(Save, PMD_Explorers.SkyNDSRom)).GetLanguageString.Result
            Dim offset As Integer = 1660

            lang(offset + 0) = txtHardyDe.Text
            lang(offset + 1) = txtHardyMP.Text
            lang(offset + 2) = txtHardyFP.Text
            lang(offset + 3) = txtDocileDe.Text
            lang(offset + 4) = txtDocileMP.Text
            lang(offset + 5) = txtDocileFP.Text
            lang(offset + 6) = txtBraveDe.Text
            lang(offset + 7) = txtBraveMP.Text
            lang(offset + 8) = txtBraveFP.Text
            lang(offset + 9) = txtJollyDe.Text
            lang(offset + 10) = txtJollyMP.Text
            lang(offset + 11) = txtJollyFP.Text
            lang(offset + 12) = txtImpishDe.Text
            lang(offset + 13) = txtImpishMP.Text
            lang(offset + 14) = txtImpishFP.Text
            lang(offset + 15) = txtNaiveDe.Text
            lang(offset + 16) = txtNaiveMP.Text
            lang(offset + 17) = txtNaiveFP.Text
            lang(offset + 18) = txtTimidDe.Text
            lang(offset + 19) = txtTimidMP.Text
            lang(offset + 20) = txtTimidFP.Text
            lang(offset + 21) = txtHastyDe.Text
            lang(offset + 22) = txtHastyMP.Text
            lang(offset + 23) = txtHastyFP.Text
            lang(offset + 24) = txtSassyDe.Text
            lang(offset + 25) = txtSassyMP.Text
            lang(offset + 26) = txtSassyFP.Text
            lang(offset + 27) = txtCalmDe.Text
            lang(offset + 28) = txtCalmMP.Text
            lang(offset + 29) = txtCalmFP.Text
            lang(offset + 30) = txtRelaxedDe.Text
            lang(offset + 31) = txtRelaxedMP.Text
            lang(offset + 32) = txtRelaxedFP.Text
            lang(offset + 33) = txtLonelyDe.Text
            lang(offset + 34) = txtLonelyMP.Text
            lang(offset + 35) = txtLonelyFP.Text
            lang(offset + 36) = txtQuirkyDe.Text
            lang(offset + 37) = txtQuirkyMP.Text
            lang(offset + 38) = txtQuirkyFP.Text
            lang(offset + 39) = txtQuietDe.Text
            lang(offset + 40) = txtQuietMP.Text
            lang(offset + 41) = txtQuietFP.Text
            lang(offset + 42) = txtRashDe.Text
            lang(offset + 43) = txtRashMP.Text
            lang(offset + 44) = txtRashFP.Text
            lang(offset + 45) = txtBoldDe.Text
            lang(offset + 46) = txtBoldMP.Text
            lang(offset + 47) = txtBoldFP.Text
            lang.Save()

            o.Save()
        End If
        Return Save
    End Function

    Private Sub PersonalityTest_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        Me.Header = "Personality Test"
        PluginHelper.TranslateForm(Me)
        For Each count In SkyEditor.Lists.SkyPokemon.Keys
            Dim item As String = SkyEditor.Lists.SkyPokemon(count)
            cbPartner01.Items.Add(item)
            cbPartner02.Items.Add(item)
            cbPartner03.Items.Add(item)
            cbPartner04.Items.Add(item)
            cbPartner05.Items.Add(item)
            cbPartner06.Items.Add(item)
            cbPartner07.Items.Add(item)
            cbPartner08.Items.Add(item)
            cbPartner09.Items.Add(item)
            cbPartner10.Items.Add(item)
            cbPartner11.Items.Add(item)
            cbPartner12.Items.Add(item)
            cbPartner13.Items.Add(item)
            cbPartner14.Items.Add(item)
            cbPartner15.Items.Add(item)
            cbPartner16.Items.Add(item)
            cbPartner17.Items.Add(item)
            cbPartner18.Items.Add(item)
            cbPartner19.Items.Add(item)
            cbPartner20.Items.Add(item)
            cbPartner21.Items.Add(item)

            cbHardyMale.Items.Add(item)
            cbHardyFemale.Items.Add(item)
            cbDocileMale.Items.Add(item)
            cbDocileFemale.Items.Add(item)
            cbBraveMale.Items.Add(item)
            cbBraveFemale.Items.Add(item)
            cbJollyMale.Items.Add(item)
            cbJollyFemale.Items.Add(item)
            cbImpishMale.Items.Add(item)
            cbImpishFemale.Items.Add(item)
            cbNaiveMale.Items.Add(item)
            cbNaiveFemale.Items.Add(item)
            cbTimidMale.Items.Add(item)
            cbTimidFemale.Items.Add(item)
            cbHastyMale.Items.Add(item)
            cbHastyFemale.Items.Add(item)
            cbSassyMale.Items.Add(item)
            cbSassyFemale.Items.Add(item)
            cbCalmMale.Items.Add(item)
            cbCalmFemale.Items.Add(item)
            cbRelaxedMale.Items.Add(item)
            cbRelaxedFemale.Items.Add(item)
            cbLonelyMale.Items.Add(item)
            cbLonelyFemale.Items.Add(item)
            cbQuirkyMale.Items.Add(item)
            cbQuirkyFemale.Items.Add(item)
            cbQuietMale.Items.Add(item)
            cbQuietFemale.Items.Add(item)
            cbRashMale.Items.Add(item)
            cbRashFemale.Items.Add(item)
            cbBoldMale.Items.Add(item)
            cbBoldFemale.Items.Add(item)
        Next
    End Sub
End Class
