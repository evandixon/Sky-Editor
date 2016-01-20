Imports SkyEditorBase

Public Class Checksums
    Public Shared Function Calculate32BitChecksum(Bits As Binary, StartIndex As Integer, EndIndex As Integer) As UInt32
        Dim rawData As Byte() = Bits.ToByteArray
        Dim words As New List(Of UInt32)
        For count As Integer = StartIndex To EndIndex Step 4
            words.Add(BitConverter.ToUInt32(rawData, count))
        Next
        Dim sum As UInt64 = 0
        For Each item In words
            sum += item
        Next
        Return sum And UInteger.MaxValue '&HFFFFFFFF 'Wait, why is this interpreted as signed?
    End Function
    Public Shared Function Calculate32BitChecksum(File As GenericFile, StartIndex As Integer, EndIndex As Integer) As UInt32
        Dim words As New List(Of UInt32)
        For count As Integer = StartIndex To EndIndex Step 4
            words.Add(BitConverter.ToUInt32(File.RawData(count, 4), 0))
        Next
        Dim sum As UInt64 = 0
        For Each item In words
            sum += item
        Next
        Return sum And UInteger.MaxValue '&HFFFFFFFF 'Wait, why is this interpreted as signed?
    End Function
End Class