Imports SkyEditor.Core
Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.IO
Imports SkyEditor.SaveEditor.Interfaces

Namespace Saves
    Public Class SkySave
        Inherits BinaryFile
        Implements IDetectableFileType
        Implements Interfaces.iParty

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


#Region "Properties"

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

        ''' <summary>
        ''' Gets or sets the original player Pokemon.
        ''' Used in-game for special episodes.
        ''' </summary>
        ''' <returns></returns>
        Public Property OriginalPlayerID As Integer
            Get
                Dim i = Bits.Int(&HBE, 0, 16)
                If i > 600 Then
                    i -= 600
                End If
                Return i
            End Get
            Set(value As Integer)
                Dim f = OriginalPlayerIsFemale
                Bits.Int(&HBE, 0, 16) = value
                OriginalPlayerIsFemale = f
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the original player gender.
        ''' Used in-game for special episodes.
        ''' </summary>
        ''' <returns></returns>
        Public Property OriginalPlayerIsFemale As Boolean
            Get
                Dim i = Bits.Int(&HBE, 0, 16)
                Return (i > 600)
            End Get
            Set(value As Boolean)
                Dim i = Bits.Int(&HBE, 0, 16)
                If i > 600 Then
                    If value Then
                        'do nothing
                    Else
                        Bits.Int(&HBE, 0, 16) -= 600
                    End If
                Else
                    If value Then
                        Bits.Int(&HBE, 0, 16) += 600
                    Else
                        'do nothing
                    End If
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the original partner Pokemon.
        ''' Used in-game for special episodes.
        ''' </summary>
        ''' <returns></returns>
        Public Property OriginalPartnerID As Integer
            Get
                Dim i = Bits.Int(&HC0, 0, 16)
                If i > 600 Then
                    i -= 600
                End If
                Return i
            End Get
            Set(value As Integer)
                Dim f = OriginalPlayerIsFemale
                Bits.Int(&HC0, 0, 16) = value
                OriginalPlayerIsFemale = f
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the original partner gender.
        ''' Used in-game for special episodes.
        ''' </summary>
        ''' <returns></returns>
        Public Property OriginalPartnerIsFemale As Boolean
            Get
                Dim i = Bits.Int(&HC0, 0, 16)
                Return (i > 600)
            End Get
            Set(value As Boolean)
                Dim i = Bits.Int(&HC0, 0, 16)
                If i > 600 Then
                    If value Then
                        'do nothing
                    Else
                        Bits.Int(&HC0, 0, 16) -= 600
                    End If
                Else
                    If value Then
                        Bits.Int(&HC0, 0, 16) += 600
                    Else
                        'do nothing
                    End If
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the original player name.
        ''' Used in-game for special episodes.
        ''' </summary>
        ''' <returns></returns>
        Public Property OriginalPlayerName As String
            Get
                Return Bits.StringPMD(&H13F, 0, 10)
            End Get
            Set(value As String)
                Bits.StringPMD(&H13F, 0, 10) = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the original partner name.
        ''' Used in-game for special episodes.
        ''' </summary>
        ''' <returns></returns>
        Public Property OriginalPartnerName As String
            Get
                Return Bits.StringPMD(&H149, 0, 10)
            End Get
            Set(value As String)
                Bits.StringPMD(&H149, 0, 10) = value
            End Set
        End Property
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

#Region "Money"
        Public Property HeldMoney As Integer
            Get
                Return Bits.Int(0, Offsets.HeldMoney, 24)
            End Get
            Set(value As Integer)
                Bits.Int(0, Offsets.HeldMoney, 32) = value
            End Set
        End Property
        Public Property SpEpisode_HeldMoney As Integer
            Get
                Return Bits.Int(0, Offsets.SPHeldMoney, 24)
            End Get
            Set(value As Integer)
                Bits.Int(0, Offsets.SPHeldMoney, 32) = value
            End Set
        End Property
        Public Property StoredMoney As Integer
            Get
                Return Bits.Int(0, Offsets.StoredMoney, 24)
            End Get
            Set(value As Integer)
                Bits.Int(0, Offsets.StoredMoney, 24) = value
            End Set
        End Property
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