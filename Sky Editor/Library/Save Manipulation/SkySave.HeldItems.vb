Imports SkyEditorBase
Partial Public Class SkySave
    Public Property HeldItems As SkyItem()
        Get
            Dim out As New List(Of SkyItem)
            Dim Offset As Integer = &H8BA2
            For i As Byte = 0 To 6
                For j As Byte = 0 To 6
                    If Not (i = 6 AndAlso j > 1) Then
                        Dim itemRaw As Byte()
                        itemRaw = {RawData(Offset + 0 + i * 33 + j * 4), RawData(Offset + 1 + i * 33 + j * 4), RawData(Offset + 2 + i * 33 + j * 4), RawData(Offset + 3 + i * 33 + j * 4), RawData(Offset + 4 + i * 33 + j * 4), 0}
                        itemRaw = BitOperations.ShiftRightPMD(itemRaw, j + 1, 0, 4)
                        Array.Reverse(itemRaw)
                        Dim item1 As New SkyItem(itemRaw)
                        If Not item1.IsTerminator Then
                            out.Add(item1)
                        Else
                            GoTo ReturnSpEp
                        End If
                    End If
                Next
                If i < 6 Then
                    Dim itemRaw8 As Byte() = {Bit8.FromBinary(RawData, Offset + i * 33 + 28, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 28, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 1 + 28, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 2 + 28, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 3 + 28, 0)}
                    Array.Reverse(itemRaw8)
                    Dim item8 As New SkyItem(itemRaw8)
                    If Not item8.IsTerminator Then
                        out.Add(item8)
                    Else
                        Exit For
                    End If
                End If
            Next
