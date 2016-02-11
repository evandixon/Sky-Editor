Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Namespace FileFormats
    Public Class PokemonDataInfo
        Inherits GenericFile
        Implements iOpenableFile
        Private Const entryLength = &H98
        Public Class PokemonInfoEntry
            Public Property Unk1toF As Byte() '16 bytes
            Public Property Moves As UInt16()
            Public Property MoveLevels As UInt16()
            Public Property Unk5E As UInt16 '0 for all Pokemon
            Public Property Unk60 As UInt32
            Public Property DexNumber As UInt16
            Public Property Unk66 As UInt16
            Public Property Category As UInt16
            Public Property ListNumber1 As UInt16
            Public Property ListNumber2 As UInt16
            Public Property Unk6E As UInt16
            Public Property BaseHP As UInt16
            Public Property BaseAttack As UInt16
            Public Property BaseSpAttack As UInt16
            Public Property BaseDefense As UInt16
            Public Property BaseSpDefense As UInt16
            Public Property BaseSpeed As UInt16
            Public Property EntryNumber As UInt16
            Public Property EvolvesFromEntry As UInt16
            Public Property ExpTableNumber As UInt16
            Public Property Unk82 As Byte() 'Length: 10
            Public Property Ability1 As Byte
            Public Property Ability2 As Byte
            Public Property AbilityHidden As Byte
            Public Property Type1 As Byte
            Public Property Type2 As Byte
            Public Property Unk91 As Byte() 'Length: 3
            Public Property IsMegaEvolution As Byte 'Maybe
            Public Property MinEvolveLevel As Byte
            Public Property Unk96 As UInt16
            Public Function ToBytes() As Byte()
                Dim out As New List(Of Byte)
                'Unknown 1-F
                Dim unk1 = Me.Unk1toF
                For count = 0 To &HF
                    out.Add(unk1(count))
                Next

                'The 26 moves
                For count = 0 To 25
                    out.AddRange(BitConverter.GetBytes(Moves(count)))
                Next

                'The 26 move levels
                For count = 0 To 25
                    out.AddRange(BitConverter.GetBytes(MoveLevels(count)))
                Next

                'Properties
                out.AddRange(BitConverter.GetBytes(Unk5E))
                out.AddRange(BitConverter.GetBytes(Unk60))
                out.AddRange(BitConverter.GetBytes(DexNumber))
                out.AddRange(BitConverter.GetBytes(Unk66))
                out.AddRange(BitConverter.GetBytes(Category))
                out.AddRange(BitConverter.GetBytes(ListNumber1))
                out.AddRange(BitConverter.GetBytes(ListNumber2))
                out.AddRange(BitConverter.GetBytes(Unk6E))
                out.AddRange(BitConverter.GetBytes(BaseHP))
                out.AddRange(BitConverter.GetBytes(BaseAttack))
                out.AddRange(BitConverter.GetBytes(BaseSpAttack))
                out.AddRange(BitConverter.GetBytes(BaseDefense))
                out.AddRange(BitConverter.GetBytes(BaseSpeed))
                out.AddRange(BitConverter.GetBytes(EntryNumber))
                out.AddRange(BitConverter.GetBytes(EvolvesFromEntry))
                out.AddRange(BitConverter.GetBytes(ExpTableNumber))

                'Unknown data
                For count = 0 To &HA
                    out.Add(Unk82(count))
                Next

                'More properties
                out.Add(Ability1)
                out.Add(Ability2)
                out.Add(AbilityHidden)
                out.Add(Type1)
                out.Add(Type2)

                out.Add(Me.Unk91(0))
                out.Add(Me.Unk91(1))
                out.Add(Me.Unk91(2))

                out.Add(IsMegaEvolution)
                out.Add(MinEvolveLevel)
                out.AddRange(BitConverter.GetBytes(Unk96))

                Return out.ToArray
            End Function
            Public Sub New(RawData As Byte())
                'The unknown data
                Dim Unk1toF(&HF) As Byte
                For count = 0 To &HF
                    Unk1toF(count) = RawData(count + 0)
                Next
                Me.Unk1toF = Unk1toF

                'The 26 moves
                Dim moves(&H25) As UInt16
                For count = 0 To 25
                    moves(count) = BitConverter.ToUInt16(RawData, count * 2 + &H10)
                Next
                Me.Moves = moves

                'The 26 levels corresponding to the above moves
                Dim moveLevels(&H25) As UInt16
                For count = 0 To 25
                    moveLevels(count) = BitConverter.ToUInt16(RawData, count * 2 + &H44)
                Next
                Me.MoveLevels = moveLevels

                Unk5E = BitConverter.ToUInt16(RawData, &H5E)
                Unk60 = BitConverter.ToUInt32(RawData, &H60)
                DexNumber = BitConverter.ToUInt16(RawData, &H64)
                Unk66 = BitConverter.ToUInt16(RawData, &H66)
                Category = BitConverter.ToUInt16(RawData, &H68)
                ListNumber1 = BitConverter.ToUInt16(RawData, &H6A)
                ListNumber2 = BitConverter.ToUInt16(RawData, &H6C)
                Unk6E = BitConverter.ToUInt16(RawData, &H6E)
                BaseHP = BitConverter.ToUInt16(RawData, &H70)
                BaseAttack = BitConverter.ToUInt16(RawData, &H72)
                BaseSpAttack = BitConverter.ToUInt16(RawData, &H74)
                BaseDefense = BitConverter.ToUInt16(RawData, &H76)
                BaseSpDefense = BitConverter.ToUInt16(RawData, &H78)
                BaseSpeed = BitConverter.ToUInt16(RawData, &H7A)
                EntryNumber = BitConverter.ToUInt16(RawData, &H7C)
                EvolvesFromEntry = BitConverter.ToUInt16(RawData, &H7E)
                ExpTableNumber = BitConverter.ToUInt16(RawData, &H80)

                'The unknown data
                Dim Unk82(&HA) As Byte
                For count = 0 To &HA
                    Unk82(count) = RawData(count + &H82)
                Next
                Me.Unk82 = Unk82

                Ability1 = RawData(&H8C)
                Ability2 = RawData(&H8D)
                AbilityHidden = RawData(&H8E)
                Type1 = RawData(&H8F)
                Type2 = RawData(&H90)

                Dim unk91(2) As Byte
                unk91(0) = RawData(&H91)
                unk91(1) = RawData(&H92)
                unk91(2) = RawData(&H93)
                Me.Unk91 = unk91

                IsMegaEvolution = RawData(&H94)
                MinEvolveLevel = RawData(&H95)
                Unk96 = BitConverter.ToUInt16(RawData, &H96)
            End Sub
        End Class

        Public Property Entries As List(Of PokemonInfoEntry)

        Public Overrides Sub OpenFile(Filename As String) Implements iOpenableFile.OpenFile
            MyBase.OpenFile(Filename)

            Dim numEntries = Math.Floor(Me.Length / entryLength)

            For count = 0 To numEntries - 1
                Entries.Add(New PokemonInfoEntry(RawData(count * entryLength, entryLength)))
            Next
        End Sub
        Public Overrides Sub Save(Destination As String)
            Me.Length = Entries.Count * entryLength

            For count = 0 To Entries.Count - 1
                RawData(count * entryLength, entryLength) = Entries(count).ToBytes
            Next

            MyBase.Save(Destination)
        End Sub

        Public Sub New()
            MyBase.New()
            Entries = New List(Of PokemonInfoEntry)
        End Sub
    End Class
End Namespace