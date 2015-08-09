Imports System.Runtime.InteropServices
Imports System.IO
Imports System.Drawing

Partial Class PokemonLib
    ''' <summary>
    ''' A structure for containing Pokémon Platinum save data.
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
<StructLayout(LayoutKind.Sequential, Size:=&H80000)> _
    Public Class PtSaveFile '0x80000 bytes

        'TODO: Fix HoF block / padding / whatever is causing save file structure to be 4 bytes too long.

#Region "Members"
        Private General1 As mPtGeneralBlock 'Starts at 0x00000, length is 0xCF2C
        Private Storage1 As mPtStorageBlock 'Starts at 0x0C100, length is 0x121E4
        <MarshalAs(UnmanagedType.ByValArray, sizeconst:=&HEF0)> _
        Public Padding1() As Byte
        Private HallOfFame1 As mPtHallOfFameBlock 'Starts at 0x20000, length is 0x20000
        '<MarshalAs(UnmanagedType.ByValArray, sizeconst:=&H1D540)> _
        'Public Padding2() As Byte
        Private General2 As mPtGeneralBlock 'Starts at 0x40000, length is 0xCF2C
        Private Storage2 As mPtStorageBlock 'Starts at 0x4C100, length is 0x121E4
        <MarshalAs(UnmanagedType.ByValArray, sizeconst:=&HEF0)> _
        Public Padding2() As Byte
        Private HallOfFame2 As mPtHallOfFameBlock 'Starts at 0x60000, length is 0x20000
        '<MarshalAs(UnmanagedType.ByValArray, sizeconst:=&H1D540)> _
        'Public Padding4() As Byte
#End Region

        Public Sub New()
            General1 = New mPtGeneralBlock
            Storage1 = New mPtStorageBlock
            Padding1 = Nothing
            HallOfFame1 = New mPtHallOfFameBlock
            Padding2 = Nothing
            General2 = New mPtGeneralBlock
            Storage2 = New mPtStorageBlock
            HallOfFame2 = New mPtHallOfFameBlock
            mGenCur = New mPtGeneralBlock
            mGenBack = New mPtGeneralBlock
            mStoCur = New mPtStorageBlock
            mStoBack = New mPtStorageBlock
            mHoFCur = New mPtHallOfFameBlock
            mHoFBack = New mPtHallOfFameBlock
            seeds = Nothing
        End Sub

#Region "Private Variables"
        <NonSerialized()> _
        Private Shared mGenCur As mPtGeneralBlock
        <NonSerialized()> _
        Private Shared mGenBack As mPtGeneralBlock
        <NonSerialized()> _
        Private Shared mStoCur As mPtStorageBlock
        <NonSerialized()> _
        Private Shared mStoBack As mPtStorageBlock
        <NonSerialized()> _
        Private Shared mHoFCur As mPtHallOfFameBlock
        <NonSerialized()> _
        Private Shared mHoFBack As mPtHallOfFameBlock
        <NonSerialized()> _
        Private seeds() As Integer
#End Region

#Region "Methods"

