Imports SkyEditorBase

Namespace Saves
    Public Class RBSave
        Inherits BinaryFile

        Public Sub New()
            MyBase.New()
        End Sub
        Public Sub New(Filename As String)
            MyBase.New(Filename)
        End Sub

        Protected Class Offsets
            Public Const BackupSaveStart As Integer = &H6000
            Public Const ChecksumEnd As Integer = &H57D0

            Public Const BaseTypeOffset As Integer = &H67 * 8

            Public Const TeamNameStart As Integer = &H4EC8 * 8
            Public Const TeamNameLength As Integer = 10

            Public Const HeldMoneyOffset As Integer = &H4E6C * 8
            Public Const HeldMoneyLength As Integer = 24

            Public Const StoredMoneyOffset As Integer = &H4E6F * 8
            Public Const StoredMoneyLength As Integer = 24

            Public Const RescuePointsOffset As Integer = &H4ED3 * 8
            Public Const RescuePointsLength As Integer = 16

            Public Const HeldItemOffset As Integer = &H4CF0 * 8
            Public Const HeldItemLength As Integer = 23
            Public Const HeldItemNumber As Integer = 20

            Public Const StoredItemOffset As Integer = &H4D2B * 8 - 2

            Public Const StoredPokemonOffset As Integer = (&H5B3 * 8 + 3) - (323 * 3)
            Public Const StoredPokemonLength As Integer = 323
            Public Const StoredPokemonNumber As Integer = 407
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
        Public Property BaseType As Byte
            Get
                Return Bits.Int(0, Offsets.BaseTypeOffset, 8)
            End Get
            Set(value As Byte)
                Bits.Int(0, Offsets.BaseTypeOffset, 8) = value
            End Set
        End Property

        Public Property RescuePoints As Integer
            Get
                Return Bits.Int(0, Offsets.RescuePointsOffset, Offsets.RescuePointsLength)
            End Get
            Set(value As Integer)
                Bits.Int(0, Offsets.RescuePointsOffset, Offsets.RescuePointsLength) = value
            End Set
        End Property
#End Region

#Region "Money"
        Public Property HeldMoney As Integer
            Get
                Return Bits.Int(0, Offsets.HeldMoneyOffset, Offsets.HeldMoneyLength)
            End Get
            Set(value As Integer)
                Bits.Int(0, Offsets.HeldMoneyOffset, Offsets.HeldMoneyLength) = value
            End Set
        End Property
        Public Property StoredMoney As Integer
            Get
                Return Bits.Int(0, Offsets.HeldMoneyOffset, Offsets.HeldMoneyLength)
            End Get
            Set(value As Integer)
                Bits.Int(0, Offsets.HeldMoneyOffset, Offsets.HeldMoneyLength) = value
            End Set
        End Property

#End Region

        Public ReadOnly Property StoredItemCounts As Integer()
            Get
                Dim out(239) As Integer
                Dim block = Bits.Range(Offsets.StoredItemOffset, 2400)
                For count As Integer = 0 To 238
                    out(count) = block.NextInt(10)
                Next
                Return out
            End Get
        End Property

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

            ''Quicksave checksum
            'TODO: Research the quick save
            'buffer = BitConverter.GetBytes(Checksums.Calculate32BitChecksum(Me, Offsets.QuicksaveChecksumStart, Offsets.QuicksaveChecksumEnd))
            'For x As Byte = 0 To 3
            '    RawData(x + Offsets.QuicksaveStart) = buffer(x)
            'Next
        End Sub
        Public Sub CopyToBackup()
            Dim e As Integer = Offsets.BackupSaveStart
            For i As Integer = 4 To e - 1
                Bits.Int(i + e, 0, 8) = Bits.Int(i, 0, 8)
            Next
        End Sub
        'Public Overrides Function DefaultSaveID() As String
        '    Return GameStrings.RBSave
        'End Function
#End Region
        Public Shared Function IsFileOfType(File As GenericFile) As Boolean
            If File.Length > Offsets.ChecksumEnd Then
                Dim buffer = BitConverter.GetBytes(Checksums.Calculate32BitChecksum(File, 4, Offsets.ChecksumEnd))
                Return (File.RawData(0) = buffer(0) AndAlso File.RawData(1) = buffer(1) AndAlso File.RawData(2) = buffer(2) AndAlso File.RawData(3) = buffer(3))
            Else
                Return False
            End If
        End Function

    End Class

End Namespace