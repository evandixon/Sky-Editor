Imports PokemonDSLib.PokemonLib

Public Class PKMGenerator

    Public Enum Methods
        Method1 = 1
        Method2
        Method3
        Method4
    End Enum

    Public PRNG As New PokePRNG

    Public Sub New(ByVal Delay As UInt32)
        PRNG.Seed = GenerateSeed(Delay)
    End Sub

    Public Sub New(ByVal Delay As UInt32, ByVal _Date As Date)
        PRNG.Seed = GenerateSeed(Delay, _Date)
    End Sub

    Public Function GenerateSeed(ByVal Delay As UInt32) As UInt32
        With Now
            Return (((.Month() * .Day + .Minute() + .Second()) Mod &H100) * &H1000000) + (.Hour() * &H10000) + (.Year() - 2000 + Delay)
        End With
    End Function

    Public Function GenerateSeed(ByVal Delay As UInt32, ByVal _Date As Date) As UInt32
        With _Date
            Return (((.Month() * .Day + .Minute() + .Second()) Mod &H100) * &H1000000) + (.Hour() * &H10000) + (.Year() - 2000 + Delay)
        End With
    End Function

    Public Function GeneratePID(Optional ByVal Method3 As Boolean = False) As UInt32
        Dim RESULT1, RESULT2 As UInt32
        RESULT1 = PRNG.NextNum >> 16 And &HFFFF
        If Method3 Then PRNG.NextNum()
        RESULT2 = PRNG.NextNum >> 16 And &HFFFF
        'Return Convert.ToUInt32(Hex(RESULT2) & Hex(RESULT1), 16)
        Return (RESULT2 << 16) + RESULT1
    End Function

    Public Function GenerateIVs(Optional ByVal Method4 As Boolean = False) As UInt32 'As Byte()
        Dim RESULT1, RESULT2 As UInt32
        RESULT1 = PRNG.NextNum >> 16 And &HFFFF
        If Method4 Then PRNG.NextNum()
        RESULT2 = PRNG.NextNum >> 16 And &HFFFF
        'Return Convert.ToUInt32((DecToBin(RESULT2, 15) & DecToBin(RESULT1, 15)), 2)
        Return ((RESULT2 And &H7FFF) << 15) + (RESULT1 And &H7FFF)
    End Function

    Public Function Generate(ByVal SpeciesID As Species,
ByVal Method As PKMGenerator.Methods,
ByVal OTName As String,
ByVal OTGender As Genders,
ByVal OTID As UInt16,
ByVal OTSID As UInt16,
ByVal MetLevel As Byte,
ByVal EggDate As Date,
ByVal DPEggMet As DSLocations,
ByVal PtEggMet As DSLocations,
ByVal DPMet As DSLocations,
ByVal PtMet As DSLocations,
ByVal MetDate As Date,
ByVal Hometown As Hometowns,
ByVal Encounter As Encounters,
ByVal Country As Countries,
ByVal BallCaught As Balls,
ByVal Move1 As Moves,
ByVal Move2 As Moves,
ByVal Move3 As Moves,
ByVal Move4 As Moves) As Pokemon

        Dim theTrainer As New mTrainer(OTName, New mGender(OTGender), OTID, OTSID)
        Dim theOrigins As New mOrigins(MetLevel, New _
                                       mEgg(EggDate, New mMet(DPEggMet, PtEggMet)),
                                       New mMet(DPMet, PtMet), MetDate, New mmHometown(Hometown),
                                       New mmCountry(Country))
        Dim theMoves(3) As mMoves
        theMoves(0) = New mMoves(Move1)
        theMoves(1) = New mMoves(Move2)
        theMoves(2) = New mMoves(Move3)
        theMoves(3) = New mMoves(Move4)

        For i As Integer = 0 To 3
            If theMoves(i).Value <> Moves.NOTHING Then
                theMoves(i).TotalPP = theMoves(i).BasePP
                theMoves(i).CurrentPP = theMoves(i).BasePP
            End If
        Next

        Return Generate(SpeciesID, Method, theTrainer, theOrigins, New mEncounters(Encounter), theMoves)

    End Function

    Public Function Generate(ByVal SpeciesID As Species, ByVal Method As Methods,
                             ByVal _Trainer As mTrainer, ByVal _Origins As mOrigins,
                             ByVal Encounter As mEncounters, ByVal _Moves As mMoves()) As Pokemon
        Dim pkmOUT As New Pokemon
        Select Case Method
            Case Methods.Method1
                pkmOUT = Method1(SpeciesID, _Trainer, _Origins, _Moves)
            Case Methods.Method2
                pkmOUT = Method2(SpeciesID, _Trainer, _Origins, _Moves)
            Case Methods.Method3
                pkmOUT = Method3(SpeciesID, _Trainer, _Origins, _Moves)
            Case Methods.Method4
                pkmOUT = Method4(SpeciesID, _Trainer, _Origins, _Moves)
            Case Else
                Return New Pokemon
        End Select
        pkmOUT.Encounter = Encounter
        Return pkmOUT
    End Function

    Private Function Method1(ByVal SpeciesID As Species, ByVal _Trainer As mTrainer, ByVal _Origins As mOrigins, ByVal _Moves As mMoves()) As Pokemon
        Method1 = New Pokemon
        Method1.Species = New mSpecies(SpeciesID)
        Method1.PID = GeneratePID()
        Dim mIVsAndEtc As UInt32 = GenerateIVs()
        'Dim theIVs As New mIVs
        'With theIVs
        '    .HP = (mIVsAndEtc >> (0)) And &H1F
        '    .Attack = (mIVsAndEtc >> (5)) And &H1F
        '    .Defense = (mIVsAndEtc >> (10)) And &H1F
        '    .Speed = (mIVsAndEtc >> (15)) And &H1F
        '    .SpAttack = (mIVsAndEtc >> (20)) And &H1F
        '    .SpDefense = (mIVsAndEtc >> (25)) And &H1F
        'End With
        'Method1.IVs = theIVs
        Method1.mIVsAndEtc = mIVsAndEtc
        Return SetValues(Method1, _Trainer, _Origins, _Moves)
    End Function

    Private Function Method2(ByVal SpeciesID As Species, ByVal _Trainer As mTrainer, ByVal _Origins As mOrigins, ByVal _Moves As mMoves()) As Pokemon
        Method2 = New Pokemon
        Method2.Species = New mSpecies(SpeciesID)
        Method2.PID = GeneratePID()
        PRNG.NextNum()
        Dim mIVsAndEtc As UInt32 = GenerateIVs()
        'Dim theIVs As New mIVs
        'With theIVs
        '    .HP = (mIVsAndEtc >> (0)) And &H1F
        '    .Attack = (mIVsAndEtc >> (5)) And &H1F
        '    .Defense = (mIVsAndEtc >> (10)) And &H1F
        '    .Speed = (mIVsAndEtc >> (15)) And &H1F
        '    .SpAttack = (mIVsAndEtc >> (20)) And &H1F
        '    .SpDefense = (mIVsAndEtc >> (25)) And &H1F
        'End With
        'Method2.IVs = theIVs
        Method2.mIVsAndEtc = mIVsAndEtc
        Return SetValues(Method2, _Trainer, _Origins, _Moves)
    End Function

    Private Function Method3(ByVal SpeciesID As Species, ByVal _Trainer As mTrainer, ByVal _Origins As mOrigins, ByVal _Moves As mMoves()) As Pokemon
        Method3 = New Pokemon
        Method3.Species = New mSpecies(SpeciesID)
        Method3.PID = GeneratePID(True)
        Dim mIVsAndEtc As UInt32 = GenerateIVs()
        'Dim theIVs As New mIVs
        'With theIVs
        '    .HP = (mIVsAndEtc >> (0)) And &H1F
        '    .Attack = (mIVsAndEtc >> (5)) And &H1F
        '    .Defense = (mIVsAndEtc >> (10)) And &H1F
        '    .Speed = (mIVsAndEtc >> (15)) And &H1F
        '    .SpAttack = (mIVsAndEtc >> (20)) And &H1F
        '    .SpDefense = (mIVsAndEtc >> (25)) And &H1F
        'End With
        'Method3.IVs = theIVs
        Method3.mIVsAndEtc = mIVsAndEtc
        Return SetValues(Method3, _Trainer, _Origins, _Moves)
    End Function

    Private Function Method4(ByVal SpeciesID As Species, ByVal _Trainer As mTrainer, ByVal _Origins As mOrigins, ByVal _Moves As mMoves()) As Pokemon
        Method4 = New Pokemon
        Method4.Species = New mSpecies(SpeciesID)
        Method4.PID = GeneratePID()
        Dim mIVsAndEtc As UInt32 = GenerateIVs(True)
        'Dim theIVs As New mIVs
        'With theIVs
        '    .HP = (mIVsAndEtc >> (0)) And &H1F
        '    .Attack = (mIVsAndEtc >> (5)) And &H1F
        '    .Defense = (mIVsAndEtc >> (10)) And &H1F
        '    .Speed = (mIVsAndEtc >> (15)) And &H1F
        '    .SpAttack = (mIVsAndEtc >> (20)) And &H1F
        '    .SpDefense = (mIVsAndEtc >> (25)) And &H1F
        'End With
        'Method4.IVs = theIVs
        Method4.mIVsAndEtc = mIVsAndEtc
        Return SetValues(Method4, _Trainer, _Origins, _Moves)
    End Function

    Public Function SetValues(ByVal PKM As Pokemon, ByVal _Trainer As mTrainer, ByVal _Origins As mOrigins, ByVal _Moves As mMoves()) As Pokemon
        With PKM
            Try
                'TODO: Set values for generated PKM.
                If .Class = 0 Or .BaseStats.Ability2 = 0 Then
                    .Ability = New mAbility(.BaseStats.Ability1)
                Else
                    .Ability = New mAbility(.BaseStats.Ability2)
                End If

                If .BaseStats.Gender = 255 Then
                    .Gender = New mGender(Genders.Genderless)
                ElseIf .BaseStats.Gender = 254 Then
                    .Gender = New mGender(Genders.Female)
                ElseIf .BaseStats.Gender = 0 Then
                    .Gender = New mGender(Genders.Male)
                Else

                    If (.PID Mod 256) > .BaseStats.Gender Then
                        .Gender = New mGender(Genders.Male)
                    Else
                        .Gender = New mGender(Genders.Female)
                    End If
                End If

                .ClearMarks()
                .EXP = 0
                .FatefulEncounter = False
                .Forme = 0
                .IsEgg = False
                .Item = New mItems(Items.NOTHING)

                'Dim theMoves(3) As mMoves
                'theMoves(0) = New mMoves(Moves.NOTHING)
                'theMoves(1) = New mMoves(Moves.NOTHING)
                'theMoves(2) = New mMoves(Moves.NOTHING)
                'theMoves(3) = New mMoves(Moves.NOTHING)
                .Moves = _Moves

                .Nickname = UCase(.Species.Name)
                .Nicknamed = False

                .Origins = _Origins
                PKM.SetEggDateBytes(New Byte() {0, 0, 0})

                .Tameness = 70
                .Trainer = _Trainer

                .Recalculate()

            Catch ex As Exception

            End Try
        End With
        Return PKM
    End Function