#Region "Save Block Checksums"

        Private Function GetSeeds() As Integer
            seeds = New Integer(&H100 - 1) {}

            Dim v0 As Integer = 0
            Dim v1 As Integer
            Dim v2 As Integer
            Dim result As Integer

            Do
                v1 = v0 << 8
                v2 = 0
                Do
                    If (CType(v1 >> 8, [Byte]) And &H80) <> 0 Then
                        v1 = (2 * v1) Xor &H1021
                    Else
                        v1 *= 2
                    End If
                    v2 += 1
                Loop While v2 < 8
                result = CUShort(v1)
                seeds(v0) = result
                v0 += 1
            Loop While v0 <= &HFF
            Return result
        End Function

        Public Sub FixBlockChecksum(ByVal Block As mPtGeneralBlock)
            Block.Footer.Checksum = SaveBlockChecksums(Block)
        End Sub

        Public Sub FixBlockChecksum(ByVal Block As mPtStorageBlock)
            Block.Footer.Checksum = SaveBlockChecksums(Block)
        End Sub

        Public Sub FixBlockChecksum(ByVal Block As mPtHallOfFameBlock)
            Block.Footer.mChecksum = SaveBlockChecksums(Block)
        End Sub

        Private Function SaveBlockChecksums(ByVal Block As mPtGeneralBlock) As UShort
            GetSeeds()
            Dim Data() As Byte = RawSerialize(Block)
            Dim v2 As Integer = &HFFFF
            Dim i As Integer
            Dim v4 As Integer = Block.Footer.Size - &H14
            Dim v6 As Byte

            i = 0
            Do While v4 > 0
                v4 -= 1
                v6 = CType(Data(i) Xor CType(v2 >> 8, [Byte]), [Byte])
                v2 = (v2 << 8) Xor seeds(v6)
                i += 1
            Loop
            Return CUShort(v2)
        End Function

        Private Function SaveBlockChecksums(ByVal Block As mPtStorageBlock) As UShort
            GetSeeds()
            Dim Data() As Byte = RawSerialize(Block)
            Dim v2 As Integer = &HFFFF
            Dim i As Integer
            Dim v4 As Integer = Block.Footer.Size - &H14
            Dim v6 As Byte

            i = 0
            Do While v4 > 0
                v4 -= 1
                v6 = CType(Data(i) Xor CType(v2 >> 8, [Byte]), [Byte])
                v2 = (v2 << 8) Xor seeds(v6)
                i += 1
            Loop
            Return CUShort(v2)
        End Function

        Private Function SaveBlockChecksums(ByVal Block As mPtHallOfFameBlock) As UShort
            GetSeeds()
            Dim Data() As Byte = RawSerialize(Block)
            Dim v2 As Integer = &HFFFF
            Dim i As Integer
            Dim v4 As Integer = Block.Footer.Size - &H18
            Dim v6 As Byte

            i = 0
            Do While v4 > 0
                v4 -= 1
                v6 = CType(Data(i) Xor CType(v2 >> 8, [Byte]), [Byte])
                v2 = (v2 << 8) Xor seeds(v6)
                i += 1
            Loop
            Return CUShort(v2)
        End Function

