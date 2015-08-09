Imports SkyEditorBase

Namespace Saves
    Public Class TDSave
        Inherits GenericSave
        Protected Property Bits As Binary
        Public Sub New(Filename As String)
            MyBase.New(Filename)
            Bits = New Binary()
            For count As Integer = 0 To Length - 1
                Bits.AppendByte(RawData(count))
            Next
        End Sub
        Protected Class Offsets
            Public Const ChecksumEnd As Integer = &HDC7B
            Public Const BackupSaveStart As Integer = &H10000
            Public Const QuicksaveStart As Integer = &H2E000
            Public Const QuicksaveChecksumStart As Integer = &H2E004
            Public Const QuicksaveChecksumEnd As Integer = &H2E0FF

            Public Const TeamNameStart As Integer = &H96F7 * 8
            Public Const TeamNameLength As Integer = 10

            Public Const HeldItemOffset As Integer = &H8B71 * 8
            Public Const HeldItemNumber As Integer = 48
            Public Const HeldItemLength As Integer = 31

            Public Const StoredPokemonOffset As Integer = &H460 * 8 + 3
            Public Const StoredPokemonLength As Integer = 388
            Public Const StoredPokemonNumber As Integer = 550

            Public Const ActivePokemonOffset As Integer = &H83CB * 8
            Public Const ActivePokemonLength As Integer = 544
            Public Const ActivePokemonNumber As Integer = 4
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
#End Region
#End Region
#Region "Technical Stuff"
        Public Overrides Sub FixChecksum()
            'Fix the first checksum
            Dim buffer = BitConverter.GetBytes(Checksums.Calculate32BitChecksum(Bits, 0, Offsets.ChecksumEnd))
            For count = 0 To 3
                RawData(count) = buffer(count)
            Next

            'Ensure backup save matches.
            'Not strictly required, as the first one will be loaded if it's valid, but looks nicer.
            CopyToBackup()

            'Quicksave checksum
            buffer = BitConverter.GetBytes(Checksums.Calculate32BitChecksum(Bits, Offsets.QuicksaveChecksumStart, Offsets.QuicksaveChecksumEnd))
            For x As Byte = 0 To 3
                RawData(x + Offsets.QuicksaveStart) = buffer(x)
            Next
        End Sub
        Public Sub CopyToBackup()
            Dim e As Integer = Offsets.BackupSaveStart
            For i As Integer = 4 To e - 1
                RawData(i + e) = RawData(i)
            Next
        End Sub

        Public Overrides Function DefaultSaveID() As String
            Return GameStrings.TDSave
        End Function

        Public Overrides Sub PreSave()
            MyBase.PreSave()
            For count As Integer = 0 To Math.Ceiling(Bits.Count / 8) - 1
                RawData(count) = Bits.Int(count, 0, 8)
            Next
        End Sub
#End Region
    End Class

End Namespace