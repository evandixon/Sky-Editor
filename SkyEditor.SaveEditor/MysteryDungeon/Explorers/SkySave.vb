Imports SkyEditor.Core
Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.IO
Imports SkyEditor.SaveEditor.Interfaces
Imports SkyEditor.SaveEditor.Modeling

Namespace MysteryDungeon.Explorers
    Public Class SkySave
        Inherits BinaryFile
        Implements IDetectableFileType
        Implements iParty
        Implements iPokemonStorage
        Implements IInventory
        Implements INotifyPropertyChanged

#Region "Child Classes"

        Protected Class Offsets
            Public Const BackupSaveStart As Integer = &HC800
            Public Const ChecksumEnd As Integer = &HB65A
            Public Const QuicksaveStart As Integer = &H19000
            Public Const QuicksaveChecksumStart As Integer = &H19004
            Public Const QuicksaveChecksumEnd As Integer = &H1E7FF

            Public Const TeamNameStart As Integer = &H994E * 8
            Public Const TeamNameLength As Integer = 10

            Public Const ExplorerRank As Integer = &H9958 * 8

            Public Const Adventures As Integer = &H8B70 * 8

            Public Const WindowFrameType As Integer = &H995F * 8 + 5

            Public Const HeldMoney As Integer = &H990C * 8 + 6
            Public Const SPHeldMoney As Integer = &H990F * 8 + 6
            Public Const StoredMoney As Integer = &H9915 * 8 + 6

            Public Const StoredPokemonOffset As Integer = &H464 * 8
            Public Const StoredPokemonLength As Integer = 362
            Public Const StoredPokemonNumber As Integer = 720

            Public Const ActivePokemonOffset As Integer = &H83D9 * 8 + 1
            Public Const SpActivePokemonOffset As Integer = &H84F4 * 8 + 2
            Public Const ActivePokemonLength As Integer = 546
            Public Const ActivePokemonNumber As Integer = 4

            Public Const HeldItemOffset As Integer = &H8BA2 * 8
            Public Const HeldItemLength As Integer = 33
            Public Const HeldItemNumber As Integer = 100 '1st 50 are the team's, 2nd 50 are the Sp. Episode

            Public Const StoredItemOffset As Integer = &H8E0C * 8 + 6
            Public Const StoredItemLength As Integer = 2 * 11
            Public Const StoredItemNumber As Integer = 1000

            Public Const ItemShop1Offset As Integer = &H98CA * 8 + 6
            Public Const ItemShopLength As Integer = 22
            Public Const ItemShop1Number As Integer = 8
            Public Const ItemShop2Offset As Integer = &H98E0 * 8 + 6
            Public Const ItemShop2Number As Integer = 4

            Public Const AdventureLogOffset As Integer = &H9958 * 8
            Public Const AdventureLogLength As Integer = 447 'Not tested

            Public Const CroagunkShopOffset As Integer = &HB475 * 8
            Public Const CroagunkShopLength As Integer = 11
            Public Const CroagunkShopNumber As Integer = 8

            Public Const QuicksavePokemonNumber As Integer = 20
            Public Const QuicksavePokemonLength As Integer = 429 * 8
            Public Const QuicksavePokemonOffset As Integer = &H19000 * 8 + (&H3170 * 8)
        End Class

        Public Enum HeldBy As Byte
            None
            Player1
            Player2
            Player3
            Player4
        End Enum

        Public Class SkyItem
            Implements IExplorersItem
            Public Property IsValid As Boolean
            Private Property Flag1 As Boolean
            Private Property Flag2 As Boolean
            Private Property Flag3 As Boolean
            Private Property Flag4 As Boolean
            Private Property Flag5 As Boolean
            Private Property Flag6 As Boolean
            Private Property Flag7 As Boolean
            Public Property ID As Integer Implements IExplorersItem.ID

            ''' <summary>
            ''' The ID of the item inside this one, if this item is a box.
            ''' </summary>
            ''' <returns></returns>
            Public Property ContainedItemID As Integer? Implements IExplorersItem.ContainedItemID
            Public Property Quantity As Integer Implements IExplorersItem.Quantity
            Public Property HeldBy As Byte Implements IExplorersItem.HeldBy
            Public ReadOnly Property IsBox As Boolean Implements IExplorersItem.IsBox
                Get
                    Return ID > 363 AndAlso ID < 400
                End Get
            End Property

            Public Overrides Function ToString() As String
                Return Lists.GetSkyItemNames(ID)
            End Function

            Public Function GetParameter() As Integer
                If IsBox Then
                    Return ContainedItemID
                Else
                    Return Quantity
                End If
            End Function

            Public Function GetHeldItemBits() As Binary
                Dim out As New Binary(Offsets.HeldItemLength)
                With out
                    .Bit(0) = IsValid
                    .Bit(1) = Flag1
                    .Bit(2) = Flag2
                    .Bit(3) = Flag3
                    .Bit(4) = Flag4
                    .Bit(5) = Flag5
                    .Bit(6) = Flag6
                    .Bit(7) = Flag7

                    Dim parameter As Integer
                    If IsBox Then
                        parameter = ContainedItemID
                    Else
                        parameter = Quantity
                    End If
                    .Int(0, 8, 11) = parameter

                    .Int(0, 19, 11) = ID
                    .Int(0, 30, 3) = HeldBy

                End With
                Return out
            End Function

            Public Shared Function FromHeldItemBits(bits As Binary) As SkyItem
                Dim out As New SkyItem
                With bits
                    out.IsValid = .Bit(0)
                    out.Flag1 = .Bit(1)
                    out.Flag2 = .Bit(2)
                    out.Flag3 = .Bit(3)
                    out.Flag4 = .Bit(4)
                    out.Flag5 = .Bit(5)
                    out.Flag6 = .Bit(6)
                    out.Flag7 = .Bit(7)
                    Dim parameter = .Int(0, 8, 11)
                    out.ID = .Int(0, 19, 11)
                    out.HeldBy = .Int(0, 30, 3)
                    If out.IsBox Then
                        out.ContainedItemID = parameter
                        out.Quantity = 1
                    Else
                        out.ContainedItemID = Nothing
                        out.Quantity = parameter
                    End If
                End With
                Return out
            End Function

            Public Shared Function FromStoredItemParts(itemID As Integer, parameter As Integer)
                Dim out As New SkyItem
                out.IsValid = True
                out.ID = itemID
                If out.IsBox Then
                    out.ContainedItemID = parameter
                    out.Quantity = 1
                Else
                    out.ContainedItemID = Nothing
                    out.Quantity = parameter
                End If
                Return out
            End Function

            Public Sub New()

            End Sub

        End Class

        <Obsolete> Public Class HeldItem
            Inherits Binary
            Implements iItemOld
            Implements IItem

            Public Const Length As Integer = 33
            Public Const MimeType As String = "application/x-sky-item"

            Public Sub New(Bits As Binary)
                MyBase.New(Bits)
            End Sub
            Public Sub New(ID As Integer, Parameter As Integer)
                MyBase.New(Length)
                Me.ID = ID
                Me.Parameter = Parameter
                Me.IsValid = True
            End Sub

            ''' <summary>
            ''' The bit the game uses to determine whether this item is valid.
            ''' Must be true for the game to recognize it.
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property IsValid As Boolean
                Get
                    Return Bits(0)
                End Get
                Set(value As Boolean)
                    Bits(0) = value
                End Set
            End Property

            Public Property Flag1 As Boolean
                Get
                    Return Bits(1)
                End Get
                Set(value As Boolean)
                    Bits(1) = value
                End Set
            End Property
            Public Property Flag2 As Boolean
                Get
                    Return Bits(2)
                End Get
                Set(value As Boolean)
                    Bits(2) = value
                End Set
            End Property
            Public Property Flag3 As Boolean
                Get
                    Return Bits(3)
                End Get
                Set(value As Boolean)
                    Bits(3) = value
                End Set
            End Property
            Public Property Flag4 As Boolean
                Get
                    Return Bits(4)
                End Get
                Set(value As Boolean)
                    Bits(4) = value
                End Set
            End Property
            Public Property Flag5 As Boolean
                Get
                    Return Bits(5)
                End Get
                Set(value As Boolean)
                    Bits(5) = value
                End Set
            End Property
            Public Property Flag6 As Boolean
                Get
                    Return Bits(6)
                End Get
                Set(value As Boolean)
                    Bits(6) = value
                End Set
            End Property
            Public Property Flag7 As Boolean
                Get
                    Return Bits(7)
                End Get
                Set(value As Boolean)
                    Bits(7) = value
                End Set
            End Property

            ''' <summary>
            ''' The extra parameter for an item.
            '''
            ''' For boxes, this is the contained item.
            ''' For sticks, rocks, etc., this is the number of items in the stack.
            ''' For the Used TM, this is the contained move.
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property Parameter As UInt16 Implements iItemOld.Parameter
                Get
                    Return Int(0, 8, 11)
                End Get
                Set(value As UInt16)
                    Int(0, 8, 11) = value
                End Set
            End Property

            Public Property Quantity As Integer Implements IItem.Quantity
                Get
                    Return Parameter
                End Get
                Set(value As Integer)
                    Parameter = value
                End Set
            End Property

            ''' <summary>
            ''' The ID of the item.
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property ID As Integer Implements iItemOld.ID, IItem.ID
                Get
                    Return Int(0, 19, 11)
                End Get
                Set(value As Integer)
                    Int(0, 19, 11) = value
                End Set
            End Property

            ''' <summary>
            ''' The team member who is holding this item.
            '''
            ''' Must be 0-4.
            ''' 0 means held by no one.
            ''' 1-4 is the number of the active team member holding it.
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property HeldBy As Byte
                Get
                    Return Int(0, 30, 3)
                End Get
                Set(value As Byte)
                    Int(0, 30, 3) = value
                End Set
            End Property

            ''' <summary>
            ''' Determines whether or not the ID is in the range of Box items.
            ''' If this is true, the Parameter is the ID of the contained item.
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public ReadOnly Property IsBox As Boolean Implements iItemOld.IsBox
                Get
                    Return ID > 363 AndAlso ID < 400
                End Get
            End Property

            Public Overrides Function ToString() As String
                If IsValid Then
                    Dim output As New Text.StringBuilder
                    output.Append(Lists.GetSkyItemNames(ID))
                    If Parameter > 0 Then
                        If IsBox Then
                            output.Append(" (")
                            output.Append(Lists.GetSkyItemNames(Parameter))
                            output.Append(")")
                        Else
                            output.Append(" (")
                            output.Append(Parameter)
                            output.Append(")")
                        End If
                    End If
                    If HeldBy > 0 Then
                        output.Append(" [")
                        output.Append(My.Resources.Language.ItemToStringHeldBy)
                        output.Append(" ")
                        output.Append(HeldBy)
                        output.Append("]")
                    End If
                    Return output.ToString
                Else
                    Return "----------"
                End If
            End Function
        End Class

        <Obsolete> Public Class StoredItemOld
            Implements iItemOld
            Implements IItem
            Public Sub New(ID As Integer, Parameter As Integer)
                Me.ID = ID
                Me.Parameter = Parameter
            End Sub

            Public ReadOnly Property IsValid As Boolean
                Get
                    Return ID > 0
                End Get
            End Property

            ''' <summary>
            ''' The extra parameter for an item.
            '''
            ''' For boxes, this is the contained item.
            ''' For sticks, rocks, etc., this is the number of items in the stack.
            ''' For the Used TM, this is the contained move.
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property Parameter As UInt16 Implements iItemOld.Parameter

            ''' <summary>
            ''' The ID of the item.
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property ID As Integer Implements iItemOld.ID, IItem.ID
            ''' <summary>
            ''' Determines whether or not the ID is in the range of Box items.
            ''' If this is true, the Parameter is the ID of the contained item.
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public ReadOnly Property IsBox As Boolean Implements iItemOld.IsBox
                Get
                    Return ID > 363 AndAlso ID < 400
                End Get
            End Property

            Public Property Quantity As Integer Implements IItem.Quantity
                Get
                    Return Parameter
                End Get
                Set(value As Integer)
                    Parameter = value
                End Set
            End Property

            Public Overrides Function ToString() As String
                If ID > 0 Then
                    Dim output As New Text.StringBuilder
                    output.Append(Lists.GetSkyItemNames(ID))
                    If Parameter > 0 Then
                        If Me.IsBox Then
                            output.Append(" (")
                            output.Append(Lists.GetSkyItemNames(Parameter))
                            output.Append(")")
                        Else
                            output.Append(" (")
                            output.Append(Parameter)
                            output.Append(")")
                        End If
                    End If
                    Return output.ToString
                Else
                    Return "----------"
                End If
            End Function
        End Class

        Public Class Attack
            Inherits Binary
            Implements iMDAttack

            Public Const Length = 21
            Public Sub New(Bits As Binary)
                MyBase.New(Bits)
            End Sub
            Public Sub New(ID As Integer, Ginseng As Integer, IsSwitched As Boolean, IsLinked As Boolean, IsSet As Boolean)
                Me.New(New Binary(Length))
                Me.ID = ID
                Me.Ginseng = Ginseng
                Me.IsSwitched = IsSwitched
                Me.IsLinked = IsLinked
                Me.IsSet = IsSet
            End Sub
            Public Property IsValid As Boolean
                Get
                    Return Bits(0)
                End Get
                Set(value As Boolean)
                    Bits(0) = value
                End Set
            End Property
            Public Property IsLinked As Boolean Implements iMDAttack.IsLinked
                Get
                    Return Bits(1)
                End Get
                Set(value As Boolean)
                    Bits(1) = value
                End Set
            End Property
            Public Property IsSwitched As Boolean Implements iMDAttack.IsSwitched
                Get
                    Return Bits(2)
                End Get
                Set(value As Boolean)
                    Bits(2) = value
                End Set
            End Property
            Public Property IsSet As Boolean Implements iMDAttack.IsSet
                Get
                    Return Bits(3)
                End Get
                Set(value As Boolean)
                    Bits(3) = value
                End Set
            End Property
            Public Property ID As Integer Implements iMDAttack.ID
                Get
                    Return Int(0, 4, 10)
                End Get
                Set(value As Integer)
                    Int(0, 4, 10) = value
                End Set
            End Property
            Public Property Ginseng As Integer Implements iMDAttack.Ginseng
                Get
                    Return Int(0, 14, 7)
                End Get
                Set(value As Integer)
                    Int(0, 14, 7) = value
                End Set
            End Property
            Function GetAttackDictionary() As IDictionary(Of Integer, String) Implements iMDAttack.GetAttackDictionary
                Return Lists.GetSkyMoves
            End Function
        End Class

        Public Class StoredPkm
            Inherits Binary
            Implements iMDPkm
            Implements iMDPkmGender
            Implements iMDPkmMetFloor
            Implements iMDPkmIQ
            Implements iPkmAttack
            Implements ISavableAs
            Implements IOnDisk
            Implements IOpenableFile
            Public Const Length As Integer = 362
            Public Const MimeType As String = "application/x-sky-pokemon"
            Public Event FileSaved As ISavable.FileSavedEventHandler Implements ISavable.FileSaved

            Public Sub New(Bits As Binary)
                MyBase.New(Bits)
            End Sub
            Public Sub New()
                MyBase.New(Length)
            End Sub
            Public ReadOnly Property IsValid As Boolean Implements iMDPkm.IsValid
                Get
                    Return Bits(0)
                End Get
            End Property
            Public Property Level As Byte Implements iMDPkm.Level
                Get
                    Return Int(0, 1, 7)
                End Get
                Set(value As Byte)
                    Int(0, 1, 7) = value
                End Set
            End Property
            Public Property ID As Integer Implements iMDPkm.ID
                Get
                    Dim i = Int(0, 8, 11)
                    If i > 600 Then
                        i -= 600
                    End If
                    Return i
                End Get
                Set(value As Integer)
                    Dim f = IsFemale
                    Int(0, 8, 11) = value
                    IsFemale = f
                End Set
            End Property
            Public Property IsFemale As Boolean Implements iMDPkmGender.IsFemale
                Get
                    Dim i = Int(0, 8, 11)
                    Return (i > 600)
                End Get
                Set(value As Boolean)
                    Dim i = Int(0, 8, 11)
                    If i > 600 Then
                        If value Then
                            'do nothing
                        Else
                            Int(0, 8, 11) -= 600
                        End If
                    Else
                        If value Then
                            Int(0, 8, 11) += 600
                        Else
                            'do nothing
                        End If
                    End If
                End Set
            End Property
            Public Property MetAt As Integer Implements iMDPkm.MetAt
                Get
                    Return Int(0, 19, 8)
                End Get
                Set(value As Integer)
                    Int(0, 19, 8) = value
                End Set
            End Property
            Public Property MetFloor As Integer Implements iMDPkmMetFloor.MetFloor
                Get
                    Return Int(0, 27, 7)
                End Get
                Set(value As Integer)
                    Int(0, 27, 7) = value
                End Set
            End Property
            'Unknown Data: Length of 15 bits
            Public Property IQ As Integer Implements iMDPkmIQ.IQ
                Get
                    Return Int(0, 49, 10)
                End Get
                Set(value As Integer)
                    Int(0, 49, 10) = value
                End Set
            End Property
            Public Property HP As Integer Implements iMDPkm.MaxHP
                Get
                    Return Int(0, 59, 10)
                End Get
                Set(value As Integer)
                    Int(0, 59, 10) = value
                End Set
            End Property
            Public Property StatAttack As Integer Implements iMDPkm.StatAttack
                Get
                    Return Int(0, 69, 8)
                End Get
                Set(value As Integer)
                    Int(0, 69, 8) = value
                End Set
            End Property
            Public Property StatDefense As Integer Implements iMDPkm.StatDefense
                Get
                    Return Int(0, 77, 8)
                End Get
                Set(value As Integer)
                    Int(0, 77, 8) = value
                End Set
            End Property
            Public Property StatSpAttack As Integer Implements iMDPkm.StatSpAttack
                Get
                    Return Int(0, 85, 8)
                End Get
                Set(value As Integer)
                    Int(0, 85, 8) = value
                End Set
            End Property
            Public Property StatSpDefense As Integer Implements iMDPkm.StatSpDefense
                Get
                    Return Int(0, 93, 8)
                End Get
                Set(value As Integer)
                    Int(0, 93, 8) = value
                End Set
            End Property
            Public Property Exp As Integer Implements iMDPkm.Exp
                Get
                    Return Int(0, 101, 24)
                End Get
                Set(value As Integer)
                    Int(0, 101, 24) = value
                End Set
            End Property
            'Unknown Data: Length of 73 bits
            Public Property Attack1 As iAttack Implements iPkmAttack.Attack1
                Get
                    Return New Attack(Range(198, Attack.Length))
                End Get
                Set(value As iAttack)
                    Range(198, Attack.Length) = value
                End Set
            End Property
            Public Property Attack2 As iAttack Implements iPkmAttack.Attack2
                Get
                    Return New Attack(Range(219, Attack.Length))
                End Get
                Set(value As iAttack)
                    Range(219, Attack.Length) = value
                End Set
            End Property
            Public Property Attack3 As iAttack Implements iPkmAttack.Attack3
                Get
                    Return New Attack(Range(240, Attack.Length))
                End Get
                Set(value As iAttack)
                    Range(240, Attack.Length) = value
                End Set
            End Property
            Public Property Attack4 As iAttack Implements iPkmAttack.Attack4
                Get
                    Return New Attack(Range(261, Attack.Length))
                End Get
                Set(value As iAttack)
                    Range(261, Attack.Length) = value
                End Set
            End Property
            Public Property Name As String Implements iMDPkm.Name
                Get
                    Return StringPMD(0, 282, 10)
                End Get
                Set(value As String)
                    StringPMD(0, 282, 10) = value
                End Set
            End Property

            Public Property Filename As String Implements IOnDisk.Filename

            Public Overrides Function ToString() As String
                If IsValid Then
                    Return String.Format("{0} (Lvl. {1} {2})", Name, Level, Lists.GetSkyPokemon(ID))
                Else
                    Return "----------"
                End If
            End Function
            Public Function GetPokemonDictionary() As IDictionary(Of Integer, String) Implements iMDPkm.GetPokemonDictionary
                Return Lists.GetSkyPokemon
            End Function

            Public Function GetMetAtDictionary() As IDictionary(Of Integer, String) Implements iMDPkm.GetMetAtDictionary
                Return Lists.GetSkyLocations
            End Function

            Public Function GetDefaultExtension() As String Implements ISavableAs.GetDefaultExtension
                Return ".skypkm"
            End Function

            Public Async Function OpenFile(Filename As String, Provider As IOProvider) As Task Implements IOpenableFile.OpenFile
                Dim toOpen As New BinaryFile
                Await toOpen.OpenFile(Filename, Provider)
                Me.Bits = toOpen.Bits.Bits
                'matix2267's convention adds 6 bits to the beginning of a file so that the name will be byte-aligned
                For i = 1 To 8 - (Length Mod 8)
                    Me.Bits.RemoveAt(0)
                Next
            End Function

            Public Sub Save(provider As IOProvider) Implements ISavable.Save
                Save(Filename, provider)
            End Sub

            Public Sub Save(Filename As String, provider As IOProvider) Implements ISavableAs.Save
                Dim toSave As New BinaryFile()
                toSave.CreateFile(Path.GetFileNameWithoutExtension(Filename))
                'matix2267's convention adds 6 bits to the beginning of a file so that the name will be byte-aligned
                For i = 1 To 8 - (Length Mod 8)
                    toSave.Bits.Bits.Add(0)
                Next
                toSave.Bits.Bits.AddRange(Me.Bits)
                toSave.Save(Filename, provider)
            End Sub
        End Class

        Public Class ActiveAttack
            Inherits Binary
            Implements iMDActiveAttack
            Public Const Length = 29
            Public Sub New(Bits As Binary)
                MyBase.New(Bits)
            End Sub

#Region "Properties"
            Public Property IsValid As Boolean
                Get
                    Return Bits(0)
                End Get
                Set(value As Boolean)
                    Bits(0) = value
                End Set
            End Property
            Public Property IsLinked As Boolean Implements iMDActiveAttack.IsLinked
                Get
                    Return Bits(1)
                End Get
                Set(value As Boolean)
                    Bits(1) = value
                End Set
            End Property
            Public Property IsSwitched As Boolean Implements iMDActiveAttack.IsSwitched
                Get
                    Return Bits(2)
                End Get
                Set(value As Boolean)
                    Bits(2) = value
                End Set
            End Property
            Public Property IsSet As Boolean Implements iMDActiveAttack.IsSet
                Get
                    Return Bits(3)
                End Get
                Set(value As Boolean)
                    Bits(3) = value
                End Set
            End Property
            Public Property IsSealed As Boolean Implements iMDActiveAttack.IsSealed
                Get
                    Return Bits(4)
                End Get
                Set(value As Boolean)
                    Bits(4) = value
                End Set
            End Property
            Public Property ID As Integer Implements iAttack.ID
                Get
                    Return Int(0, 5, 10)
                End Get
                Set(value As Integer)
                    Int(0, 5, 10) = value
                End Set
            End Property
            Public Property PP As Integer Implements iMDActiveAttack.PP
                Get
                    Return Int(0, 15, 7)
                End Get
                Set(value As Integer)
                    Int(0, 15, 7) = value
                End Set
            End Property
            Public Property Ginseng As Integer Implements iMDActiveAttack.Ginseng
                Get
                    Return Int(0, 22, 7)
                End Get
                Set(value As Integer)
                    Int(0, 22, 7) = value
                End Set
            End Property
#End Region

            Function GetAttackDictionary() As IDictionary(Of Integer, String) Implements iAttack.GetAttackDictionary
                Return Lists.GetSkyMoves
            End Function
        End Class

        Public Class ActivePkm
            Inherits Binary
            Implements Interfaces.iMDPkm
            Implements Interfaces.iPkmAttack
            Implements Interfaces.iMDPkmMetFloor
            Implements Interfaces.iMDPkmCurrentHP
            Implements Interfaces.iMDPkmGender
            Implements Interfaces.iMDPkmIQ
            Public Const Length As Integer = 0
            Public Sub New(Bits As Binary)
                MyBase.New(Bits)
            End Sub

#Region "Properties"
            Public ReadOnly Property IsValid As Boolean Implements iMDPkm.IsValid
                Get
                    Return Bits(0)
                End Get
            End Property
            'Unknown data: 5 bits
            Public Property Level As Byte Implements iMDPkm.Level
                Get
                    Return Int(0, 5, 7)
                End Get
                Set(value As Byte)
                    Int(0, 5, 7) = value
                End Set
            End Property
            Public Property MetAt As Integer Implements iMDPkm.MetAt
                Get
                    Return Int(0, 12, 8)
                End Get
                Set(value As Integer)
                    Int(0, 12, 8) = value
                End Set
            End Property
            Public Property MetFloor As Integer Implements iMDPkmMetFloor.MetFloor
                Get
                    Return Int(0, 20, 7)
                End Get
                Set(value As Integer)
                    Int(0, 20, 7) = value
                End Set
            End Property
            'Unknown data: 1 bit
            Public Property IQ As Integer Implements iMDPkmIQ.IQ
                Get
                    Return Int(0, 28, 10)
                End Get
                Set(value As Integer)
                    Int(0, 28, 10) = value
                End Set
            End Property
            Public Property RosterNumber As UInt16
                Get
                    Return Int(0, 38, 10)
                End Get
                Set(value As UInt16)
                    Int(0, 38, 10) = value
                End Set
            End Property
            'Unknown Data: 22 bits
            Public Property ID As Integer Implements iMDPkm.ID
                Get
                    Dim i = Int(0, 70, 11)
                    If i > 600 Then
                        i -= 600
                    End If
                    Return i
                End Get
                Set(value As Integer)
                    Dim f = IsFemale
                    Int(0, 70, 11) = value
                    IsFemale = f
                End Set
            End Property
            Public Property IsFemale As Boolean Implements iMDPkmGender.IsFemale
                Get
                    Dim i = Int(0, 70, 11)
                    Return (i > 600)
                End Get
                Set(value As Boolean)
                    Dim i = Int(0, 70, 11)
                    If i > 600 Then
                        If value Then
                            'do nothing
                        Else
                            Int(0, 70, 11) -= 600
                        End If
                    Else
                        If value Then
                            Int(0, 70, 11) += 600
                        Else
                            'do nothing
                        End If
                    End If
                End Set
            End Property
            Public Property HP1 As Integer Implements iMDPkmCurrentHP.CurrentHP
                Get
                    Return Int(0, 81, 10)
                End Get
                Set(value As Integer)
                    Int(0, 81, 10) = value
                End Set
            End Property
            Public Property HP2 As Integer Implements iMDPkm.MaxHP
                Get
                    Return Int(0, 91, 10)
                End Get
                Set(value As Integer)
                    Int(0, 91, 10) = value
                End Set
            End Property
            Public Property StatAttack As Integer Implements iMDPkm.StatAttack
                Get
                    Return Int(0, 101, 8)
                End Get
                Set(value As Integer)
                    Int(0, 101, 8) = value
                End Set
            End Property
            Public Property StatDefense As Integer Implements iMDPkm.StatDefense
                Get
                    Return Int(0, 109, 8)
                End Get
                Set(value As Integer)
                    Int(0, 109, 8) = value
                End Set
            End Property
            Public Property StatSpAttack As Integer Implements iMDPkm.StatSpAttack
                Get
                    Return Int(0, 117, 8)
                End Get
                Set(value As Integer)
                    Int(0, 117, 8) = value
                End Set
            End Property
            Public Property StatSpDefense As Integer Implements iMDPkm.StatSpDefense
                Get
                    Return Int(0, 125, 8)
                End Get
                Set(value As Integer)
                    Int(0, 125, 8) = value
                End Set
            End Property
            Public Property Exp As Integer Implements iMDPkm.Exp
                Get
                    Return Int(0, 133, 24)
                End Get
                Set(value As Integer)
                    Int(0, 133, 24) = value
                End Set
            End Property
            Public Property Attack1 As Interfaces.iAttack Implements iPkmAttack.Attack1
                Get
                    Return New ActiveAttack(Range(157, ActiveAttack.Length))
                End Get
                Set(value As Interfaces.iAttack)
                    Range(157, ActiveAttack.Length) = value
                End Set
            End Property
            Public Property Attack2 As Interfaces.iAttack Implements iPkmAttack.Attack2
                Get
                    Return New ActiveAttack(Range(186, ActiveAttack.Length))
                End Get
                Set(value As Interfaces.iAttack)
                    Range(186, ActiveAttack.Length) = value
                End Set
            End Property
            Public Property Attack3 As Interfaces.iAttack Implements iPkmAttack.Attack3
                Get
                    Return New ActiveAttack(Range(215, ActiveAttack.Length))
                End Get
                Set(value As Interfaces.iAttack)
                    Range(215, ActiveAttack.Length) = value
                End Set
            End Property
            Public Property Attack4 As Interfaces.iAttack Implements iPkmAttack.Attack4
                Get
                    Return New ActiveAttack(Range(244, ActiveAttack.Length))
                End Get
                Set(value As Interfaces.iAttack)
                    Range(244, ActiveAttack.Length) = value
                End Set
            End Property
            'Unknown data: 193 bits
            Public Property Name As String Implements iMDPkm.Name
                Get
                    Return StringPMD(0, 466, 10)
                End Get
                Set(value As String)
                    StringPMD(0, 466, 10) = value
                End Set
            End Property
#End Region

            Public Overrides Function ToString() As String
                If IsValid Then
                    Return String.Format("{0} (Lvl. {1} {2})", Name, Level, Lists.GetSkyPokemon(ID))
                Else
                    Return "----------"
                End If
            End Function

            Public Function GetMetAtDictionary() As IDictionary(Of Integer, String) Implements iMDPkm.GetMetAtDictionary
                Return Lists.GetSkyLocations
            End Function

            Public Function GetPokemonDictionary() As IDictionary(Of Integer, String) Implements iMDPkm.GetPokemonDictionary
                Return Lists.GetSkyPokemon
            End Function
        End Class

        Public Class QuicksaveAttack
            Inherits Binary
            Implements iMDActiveAttack
            Public Const Length = 48
            Public Sub New(Bits As Binary)
                MyBase.New(Bits)
            End Sub
            Public Property IsValid As Boolean
                Get
                    Return Bits(0)
                End Get
                Set(value As Boolean)
                    Bits(0) = value
                End Set
            End Property
            Public Property IsLinked As Boolean Implements iMDActiveAttack.IsLinked
                Get
                    Return Bits(1)
                End Get
                Set(value As Boolean)
                    Bits(1) = value
                End Set
            End Property
            Public Property IsSwitched As Boolean Implements iMDActiveAttack.IsSwitched
                Get
                    Return Bits(2)
                End Get
                Set(value As Boolean)
                    Bits(2) = value
                End Set
            End Property
            Public Property IsSet As Boolean Implements iMDActiveAttack.IsSet
                Get
                    Return Bits(3)
                End Get
                Set(value As Boolean)
                    Bits(3) = value
                End Set
            End Property
            Public Property IsSealed As Boolean Implements iMDActiveAttack.IsSealed
                Get
                    Return Bits(4)
                End Get
                Set(value As Boolean)
                    Bits(4) = value
                End Set
            End Property
            'Unknown flags: 3 bits
            'Unknown data: 8 bits
            Public Property ID As Integer Implements iMDActiveAttack.ID
                Get
                    Return Int(0, 16, 16)
                End Get
                Set(value As Integer)
                    Int(0, 16, 16) = value
                End Set
            End Property
            Public Property PP As Integer Implements iMDActiveAttack.PP
                Get
                    Return Int(0, 32, 8)
                End Get
                Set(value As Integer)
                    Int(0, 32, 8) = value
                End Set
            End Property
            Public Property Ginseng As Integer Implements iMDActiveAttack.Ginseng
                Get
                    Return Int(0, 40, 8)
                End Get
                Set(value As Integer)
                    Int(0, 40, 8) = value
                End Set
            End Property

            Public Function GetAttackDictionary() As IDictionary(Of Integer, String) Implements iAttack.GetAttackDictionary
                Return Lists.GetSkyMoves
            End Function
        End Class

        Public Class QuicksavePkm
            Inherits Binary
            Implements iPkmAttack
            Public Const Length As Integer = 429 * 8
            Public Sub New(Bits As Binary)
                MyBase.New(Bits)
            End Sub
            Public Sub New()
                MyBase.New(New Binary(Length))
            End Sub
            Public ReadOnly Property IsValid As Boolean
                Get
                    Return ID > 0
                End Get
            End Property
            'Unknown data: 80 bits
            Public Property TransformedID As Integer
                Get
                    Dim i = Int(0, 80, 16)
                    If i > 600 Then
                        i -= 600
                    End If
                    Return i
                End Get
                Set(value As Integer)
                    Dim f = IsFemale
                    Int(0, 80, 16) = value
                    IsFemale = f
                End Set
            End Property
            Public Property TransformedIsFemale As Boolean
                Get
                    Dim i = Int(0, 80, 16)
                    Return (i > 600)
                End Get
                Set(value As Boolean)
                    Dim i = Int(0, 80, 16)
                    If i > 600 Then
                        If value Then
                            'do nothing
                        Else
                            Int(0, 80, 16) -= 600
                        End If
                    Else
                        If value Then
                            Int(0, 80, 16) += 600
                        Else
                            'do nothing
                        End If
                    End If
                End Set
            End Property
            Public Property ID As Integer
                Get
                    Dim i = Int(0, 96, 16)
                    If i > 600 Then
                        i -= 600
                    End If
                    Return i
                End Get
                Set(value As Integer)
                    Dim f = IsFemale
                    Int(0, 96, 16) = value
                    IsFemale = f
                End Set
            End Property
            Public Property IsFemale As Boolean
                Get
                    Dim i = Int(0, 96, 16)
                    Return (i > 600)
                End Get
                Set(value As Boolean)
                    Dim i = Int(0, 96, 16)
                    If i > 600 Then
                        If value Then
                            'do nothing
                        Else
                            Int(0, 96, 16) -= 600
                        End If
                    Else
                        If value Then
                            Int(0, 96, 16) += 600
                        Else
                            'do nothing
                        End If
                    End If
                End Set
            End Property
            'Unknown data
            Public Property Level As Byte
                Get
                    Return Int(0, 144, 8)
                End Get
                Set(value As Byte)
                    Int(0, 144, 8) = value
                End Set
            End Property
            'Unknown data
            Public Property CurrentHP As UInt16
                Get
                    Return Int(0, 192, 16)
                End Get
                Set(value As UInt16)
                    Int(0, 192, 16) = value
                End Set
            End Property
            Public Property BaseHP As UInt16
                Get
                    Return Int(0, 208, 16)
                End Get
                Set(value As UInt16)
                    Int(0, 208, 16) = value
                End Set
            End Property
            Public Property HPBoost As UInt16
                Get
                    Return Int(0, 224, 16)
                End Get
                Set(value As UInt16)
                    Int(0, 224, 16) = value
                End Set
            End Property
            'Unknown Data:
            Public Property StatAttack As Byte
                Get
                    Return Int(0, 256, 8)
                End Get
                Set(value As Byte)
                    Int(0, 256, 8) = value
                End Set
            End Property
            Public Property StatDefense As Byte
                Get
                    Return Int(0, 264, 8)
                End Get
                Set(value As Byte)
                    Int(0, 264, 8) = value
                End Set
            End Property
            Public Property StatSpAttack As Byte
                Get
                    Return Int(0, 272, 8)
                End Get
                Set(value As Byte)
                    Int(0, 272, 8) = value
                End Set
            End Property
            Public Property StatSpDefense As Byte
                Get
                    Return Int(0, 280, 8)
                End Get
                Set(value As Byte)
                    Int(0, 280, 8) = value
                End Set
            End Property
            Public Property Exp As Integer
                Get
                    Return Int(0, 288, 32)
                End Get
                Set(value As Integer)
                    Int(0, 288, 32) = value
                End Set
            End Property
            'Unknown Data
            Public Property Attack1 As iAttack Implements iPkmAttack.Attack1
                Get
                    Return New QuicksaveAttack(Range(2696, QuicksaveAttack.Length))
                End Get
                Set(value As iAttack)
                    Range(2696, QuicksaveAttack.Length) = value
                End Set
            End Property
            Public Property Attack2 As iAttack Implements iPkmAttack.Attack2
                Get
                    Return New QuicksaveAttack(Range(2696 + 1 * QuicksaveAttack.Length, QuicksaveAttack.Length))
                End Get
                Set(value As iAttack)
                    Range(2696 + 1 * QuicksaveAttack.Length, QuicksaveAttack.Length) = value
                End Set
            End Property
            Public Property Attack3 As iAttack Implements iPkmAttack.Attack3
                Get
                    Return New QuicksaveAttack(Range(2696 + 2 * QuicksaveAttack.Length, QuicksaveAttack.Length))
                End Get
                Set(value As iAttack)
                    Range(2696 + 2 * QuicksaveAttack.Length, QuicksaveAttack.Length) = value
                End Set
            End Property
            Public Property Attack4 As iAttack Implements iPkmAttack.Attack4
                Get
                    Return New QuicksaveAttack(Range(2696 + 3 * QuicksaveAttack.Length, QuicksaveAttack.Length))
                End Get
                Set(value As iAttack)
                    Range(2696 + 3 * QuicksaveAttack.Length, QuicksaveAttack.Length) = value
                End Set
            End Property
            Public Overrides Function ToString() As String
                If IsValid Then
                    Return String.Format("Lvl. {0} {1}", Level, Lists.GetSkyPokemon(ID))
                Else
                    Return "----------"
                End If
            End Function
        End Class

#End Region

        Public Sub New()
            MyBase.New

            'Init Items
            StoredItems = New ObservableCollection(Of SkyItem)
            HeldItems = New ObservableCollection(Of SkyItem)
            SpEpisodeHeldItems = New ObservableCollection(Of SkyItem)
            InitItemSlots()
        End Sub

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Public Overrides Async Function OpenFile(Filename As String, Provider As IOProvider) As Task
            Await MyBase.OpenFile(Filename, Provider)

            'Load items
            StoredItems.Clear()
            For Each item In GetStoredItems()
                StoredItems.Add(item)
            Next

            HeldItems.Clear()
            For Each item In GetHeldItems()
                HeldItems.Add(item)
            Next

            SpEpisodeHeldItems.Clear()
            For Each item In GetSpEpisodeHeldItems()
                SpEpisodeHeldItems.Add(item)
            Next

            LoadHistory()

        End Function

        Public Overrides Sub Save(Destination As String, provider As IOProvider)
            SetStoredItems(StoredItems)
            SetHeldItems(HeldItems)
            SetSpEpisodeHeldItems(SpEpisodeHeldItems)

            SaveHistory()

            MyBase.Save(Destination, provider)
        End Sub

#Region "Save Interaction"

#Region "General"
        ''' <summary>
        ''' Gets or sets the save file's Team Name.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property TeamName As String
            Get
                Return Bits.StringPMD(0, Offsets.TeamNameStart, Offsets.TeamNameLength)
            End Get
            Set(value As String)
                Bits.StringPMD(0, Offsets.TeamNameStart, Offsets.TeamNameLength) = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the held money in the main game
        ''' </summary>
        ''' <returns></returns>
        Public Property HeldMoney As Integer
            Get
                Return Bits.Int(0, Offsets.HeldMoney, 24)
            End Get
            Set(value As Integer)
                Bits.Int(0, Offsets.HeldMoney, 32) = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the held money in the active special episode
        ''' </summary>
        ''' <returns></returns>
        Public Property SpEpisodeHeldMoney As Integer
            Get
                Return Bits.Int(0, Offsets.SPHeldMoney, 24)
            End Get
            Set(value As Integer)
                Bits.Int(0, Offsets.SPHeldMoney, 32) = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the money in storage
        ''' </summary>
        ''' <returns></returns>
        Public Property StoredMoney As Integer
            Get
                Return Bits.Int(0, Offsets.StoredMoney, 24)
            End Get
            Set(value As Integer)
                Bits.Int(0, Offsets.StoredMoney, 24) = value
            End Set
        End Property
#End Region

#Region "Adventure Log"

        ''' <summary>
        ''' Gets or sets the number of adventures the team has had.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>This is displayed as a signed integer in-game, so if this is set to a negative number, it will appear negative.</remarks>
        Public Property Adventures As Integer
            Get
                Return Bits.Int(0, Offsets.Adventures, 32)
            End Get
            Set(value As Integer)
                Bits.Int(0, Offsets.Adventures, 32) = value
            End Set
        End Property
#End Region

#Region "Team Info"

        ''' <summary>
        ''' Gets or sets the team's exploration rank points.
        ''' When set in certain ranges, the rank changes (ex. Silver, Gold, Master, etc).
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ExplorerRank As Integer
            Get
                Return Bits.Int(0, Offsets.ExplorerRank, 32)
            End Get
            Set(value As Integer)
                Bits.Int(0, Offsets.ExplorerRank, 32) = value
            End Set
        End Property