#End Region

        Public Function WithdrawPokemon(ByVal Box As Byte, ByVal Slot As Byte) As Boolean
            If PartyPokemon.Count < 6 Then
                Dim PKM As Pokemon = StorageBlock.RemovePCPokemon(Box, Slot)
                AddPartyPokemon(PKM)
                Return True
            End If
            Return False
        End Function

        Public Function DepositPokemon(ByVal PartySlot As Byte, ByVal Box As Byte, Optional ByVal PCSlot As Byte = 0) As Boolean
            If PartyPokemon.Count > 1 Then
                If StorageBlock.AddPCPokemon(Box, PartyPokemon(PartySlot - 1), PCSlot) Then
                    RemovePartyPokemon(PartySlot)
                    Return True
                End If
            End If
            Return False
        End Function

        Public Sub AddPartyPokemon(ByVal PKM As Pokemon, Optional ByVal Slot As Integer = -1)

            Dim PKMLIST As List(Of Pokemon) = PartyPokemon
            If PKMLIST.Count >= 6 Then Exit Sub
            PKM = PC_to_Party(PKM)
            If Slot = -1 Or Slot > PKMLIST.Count Then
                PKMLIST.Add(PKM)
            Else
                PKMLIST.Insert(Slot, PKM)
            End If

            Dim buf(GeneralBlock.ptPKM.Length - 1) As Byte
            Array.Copy(GeneralBlock.ptPKM, 0, buf, 0, GeneralBlock.ptPKM.Length)

            Array.Copy(BlankParty, 0, buf, &H0, 236)
            Array.Copy(BlankParty, 0, buf, &HEC, 236)
            Array.Copy(BlankParty, 0, buf, &H1D8, 236)
            Array.Copy(BlankParty, 0, buf, &H2C4, 236)
            Array.Copy(BlankParty, 0, buf, &H3B0, 236)
            Array.Copy(BlankParty, 0, buf, &H49C, 236)

            'If PKMLIST.Count = 0 Then
            '    Array.Copy(EncryptPokemon(RawSerialize(PKMLIST(0))), 0, buf, &H0, 236)
            '    Array.Copy(EncryptPokemon(RawSerialize(PKMLIST(1))), 0, buf, &HEC, 236)
            '    Array.Copy(EncryptPokemon(RawSerialize(PKMLIST(2))), 0, buf, &H1D8, 236)
            '    Array.Copy(EncryptPokemon(RawSerialize(PKMLIST(3))), 0, buf, &H2C4, 236)
            '    Array.Copy(EncryptPokemon(RawSerialize(PKMLIST(4))), 0, buf, &H3B0, 236)
            '    Array.Copy(EncryptPokemon(RawSerialize(PKMLIST(5))), 0, buf, &H49C, 236)
            'End If

            If PKMLIST.Count >= 1 Then _
            Array.Copy(EncryptPokemon(RawSerialize(PKMLIST(0))), 0, buf, &H0, 236)

            If PKMLIST.Count >= 2 Then _
            Array.Copy(EncryptPokemon(RawSerialize(PKMLIST(1))), 0, buf, &HEC, 236)

            If PKMLIST.Count >= 3 Then _
            Array.Copy(EncryptPokemon(RawSerialize(PKMLIST(2))), 0, buf, &H1D8, 236)

            If PKMLIST.Count >= 4 Then _
            Array.Copy(EncryptPokemon(RawSerialize(PKMLIST(3))), 0, buf, &H2C4, 236)

            If PKMLIST.Count >= 5 Then _
            Array.Copy(EncryptPokemon(RawSerialize(PKMLIST(4))), 0, buf, &H3B0, 236)

            If PKMLIST.Count = 6 Then _
            Array.Copy(EncryptPokemon(RawSerialize(PKMLIST(5))), 0, buf, &H49C, 236)

            Array.Copy(buf, GeneralBlock.ptPKM, buf.Length)
            mGenCur.SetPartyCount(PKMLIST.Count)
        End Sub

        Public Sub RemovePartyPokemon(ByVal SlotNum As Byte, Optional ByVal OverrideMin As Boolean = False)

            Dim PKMLIST As List(Of Pokemon) = PartyPokemon
            If (PKMLIST.Count <= 1) And (Not OverrideMin) Then Exit Sub
            If SlotNum > PKMLIST.Count Then Exit Sub
            PKMLIST.RemoveAt(SlotNum - 1)
            Dim buf(GeneralBlock.ptPKM.Length - 1) As Byte
            Array.Copy(GeneralBlock.ptPKM, 0, buf, 0, GeneralBlock.ptPKM.Length)

            Array.Copy(BlankParty, 0, buf, &H0, 236)
            Array.Copy(BlankParty, 0, buf, &HEC, 236)
            Array.Copy(BlankParty, 0, buf, &H1D8, 236)
            Array.Copy(BlankParty, 0, buf, &H2C4, 236)
            Array.Copy(BlankParty, 0, buf, &H3B0, 236)
            Array.Copy(BlankParty, 0, buf, &H49C, 236)

            If PKMLIST.Count >= 1 Then _
            Array.Copy(EncryptPokemon(RawSerialize(PKMLIST(0))), 0, buf, &H0, 236)

            If PKMLIST.Count >= 2 Then _
            Array.Copy(EncryptPokemon(RawSerialize(PKMLIST(1))), 0, buf, &HEC, 236)

            If PKMLIST.Count >= 3 Then _
            Array.Copy(EncryptPokemon(RawSerialize(PKMLIST(2))), 0, buf, &H1D8, 236)

            If PKMLIST.Count >= 4 Then _
            Array.Copy(EncryptPokemon(RawSerialize(PKMLIST(3))), 0, buf, &H2C4, 236)

            If PKMLIST.Count >= 5 Then _
            Array.Copy(EncryptPokemon(RawSerialize(PKMLIST(4))), 0, buf, &H3B0, 236)

            If PKMLIST.Count = 6 Then _
            Array.Copy(EncryptPokemon(RawSerialize(PKMLIST(5))), 0, buf, &H49C, 236)

            Array.Copy(buf, GeneralBlock.ptPKM, buf.Length)
            mGenCur.SetPartyCount(PKMLIST.Count)
        End Sub

        Public Function GetBlocks(Optional ByVal Reassign As Boolean = True, Optional ByVal IgnoreErr As Boolean = False) As Byte()

            'Dim out(1) As Byte
            Dim LoadErr1 As New Exception("The save file is corrupted.  The previous save will be loaded.")
            Dim LoadErr2 As New Exception("The save file is corrupted.")
            Dim ErrOUT As New Exception("")

            If (General1.Footer.GenCount <> BitConverter.ToUInt32(New Byte() {&HFF, &HFF, &HFF, &HFF}, 0)) _
            And (General1.Footer.StoCount < BitConverter.ToUInt32(New Byte() {&HFF, &HFF, &HFF, &HFF}, 0)) _
            And (General1.Footer.GenCount > General2.Footer.GenCount) Then
                If Reassign Then
                    If General1.Footer.Checksum = SaveBlockChecksums(General1) Then
                        mGenCur = General1
                        mGenBack = General2
                        out(0) = 0
                    Else
                        If General2.Footer.Checksum = SaveBlockChecksums(General2) Then
                            mGenCur = General2
                            mGenBack = General1
                            out(0) = 1
                            ErrOUT = LoadErr1
                        Else
                            ErrOUT = LoadErr2
                        End If
                    End If
                End If
            Else
                If Reassign Then
                    If General2.Footer.Checksum = SaveBlockChecksums(General2) Then
                        mGenCur = General2
                        mGenBack = General1
                        out(0) = 1
                    Else
                        If General1.Footer.Checksum = SaveBlockChecksums(General1) Then
                            mGenCur = General1
                            mGenBack = General2
                            out(0) = 0
                            ErrOUT = LoadErr1
                        Else
                            ErrOUT = LoadErr2
                        End If
                    End If
                End If
            End If

            If (Storage1.Footer.GenCount < BitConverter.ToUInt32(New Byte() {&HFF, &HFF, &HFF, &HFF}, 0) _
                And Storage2.Footer.StoCount < BitConverter.ToUInt32(New Byte() {&HFF, &HFF, &HFF, &HFF}, 0)) _
                And Storage1.Footer.StoCount = Storage2.Footer.StoCount Then
                If Storage1.Footer.GenCount > Storage2.Footer.GenCount Then
                    If Reassign Then
                        If Storage1.Footer.Checksum = SaveBlockChecksums(Storage1) Then
                            mStoCur = Storage1
                            mStoBack = Storage2
                            out(1) = 0
                        Else
                            If Storage2.Footer.Checksum = SaveBlockChecksums(Storage2) Then
                                mStoCur = Storage2
                                mStoBack = Storage1
                                out(1) = 1
                                ErrOUT = LoadErr1
                            Else
                                ErrOUT = LoadErr2
                            End If
                        End If
                    End If
                Else
                    If Reassign Then
                        If Storage2.Footer.Checksum = SaveBlockChecksums(Storage2) Then
                            mStoCur = Storage2
                            mStoBack = Storage1
                            out(1) = 1
                        Else
                            If Storage1.Footer.Checksum = SaveBlockChecksums(Storage1) Then
                                mStoCur = Storage1
                                mStoBack = Storage2
                                out(1) = 0
                                ErrOUT = LoadErr1
                            Else
                                ErrOUT = LoadErr2
                            End If
                        End If
                    End If
                End If
            Else
                If mGenCur.Footer.StoCount = Storage1.Footer.StoCount Then
                    If Reassign Then
                        If Storage1.Footer.Checksum = SaveBlockChecksums(Storage1) Then
                            mStoCur = Storage1
                            mStoBack = Storage2
                            out(1) = 0
                        Else
                            If Storage2.Footer.Checksum = SaveBlockChecksums(Storage2) Then
                                mStoCur = Storage2
                                mStoBack = Storage1
                                out(1) = 1
                                ErrOUT = LoadErr1
                            Else
                                ErrOUT = LoadErr2
                            End If
                        End If
                    End If
                Else
                    If Reassign Then
                        If Storage2.Footer.Checksum = SaveBlockChecksums(Storage2) Then
                            mStoCur = Storage2
                            mStoBack = Storage1
                            out(1) = 1
                        Else
                            If Storage1.Footer.Checksum = SaveBlockChecksums(Storage1) Then
                                mStoCur = Storage1
                                mStoBack = Storage2
                                out(1) = 0
                                ErrOUT = LoadErr1
                            Else
                                ErrOUT = LoadErr2
                            End If
                        End If
                    End If
                End If
            End If

            GetBlocks = out

            If Not IgnoreErr Then
                If ErrOUT.Message <> "" Then Throw ErrOUT
            Else
                Console.WriteLine(ErrOUT.Message)
            End If

        End Function

        Public Sub SaveData(ByVal FileName As String)
            Using fs As New FileStream(FileName, FileMode.Create, FileAccess.Write)
                Using bW As New BinaryWriter(fs)
                    bW.Write(SaveData)
                End Using
            End Using
        End Sub

        Public Function SaveData() As Byte()

            Dim BlockState() As Byte = GetBlocks(False)

            Dim _GENblock As mPtGeneralBlock = GeneralBlock
            Dim _STOblock As mPtStorageBlock = StorageBlock
            Dim _HOF As mPtHallOfFameBlock = HallOfFameBlock
            Dim _GeneralFooter As GeneralFooter = GeneralBlock.Footer
            Dim _StorageFooter As GeneralFooter = StorageBlock.Footer
            Dim _HFT As HallOfFameFooter = HallOfFameBlock.Footer

            'First the checksum of the in-memory general block is updated.
            '_GeneralFooter.Checksum = SaveBlockChecksums(_GENblock)
            'Next the save count of the in-memory general block is incremented by 1.
            _GeneralFooter.GenCount += 1

            If _STOblock.Footer.Checksum <> SaveBlockChecksums(_STOblock) Then
                _StorageFooter.Checksum = SaveBlockChecksums(_STOblock)
                _GeneralFooter.StoCount += 1
                _StorageFooter.StoCount += 1
                _StorageFooter.GenCount = _GeneralFooter.GenCount
                '_StorageFooter.GenCount += 1
                _STOblock.Footer = _StorageFooter

                'mStoBack = _STOblock
                mStoCur = _STOblock

                If BlockState(1) = 0 Then
                    'Storage2 = mStoBack
                    'Storage1 = mStoCur
                    Storage1 = mStoBack
                    Storage2 = mStoCur
                End If
                If BlockState(1) = 1 Then
                    'Storage1 = mStoBack
                    'Storage2 = mStoCur
                    Storage2 = mStoBack
                    Storage1 = mStoCur
                End If

            End If

            _GeneralFooter.Checksum = SaveBlockChecksums(_GENblock)
            _GENblock.Footer = _GeneralFooter
            'The backup general block has now become the current general block.

            'mGenBack = _GENblock
            mGenCur = _GENblock

            If BlockState(0) = 0 Then
                'General2 = mGenBack
                'General1 = mGenCur
                General1 = mGenBack
                General2 = mGenCur
            End If
            If BlockState(0) = 1 Then
                'General1 = mGenBack
                'General2 = mGenCur
                General2 = mGenBack
                General1 = mGenCur
            End If

            Dim DATA() As Byte = RawSerialize(Me)
            Dim OUT(&H80000 - 1) As Byte

            Array.Copy(DATA, 0, OUT, 0, &H80000)

            Return OUT
        End Function

        Public Sub SetBadge(ByVal _Badge As DSBadges, ByVal State As Boolean)
            SetBit(GeneralBlock.trBadges, _Badge + 1, State)
        End Sub

