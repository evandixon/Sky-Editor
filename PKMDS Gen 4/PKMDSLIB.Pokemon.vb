Imports System.Runtime.InteropServices
Imports System.Drawing
Imports System.IO
Partial Class PokemonLib
    ''' <summary>
    ''' A structure for containing decrypted, unshuffled Pokémon data.
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
<StructLayout(LayoutKind.Sequential, Size:=236)> _
    Public Class Pokemon

#Region "Members"

#Region "Unencrypted Data"
        Public PID As UInt32
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> _
        Private Unused1 As Byte()
        Private Checksum As UInt16
#End Region

#Region "Block A"
        Private mSpeciesID As Species
        Private mItem As Items
        Private mTID As UInt16
        Private mSID As UInt16
        Public EXP As UInt32
        Public Tameness As Byte
        Private mmAbility As Abilities
        Private mMarkings As Byte
        Private mCountry As Countries
        Private HPEV As Byte
        Private AtkEV As Byte
        Private DefEV As Byte
        Private SpdEV As Byte
        Private SPAtkEV As Byte
        Private SPDefEV As Byte
        Private mCool As Byte
        Private mBeauty As Byte
        Private mCute As Byte
        Private mSmart As Byte
        Private mTough As Byte
        Private mSheen As Byte
        Private mSRibbon1 As UInt16
        Private mSRibbon2 As UInt16
#End Region

#Region "Block B"
        Private mMove1 As Moves
        Private mMove2 As Moves
        Private mMove3 As Moves
        Private mMove4 As Moves
        Private mM1PP As Byte
        Private mM2PP As Byte
        Private mM3PP As Byte
        Private mM4PP As Byte
        Public mM1PPUp As Byte
        Public mM2PPUp As Byte
        Public mM3PPUp As Byte
        Public mM4PPUp As Byte
        Public mIVsAndEtc As UInt32
        Private mHRibbon1 As UInt16
        Private mHRibbon2 As UInt16
        Private mFormes As UInt16 'Contains gender as well.
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> _
        Private Unused2 As Byte()
        Private mPlatEggMet As DSLocations
        Private mPtMet As DSLocations
#End Region

#Region "Block C"
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=22)> _
        Private mNickname As Byte()
        Private Unused3 As Byte
        Private mHometown As Hometowns
        Private mSRibbon3 As UInt16
        Private mSRibbon4 As UInt16
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=4)> _
        Private Unused4 As Byte()
#End Region

#Region "Block D"
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=16)> _
        Private mOTName As Byte()
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=3)> _
        Private mEggDate As Byte()
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=3)> _
        Private mMetDate As Byte()
        Private mDPEggMet As DSLocations
        Private mDPMet As DSLocations
        Private mPKRS As Byte
        Private mBall As Balls
        Private mMetLevelEtc As Byte
        Private mEncounter As Encounters
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> _
        Private Unused5 As Byte()
#End Region

#Region "Battle Stats"

        Private mAilment As Byte
        Private UnknownFlags As Byte
        Private Unknown As UInt16
        Private mLevel As Byte
        Private CapsuleID As Byte
        Private mCurHP As UInt16
        Private mMaxHP As UInt16
        Private mAtk As UInt16
        Private mDef As UInt16
        Private mSpd As UInt16
        Private mSpAtk As UInt16
        Private mSpDef As UInt16
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H38)> _
        Private UnknownTrash As Byte()
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H18)> _
        Private SealCoordinates As Byte()
#End Region

#End Region

#Region "Constructors"

        Public Sub New()
            PID = 0UI
            'Unused1 = Nothing
            Checksum = 0US
            mSpeciesID = 0US
            mItem = 0US
            mTID = 0US
            mSID = 0US
            EXP = 0UI
            Tameness = 0
            mmAbility = 0
            mMarkings = 0
            mCountry = 0
            HPEV = 0
            AtkEV = 0
            DefEV = 0
            SpdEV = 0
            SPAtkEV = 0
            SPDefEV = 0
            mCool = 0
            mBeauty = 0
            mCute = 0
            mSmart = 0
            mTough = 0
            mSheen = 0
            mSRibbon1 = 0US
            mSRibbon2 = 0US
            mMove1 = 0US
            mMove2 = 0US
            mMove3 = 0US
            mMove4 = 0US
            mM1PP = 0
            mM2PP = 0
            mM3PP = 0
            mM4PP = 0
            mM1PPUp = 0
            mM2PPUp = 0
            mM3PPUp = 0
            mM4PPUp = 0
            mIVsAndEtc = 0UI
            mHRibbon1 = 0US
            mHRibbon2 = 0US
            mFormes = 0US
            'Unused2 = Nothing
            mPlatEggMet = 0US
            mPtMet = 0US
            'mNickname = Nothing
            Unused3 = 0
            mHometown = 0
            mSRibbon3 = 0US
            mSRibbon4 = 0US
            'Unused4 = Nothing
            'mOTName = Nothing
            'mEggDate = Nothing
            'mMetDate = Nothing
            mDPEggMet = 0US
            mDPMet = 0US
            mPKRS = 0
            mBall = 0
            mMetLevelEtc = 0
            mEncounter = 0
            'Unused5 = Nothing
            mAilment = 0
            UnknownFlags = 0
            Unknown = 0US
            mLevel = 0
            CapsuleID = 0
            mCurHP = 0US
            mMaxHP = 0US
            mAtk = 0US
            mDef = 0US
            mSpd = 0US
            mSpAtk = 0US
            mSpDef = 0US
            'UnknownTrash = Nothing
            'SealCoordinates = Nothing
        End Sub

#End Region

