Imports System.Drawing
Imports SkyEditorBase

Namespace FileFormats



    Enum EvolveType
        BaseForm = 0
        EvolveLevel = 1
    End Enum
    Enum Gender
        Dummy = 0

    End Enum

    Public Class MonsterMDEntry
        Dim index As Int16
        Dim unk_02 As Int16
        Dim dex As Int16 'Enum DEX_ID
        Dim unk_06 As Int16 'Classification?
        Dim evolveFrom As Int16 'Weirdness exists with indexing
        Dim evolveMethod As Int16
        Dim evolveParam1 As Int16
        Dim unk_0E As Int16
        Dim spriteID As Int16
        Dim gender As Byte
        Dim bodySize As Byte
        Dim mainType As Byte
        Dim altType As Byte
        Dim unk_16 As Long
        Dim recruitRate As Int16
        Dim baseHP As Int16
        Dim unk_22 As Int16
        Dim baseATK As Byte
        Dim baseSPATK As Byte
        Dim baseDEF As Byte
        Dim baseSPDEF As Byte
        Dim unk_28 As Long
        Dim unk_29 As Int16
        Dim family As Int16
        Dim XItem0 As Int16
        Dim XItem1 As Int16
        Dim XItem2 As Int16
        Dim XItem3 As Int16
        Dim unk3C As Int16
        Dim unk3E As Int16
        Dim unk40 As Int16
        Dim unk42 As Int16
        Public Shared Function FromBytes(RawData As Byte())
            Dim e As New MonsterMDEntry
            e.index = BitConverter.ToInt16(RawData, 0)
            e.unk_02 = BitConverter.ToInt16(RawData, 2)
            e.dex = BitConverter.ToInt16(RawData, 4)
            e.unk_06 = BitConverter.ToInt16(RawData, 6)
            e.evolveFrom = BitConverter.ToInt16(RawData, 8)
            e.evolveMethod = BitConverter.ToInt16(RawData, &HA)
            e.evolveParam1 = BitConverter.ToInt16(RawData, &HC)
            e.unk_0E = BitConverter.ToInt16(RawData, &HE)
            e.spriteID = BitConverter.ToInt16(RawData, &H10)
            e.gender = RawData(&H12)
            e.bodySize = RawData(&H13)
            e.mainType = RawData(&H14)
            e.altType = RawData(&H15)
            e.unk_16 = BitConverter.ToInt64(RawData, &H16)
            e.recruitRate = BitConverter.ToInt16(RawData, &H1E)
            e.baseHP = BitConverter.ToInt16(RawData, &H20)
            e.unk_22 = BitConverter.ToInt16(RawData, &H22)
            e.baseATK = RawData(&H24)
            e.baseSPATK = RawData(&H25)
            e.baseDEF = RawData(&H26)
            e.baseSPDEF = RawData(&H27)
            e.unk_28 = BitConverter.ToInt64(RawData, &H28)
            e.unk_29 = BitConverter.ToInt16(RawData, &H30)
            e.family = BitConverter.ToInt16(RawData, &H32)
            e.XItem0 = BitConverter.ToInt16(RawData, &H34)
            e.XItem1 = BitConverter.ToInt16(RawData, &H36)
            e.XItem2 = BitConverter.ToInt16(RawData, &H38)
            e.XItem3 = BitConverter.ToInt16(RawData, &H3A)
            e.unk3C = BitConverter.ToInt16(RawData, &H3C)
            e.unk3E = BitConverter.ToInt16(RawData, &H3E)
            e.unk40 = BitConverter.ToInt16(RawData, &H40)
            e.unk42 = BitConverter.ToInt16(RawData, &H42)
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
            out.magic = BitConverter.ToInt32(RawData, 0)
            out.nEntries = BitConverter.ToUInt32(RawData, 4)
            For count As UInteger = 0 To out.nEntries - 1
                out.Entries.Add(MonsterMDEntry.FromBytes(SkyEditorBase.GenericArrayOperations(Of Byte).CopyOfRange(RawData, 8 + (count * &H44), 8 - 1 + ((count + 1) * &H44))))
            Next
            Return out
        End Function
    End Class
End Namespace