#End Region

#Region "Properties"

#Region "User Accessible Blocks"

        Public Property GeneralBlock() As mPtGeneralBlock
            Get
                Return mGenCur
            End Get
            Set(ByVal value As mPtGeneralBlock)
                mGenCur = value
            End Set
        End Property

        Public Property StorageBlock() As mPtStorageBlock
            Get
                Return mStoCur
            End Get
            Set(ByVal value As mPtStorageBlock)
                mStoCur = value
            End Set
        End Property

        Public Property HallOfFameBlock() As mPtHallOfFameBlock
            Get
                Return mHoFCur
            End Get
            Set(ByVal value As mPtHallOfFameBlock)
                mHoFCur = value
            End Set
        End Property

#End Region

#Region "User Hidden Blocks"

        Private ReadOnly Property GeneralBlockCurrent() As mPtGeneralBlock
            Get
                Return mGenCur
            End Get
        End Property

        Private ReadOnly Property GeneralBlockBackup() As mPtGeneralBlock
            Get
                Return mGenBack
            End Get
        End Property

        Private Property StorageBlockCurrent() As mPtStorageBlock
            Get
                Return mStoCur
            End Get
            Set(ByVal value As mPtStorageBlock)
                mStoCur = value
            End Set
        End Property

        Private Property StorageBlockBackup() As mPtStorageBlock
            Get
                Return mStoBack
            End Get
            Set(ByVal value As mPtStorageBlock)
                mStoBack = value
            End Set
        End Property

        Private ReadOnly Property HallOfFameBlockCurrent() As mPtHallOfFameBlock
            Get
                Return mHoFCur
            End Get
        End Property

        Private ReadOnly Property HallOfFameBlockBackup() As mPtHallOfFameBlock
            Get
                Return mHoFBack
            End Get
        End Property