#End Region

#Region "Items"

#Region "Stored"
        Public Property StoredItems As ObservableCollection(Of SkyItem)

        Private Function GetStoredItems() As List(Of SkyItem)
            Dim ids = Bits.Range(Offsets.StoredItemOffset, 11 * Offsets.StoredItemNumber)
            Dim params = Bits.Range(Offsets.StoredItemOffset + 11 * Offsets.StoredItemNumber, 11 * Offsets.StoredItemNumber)
            Dim items As New List(Of SkyItem)
            For count As Integer = 0 To 999
                Dim id = ids.NextInt(11)
                Dim p = params.NextInt(11)
                If id > 0 Then
                    items.Add(SkyItem.FromStoredItemParts(id, p))
                Else
                    Exit For
                End If
            Next
            Return items
        End Function

        Private Sub SetStoredItems(Value As IList(Of SkyItem))
            Dim ids As New Binary(11 * Offsets.StoredItemNumber)
            Dim params As New Binary(11 * Offsets.StoredItemNumber)
            For count As Integer = 0 To Offsets.StoredItemNumber - 1
                If Value.Count > count Then
                    ids.NextInt(11) = Value(count).ID
                    params.NextInt(11) = Value(count).GetParameter
                Else
                    ids.NextInt(11) = 0
                    params.NextInt(11) = 0
                End If
            Next
            Bits.Range(Offsets.StoredItemOffset, 11 * Offsets.StoredItemNumber) = ids
            Bits.Range(Offsets.StoredItemOffset + 11 * Offsets.StoredItemNumber, 11 * Offsets.StoredItemNumber) = params
        End Sub
