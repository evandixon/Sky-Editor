﻿Imports SkyEditorBase
Imports SkyEditor

Namespace Saves
    Public Class GatesGameData
        Inherits GenericSave
        Protected Property Bits As Binary
        Dim originalChecksum As Byte
        Public Sub New(Filename As String)
            MyBase.New(Filename)
            Bits = New Binary()
            For count As Integer = 0 To Length - 1
                Bits.AppendByte(RawData(count))
            Next
            originalChecksum = CalculateChecksum()
        End Sub

        Public Class Offsets
            Public Const HeldItemsOffset As Integer = &H20A * 8 + 2
        End Class

        Private Function FindChecksumBitOffset() As Integer
            Dim offset As Integer = -1
            For count = Bits.Count - 120 - 1 To Bits.Count - 100 - 1
                If Bits(count) Then
                    offset = count + 96
                    Exit For
                End If
            Next
            If offset > -1 Then
                Return offset
            Else
                Throw New IndexOutOfRangeException
            End If
        End Function
        Protected Overrides Sub PreSave()
            MyBase.PreSave()
            For count As Integer = 0 To Math.Ceiling(Bits.Count / 8) - 1
                RawData(count) = Bits.Int(count, 0, 8)
            Next
        End Sub
        Public Property StoredChecksum As Byte
            Get
                Return Bits.Int(0, FindChecksumBitOffset, 8)
            End Get
            Set(value As Byte)
                Bits.Int(0, FindChecksumBitOffset, 8) = value
            End Set
        End Property
        Public Function CalculateChecksum() As Byte
            Dim sum As Long = 0
            For count As Integer = 0 To FindChecksumBitOffset() - 1 'RawData.Length - 1 -  '&H417
                If Bits(count) Then
                    sum += 1
                End If
            Next
            Dim sum1 = sum And &HFF
            Dim sum2 = sum >> 8 And &HFF
            Select Case sum2
                Case &H9B
                    If sum1 + &H22 <= 255 Then
                        sum1 += &H22
                    End If
                Case &H99
                    If sum1 + 5 <= 255 Then
                        sum1 += 5
                    End If
                Case &H97
                    If sum1 > 32 Then
                        sum1 -= 32
                    End If
            End Select
            'If sum = 182 Then
            '    sum += 5
            'ElseIf sum = 93 Then
            '    sum += 125
            'ElseIf sum + 189 < 255 Then
            '    sum += 189
            'End If
            Return sum1
        End Function
        Public Overrides Sub FixChecksum()
            MyBase.FixChecksum()
            StoredChecksum = (StoredChecksum + (CalculateChecksum() - originalChecksum) And &HFF)
        End Sub
        Public Property HeldMoney As Integer
            Get
                Return Bits.Int(5, 0, 16)
            End Get
            Set(value As Integer)
                Bits.Int(5, 0, 16) = value
            End Set
        End Property
        Public Property CompanionHeldMoney As Integer
            Get
                Return Bits.Int(9, 0, 16)
            End Get
            Set(value As Integer)
                Bits.Int(9, 0, 16) = value
            End Set
        End Property
        Public Overrides Sub DebugInfo()
            MyBase.DebugInfo()
            PluginHelper.Writeline("Calculated Checksum: " & CalculateChecksum())
            PluginHelper.Writeline("    Stored Checksum: " & StoredChecksum)
        End Sub

        Public Overrides Function DefaultSaveID() As String
            Return GameStrings.MDGatesData
        End Function
    End Class

End Namespace