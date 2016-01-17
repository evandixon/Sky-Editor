Imports SkyEditorBase
Imports ROMEditor.FileFormats

Namespace ObjectControls
    Public Class PersonalityTest
        Inherits ObjectControl(Of PersonalityTestContainer)
        Private Property ReportModified As Boolean

        Public Overrides Sub RefreshDisplay()
            ReportModified = False
            Dim overlay As PersonalityTestContainer = Nothing
            If TypeOf EditingObject Is ObjectFile(Of PersonalityTestContainer) Then
                overlay = DirectCast(EditingObject, ObjectFile(Of PersonalityTestContainer)).ContainedObject
            ElseIf TypeOf EditingObject Is PersonalityTestContainer Then
                overlay = DirectCast(EditingObject, PersonalityTestContainer)
            End If
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

            'Dim lang = Await (DirectCast(Save, SkyNDSRom).GetLanguageString)
            'Dim offset As Integer = 1660

            'txtHardyDe.Text = lang(offset + 0)
            'txtHardyMP.Text = lang(offset + 1)
            'txtHardyFP.Text = lang(offset + 2)
            'txtDocileDe.Text = lang(offset + 3)
            'txtDocileMP.Text = lang(offset + 4)
            'txtDocileFP.Text = lang(offset + 5)
            'txtBraveDe.Text = lang(offset + 6)
            'txtBraveMP.Text = lang(offset + 7)
            'txtBraveFP.Text = lang(offset + 8)
            'txtJollyDe.Text = lang(offset + 9)
            'txtJollyMP.Text = lang(offset + 10)
            'txtJollyFP.Text = lang(offset + 11)
            'txtImpishDe.Text = lang(offset + 12)
            'txtImpishMP.Text = lang(offset + 13)
            'txtImpishFP.Text = lang(offset + 14)
            'txtNaiveDe.Text = lang(offset + 15)
            'txtNaiveMP.Text = lang(offset + 16)
            'txtNaiveFP.Text = lang(offset + 17)
            'txtTimidDe.Text = lang(offset + 18)
            'txtTimidMP.Text = lang(offset + 19)
            'txtTimidFP.Text = lang(offset + 20)
            'txtHastyDe.Text = lang(offset + 21)
            'txtHastyMP.Text = lang(offset + 22)
            'txtHastyFP.Text = lang(offset + 23)
            'txtSassyDe.Text = lang(offset + 24)
            'txtSassyMP.Text = lang(offset + 25)
            'txtSassyFP.Text = lang(offset + 26)
            'txtCalmDe.Text = lang(offset + 27)
            'txtCalmMP.Text = lang(offset + 28)
            'txtCalmFP.Text = lang(offset + 29)
            'txtRelaxedDe.Text = lang(offset + 30)
            'txtRelaxedMP.Text = lang(offset + 31)
            'txtRelaxedFP.Text = lang(offset + 32)
            'txtLonelyDe.Text = lang(offset + 33)
            'txtLonelyMP.Text = lang(offset + 34)
            'txtLonelyFP.Text = lang(offset + 35)
            'txtQuirkyDe.Text = lang(offset + 36)
            'txtQuirkyMP.Text = lang(offset + 37)
            'txtQuirkyFP.Text = lang(offset + 38)
            'txtQuietDe.Text = lang(offset + 39)
            'txtQuietMP.Text = lang(offset + 40)
            'txtQuietFP.Text = lang(offset + 41)
            'txtRashDe.Text = lang(offset + 42)
            'txtRashMP.Text = lang(offset + 43)
            'txtRashFP.Text = lang(offset + 44)
            'txtBoldDe.Text = lang(offset + 45)
            'txtBoldMP.Text = lang(offset + 46)
            'txtBoldFP.Text = lang(offset + 47)
            ReportModified = True
        End Sub

        Public Overrides Sub UpdateObject()
            Dim o As PersonalityTestContainer = Nothing
            If TypeOf EditingObject Is ObjectFile(Of PersonalityTestContainer) Then
                o = DirectCast(EditingObject, ObjectFile(Of PersonalityTestContainer)).ContainedObject
            ElseIf TypeOf EditingObject Is PersonalityTestContainer Then
                o = DirectCast(EditingObject, PersonalityTestContainer)
            End If
            o.Partner01 = Overlay13.SetPokemonIDGender(cbPartner01.LastSafeIndex, chbPartner01.IsChecked)
            o.Partner02 = Overlay13.SetPokemonIDGender(cbPartner02.LastSafeIndex, chbPartner02.IsChecked)
            o.Partner03 = Overlay13.SetPokemonIDGender(cbPartner03.LastSafeIndex, chbPartner03.IsChecked)
            o.Partner04 = Overlay13.SetPokemonIDGender(cbPartner04.LastSafeIndex, chbPartner04.IsChecked)
            o.Partner05 = Overlay13.SetPokemonIDGender(cbPartner05.LastSafeIndex, chbPartner05.IsChecked)
            o.Partner06 = Overlay13.SetPokemonIDGender(cbPartner06.LastSafeIndex, chbPartner06.IsChecked)
            o.Partner07 = Overlay13.SetPokemonIDGender(cbPartner07.LastSafeIndex, chbPartner07.IsChecked)
            o.Partner08 = Overlay13.SetPokemonIDGender(cbPartner08.LastSafeIndex, chbPartner08.IsChecked)
            o.Partner09 = Overlay13.SetPokemonIDGender(cbPartner09.LastSafeIndex, chbPartner09.IsChecked)
            o.Partner10 = Overlay13.SetPokemonIDGender(cbPartner10.LastSafeIndex, chbPartner10.IsChecked)
            o.Partner11 = Overlay13.SetPokemonIDGender(cbPartner11.LastSafeIndex, chbPartner11.IsChecked)
            o.Partner12 = Overlay13.SetPokemonIDGender(cbPartner12.LastSafeIndex, chbPartner12.IsChecked)
            o.Partner13 = Overlay13.SetPokemonIDGender(cbPartner13.LastSafeIndex, chbPartner13.IsChecked)
            o.Partner14 = Overlay13.SetPokemonIDGender(cbPartner14.LastSafeIndex, chbPartner14.IsChecked)
            o.Partner15 = Overlay13.SetPokemonIDGender(cbPartner15.LastSafeIndex, chbPartner15.IsChecked)
            o.Partner16 = Overlay13.SetPokemonIDGender(cbPartner16.LastSafeIndex, chbPartner16.IsChecked)
            o.Partner17 = Overlay13.SetPokemonIDGender(cbPartner17.LastSafeIndex, chbPartner17.IsChecked)
            o.Partner18 = Overlay13.SetPokemonIDGender(cbPartner18.LastSafeIndex, chbPartner18.IsChecked)
            o.Partner19 = Overlay13.SetPokemonIDGender(cbPartner19.LastSafeIndex, chbPartner19.IsChecked)
            o.Partner20 = Overlay13.SetPokemonIDGender(cbPartner20.LastSafeIndex, chbPartner20.IsChecked)
            o.Partner21 = Overlay13.SetPokemonIDGender(cbPartner21.LastSafeIndex, chbPartner21.IsChecked)

            o.HardyMale = Overlay13.SetPokemonIDGender(cbHardyMale.LastSafeIndex, chbHardyMale.IsChecked)
            o.HardyFemale = Overlay13.SetPokemonIDGender(cbHardyFemale.LastSafeIndex, chbHardyFemale.IsChecked)
            o.DocileMale = Overlay13.SetPokemonIDGender(cbDocileMale.LastSafeIndex, chbDocileMale.IsChecked)
            o.DocileFemale = Overlay13.SetPokemonIDGender(cbDocileFemale.LastSafeIndex, chbDocileFemale.IsChecked)
            o.BraveMale = Overlay13.SetPokemonIDGender(cbBraveMale.LastSafeIndex, chbBraveMale.IsChecked)
            o.BraveFemale = Overlay13.SetPokemonIDGender(cbBraveFemale.LastSafeIndex, chbBraveFemale.IsChecked)
            o.JollyMale = Overlay13.SetPokemonIDGender(cbJollyMale.LastSafeIndex, chbJollyMale.IsChecked)
            o.JollyFemale = Overlay13.SetPokemonIDGender(cbJollyFemale.LastSafeIndex, chbJollyFemale.IsChecked)
            o.ImpishMale = Overlay13.SetPokemonIDGender(cbImpishMale.LastSafeIndex, chbImpishMale.IsChecked)
            o.ImpishFemale = Overlay13.SetPokemonIDGender(cbImpishFemale.LastSafeIndex, chbImpishFemale.IsChecked)
            o.NaiveMale = Overlay13.SetPokemonIDGender(cbNaiveMale.LastSafeIndex, chbNaiveMale.IsChecked)
            o.NaiveFemale = Overlay13.SetPokemonIDGender(cbNaiveFemale.LastSafeIndex, chbNaiveFemale.IsChecked)
            o.TimidMale = Overlay13.SetPokemonIDGender(cbTimidMale.LastSafeIndex, chbTimidMale.IsChecked)
            o.TimidFemale = Overlay13.SetPokemonIDGender(cbTimidFemale.LastSafeIndex, chbTimidFemale.IsChecked)
            o.HastyMale = Overlay13.SetPokemonIDGender(cbHastyMale.LastSafeIndex, chbHastyMale.IsChecked)
            o.HastyFemale = Overlay13.SetPokemonIDGender(cbHastyFemale.LastSafeIndex, chbHastyFemale.IsChecked)
            o.SassyMale = Overlay13.SetPokemonIDGender(cbSassyMale.LastSafeIndex, chbSassyMale.IsChecked)
            o.SassyFemale = Overlay13.SetPokemonIDGender(cbSassyFemale.LastSafeIndex, chbSassyFemale.IsChecked)
            o.CalmMale = Overlay13.SetPokemonIDGender(cbCalmMale.LastSafeIndex, chbCalmMale.IsChecked)
            o.CalmFemale = Overlay13.SetPokemonIDGender(cbCalmFemale.LastSafeIndex, chbCalmFemale.IsChecked)
            o.RelaxedMale = Overlay13.SetPokemonIDGender(cbRelaxedMale.LastSafeIndex, chbRelaxedMale.IsChecked)
            o.RelaxedFemale = Overlay13.SetPokemonIDGender(cbRelaxedFemale.LastSafeIndex, chbRelaxedFemale.IsChecked)
            o.LonelyMale = Overlay13.SetPokemonIDGender(cbLonelyMale.LastSafeIndex, chbLonelyMale.IsChecked)
            o.LonelyFemale = Overlay13.SetPokemonIDGender(cbLonelyFemale.LastSafeIndex, chbLonelyFemale.IsChecked)
            o.QuirkyMale = Overlay13.SetPokemonIDGender(cbQuirkyMale.LastSafeIndex, chbQuirkyMale.IsChecked)
            o.QuirkyFemale = Overlay13.SetPokemonIDGender(cbQuirkyFemale.LastSafeIndex, chbQuirkyFemale.IsChecked)
            o.QuietMale = Overlay13.SetPokemonIDGender(cbQuietMale.LastSafeIndex, chbQuietMale.IsChecked)
            o.QuietFemale = Overlay13.SetPokemonIDGender(cbQuietFemale.LastSafeIndex, chbQuietFemale.IsChecked)
            o.RashMale = Overlay13.SetPokemonIDGender(cbRashMale.LastSafeIndex, chbRashMale.IsChecked)
            o.RashFemale = Overlay13.SetPokemonIDGender(cbRashFemale.LastSafeIndex, chbRashFemale.IsChecked)
            o.BoldMale = Overlay13.SetPokemonIDGender(cbBoldMale.LastSafeIndex, chbBoldMale.IsChecked)
            o.BoldFemale = Overlay13.SetPokemonIDGender(cbBoldFemale.LastSafeIndex, chbBoldFemale.IsChecked)

            'Dim lang = (DirectCast(Save, SkyNDSRom)).GetLanguageString.Result
            'Dim offset As Integer = 1660

            'lang(offset + 0) = txtHardyDe.Text
            'lang(offset + 1) = txtHardyMP.Text
            'lang(offset + 2) = txtHardyFP.Text
            'lang(offset + 3) = txtDocileDe.Text
            'lang(offset + 4) = txtDocileMP.Text
            'lang(offset + 5) = txtDocileFP.Text
            'lang(offset + 6) = txtBraveDe.Text
            'lang(offset + 7) = txtBraveMP.Text
            'lang(offset + 8) = txtBraveFP.Text
            'lang(offset + 9) = txtJollyDe.Text
            'lang(offset + 10) = txtJollyMP.Text
            'lang(offset + 11) = txtJollyFP.Text
            'lang(offset + 12) = txtImpishDe.Text
            'lang(offset + 13) = txtImpishMP.Text
            'lang(offset + 14) = txtImpishFP.Text
            'lang(offset + 15) = txtNaiveDe.Text
            'lang(offset + 16) = txtNaiveMP.Text
            'lang(offset + 17) = txtNaiveFP.Text
            'lang(offset + 18) = txtTimidDe.Text
            'lang(offset + 19) = txtTimidMP.Text
            'lang(offset + 20) = txtTimidFP.Text
            'lang(offset + 21) = txtHastyDe.Text
            'lang(offset + 22) = txtHastyMP.Text
            'lang(offset + 23) = txtHastyFP.Text
            'lang(offset + 24) = txtSassyDe.Text
            'lang(offset + 25) = txtSassyMP.Text
            'lang(offset + 26) = txtSassyFP.Text
            'lang(offset + 27) = txtCalmDe.Text
            'lang(offset + 28) = txtCalmMP.Text
            'lang(offset + 29) = txtCalmFP.Text
            'lang(offset + 30) = txtRelaxedDe.Text
            'lang(offset + 31) = txtRelaxedMP.Text
            'lang(offset + 32) = txtRelaxedFP.Text
            'lang(offset + 33) = txtLonelyDe.Text
            'lang(offset + 34) = txtLonelyMP.Text
            'lang(offset + 35) = txtLonelyFP.Text
            'lang(offset + 36) = txtQuirkyDe.Text
            'lang(offset + 37) = txtQuirkyMP.Text
            'lang(offset + 38) = txtQuirkyFP.Text
            'lang(offset + 39) = txtQuietDe.Text
            'lang(offset + 40) = txtQuietMP.Text
            'lang(offset + 41) = txtQuietFP.Text
            'lang(offset + 42) = txtRashDe.Text
            'lang(offset + 43) = txtRashMP.Text
            'lang(offset + 44) = txtRashFP.Text
            'lang(offset + 45) = txtBoldDe.Text
            'lang(offset + 46) = txtBoldMP.Text
            'lang(offset + 47) = txtBoldFP.Text
            'lang.Save()

            'o.Save()
            'EditingItem = o
        End Sub

        Private Sub PersonalityTest_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded
            ReportModified = False
            'Me.Header = "Personality Test"
            PluginHelper.TranslateForm(Me)
            For Each count In SaveEditor.Lists.SkyPokemon.Keys
                Dim item As String = SaveEditor.Lists.SkyPokemon(count)
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
            ReportModified = True
        End Sub

        Private Sub OnModified(sender As Object, e As EventArgs) Handles chbPartner01.Checked,
                                                                         chbPartner02.Checked,
                                                                         chbPartner03.Checked,
                                                                         chbPartner04.Checked,
                                                                         chbPartner05.Checked,
                                                                         chbPartner06.Checked,
                                                                         chbPartner07.Checked,
                                                                         chbPartner08.Checked,
                                                                         chbPartner09.Checked,
                                                                         chbPartner10.Checked,
                                                                         chbPartner11.Checked,
                                                                         chbPartner12.Checked,
                                                                         chbPartner13.Checked,
                                                                         chbPartner14.Checked,
                                                                         chbPartner15.Checked,
                                                                         chbPartner16.Checked,
                                                                         chbPartner17.Checked,
                                                                         chbPartner18.Checked,
                                                                         chbPartner19.Checked,
                                                                         chbPartner20.Checked,
                                                                         chbPartner21.Checked,
                                                                         chbPartner01.Unchecked,
                                                                         chbPartner02.Unchecked,
                                                                         chbPartner03.Unchecked,
                                                                         chbPartner04.Unchecked,
                                                                         chbPartner05.Unchecked,
                                                                         chbPartner06.Unchecked,
                                                                         chbPartner07.Unchecked,
                                                                         chbPartner08.Unchecked,
                                                                         chbPartner09.Unchecked,
                                                                         chbPartner10.Unchecked,
                                                                         chbPartner11.Unchecked,
                                                                         chbPartner12.Unchecked,
                                                                         chbPartner13.Unchecked,
                                                                         chbPartner14.Unchecked,
                                                                         chbPartner15.Unchecked,
                                                                         chbPartner16.Unchecked,
                                                                         chbPartner17.Unchecked,
                                                                         chbPartner18.Unchecked,
                                                                         chbPartner19.Unchecked,
                                                                         chbPartner20.Unchecked,
                                                                         chbPartner21.Unchecked,
                                                                         cbPartner01.SelectionChanged,
                                                                         cbPartner02.SelectionChanged,
                                                                         cbPartner03.SelectionChanged,
                                                                         cbPartner04.SelectionChanged,
                                                                         cbPartner05.SelectionChanged,
                                                                         cbPartner06.SelectionChanged,
                                                                         cbPartner07.SelectionChanged,
                                                                         cbPartner08.SelectionChanged,
                                                                         cbPartner09.SelectionChanged,
                                                                         cbPartner10.SelectionChanged,
                                                                         cbPartner11.SelectionChanged,
                                                                         cbPartner12.SelectionChanged,
                                                                         cbPartner13.SelectionChanged,
                                                                         cbPartner14.SelectionChanged,
                                                                         cbPartner15.SelectionChanged,
                                                                         cbPartner16.SelectionChanged,
                                                                         cbPartner17.SelectionChanged,
                                                                         cbPartner18.SelectionChanged,
                                                                         cbPartner19.SelectionChanged,
                                                                         cbPartner20.SelectionChanged,
                                                                         cbPartner21.SelectionChanged,
                                                                         chbHardyMale.Checked,
                                                                         chbHardyFemale.Checked,
                                                                         chbDocileMale.Checked,
                                                                         chbDocileFemale.Checked,
                                                                         chbBraveMale.Checked,
                                                                         chbBraveFemale.Checked,
                                                                         chbJollyMale.Checked,
                                                                         chbJollyFemale.Checked,
                                                                         chbImpishMale.Checked,
                                                                         chbImpishFemale.Checked,
                                                                         chbNaiveMale.Checked,
                                                                         chbNaiveFemale.Checked,
                                                                         chbTimidMale.Checked,
                                                                         chbTimidFemale.Checked,
                                                                         chbHastyMale.Checked,
                                                                         chbHastyFemale.Checked,
                                                                         chbSassyMale.Checked,
                                                                         chbSassyFemale.Checked,
                                                                         chbCalmMale.Checked,
                                                                         chbCalmFemale.Checked,
                                                                         chbRelaxedMale.Checked,
                                                                         chbRelaxedFemale.Checked,
                                                                         chbLonelyMale.Checked,
                                                                         chbLonelyFemale.Checked,
                                                                         chbQuirkyMale.Checked,
                                                                         chbQuirkyFemale.Checked,
                                                                         chbQuietMale.Checked,
                                                                         chbQuietFemale.Checked,
                                                                         chbRashMale.Checked,
                                                                         chbRashFemale.Checked,
                                                                         chbBoldMale.Checked,
                                                                         chbBoldFemale.Checked,
                                                                         chbHardyMale.Unchecked,
                                                                         chbHardyFemale.Unchecked,
                                                                         chbDocileMale.Unchecked,
                                                                         chbDocileFemale.Unchecked,
                                                                         chbBraveMale.Unchecked,
                                                                         chbBraveFemale.Unchecked,
                                                                         chbJollyMale.Unchecked,
                                                                         chbJollyFemale.Unchecked,
                                                                         chbImpishMale.Unchecked,
                                                                         chbImpishFemale.Unchecked,
                                                                         chbNaiveMale.Unchecked,
                                                                         chbNaiveFemale.Unchecked,
                                                                         chbTimidMale.Unchecked,
                                                                         chbTimidFemale.Unchecked,
                                                                         chbHastyMale.Unchecked,
                                                                         chbHastyFemale.Unchecked,
                                                                         chbSassyMale.Unchecked,
                                                                         chbSassyFemale.Unchecked,
                                                                         chbCalmMale.Unchecked,
                                                                         chbCalmFemale.Unchecked,
                                                                         chbRelaxedMale.Unchecked,
                                                                         chbRelaxedFemale.Unchecked,
                                                                         chbLonelyMale.Unchecked,
                                                                         chbLonelyFemale.Unchecked,
                                                                         chbQuirkyMale.Unchecked,
                                                                         chbQuirkyFemale.Unchecked,
                                                                         chbQuietMale.Unchecked,
                                                                         chbQuietFemale.Unchecked,
                                                                         chbRashMale.Unchecked,
                                                                         chbRashFemale.Unchecked,
                                                                         chbBoldMale.Unchecked,
                                                                         chbBoldFemale.Unchecked,
                                                                         cbHardyMale.SelectionChanged,
                                                                         cbHardyFemale.SelectionChanged,
                                                                         cbDocileMale.SelectionChanged,
                                                                         cbDocileFemale.SelectionChanged,
                                                                         cbBraveMale.SelectionChanged,
                                                                         cbBraveFemale.SelectionChanged,
                                                                         cbJollyMale.SelectionChanged,
                                                                         cbJollyFemale.SelectionChanged,
                                                                         cbImpishMale.SelectionChanged,
                                                                         cbImpishFemale.SelectionChanged,
                                                                         cbNaiveMale.SelectionChanged,
                                                                         cbNaiveFemale.SelectionChanged,
                                                                         cbTimidMale.SelectionChanged,
                                                                         cbTimidFemale.SelectionChanged,
                                                                         cbHastyMale.SelectionChanged,
                                                                         cbHastyFemale.SelectionChanged,
                                                                         cbSassyMale.SelectionChanged,
                                                                         cbSassyFemale.SelectionChanged,
                                                                         cbCalmMale.SelectionChanged,
                                                                         cbCalmFemale.SelectionChanged,
                                                                         cbRelaxedMale.SelectionChanged,
                                                                         cbRelaxedFemale.SelectionChanged,
                                                                         cbLonelyMale.SelectionChanged,
                                                                         cbLonelyFemale.SelectionChanged,
                                                                         cbQuirkyMale.SelectionChanged,
                                                                         cbQuirkyFemale.SelectionChanged,
                                                                         cbQuietMale.SelectionChanged,
                                                                         cbQuietFemale.SelectionChanged,
                                                                         cbRashMale.SelectionChanged,
                                                                         cbRashFemale.SelectionChanged,
                                                                         cbBoldMale.SelectionChanged,
                                                                         cbBoldFemale.SelectionChanged
            If ReportModified Then RaiseModified()
        End Sub
    End Class
End Namespace