#End Region

#Region "Held"
        Public Property HeldItems As ObservableCollection(Of SkyItem)

        Private Function GetHeldItems() As List(Of SkyItem)
            Dim output As New List(Of SkyItem)

            For count As Integer = 0 To Offsets.HeldItemNumber - 1
                Dim item = SkyItem.FromHeldItemBits(Me.Bits.Range(Offsets.HeldItemOffset + count * Offsets.HeldItemLength, Offsets.HeldItemLength))
                If item.IsValid Then
                    output.Add(item)
                Else
                    Exit For
                End If
            Next

            Return output
        End Function

        Private Sub SetHeldItems(items As IList(Of SkyItem))
            For count As Integer = 0 To Offsets.HeldItemNumber - 1
                Dim index = Offsets.HeldItemOffset + count * Offsets.HeldItemLength
                If items.Count > count Then
                    Me.Bits.Range(index, Offsets.HeldItemLength) = items(count).GetHeldItemBits
                Else
                    Me.Bits.Range(index, Offsets.HeldItemLength) = New Binary(Offsets.HeldItemLength)
                End If
            Next
        End Sub
#End Region

#Region "Special Episode Held"
        Public Property SpEpisodeHeldItems As ObservableCollection(Of SkyItem)

        Private Function GetSpEpisodeHeldItems() As List(Of SkyItem)
            Dim output As New List(Of SkyItem)

            For count As Integer = Offsets.HeldItemNumber To Offsets.HeldItemNumber + Offsets.HeldItemNumber - 1
                Dim item = SkyItem.FromHeldItemBits(Me.Bits.Range(Offsets.HeldItemOffset + count * Offsets.HeldItemLength, Offsets.HeldItemLength))
                If item.IsValid Then
                    output.Add(item)
                Else
                    Exit For
                End If
            Next

            Return output
        End Function

        Private Sub SetSpEpisodeHeldItems(items As IList(Of SkyItem))
            For count As Integer = Offsets.HeldItemNumber To Offsets.HeldItemNumber + Offsets.HeldItemNumber - 1
                Dim index = Offsets.HeldItemOffset + count * Offsets.HeldItemLength
                If items.Count > count Then
                    Me.Bits.Range(index, Offsets.HeldItemLength) = items(count).GetHeldItemBits
                Else
                    Me.Bits.Range(index, Offsets.HeldItemLength) = New Binary(Offsets.HeldItemLength)
                End If
            Next
        End Sub