ReturnSpEp: Return out.ToArray
        End Get
        Set(value As SkyItem())
            'I hope to update this later.  I updated the Get to make it more efficient, but not this
            Const Offset As Integer = &H8BA2
            If value.Length > 0 Then
                Dim terminated As Boolean = False
                For i As Byte = 0 To 6
                    If (Not terminated) AndAlso value.Length > i * 8 + 0 AndAlso Not value(i * 8 + 0).IsTerminator Then

                        Dim itemRaw1 As Byte() = {Bit8.FromBinary(RawData, Offset + i * 33, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 1, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 2, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 3, 0)}
                        Dim itemData1 As Byte() = value(i * 8 + 0).ItemData
                        Dim i1b0 As Bits8 = itemRaw1(0)
                        Dim i1b1 As Bits8 = itemRaw1(1)
                        Dim i1b2 As Bits8 = itemRaw1(2)
                        Dim i1b3 As Bits8 = itemRaw1(3)
                        Dim i1b4 As Bits8 = itemRaw1(4)
                        i1b4.Bit1 = itemData1(0).Bit8
                        i1b3.Bit8 = itemData1(0).Bit7
                        i1b3.Bit7 = itemData1(0).Bit6
                        i1b3.Bit6 = itemData1(0).Bit5
                        i1b3.Bit5 = itemData1(0).Bit4
                        i1b3.Bit4 = itemData1(0).Bit3
                        i1b3.Bit3 = itemData1(0).Bit2
                        i1b3.Bit2 = itemData1(0).Bit1
                        i1b3.Bit1 = itemData1(1).Bit8
                        i1b2.Bit8 = itemData1(1).Bit7
                        i1b2.Bit7 = itemData1(1).Bit6
                        i1b2.Bit6 = itemData1(1).Bit5
                        i1b2.Bit5 = itemData1(1).Bit4
                        i1b2.Bit4 = itemData1(1).Bit3
                        i1b2.Bit3 = itemData1(1).Bit2
                        i1b2.Bit2 = itemData1(1).Bit1
                        i1b2.Bit1 = itemData1(2).Bit8
                        i1b1.Bit8 = itemData1(2).Bit7
                        i1b1.Bit7 = itemData1(2).Bit6
                        i1b1.Bit6 = itemData1(2).Bit5
                        i1b1.Bit5 = itemData1(2).Bit4
                        i1b1.Bit4 = itemData1(2).Bit3
                        i1b1.Bit3 = itemData1(2).Bit2
                        i1b1.Bit2 = itemData1(2).Bit1
                        i1b1.Bit1 = itemData1(3).Bit8
                        i1b0.Bit8 = itemData1(3).Bit7
                        i1b0.Bit7 = itemData1(3).Bit6
                        i1b0.Bit6 = itemData1(3).Bit5
                        i1b0.Bit5 = itemData1(3).Bit4
                        i1b0.Bit4 = itemData1(3).Bit3
                        i1b0.Bit3 = itemData1(3).Bit2
                        i1b0.Bit2 = itemData1(3).Bit1
                        i1b0.Bit1 = True 'Not Terminator
                        RawData(Offset + 0 + (i * 33) + 0) = i1b0
                        RawData(Offset + 1 + (i * 33) + 0) = i1b1
                        RawData(Offset + 2 + (i * 33) + 0) = i1b2
                        RawData(Offset + 3 + (i * 33) + 0) = i1b3
                        RawData(Offset + 4 + (i * 33) + 0) = i1b4
                    Else
                        RawData(Offset + 0 + (i * 33) + 0) = 0
                        RawData(Offset + 1 + (i * 33) + 0) = 0
                        RawData(Offset + 2 + (i * 33) + 0) = 0
                        RawData(Offset + 3 + (i * 33) + 0) = 0
                        RawData(Offset + 4 + (i * 33) + 0) = 0
                        terminated = True
                    End If
                    If (Not terminated) AndAlso value.Length > i * 8 + 1 AndAlso Not value(i * 8 + 1).IsTerminator Then
                        Dim itemRaw1 As Byte() = {Bit8.FromBinary(RawData, Offset + i * 33 + 4, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 4, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 1 + 4, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 2 + 4, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 3 + 4, 0)}
                        Dim itemData1 As Byte() = value(i * 8 + 1).ItemData
                        Dim i1b0 As Bits8 = itemRaw1(0)
                        Dim i1b1 As Bits8 = itemRaw1(1)
                        Dim i1b2 As Bits8 = itemRaw1(2)
                        Dim i1b3 As Bits8 = itemRaw1(3)
                        Dim i1b4 As Bits8 = itemRaw1(4)
                        i1b4.Bit2 = itemData1(0).Bit8
                        i1b4.Bit1 = itemData1(0).Bit7
                        i1b3.Bit8 = itemData1(0).Bit6
                        i1b3.Bit7 = itemData1(0).Bit5
                        i1b3.Bit6 = itemData1(0).Bit4
                        i1b3.Bit5 = itemData1(0).Bit3
                        i1b3.Bit4 = itemData1(0).Bit2
                        i1b3.Bit3 = itemData1(0).Bit1
                        i1b3.Bit2 = itemData1(1).Bit8
                        i1b3.Bit1 = itemData1(1).Bit7
                        i1b2.Bit8 = itemData1(1).Bit6
                        i1b2.Bit7 = itemData1(1).Bit5
                        i1b2.Bit6 = itemData1(1).Bit4
                        i1b2.Bit5 = itemData1(1).Bit3
                        i1b2.Bit4 = itemData1(1).Bit2
                        i1b2.Bit3 = itemData1(1).Bit1
                        i1b2.Bit2 = itemData1(2).Bit8
                        i1b2.Bit1 = itemData1(2).Bit7
                        i1b1.Bit8 = itemData1(2).Bit6
                        i1b1.Bit7 = itemData1(2).Bit5
                        i1b1.Bit6 = itemData1(2).Bit4
                        i1b1.Bit5 = itemData1(2).Bit3
                        i1b1.Bit4 = itemData1(2).Bit2
                        i1b1.Bit3 = itemData1(2).Bit1
                        i1b1.Bit2 = itemData1(3).Bit8
                        i1b1.Bit1 = itemData1(3).Bit7
                        i1b0.Bit8 = itemData1(3).Bit6
                        i1b0.Bit7 = itemData1(3).Bit5
                        i1b0.Bit6 = itemData1(3).Bit4
                        i1b0.Bit5 = itemData1(3).Bit3
                        i1b0.Bit4 = itemData1(3).Bit2
                        i1b0.Bit3 = itemData1(3).Bit1
                        i1b0.Bit2 = True 'Not Terminator
                        RawData(Offset + 0 + (i * 33) + 4) = i1b0
                        RawData(Offset + 1 + (i * 33) + 4) = i1b1
                        RawData(Offset + 2 + (i * 33) + 4) = i1b2
                        RawData(Offset + 3 + (i * 33) + 4) = i1b3
                        RawData(Offset + 4 + (i * 33) + 4) = i1b4
                    Else
                        RawData(Offset + 0 + (i * 33) + 4) = 0
                        RawData(Offset + 1 + (i * 33) + 4) = 0
                        RawData(Offset + 2 + (i * 33) + 4) = 0
                        RawData(Offset + 3 + (i * 33) + 4) = 0
                        RawData(Offset + 4 + (i * 33) + 4) = 0
                        terminated = True
                    End If
                    If i < 6 Then
                        If (Not terminated) AndAlso value.Length > i * 8 + 2 AndAlso Not value(i * 8 + 2).IsTerminator Then
                            Dim itemRaw1 As Byte() = {Bit8.FromBinary(RawData, Offset + i * 33 + 8, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 8, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 1 + 8, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 2 + 8, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 3 + 8, 0)}
                            Dim itemData1 As Byte() = value(i * 8 + 2).ItemData
                            Dim i1b0 As Bits8 = itemRaw1(0)
                            Dim i1b1 As Bits8 = itemRaw1(1)
                            Dim i1b2 As Bits8 = itemRaw1(2)
                            Dim i1b3 As Bits8 = itemRaw1(3)
                            Dim i1b4 As Bits8 = itemRaw1(4)
                            i1b4.Bit3 = itemData1(0).Bit8
                            i1b4.Bit2 = itemData1(0).Bit7
                            i1b4.Bit1 = itemData1(0).Bit6
                            i1b3.Bit8 = itemData1(0).Bit5
                            i1b3.Bit7 = itemData1(0).Bit4
                            i1b3.Bit6 = itemData1(0).Bit3
                            i1b3.Bit5 = itemData1(0).Bit2
                            i1b3.Bit4 = itemData1(0).Bit1
                            i1b3.Bit3 = itemData1(1).Bit8
                            i1b3.Bit2 = itemData1(1).Bit7
                            i1b3.Bit1 = itemData1(1).Bit6
                            i1b2.Bit8 = itemData1(1).Bit5
                            i1b2.Bit7 = itemData1(1).Bit4
                            i1b2.Bit6 = itemData1(1).Bit3
                            i1b2.Bit5 = itemData1(1).Bit2
                            i1b2.Bit4 = itemData1(1).Bit1
                            i1b2.Bit3 = itemData1(2).Bit8
                            i1b2.Bit2 = itemData1(2).Bit7
                            i1b2.Bit1 = itemData1(2).Bit6
                            i1b1.Bit8 = itemData1(2).Bit5
                            i1b1.Bit7 = itemData1(2).Bit4
                            i1b1.Bit6 = itemData1(2).Bit3
                            i1b1.Bit5 = itemData1(2).Bit2
                            i1b1.Bit4 = itemData1(2).Bit1
                            i1b1.Bit3 = itemData1(3).Bit8
                            i1b1.Bit2 = itemData1(3).Bit7
                            i1b1.Bit1 = itemData1(3).Bit6
                            i1b0.Bit8 = itemData1(3).Bit5
                            i1b0.Bit7 = itemData1(3).Bit4
                            i1b0.Bit6 = itemData1(3).Bit3
                            i1b0.Bit5 = itemData1(3).Bit2
                            i1b0.Bit4 = itemData1(3).Bit1
                            i1b0.Bit3 = True 'Not Terminator
                            RawData(Offset + 0 + (i * 33) + 8) = i1b0
                            RawData(Offset + 1 + (i * 33) + 8) = i1b1
                            RawData(Offset + 2 + (i * 33) + 8) = i1b2
                            RawData(Offset + 3 + (i * 33) + 8) = i1b3
                            RawData(Offset + 4 + (i * 33) + 8) = i1b4
                        Else
                            RawData(Offset + 0 + (i * 33) + 8) = 0
                            RawData(Offset + 1 + (i * 33) + 8) = 0
                            RawData(Offset + 2 + (i * 33) + 8) = 0
                            RawData(Offset + 3 + (i * 33) + 8) = 0
                            RawData(Offset + 4 + (i * 33) + 8) = 0
                            terminated = True
                        End If
                        If (Not terminated) AndAlso value.Length > i * 8 + 3 AndAlso Not value(i * 8 + 3).IsTerminator Then
                            Dim itemRaw1 As Byte() = {Bit8.FromBinary(RawData, Offset + i * 33 + 12, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 12, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 1 + 12, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 2 + 12, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 3 + 12, 0)}
                            Dim itemData1 As Byte() = value(i * 8 + 3).ItemData
                            Dim i1b0 As Bits8 = itemRaw1(0)
                            Dim i1b1 As Bits8 = itemRaw1(1)
                            Dim i1b2 As Bits8 = itemRaw1(2)
                            Dim i1b3 As Bits8 = itemRaw1(3)
                            Dim i1b4 As Bits8 = itemRaw1(4)
                            i1b4.Bit4 = itemData1(0).Bit8
                            i1b4.Bit3 = itemData1(0).Bit7
                            i1b4.Bit2 = itemData1(0).Bit6
                            i1b4.Bit1 = itemData1(0).Bit5
                            i1b3.Bit8 = itemData1(0).Bit4
                            i1b3.Bit7 = itemData1(0).Bit3
                            i1b3.Bit6 = itemData1(0).Bit2
                            i1b3.Bit5 = itemData1(0).Bit1
                            i1b3.Bit4 = itemData1(1).Bit8
                            i1b3.Bit3 = itemData1(1).Bit7
                            i1b3.Bit2 = itemData1(1).Bit6
                            i1b3.Bit1 = itemData1(1).Bit5
                            i1b2.Bit8 = itemData1(1).Bit4
                            i1b2.Bit7 = itemData1(1).Bit3
                            i1b2.Bit6 = itemData1(1).Bit2
                            i1b2.Bit5 = itemData1(1).Bit1
                            i1b2.Bit4 = itemData1(2).Bit8
                            i1b2.Bit3 = itemData1(2).Bit7
                            i1b2.Bit2 = itemData1(2).Bit6
                            i1b2.Bit1 = itemData1(2).Bit5
                            i1b1.Bit8 = itemData1(2).Bit4
                            i1b1.Bit7 = itemData1(2).Bit3
                            i1b1.Bit6 = itemData1(2).Bit2
                            i1b1.Bit5 = itemData1(2).Bit1
                            i1b1.Bit4 = itemData1(3).Bit8
                            i1b1.Bit3 = itemData1(3).Bit7
                            i1b1.Bit2 = itemData1(3).Bit6
                            i1b1.Bit1 = itemData1(3).Bit5
                            i1b0.Bit8 = itemData1(3).Bit4
                            i1b0.Bit7 = itemData1(3).Bit3
                            i1b0.Bit6 = itemData1(3).Bit2
                            i1b0.Bit5 = itemData1(3).Bit1
                            i1b0.Bit4 = True 'Not Terminator
                            RawData(Offset + 0 + (i * 33) + 12) = i1b0
                            RawData(Offset + 1 + (i * 33) + 12) = i1b1
                            RawData(Offset + 2 + (i * 33) + 12) = i1b2
                            RawData(Offset + 3 + (i * 33) + 12) = i1b3
                            RawData(Offset + 4 + (i * 33) + 12) = i1b4
                        Else
                            RawData(Offset + 0 + (i * 33) + 12) = 0
                            RawData(Offset + 1 + (i * 33) + 12) = 0
                            RawData(Offset + 2 + (i * 33) + 12) = 0
                            RawData(Offset + 3 + (i * 33) + 12) = 0
                            RawData(Offset + 4 + (i * 33) + 12) = 0
                            terminated = True
                        End If
                        If (Not terminated) AndAlso value.Length > i * 8 + 4 AndAlso Not value(i * 8 + 4).IsTerminator Then
                            Dim itemRaw1 As Byte() = {Bit8.FromBinary(RawData, Offset + i * 33 + 16, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 16, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 1 + 16, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 2 + 16, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 3 + 16, 0)}
                            Dim itemData1 As Byte() = value(i * 8 + 4).ItemData
                            Dim i1b0 As Bits8 = itemRaw1(0)
                            Dim i1b1 As Bits8 = itemRaw1(1)
                            Dim i1b2 As Bits8 = itemRaw1(2)
                            Dim i1b3 As Bits8 = itemRaw1(3)
                            Dim i1b4 As Bits8 = itemRaw1(4)
                            i1b4.Bit5 = itemData1(0).Bit8
                            i1b4.Bit4 = itemData1(0).Bit7
                            i1b4.Bit3 = itemData1(0).Bit6
                            i1b4.Bit2 = itemData1(0).Bit5
                            i1b4.Bit1 = itemData1(0).Bit4
                            i1b3.Bit8 = itemData1(0).Bit3
                            i1b3.Bit7 = itemData1(0).Bit2
                            i1b3.Bit6 = itemData1(0).Bit1
                            i1b3.Bit5 = itemData1(1).Bit8
                            i1b3.Bit4 = itemData1(1).Bit7
                            i1b3.Bit3 = itemData1(1).Bit6
                            i1b3.Bit2 = itemData1(1).Bit5
                            i1b3.Bit1 = itemData1(1).Bit4
                            i1b2.Bit8 = itemData1(1).Bit3
                            i1b2.Bit7 = itemData1(1).Bit2
                            i1b2.Bit6 = itemData1(1).Bit1
                            i1b2.Bit5 = itemData1(2).Bit8
                            i1b2.Bit4 = itemData1(2).Bit7
                            i1b2.Bit3 = itemData1(2).Bit6
                            i1b2.Bit2 = itemData1(2).Bit5
                            i1b2.Bit1 = itemData1(2).Bit4
                            i1b1.Bit8 = itemData1(2).Bit3
                            i1b1.Bit7 = itemData1(2).Bit2
                            i1b1.Bit6 = itemData1(2).Bit1
                            i1b1.Bit5 = itemData1(3).Bit8
                            i1b1.Bit4 = itemData1(3).Bit7
                            i1b1.Bit3 = itemData1(3).Bit6
                            i1b1.Bit2 = itemData1(3).Bit5
                            i1b1.Bit1 = itemData1(3).Bit4
                            i1b0.Bit8 = itemData1(3).Bit3
                            i1b0.Bit7 = itemData1(3).Bit2
                            i1b0.Bit6 = itemData1(3).Bit1
                            i1b0.Bit5 = True 'Not Terminator
                            RawData(Offset + 0 + (i * 33) + 16) = i1b0
                            RawData(Offset + 1 + (i * 33) + 16) = i1b1
                            RawData(Offset + 2 + (i * 33) + 16) = i1b2
                            RawData(Offset + 3 + (i * 33) + 16) = i1b3
                            RawData(Offset + 4 + (i * 33) + 16) = i1b4
                        Else
                            RawData(Offset + 0 + (i * 33) + 16) = 0
                            RawData(Offset + 1 + (i * 33) + 16) = 0
                            RawData(Offset + 2 + (i * 33) + 16) = 0
                            RawData(Offset + 3 + (i * 33) + 16) = 0
                            RawData(Offset + 4 + (i * 33) + 16) = 0
                            terminated = True
                        End If
                        If (Not terminated) AndAlso value.Length > i * 8 + 5 AndAlso Not value(i * 8 + 5).IsTerminator Then
                            Dim itemRaw1 As Byte() = {Bit8.FromBinary(RawData, Offset + i * 33 + 20, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 20, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 1 + 20, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 2 + 20, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 3 + 20, 0)}
                            Dim itemData1 As Byte() = value(i * 8 + 5).ItemData
                            Dim i1b0 As Bits8 = itemRaw1(0)
                            Dim i1b1 As Bits8 = itemRaw1(1)
                            Dim i1b2 As Bits8 = itemRaw1(2)
                            Dim i1b3 As Bits8 = itemRaw1(3)
                            Dim i1b4 As Bits8 = itemRaw1(4)
                            i1b4.Bit6 = itemData1(0).Bit8
                            i1b4.Bit5 = itemData1(0).Bit7
                            i1b4.Bit4 = itemData1(0).Bit6
                            i1b4.Bit3 = itemData1(0).Bit5
                            i1b4.Bit2 = itemData1(0).Bit4
                            i1b4.Bit1 = itemData1(0).Bit3
                            i1b3.Bit8 = itemData1(0).Bit2
                            i1b3.Bit7 = itemData1(0).Bit1
                            i1b3.Bit6 = itemData1(1).Bit8
                            i1b3.Bit5 = itemData1(1).Bit7
                            i1b3.Bit4 = itemData1(1).Bit6
                            i1b3.Bit3 = itemData1(1).Bit5
                            i1b3.Bit2 = itemData1(1).Bit4
                            i1b3.Bit1 = itemData1(1).Bit3
                            i1b2.Bit8 = itemData1(1).Bit2
                            i1b2.Bit7 = itemData1(1).Bit1
                            i1b2.Bit6 = itemData1(2).Bit8
                            i1b2.Bit5 = itemData1(2).Bit7
                            i1b2.Bit4 = itemData1(2).Bit6
                            i1b2.Bit3 = itemData1(2).Bit5
                            i1b2.Bit2 = itemData1(2).Bit4
                            i1b2.Bit1 = itemData1(2).Bit3
                            i1b1.Bit8 = itemData1(2).Bit2
                            i1b1.Bit7 = itemData1(2).Bit1
                            i1b1.Bit6 = itemData1(3).Bit8
                            i1b1.Bit5 = itemData1(3).Bit7
                            i1b1.Bit4 = itemData1(3).Bit6
                            i1b1.Bit3 = itemData1(3).Bit5
                            i1b1.Bit2 = itemData1(3).Bit4
                            i1b1.Bit1 = itemData1(3).Bit3
                            i1b0.Bit8 = itemData1(3).Bit2
                            i1b0.Bit7 = itemData1(3).Bit1
                            i1b0.Bit6 = True 'Not Terminator
                            RawData(Offset + 0 + (i * 33) + 20) = i1b0
                            RawData(Offset + 1 + (i * 33) + 20) = i1b1
                            RawData(Offset + 2 + (i * 33) + 20) = i1b2
                            RawData(Offset + 3 + (i * 33) + 20) = i1b3
                            RawData(Offset + 4 + (i * 33) + 20) = i1b4
                        Else
                            RawData(Offset + 0 + (i * 33) + 20) = 0
                            RawData(Offset + 1 + (i * 33) + 20) = 0
                            RawData(Offset + 2 + (i * 33) + 20) = 0
                            RawData(Offset + 3 + (i * 33) + 20) = 0
                            RawData(Offset + 4 + (i * 33) + 20) = 0
                            terminated = True
                        End If
                        If (Not terminated) AndAlso value.Length > i * 8 + 6 AndAlso Not value(i * 8 + 6).IsTerminator Then
                            Dim itemRaw1 As Byte() = {Bit8.FromBinary(RawData, Offset + i * 33 + 24, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 24, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 1 + 24, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 2 + 24, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 3 + 24, 0)}
                            Dim itemData1 As Byte() = value(i * 8 + 6).ItemData
                            Dim i1b0 As Bits8 = itemRaw1(0)
                            Dim i1b1 As Bits8 = itemRaw1(1)
                            Dim i1b2 As Bits8 = itemRaw1(2)
                            Dim i1b3 As Bits8 = itemRaw1(3)
                            Dim i1b4 As Bits8 = itemRaw1(4)
                            i1b4.Bit7 = itemData1(0).Bit8
                            i1b4.Bit6 = itemData1(0).Bit7
                            i1b4.Bit5 = itemData1(0).Bit6
                            i1b4.Bit4 = itemData1(0).Bit5
                            i1b4.Bit3 = itemData1(0).Bit4
                            i1b4.Bit2 = itemData1(0).Bit3
                            i1b4.Bit1 = itemData1(0).Bit2
                            i1b3.Bit8 = itemData1(0).Bit1
                            i1b3.Bit7 = itemData1(1).Bit8
                            i1b3.Bit6 = itemData1(1).Bit7
                            i1b3.Bit5 = itemData1(1).Bit6
                            i1b3.Bit4 = itemData1(1).Bit5
                            i1b3.Bit3 = itemData1(1).Bit4
                            i1b3.Bit2 = itemData1(1).Bit3
                            i1b3.Bit1 = itemData1(1).Bit2
                            i1b2.Bit8 = itemData1(1).Bit1
                            i1b2.Bit7 = itemData1(2).Bit8
                            i1b2.Bit6 = itemData1(2).Bit7
                            i1b2.Bit5 = itemData1(2).Bit6
                            i1b2.Bit4 = itemData1(2).Bit5
                            i1b2.Bit3 = itemData1(2).Bit4
                            i1b2.Bit2 = itemData1(2).Bit3
                            i1b2.Bit1 = itemData1(2).Bit2
                            i1b1.Bit8 = itemData1(2).Bit1
                            i1b1.Bit7 = itemData1(3).Bit8
                            i1b1.Bit6 = itemData1(3).Bit7
                            i1b1.Bit5 = itemData1(3).Bit6
                            i1b1.Bit4 = itemData1(3).Bit5
                            i1b1.Bit3 = itemData1(3).Bit4
                            i1b1.Bit2 = itemData1(3).Bit3
                            i1b0.Bit1 = itemData1(3).Bit2
                            i1b0.Bit8 = itemData1(3).Bit1
                            i1b0.Bit7 = True 'Not Terminator
                            RawData(Offset + 0 + (i * 33) + 24) = i1b0
                            RawData(Offset + 1 + (i * 33) + 24) = i1b1
                            RawData(Offset + 2 + (i * 33) + 24) = i1b2
                            RawData(Offset + 3 + (i * 33) + 24) = i1b3
                            RawData(Offset + 4 + (i * 33) + 24) = i1b4
                        Else
                            RawData(Offset + 0 + (i * 33) + 24) = 0
                            RawData(Offset + 1 + (i * 33) + 24) = 0
                            RawData(Offset + 2 + (i * 33) + 24) = 0
                            RawData(Offset + 3 + (i * 33) + 24) = 0
                            RawData(Offset + 4 + (i * 33) + 24) = 0
                            terminated = True
                        End If
                        If (Not terminated) AndAlso value.Length > i * 8 + 7 AndAlso Not value(i * 8 + 7).IsTerminator Then
                            Dim itemRaw1 As Byte() = {Bit8.FromBinary(RawData, Offset + i * 33 + 28, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 28, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 1 + 28, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 2 + 28, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 3 + 28, 0)}
                            Dim itemData1 As Byte() = value(i * 8 + 7).ItemData
                            Dim i1b0 As Bits8 = itemRaw1(0)
                            Dim i1b1 As Bits8 = itemRaw1(1)
                            Dim i1b2 As Bits8 = itemRaw1(2)
                            Dim i1b3 As Bits8 = itemRaw1(3)
                            Dim i1b4 As Bits8 = itemRaw1(4)
                            i1b4.Bit8 = itemData1(0).Bit8
                            i1b4.Bit7 = itemData1(0).Bit7
                            i1b4.Bit6 = itemData1(0).Bit6
                            i1b4.Bit5 = itemData1(0).Bit5
                            i1b4.Bit4 = itemData1(0).Bit4
                            i1b4.Bit3 = itemData1(0).Bit3
                            i1b4.Bit2 = itemData1(0).Bit2
                            i1b4.Bit1 = itemData1(0).Bit1
                            i1b3.Bit8 = itemData1(1).Bit8
                            i1b3.Bit7 = itemData1(1).Bit7
                            i1b3.Bit6 = itemData1(1).Bit6
                            i1b3.Bit5 = itemData1(1).Bit5
                            i1b3.Bit4 = itemData1(1).Bit4
                            i1b3.Bit3 = itemData1(1).Bit3
                            i1b3.Bit2 = itemData1(1).Bit2
                            i1b3.Bit1 = itemData1(1).Bit1
                            i1b2.Bit8 = itemData1(2).Bit8
                            i1b2.Bit7 = itemData1(2).Bit7
                            i1b2.Bit6 = itemData1(2).Bit6
                            i1b2.Bit5 = itemData1(2).Bit5
                            i1b2.Bit4 = itemData1(2).Bit4
                            i1b2.Bit3 = itemData1(2).Bit3
                            i1b2.Bit2 = itemData1(2).Bit2
                            i1b2.Bit1 = itemData1(2).Bit1
                            i1b1.Bit8 = itemData1(3).Bit8
                            i1b1.Bit7 = itemData1(3).Bit7
                            i1b1.Bit6 = itemData1(3).Bit6
                            i1b1.Bit5 = itemData1(3).Bit5
                            i1b1.Bit4 = itemData1(3).Bit4
                            i1b1.Bit3 = itemData1(3).Bit3
                            i1b1.Bit2 = itemData1(3).Bit2
                            i1b1.Bit1 = itemData1(3).Bit1
                            i1b0.Bit8 = True 'Not Terminator
                            RawData(Offset + 0 + (i * 33) + 28) = i1b0
                            RawData(Offset + 1 + (i * 33) + 28) = i1b1
                            RawData(Offset + 2 + (i * 33) + 28) = i1b2
                            RawData(Offset + 3 + (i * 33) + 28) = i1b3
                            RawData(Offset + 4 + (i * 33) + 28) = i1b4
                        Else
                            RawData(Offset + 0 + (i * 33) + 28) = 0
                            RawData(Offset + 1 + (i * 33) + 28) = 0
                            RawData(Offset + 2 + (i * 33) + 28) = 0
                            RawData(Offset + 3 + (i * 33) + 28) = 0
                            RawData(Offset + 4 + (i * 33) + 28) = 0
                            terminated = True
                        End If
                    End If
                Next
            End If
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the held items in the current Special Episode.  Set currently will not save first two items properly.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SpEpisode_HeldItems As SkyItem()
        Get
            Dim out As New List(Of SkyItem)
            Const Offset As Integer = &H8C68
            For i As Byte = 0 To 6
                For j As Byte = 0 To 6
                    If Not (i = 0 AndAlso j < 2) AndAlso Not (i = 6 AndAlso j > 3) Then
                        Dim itemRaw As Byte()
                        itemRaw = {RawData(Offset + 0 + i * 33 + j * 4), RawData(Offset + 1 + i * 33 + j * 4), RawData(Offset + 2 + i * 33 + j * 4), RawData(Offset + 3 + i * 33 + j * 4), RawData(Offset + 4 + i * 33 + j * 4), 0}
                        itemRaw = BitOperations.ShiftRightPMD(itemRaw, j + 1, 0, 5)
                        Array.Reverse(itemRaw)
                        Dim item1 As New SkyItem(itemRaw)
                        If Not item1.IsTerminator Then
                            out.Add(item1)
                        Else
                            GoTo ReturnSpEp
                        End If
                    End If
                Next
                If i < 6 Then
                    Dim itemRaw8 As Byte() = {Bit8.FromBinary(RawData, Offset + i * 33 + 28, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 28, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 1 + 28, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 2 + 28, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 3 + 28, 0)}
                    Array.Reverse(itemRaw8)
                    Dim item8 As New SkyItem(itemRaw8)
                    If Not item8.IsTerminator Then
                        out.Add(item8)
                    Else
                        Exit For
                    End If
                End If
            Next