#End Region

        'Public ReadOnly Property Score() As UInt32
        '    Get
        '        Return GeneralBlock.trScore
        '    End Get
        'End Property

        Public ReadOnly Property Country() As mmCountry
            Get
                Country = New mmCountry
                Country.Value = GeneralBlock.trCountry
            End Get
        End Property

        Public ReadOnly Property AdventureStarted() As Date
            Get
                Return EpochToDateTime(GeneralBlock.adStartDate)
            End Get
        End Property

        Public ReadOnly Property LeagueChampDate() As Date
            Get
                Return EpochToDateTime(GeneralBlock.leagueBeatDate)
            End Get
        End Property

        Public Property Trainer() As mTrainer
            Get
                With GeneralBlock
                    Return New mTrainer(.trName, New mGender(.trGender), .trID, .trSID, True)
                End With
            End Get
            Set(ByVal value As mTrainer)
                With GeneralBlock
                    .trName = StringToPKMBytes(value.Name, True)

                End With
            End Set
        End Property

        Public ReadOnly Property Money() As UInt32
            Get
                Return GeneralBlock.trMoney
            End Get
        End Property

        Public Property PartyPokemon() As List(Of Pokemon) 'Pokemon()
            Get
                Dim _Party As New List(Of Pokemon)
                Dim Offset As Integer = 0
                For i As Integer = 0 To GeneralBlock.numPtPKM - 1
                    Dim p(235) As Byte
                    Array.Copy(GeneralBlock.ptPKM, Offset, p, 0, 236)
                    p = DecryptPokemon(p)
                    Dim pkm As New Pokemon
                    pkm = RawDeserialize(p, pkm.GetType)
                    If pkm.Species.ID > 0 Or pkm.Species.ID <= 493 Then
                        _Party.Add(pkm)
                    End If
                    p = Nothing
                    Offset += 236
                Next
                Return _Party
            End Get
            Set(ByVal value As List(Of Pokemon))
                For i As Integer = 0 To value.Count - 1
                    Dim p() As Byte = EncryptPokemon(RawSerialize(value(i)))
                    Array.Copy(p, 0, GeneralBlock.ptPKM, (236 * i), 236)
                Next
            End Set
        End Property

        Public Property RivalName() As String
            Get
                Return PKMBytesToString(GeneralBlock.trRivalName)
            End Get
            Set(ByVal value As String)
                If value.Length > 10 Then value = value.Substring(0, 10)
                Dim out() As Byte
                out = StringToPKMBytes(value, True)
                Array.Copy(out, GeneralBlock.trRivalName, out.Length)
            End Set
        End Property

        Public ReadOnly Property Avatar() As String
            Get
                Try
                    Return dpAvatarNames(GeneralBlock.trAvatar)
                Catch ex As Exception
                    Return ""
                End Try
            End Get
        End Property

        Public ReadOnly Property StarterPokemon() As String
            Get
                PokemonLib.InitializeDictionaries()
                If GeneralBlock.trStarter = 0 Then Return ""
                Return dPKMSpecies(GeneralBlock.trStarter)
            End Get
        End Property

        'Public ReadOnly Property GTSPokemon() As Pokemon
        '    Get
        '        GTSPokemon = New Pokemon
        '        Return RawDeserialize(UnShuffleBytes(Decrypt_Data(GeneralBlock.trGTSPKM)), GTSPokemon.GetType)
        '    End Get
        'End Property

        Public ReadOnly Property Map() As mMap
            Get
                With GeneralBlock
                    Return New mMap(.trMapID, .trMapX, .trMapY, .trMapZ)
                End With
            End Get
        End Property

        Public ReadOnly Property Playtime() As TimeSpan
            Get
                With GeneralBlock
                    Return New TimeSpan(.mPlayHrs, .mPlayMin, .mPlaySec)
                End With
            End Get
        End Property

        Public ReadOnly Property Signature() As Bitmap
            Get
                Signature = New Bitmap(192, 64)
                Dim ByteNum, BlockNum, BlockRowNum As Integer
                Dim BA As BitArray
                Dim X, Y As Integer
                For Iterate As Integer = 0 To 7
                    For i As Integer = (0 + (Iterate * 192)) To (191 + (Iterate * 192))
                        BA = New BitArray(New Byte() {GeneralBlock.TCardSig(i)})
                        For iC As Integer = 0 To 7
                            X = (BlockNum * 8) + iC
                            If BA(iC) Then
                                Signature.SetPixel(X, (Y + (8 * Iterate)), Color.Black)
                            Else
                            End If
                        Next iC
                        ByteNum += 1
                        Y = ByteNum Mod 8
                        If ByteNum Mod 8 = 0 Then
                            BlockNum += 1
                        End If
                        If ByteNum Mod 192 = 0 Then
                            BlockRowNum += 1
                        End If
                    Next i
                    ByteNum = 0
                    BlockNum = 0
                    BlockRowNum = 0
                    Y = 0
                Next Iterate
            End Get
        End Property

        Public ReadOnly Property Poketch() As mPoketch
            Get
                Dim MP As New mPoketch
                MP.PokemonHistory = New _pktchPKMHistory(GeneralBlock.trPktchPKMHist)
                'MP.TrainerHistory = New _pktchTHistory(GeneralBlock.trPktchTHistory)
                MP.StepCounter = New _pktchStepCounter(GeneralBlock.trPktchSteps)
                Return MP
            End Get
        End Property

        Public ReadOnly Property FriendCode() As UInt64
            Get
                Dim FCM As New FriendCodeManager
                Return FCM.GetFC(GeneralBlock.mFC_1)
            End Get
        End Property