#End Region

        Public Property ItemSlots As IEnumerable(Of IItemSlot) Implements IInventory.ItemSlots
            Get
                Return _itemSlots
            End Get
            Private Set(value As IEnumerable(Of IItemSlot))
                _itemSlots = value
            End Set
        End Property
        Dim _itemSlots As ObservableCollection(Of IItemSlot)

        Private Sub InitItemSlots()
            Dim slots As New ObservableCollection(Of IItemSlot)
            slots.Add(New ItemSlot(My.Resources.Language.StoredItemsSlot, StoredItems, Offsets.StoredItemNumber))
            slots.Add(New ItemSlot(My.Resources.Language.HeldItemsSlot, HeldItems, Offsets.HeldItemNumber))
            slots.Add(New ItemSlot(My.Resources.Language.EpisodeHeldItems, SpEpisodeHeldItems, Offsets.HeldItemNumber))
            ItemSlots = slots
        End Sub
#End Region

#Region "Stored Pokemon"
        Public Property StoredPokemon(Index As Integer) As StoredPkm
            Get
                Return New StoredPkm(Bits.Range(Offsets.StoredPokemonOffset + Index * Offsets.StoredPokemonLength, Offsets.StoredPokemonLength))
            End Get
            Set(value As StoredPkm)
                Bits.Range(Offsets.StoredPokemonOffset + Index * Offsets.StoredPokemonLength, Offsets.StoredPokemonLength) = value
            End Set
        End Property
        Public Property StoredPokemon() As StoredPkm()
            Get
                Dim output As New List(Of StoredPkm)
                For count As Integer = 0 To Offsets.StoredPokemonNumber - 1
                    Dim i = StoredPokemon(count)
                    'If i.IsValid OrElse count < 9 Then 'Excepting when count < 9 because the first 8 pokemon slots are special
                    output.Add(i)
                    'End If
                Next
                Return output.ToArray
            End Get
            Set(value As StoredPkm())
                For count As Integer = 0 To Offsets.StoredPokemonNumber - 1
                    If value.Length > count Then
                        StoredPokemon(count) = value(count)
                    Else
                        StoredPokemon(count) = New StoredPkm(New Binary(Offsets.StoredPokemonLength))
                    End If
                Next
            End Set
        End Property
        Public Function GetPokemon() As iMDPkm() Implements iPokemonStorage.GetPokemon
            Return StoredPokemon
        End Function

        Public Function GetStoredPokemonOffsets() As StoredPokemonSlotDefinition() Implements iPokemonStorage.GetStoredPokemonOffsets
            Return StoredPokemonSlotDefinition.FromLines(My.Resources.ListResources.SkyFriendAreaOffsets).ToArray
        End Function

        Public Sub SetPokemon(Pokemon() As iMDPkm) Implements iPokemonStorage.SetPokemon
            StoredPokemon = Pokemon
        End Sub