#Region "Copyright Notice"
    '
    ' * This file is part of RNG Reporter
    ' * Copyright (C) 2009 by Bill Young
    ' *
    ' * This program is free software; you can redistribute it and/or
    ' * modify it under the terms of the GNU General Public License
    ' * as published by the Free Software Foundation; either version 2
    ' * of the License, or (at your option) any later version.
    ' *
    ' * This program is distributed in the hope that it will be useful,
    ' * but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' * GNU General Public License for more details.
    ' *
    ' * You should have received a copy of the GNU General Public License
    ' * along with this program; if not, write to the Free Software
    ' * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
    '
#End Region

    Public Enum FrameTypes
        Method1 = 1
        Method2
        Method3
        Method4
    End Enum

    Public Class seed
        Private mframe As FrameTypes
        Public PID As UInt32
        Public MonsterSeed As UInt32
        Public SID As UInt16

        Public Property FrameType() As FrameTypes
            Get
                Return mframe
            End Get
            Set(ByVal value As FrameTypes)
                mframe = value
            End Set
        End Property

    End Class

    ''' <summary>
    ''' This class is going to do an IV/PID/Seed calculation given a particular
    ''' method (1, 2 or 3, or 4).  Should use the same code to develop canidate IVs.
    ''' </summary>
    Class IVtoSeed
        '  We need a function to return a list of monster seeds,
        '  which will be updated to include a method.

        Public Shared Function GetSeeds(ByVal IVs As PokemonLib.mIVs, ByVal Nature As PokemonLib.Natures, ByVal ID As UInt16) As List(Of seed)

            Dim hp, atk, def, spa, spd, spe As UInteger

            With IVs
                hp = .HP
                atk = .Attack
                def = .Defense
                spa = .SpAttack
                spd = .SpDefense
                spe = .Speed
            End With

            Dim seeds As New List(Of seed)()

            Dim x4 As UInteger = 0
            Dim x4_2 As UInteger = 0

            x4 = spe + (spa << 5) + (spd << 10)
            x4_2 = x4 Xor &H8000

            '  Now we want to start with IV2 and call the RNG for
            '  values between 0 and FFFF in the low order bits.
            For cnt As UInteger = 0 To &H1FFFE
                Dim x_test As UInteger

                '  We want to test with the high bit
                '  both set and not set, so we're going
                '  to sneakily do them both.  god help
                '  me if i ever have to figure this out
                '  in the future.
                If cnt Mod 2 = 0 Then
                    x_test = x4
                Else
                    x_test = x4_2
                End If

                '  Set our test seed here so we can start
                '  working backwards to see if the rest
                '  of the information we were provided
                '  is a match.

                Dim seed As UInteger = (x_test << 16) + (cnt Mod &HFFFF)
                Dim rng As New PokePRNG 'LCRNGR(seed)
                rng.Seed = seed

                '  Right now, this simply assumes method
                '  1 and gets the value previous to check
                '  for  match.  We need a clean way to do
                '  this for all of our methods.

                '  We have a max of 5 total RNG calls
                '  to make a pokemon and we already have
                '  one so lets go ahead and get 4 more.
                Dim rng1 As UShort = rng.PreviousNum >> 16 And &HFFFF 'rng.GetPrevious16BitNumber()
                Dim rng2 As UShort = rng.PreviousNum >> 16 And &HFFFF 'rng.GetPrevious16BitNumber()
                Dim rng3 As UShort = rng.PreviousNum >> 16 And &HFFFF 'rng.GetPrevious16BitNumber()
                Dim rng4 As UShort = rng.PreviousNum >> 16 And &HFFFF 'rng.GetPrevious16BitNumber()

                Dim method1Seed As UInteger = rng.Seed
                rng.PreviousNum()
                Dim method234Seed As UInteger = rng.Seed

                '  Check Method 1
                ' [PID] [PID] [IVs] [IVs]
                ' [rng3] [rng2] [rng1] [START]
                If Check(rng1, rng2, rng3, hp, atk, def,
                 Nature) Then
                    '  Build a seed to add to our collection
                    Dim newSeed As New seed()
                    newSeed.FrameType = FrameTypes.Method1
                    newSeed.PID = (CUInt(rng2) << 16) + CUInt(rng3)
                    newSeed.MonsterSeed = method1Seed
                    newSeed.SID = (CUInt(rng2) Xor CUInt(rng3) Xor ID) And &HFFF8

                    seeds.Add(newSeed)
                End If

                '  Check Method 2
                ' [PID] [PID] [xxxx] [IVs] [IVs]
                ' [rng4] [rng3] [xxxx] [rng1] [START]
                If Check(rng1, rng3, rng4, hp, atk, def,
                 Nature) Then
                    '  Build a seed to add to our collection
                    Dim newSeed As New seed()
                    newSeed.FrameType = FrameTypes.Method2
                    newSeed.PID = (CUInt(rng3) << 16) + CUInt(rng4)
                    newSeed.MonsterSeed = method234Seed
                    newSeed.SID = (CUInt(rng3) Xor CUInt(rng4) Xor ID) And &HFFF8

                    seeds.Add(newSeed)
                End If

                '  Check Method 3
                '  [PID] [xxxx] [PID] [IVs] [IVs]
                '  [rng4] [xxxx] [rng2] [rng1] [START]
                If Check(rng1, rng2, rng4, hp, atk, def,
                 Nature) Then
                    '  Build a seed to add to our collection
                    Dim newSeed As New seed()
                    newSeed.FrameType = FrameTypes.Method3
                    newSeed.PID = (CUInt(rng2) << 16) + CUInt(rng4)
                    newSeed.MonsterSeed = method234Seed
                    newSeed.SID = (CUInt(rng2) Xor CUInt(rng4) Xor ID) And &HFFF8

                    seeds.Add(newSeed)
                End If

                '  Check Method 4
                '  [PID] [PID] [IVs] [xxxx] [IVs]
                '  [rng4] [rng3] [rng2] [xxxx] [START]
                If Check(rng2, rng3, rng4, hp, atk, def,
                 Nature) Then
                    '  Build a seed to add to our collection
                    Dim newSeed As New seed()
                    newSeed.FrameType = FrameTypes.Method4
                    newSeed.PID = (CUInt(rng3) << 16) + CUInt(rng4)
                    newSeed.MonsterSeed = method234Seed
                    newSeed.SID = (CUInt(rng3) Xor CUInt(rng4) Xor ID) And &HFFF8

                    seeds.Add(newSeed)
                End If
            Next

            Return seeds
        End Function

        Public Shared Function GetSeeds(ByVal hp As UInteger, ByVal atk As UInteger, ByVal def As UInteger,
                                        ByVal spa As UInteger, ByVal spd As UInteger, ByVal spe As UInteger,
                                        ByVal nature As UInteger, ByVal id As UInteger) As List(Of seed)
            Dim seeds As New List(Of seed)()

            Dim x4 As UInteger = 0
            Dim x4_2 As UInteger = 0

            x4 = spe + (spa << 5) + (spd << 10)
            x4_2 = x4 Xor &H8000

            '  Now we want to start with IV2 and call the RNG for
            '  values between 0 and FFFF in the low order bits.
            For cnt As UInteger = 0 To &H1FFFE
                Dim x_test As UInteger

                '  We want to test with the high bit
                '  both set and not set, so we're going
                '  to sneakily do them both.  god help
                '  me if i ever have to figure this out
                '  in the future.
                If cnt Mod 2 = 0 Then
                    x_test = x4
                Else
                    x_test = x4_2
                End If

                '  Set our test seed here so we can start
                '  working backwards to see if the rest
                '  of the information we were provided
                '  is a match.

                Dim seed As UInteger = (x_test << 16) + (cnt Mod &HFFFF)
                Dim rng As New PokePRNG 'LCRNGR(seed)
                rng.Seed = seed

                '  Right now, this simply assumes method
                '  1 and gets the value previous to check
                '  for  match.  We need a clean way to do
                '  this for all of our methods.

                '  We have a max of 5 total RNG calls
                '  to make a pokemon and we already have
                '  one so lets go ahead and get 4 more.
                Dim rng1 As UShort = rng.PreviousNum >> 16 And &HFFFF 'rng.GetPrevious16BitNumber()
                Dim rng2 As UShort = rng.PreviousNum >> 16 And &HFFFF 'rng.GetPrevious16BitNumber()
                Dim rng3 As UShort = rng.PreviousNum >> 16 And &HFFFF 'rng.GetPrevious16BitNumber()
                Dim rng4 As UShort = rng.PreviousNum >> 16 And &HFFFF 'rng.GetPrevious16BitNumber()

                Dim method1Seed As UInteger = rng.Seed
                rng.PreviousNum()
                Dim method234Seed As UInteger = rng.Seed

                '  Check Method 1
                ' [PID] [PID] [IVs] [IVs]
                ' [rng3] [rng2] [rng1] [START]
                If Check(rng1, rng2, rng3, hp, atk, def,
                 nature) Then
                    '  Build a seed to add to our collection
                    Dim newSeed As New seed()
                    newSeed.FrameType = FrameTypes.Method1
                    newSeed.PID = (CUInt(rng2) << 16) + CUInt(rng3)
                    newSeed.MonsterSeed = method1Seed
                    newSeed.SID = (CUInt(rng2) Xor CUInt(rng3) Xor id) And &HFFF8

                    seeds.Add(newSeed)
                End If

                '  Check Method 2
                ' [PID] [PID] [xxxx] [IVs] [IVs]
                ' [rng4] [rng3] [xxxx] [rng1] [START]
                If Check(rng1, rng3, rng4, hp, atk, def,
                 nature) Then
                    '  Build a seed to add to our collection
                    Dim newSeed As New seed()
                    newSeed.FrameType = FrameTypes.Method2
                    newSeed.PID = (CUInt(rng3) << 16) + CUInt(rng4)
                    newSeed.MonsterSeed = method234Seed
                    newSeed.SID = (CUInt(rng3) Xor CUInt(rng4) Xor id) And &HFFF8

                    seeds.Add(newSeed)
                End If

                '  Check Method 3
                '  [PID] [xxxx] [PID] [IVs] [IVs]
                '  [rng4] [xxxx] [rng2] [rng1] [START]
                If Check(rng1, rng2, rng4, hp, atk, def,
                 nature) Then
                    '  Build a seed to add to our collection
                    Dim newSeed As New seed()
                    newSeed.FrameType = FrameTypes.Method3
                    newSeed.PID = (CUInt(rng2) << 16) + CUInt(rng4)
                    newSeed.MonsterSeed = method234Seed
                    newSeed.SID = (CUInt(rng2) Xor CUInt(rng4) Xor id) And &HFFF8

                    seeds.Add(newSeed)
                End If

                '  Check Method 4
                '  [PID] [PID] [IVs] [xxxx] [IVs]
                '  [rng4] [rng3] [rng2] [xxxx] [START]
                If Check(rng2, rng3, rng4, hp, atk, def,
                 nature) Then
                    '  Build a seed to add to our collection
                    Dim newSeed As New seed()
                    newSeed.FrameType = FrameTypes.Method4
                    newSeed.PID = (CUInt(rng3) << 16) + CUInt(rng4)
                    newSeed.MonsterSeed = method234Seed
                    newSeed.SID = (CUInt(rng3) Xor CUInt(rng4) Xor id) And &HFFF8

                    seeds.Add(newSeed)
                End If
            Next

            Return seeds
        End Function

        Public Shared Function Check(ByVal iv As UShort, ByVal pid2 As UShort, ByVal pid1 As UShort, ByVal hp As UInteger, ByVal atk As UInteger, ByVal def As UInteger,
         ByVal nature As UInteger) As Boolean
            Dim ret As Boolean = False

            Dim test_hp As UInteger = CUInt(iv) And &H1F
            Dim test_atk As UInteger = (CUInt(iv) And &H3E0) >> 5
            Dim test_def As UInteger = (CUInt(iv) And &H7C00) >> 10

            If test_hp = hp AndAlso test_atk = atk AndAlso test_def = def Then

                '  Use these two values to see if we have a possible
                '  match for the nature of this pokemon.  Also, if
                '  we have a match then the RNG will contain a
                '  seeding possibility.

                Dim pid As UInteger = (CUInt(pid2) << 16) + CUInt(pid1)

                Dim pidNature As UInteger = pid Mod 25

                '  Do a nature comparison with what we have selected
                '  in the dropdown and if we have a good match we can
                '  go ahead and add this to our starting seeds.
                If nature = pidNature Then
                    ret = True
                End If
            End If

            Return ret
        End Function
        ' bool Check(ushort iv, short pid2, ushort pid1)
    End Class

End Class