Imports System.ComponentModel

Namespace FileFormats.Explorers
    ''' <summary>
    ''' Stores the starter data from Overlay13.
    ''' </summary>
    Public Class PersonalityTestContainer
        Implements INotifyPropertyChanged

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Public Sub New()

        End Sub

        Public Sub New(Overlay As Overlay13)
            If Overlay.Partner01 > 600 Then
                Me.Partner01Pokemon = Overlay.Partner01 - 600
                Me.Partner01IsFemale = True
            Else
                Me.Partner01Pokemon = Overlay.Partner01
                Me.Partner01IsFemale = False
            End If

            If Overlay.Partner02 > 600 Then
                Me.Partner02Pokemon = Overlay.Partner02 - 600
                Me.Partner02IsFemale = True
            Else
                Me.Partner02Pokemon = Overlay.Partner02
                Me.Partner02IsFemale = False
            End If

            If Overlay.Partner03 > 600 Then
                Me.Partner03Pokemon = Overlay.Partner03 - 600
                Me.Partner03IsFemale = True
            Else
                Me.Partner03Pokemon = Overlay.Partner03
                Me.Partner03IsFemale = False
            End If

            If Overlay.Partner04 > 600 Then
                Me.Partner04Pokemon = Overlay.Partner04 - 600
                Me.Partner04IsFemale = True
            Else
                Me.Partner04Pokemon = Overlay.Partner04
                Me.Partner04IsFemale = False
            End If

            If Overlay.Partner05 > 600 Then
                Me.Partner05Pokemon = Overlay.Partner05 - 600
                Me.Partner05IsFemale = True
            Else
                Me.Partner05Pokemon = Overlay.Partner05
                Me.Partner05IsFemale = False
            End If

            If Overlay.Partner06 > 600 Then
                Me.Partner06Pokemon = Overlay.Partner06 - 600
                Me.Partner06IsFemale = True
            Else
                Me.Partner06Pokemon = Overlay.Partner06
                Me.Partner06IsFemale = False
            End If

            If Overlay.Partner07 > 600 Then
                Me.Partner07Pokemon = Overlay.Partner07 - 600
                Me.Partner07IsFemale = True
            Else
                Me.Partner07Pokemon = Overlay.Partner07
                Me.Partner07IsFemale = False
            End If

            If Overlay.Partner08 > 600 Then
                Me.Partner08Pokemon = Overlay.Partner08 - 600
                Me.Partner08IsFemale = True
            Else
                Me.Partner08Pokemon = Overlay.Partner08
                Me.Partner08IsFemale = False
            End If

            If Overlay.Partner09 > 600 Then
                Me.Partner09Pokemon = Overlay.Partner09 - 600
                Me.Partner09IsFemale = True
            Else
                Me.Partner09Pokemon = Overlay.Partner09
                Me.Partner09IsFemale = False
            End If

            If Overlay.Partner10 > 600 Then
                Me.Partner10Pokemon = Overlay.Partner10 - 600
                Me.Partner10IsFemale = True
            Else
                Me.Partner10Pokemon = Overlay.Partner10
                Me.Partner10IsFemale = False
            End If

            If Overlay.Partner11 > 600 Then
                Me.Partner11Pokemon = Overlay.Partner11 - 600
                Me.Partner11IsFemale = True
            Else
                Me.Partner11Pokemon = Overlay.Partner11
                Me.Partner11IsFemale = False
            End If

            If Overlay.Partner12 > 600 Then
                Me.Partner12Pokemon = Overlay.Partner12 - 600
                Me.Partner12IsFemale = True
            Else
                Me.Partner12Pokemon = Overlay.Partner12
                Me.Partner12IsFemale = False
            End If

            If Overlay.Partner13 > 600 Then
                Me.Partner13Pokemon = Overlay.Partner13 - 600
                Me.Partner13IsFemale = True
            Else
                Me.Partner13Pokemon = Overlay.Partner13
                Me.Partner13IsFemale = False
            End If

            If Overlay.Partner14 > 600 Then
                Me.Partner14Pokemon = Overlay.Partner14 - 600
                Me.Partner14IsFemale = True
            Else
                Me.Partner14Pokemon = Overlay.Partner14
                Me.Partner14IsFemale = False
            End If

            If Overlay.Partner15 > 600 Then
                Me.Partner15Pokemon = Overlay.Partner15 - 600
                Me.Partner15IsFemale = True
            Else
                Me.Partner15Pokemon = Overlay.Partner15
                Me.Partner15IsFemale = False
            End If

            If Overlay.Partner16 > 600 Then
                Me.Partner16Pokemon = Overlay.Partner16 - 600
                Me.Partner16IsFemale = True
            Else
                Me.Partner16Pokemon = Overlay.Partner16
                Me.Partner16IsFemale = False
            End If

            If Overlay.Partner17 > 600 Then
                Me.Partner17Pokemon = Overlay.Partner17 - 600
                Me.Partner17IsFemale = True
            Else
                Me.Partner17Pokemon = Overlay.Partner17
                Me.Partner17IsFemale = False
            End If

            If Overlay.Partner18 > 600 Then
                Me.Partner18Pokemon = Overlay.Partner18 - 600
                Me.Partner18IsFemale = True
            Else
                Me.Partner18Pokemon = Overlay.Partner18
                Me.Partner18IsFemale = False
            End If

            If Overlay.Partner19 > 600 Then
                Me.Partner19Pokemon = Overlay.Partner19 - 600
                Me.Partner19IsFemale = True
            Else
                Me.Partner19Pokemon = Overlay.Partner19
                Me.Partner19IsFemale = False
            End If

            If Overlay.Partner20 > 600 Then
                Me.Partner20Pokemon = Overlay.Partner20 - 600
                Me.Partner20IsFemale = True
            Else
                Me.Partner20Pokemon = Overlay.Partner20
                Me.Partner20IsFemale = False
            End If

            If Overlay.Partner21 > 600 Then
                Me.Partner21Pokemon = Overlay.Partner21 - 600
                Me.Partner21IsFemale = True
            Else
                Me.Partner21Pokemon = Overlay.Partner21
                Me.Partner21IsFemale = False
            End If

            If Overlay.HardyMale > 600 Then
                Me.HardyMalePokemon = Overlay.HardyMale - 600
                Me.HardyMaleIsFemale = True
            Else
                Me.HardyMalePokemon = Overlay.HardyMale
                Me.HardyMaleIsFemale = False
            End If

            If Overlay.HardyFemale > 600 Then
                Me.HardyFemalePokemon = Overlay.HardyFemale - 600
                Me.HardyFemaleIsFemale = True
            Else
                Me.HardyFemalePokemon = Overlay.HardyFemale
                Me.HardyFemaleIsFemale = False
            End If

            If Overlay.DocileMale > 600 Then
                Me.DocileMalePokemon = Overlay.DocileMale - 600
                Me.DocileMaleIsFemale = True
            Else
                Me.DocileMalePokemon = Overlay.DocileMale
                Me.DocileMaleIsFemale = False
            End If

            If Overlay.DocileFemale > 600 Then
                Me.DocileFemalePokemon = Overlay.DocileFemale - 600
                Me.DocileFemaleIsFemale = True
            Else
                Me.DocileFemalePokemon = Overlay.DocileFemale
                Me.DocileFemaleIsFemale = False
            End If

            If Overlay.BraveMale > 600 Then
                Me.BraveMalePokemon = Overlay.BraveMale - 600
                Me.BraveMaleIsFemale = True
            Else
                Me.BraveMalePokemon = Overlay.BraveMale
                Me.BraveMaleIsFemale = False
            End If

            If Overlay.BraveFemale > 600 Then
                Me.BraveFemalePokemon = Overlay.BraveFemale - 600
                Me.BraveFemaleIsFemale = True
            Else
                Me.BraveFemalePokemon = Overlay.BraveFemale
                Me.BraveFemaleIsFemale = False
            End If

            If Overlay.JollyMale > 600 Then
                Me.JollyMalePokemon = Overlay.JollyMale - 600
                Me.JollyMaleIsFemale = True
            Else
                Me.JollyMalePokemon = Overlay.JollyMale
                Me.JollyMaleIsFemale = False
            End If

            If Overlay.JollyFemale > 600 Then
                Me.JollyFemalePokemon = Overlay.JollyFemale - 600
                Me.JollyFemaleIsFemale = True
            Else
                Me.JollyFemalePokemon = Overlay.JollyFemale
                Me.JollyFemaleIsFemale = False
            End If

            If Overlay.ImpishMale > 600 Then
                Me.ImpishMalePokemon = Overlay.ImpishMale - 600
                Me.ImpishMaleIsFemale = True
            Else
                Me.ImpishMalePokemon = Overlay.ImpishMale
                Me.ImpishMaleIsFemale = False
            End If

            If Overlay.ImpishFemale > 600 Then
                Me.ImpishFemalePokemon = Overlay.ImpishFemale - 600
                Me.ImpishFemaleIsFemale = True
            Else
                Me.ImpishFemalePokemon = Overlay.ImpishFemale
                Me.ImpishFemaleIsFemale = False
            End If

            If Overlay.NaiveMale > 600 Then
                Me.NaiveMalePokemon = Overlay.NaiveMale - 600
                Me.NaiveMaleIsFemale = True
            Else
                Me.NaiveMalePokemon = Overlay.NaiveMale
                Me.NaiveMaleIsFemale = False
            End If

            If Overlay.NaiveFemale > 600 Then
                Me.NaiveFemalePokemon = Overlay.NaiveFemale - 600
                Me.NaiveFemaleIsFemale = True
            Else
                Me.NaiveFemalePokemon = Overlay.NaiveFemale
                Me.NaiveFemaleIsFemale = False
            End If

            If Overlay.TimidMale > 600 Then
                Me.TimidMalePokemon = Overlay.TimidMale - 600
                Me.TimidMaleIsFemale = True
            Else
                Me.TimidMalePokemon = Overlay.TimidMale
                Me.TimidMaleIsFemale = False
            End If

            If Overlay.TimidFemale > 600 Then
                Me.TimidFemalePokemon = Overlay.TimidFemale - 600
                Me.TimidFemaleIsFemale = True
            Else
                Me.TimidFemalePokemon = Overlay.TimidFemale
                Me.TimidFemaleIsFemale = False
            End If

            If Overlay.HastyMale > 600 Then
                Me.HastyMalePokemon = Overlay.HastyMale - 600
                Me.HastyMaleIsFemale = True
            Else
                Me.HastyMalePokemon = Overlay.HastyMale
                Me.HastyMaleIsFemale = False
            End If

            If Overlay.HastyFemale > 600 Then
                Me.HastyFemalePokemon = Overlay.HastyFemale - 600
                Me.HastyFemaleIsFemale = True
            Else
                Me.HastyFemalePokemon = Overlay.HastyFemale
                Me.HastyFemaleIsFemale = False
            End If

            If Overlay.SassyMale > 600 Then
                Me.SassyMalePokemon = Overlay.SassyMale - 600
                Me.SassyMaleIsFemale = True
            Else
                Me.SassyMalePokemon = Overlay.SassyMale
                Me.SassyMaleIsFemale = False
            End If

            If Overlay.SassyFemale > 600 Then
                Me.SassyFemalePokemon = Overlay.SassyFemale - 600
                Me.SassyFemaleIsFemale = True
            Else
                Me.SassyFemalePokemon = Overlay.SassyFemale
                Me.SassyFemaleIsFemale = False
            End If

            If Overlay.CalmMale > 600 Then
                Me.CalmMalePokemon = Overlay.CalmMale - 600
                Me.CalmMaleIsFemale = True
            Else
                Me.CalmMalePokemon = Overlay.CalmMale
                Me.CalmMaleIsFemale = False
            End If

            If Overlay.CalmFemale > 600 Then
                Me.CalmFemalePokemon = Overlay.CalmFemale - 600
                Me.CalmFemaleIsFemale = True
            Else
                Me.CalmFemalePokemon = Overlay.CalmFemale
                Me.CalmFemaleIsFemale = False
            End If

            If Overlay.RelaxedMale > 600 Then
                Me.RelaxedMalePokemon = Overlay.RelaxedMale - 600
                Me.RelaxedMaleIsFemale = True
            Else
                Me.RelaxedMalePokemon = Overlay.RelaxedMale
                Me.RelaxedMaleIsFemale = False
            End If

            If Overlay.RelaxedFemale > 600 Then
                Me.RelaxedFemalePokemon = Overlay.RelaxedFemale - 600
                Me.RelaxedFemaleIsFemale = True
            Else
                Me.RelaxedFemalePokemon = Overlay.RelaxedFemale
                Me.RelaxedFemaleIsFemale = False
            End If

            If Overlay.LonelyMale > 600 Then
                Me.LonelyMalePokemon = Overlay.LonelyMale - 600
                Me.LonelyMaleIsFemale = True
            Else
                Me.LonelyMalePokemon = Overlay.LonelyMale
                Me.LonelyMaleIsFemale = False
            End If

            If Overlay.LonelyFemale > 600 Then
                Me.LonelyFemalePokemon = Overlay.LonelyFemale - 600
                Me.LonelyFemaleIsFemale = True
            Else
                Me.LonelyFemalePokemon = Overlay.LonelyFemale
                Me.LonelyFemaleIsFemale = False
            End If

            If Overlay.QuirkyMale > 600 Then
                Me.QuirkyMalePokemon = Overlay.QuirkyMale - 600
                Me.QuirkyMaleIsFemale = True
            Else
                Me.QuirkyMalePokemon = Overlay.QuirkyMale
                Me.QuirkyMaleIsFemale = False
            End If

            If Overlay.QuirkyFemale > 600 Then
                Me.QuirkyFemalePokemon = Overlay.QuirkyFemale - 600
                Me.QuirkyFemaleIsFemale = True
            Else
                Me.QuirkyFemalePokemon = Overlay.QuirkyFemale
                Me.QuirkyFemaleIsFemale = False
            End If

            If Overlay.QuietMale > 600 Then
                Me.QuietMalePokemon = Overlay.QuietMale - 600
                Me.QuietMaleIsFemale = True
            Else
                Me.QuietMalePokemon = Overlay.QuietMale
                Me.QuietMaleIsFemale = False
            End If

            If Overlay.QuietFemale > 600 Then
                Me.QuietFemalePokemon = Overlay.QuietFemale - 600
                Me.QuietFemaleIsFemale = True
            Else
                Me.QuietFemalePokemon = Overlay.QuietFemale
                Me.QuietFemaleIsFemale = False
            End If

            If Overlay.RashMale > 600 Then
                Me.RashMalePokemon = Overlay.RashMale - 600
                Me.RashMaleIsFemale = True
            Else
                Me.RashMalePokemon = Overlay.RashMale
                Me.RashMaleIsFemale = False
            End If

            If Overlay.RashFemale > 600 Then
                Me.RashFemalePokemon = Overlay.RashFemale - 600
                Me.RashFemaleIsFemale = True
            Else
                Me.RashFemalePokemon = Overlay.RashFemale
                Me.RashFemaleIsFemale = False
            End If

            If Overlay.BoldMale > 600 Then
                Me.BoldMalePokemon = Overlay.BoldMale - 600
                Me.BoldMaleIsFemale = True
            Else
                Me.BoldMalePokemon = Overlay.BoldMale
                Me.BoldMaleIsFemale = False
            End If

            If Overlay.BoldFemale > 600 Then
                Me.BoldFemalePokemon = Overlay.BoldFemale - 600
                Me.BoldFemaleIsFemale = True
            Else
                Me.BoldFemalePokemon = Overlay.BoldFemale
                Me.BoldFemaleIsFemale = False
            End If


        End Sub

        Public Sub UpdateOverlay(Overlay As Overlay13)
            If Me.Partner01IsFemale Then
                Overlay.Partner01 = Me.Partner01Pokemon + 600
            Else
                Overlay.Partner01 = Me.Partner01Pokemon
            End If
            If Me.Partner02IsFemale Then
                Overlay.Partner02 = Me.Partner02Pokemon + 600
            Else
                Overlay.Partner02 = Me.Partner02Pokemon
            End If
            If Me.Partner03IsFemale Then
                Overlay.Partner03 = Me.Partner03Pokemon + 600
            Else
                Overlay.Partner03 = Me.Partner03Pokemon
            End If
            If Me.Partner04IsFemale Then
                Overlay.Partner04 = Me.Partner04Pokemon + 600
            Else
                Overlay.Partner04 = Me.Partner04Pokemon
            End If
            If Me.Partner05IsFemale Then
                Overlay.Partner05 = Me.Partner05Pokemon + 600
            Else
                Overlay.Partner05 = Me.Partner05Pokemon
            End If
            If Me.Partner06IsFemale Then
                Overlay.Partner06 = Me.Partner06Pokemon + 600
            Else
                Overlay.Partner06 = Me.Partner06Pokemon
            End If
            If Me.Partner07IsFemale Then
                Overlay.Partner07 = Me.Partner07Pokemon + 600
            Else
                Overlay.Partner07 = Me.Partner07Pokemon
            End If
            If Me.Partner08IsFemale Then
                Overlay.Partner08 = Me.Partner08Pokemon + 600
            Else
                Overlay.Partner08 = Me.Partner08Pokemon
            End If
            If Me.Partner09IsFemale Then
                Overlay.Partner09 = Me.Partner09Pokemon + 600
            Else
                Overlay.Partner09 = Me.Partner09Pokemon
            End If
            If Me.Partner10IsFemale Then
                Overlay.Partner10 = Me.Partner10Pokemon + 600
            Else
                Overlay.Partner10 = Me.Partner10Pokemon
            End If
            If Me.Partner11IsFemale Then
                Overlay.Partner11 = Me.Partner11Pokemon + 600
            Else
                Overlay.Partner11 = Me.Partner11Pokemon
            End If
            If Me.Partner12IsFemale Then
                Overlay.Partner12 = Me.Partner12Pokemon + 600
            Else
                Overlay.Partner12 = Me.Partner12Pokemon
            End If
            If Me.Partner13IsFemale Then
                Overlay.Partner13 = Me.Partner13Pokemon + 600
            Else
                Overlay.Partner13 = Me.Partner13Pokemon
            End If
            If Me.Partner14IsFemale Then
                Overlay.Partner14 = Me.Partner14Pokemon + 600
            Else
                Overlay.Partner14 = Me.Partner14Pokemon
            End If
            If Me.Partner15IsFemale Then
                Overlay.Partner15 = Me.Partner15Pokemon + 600
            Else
                Overlay.Partner15 = Me.Partner15Pokemon
            End If
            If Me.Partner16IsFemale Then
                Overlay.Partner16 = Me.Partner16Pokemon + 600
            Else
                Overlay.Partner16 = Me.Partner16Pokemon
            End If
            If Me.Partner17IsFemale Then
                Overlay.Partner17 = Me.Partner17Pokemon + 600
            Else
                Overlay.Partner17 = Me.Partner17Pokemon
            End If
            If Me.Partner18IsFemale Then
                Overlay.Partner18 = Me.Partner18Pokemon + 600
            Else
                Overlay.Partner18 = Me.Partner18Pokemon
            End If
            If Me.Partner19IsFemale Then
                Overlay.Partner19 = Me.Partner19Pokemon + 600
            Else
                Overlay.Partner19 = Me.Partner19Pokemon
            End If
            If Me.Partner20IsFemale Then
                Overlay.Partner20 = Me.Partner20Pokemon + 600
            Else
                Overlay.Partner20 = Me.Partner20Pokemon
            End If
            If Me.Partner21IsFemale Then
                Overlay.Partner21 = Me.Partner21Pokemon + 600
            Else
                Overlay.Partner21 = Me.Partner21Pokemon
            End If
            If Me.HardyMaleIsFemale Then
                Overlay.HardyMale = Me.HardyMalePokemon + 600
            Else
                Overlay.HardyMale = Me.HardyMalePokemon
            End If
            If Me.HardyFemaleIsFemale Then
                Overlay.HardyFemale = Me.HardyFemalePokemon + 600
            Else
                Overlay.HardyFemale = Me.HardyFemalePokemon
            End If
            If Me.DocileMaleIsFemale Then
                Overlay.DocileMale = Me.DocileMalePokemon + 600
            Else
                Overlay.DocileMale = Me.DocileMalePokemon
            End If
            If Me.DocileFemaleIsFemale Then
                Overlay.DocileFemale = Me.DocileFemalePokemon + 600
            Else
                Overlay.DocileFemale = Me.DocileFemalePokemon
            End If
            If Me.BraveMaleIsFemale Then
                Overlay.BraveMale = Me.BraveMalePokemon + 600
            Else
                Overlay.BraveMale = Me.BraveMalePokemon
            End If
            If Me.BraveFemaleIsFemale Then
                Overlay.BraveFemale = Me.BraveFemalePokemon + 600
            Else
                Overlay.BraveFemale = Me.BraveFemalePokemon
            End If
            If Me.JollyMaleIsFemale Then
                Overlay.JollyMale = Me.JollyMalePokemon + 600
            Else
                Overlay.JollyMale = Me.JollyMalePokemon
            End If
            If Me.JollyFemaleIsFemale Then
                Overlay.JollyFemale = Me.JollyFemalePokemon + 600
            Else
                Overlay.JollyFemale = Me.JollyFemalePokemon
            End If
            If Me.ImpishMaleIsFemale Then
                Overlay.ImpishMale = Me.ImpishMalePokemon + 600
            Else
                Overlay.ImpishMale = Me.ImpishMalePokemon
            End If
            If Me.ImpishFemaleIsFemale Then
                Overlay.ImpishFemale = Me.ImpishFemalePokemon + 600
            Else
                Overlay.ImpishFemale = Me.ImpishFemalePokemon
            End If
            If Me.NaiveMaleIsFemale Then
                Overlay.NaiveMale = Me.NaiveMalePokemon + 600
            Else
                Overlay.NaiveMale = Me.NaiveMalePokemon
            End If
            If Me.NaiveFemaleIsFemale Then
                Overlay.NaiveFemale = Me.NaiveFemalePokemon + 600
            Else
                Overlay.NaiveFemale = Me.NaiveFemalePokemon
            End If
            If Me.TimidMaleIsFemale Then
                Overlay.TimidMale = Me.TimidMalePokemon + 600
            Else
                Overlay.TimidMale = Me.TimidMalePokemon
            End If
            If Me.TimidFemaleIsFemale Then
                Overlay.TimidFemale = Me.TimidFemalePokemon + 600
            Else
                Overlay.TimidFemale = Me.TimidFemalePokemon
            End If
            If Me.HastyMaleIsFemale Then
                Overlay.HastyMale = Me.HastyMalePokemon + 600
            Else
                Overlay.HastyMale = Me.HastyMalePokemon
            End If
            If Me.HastyFemaleIsFemale Then
                Overlay.HastyFemale = Me.HastyFemalePokemon + 600
            Else
                Overlay.HastyFemale = Me.HastyFemalePokemon
            End If
            If Me.SassyMaleIsFemale Then
                Overlay.SassyMale = Me.SassyMalePokemon + 600
            Else
                Overlay.SassyMale = Me.SassyMalePokemon
            End If
            If Me.SassyFemaleIsFemale Then
                Overlay.SassyFemale = Me.SassyFemalePokemon + 600
            Else
                Overlay.SassyFemale = Me.SassyFemalePokemon
            End If
            If Me.CalmMaleIsFemale Then
                Overlay.CalmMale = Me.CalmMalePokemon + 600
            Else
                Overlay.CalmMale = Me.CalmMalePokemon
            End If
            If Me.CalmFemaleIsFemale Then
                Overlay.CalmFemale = Me.CalmFemalePokemon + 600
            Else
                Overlay.CalmFemale = Me.CalmFemalePokemon
            End If
            If Me.RelaxedMaleIsFemale Then
                Overlay.RelaxedMale = Me.RelaxedMalePokemon + 600
            Else
                Overlay.RelaxedMale = Me.RelaxedMalePokemon
            End If
            If Me.RelaxedFemaleIsFemale Then
                Overlay.RelaxedFemale = Me.RelaxedFemalePokemon + 600
            Else
                Overlay.RelaxedFemale = Me.RelaxedFemalePokemon
            End If
            If Me.LonelyMaleIsFemale Then
                Overlay.LonelyMale = Me.LonelyMalePokemon + 600
            Else
                Overlay.LonelyMale = Me.LonelyMalePokemon
            End If
            If Me.LonelyFemaleIsFemale Then
                Overlay.LonelyFemale = Me.LonelyFemalePokemon + 600
            Else
                Overlay.LonelyFemale = Me.LonelyFemalePokemon
            End If
            If Me.QuirkyMaleIsFemale Then
                Overlay.QuirkyMale = Me.QuirkyMalePokemon + 600
            Else
                Overlay.QuirkyMale = Me.QuirkyMalePokemon
            End If
            If Me.QuirkyFemaleIsFemale Then
                Overlay.QuirkyFemale = Me.QuirkyFemalePokemon + 600
            Else
                Overlay.QuirkyFemale = Me.QuirkyFemalePokemon
            End If
            If Me.QuietMaleIsFemale Then
                Overlay.QuietMale = Me.QuietMalePokemon + 600
            Else
                Overlay.QuietMale = Me.QuietMalePokemon
            End If
            If Me.QuietFemaleIsFemale Then
                Overlay.QuietFemale = Me.QuietFemalePokemon + 600
            Else
                Overlay.QuietFemale = Me.QuietFemalePokemon
            End If
            If Me.RashMaleIsFemale Then
                Overlay.RashMale = Me.RashMalePokemon + 600
            Else
                Overlay.RashMale = Me.RashMalePokemon
            End If
            If Me.RashFemaleIsFemale Then
                Overlay.RashFemale = Me.RashFemalePokemon + 600
            Else
                Overlay.RashFemale = Me.RashFemalePokemon
            End If
            If Me.BoldMaleIsFemale Then
                Overlay.BoldMale = Me.BoldMalePokemon + 600
            Else
                Overlay.BoldMale = Me.BoldMalePokemon
            End If
            If Me.BoldFemaleIsFemale Then
                Overlay.BoldFemale = Me.BoldFemalePokemon + 600
            Else
                Overlay.BoldFemale = Me.BoldFemalePokemon
            End If

        End Sub

#Region "Properties"


        Public Property Partner01Pokemon As UShort
            Get
                Return _Partner01Pokemon
            End Get
            Set(value As UShort)
                If Not _Partner01Pokemon = value Then
                    _Partner01Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner01Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner01Pokemon As UShort

        Public Property Partner01IsFemale As Boolean
            Get
                Return _Partner01IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner01IsFemale = value Then
                    _Partner01IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner01IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner01IsFemale As Boolean

        Public Property Partner02Pokemon As UShort
            Get
                Return _Partner02Pokemon
            End Get
            Set(value As UShort)
                If Not _Partner02Pokemon = value Then
                    _Partner02Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner02Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner02Pokemon As UShort

        Public Property Partner02IsFemale As Boolean
            Get
                Return _Partner02IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner02IsFemale = value Then
                    _Partner02IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner02IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner02IsFemale As Boolean

        Public Property Partner03Pokemon As UShort
            Get
                Return _Partner03Pokemon
            End Get
            Set(value As UShort)
                If Not _Partner03Pokemon = value Then
                    _Partner03Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner03Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner03Pokemon As UShort

        Public Property Partner03IsFemale As Boolean
            Get
                Return _Partner03IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner03IsFemale = value Then
                    _Partner03IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner03IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner03IsFemale As Boolean

        Public Property Partner04Pokemon As UShort
            Get
                Return _Partner04Pokemon
            End Get
            Set(value As UShort)
                If Not _Partner04Pokemon = value Then
                    _Partner04Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner04Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner04Pokemon As UShort

        Public Property Partner04IsFemale As Boolean
            Get
                Return _Partner04IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner04IsFemale = value Then
                    _Partner04IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner04IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner04IsFemale As Boolean

        Public Property Partner05Pokemon As UShort
            Get
                Return _Partner05Pokemon
            End Get
            Set(value As UShort)
                If Not _Partner05Pokemon = value Then
                    _Partner05Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner05Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner05Pokemon As UShort

        Public Property Partner05IsFemale As Boolean
            Get
                Return _Partner05IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner05IsFemale = value Then
                    _Partner05IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner05IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner05IsFemale As Boolean

        Public Property Partner06Pokemon As UShort
            Get
                Return _Partner06Pokemon
            End Get
            Set(value As UShort)
                If Not _Partner06Pokemon = value Then
                    _Partner06Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner06Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner06Pokemon As UShort

        Public Property Partner06IsFemale As Boolean
            Get
                Return _Partner06IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner06IsFemale = value Then
                    _Partner06IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner06IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner06IsFemale As Boolean

        Public Property Partner07Pokemon As UShort
            Get
                Return _Partner07Pokemon
            End Get
            Set(value As UShort)
                If Not _Partner07Pokemon = value Then
                    _Partner07Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner07Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner07Pokemon As UShort

        Public Property Partner07IsFemale As Boolean
            Get
                Return _Partner07IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner07IsFemale = value Then
                    _Partner07IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner07IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner07IsFemale As Boolean

        Public Property Partner08Pokemon As UShort
            Get
                Return _Partner08Pokemon
            End Get
            Set(value As UShort)
                If Not _Partner08Pokemon = value Then
                    _Partner08Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner08Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner08Pokemon As UShort

        Public Property Partner08IsFemale As Boolean
            Get
                Return _Partner08IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner08IsFemale = value Then
                    _Partner08IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner08IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner08IsFemale As Boolean

        Public Property Partner09Pokemon As UShort
            Get
                Return _Partner09Pokemon
            End Get
            Set(value As UShort)
                If Not _Partner09Pokemon = value Then
                    _Partner09Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner09Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner09Pokemon As UShort

        Public Property Partner09IsFemale As Boolean
            Get
                Return _Partner09IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner09IsFemale = value Then
                    _Partner09IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner09IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner09IsFemale As Boolean

        Public Property Partner10Pokemon As UShort
            Get
                Return _Partner10Pokemon
            End Get
            Set(value As UShort)
                If Not _Partner10Pokemon = value Then
                    _Partner10Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner10Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner10Pokemon As UShort

        Public Property Partner10IsFemale As Boolean
            Get
                Return _Partner10IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner10IsFemale = value Then
                    _Partner10IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner10IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner10IsFemale As Boolean

        Public Property Partner11Pokemon As UShort
            Get
                Return _Partner11Pokemon
            End Get
            Set(value As UShort)
                If Not _Partner11Pokemon = value Then
                    _Partner11Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner11Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner11Pokemon As UShort

        Public Property Partner11IsFemale As Boolean
            Get
                Return _Partner11IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner11IsFemale = value Then
                    _Partner11IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner11IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner11IsFemale As Boolean

        Public Property Partner12Pokemon As UShort
            Get
                Return _Partner12Pokemon
            End Get
            Set(value As UShort)
                If Not _Partner12Pokemon = value Then
                    _Partner12Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner12Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner12Pokemon As UShort

        Public Property Partner12IsFemale As Boolean
            Get
                Return _Partner12IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner12IsFemale = value Then
                    _Partner12IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner12IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner12IsFemale As Boolean

        Public Property Partner13Pokemon As UShort
            Get
                Return _Partner13Pokemon
            End Get
            Set(value As UShort)
                If Not _Partner13Pokemon = value Then
                    _Partner13Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner13Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner13Pokemon As UShort

        Public Property Partner13IsFemale As Boolean
            Get
                Return _Partner13IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner13IsFemale = value Then
                    _Partner13IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner13IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner13IsFemale As Boolean

        Public Property Partner14Pokemon As UShort
            Get
                Return _Partner14Pokemon
            End Get
            Set(value As UShort)
                If Not _Partner14Pokemon = value Then
                    _Partner14Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner14Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner14Pokemon As UShort

        Public Property Partner14IsFemale As Boolean
            Get
                Return _Partner14IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner14IsFemale = value Then
                    _Partner14IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner14IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner14IsFemale As Boolean

        Public Property Partner15Pokemon As UShort
            Get
                Return _Partner15Pokemon
            End Get
            Set(value As UShort)
                If Not _Partner15Pokemon = value Then
                    _Partner15Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner15Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner15Pokemon As UShort

        Public Property Partner15IsFemale As Boolean
            Get
                Return _Partner15IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner15IsFemale = value Then
                    _Partner15IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner15IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner15IsFemale As Boolean

        Public Property Partner16Pokemon As UShort
            Get
                Return _Partner16Pokemon
            End Get
            Set(value As UShort)
                If Not _Partner16Pokemon = value Then
                    _Partner16Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner16Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner16Pokemon As UShort

        Public Property Partner16IsFemale As Boolean
            Get
                Return _Partner16IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner16IsFemale = value Then
                    _Partner16IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner16IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner16IsFemale As Boolean

        Public Property Partner17Pokemon As UShort
            Get
                Return _Partner17Pokemon
            End Get
            Set(value As UShort)
                If Not _Partner17Pokemon = value Then
                    _Partner17Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner17Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner17Pokemon As UShort

        Public Property Partner17IsFemale As Boolean
            Get
                Return _Partner17IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner17IsFemale = value Then
                    _Partner17IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner17IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner17IsFemale As Boolean

        Public Property Partner18Pokemon As UShort
            Get
                Return _Partner18Pokemon
            End Get
            Set(value As UShort)
                If Not _Partner18Pokemon = value Then
                    _Partner18Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner18Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner18Pokemon As UShort

        Public Property Partner18IsFemale As Boolean
            Get
                Return _Partner18IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner18IsFemale = value Then
                    _Partner18IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner18IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner18IsFemale As Boolean

        Public Property Partner19Pokemon As UShort
            Get
                Return _Partner19Pokemon
            End Get
            Set(value As UShort)
                If Not _Partner19Pokemon = value Then
                    _Partner19Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner19Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner19Pokemon As UShort

        Public Property Partner19IsFemale As Boolean
            Get
                Return _Partner19IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner19IsFemale = value Then
                    _Partner19IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner19IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner19IsFemale As Boolean

        Public Property Partner20Pokemon As UShort
            Get
                Return _Partner20Pokemon
            End Get
            Set(value As UShort)
                If Not _Partner20Pokemon = value Then
                    _Partner20Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner20Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner20Pokemon As UShort

        Public Property Partner20IsFemale As Boolean
            Get
                Return _Partner20IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner20IsFemale = value Then
                    _Partner20IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner20IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner20IsFemale As Boolean

        Public Property Partner21Pokemon As UShort
            Get
                Return _Partner21Pokemon
            End Get
            Set(value As UShort)
                If Not _Partner21Pokemon = value Then
                    _Partner21Pokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner21Pokemon)))
                End If
            End Set
        End Property
        Dim _Partner21Pokemon As UShort

        Public Property Partner21IsFemale As Boolean
            Get
                Return _Partner21IsFemale
            End Get
            Set(value As Boolean)
                If Not _Partner21IsFemale = value Then
                    _Partner21IsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Partner21IsFemale)))
                End If
            End Set
        End Property
        Dim _Partner21IsFemale As Boolean

        Public Property HardyMalePokemon As UShort
            Get
                Return _HardyMalePokemon
            End Get
            Set(value As UShort)
                If Not _HardyMalePokemon = value Then
                    _HardyMalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HardyMalePokemon)))
                End If
            End Set
        End Property
        Dim _HardyMalePokemon As UShort

        Public Property HardyMaleIsFemale As Boolean
            Get
                Return _HardyMaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _HardyMaleIsFemale = value Then
                    _HardyMaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HardyMaleIsFemale)))
                End If
            End Set
        End Property
        Dim _HardyMaleIsFemale As Boolean

        Public Property HardyFemalePokemon As UShort
            Get
                Return _HardyFemalePokemon
            End Get
            Set(value As UShort)
                If Not _HardyFemalePokemon = value Then
                    _HardyFemalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HardyFemalePokemon)))
                End If
            End Set
        End Property
        Dim _HardyFemalePokemon As UShort

        Public Property HardyFemaleIsFemale As Boolean
            Get
                Return _HardyFemaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _HardyFemaleIsFemale = value Then
                    _HardyFemaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HardyFemaleIsFemale)))
                End If
            End Set
        End Property
        Dim _HardyFemaleIsFemale As Boolean

        Public Property DocileMalePokemon As UShort
            Get
                Return _DocileMalePokemon
            End Get
            Set(value As UShort)
                If Not _DocileMalePokemon = value Then
                    _DocileMalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(DocileMalePokemon)))
                End If
            End Set
        End Property
        Dim _DocileMalePokemon As UShort

        Public Property DocileMaleIsFemale As Boolean
            Get
                Return _DocileMaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _DocileMaleIsFemale = value Then
                    _DocileMaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(DocileMaleIsFemale)))
                End If
            End Set
        End Property
        Dim _DocileMaleIsFemale As Boolean

        Public Property DocileFemalePokemon As UShort
            Get
                Return _DocileFemalePokemon
            End Get
            Set(value As UShort)
                If Not _DocileFemalePokemon = value Then
                    _DocileFemalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(DocileFemalePokemon)))
                End If
            End Set
        End Property
        Dim _DocileFemalePokemon As UShort

        Public Property DocileFemaleIsFemale As Boolean
            Get
                Return _DocileFemaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _DocileFemaleIsFemale = value Then
                    _DocileFemaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(DocileFemaleIsFemale)))
                End If
            End Set
        End Property
        Dim _DocileFemaleIsFemale As Boolean

        Public Property BraveMalePokemon As UShort
            Get
                Return _BraveMalePokemon
            End Get
            Set(value As UShort)
                If Not _BraveMalePokemon = value Then
                    _BraveMalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(BraveMalePokemon)))
                End If
            End Set
        End Property
        Dim _BraveMalePokemon As UShort

        Public Property BraveMaleIsFemale As Boolean
            Get
                Return _BraveMaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _BraveMaleIsFemale = value Then
                    _BraveMaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(BraveMaleIsFemale)))
                End If
            End Set
        End Property
        Dim _BraveMaleIsFemale As Boolean

        Public Property BraveFemalePokemon As UShort
            Get
                Return _BraveFemalePokemon
            End Get
            Set(value As UShort)
                If Not _BraveFemalePokemon = value Then
                    _BraveFemalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(BraveFemalePokemon)))
                End If
            End Set
        End Property
        Dim _BraveFemalePokemon As UShort

        Public Property BraveFemaleIsFemale As Boolean
            Get
                Return _BraveFemaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _BraveFemaleIsFemale = value Then
                    _BraveFemaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(BraveFemaleIsFemale)))
                End If
            End Set
        End Property
        Dim _BraveFemaleIsFemale As Boolean

        Public Property JollyMalePokemon As UShort
            Get
                Return _JollyMalePokemon
            End Get
            Set(value As UShort)
                If Not _JollyMalePokemon = value Then
                    _JollyMalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(JollyMalePokemon)))
                End If
            End Set
        End Property
        Dim _JollyMalePokemon As UShort

        Public Property JollyMaleIsFemale As Boolean
            Get
                Return _JollyMaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _JollyMaleIsFemale = value Then
                    _JollyMaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(JollyMaleIsFemale)))
                End If
            End Set
        End Property
        Dim _JollyMaleIsFemale As Boolean

        Public Property JollyFemalePokemon As UShort
            Get
                Return _JollyFemalePokemon
            End Get
            Set(value As UShort)
                If Not _JollyFemalePokemon = value Then
                    _JollyFemalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(JollyFemalePokemon)))
                End If
            End Set
        End Property
        Dim _JollyFemalePokemon As UShort

        Public Property JollyFemaleIsFemale As Boolean
            Get
                Return _JollyFemaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _JollyFemaleIsFemale = value Then
                    _JollyFemaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(JollyFemaleIsFemale)))
                End If
            End Set
        End Property
        Dim _JollyFemaleIsFemale As Boolean

        Public Property ImpishMalePokemon As UShort
            Get
                Return _ImpishMalePokemon
            End Get
            Set(value As UShort)
                If Not _ImpishMalePokemon = value Then
                    _ImpishMalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ImpishMalePokemon)))
                End If
            End Set
        End Property
        Dim _ImpishMalePokemon As UShort

        Public Property ImpishMaleIsFemale As Boolean
            Get
                Return _ImpishMaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _ImpishMaleIsFemale = value Then
                    _ImpishMaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ImpishMaleIsFemale)))
                End If
            End Set
        End Property
        Dim _ImpishMaleIsFemale As Boolean

        Public Property ImpishFemalePokemon As UShort
            Get
                Return _ImpishFemalePokemon
            End Get
            Set(value As UShort)
                If Not _ImpishFemalePokemon = value Then
                    _ImpishFemalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ImpishFemalePokemon)))
                End If
            End Set
        End Property
        Dim _ImpishFemalePokemon As UShort

        Public Property ImpishFemaleIsFemale As Boolean
            Get
                Return _ImpishFemaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _ImpishFemaleIsFemale = value Then
                    _ImpishFemaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ImpishFemaleIsFemale)))
                End If
            End Set
        End Property
        Dim _ImpishFemaleIsFemale As Boolean

        Public Property NaiveMalePokemon As UShort
            Get
                Return _NaiveMalePokemon
            End Get
            Set(value As UShort)
                If Not _NaiveMalePokemon = value Then
                    _NaiveMalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(NaiveMalePokemon)))
                End If
            End Set
        End Property
        Dim _NaiveMalePokemon As UShort

        Public Property NaiveMaleIsFemale As Boolean
            Get
                Return _NaiveMaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _NaiveMaleIsFemale = value Then
                    _NaiveMaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(NaiveMaleIsFemale)))
                End If
            End Set
        End Property
        Dim _NaiveMaleIsFemale As Boolean

        Public Property NaiveFemalePokemon As UShort
            Get
                Return _NaiveFemalePokemon
            End Get
            Set(value As UShort)
                If Not _NaiveFemalePokemon = value Then
                    _NaiveFemalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(NaiveFemalePokemon)))
                End If
            End Set
        End Property
        Dim _NaiveFemalePokemon As UShort

        Public Property NaiveFemaleIsFemale As Boolean
            Get
                Return _NaiveFemaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _NaiveFemaleIsFemale = value Then
                    _NaiveFemaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(NaiveFemaleIsFemale)))
                End If
            End Set
        End Property
        Dim _NaiveFemaleIsFemale As Boolean

        Public Property TimidMalePokemon As UShort
            Get
                Return _TimidMalePokemon
            End Get
            Set(value As UShort)
                If Not _TimidMalePokemon = value Then
                    _TimidMalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TimidMalePokemon)))
                End If
            End Set
        End Property
        Dim _TimidMalePokemon As UShort

        Public Property TimidMaleIsFemale As Boolean
            Get
                Return _TimidMaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _TimidMaleIsFemale = value Then
                    _TimidMaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TimidMaleIsFemale)))
                End If
            End Set
        End Property
        Dim _TimidMaleIsFemale As Boolean

        Public Property TimidFemalePokemon As UShort
            Get
                Return _TimidFemalePokemon
            End Get
            Set(value As UShort)
                If Not _TimidFemalePokemon = value Then
                    _TimidFemalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TimidFemalePokemon)))
                End If
            End Set
        End Property
        Dim _TimidFemalePokemon As UShort

        Public Property TimidFemaleIsFemale As Boolean
            Get
                Return _TimidFemaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _TimidFemaleIsFemale = value Then
                    _TimidFemaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TimidFemaleIsFemale)))
                End If
            End Set
        End Property
        Dim _TimidFemaleIsFemale As Boolean

        Public Property HastyMalePokemon As UShort
            Get
                Return _HastyMalePokemon
            End Get
            Set(value As UShort)
                If Not _HastyMalePokemon = value Then
                    _HastyMalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HastyMalePokemon)))
                End If
            End Set
        End Property
        Dim _HastyMalePokemon As UShort

        Public Property HastyMaleIsFemale As Boolean
            Get
                Return _HastyMaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _HastyMaleIsFemale = value Then
                    _HastyMaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HastyMaleIsFemale)))
                End If
            End Set
        End Property
        Dim _HastyMaleIsFemale As Boolean

        Public Property HastyFemalePokemon As UShort
            Get
                Return _HastyFemalePokemon
            End Get
            Set(value As UShort)
                If Not _HastyFemalePokemon = value Then
                    _HastyFemalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HastyFemalePokemon)))
                End If
            End Set
        End Property
        Dim _HastyFemalePokemon As UShort

        Public Property HastyFemaleIsFemale As Boolean
            Get
                Return _HastyFemaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _HastyFemaleIsFemale = value Then
                    _HastyFemaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HastyFemaleIsFemale)))
                End If
            End Set
        End Property
        Dim _HastyFemaleIsFemale As Boolean

        Public Property SassyMalePokemon As UShort
            Get
                Return _SassyMalePokemon
            End Get
            Set(value As UShort)
                If Not _SassyMalePokemon = value Then
                    _SassyMalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SassyMalePokemon)))
                End If
            End Set
        End Property
        Dim _SassyMalePokemon As UShort

        Public Property SassyMaleIsFemale As Boolean
            Get
                Return _SassyMaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _SassyMaleIsFemale = value Then
                    _SassyMaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SassyMaleIsFemale)))
                End If
            End Set
        End Property
        Dim _SassyMaleIsFemale As Boolean

        Public Property SassyFemalePokemon As UShort
            Get
                Return _SassyFemalePokemon
            End Get
            Set(value As UShort)
                If Not _SassyFemalePokemon = value Then
                    _SassyFemalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SassyFemalePokemon)))
                End If
            End Set
        End Property
        Dim _SassyFemalePokemon As UShort

        Public Property SassyFemaleIsFemale As Boolean
            Get
                Return _SassyFemaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _SassyFemaleIsFemale = value Then
                    _SassyFemaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SassyFemaleIsFemale)))
                End If
            End Set
        End Property
        Dim _SassyFemaleIsFemale As Boolean

        Public Property CalmMalePokemon As UShort
            Get
                Return _CalmMalePokemon
            End Get
            Set(value As UShort)
                If Not _CalmMalePokemon = value Then
                    _CalmMalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(CalmMalePokemon)))
                End If
            End Set
        End Property
        Dim _CalmMalePokemon As UShort

        Public Property CalmMaleIsFemale As Boolean
            Get
                Return _CalmMaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _CalmMaleIsFemale = value Then
                    _CalmMaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(CalmMaleIsFemale)))
                End If
            End Set
        End Property
        Dim _CalmMaleIsFemale As Boolean

        Public Property CalmFemalePokemon As UShort
            Get
                Return _CalmFemalePokemon
            End Get
            Set(value As UShort)
                If Not _CalmFemalePokemon = value Then
                    _CalmFemalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(CalmFemalePokemon)))
                End If
            End Set
        End Property
        Dim _CalmFemalePokemon As UShort

        Public Property CalmFemaleIsFemale As Boolean
            Get
                Return _CalmFemaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _CalmFemaleIsFemale = value Then
                    _CalmFemaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(CalmFemaleIsFemale)))
                End If
            End Set
        End Property
        Dim _CalmFemaleIsFemale As Boolean

        Public Property RelaxedMalePokemon As UShort
            Get
                Return _RelaxedMalePokemon
            End Get
            Set(value As UShort)
                If Not _RelaxedMalePokemon = value Then
                    _RelaxedMalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(RelaxedMalePokemon)))
                End If
            End Set
        End Property
        Dim _RelaxedMalePokemon As UShort

        Public Property RelaxedMaleIsFemale As Boolean
            Get
                Return _RelaxedMaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _RelaxedMaleIsFemale = value Then
                    _RelaxedMaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(RelaxedMaleIsFemale)))
                End If
            End Set
        End Property
        Dim _RelaxedMaleIsFemale As Boolean

        Public Property RelaxedFemalePokemon As UShort
            Get
                Return _RelaxedFemalePokemon
            End Get
            Set(value As UShort)
                If Not _RelaxedFemalePokemon = value Then
                    _RelaxedFemalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(RelaxedFemalePokemon)))
                End If
            End Set
        End Property
        Dim _RelaxedFemalePokemon As UShort

        Public Property RelaxedFemaleIsFemale As Boolean
            Get
                Return _RelaxedFemaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _RelaxedFemaleIsFemale = value Then
                    _RelaxedFemaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(RelaxedFemaleIsFemale)))
                End If
            End Set
        End Property
        Dim _RelaxedFemaleIsFemale As Boolean

        Public Property LonelyMalePokemon As UShort
            Get
                Return _LonelyMalePokemon
            End Get
            Set(value As UShort)
                If Not _LonelyMalePokemon = value Then
                    _LonelyMalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(LonelyMalePokemon)))
                End If
            End Set
        End Property
        Dim _LonelyMalePokemon As UShort

        Public Property LonelyMaleIsFemale As Boolean
            Get
                Return _LonelyMaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _LonelyMaleIsFemale = value Then
                    _LonelyMaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(LonelyMaleIsFemale)))
                End If
            End Set
        End Property
        Dim _LonelyMaleIsFemale As Boolean

        Public Property LonelyFemalePokemon As UShort
            Get
                Return _LonelyFemalePokemon
            End Get
            Set(value As UShort)
                If Not _LonelyFemalePokemon = value Then
                    _LonelyFemalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(LonelyFemalePokemon)))
                End If
            End Set
        End Property
        Dim _LonelyFemalePokemon As UShort

        Public Property LonelyFemaleIsFemale As Boolean
            Get
                Return _LonelyFemaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _LonelyFemaleIsFemale = value Then
                    _LonelyFemaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(LonelyFemaleIsFemale)))
                End If
            End Set
        End Property
        Dim _LonelyFemaleIsFemale As Boolean

        Public Property QuirkyMalePokemon As UShort
            Get
                Return _QuirkyMalePokemon
            End Get
            Set(value As UShort)
                If Not _QuirkyMalePokemon = value Then
                    _QuirkyMalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(QuirkyMalePokemon)))
                End If
            End Set
        End Property
        Dim _QuirkyMalePokemon As UShort

        Public Property QuirkyMaleIsFemale As Boolean
            Get
                Return _QuirkyMaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _QuirkyMaleIsFemale = value Then
                    _QuirkyMaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(QuirkyMaleIsFemale)))
                End If
            End Set
        End Property
        Dim _QuirkyMaleIsFemale As Boolean

        Public Property QuirkyFemalePokemon As UShort
            Get
                Return _QuirkyFemalePokemon
            End Get
            Set(value As UShort)
                If Not _QuirkyFemalePokemon = value Then
                    _QuirkyFemalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(QuirkyFemalePokemon)))
                End If
            End Set
        End Property
        Dim _QuirkyFemalePokemon As UShort

        Public Property QuirkyFemaleIsFemale As Boolean
            Get
                Return _QuirkyFemaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _QuirkyFemaleIsFemale = value Then
                    _QuirkyFemaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(QuirkyFemaleIsFemale)))
                End If
            End Set
        End Property
        Dim _QuirkyFemaleIsFemale As Boolean

        Public Property QuietMalePokemon As UShort
            Get
                Return _QuietMalePokemon
            End Get
            Set(value As UShort)
                If Not _QuietMalePokemon = value Then
                    _QuietMalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(QuietMalePokemon)))
                End If
            End Set
        End Property
        Dim _QuietMalePokemon As UShort

        Public Property QuietMaleIsFemale As Boolean
            Get
                Return _QuietMaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _QuietMaleIsFemale = value Then
                    _QuietMaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(QuietMaleIsFemale)))
                End If
            End Set
        End Property
        Dim _QuietMaleIsFemale As Boolean

        Public Property QuietFemalePokemon As UShort
            Get
                Return _QuietFemalePokemon
            End Get
            Set(value As UShort)
                If Not _QuietFemalePokemon = value Then
                    _QuietFemalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(QuietFemalePokemon)))
                End If
            End Set
        End Property
        Dim _QuietFemalePokemon As UShort

        Public Property QuietFemaleIsFemale As Boolean
            Get
                Return _QuietFemaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _QuietFemaleIsFemale = value Then
                    _QuietFemaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(QuietFemaleIsFemale)))
                End If
            End Set
        End Property
        Dim _QuietFemaleIsFemale As Boolean

        Public Property RashMalePokemon As UShort
            Get
                Return _RashMalePokemon
            End Get
            Set(value As UShort)
                If Not _RashMalePokemon = value Then
                    _RashMalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(RashMalePokemon)))
                End If
            End Set
        End Property
        Dim _RashMalePokemon As UShort

        Public Property RashMaleIsFemale As Boolean
            Get
                Return _RashMaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _RashMaleIsFemale = value Then
                    _RashMaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(RashMaleIsFemale)))
                End If
            End Set
        End Property
        Dim _RashMaleIsFemale As Boolean

        Public Property RashFemalePokemon As UShort
            Get
                Return _RashFemalePokemon
            End Get
            Set(value As UShort)
                If Not _RashFemalePokemon = value Then
                    _RashFemalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(RashFemalePokemon)))
                End If
            End Set
        End Property
        Dim _RashFemalePokemon As UShort

        Public Property RashFemaleIsFemale As Boolean
            Get
                Return _RashFemaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _RashFemaleIsFemale = value Then
                    _RashFemaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(RashFemaleIsFemale)))
                End If
            End Set
        End Property
        Dim _RashFemaleIsFemale As Boolean

        Public Property BoldMalePokemon As UShort
            Get
                Return _BoldMalePokemon
            End Get
            Set(value As UShort)
                If Not _BoldMalePokemon = value Then
                    _BoldMalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(BoldMalePokemon)))
                End If
            End Set
        End Property
        Dim _BoldMalePokemon As UShort

        Public Property BoldMaleIsFemale As Boolean
            Get
                Return _BoldMaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _BoldMaleIsFemale = value Then
                    _BoldMaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(BoldMaleIsFemale)))
                End If
            End Set
        End Property
        Dim _BoldMaleIsFemale As Boolean

        Public Property BoldFemalePokemon As UShort
            Get
                Return _BoldFemalePokemon
            End Get
            Set(value As UShort)
                If Not _BoldFemalePokemon = value Then
                    _BoldFemalePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(BoldFemalePokemon)))
                End If
            End Set
        End Property
        Dim _BoldFemalePokemon As UShort

        Public Property BoldFemaleIsFemale As Boolean
            Get
                Return _BoldFemaleIsFemale
            End Get
            Set(value As Boolean)
                If Not _BoldFemaleIsFemale = value Then
                    _BoldFemaleIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(BoldFemaleIsFemale)))
                End If
            End Set
        End Property
        Dim _BoldFemaleIsFemale As Boolean

#End Region
    End Class
End Namespace