#End Region

#Region "Active Pokemon"

        Public Property ActivePokemon(Index As Integer) As ActivePkm
            Get
                Return New ActivePkm(Me.Bits.Range(Offsets.ActivePokemonOffset + Index * Offsets.ActivePokemonLength, Offsets.ActivePokemonLength))
            End Get
            Set(value As ActivePkm)
                Me.Bits.Range(Offsets.ActivePokemonOffset + Index * Offsets.ActivePokemonLength, Offsets.ActivePokemonLength) = value
            End Set
        End Property
        Public Property ActivePokemon() As iMDPkm()
            Get
                Dim output As New List(Of ActivePkm)
                For count As Integer = 0 To Offsets.ActivePokemonNumber - 1
                    Dim i = ActivePokemon(count)
                    If i.IsValid OrElse count < 9 Then 'Excepting when count < 9 because the first 8 pokemon slots are special
                        output.Add(i)
                    End If
                Next
                Return output.ToArray
            End Get
            Set(value As iMDPkm())
                For count As Integer = 0 To Offsets.ActivePokemonNumber - 1
                    If value.Length > count Then
                        ActivePokemon(count) = value(count)
                    Else
                        ActivePokemon(count) = New ActivePkm(New Binary(Offsets.ActivePokemonLength))
                    End If
                Next
            End Set
        End Property

        Public Property SpEpisodeActivePokemon(Index As Integer) As ActivePkm
            Get
                Return New ActivePkm(Me.Bits.Range(Offsets.SpActivePokemonOffset + Index * Offsets.ActivePokemonLength, Offsets.ActivePokemonLength))
            End Get
            Set(value As ActivePkm)
                Me.Bits.Range(Offsets.SpActivePokemonOffset + Index * Offsets.ActivePokemonLength, Offsets.ActivePokemonLength) = value
            End Set
        End Property
        Public Property SpEpisodeActivePokemon() As iMDPkm()
            Get
                Dim output As New List(Of ActivePkm)
                For count As Integer = 0 To Offsets.ActivePokemonNumber - 1
                    Dim i = SpEpisodeActivePokemon(count)
                    If i.IsValid OrElse count < 9 Then 'Excepting when count < 9 because the first 8 pokemon slots are special
                        output.Add(i)
                    End If
                Next
                Return output.ToArray
            End Get
            Set(value As iMDPkm())
                For count As Integer = 0 To Offsets.ActivePokemonNumber - 1
                    If value.Length > count Then
                        SpEpisodeActivePokemon(count) = value(count)
                    Else
                        SpEpisodeActivePokemon(count) = New ActivePkm(New Binary(Offsets.ActivePokemonLength))
                    End If
                Next
            End Set
        End Property

        Public Function GetActivePokemon() As Interfaces.iMDPkm() Implements Interfaces.iParty.GetPokemon
            Return ActivePokemon
        End Function

        Public Sub SetActivePokemon(Pokemon() As Interfaces.iMDPkm) Implements Interfaces.iParty.SetPokemon
            ActivePokemon = Pokemon
        End Sub
