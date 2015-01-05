Imports SkyEditorBase

''' <summary>
''' Everything in this file is subject to removal.  It's only still around for reference.
''' </summary>
''' <remarks></remarks>
Partial Class SkySave
    Public Property EncodedString(Offset As Integer, StartBit As Integer, StringLength As Integer) As String
        Get
            Dim out As String = ""
            Dim buffer As Byte() = BitOperations.ShiftRightPMD(GenericArrayOperations(Of Byte).CopyOfRange(RawData, Offset, Offset + StringLength), StartBit, 0, StringLength)
            For Each b In buffer 'SubByteArr(RawData, CurrentOffsets.TeamNameStart, CurrentOffsets.TeamNameStart + 7)
                If b > 0 Then
                    If Lists.StringEncoding.Keys.Contains(b) Then
                        out = out & Lists.StringEncoding(b)
                    Else
                        out = out & "[" & b.ToString & "]"
                    End If
                Else
                    Exit For
                End If
            Next
            Return out
        End Get
        Set(value As String)
            Dim buffer As Byte() = StringUtilities.StringToPMDEncoding(value)
            Array.Resize(buffer, 10)
            Array.Reverse(buffer)
            Array.Resize(buffer, 11)
            Array.Reverse(buffer)
            buffer = BitOperations.EncodeBytes(buffer, StartBit, StringLength * 8)
            'buffer = BitOperations.EncodeBytes(buffer, StartBit, StringLength * 8)
            For x As Integer = 0 To 10
                If buffer.Length > x Then
                    RawData(Offset + x) = buffer(x)
                Else
                    RawData(Offset + x) = 0
                End If
            Next
        End Set
    End Property
End Class