#Region "Pal Pad"

        Public ReadOnly Property PalPad() As List(Of mPalPadEntry)
            Get
                PalPad = New List(Of mPalPadEntry)
                Dim FCList As New List(Of UInt64)
                Dim FCM As New FriendCodeManager

                For i As Integer = 0 To &H180 - &HC Step &HC
                    Dim theFC As UInt32 = BitConverter.ToUInt32(GeneralBlock.PalPadFCs, i + 4)
                    If theFC = 0 Then
                        Exit For
                    End If
                    FCList.Add(FCM.GetFC(theFC))
                Next

                Dim DataStep As Integer = GeneralBlock.PalPadNames.Length / 32

                For i As Integer = 0 To GeneralBlock.PalPadNames.Length - DataStep Step DataStep
                    If i / DataStep >= FCList.Count Then Exit For
                    Dim newPPEntry As New mPalPadEntry
                    Dim PPEntryData(DataStep - 1) As Byte
                    Array.Copy(GeneralBlock.PalPadNames, i, PPEntryData, 0, DataStep)
                    newPPEntry = RawDeserialize(PPEntryData, newPPEntry.GetType)
                    newPPEntry.FriendCode = FCList(i / DataStep)
                    If FCList(i / DataStep) <> 0 Then _
                    PalPad.Add(newPPEntry)
                Next

            End Get
        End Property

#End Region

        Public Property Badges() As Badge()
            Get
                Dim theBadges(7) As Badge
                For i As Integer = 0 To 7
                    theBadges(i) = New Badge
                    theBadges(i).Name = DSBadgeNames(i)
                    theBadges(i).Obtained = ExamineBit(GeneralBlock.trBadges, i + 1)
                Next
                Return theBadges
            End Get
            Set(ByVal value As Badge())
                For i As Integer = 0 To value.Count - 1
                    If i = 8 Then Exit For
                    SetBit(GeneralBlock.trBadges, i + 1, value(i).Obtained)
                    'TODO: Set badge shininess
                Next
            End Set
        End Property

#End Region

    End Class
End Class