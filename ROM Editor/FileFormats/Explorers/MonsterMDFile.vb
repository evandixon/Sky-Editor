Imports SkyEditor.Core.Utilities.Utilities

Namespace FileFormats.Explorers
    Public Class MonsterMDEntry
        Dim entityID As UInt16
        Dim unk_02 As UInt16
        Dim dex As UInt16 'National Pokédex number, as displayed in Chimecho's assembly.
        Dim unk_06 As UInt16 'Classification?
        Dim evolveFrom As UInt16 'Index of entity in Monster.MD.  NOT the EntityID.
        Dim evolveMethod As UInt16
        Dim evolveParam1 As UInt16
        Dim evolveParam2 As UInt16
        Dim spriteIndex As UInt16
        Dim gender As Byte '0=Invalid, 1=Male, 2=Female, 3=Genderless
        Dim bodySize As Byte
        Dim mainType As Byte
        Dim altType As Byte
        Dim movementType As Byte
        Dim iqGroup As Byte
        Dim ability1 As Byte
        Dim ability2 As Byte
        Dim unk_1a As UInt16
        Dim expYield As UInt16 'Possibly
        Dim recruitRate As UInt16
        Dim baseHP As UInt16
        Dim recruitRate2 As UInt16 'Possibly
        Dim baseATK As Byte
        Dim baseSPATK As Byte
        Dim baseDEF As Byte
        Dim baseSPDEF As Byte
        Dim weight As UInt16
        Dim size As UInt16
        Dim unk_29 As UInt32
        Dim baseFormIndex As UInt16 'Index of entity in Monster.MD
        Dim XItem0 As UInt16
        Dim XItem1 As UInt16
        Dim XItem2 As UInt16
        Dim XItem3 As UInt16
        Dim unk3C As UInt16
        Dim unk3E As UInt16
        Dim unk40 As UInt16
        Dim unk42 As UInt16
        Public Shared Function FromBytes(RawData As Byte())
            Dim e As New MonsterMDEntry
            e.entityID = BitConverter.ToUInt16(RawData, 0)
            e.unk_02 = BitConverter.ToUInt16(RawData, 2)
            e.dex = BitConverter.ToUInt16(RawData, 4)
            e.unk_06 = BitConverter.ToUInt16(RawData, 6)
            e.evolveFrom = BitConverter.ToUInt16(RawData, 8)
            e.evolveMethod = BitConverter.ToUInt16(RawData, &HA)
            e.evolveParam1 = BitConverter.ToUInt16(RawData, &HC)
            e.evolveParam2 = BitConverter.ToUInt16(RawData, &HE)
            e.spriteIndex = BitConverter.ToUInt16(RawData, &H10)
            e.gender = RawData(&H12)
            e.bodySize = RawData(&H13)
            e.mainType = RawData(&H14)
            e.altType = RawData(&H15)
            e.movementType = RawData(&H16)
            e.iqGroup = RawData(&H17)
            e.ability1 = RawData(&H18)
            e.ability2 = RawData(&H19)
            e.unk_1a = BitConverter.ToUInt16(RawData, &H1A)
            e.expYield = BitConverter.ToUInt16(RawData, &H1C)
            e.recruitRate = BitConverter.ToUInt16(RawData, &H1E)
            e.baseHP = BitConverter.ToUInt16(RawData, &H20)
            e.recruitRate2 = BitConverter.ToUInt16(RawData, &H22)
            e.baseATK = RawData(&H24)
            e.baseSPATK = RawData(&H25)
            e.baseDEF = RawData(&H26)
            e.baseSPDEF = RawData(&H27)
            e.weight = BitConverter.ToUInt16(RawData, &H28)
            e.size = BitConverter.ToUInt16(RawData, &H2A)
            'unknown bytes
            e.baseFormIndex = BitConverter.ToUInt16(RawData, &H32)
            e.XItem0 = BitConverter.ToUInt16(RawData, &H34)
            e.XItem1 = BitConverter.ToUInt16(RawData, &H36)
            e.XItem2 = BitConverter.ToUInt16(RawData, &H38)
            e.XItem3 = BitConverter.ToUInt16(RawData, &H3A)
            e.unk3C = BitConverter.ToUInt16(RawData, &H3C)
            e.unk3E = BitConverter.ToUInt16(RawData, &H3E)
            e.unk40 = BitConverter.ToUInt16(RawData, &H40)
            e.unk42 = BitConverter.ToUInt16(RawData, &H42)
            Return e
        End Function
    End Class
    ''' <summary>
    ''' Not 100% correct
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MonsterMDFile
        Dim magic As Int32
        Dim nEntries As UInt32
        Dim Entries As Generic.List(Of MonsterMDEntry)
        Public Shared Function FromBytes(RawData As Byte())
            Dim out As New MonsterMDFile
            out.Entries = New List(Of MonsterMDEntry)
            out.magic = BitConverter.ToInt32(RawData, 0) 'MD\0\0
            out.nEntries = BitConverter.ToUInt32(RawData, 4)
            For count As UInteger = 0 To out.nEntries - 1
                out.Entries.Add(MonsterMDEntry.FromBytes(GenericArrayOperations(Of Byte).CopyOfRange(RawData, 8 + (count * &H44), 8 - 1 + ((count + 1) * &H44))))
            Next
            Return out
        End Function
    End Class
End Namespace