#End Region

#Region "Quicksave Pokemon"
        Public Property QuicksavePokemon(Index As Integer) As QuicksavePkm
            Get
                Return New QuicksavePkm(Bits.Range(Offsets.QuicksavePokemonOffset + Offsets.QuicksavePokemonLength * Index, Offsets.QuicksavePokemonLength))
            End Get
            Set(value As QuicksavePkm)
                Bits.Range(Offsets.QuicksavePokemonOffset + Offsets.QuicksavePokemonLength * Index, Offsets.QuicksavePokemonLength) = value
            End Set
        End Property

        Public Property QuicksavePokemon As QuicksavePkm()
            Get
                Dim out As New List(Of QuicksavePkm)
                For count = 0 To Offsets.QuicksavePokemonNumber - 1
                    out.Add(QuicksavePokemon(count))
                Next
                Return out.ToArray
            End Get
            Set(value As QuicksavePkm())
                For count = 0 To Offsets.QuicksavePokemonNumber - 1
                    If value.Length > count Then
                        QuicksavePokemon(count) = value(count)
                    Else
                        QuicksavePokemon(count) = New QuicksavePkm()
                    End If
                Next
            End Set
        End Property
#End Region

#Region "History"
        Private Sub LoadHistory()
            '----------
            '--History
            '----------

            '-----Original Player ID & Gender
            Dim rawOriginalPlayerID = Bits.Int(&HBE, 0, 16)
            If rawOriginalPlayerID > 600 Then
                OriginalPlayerID = rawOriginalPlayerID - 600
                OriginalPlayerIsFemale = True
            Else
                OriginalPlayerID = rawOriginalPlayerID
                OriginalPlayerIsFemale = False
            End If

            '-----Original Partner ID & Gender
            Dim rawOriginalPartnerID = Bits.Int(&HC0, 0, 16)
            If rawOriginalPartnerID > 600 Then
                OriginalPartnerID = rawOriginalPartnerID - 600
                OriginalPartnerIsFemale = True
            Else
                OriginalPartnerID = rawOriginalPartnerID
                OriginalPartnerIsFemale = False
            End If

            '-----Original Names
            OriginalPlayerName = Bits.StringPMD(&H13F, 0, 10)
            OriginalPartnerName = Bits.StringPMD(&H149, 0, 10)
        End Sub

        Private Sub SaveHistory()
            '----------
            '--History
            '----------

            '-----Original Player ID & Gender
            Dim rawOriginalPlayerID = OriginalPlayerID
            If OriginalPlayerIsFemale Then
                rawOriginalPlayerID += 600
            End If
            Bits.Int(&HBE, 0, 16) = rawOriginalPlayerID

            '-----Original Partner ID & Gender
            Dim rawOriginalPartnerID = OriginalPartnerIsFemale
            If OriginalPartnerIsFemale Then
                rawOriginalPartnerID += 600
            End If
            Bits.Int(&HC0, 0, 16) = rawOriginalPlayerID

            '-----Original Names
            Bits.StringPMD(&H13F, 0, 10) = OriginalPlayerName
            Bits.StringPMD(&H149, 0, 10) = OriginalPartnerName
        End Sub

        ''' <summary>
        ''' Gets or sets the original player Pokemon.
        ''' Used in-game for special episodes.
        ''' </summary>
        ''' <returns></returns>
        Public Property OriginalPlayerID As Integer
            Get
                Return _originalPlayerID
            End Get
            Set(value As Integer)
                If Not value = _originalPlayerID Then
                    _originalPlayerID = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(OriginalPartnerID)))
                End If
            End Set
        End Property
        Dim _originalPlayerID As Integer

        ''' <summary>
        ''' Gets or sets the original player gender.
        ''' Used in-game for special episodes.
        ''' </summary>
        ''' <returns></returns>
        Public Property OriginalPlayerIsFemale As Boolean
            Get
                Return _originalPlayerIsFemale
            End Get
            Set(value As Boolean)
                If Not value = _originalPlayerIsFemale Then
                    _originalPlayerIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(OriginalPlayerIsFemale)))
                End If
            End Set
        End Property
        Dim _originalPlayerIsFemale As Boolean

        ''' <summary>
        ''' Gets or sets the original partner Pokemon.
        ''' Used in-game for special episodes.
        ''' </summary>
        ''' <returns></returns>
        Public Property OriginalPartnerID As Integer
            Get
                Return _originalPartnerID
            End Get
            Set(value As Integer)
                If Not _originalPartnerID = value Then
                    _originalPartnerID = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(OriginalPartnerID)))
                End If
            End Set
        End Property
        Dim _originalPartnerID As Integer

        ''' <summary>
        ''' Gets or sets the original partner gender.
        ''' Used in-game for special episodes.
        ''' </summary>
        ''' <returns></returns>
        Public Property OriginalPartnerIsFemale As Boolean
            Get
                Return _originalPartnerIsFemale
            End Get
            Set(value As Boolean)
                If Not _originalPartnerIsFemale = value Then
                    _originalPartnerIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(OriginalPartnerIsFemale)))
                End If
            End Set
        End Property
        Dim _originalPartnerIsFemale As Boolean

        ''' <summary>
        ''' Gets or sets the original player name.
        ''' Used in-game for special episodes.
        ''' </summary>
        ''' <returns></returns>
        Public Property OriginalPlayerName As String
            Get
                Return _originalPlayerName
            End Get
            Set(value As String)
                If Not _originalPlayerName = value Then
                    _originalPlayerName = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(OriginalPlayerName)))
                End If
            End Set
        End Property
        Dim _originalPlayerName As String

        ''' <summary>
        ''' Gets or sets the original partner name.
        ''' Used in-game for special episodes.
        ''' </summary>
        ''' <returns></returns>
        Public Property OriginalPartnerName As String
            Get
                Return _originalPartnerName
            End Get
            Set(value As String)
                If Not _originalPartnerName = value Then
                    _originalPartnerName = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(OriginalPartnerName)))
                End If
            End Set
        End Property
        Dim _originalPartnerName As String