ReturnSpEp: Return out.ToArray
        End Get
        Set(value As SkyItem())
            Const Offset As Integer = &H8C68
            If value.Length > 0 Then
                Dim terminated As Boolean = False
                Array.Reverse(value)
                Array.Resize(value, value.Length + 2)
                Array.Reverse(value)
                For i As Byte = 0 To 6
                    If i > 0 Then
                        If (Not terminated) AndAlso value.Length > i * 8 + 0 AndAlso Not value(i * 8 + 0).IsTerminator Then
                            Dim itemRaw1 As Byte() = {Bit8.FromBinary(RawData, Offset + i * 33, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 1, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 2, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 3, 0)}
                            Dim itemData1 As Byte() = value(i * 8 + 0).ItemData
                            Dim i1b0 As Bits8 = itemRaw1(0)
                            Dim i1b1 As Bits8 = itemRaw1(1)
                            Dim i1b2 As Bits8 = itemRaw1(2)
                            Dim i1b3 As Bits8 = itemRaw1(3)
                            Dim i1b4 As Bits8 = itemRaw1(4)
                            i1b4.Bit1 = itemData1(0).Bit8
                            i1b3.Bit8 = itemData1(0).Bit7
                            i1b3.Bit7 = itemData1(0).Bit6
                            i1b3.Bit6 = itemData1(0).Bit5
                            i1b3.Bit5 = itemData1(0).Bit4
                            i1b3.Bit4 = itemData1(0).Bit3
                            i1b3.Bit3 = itemData1(0).Bit2
                            i1b3.Bit2 = itemData1(0).Bit1
                            i1b3.Bit1 = itemData1(1).Bit8
                            i1b2.Bit8 = itemData1(1).Bit7
                            i1b2.Bit7 = itemData1(1).Bit6
                            i1b2.Bit6 = itemData1(1).Bit5
                            i1b2.Bit5 = itemData1(1).Bit4
                            i1b2.Bit4 = itemData1(1).Bit3
                            i1b2.Bit3 = itemData1(1).Bit2
                            i1b2.Bit2 = itemData1(1).Bit1
                            i1b2.Bit1 = itemData1(2).Bit8
                            i1b1.Bit8 = itemData1(2).Bit7
                            i1b1.Bit7 = itemData1(2).Bit6
                            i1b1.Bit6 = itemData1(2).Bit5
                            i1b1.Bit5 = itemData1(2).Bit4
                            i1b1.Bit4 = itemData1(2).Bit3
                            i1b1.Bit3 = itemData1(2).Bit2
                            i1b1.Bit2 = itemData1(2).Bit1
                            i1b1.Bit1 = itemData1(3).Bit8
                            i1b0.Bit8 = itemData1(3).Bit7
                            i1b0.Bit7 = itemData1(3).Bit6
                            i1b0.Bit6 = itemData1(3).Bit5
                            i1b0.Bit5 = itemData1(3).Bit4
                            i1b0.Bit4 = itemData1(3).Bit3
                            i1b0.Bit3 = itemData1(3).Bit2
                            i1b0.Bit2 = itemData1(3).Bit1
                            i1b0.Bit1 = True 'Not Terminator
                            RawData(Offset + 0 + (i * 33) + 0) = i1b0
                            RawData(Offset + 1 + (i * 33) + 0) = i1b1
                            RawData(Offset + 2 + (i * 33) + 0) = i1b2
                            RawData(Offset + 3 + (i * 33) + 0) = i1b3
                            RawData(Offset + 4 + (i * 33) + 0) = i1b4
                        Else
                            RawData(Offset + 0 + (i * 33) + 0) = 0
                            RawData(Offset + 1 + (i * 33) + 0) = 0
                            RawData(Offset + 2 + (i * 33) + 0) = 0
                            RawData(Offset + 3 + (i * 33) + 0) = 0
                            RawData(Offset + 4 + (i * 33) + 0) = 0
                            terminated = True
                        End If
                        If (Not terminated) AndAlso value.Length > i * 8 + 1 AndAlso Not value(i * 8 + 1).IsTerminator Then
                            Dim itemRaw1 As Byte() = {Bit8.FromBinary(RawData, Offset + i * 33 + 4, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 4, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 1 + 4, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 2 + 4, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 3 + 4, 0)}
                            Dim itemData1 As Byte() = value(i * 8 + 1).ItemData
                            Dim i1b0 As Bits8 = itemRaw1(0)
                            Dim i1b1 As Bits8 = itemRaw1(1)
                            Dim i1b2 As Bits8 = itemRaw1(2)
                            Dim i1b3 As Bits8 = itemRaw1(3)
                            Dim i1b4 As Bits8 = itemRaw1(4)
                            i1b4.Bit2 = itemData1(0).Bit8
                            i1b4.Bit1 = itemData1(0).Bit7
                            i1b3.Bit8 = itemData1(0).Bit6
                            i1b3.Bit7 = itemData1(0).Bit5
                            i1b3.Bit6 = itemData1(0).Bit4
                            i1b3.Bit5 = itemData1(0).Bit3
                            i1b3.Bit4 = itemData1(0).Bit2
                            i1b3.Bit3 = itemData1(0).Bit1
                            i1b3.Bit2 = itemData1(1).Bit8
                            i1b3.Bit1 = itemData1(1).Bit7
                            i1b2.Bit8 = itemData1(1).Bit6
                            i1b2.Bit7 = itemData1(1).Bit5
                            i1b2.Bit6 = itemData1(1).Bit4
                            i1b2.Bit5 = itemData1(1).Bit3
                            i1b2.Bit4 = itemData1(1).Bit2
                            i1b2.Bit3 = itemData1(1).Bit1
                            i1b2.Bit2 = itemData1(2).Bit8
                            i1b2.Bit1 = itemData1(2).Bit7
                            i1b1.Bit8 = itemData1(2).Bit6
                            i1b1.Bit7 = itemData1(2).Bit5
                            i1b1.Bit6 = itemData1(2).Bit4
                            i1b1.Bit5 = itemData1(2).Bit3
                            i1b1.Bit4 = itemData1(2).Bit2
                            i1b1.Bit3 = itemData1(2).Bit1
                            i1b1.Bit2 = itemData1(3).Bit8
                            i1b1.Bit1 = itemData1(3).Bit7
                            i1b0.Bit8 = itemData1(3).Bit6
                            i1b0.Bit7 = itemData1(3).Bit5
                            i1b0.Bit6 = itemData1(3).Bit4
                            i1b0.Bit5 = itemData1(3).Bit3
                            i1b0.Bit4 = itemData1(3).Bit2
                            i1b0.Bit3 = itemData1(3).Bit1
                            i1b0.Bit2 = True 'Not Terminator
                            RawData(Offset + 0 + (i * 33) + 4) = i1b0
                            RawData(Offset + 1 + (i * 33) + 4) = i1b1
                            RawData(Offset + 2 + (i * 33) + 4) = i1b2
                            RawData(Offset + 3 + (i * 33) + 4) = i1b3
                            RawData(Offset + 4 + (i * 33) + 4) = i1b4
                        Else
                            RawData(Offset + 0 + (i * 33) + 4) = 0
                            RawData(Offset + 1 + (i * 33) + 4) = 0
                            RawData(Offset + 2 + (i * 33) + 4) = 0
                            RawData(Offset + 3 + (i * 33) + 4) = 0
                            RawData(Offset + 4 + (i * 33) + 4) = 0
                            terminated = True
                        End If
                    End If

                    If (Not terminated) AndAlso value.Length > i * 8 + 2 AndAlso Not value(i * 8 + 2).IsTerminator Then
                        Dim itemRaw1 As Byte() = {Bit8.FromBinary(RawData, Offset + i * 33 + 8, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 8, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 1 + 8, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 2 + 8, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 3 + 8, 0)}
                        Dim itemData1 As Byte() = value(i * 8 + 2).ItemData
                        Dim i1b0 As Bits8 = itemRaw1(0)
                        Dim i1b1 As Bits8 = itemRaw1(1)
                        Dim i1b2 As Bits8 = itemRaw1(2)
                        Dim i1b3 As Bits8 = itemRaw1(3)
                        Dim i1b4 As Bits8 = itemRaw1(4)
                        i1b4.Bit3 = itemData1(0).Bit8
                        i1b4.Bit2 = itemData1(0).Bit7
                        i1b4.Bit1 = itemData1(0).Bit6
                        i1b3.Bit8 = itemData1(0).Bit5
                        i1b3.Bit7 = itemData1(0).Bit4
                        i1b3.Bit6 = itemData1(0).Bit3
                        i1b3.Bit5 = itemData1(0).Bit2
                        i1b3.Bit4 = itemData1(0).Bit1
                        i1b3.Bit3 = itemData1(1).Bit8
                        i1b3.Bit2 = itemData1(1).Bit7
                        i1b3.Bit1 = itemData1(1).Bit6
                        i1b2.Bit8 = itemData1(1).Bit5
                        i1b2.Bit7 = itemData1(1).Bit4
                        i1b2.Bit6 = itemData1(1).Bit3
                        i1b2.Bit5 = itemData1(1).Bit2
                        i1b2.Bit4 = itemData1(1).Bit1
                        i1b2.Bit3 = itemData1(2).Bit8
                        i1b2.Bit2 = itemData1(2).Bit7
                        i1b2.Bit1 = itemData1(2).Bit6
                        i1b1.Bit8 = itemData1(2).Bit5
                        i1b1.Bit7 = itemData1(2).Bit4
                        i1b1.Bit6 = itemData1(2).Bit3
                        i1b1.Bit5 = itemData1(2).Bit2
                        i1b1.Bit4 = itemData1(2).Bit1
                        i1b1.Bit3 = itemData1(3).Bit8
                        i1b1.Bit2 = itemData1(3).Bit7
                        i1b1.Bit1 = itemData1(3).Bit6
                        i1b0.Bit8 = itemData1(3).Bit5
                        i1b0.Bit7 = itemData1(3).Bit4
                        i1b0.Bit6 = itemData1(3).Bit3
                        i1b0.Bit5 = itemData1(3).Bit2
                        i1b0.Bit4 = itemData1(3).Bit1
                        i1b0.Bit3 = True 'Not Terminator
                        RawData(Offset + 0 + (i * 33) + 8) = i1b0
                        RawData(Offset + 1 + (i * 33) + 8) = i1b1
                        RawData(Offset + 2 + (i * 33) + 8) = i1b2
                        RawData(Offset + 3 + (i * 33) + 8) = i1b3
                        RawData(Offset + 4 + (i * 33) + 8) = i1b4
                    Else
                        RawData(Offset + 0 + (i * 33) + 8) = 0
                        RawData(Offset + 1 + (i * 33) + 8) = 0
                        RawData(Offset + 2 + (i * 33) + 8) = 0
                        RawData(Offset + 3 + (i * 33) + 8) = 0
                        RawData(Offset + 4 + (i * 33) + 8) = 0
                        terminated = True
                    End If
                    If (Not terminated) AndAlso value.Length > i * 8 + 3 AndAlso Not value(i * 8 + 3).IsTerminator Then
                        Dim itemRaw1 As Byte() = {Bit8.FromBinary(RawData, Offset + i * 33 + 12, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 12, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 1 + 12, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 2 + 12, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 3 + 12, 0)}
                        Dim itemData1 As Byte() = value(i * 8 + 3).ItemData
                        Dim i1b0 As Bits8 = itemRaw1(0)
                        Dim i1b1 As Bits8 = itemRaw1(1)
                        Dim i1b2 As Bits8 = itemRaw1(2)
                        Dim i1b3 As Bits8 = itemRaw1(3)
                        Dim i1b4 As Bits8 = itemRaw1(4)
                        i1b4.Bit4 = itemData1(0).Bit8
                        i1b4.Bit3 = itemData1(0).Bit7
                        i1b4.Bit2 = itemData1(0).Bit6
                        i1b4.Bit1 = itemData1(0).Bit5
                        i1b3.Bit8 = itemData1(0).Bit4
                        i1b3.Bit7 = itemData1(0).Bit3
                        i1b3.Bit6 = itemData1(0).Bit2
                        i1b3.Bit5 = itemData1(0).Bit1
                        i1b3.Bit4 = itemData1(1).Bit8
                        i1b3.Bit3 = itemData1(1).Bit7
                        i1b3.Bit2 = itemData1(1).Bit6
                        i1b3.Bit1 = itemData1(1).Bit5
                        i1b2.Bit8 = itemData1(1).Bit4
                        i1b2.Bit7 = itemData1(1).Bit3
                        i1b2.Bit6 = itemData1(1).Bit2
                        i1b2.Bit5 = itemData1(1).Bit1
                        i1b2.Bit4 = itemData1(2).Bit8
                        i1b2.Bit3 = itemData1(2).Bit7
                        i1b2.Bit2 = itemData1(2).Bit6
                        i1b2.Bit1 = itemData1(2).Bit5
                        i1b1.Bit8 = itemData1(2).Bit4
                        i1b1.Bit7 = itemData1(2).Bit3
                        i1b1.Bit6 = itemData1(2).Bit2
                        i1b1.Bit5 = itemData1(2).Bit1
                        i1b1.Bit4 = itemData1(3).Bit8
                        i1b1.Bit3 = itemData1(3).Bit7
                        i1b1.Bit2 = itemData1(3).Bit6
                        i1b1.Bit1 = itemData1(3).Bit5
                        i1b0.Bit8 = itemData1(3).Bit4
                        i1b0.Bit7 = itemData1(3).Bit3
                        i1b0.Bit6 = itemData1(3).Bit2
                        i1b0.Bit5 = itemData1(3).Bit1
                        i1b0.Bit4 = True 'Not Terminator
                        RawData(Offset + 0 + (i * 33) + 12) = i1b0
                        RawData(Offset + 1 + (i * 33) + 12) = i1b1
                        RawData(Offset + 2 + (i * 33) + 12) = i1b2
                        RawData(Offset + 3 + (i * 33) + 12) = i1b3
                        RawData(Offset + 4 + (i * 33) + 12) = i1b4
                    Else
                        RawData(Offset + 0 + (i * 33) + 12) = 0
                        RawData(Offset + 1 + (i * 33) + 12) = 0
                        RawData(Offset + 2 + (i * 33) + 12) = 0
                        RawData(Offset + 3 + (i * 33) + 12) = 0
                        RawData(Offset + 4 + (i * 33) + 12) = 0
                        terminated = True
                    End If
                    If i < 6 Then
                        If (Not terminated) AndAlso value.Length > i * 8 + 4 AndAlso Not value(i * 8 + 4).IsTerminator Then
                            Dim itemRaw1 As Byte() = {Bit8.FromBinary(RawData, Offset + i * 33 + 16, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 16, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 1 + 16, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 2 + 16, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 3 + 16, 0)}
                            Dim itemData1 As Byte() = value(i * 8 + 4).ItemData
                            Dim i1b0 As Bits8 = itemRaw1(0)
                            Dim i1b1 As Bits8 = itemRaw1(1)
                            Dim i1b2 As Bits8 = itemRaw1(2)
                            Dim i1b3 As Bits8 = itemRaw1(3)
                            Dim i1b4 As Bits8 = itemRaw1(4)
                            i1b4.Bit5 = itemData1(0).Bit8
                            i1b4.Bit4 = itemData1(0).Bit7
                            i1b4.Bit3 = itemData1(0).Bit6
                            i1b4.Bit2 = itemData1(0).Bit5
                            i1b4.Bit1 = itemData1(0).Bit4
                            i1b3.Bit8 = itemData1(0).Bit3
                            i1b3.Bit7 = itemData1(0).Bit2
                            i1b3.Bit6 = itemData1(0).Bit1
                            i1b3.Bit5 = itemData1(1).Bit8
                            i1b3.Bit4 = itemData1(1).Bit7
                            i1b3.Bit3 = itemData1(1).Bit6
                            i1b3.Bit2 = itemData1(1).Bit5
                            i1b3.Bit1 = itemData1(1).Bit4
                            i1b2.Bit8 = itemData1(1).Bit3
                            i1b2.Bit7 = itemData1(1).Bit2
                            i1b2.Bit6 = itemData1(1).Bit1
                            i1b2.Bit5 = itemData1(2).Bit8
                            i1b2.Bit4 = itemData1(2).Bit7
                            i1b2.Bit3 = itemData1(2).Bit6
                            i1b2.Bit2 = itemData1(2).Bit5
                            i1b2.Bit1 = itemData1(2).Bit4
                            i1b1.Bit8 = itemData1(2).Bit3
                            i1b1.Bit7 = itemData1(2).Bit2
                            i1b1.Bit6 = itemData1(2).Bit1
                            i1b1.Bit5 = itemData1(3).Bit8
                            i1b1.Bit4 = itemData1(3).Bit7
                            i1b1.Bit3 = itemData1(3).Bit6
                            i1b1.Bit2 = itemData1(3).Bit5
                            i1b1.Bit1 = itemData1(3).Bit4
                            i1b0.Bit8 = itemData1(3).Bit3
                            i1b0.Bit7 = itemData1(3).Bit2
                            i1b0.Bit6 = itemData1(3).Bit1
                            i1b0.Bit5 = True 'Not Terminator
                            RawData(Offset + 0 + (i * 33) + 16) = i1b0
                            RawData(Offset + 1 + (i * 33) + 16) = i1b1
                            RawData(Offset + 2 + (i * 33) + 16) = i1b2
                            RawData(Offset + 3 + (i * 33) + 16) = i1b3
                            RawData(Offset + 4 + (i * 33) + 16) = i1b4
                        Else
                            RawData(Offset + 0 + (i * 33) + 16) = 0
                            RawData(Offset + 1 + (i * 33) + 16) = 0
                            RawData(Offset + 2 + (i * 33) + 16) = 0
                            RawData(Offset + 3 + (i * 33) + 16) = 0
                            RawData(Offset + 4 + (i * 33) + 16) = 0
                            terminated = True
                        End If
                        If (Not terminated) AndAlso value.Length > i * 8 + 5 AndAlso Not value(i * 8 + 5).IsTerminator Then
                            Dim itemRaw1 As Byte() = {Bit8.FromBinary(RawData, Offset + i * 33 + 20, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 20, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 1 + 20, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 2 + 20, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 3 + 20, 0)}
                            Dim itemData1 As Byte() = value(i * 8 + 5).ItemData
                            Dim i1b0 As Bits8 = itemRaw1(0)
                            Dim i1b1 As Bits8 = itemRaw1(1)
                            Dim i1b2 As Bits8 = itemRaw1(2)
                            Dim i1b3 As Bits8 = itemRaw1(3)
                            Dim i1b4 As Bits8 = itemRaw1(4)
                            i1b4.Bit6 = itemData1(0).Bit8
                            i1b4.Bit5 = itemData1(0).Bit7
                            i1b4.Bit4 = itemData1(0).Bit6
                            i1b4.Bit3 = itemData1(0).Bit5
                            i1b4.Bit2 = itemData1(0).Bit4
                            i1b4.Bit1 = itemData1(0).Bit3
                            i1b3.Bit8 = itemData1(0).Bit2
                            i1b3.Bit7 = itemData1(0).Bit1
                            i1b3.Bit6 = itemData1(1).Bit8
                            i1b3.Bit5 = itemData1(1).Bit7
                            i1b3.Bit4 = itemData1(1).Bit6
                            i1b3.Bit3 = itemData1(1).Bit5
                            i1b3.Bit2 = itemData1(1).Bit4
                            i1b3.Bit1 = itemData1(1).Bit3
                            i1b2.Bit8 = itemData1(1).Bit2
                            i1b2.Bit7 = itemData1(1).Bit1
                            i1b2.Bit6 = itemData1(2).Bit8
                            i1b2.Bit5 = itemData1(2).Bit7
                            i1b2.Bit4 = itemData1(2).Bit6
                            i1b2.Bit3 = itemData1(2).Bit5
                            i1b2.Bit2 = itemData1(2).Bit4
                            i1b2.Bit1 = itemData1(2).Bit3
                            i1b1.Bit8 = itemData1(2).Bit2
                            i1b1.Bit7 = itemData1(2).Bit1
                            i1b1.Bit6 = itemData1(3).Bit8
                            i1b1.Bit5 = itemData1(3).Bit7
                            i1b1.Bit4 = itemData1(3).Bit6
                            i1b1.Bit3 = itemData1(3).Bit5
                            i1b1.Bit2 = itemData1(3).Bit4
                            i1b1.Bit1 = itemData1(3).Bit3
                            i1b0.Bit8 = itemData1(3).Bit2
                            i1b0.Bit7 = itemData1(3).Bit1
                            i1b0.Bit6 = True 'Not Terminator
                            RawData(Offset + 0 + (i * 33) + 20) = i1b0
                            RawData(Offset + 1 + (i * 33) + 20) = i1b1
                            RawData(Offset + 2 + (i * 33) + 20) = i1b2
                            RawData(Offset + 3 + (i * 33) + 20) = i1b3
                            RawData(Offset + 4 + (i * 33) + 20) = i1b4
                        Else
                            RawData(Offset + 0 + (i * 33) + 20) = 0
                            RawData(Offset + 1 + (i * 33) + 20) = 0
                            RawData(Offset + 2 + (i * 33) + 20) = 0
                            RawData(Offset + 3 + (i * 33) + 20) = 0
                            RawData(Offset + 4 + (i * 33) + 20) = 0
                            terminated = True
                        End If
                        If (Not terminated) AndAlso value.Length > i * 8 + 6 AndAlso Not value(i * 8 + 6).IsTerminator Then
                            Dim itemRaw1 As Byte() = {Bit8.FromBinary(RawData, Offset + i * 33 + 24, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 24, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 1 + 24, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 2 + 24, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 3 + 24, 0)}
                            Dim itemData1 As Byte() = value(i * 8 + 6).ItemData
                            Dim i1b0 As Bits8 = itemRaw1(0)
                            Dim i1b1 As Bits8 = itemRaw1(1)
                            Dim i1b2 As Bits8 = itemRaw1(2)
                            Dim i1b3 As Bits8 = itemRaw1(3)
                            Dim i1b4 As Bits8 = itemRaw1(4)
                            i1b4.Bit7 = itemData1(0).Bit8
                            i1b4.Bit6 = itemData1(0).Bit7
                            i1b4.Bit5 = itemData1(0).Bit6
                            i1b4.Bit4 = itemData1(0).Bit5
                            i1b4.Bit3 = itemData1(0).Bit4
                            i1b4.Bit2 = itemData1(0).Bit3
                            i1b4.Bit1 = itemData1(0).Bit2
                            i1b3.Bit8 = itemData1(0).Bit1
                            i1b3.Bit7 = itemData1(1).Bit8
                            i1b3.Bit6 = itemData1(1).Bit7
                            i1b3.Bit5 = itemData1(1).Bit6
                            i1b3.Bit4 = itemData1(1).Bit5
                            i1b3.Bit3 = itemData1(1).Bit4
                            i1b3.Bit2 = itemData1(1).Bit3
                            i1b3.Bit1 = itemData1(1).Bit2
                            i1b2.Bit8 = itemData1(1).Bit1
                            i1b2.Bit7 = itemData1(2).Bit8
                            i1b2.Bit6 = itemData1(2).Bit7
                            i1b2.Bit5 = itemData1(2).Bit6
                            i1b2.Bit4 = itemData1(2).Bit5
                            i1b2.Bit3 = itemData1(2).Bit4
                            i1b2.Bit2 = itemData1(2).Bit3
                            i1b2.Bit1 = itemData1(2).Bit2
                            i1b1.Bit8 = itemData1(2).Bit1
                            i1b1.Bit7 = itemData1(3).Bit8
                            i1b1.Bit6 = itemData1(3).Bit7
                            i1b1.Bit5 = itemData1(3).Bit6
                            i1b1.Bit4 = itemData1(3).Bit5
                            i1b1.Bit3 = itemData1(3).Bit4
                            i1b1.Bit2 = itemData1(3).Bit3
                            i1b1.Bit1 = itemData1(3).Bit2
                            i1b0.Bit8 = itemData1(3).Bit1
                            i1b0.Bit7 = True 'Not Terminator
                            RawData(Offset + 0 + (i * 33) + 24) = i1b0
                            RawData(Offset + 1 + (i * 33) + 24) = i1b1
                            RawData(Offset + 2 + (i * 33) + 24) = i1b2
                            RawData(Offset + 3 + (i * 33) + 24) = i1b3
                            RawData(Offset + 4 + (i * 33) + 24) = i1b4
                        Else
                            RawData(Offset + 0 + (i * 33) + 24) = 0
                            RawData(Offset + 1 + (i * 33) + 24) = 0
                            RawData(Offset + 2 + (i * 33) + 24) = 0
                            RawData(Offset + 3 + (i * 33) + 24) = 0
                            RawData(Offset + 4 + (i * 33) + 24) = 0
                            terminated = True
                        End If
                        If (Not terminated) AndAlso value.Length > i * 8 + 7 AndAlso Not value(i * 8 + 7).IsTerminator Then
                            Dim itemRaw1 As Byte() = {Bit8.FromBinary(RawData, Offset + i * 33 + 28, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 28, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 1 + 28, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 2 + 28, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 3 + 28, 0)}
                            Dim itemData1 As Byte() = value(i * 8 + 7).ItemData
                            Dim i1b0 As Bits8 = itemRaw1(0)
                            Dim i1b1 As Bits8 = itemRaw1(1)
                            Dim i1b2 As Bits8 = itemRaw1(2)
                            Dim i1b3 As Bits8 = itemRaw1(3)
                            Dim i1b4 As Bits8 = itemRaw1(4)
                            i1b4.Bit8 = itemData1(0).Bit8
                            i1b4.Bit7 = itemData1(0).Bit7
                            i1b4.Bit6 = itemData1(0).Bit6
                            i1b4.Bit5 = itemData1(0).Bit5
                            i1b4.Bit4 = itemData1(0).Bit4
                            i1b4.Bit3 = itemData1(0).Bit3
                            i1b4.Bit2 = itemData1(0).Bit2
                            i1b4.Bit1 = itemData1(0).Bit1
                            i1b3.Bit8 = itemData1(1).Bit8
                            i1b3.Bit7 = itemData1(1).Bit7
                            i1b3.Bit6 = itemData1(1).Bit6
                            i1b3.Bit5 = itemData1(1).Bit5
                            i1b3.Bit4 = itemData1(1).Bit4
                            i1b3.Bit3 = itemData1(1).Bit3
                            i1b3.Bit2 = itemData1(1).Bit2
                            i1b3.Bit1 = itemData1(1).Bit1
                            i1b2.Bit8 = itemData1(2).Bit8
                            i1b2.Bit7 = itemData1(2).Bit7
                            i1b2.Bit6 = itemData1(2).Bit6
                            i1b2.Bit5 = itemData1(2).Bit5
                            i1b2.Bit4 = itemData1(2).Bit4
                            i1b2.Bit3 = itemData1(2).Bit3
                            i1b2.Bit2 = itemData1(2).Bit2
                            i1b2.Bit1 = itemData1(2).Bit1
                            i1b1.Bit8 = itemData1(3).Bit8
                            i1b1.Bit7 = itemData1(3).Bit7
                            i1b1.Bit6 = itemData1(3).Bit6
                            i1b1.Bit5 = itemData1(3).Bit5
                            i1b1.Bit4 = itemData1(3).Bit4
                            i1b1.Bit3 = itemData1(3).Bit3
                            i1b1.Bit2 = itemData1(3).Bit2
                            i1b1.Bit1 = itemData1(3).Bit1
                            i1b0.Bit8 = True 'Not Terminator
                            RawData(Offset + 0 + (i * 33) + 28) = i1b0
                            RawData(Offset + 1 + (i * 33) + 28) = i1b1
                            RawData(Offset + 2 + (i * 33) + 28) = i1b2
                            RawData(Offset + 3 + (i * 33) + 28) = i1b3
                            RawData(Offset + 4 + (i * 33) + 28) = i1b4
                        Else
                            RawData(Offset + 0 + (i * 33) + 28) = 0
                            RawData(Offset + 1 + (i * 33) + 28) = 0
                            RawData(Offset + 2 + (i * 33) + 28) = 0
                            RawData(Offset + 3 + (i * 33) + 28) = 0
                            RawData(Offset + 4 + (i * 33) + 28) = 0
                            terminated = True
                        End If
                    End If
                Next
                RawData = RawData
            End If
        End Set
    End Property
End Class
