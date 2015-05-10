Imports SkyEditor.skyjed.buffer
Imports SkyEditor.skyjed.util
Imports SkyEditorBase
Public Class MDSaveBase
    Inherits GenericSave
    Sub New(save As Byte())
        MyBase.New(save)
        'Dim offsets = OffsetManager.GetOffsets(RawData)

    End Sub

    Public Overrides Sub FixChecksum()
        'First checksum
        Dim words As New List(Of UInt32)
        For count As Integer = 4 To CurrentOffsets.ChecksumEnd Step 4
            words.Add(BitConverter.ToUInt32(RawData, count))
        Next
        Dim sum As UInt64 = 0
        For Each item In words
            sum += item
        Next
        If TypeOf Me Is RBSaveEU Then
            sum -= 1
        End If
        Dim buffer() As Byte = BitConverter.GetBytes(sum)
        For x As Byte = 0 To 3
            RawData(x) = buffer(x)
            RawData(x + CurrentOffsets.BackupSaveStart) = buffer(x)
        Next
        'Ensure backup save matches
        CopyToBackup()
        'Quicksave checksum
        If CurrentOffsets.QuicksaveStart.HasValue Then
            words = New List(Of UInt32)
            For count As Integer = CurrentOffsets.QuicksaveChecksumStart To CurrentOffsets.QuicksaveChecksumEnd Step 4
                words.Add(BitConverter.ToUInt32(RawData, count))
            Next
            sum = 0
            For Each item In words
                sum += item
            Next
            If TypeOf Me Is RBSaveEU Then
                sum -= 1
            End If
            buffer = BitConverter.GetBytes(sum)
            For x As Byte = 0 To 3
                RawData(x + CurrentOffsets.QuicksaveStart) = buffer(x)
            Next
        End If
    End Sub
    Public Sub CopyToBackup()
        Dim e As Integer = CurrentOffsets.BackupSaveStart
        For i As Integer = 4 To e - 1
            RawData(i + e) = RawData(i)
        Next
    End Sub

    Friend Function CurrentOffsets() As Offsets
        Return OffsetManager.GetOffsets(Me)
    End Function
    Public Property RawDataBits As BooleanBufferArray
        Get
            Dim split_data = BitConverterLE.splitBits(RawData)
            Return New BooleanBufferArray(split_data)
        End Get
        Set(value As BooleanBufferArray)
            RawData = BitConverterLE.packBits(value.GetSplitBytes)
        End Set
    End Property

    Public Overrides Function DefaultSaveID() As String
        Return "Generic Mystery Dungeon Save"
    End Function

    Public Overrides Sub DebugInfo()
        MyBase.DebugInfo()
        Dim words As New List(Of UInt32)
        For count As Integer = 4 To CurrentOffsets.ChecksumEnd Step 4
            words.Add(BitConverter.ToUInt32(RawData, count))
        Next
        Dim sum As UInt64 = 0
        For Each item In words
            sum += item
        Next
        Dim buffer() As Byte = BitConverter.GetBytes(sum)
        Dim checksum As UInt32 = BitConverter.ToUInt32(buffer, 0)
        Dim checksumActual As UInt32 = BitConverter.ToUInt32(RawData, 0)
        If checksum = checksumActual Then
            PluginHelper.Writeline("Checksum valid!")
        Else
            PluginHelper.Writeline("Invalid checksum.")
        End If
    End Sub
End Class