#End Region

#Region "Settings"
        ''' <summary>
        ''' Gets or sets the type of window frame used in the game.  Must be 1-5.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property WindowFrameType As Byte
            Get
                Return Bits.Int(0, Offsets.WindowFrameType, 3)
            End Get
            Set(value As Byte)
                Bits.Int(0, Offsets.WindowFrameType, 3) = value
            End Set
        End Property
#End Region

#End Region

#Region "Technical Stuff"

        ''' <summary>
        ''' Fixes the save file's checksum to reflect any changes made
        ''' </summary>
        Protected Overrides Sub FixChecksum()
            'Fix the first checksum
            Dim buffer = BitConverter.GetBytes(Checksums.Calculate32BitChecksum(Bits, 4, Offsets.ChecksumEnd))
            For count = 0 To 3
                Bits.Int(count, 0, 8) = buffer(count)
            Next

            'Ensure backup save matches.
            'Not strictly required, as the first one will be loaded if it's valid, but looks nicer.
            CopyToBackup()

            'Quicksave checksum
            buffer = BitConverter.GetBytes(Checksums.Calculate32BitChecksum(Bits, Offsets.QuicksaveChecksumStart, Offsets.QuicksaveChecksumEnd))
            For x As Byte = 0 To 3
                Bits.Int(x + Offsets.QuicksaveStart, 0, 8) = buffer(x)
            Next
        End Sub

        ''' <summary>
        ''' Copies the primary save to the backup save.
        ''' </summary>
        Private Sub CopyToBackup()
            Dim e As Integer = Offsets.BackupSaveStart
            For i As Integer = 4 To e - 1
                Bits.Int(i + e, 0, 8) = Bits.Int(i, 0, 8)
            Next
        End Sub

        ''' <summary>
        ''' Determines whether or not the given file is a SkySave.
        ''' </summary>
        ''' <param name="File">File to determine the type of.</param>
        ''' <returns></returns>
        Public Function IsFileOfType(File As GenericFile) As Task(Of Boolean) Implements IDetectableFileType.IsOfType
            If File.Length > Offsets.ChecksumEnd Then
                Dim buffer = BitConverter.GetBytes(Checksums.Calculate32BitChecksum(File, 4, Offsets.ChecksumEnd))
                Return Task.FromResult(File.RawData(0) = buffer(0) AndAlso File.RawData(1) = buffer(1) AndAlso File.RawData(2) = buffer(2) AndAlso File.RawData(3) = buffer(3))
            Else
                Return Task.FromResult(False)
            End If
        End Function
#End Region



    End Class

End Namespace