Partial Class SkySave
    Public ReadOnly Property StoredItemIDs As UInt16()
        Get
            Return EncodedListOf11BitIntegers(&H8E0C, True)
        End Get
    End Property
    Public ReadOnly Property StoredItemParamaters As UInt16()
        Get
            Return EncodedListOf11BitIntegers(&H936B, False)
        End Get
    End Property
    Private ReadOnly Property EncodedListOf11BitIntegers(Offset As Integer, TerminateOnZero As Boolean) As UInt16()
        Get
            Dim out As New List(Of UInt16)
            For x As Integer = 0 To 124
                Dim itemIDRaw1 As Byte() = {Bit8.FromBits(RawData(Offset + 11 * x).Bit7,
                                                         RawData(Offset + 11 * x).Bit8,
                                                         RawData(Offset + 11 * x + 1).Bit1,
                                                         RawData(Offset + 11 * x + 1).Bit2,
                                                         RawData(Offset + 11 * x + 1).Bit3,
                                                         RawData(Offset + 11 * x + 1).Bit4,
                                                         RawData(Offset + 11 * x + 1).Bit5,
                                                         RawData(Offset + 11 * x + 1).Bit6),
                                           Bit8.FromBits(RawData(Offset + 11 * x + 1).Bit7,
                                                         RawData(Offset + 11 * x + 1).Bit8,
                                                         RawData(Offset + 11 * x + 2).Bit1,
                                                         0, 0, 0, 0, 0)}
                Dim itemID1 As UInt16 = BitConverter.ToUInt16(itemIDRaw1, 0)
                If itemID1 > 0 OrElse Not TerminateOnZero Then
                    out.Add(itemID1)
                Else
                    Exit For
                End If
                Dim itemIDRaw2 As Byte() = {Bit8.FromBits(RawData(Offset + 11 * x + 2).Bit2,
                                                          RawData(Offset + 11 * x + 2).Bit3,
                                                          RawData(Offset + 11 * x + 2).Bit4,
                                                          RawData(Offset + 11 * x + 2).Bit5,
                                                          RawData(Offset + 11 * x + 2).Bit6,
                                                          RawData(Offset + 11 * x + 2).Bit7,
                                                          RawData(Offset + 11 * x + 2).Bit8,
                                                          RawData(Offset + 11 * x + 3).Bit1),
                                            Bit8.FromBits(RawData(Offset + 11 * x + 3).Bit2,
                                                          RawData(Offset + 11 * x + 3).Bit3,
                                                          RawData(Offset + 11 * x + 3).Bit4,
                                                          0, 0, 0, 0, 0)}
                Dim itemID2 As UInt16 = BitConverter.ToUInt16(itemIDRaw2, 0)
                If itemID2 > 0 OrElse Not TerminateOnZero Then
                    out.Add(itemID2)
                Else
                    Exit For
                End If
                Dim itemIDRaw3 As Byte() = {Bit8.FromBits(RawData(Offset + 11 * x + 3).Bit5,
                                                          RawData(Offset + 11 * x + 3).Bit6,
                                                          RawData(Offset + 11 * x + 3).Bit7,
                                                          RawData(Offset + 11 * x + 3).Bit8,
                                                          RawData(Offset + 11 * x + 4).Bit1,
                                                          RawData(Offset + 11 * x + 4).Bit2,
                                                          RawData(Offset + 11 * x + 4).Bit3,
                                                          RawData(Offset + 11 * x + 4).Bit4),
                                            Bit8.FromBits(RawData(Offset + 11 * x + 4).Bit5,
                                                          RawData(Offset + 11 * x + 4).Bit6,
                                                          RawData(Offset + 11 * x + 4).Bit7,
                                                          0, 0, 0, 0, 0)}
                Dim itemID3 As UInt16 = BitConverter.ToUInt16(itemIDRaw3, 0)
                If itemID3 > 0 OrElse Not TerminateOnZero Then
                    out.Add(itemID3)
                Else
                    Exit For
                End If
                Dim itemIDRaw4 As Byte() = {Bit8.FromBits(RawData(Offset + 11 * x + 4).Bit8,
                                                          RawData(Offset + 11 * x + 5).Bit1,
                                                          RawData(Offset + 11 * x + 5).Bit2,
                                                          RawData(Offset + 11 * x + 5).Bit3,
                                                          RawData(Offset + 11 * x + 5).Bit4,
                                                          RawData(Offset + 11 * x + 5).Bit5,
                                                          RawData(Offset + 11 * x + 5).Bit6,
                                                          RawData(Offset + 11 * x + 5).Bit7),
                                            Bit8.FromBits(RawData(Offset + 11 * x + 5).Bit8,
                                                          RawData(Offset + 11 * x + 6).Bit1,
                                                          RawData(Offset + 11 * x + 6).Bit2,
                                                          0, 0, 0, 0, 0)}
                Dim itemID4 As UInt16 = BitConverter.ToUInt16(itemIDRaw4, 0)
                If itemID4 > 0 OrElse Not TerminateOnZero Then
                    out.Add(itemID4)
                Else
                    Exit For
                End If
                Dim itemIDRaw5 As Byte() = {Bit8.FromBits(RawData(Offset + 11 * x + 6).Bit3,
                                                          RawData(Offset + 11 * x + 6).Bit4,
                                                          RawData(Offset + 11 * x + 6).Bit5,
                                                          RawData(Offset + 11 * x + 6).Bit6,
                                                          RawData(Offset + 11 * x + 6).Bit7,
                                                          RawData(Offset + 11 * x + 6).Bit8,
                                                          RawData(Offset + 11 * x + 7).Bit1,
                                                          RawData(Offset + 11 * x + 7).Bit2),
                                            Bit8.FromBits(RawData(Offset + 11 * x + 7).Bit3,
                                                          RawData(Offset + 11 * x + 7).Bit4,
                                                          RawData(Offset + 11 * x + 7).Bit5,
                                                          0, 0, 0, 0, 0)}
                Dim itemID5 As UInt16 = BitConverter.ToUInt16(itemIDRaw5, 0)
                If itemID5 > 0 OrElse Not TerminateOnZero Then
                    out.Add(itemID5)
                Else
                    Exit For
                End If
                Dim itemIDRaw6 As Byte() = {Bit8.FromBits(RawData(Offset + 11 * x + 7).Bit6,
                                                          RawData(Offset + 11 * x + 7).Bit7,
                                                          RawData(Offset + 11 * x + 7).Bit8,
                                                          RawData(Offset + 11 * x + 8).Bit1,
                                                          RawData(Offset + 11 * x + 8).Bit2,
                                                          RawData(Offset + 11 * x + 8).Bit3,
                                                          RawData(Offset + 11 * x + 8).Bit4,
                                                          RawData(Offset + 11 * x + 8).Bit5),
                                            Bit8.FromBits(RawData(Offset + 11 * x + 8).Bit6,
                                                          RawData(Offset + 11 * x + 8).Bit7,
                                                          RawData(Offset + 11 * x + 8).Bit8,
                                                          0, 0, 0, 0, 0)}
                Dim itemID6 As UInt16 = BitConverter.ToUInt16(itemIDRaw6, 0)
                If itemID6 > 0 OrElse Not TerminateOnZero Then
                    out.Add(itemID6)
                Else
                    Exit For
                End If
                Dim itemIDRaw7 As Byte() = {Bit8.FromBits(RawData(Offset + 11 * x + 9).Bit1,
                                                          RawData(Offset + 11 * x + 9).Bit2,
                                                          RawData(Offset + 11 * x + 9).Bit3,
                                                          RawData(Offset + 11 * x + 9).Bit4,
                                                          RawData(Offset + 11 * x + 9).Bit5,
                                                          RawData(Offset + 11 * x + 9).Bit6,
                                                          RawData(Offset + 11 * x + 9).Bit7,
                                                          RawData(Offset + 11 * x + 9).Bit8),
                                            Bit8.FromBits(RawData(Offset + 11 * x + 10).Bit1,
                                                          RawData(Offset + 11 * x + 10).Bit2,
                                                          RawData(Offset + 11 * x + 10).Bit3,
                                                          0, 0, 0, 0, 0)}
                Dim itemID7 As UInt16 = BitConverter.ToUInt16(itemIDRaw7, 0)
                If itemID7 > 0 OrElse Not TerminateOnZero Then
                    out.Add(itemID7)
                Else
                    Exit For
                End If
                Dim itemIDRaw8 As Byte() = {Bit8.FromBits(RawData(Offset + 11 * x + 10).Bit4,
                                                          RawData(Offset + 11 * x + 10).Bit5,
                                                          RawData(Offset + 11 * x + 10).Bit6,
                                                          RawData(Offset + 11 * x + 10).Bit7,
                                                          RawData(Offset + 11 * x + 10).Bit8,
                                                          RawData(Offset + 11 * x + 11).Bit1,
                                                          RawData(Offset + 11 * x + 11).Bit2,
                                                          RawData(Offset + 11 * x + 11).Bit3),
                                            Bit8.FromBits(RawData(Offset + 11 * x + 11).Bit4,
                                                          RawData(Offset + 11 * x + 11).Bit5,
                                                          RawData(Offset + 11 * x + 11).Bit6,
                                                          0, 0, 0, 0, 0)}
                Dim itemID8 As UInt16 = BitConverter.ToUInt16(itemIDRaw8, 0)
                If itemID8 > 0 OrElse Not TerminateOnZero Then
                    out.Add(itemID8)
                Else
                    Exit For
                End If
            Next
            Return out.ToArray
        End Get
    End Property
End Class