#Region "Properties"

        Public Property Nickname() As String
            Get
                InitializeDictionaries()
                Return PKMBytesToString(mNickname)
            End Get
            Set(ByVal value As String)
                If value.Length > 10 Then value = value.Substring(0, 10)
                Dim out() As Byte
                out = StringToPKMBytes(value, True)
                ReDim mNickname(21)
                Array.Copy(out, 0, mNickname, 0, out.Length)
            End Set
        End Property

        Public Property Trainer() As mTrainer
            Get
                Dim BA As BitArray = New BitArray(System.BitConverter.GetBytes(CByte(mMetLevelEtc)))
                Dim Index As Byte = 0
                If BA(7) Then Index = 1
                Return New mTrainer(mOTName, New mGender(Index), mTID, mSID)
            End Get
            Set(ByVal value As mTrainer)
                With value
                    ReDim mOTName(15)
                    Array.Copy(StringToPKMBytes(.Name, True), 0, mOTName, 0, StringToPKMBytes(.Name, True).Length)
                    mTID = .ID
                    mSID = .SID
                    Dim mMetLevelEtcBIN As String = DecToBin(mMetLevelEtc, 8)
                    If value.Gender.Value = Genders.Male Then
                        mMetLevelEtcBIN = "0" & mMetLevelEtcBIN.Substring(1, 7)
                    Else
                        mMetLevelEtcBIN = "1" & mMetLevelEtcBIN.Substring(1, 7)
                    End If
                    mMetLevelEtc = Convert.ToUInt16(mMetLevelEtcBIN, 2)
                End With
            End Set
        End Property

        Public Property Gender() As mGender
            Get
                If (mFormes And 4) Then Return New mGender(Genders.Genderless)
                If (mFormes And 2) Then Return New mGender(Genders.Female)
                Return New mGender(Genders.Male)
            End Get
            Set(ByVal value As mGender)
                Dim GenderSTRING As String = DecToBin(mFormes, 16).Substring(0, 13)
                Select Case value.Value
                    Case Genders.Male
                        GenderSTRING &= "000"
                    Case Genders.Female
                        GenderSTRING &= "010"
                    Case Genders.Genderless
                        GenderSTRING &= "100"
                End Select
                mFormes = Convert.ToUInt16(GenderSTRING, 2)
            End Set
        End Property

        Public Property Species() As mSpecies
            Get
                Dim _species As New mSpecies(mSpeciesID)
                Return _species
            End Get
            Set(ByVal value As mSpecies)
                mSpeciesID = value.ID
            End Set
        End Property

        Public Property Moves() As mMoves()
            Get
                InitializeDictionaries()
                Dim retMoves(3) As mMoves
                retMoves(0) = New mMoves
                retMoves(1) = New mMoves
                retMoves(2) = New mMoves
                retMoves(3) = New mMoves
                retMoves(0) = New mMoves(mMove1)
                If mMove2 <> 0 Then _
                retMoves(1) = New mMoves(mMove2)
                If mMove3 <> 0 Then _
                retMoves(2) = New mMoves(mMove3)
                If mMove4 <> 0 Then _
                retMoves(3) = New mMoves(mMove4)
                retMoves(0).CurrentPP = mM1PP
                retMoves(0).TotalPP = retMoves(0).BasePP + (retMoves(0).BasePP * 0.2 * mM1PPUp)
                retMoves(1).CurrentPP = mM2PP
                retMoves(1).TotalPP = retMoves(1).BasePP + (retMoves(1).BasePP * 0.2 * mM2PPUp)
                retMoves(2).CurrentPP = mM3PP
                retMoves(2).TotalPP = retMoves(2).BasePP + (retMoves(2).BasePP * 0.2 * mM3PPUp)
                retMoves(3).CurrentPP = mM4PP
                retMoves(3).TotalPP = retMoves(3).BasePP + (retMoves(3).BasePP * 0.2 * mM4PPUp)
                Return retMoves
            End Get
            Set(ByVal value As mMoves())
                mMove1 = value(0).Value
                mMove2 = value(1).Value
                mMove3 = value(2).Value
                mMove4 = value(3).Value
                mM1PP = value(0).CurrentPP
                mM2PP = value(1).CurrentPP
                mM3PP = value(2).CurrentPP
                mM4PP = value(3).CurrentPP
                If value(0).BasePP <> 1 Then mM1PPUp = (value(0).TotalPP - value(0).BasePP) / (value(0).BasePP * 0.2)
                If value(1).BasePP <> 1 Then mM2PPUp = (value(1).TotalPP - value(1).BasePP) / (value(1).BasePP * 0.2)
                If value(2).BasePP <> 1 Then mM3PPUp = (value(2).TotalPP - value(2).BasePP) / (value(2).BasePP * 0.2)
                If value(3).BasePP <> 1 Then mM4PPUp = (value(3).TotalPP - value(3).BasePP) / (value(3).BasePP * 0.2)
            End Set
        End Property

        Public Property Level() As Byte
            Get
                InitializeDictionaries()
                For i As Integer = 0 To 99
                    If EXP < LevelTable(i, BaseStats.GrowthGroup) Then
                        mLevel = i
                        Return i
                    End If
                Next
                mLevel = 100
                Return 100
            End Get
            Set(ByVal value As Byte)
                If value < 1 Then value = 1
                If value > 100 Then value = 100
                EXP = LevelTable(value - 1, BaseStats.GrowthGroup)
                mLevel = value
            End Set
        End Property

        Public ReadOnly Property TNL() As UInt32
            Get
                InitializeDictionaries()
                If Level = 100 Then Return 0
                Return LevelTable(Level, BaseStats.GrowthGroup) - EXP
            End Get
        End Property

        Public ReadOnly Property TNLPercent() As Decimal
            Get
                If Level = 100 Then Return 0
                If EXP = 0 Then Return 0
                Dim Max As UInteger = LevelTable(Level, BaseStats.GrowthGroup) - LevelTable(Level - 1, BaseStats.GrowthGroup)
                Return TNL / Max
            End Get
        End Property

        Public ReadOnly Property Nature() As mNatures
            Get
                Nature = New mNatures(PID)
            End Get
        End Property

        Public Property Item() As mItems
            Get
                PokemonLib.InitializeDictionaries()
                Dim _item As New mItems(mItem)
                Return _item
            End Get
            Set(ByVal value As mItems)
                mItem = value.Value
                If Species.ID = PokemonLib.Species.Arceus Then
                    Select Case value.Value
                        Case Items.Flame_Plate
                            Forme = ArceusFormes.Flame
                        Case Items.Meadow_Plate
                            Forme = ArceusFormes.Meadow
                        Case Items.Insect_Plate
                            Forme = ArceusFormes.Insect
                        Case Items.Spooky_Plate
                            Forme = ArceusFormes.Spooky
                        Case Items.Earth_Plate
                            Forme = ArceusFormes.Earth
                        Case Items.Stone_Plate
                            Forme = ArceusFormes.Stone
                        Case Items.Draco_Plate
                            Forme = ArceusFormes.Draco
                        Case Items.Toxic_Plate
                            Forme = ArceusFormes.Toxic
                        Case Items.Sky_Plate
                            Forme = ArceusFormes.Sky
                        Case Items.Splash_Plate
                            Forme = ArceusFormes.Splash
                        Case Items.Zap_Plate
                            Forme = ArceusFormes.Zap
                        Case Items.Mind_Plate
                            Forme = ArceusFormes.Mind
                        Case Items.Dread_Plate
                            Forme = ArceusFormes.Dread
                        Case Items.Fist_Plate
                            Forme = ArceusFormes.Fist
                        Case Items.Iron_Plate
                            Forme = ArceusFormes.Iron
                        Case Items.Icicle_Plate
                            Forme = ArceusFormes.Icicle
                        Case Else
                            Forme = ArceusFormes.Normal
                    End Select
                End If
                If Species.ID = PokemonLib.Species.Giratina Then
                    If value.Value = Items.Griseous_Orb Then
                        Forme = GiratinaFormes.Origin
                    Else
                        Forme = GiratinaFormes.Altered
                    End If
                End If
            End Set
        End Property

        Public Property Ball() As mItems
            Get
                PokemonLib.InitializeDictionaries()
                Dim _ball As New mItems(mBall)
                Return _ball
            End Get
            Set(ByVal value As mItems)
                mBall = value.Value
            End Set
        End Property

        Public Property Markings(ByVal ActiveColor As Color, ByVal InactiveColor As Color) As _mMarkings 'BitArray
            Get
                Dim MOUT As New _mMarkings
                MOUT.Circle.Name = "Circle"
                MOUT.Circle.Image = My.Resources.Circle
                MOUT.Circle.Image.MakeTransparent(Color.Black)
                MOUT.Triangle.Name = "Triangle"
                MOUT.Triangle.Image = My.Resources.Triangle
                MOUT.Triangle.Image.MakeTransparent(Color.Black)
                MOUT.Heart.Name = "Heart"
                MOUT.Heart.Image = My.Resources.Heart
                MOUT.Heart.Image.MakeTransparent(Color.Black)
                MOUT.Square.Name = "Square"
                MOUT.Square.Image = My.Resources.Square
                MOUT.Square.Image.MakeTransparent(Color.Black)
                MOUT.Star.Name = "Star"
                MOUT.Star.Image = My.Resources.Star
                MOUT.Star.Image.MakeTransparent(Color.Black)
                MOUT.Diamond.Name = "Diamond"
                MOUT.Diamond.Image = My.Resources.Diamond
                MOUT.Diamond.Image.MakeTransparent(Color.Black)
                Dim ba As BitArray = New BitArray(System.BitConverter.GetBytes(mMarkings))
                ba.Length = 6

                If ba(0) Then
                    MOUT.Circle.Value = True
                    For x As Integer = 0 To MOUT.Circle.Image.Width - 1
                        For y As Integer = 0 To MOUT.Circle.Image.Height - 1
                            If MOUT.Circle.Image.GetPixel(x, y) = MarkOriginal Then _
                            MOUT.Circle.Image.SetPixel(x, y, ActiveColor)
                        Next
                    Next
                Else
                    For x As Integer = 0 To MOUT.Circle.Image.Width - 1
                        For y As Integer = 0 To MOUT.Circle.Image.Height - 1
                            If MOUT.Circle.Image.GetPixel(x, y) = MarkOriginal Then _
                            MOUT.Circle.Image.SetPixel(x, y, InactiveColor)
                        Next
                    Next
                End If

                If ba(1) Then
                    MOUT.Triangle.Value = True
                    For x As Integer = 0 To MOUT.Triangle.Image.Width - 1
                        For y As Integer = 0 To MOUT.Triangle.Image.Height - 1
                            If MOUT.Triangle.Image.GetPixel(x, y) = MarkOriginal Then _
                            MOUT.Triangle.Image.SetPixel(x, y, ActiveColor)
                        Next
                    Next
                Else
                    For x As Integer = 0 To MOUT.Triangle.Image.Width - 1
                        For y As Integer = 0 To MOUT.Triangle.Image.Height - 1
                            If MOUT.Triangle.Image.GetPixel(x, y) = MarkOriginal Then _
                            MOUT.Triangle.Image.SetPixel(x, y, InactiveColor)
                        Next
                    Next
                End If

                If ba(2) Then
                    MOUT.Square.Value = True
                    For x As Integer = 0 To MOUT.Square.Image.Width - 1
                        For y As Integer = 0 To MOUT.Square.Image.Height - 1
                            If MOUT.Square.Image.GetPixel(x, y) = MarkOriginal Then _
                            MOUT.Square.Image.SetPixel(x, y, ActiveColor)
                        Next
                    Next
                Else
                    For x As Integer = 0 To MOUT.Square.Image.Width - 1
                        For y As Integer = 0 To MOUT.Square.Image.Height - 1
                            If MOUT.Square.Image.GetPixel(x, y) = MarkOriginal Then _
                            MOUT.Square.Image.SetPixel(x, y, InactiveColor)
                        Next
                    Next
                End If

                If ba(3) Then
                    MOUT.Heart.Value = True
                    For x As Integer = 0 To MOUT.Heart.Image.Width - 1
                        For y As Integer = 0 To MOUT.Heart.Image.Height - 1
                            If MOUT.Heart.Image.GetPixel(x, y) = MarkOriginal Then _
                            MOUT.Heart.Image.SetPixel(x, y, ActiveColor)
                        Next
                    Next
                Else
                    For x As Integer = 0 To MOUT.Heart.Image.Width - 1
                        For y As Integer = 0 To MOUT.Heart.Image.Height - 1
                            If MOUT.Heart.Image.GetPixel(x, y) = MarkOriginal Then _
                            MOUT.Heart.Image.SetPixel(x, y, InactiveColor)
                        Next
                    Next
                End If

                If ba(4) Then
                    MOUT.Star.Value = True
                    For x As Integer = 0 To MOUT.Star.Image.Width - 1
                        For y As Integer = 0 To MOUT.Star.Image.Height - 1
                            If MOUT.Star.Image.GetPixel(x, y) = MarkOriginal Then _
                            MOUT.Star.Image.SetPixel(x, y, ActiveColor)
                        Next
                    Next
                Else
                    For x As Integer = 0 To MOUT.Star.Image.Width - 1
                        For y As Integer = 0 To MOUT.Star.Image.Height - 1
                            If MOUT.Star.Image.GetPixel(x, y) = MarkOriginal Then _
                            MOUT.Star.Image.SetPixel(x, y, InactiveColor)
                        Next
                    Next
                End If

                If ba(5) Then
                    MOUT.Diamond.Value = True
                    For x As Integer = 0 To MOUT.Diamond.Image.Width - 1
                        For y As Integer = 0 To MOUT.Diamond.Image.Height - 1
                            If MOUT.Diamond.Image.GetPixel(x, y) = MarkOriginal Then _
                            MOUT.Diamond.Image.SetPixel(x, y, ActiveColor)
                        Next
                    Next
                Else
                    For x As Integer = 0 To MOUT.Diamond.Image.Width - 1
                        For y As Integer = 0 To MOUT.Diamond.Image.Height - 1
                            If MOUT.Diamond.Image.GetPixel(x, y) = MarkOriginal Then _
                            MOUT.Diamond.Image.SetPixel(x, y, InactiveColor)
                        Next
                    Next
                End If
                Return MOUT
            End Get
            Set(ByVal value As _mMarkings)
                'mMarkings
                Dim mMarkingsBIN As String = DecToBin(mMarkings, 8).Substring(0, 2)
                With value
                    If .Circle.Value Then
                        mMarkingsBIN &= "1"
                    Else
                        mMarkingsBIN &= "0"
                    End If
                    If .Triangle.Value Then
                        mMarkingsBIN &= "1"
                    Else
                        mMarkingsBIN &= "0"
                    End If
                    If .Square.Value Then
                        mMarkingsBIN &= "1"
                    Else
                        mMarkingsBIN &= "0"
                    End If
                    If .Heart.Value Then
                        mMarkingsBIN &= "1"
                    Else
                        mMarkingsBIN &= "0"
                    End If
                    If .Star.Value Then
                        mMarkingsBIN &= "1"
                    Else
                        mMarkingsBIN &= "0"
                    End If
                    If .Diamond.Value Then
                        mMarkingsBIN &= "1"
                    Else
                        mMarkingsBIN &= "0"
                    End If
                End With
                mMarkings = Convert.ToByte(mMarkingsBIN, 2)
            End Set
        End Property

        Public Property Condition() As mContestStats
            Get
                Return New mContestStats(mCool, mBeauty, mCute, mSmart, mTough, mSheen)
            End Get
            Set(ByVal value As mContestStats)
                With value
                    mCool = .Cool
                    mBeauty = .Beauty
                    mCute = .Cute
                    mSmart = .Smart
                    mTough = .Tough
                    mSheen = .Sheen
                End With
            End Set
        End Property

        Public ReadOnly Property BaseStats() As mBaseStats
            Get
                Return New mBaseStats(Species.ID, Forme)
            End Get
        End Property

        Public Property IVs() As mIVs
            Get
                IVs = New mIVs
                With IVs
                    .HP = (mIVsAndEtc >> (0)) And &H1F
                    .Attack = (mIVsAndEtc >> (5)) And &H1F
                    .Defense = (mIVsAndEtc >> (10)) And &H1F
                    .Speed = (mIVsAndEtc >> (15)) And &H1F
                    .SpAttack = (mIVsAndEtc >> (20)) And &H1F
                    .SpDefense = (mIVsAndEtc >> (25)) And &H1F
                End With
            End Get
            Set(ByVal newIVs As mIVs)
                Dim retString As String = ""
                If Nicknamed Then
                    retString &= "1"
                Else
                    retString &= "0"
                End If
                If IsEgg Then
                    retString &= "1"
                Else
                    retString &= "0"
                End If
                With newIVs
                    retString &= DecToBin(.SpDefense, 5).PadLeft(5, "0")
                    retString &= DecToBin(.SpAttack, 5).PadLeft(5, "0")
                    retString &= DecToBin(.Speed, 5).PadLeft(5, "0")
                    retString &= DecToBin(.Defense, 5).PadLeft(5, "0")
                    retString &= DecToBin(.Attack, 5).PadLeft(5, "0")
                    retString &= DecToBin(.HP, 5).PadLeft(5, "0")
                End With
                mIVsAndEtc = Convert.ToUInt32(retString, 2)
            End Set
        End Property

        Public Property BattleStats() As mBattleStats
            Get
                InitializeDictionaries()
                BattleStats = New mBattleStats
                With BattleStats
                    Dim Num As Integer = 10 + Level
                    .Calculated.MaxHP = _
                     Math.Floor((Math.Floor((BaseStats.HP * 2 + IVs.HP + _
                                             Math.Floor(HPEV / 4)) * Level / 100) + _
                                            Num) * dpNatures(Nature.Value)(0))
                    .Calculated.CurrentHP = .Calculated.MaxHP
                    Num = 5
                    .Calculated.Attack = _
                     Math.Floor((Math.Floor((BaseStats.Attack * 2 + IVs.Attack + _
                                             Math.Floor(AtkEV / 4)) * Level / 100) + _
                                            Num) * dpNatures(Nature.Value)(1))
                    .Calculated.Defense = _
                     Math.Floor((Math.Floor((BaseStats.Defense * 2 + IVs.Defense + _
                                             Math.Floor(DefEV / 4)) * Level / 100) + _
                                            Num) * dpNatures(Nature.Value)(2))
                    .Calculated.Speed = _
                     Math.Floor((Math.Floor((BaseStats.Speed * 2 + IVs.Speed + _
                                             Math.Floor(SpdEV / 4)) * Level / 100) + _
                                            Num) * dpNatures(Nature.Value)(3))
                    .Calculated.SpAttack = _
                     Math.Floor((Math.Floor((BaseStats.SpAttack * 2 + IVs.SpAttack + _
                                             Math.Floor(SPAtkEV / 4)) * Level / 100) + _
                                            Num) * dpNatures(Nature.Value)(4))
                    .Calculated.SpDefense = _
                     Math.Floor((Math.Floor((BaseStats.SpDefense * 2 + IVs.SpDefense + _
                                             Math.Floor(SPDefEV / 4)) * Level / 100) + _
                                            Num) * dpNatures(Nature.Value)(5))

                    .Current.CurrentHP = mCurHP
                    .Current.MaxHP = mMaxHP
                    .Current.Attack = mAtk
                    .Current.Defense = mDef
                    .Current.Speed = mSpd
                    .Current.SpAttack = mSpAtk
                    .Current.SpDefense = mSpDef
                End With
            End Get
            Set(ByVal value As mBattleStats)
                With value.Current
                    mCurHP = .CurrentHP
                    mMaxHP = .MaxHP
                    mAtk = .Attack
                    mDef = .Defense
                    mSpd = .Speed
                    mSpAtk = .SpAttack
                    mSpDef = .SpDefense
                End With
            End Set
        End Property

        Public ReadOnly Property Flavors() As mFlavors
            Get
                Return New mFlavors(Nature)
            End Get
        End Property

        Public ReadOnly Property Shiny() As Boolean
            Get
                Dim p1 As UInt16 = BitConverter.ToUInt16(BitConverter.GetBytes(PID), 0)
                Dim p2 As UInt16 = BitConverter.ToUInt16(BitConverter.GetBytes(PID), 2)
                Dim E As UInteger = Trainer.ID Xor Trainer.SID
                Dim F As UInteger = Val(p1) Xor Val(p2)
                Return (E Xor F) < 8
            End Get
        End Property

        Public ReadOnly Property HiddenPower() As mMoves
            Get
                Dim RetMove As New mMoves(237)
                With RetMove
                    Dim u, v, w, x, y, z As Integer
                    If IVs.HP Mod 4 = 2 Or IVs.HP Mod 4 = 3 Then u = 1
                    If IVs.Attack Mod 4 = 2 Or IVs.Attack Mod 4 = 3 Then v = 1
                    If IVs.Defense Mod 4 = 2 Or IVs.Defense Mod 4 = 3 Then w = 1
                    If IVs.Speed Mod 4 = 2 Or IVs.Speed Mod 4 = 3 Then x = 1
                    If IVs.SpAttack Mod 4 = 2 Or IVs.SpAttack Mod 4 = 3 Then y = 1
                    If IVs.SpDefense Mod 4 = 2 Or IVs.SpDefense Mod 4 = 3 Then z = 1
                    .Power = Math.Floor(((u + 2 * v + 4 * w + 8 * x + 16 * y + 32 * z) _
                                * 40 / 63) + 30)
                    Dim a, b, c, d, e, f, TypeNum As Integer
                    a = Math.Abs(Val(Not (IVs.HP Mod 2 = 0)))
                    b = Math.Abs(Val(Not (IVs.Attack Mod 2 = 0)))
                    c = Math.Abs(Val(Not (IVs.Defense Mod 2 = 0)))
                    d = Math.Abs(Val(Not (IVs.Speed Mod 2 = 0)))
                    e = Math.Abs(Val(Not (IVs.SpAttack Mod 2 = 0)))
                    f = Math.Abs(Val(Not (IVs.SpDefense Mod 2 = 0)))
                    TypeNum = Math.Floor((a + 2 * b + 4 * c + 8 * _
                                         d + 16 * e + 32 * f) * 15 / 63)
                    Select Case TypeNum
                        Case 0
                            .Type = New mType(Types.Fighting)
                        Case 1
                            .Type = New mType(Types.Flying)
                        Case 2
                            .Type = New mType(Types.Poison)
                        Case 3
                            .Type = New mType(Types.Ground)
                        Case 4
                            .Type = New mType(Types.Rock)
                        Case 5
                            .Type = New mType(Types.Bug)
                        Case 6
                            .Type = New mType(Types.Ghost)
                        Case 7
                            .Type = New mType(Types.Steel)
                        Case 8
                            .Type = New mType(Types.Fire)
                        Case 9
                            .Type = New mType(Types.Water)
                        Case 10
                            .Type = New mType(Types.Grass)
                        Case 11
                            .Type = New mType(Types.Electric)
                        Case 12
                            .Type = New mType(Types.Psychic)
                        Case 13
                            .Type = New mType(Types.Ice)
                        Case 14
                            .Type = New mType(Types.Dragon)
                        Case 15
                            .Type = New mType(Types.Dark)
                    End Select
                End With
                Return RetMove
            End Get
        End Property

        Public Property Origins() As mOrigins
            Get
                InitializeDictionaries()
                Origins = New mOrigins
                With Origins
                    .Country.Value = mCountry
                    .Hometown.Value = mHometown
                    .Level = ((mMetLevelEtc << 1) And &HFF) >> 1
                    .Location.DiamondPearl.Value = mDPMet
                    .Location.Platinum.Value = mPtMet
                    Try
                        .DateMet = New Date(2000 + mMetDate(0), mMetDate(1), mMetDate(2))
                        .Egg.EggDate = New Date(2000 + mEggDate(0), mEggDate(1), mEggDate(2))
                    Catch When 5
                    End Try
                    .Egg.Location.DiamondPearl.Value = mDPEggMet
                    .Egg.Location.Platinum.Value = mPlatEggMet
                End With
            End Get
            Set(ByVal value As mOrigins)
                With value
                    mCountry = .Country.Value
                    mHometown = .Hometown.Value
                    mMetLevelEtc = CByte(Convert.ToUInt16(DecToBin(mMetLevelEtc, 8).Substring(0, 1) & DecToBin(.Level, 8).Substring(1, 7), 2))
                    mDPMet = .Location.DiamondPearl.Value
                    mPtMet = .Location.Platinum.Value
                    ReDim mMetDate(2)
                    ReDim mEggDate(2)
                    mMetDate(0) = .DateMet.Year - 2000
                    mMetDate(1) = .DateMet.Month
                    mMetDate(2) = .DateMet.Day
                    If .Egg.EggDate = Nothing Then
                        mEggDate(0) = 0
                        mEggDate(1) = 0
                        mEggDate(2) = 0
                    Else
                        mEggDate(0) = .Egg.EggDate.Year - 2000
                        mEggDate(1) = .Egg.EggDate.Month
                        mEggDate(2) = .Egg.EggDate.Day
                    End If
                    mDPEggMet = .Egg.Location.DiamondPearl.Value
                    mPlatEggMet = .Egg.Location.Platinum.Value
                End With
            End Set
        End Property

        Public ReadOnly Property Characteristic() As String
            Get
                Dim IVArray(5) As Byte
                With IVs
                    IVArray(0) = .HP
                    IVArray(1) = .Attack
                    IVArray(2) = .Defense
                    IVArray(3) = .Speed
                    IVArray(4) = .SpAttack
                    IVArray(5) = .SpDefense
                End With
                Dim IVABuf(5) As Byte
                Array.Copy(IVArray, 0, IVABuf, 0, 6)
                Array.Sort(IVABuf)
                Dim HighValue As Byte = IVABuf.Last
                Dim TiedIVs As New List(Of Byte)
                For i As Integer = 0 To 5
                    If IVArray(i) = HighValue Then TiedIVs.Add(i)
                Next
                If TiedIVs.Count = 1 Then Return (dpPKMCharacterstics(HighValue Mod 5)(TiedIVs(0)))
                Dim PIDMOD As Integer = PID Mod 6
                For i As Integer = 0 To 5
                    If IVArray(i) = HighValue And PIDMOD = i And TiedIVs.Contains(PIDMOD) Then Return dpPKMCharacterstics(HighValue Mod 5)(i)
                Next
                PIDMOD += 1
                If PIDMOD = 6 Then PIDMOD = 0
                For i As Integer = 0 To 5
                    If IVArray(i) = HighValue And PIDMOD = i And TiedIVs.Contains(PIDMOD) Then Return dpPKMCharacterstics(HighValue Mod 5)(i)
                Next
                PIDMOD += 1
                If PIDMOD = 6 Then PIDMOD = 0
                For i As Integer = 0 To 5
                    If IVArray(i) = HighValue And PIDMOD = i And TiedIVs.Contains(PIDMOD) Then Return dpPKMCharacterstics(HighValue Mod 5)(i)
                Next
                PIDMOD += 1
                If PIDMOD = 6 Then PIDMOD = 0
                For i As Integer = 0 To 5
                    If IVArray(i) = HighValue And PIDMOD = i And TiedIVs.Contains(PIDMOD) Then Return dpPKMCharacterstics(HighValue Mod 5)(i)
                Next
                PIDMOD += 1
                If PIDMOD = 6 Then PIDMOD = 0
                For i As Integer = 0 To 5
                    If IVArray(i) = HighValue And PIDMOD = i And TiedIVs.Contains(PIDMOD) Then Return dpPKMCharacterstics(HighValue Mod 5)(i)
                Next
                PIDMOD += 1
                If PIDMOD = 6 Then PIDMOD = 0
                For i As Integer = 0 To 5
                    If IVArray(i) = HighValue And PIDMOD = i And TiedIVs.Contains(PIDMOD) Then Return dpPKMCharacterstics(HighValue Mod 5)(i)
                Next
                Return ""
            End Get
        End Property

        Public Property Ability() As mAbility
            Get
                Return New mAbility(mmAbility)
            End Get
            Set(ByVal value As mAbility)
                mmAbility = value.Value
            End Set
        End Property

        Public Property EVs() As mEVs
            Get
                Return New mEVs(HPEV, AtkEV, DefEV, SpdEV, SPAtkEV, SPDefEV)
            End Get
            Set(ByVal value As mEVs)
                With value
                    HPEV = .HP
                    AtkEV = .Attack
                    DefEV = .Defense
                    SpdEV = .Speed
                    SPAtkEV = .SpAttack
                    SPDefEV = .SpDefense
                End With
            End Set
        End Property

        Public Property FatefulEncounter() As Boolean
            Get
                Dim BA As New BitArray(BitConverter.GetBytes(mFormes))
                Return BA(0)
            End Get
            Set(ByVal value As Boolean)
                Dim mFormesBIN As String = DecToBin(mFormes, 16)
                If value Then
                    mFormesBIN = mFormesBIN.Substring(0, 15) & "1"
                Else
                    mFormesBIN = mFormesBIN.Substring(0, 15) & "0"
                End If
                mFormes = Convert.ToUInt16(mFormesBIN, 2)
            End Set
        End Property

        Public Property Nicknamed() As Boolean
            Get
                Dim ba As New BitArray(BitConverter.GetBytes(mIVsAndEtc))
                Return ba(31)
            End Get
            Set(ByVal value As Boolean)
                Dim mIVsAndEtcBIN As String = DecToBin(mIVsAndEtc, 32)
                If value Then
                    mIVsAndEtcBIN = "1" & mIVsAndEtcBIN.Substring(1, 31)
                Else
                    mIVsAndEtcBIN = "0" & mIVsAndEtcBIN.Substring(1, 31)
                End If
                mIVsAndEtc = Convert.ToUInt32(mIVsAndEtcBIN, 2)
            End Set
        End Property

        Public Property IsEgg() As Boolean
            Get
                Dim ba As New BitArray(BitConverter.GetBytes(mIVsAndEtc))
                Return ba(30)
            End Get
            Set(ByVal value As Boolean)
                Dim mIVsAndEtcBIN As String = DecToBin(mIVsAndEtc, 32)
                If value Then
                    mIVsAndEtcBIN = mIVsAndEtcBIN.Substring(0, 1) & "1" & mIVsAndEtcBIN.Substring(2, 30)
                Else
                    mIVsAndEtcBIN = mIVsAndEtcBIN.Substring(0, 1) & "0" & mIVsAndEtcBIN.Substring(2, 30)
                End If
                mIVsAndEtc = Convert.ToUInt32(mIVsAndEtcBIN, 2)
            End Set
        End Property

        Public Property Status() As mStatus
            Get
                Return New mStatus(mAilment)
            End Get
            Set(ByVal value As mStatus)
                If value.Asleep And value.SleepRounds = 0 Then value.SleepRounds = 1
                If value.SleepRounds > 0 Then value.Asleep = True
                mAilment = value.SleepRounds
                SetBit(mAilment, StatusAilments.Burn, value.Burned)
                SetBit(mAilment, StatusAilments.Freeze, value.Frozen)
                SetBit(mAilment, StatusAilments.Paralysis, value.Paralyzed)
                SetBit(mAilment, StatusAilments.Poison, value.Poisoned)
                SetBit(mAilment, StatusAilments.Toxic, value.Toxic)
            End Set
        End Property

        Public ReadOnly Property Ribbons() As mRibbons
            Get
                Dim ribOut As New mRibbons
                ribOut.Names = New List(Of String)
                ribOut.Images = New List(Of Bitmap)
                Dim S1 As BitArray = New BitArray(BitConverter.GetBytes(mSRibbon1))
                Dim S2 As BitArray = New BitArray(BitConverter.GetBytes(mSRibbon2))
                Dim S3 As BitArray = New BitArray(BitConverter.GetBytes(mSRibbon3))
                Dim S4 As BitArray = New BitArray(BitConverter.GetBytes(mSRibbon4))
                Dim H1 As BitArray = New BitArray(BitConverter.GetBytes(mHRibbon1))
                Dim H2 As BitArray = New BitArray(BitConverter.GetBytes(mHRibbon2))
                With ribOut
                    If H2(4) Then
                        .Names.Add("Champion Ribbon")
                        .Images.Add(My.Resources.champion)
                    End If
                    If H1(0) Then
                        .Names.Add("Cool Ribbon")
                        .Images.Add(My.Resources.cool_h)
                    End If
                    If H1(1) Then
                        .Names.Add("Cool Ribbon Super")
                        .Images.Add(My.Resources.cool_super_h)
                    End If
                    If H1(2) Then
                        .Names.Add("Cool Ribbon Hyper")
                        .Images.Add(My.Resources.cool_hyper_h)
                    End If
                    If H1(3) Then
                        .Names.Add("Cool Ribbon Master")
                        .Images.Add(My.Resources.cool_master_h)
                    End If
                    If H1(4) Then
                        .Names.Add("Beauty Ribbon")
                        .Images.Add(My.Resources.beauty_h)
                    End If
                    If H1(5) Then
                        .Names.Add("Beauty Ribbon Super")
                        .Images.Add(My.Resources.beauty_super_h)
                    End If
                    If H1(6) Then
                        .Names.Add("Beauty Ribbon Hyper")
                        .Images.Add(My.Resources.beauty_hyper_h)
                    End If
                    If H1(7) Then
                        .Names.Add("Beauty Ribbon Master")
                        .Images.Add(My.Resources.beauty_master_h)
                    End If
                    If H1(8) Then
                        .Names.Add("Cute Ribbon")
                        .Images.Add(My.Resources.cute_h)
                    End If
                    If H1(9) Then
                        .Names.Add("Cute Ribbon Super")
                        .Images.Add(My.Resources.cute_super_h)
                    End If
                    If H1(10) Then
                        .Names.Add("Cute Ribbon Hyper")
                        .Images.Add(My.Resources.cute_hyper_h)
                    End If
                    If H1(11) Then
                        .Names.Add("Cute Ribbon Master")
                        .Images.Add(My.Resources.cute_master_h)
                    End If
                    If H1(12) Then
                        .Names.Add("Smart Ribbon")
                        .Images.Add(My.Resources.smart_h)
                    End If
                    If H1(13) Then
                        .Names.Add("Smart Ribbon Super")
                        .Images.Add(My.Resources.smart_super_h)
                    End If
                    If H1(14) Then
                        .Names.Add("Smart Ribbon Hyper")
                        .Images.Add(My.Resources.smart_hyper_h)
                    End If
                    If H1(15) Then
                        .Names.Add("Smart Ribbon Master")
                        .Images.Add(My.Resources.smart_master_h)
                    End If
                    If H2(0) Then
                        .Names.Add("Tough Ribbon")
                        .Images.Add(My.Resources.tough_h)
                    End If
                    If H2(1) Then
                        .Names.Add("Tough Ribbon Super")
                        .Images.Add(My.Resources.tough_super_h)
                    End If
                    If H2(2) Then
                        .Names.Add("Tough Ribbon Hyper")
                        .Images.Add(My.Resources.tough_hyper_h)
                    End If
                    If H2(3) Then
                        .Names.Add("Tough Ribbon Master")
                        .Images.Add(My.Resources.tough_master_h)
                    End If
                    If H2(5) Then
                        .Names.Add("Winning Ribbon")
                        .Images.Add(My.Resources.winning)
                    End If
                    If H2(6) Then
                        .Names.Add("Victory Ribbon")
                        .Images.Add(My.Resources.victory)
                    End If
                    If H2(7) Then
                        .Names.Add("Artist Ribbon")
                        .Images.Add(My.Resources.artist)
                    End If
                    If H2(8) Then
                        .Names.Add("Effort Ribbon")
                        .Images.Add(My.Resources.effort)
                    End If
                    If H2(9) Then
                        .Names.Add("Marine Ribbon")
                        .Images.Add(My.Resources.marine)
                    End If
                    If H2(10) Then
                        .Names.Add("Land Ribbon")
                        .Images.Add(My.Resources.land)
                    End If
                    If H2(11) Then
                        .Names.Add("Sky Ribbon")
                        .Images.Add(My.Resources.sky)
                    End If
                    If H2(12) Then
                        .Names.Add("Country Ribbon")
                        .Images.Add(My.Resources.country)
                    End If
                    If H2(13) Then
                        .Names.Add("National Ribbon")
                        .Images.Add(My.Resources.national)
                    End If
                    If H2(14) Then
                        .Names.Add("Earth Ribbon")
                        .Images.Add(My.Resources.earth)
                    End If
                    If H2(15) Then
                        .Names.Add("World Ribbon")
                        .Images.Add(My.Resources.world)
                    End If
                    If S1(0) Then
                        .Names.Add("Sinnoh Champ Ribbon")
                        .Images.Add(My.Resources.sinnoh_champ)
                    End If
                    If S3(0) Then
                        .Names.Add("Cool Ribbon")
                        .Images.Add(My.Resources.coolR)
                    End If
                    If S3(1) Then
                        .Names.Add("Cool Ribbon Great")
                        .Images.Add(My.Resources.cool_super)
                    End If
                    If S3(2) Then
                        .Names.Add("Cool Ribbon Ultra")
                        .Images.Add(My.Resources.cool_hyper)
                    End If
                    If S3(3) Then
                        .Names.Add("Cool Ribbon Master")
                        .Images.Add(My.Resources.cool_master)
                    End If
                    If S3(4) Then
                        .Names.Add("Beauty Ribbon")
                        .Images.Add(My.Resources.beautyR)
                    End If
                    If S3(5) Then
                        .Names.Add("Beauty Ribbon Great")
                        .Images.Add(My.Resources.beauty_super)
                    End If
                    If S3(6) Then
                        .Names.Add("Beauty Ribbon Ultra")
                        .Images.Add(My.Resources.beauty_hyper)
                    End If
                    If S3(7) Then
                        .Names.Add("Beauty Ribbon Master")
                        .Images.Add(My.Resources.beauty_master)
                    End If
                    If S3(8) Then
                        .Names.Add("Cute Ribbon")
                        .Images.Add(My.Resources.cuteR)
                    End If
                    If S3(9) Then
                        .Names.Add("Cute Ribbon Great")
                        .Images.Add(My.Resources.cute_super)
                    End If
                    If S3(10) Then
                        .Names.Add("Cute Ribbon Ultra")
                        .Images.Add(My.Resources.cute_hyper)
                    End If
                    If S3(11) Then
                        .Names.Add("Cute Ribbon Master")
                        .Images.Add(My.Resources.cute_master)
                    End If
                    If S3(12) Then
                        .Names.Add("Smart Ribbon")
                        .Images.Add(My.Resources.smartR)
                    End If
                    If S3(13) Then
                        .Names.Add("Smart Ribbon Great")
                        .Images.Add(My.Resources.smart_super)
                    End If
                    If S3(14) Then
                        .Names.Add("Smart Ribbon Ultra")
                        .Images.Add(My.Resources.smart_hyper)
                    End If
                    If S3(15) Then
                        .Names.Add("Smart Ribbon Master")
                        .Images.Add(My.Resources.smart_master)
                    End If
                    If S4(0) Then
                        .Names.Add("Tough Ribbon")
                        .Images.Add(My.Resources.toughR)
                    End If
                    If S4(1) Then
                        .Names.Add("Tough Ribbon Great")
                        .Images.Add(My.Resources.tough_super)
                    End If
                    If S4(2) Then
                        .Names.Add("Tough Ribbon Ultra")
                        .Images.Add(My.Resources.tough_hyper)
                    End If
                    If S4(3) Then
                        .Names.Add("Tough Ribbon Master")
                        .Images.Add(My.Resources.tough_master)
                    End If
                    If S1(1) Then
                        .Names.Add("Ability Ribbon")
                        .Images.Add(My.Resources.alert)
                    End If
                    If S1(2) Then
                        .Names.Add("Great Ability Ribbon")
                        .Images.Add(My.Resources.great_ability)
                    End If
                    If S1(3) Then
                        .Names.Add("Double Ability Ribbon")
                        .Images.Add(My.Resources.double_ability)
                    End If
                    If S1(4) Then
                        .Names.Add("Multi Ability Ribbon")
                        .Images.Add(My.Resources.multi_ability)
                    End If
                    If S1(5) Then
                        .Names.Add("Pair Ability Ribbon")
                        .Images.Add(My.Resources.pair_ability)
                    End If
                    If S1(6) Then
                        .Names.Add("World Ability Ribbon")
                        .Images.Add(My.Resources.world_ability)
                    End If
                    If S1(7) Then
                        .Names.Add("Alert Ribbon")
                        .Images.Add(My.Resources.alert)
                    End If
                    If S1(8) Then
                        .Names.Add("Shock Ribbon")
                        .Images.Add(My.Resources.shock)
                    End If
                    If S1(9) Then
                        .Names.Add("Downcast Ribbon")
                        .Images.Add(My.Resources.downcast)
                    End If
                    If S1(10) Then
                        .Names.Add("Careless Ribbon")
                        .Images.Add(My.Resources.careless)
                    End If
                    If S1(11) Then
                        .Names.Add("Relax Ribbon")
                        .Images.Add(My.Resources.relax)
                    End If
                    If S1(12) Then
                        .Names.Add("Snooze Ribbon")
                        .Images.Add(My.Resources.snooze)
                    End If
                    If S1(13) Then
                        .Names.Add("Smile Ribbon")
                        .Images.Add(My.Resources.smile)
                    End If
                    If S1(14) Then
                        .Names.Add("Gorgeous Ribbon")
                        .Images.Add(My.Resources.gorgeous)
                    End If
                    If S1(15) Then
                        .Names.Add("Royal Ribbon")
                        .Images.Add(My.Resources.royal)
                    End If
                    If S2(0) Then
                        .Names.Add("Gorgeous Royal Ribbon")
                        .Images.Add(My.Resources.gorgeous_royal)
                    End If
                    If S2(1) Then
                        .Names.Add("Footprint Ribbon")
                        .Images.Add(My.Resources.footprint)
                    End If
                    If S2(2) Then
                        .Names.Add("Record Ribbon")
                        .Images.Add(My.Resources.record)
                    End If
                    If S2(3) Then
                        .Names.Add("History Ribbon")
                        .Images.Add(My.Resources.history)
                    End If
                    If S2(4) Then
                        .Names.Add("Legend Ribbon")
                        .Images.Add(My.Resources.legend)
                    End If
                    If S2(5) Then
                        .Names.Add("Red Ribbon")
                        .Images.Add(My.Resources.red)
                    End If
                    If S2(6) Then
                        .Names.Add("Green Ribbon")
                        .Images.Add(My.Resources.green)
                    End If
                    If S2(7) Then
                        .Names.Add("Blue Ribbon")
                        .Images.Add(My.Resources.blue)
                    End If
                    If S2(8) Then
                        .Names.Add("Festival Ribbon")
                        .Images.Add(My.Resources.festival)
                    End If
                    If S2(9) Then
                        .Names.Add("Carnival Ribbon")
                        .Images.Add(My.Resources.carnival)
                    End If
                    If S2(10) Then
                        .Names.Add("Classic Ribbon")
                        .Images.Add(My.Resources.classic)
                    End If
                    If S2(11) Then
                        .Names.Add("Premier Ribbon")
                        .Images.Add(My.Resources.premier)
                    End If
                End With
                Return ribOut
            End Get
        End Property

        Public Property Forme() As UInt16
            Get
                Return mFormes >> 3
            End Get
            Set(ByVal value As UInt16)
                Dim valBIN As String = DecToBin(value, 16)
                Dim mFormeBin As String = DecToBin(mFormes, 16)

                'MsgBox(valBIN.Substring(3, 13) & mFormeBin.Substring(13, 3))

                mFormes = Convert.ToUInt16(valBIN.Substring(3, 13) & mFormeBin.Substring(13, 3), 2)
                'For i As Integer = 16 To 14 Step -1
                '    SetBit(mFormes, i, ExamineBit(value, i))
                'Next
            End Set
        End Property

        Public ReadOnly Property BoxIcon() As Bitmap
            Get
                InitializeDictionaries()
                If IsEgg Then
                    If mSpeciesID = PokemonLib.Species.Manaphy Then
                        Return My.Resources.BoxManaphyEgg
                    Else
                        Return My.Resources.BoxEgg
                    End If
                Else
                    Select Case mSpeciesID
                        Case PokemonLib.Species.Unown
                            Return dpUnownBoxIcons(Forme)
                        Case PokemonLib.Species.Deoxys
                            Return dpDeoxysBoxIcons(Forme)
                        Case PokemonLib.Species.Burmy
                            Return dpBurmyBoxIcons(Forme)
                        Case PokemonLib.Species.Wormadam
                            Return dpWormadamBoxIcons(Forme)
                        Case PokemonLib.Species.Shellos
                            Return dpShellosBoxIcons(Forme)
                        Case PokemonLib.Species.Gastrodon
                            Return dpGastrodonBoxIcons(Forme)
                        Case PokemonLib.Species.Rotom
                            Return dpRotomBoxIcons(Forme)
                        Case PokemonLib.Species.Giratina
                            Return dpGiratinaBoxIcons(Forme)
                        Case PokemonLib.Species.Shaymin
                            Return dpShayminBoxIcons(Forme)
                        Case Else
                            Return dpBoxIcons(mSpeciesID)
                    End Select
                End If
                Return My.Resources._0000
            End Get
        End Property

        Public ReadOnly Property Sprite() As Bitmap
            Get
                InitializeDictionaries()
                Dim imgOUT As New Bitmap(80, 80)
                If IsEgg Then
                    If mSpeciesID = PokemonLib.Species.Manaphy Then
                        imgOUT = My.Resources.ManaphyEgg
                        imgOUT.MakeTransparent(SpriteBack)
                        Return imgOUT
                    Else
                        imgOUT = My.Resources.Egg
                        imgOUT.MakeTransparent(SpriteBack)
                        Return imgOUT
                    End If
                Else
                    Select Case mSpeciesID
                        Case PokemonLib.Species.Spinda
                            Return RenderSpindaSprite(PID, Shiny)
                        Case PokemonLib.Species.Unown
                            If Shiny Then
                                imgOUT = dpSUnownSprites(Forme)
                                imgOUT.MakeTransparent(SpriteBack)
                                Return imgOUT
                            Else
                                imgOUT = dpUnownSprites(Forme)
                                imgOUT.MakeTransparent(SpriteBack)
                                Return imgOUT
                            End If
                        Case PokemonLib.Species.Deoxys
                            If Shiny Then
                                imgOUT = dpSDeoxysSprites(Forme)
                                imgOUT.MakeTransparent(SpriteBack)
                                Return imgOUT
                            Else
                                imgOUT = dpDeoxysSprites(Forme)
                                imgOUT.MakeTransparent(SpriteBack)
                                Return imgOUT
                            End If
                        Case PokemonLib.Species.Burmy
                            If Shiny Then
                                imgOUT = dpSBurmySprites(Forme)
                                imgOUT.MakeTransparent(SpriteBack)
                                Return imgOUT
                            Else
                                imgOUT = dpBurmySprites(Forme)
                                imgOUT.MakeTransparent(SpriteBack)
                                Return imgOUT
                            End If
                        Case PokemonLib.Species.Wormadam
                            If Shiny Then
                                imgOUT = dpSWormadamSprites(Forme)
                                imgOUT.MakeTransparent(SpriteBack)
                                Return imgOUT
                            Else
                                imgOUT = dpWormadamSprites(Forme)
                                imgOUT.MakeTransparent(SpriteBack)
                                Return imgOUT
                            End If
                        Case PokemonLib.Species.Shellos
                            If Shiny Then
                                imgOUT = dpSShellosSprites(Forme)
                                imgOUT.MakeTransparent(SpriteBack)
                                Return imgOUT
                            Else
                                imgOUT = dpShellosSprites(Forme)
                                imgOUT.MakeTransparent(SpriteBack)
                                Return imgOUT
                            End If
                        Case PokemonLib.Species.Gastrodon
                            If Shiny Then
                                imgOUT = dpSGastrodonSprites(Forme)
                                imgOUT.MakeTransparent(SpriteBack)
                                Return imgOUT
                            Else
                                imgOUT = dpGastrodonSprites(Forme)
                                imgOUT.MakeTransparent(SpriteBack)
                                Return imgOUT
                            End If
                        Case PokemonLib.Species.Rotom
                            If Shiny Then
                                imgOUT = dpSRotomSprites(Forme)
                                imgOUT.MakeTransparent(SpriteBack)
                                Return imgOUT
                            Else
                                imgOUT = dpRotomSprites(Forme)
                                imgOUT.MakeTransparent(SpriteBack)
                                Return imgOUT
                            End If
                        Case PokemonLib.Species.Giratina
                            If Shiny Then
                                imgOUT = dpSGiratinaSprites(Forme)
                                imgOUT.MakeTransparent(SpriteBack)
                                Return imgOUT
                            Else
                                imgOUT = dpGiratinaSprites(Forme)
                                imgOUT.MakeTransparent(SpriteBack)
                                Return imgOUT
                            End If
                        Case PokemonLib.Species.Shaymin
                            If Shiny Then
                                imgOUT = dpSShayminSprites(Forme)
                                imgOUT.MakeTransparent(SpriteBack)
                                Return imgOUT
                            Else
                                imgOUT = dpShayminSprites(Forme)
                                imgOUT.MakeTransparent(SpriteBack)
                                Return imgOUT
                            End If
                        Case PokemonLib.Species.Arceus
                            If Shiny Then
                                imgOUT = dpSArceusSprites(Forme)
                                imgOUT.MakeTransparent(SpriteBack)
                                Return imgOUT
                            Else
                                imgOUT = dpArceusSprites(Forme)
                                imgOUT.MakeTransparent(SpriteBack)
                                Return imgOUT
                            End If
                        Case Else
                            If Shiny Then
                                If Gender.Value = Genders.Female Then
                                    imgOUT = dpSFSprites(mSpeciesID)
                                    imgOUT.MakeTransparent(SpriteBack)
                                    Return imgOUT
                                Else
                                    imgOUT = dpSMSprites(mSpeciesID)
                                    imgOUT.MakeTransparent(SpriteBack)
                                    Return imgOUT
                                End If
                            Else
                                If Gender.Value = Genders.Female Then
                                    imgOUT = dpNFSprites(mSpeciesID)
                                    imgOUT.MakeTransparent(SpriteBack)
                                    Return imgOUT
                                Else
                                    imgOUT = dpNMSprites(mSpeciesID)
                                    imgOUT.MakeTransparent(SpriteBack)
                                    Return imgOUT
                                End If
                            End If
                    End Select
                End If
                Return imgOUT
            End Get
        End Property

        Public ReadOnly Property TrainerMemo() As String
            Get
                TrainerMemo = ""
                If IsEgg Then
                    With Origins
                        If .Hometown.Value = Hometowns.Platinum Then

                        Else

                        End If
                    End With
                Else

                    TrainerMemo &= Nature.Name & " nature." & vbNewLine
                    With Origins
                        Dim MetArrive As String = "Met at Lv. "
                        If .Hometown.Value = Hometowns.Platinum Then
                            If .Egg.Location.Platinum.Value <> 0 Then
                                'Platinum egg
                                If .Egg.EggDate.Month = 5 Then
                                    TrainerMemo &= .Egg.EggDate.ToString("MMM d, yyyy") & vbNewLine
                                Else
                                    TrainerMemo &= .Egg.EggDate.ToString("MMM. d, yyyy") & vbNewLine
                                End If
                                If .Egg.Location.Platinum.Value = 2011 Then
                                    TrainerMemo &= "Cynthia" & vbNewLine
                                Else
                                    TrainerMemo &= .Egg.Location.Platinum.Name & vbNewLine
                                End If
                                TrainerMemo &= "Egg received." & vbNewLine
                                If .DateMet.Month = 5 Then
                                    TrainerMemo &= .DateMet.ToString("MMM d, yyyy") & vbNewLine
                                Else
                                    TrainerMemo &= .DateMet.ToString("MMM. d, yyyy") & vbNewLine
                                End If
                                TrainerMemo &= .Location.Platinum.Name & vbNewLine
                                TrainerMemo &= "Egg hatched." & vbNewLine
                            Else
                                'Platinum not egg
                                If .DateMet.Month = 5 Then
                                    TrainerMemo &= .DateMet.ToString("MMM d, yyyy") & vbNewLine
                                Else
                                    TrainerMemo &= .DateMet.ToString("MMM. d, yyyy") & vbNewLine
                                End If
                                If .Hometown.Value <> 10 And .Hometown.Value <> 11 And .Hometown.Value <> 12 Then
                                    If .Hometown.Value = 1 Or .Hometown.Value = 2 Or .Hometown.Value = 3 Then TrainerMemo &= "Hoenn" & vbNewLine
                                    If .Hometown.Value = 4 Or .Hometown.Value = 5 Then TrainerMemo &= "Kanto" & vbNewLine
                                    If .Hometown.Value = Hometowns.Gold Or .Hometown.Value = Hometowns.Silver Then TrainerMemo &= "Johto" & vbNewLine
                                    If .Hometown.Value = 15 Then TrainerMemo &= "Distant Land" & vbNewLine
                                    MetArrive = "Arrived at Lv. "
                                Else
                                    TrainerMemo &= .Location.Platinum.Name & vbNewLine
                                End If
                                If FatefulEncounter Then TrainerMemo &= "Fateful encounter." & vbNewLine
                                TrainerMemo &= MetArrive & .Level.ToString & vbNewLine
                            End If
                        Else
                            If .Egg.Location.DiamondPearl.Value <> 0 Then
                                'Diamond/Pearl egg
                                If .Egg.EggDate.Month = 5 Then
                                    TrainerMemo &= .Egg.EggDate.ToString("MMM d, yyyy") & vbNewLine
                                Else
                                    TrainerMemo &= .Egg.EggDate.ToString("MMM. d, yyyy") & vbNewLine
                                End If
                                TrainerMemo &= .Egg.Location.DiamondPearl.Name & vbNewLine
                                TrainerMemo &= "Egg received." & vbNewLine
                                TrainerMemo &= .DateMet.ToString("MMM. d, yyyy") & vbNewLine
                                TrainerMemo &= .Location.DiamondPearl.Name & vbNewLine
                                TrainerMemo &= "Egg hatched." & vbNewLine
                            Else
                                'Diamond/Pearl not egg
                                If .DateMet.Month = 5 Then
                                    TrainerMemo &= .DateMet.ToString("MMM d, yyyy") & vbNewLine
                                Else
                                    TrainerMemo &= .DateMet.ToString("MMM. d, yyyy") & vbNewLine
                                End If
                                If .Hometown.Value <> 10 And .Hometown.Value <> 11 And .Hometown.Value <> 12 Then
                                    If .Hometown.Value = 1 Or .Hometown.Value = 2 Or .Hometown.Value = 3 Then TrainerMemo &= "Hoenn" & vbNewLine
                                    If .Hometown.Value = 4 Or .Hometown.Value = 5 Then TrainerMemo &= "Kanto" & vbNewLine
                                    If .Hometown.Value = Hometowns.Gold Or .Hometown.Value = Hometowns.Silver Then TrainerMemo &= "Johto" & vbNewLine
                                    If .Hometown.Value = 15 Then TrainerMemo &= "Distant Land" & vbNewLine
                                    MetArrive = "Arrived at Lv. "
                                Else
                                    TrainerMemo &= .Location.DiamondPearl.Name & vbNewLine
                                End If
                                If FatefulEncounter Then TrainerMemo &= "Fateful encounter." & vbNewLine
                                TrainerMemo &= MetArrive & .Level.ToString & "." & vbNewLine
                            End If
                        End If
                    End With
                    TrainerMemo &= Characteristic & "." & vbNewLine
                    If Flavors.Likes.Contains("NOTHING") Then
                        TrainerMemo &= "Happily eats anything."
                    Else
                        TrainerMemo &= "Likes " & LCase(Flavors.Likes) & " food."
                    End If

                End If
            End Get
        End Property

        Public ReadOnly Property [Class]() As Byte
            Get
                Return PID Mod 2
            End Get
        End Property

        Public Property Encounter() As mEncounters
            Get
                Return New mEncounters(mEncounter)
            End Get
            Set(ByVal value As mEncounters)
                mEncounter = value.Value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function UpdateLevel() As Byte
            Return Level
        End Function

        Public Sub Export(ByVal FileName As String, Optional ByVal Encrypt As Boolean = False, Optional ByVal Size As PKM_Size_Formats = PKM_Size_Formats.Party)
            'Try
            Dim PPKK As New Pokemon
            PPKK = PC_to_Party(Me)
            Dim data() As Byte = RawSerialize(PPKK)
            Dim checksumbytes() As Byte = BitConverter.GetBytes(Calculate_Checksum(data))
            Array.Copy(checksumbytes, 0, data, 6, checksumbytes.Length)
            PPKK = RawDeserialize(data, PPKK.GetType)
            data = Nothing
            checksumbytes = Nothing
            If Size <> 136 And Size <> 236 Then
                Size = 236
            End If

            Dim fs As New System.IO.FileStream(FileName, FileMode.Create, FileAccess.Write)
            Try
                Using bW As New BinaryWriter(fs)
                    If Encrypt Then
                        bW.Write(EncryptPokemon(RawSerialize(PPKK)), 0, Size)
                    Else
                        bW.Write(RawSerialize(PPKK), 0, Size)
                    End If
                End Using
            Finally
                If fs IsNot Nothing Then
                    fs.Dispose()
                End If
            End Try
            'Catch ex As Exception
            '    Console.WriteLine("Error " & Err.Number & ": " & ex.Message)
            'End Try
        End Sub

        Public Sub SetMark(ByVal MarkToSet As Marks, ByVal State As Boolean)
            If State Then
                SetBit(mMarkings, MarkToSet)
            Else
                ClearBit(mMarkings, MarkToSet)
            End If
        End Sub

        Public Sub SetMarks(ByVal Circle As Boolean, ByVal Diamond As Boolean, _
                                    ByVal Heart As Boolean, ByVal Square As Boolean, _
                                    ByVal Star As Boolean, ByVal Triangle As Boolean)
            SetMark(Marks.Circle, Circle)
            SetMark(Marks.Diamond, Diamond)
            SetMark(Marks.Heart, Heart)
            SetMark(Marks.Square, Square)
            SetMark(Marks.Star, Star)
            SetMark(Marks.Triangle, Triangle)
        End Sub

        Public Sub ClearMarks()
            ClearBit(mMarkings, Marks.Circle)
            ClearBit(mMarkings, Marks.Diamond)
            ClearBit(mMarkings, Marks.Heart)
            ClearBit(mMarkings, Marks.Square)
            ClearBit(mMarkings, Marks.Star)
            ClearBit(mMarkings, Marks.Triangle)
        End Sub

        Public Sub SetEggDateBytes(ByVal NewDateBytes() As Byte)
            ReDim mEggDate(2)
            Array.Copy(NewDateBytes, 0, mEggDate, 0, 3)
        End Sub

        Public Sub SetMetDateBytes(ByVal NewDateBytes() As Byte)
            ReDim mMetDate(2)
            Array.Copy(NewDateBytes, 0, mMetDate, 0, 3)
        End Sub

        Public Sub Recalculate()
            With BattleStats.Calculated
                mMaxHP = .MaxHP
                mCurHP = .MaxHP
                mAtk = .Attack
                mDef = .Defense
                mSpd = .Speed
                mSpAtk = .SpAttack
                mSpDef = .SpDefense
            End With
        End Sub

#End Region

    End Class
End Class