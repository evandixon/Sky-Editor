Imports SkyEditor.Core
Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.IO

Namespace Saves
    Public Class SkySave
        Inherits BinaryFile
        Implements iDetectableFileType

        Public Sub New()
            MyBase.New
        End Sub
        'Public Sub New(Filename As String)
        '    MyBase.New(Filename)
        'End Sub
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
            <Obsolete("Untested")> Public Const AdventureLogLength As Integer = 447

            Public Const CroagunkShopOffset As Integer = &HB475 * 8
            Public Const CroagunkShopLength As Integer = 11
            Public Const CroagunkShopNumber As Integer = 8

            Public Const QuicksavePokemonNumber As Integer = 20
            Public Const QuicksavePokemonLength As Integer = 429 * 8
            Public Const QuicksavePokemonOffset As Integer = &H19000 * 8 + (&H3170 * 8)
        End Class

#Region "Properties"

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
        Public Sub CopyToBackup()
            Dim e As Integer = Offsets.BackupSaveStart
            For i As Integer = 4 To e - 1
                Bits.Int(i + e, 0, 8) = Bits.Int(i, 0, 8)
            Next
        End Sub
        'Public Overrides Function DefaultSaveID() As String
        '    Return GameStrings.SkySave
        'End Function
        'Protected Overrides Sub PreSave()
        '    MyBase.PreSave()
        '    For count As Integer = 0 To Math.Ceiling(Bits.Count / 8) - 1
        '        RawData(count) = Bits.Int(count, 0, 8)
        '    Next
        'End Sub
#End Region

        Public Function IsFileOfType(File As GenericFile) As Boolean Implements iDetectableFileType.IsOfType
            If File.Length > Offsets.ChecksumEnd Then
                Dim buffer = BitConverter.GetBytes(Checksums.Calculate32BitChecksum(File, 4, Offsets.ChecksumEnd))
                Return (File.RawData(0) = buffer(0) AndAlso File.RawData(1) = buffer(1) AndAlso File.RawData(2) = buffer(2) AndAlso File.RawData(3) = buffer(3))
            Else
                Return False
            End If
        End Function

    End Class

End Namespace