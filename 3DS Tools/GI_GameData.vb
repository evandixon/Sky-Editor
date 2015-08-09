Imports SkyEditorBase

Public Class GI_GameData
    Inherits GenericSave
    Public Property Checksum As UInt16
        Get
            Return (BitConverter.ToUInt16(RawData(0, 2), Length - 2) >> 2) And &HFF
        End Get
        Set(value As UInt16)
            Dim data As UInt16 = value << 2
            Dim dataBytes = BitConverter.GetBytes(data)
            Me.RawData(Me.Length - 2) = dataBytes(0)
            Me.RawData(Me.Length - 1) = dataBytes(1)
        End Set
    End Property
    Dim _guessedChecksumEnd As Integer?
    Private ReadOnly Property GuessedChecksumEnd As Integer?
        Get
            If Not _guessedChecksumEnd.HasValue Then
                _guessedChecksumEnd = GuessChecksumEnd()
            End If
            Return _guessedChecksumEnd
        End Get
    End Property
    Public Function GuessChecksumEnd() As Integer?
        Dim actual = Checksum
        Dim output As Integer? = Nothing
        For count As Integer = Length - 1 To 0 Step -1
            If CalculateChecksum(count) = actual Then
                output = count
                Exit For
            End If
        Next
        Return output
    End Function
    Public Function CalculateChecksum(EndIndex As Integer) As UInt16
        Dim sum As Long = 0
        For count As Integer = 0 To EndIndex 'RawData.Length - 1 -  '&H417
            Dim b8 As New SkyEditorBase.Utilities.Bits8(RawData(count))
            If b8.Bit1 Then
                sum += 1
            End If
            If b8.Bit2 Then
                sum += 1
            End If
            If b8.Bit3 Then
                sum += 1
            End If
            If b8.Bit4 Then
                sum += 1
            End If
            If b8.Bit5 Then
                sum += 1
            End If
            If b8.Bit6 Then
                sum += 1
            End If
            If b8.Bit7 Then
                sum += 1
            End If
            If b8.Bit8 Then
                sum += 1
            End If
        Next
        Return sum 'And &HFF
    End Function
    Public Function ByteSum(EndIndex As Integer) As Long
        Dim sum As Long = 0
        For count As Integer = 0 To EndIndex 'RawData.Length - 1 -  '&H417
            sum += RawData(count)
        Next
        Return sum 'And &HFF
    End Function
    Public Overrides Sub FixChecksum()
        If GuessedChecksumEnd.HasValue Then
            Checksum = CalculateChecksum(GuessedChecksumEnd)
        Else
            PluginHelper.Writeline("Unable to fix the checksum.", PluginHelper.LineType.Error)
        End If
    End Sub
    Public Overrides Function DefaultSaveID() As String
        Return GameConstants.MDGatesData
    End Function
    Public Overrides Sub DebugInfo()
        MyBase.DebugInfo()
        PluginHelper.Writeline("Bit Sum:  " & Conversion.Hex(CalculateChecksum(Length - 3)).ToUpper)
        PluginHelper.Writeline("Byte Sum: " & Conversion.Hex(ByteSum(Length - 3)).ToUpper)
        'Dim endIndex = GuessChecksumEnd()
        'If endIndex.HasValue Then
        '    PluginHelper.Writeline("Calculated checksum: " & Conversion.Hex(CalculateChecksum(GuessChecksumEnd)).ToUpper)
        '    PluginHelper.Writeline("    Actual checksum: " & Conversion.Hex(Checksum).ToUpper)
        '    PluginHelper.Writeline("       Checksum end: " & Conversion.Hex(endIndex).ToUpper)
        '    _guessedChecksumEnd = endIndex
        'Else
        '    PluginHelper.Writeline("Could not calculate checksum.", PluginHelper.LineType.Error)
        'End If
    End Sub
End Class