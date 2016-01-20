Imports SkyEditorBase
Imports SkyEditorBase.Utilities

Public Class SkySave
    Inherits MDSaveBase
    ''' <summary>
    ''' Gets or sets the save file's Team Name.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TeamName As String
        Get
            Return StringUtilities.PMDEncodingToString(RawData(CurrentOffsets.TeamNameStart, 9))
        End Get
        Set(value As String)
            Dim buffer As Byte() = StringUtilities.StringToPMDEncoding(value)
            For x As Integer = 0 To 9
                If buffer.Length > x Then
                    RawData(CurrentOffsets.TeamNameStart + x) = buffer(x)
                    RawData(CurrentOffsets.TeamNameStart + CurrentOffsets.BackupSaveStart + x) = buffer(x)
                Else
                    RawData(CurrentOffsets.TeamNameStart + x) = 0
                End If
            Next
        End Set
    End Property
    Public Property RankPoints As Integer
        Get
            Return BitConverter.ToInt32(RawData(0, 4), &H9958)
        End Get
        Set(value As Integer)
            Dim bytes = BitConverter.GetBytes(value)
            For i As Byte = 0 To 3
                RawData(&H9958 + i) = bytes(i)
            Next
        End Set
    End Property

    Public Property JSave As skyjed.save.SaveSlot
        Get
            Return (New skyjed.save.SaveFile(RawData)).slot1
        End Get
        Set(value As skyjed.save.SaveSlot)
            Dim j = (New skyjed.save.SaveFile(RawData))
            j.slot1 = value
            RawData = j.toByteA
        End Set
    End Property
    Public Property QSave As skyjed.save.QuickSaveSlot
        Get
            Return (New skyjed.save.SaveFile(RawData)).qslot
        End Get
        Set(value As skyjed.save.QuickSaveSlot)
            Dim j = (New skyjed.save.SaveFile(RawData))
            j.qslot = value
            RawData = j.toByteA
        End Set
    End Property
    Public Class SkyItem
        Public Property BoxContents As SkyItem
            Get
                Dim count As Bits8 = Bits8.FromBits(ItemData(3).Bit1, ItemData(3).Bit2, ItemData(3).Bit3, ItemData(3).Bit4, ItemData(3).Bit5, ItemData(3).Bit6, ItemData(3).Bit7, 0)
                Dim id As UInt16 = BitConverter.ToUInt16({Bits8.FromBinary(ItemData, 2, 1), (Bits8.FromBits(ItemData(2).Bit8, ItemData(1).Bit1, ItemData(1).Bit2, 0, 0, 0, 0, 0))}, 0)
                Return New SkyItem(id, count)
            End Get
            Set(value As SkyItem)
                Dim idBuffer As Byte() = BitConverter.GetBytes(value.ItemID)
                Dim countBuffer As Byte() = BitConverter.GetBytes(value.Number)
                Dim b1 As Bits8 = ItemData(1)
                Dim b2 As Bits8 = ItemData(2)
                Dim b3 As Bits8 = ItemData(3)
                b1.Bit2 = idBuffer(1).Bit3
                b1.Bit1 = idBuffer(1).Bit2
                b2.Bit8 = idBuffer(1).Bit1
                b2.Bit7 = idBuffer(0).Bit8
                b2.Bit6 = idBuffer(0).Bit7
                b2.Bit5 = idBuffer(0).Bit6
                b2.Bit4 = idBuffer(0).Bit5
                b2.Bit3 = idBuffer(0).Bit4
                b2.Bit2 = idBuffer(0).Bit3
                b2.Bit1 = idBuffer(0).Bit2
                b3.Bit8 = idBuffer(0).Bit1
                b3.Bit7 = countBuffer(0).Bit7
                b3.Bit6 = countBuffer(0).Bit6
                b3.Bit5 = countBuffer(0).Bit5
                b3.Bit4 = countBuffer(0).Bit4
                b3.Bit3 = countBuffer(0).Bit3
                b3.Bit2 = countBuffer(0).Bit2
                b3.Bit1 = countBuffer(0).Bit1
                ItemData(1) = b1
                ItemData(2) = b2
                ItemData(3) = b3
            End Set
        End Property
        Public ReadOnly Property FriendlyName As String
            Get
                Dim out As String = ""
                If Lists.SkyItemNames.ContainsKey(ItemID) Then
                    out = Lists.SkyItemNames(ItemID)
                    If (Number > 0) AndAlso Not IsBox Then
                        out = out & " (" & Number & ")"
                    End If
                    If HeldBy > 0 Then
                        out = "[Held By: " & HeldBy & "] " & out
                    End If
                    If IsBox Then
                        If Not BoxContents.ItemID = 0 Then
                            out = out & " [Item: " & BoxContents.ToString & "]"
                        Else
                            out = out & " [Empty]"
                        End If
                    End If
                End If
                'If Not Lists.SkyItemNames.ContainsKey(ItemID) OrElse Settings.DebugMode Then
                '    out = "[Item " & ItemID & "] (" & Number & ") " & out
                'End If
                Return out.Trim
            End Get
        End Property
        Public Property ItemData As Byte()
        Public Property ItemID As UInt16
            Get
                Return BitConverter.ToUInt16({Bits8.FromBinary(ItemData, 0, 6), (Bits8.FromBits(ItemData(0).Bit3, ItemData(0).Bit4, ItemData(0).Bit5, 0, 0, 0, 0, 0))}, 0)
            End Get
            Set(value As UInt16)
                Dim buffer As Byte() = BitConverter.GetBytes(value)
                Dim b0 As Bits8 = ItemData(0)
                Dim b1 As Bits8 = ItemData(1)
                b0.Bit2 = buffer(0).Bit8
                b0.Bit1 = buffer(0).Bit7
                b1.Bit8 = buffer(0).Bit6
                b1.Bit7 = buffer(0).Bit5
                b1.Bit6 = buffer(0).Bit4
                b1.Bit5 = buffer(0).Bit3
                b1.Bit4 = buffer(0).Bit2
                b1.Bit3 = buffer(0).Bit1

                b0.Bit3 = buffer(1).Bit1
                b0.Bit4 = buffer(1).Bit2
                b0.Bit5 = buffer(1).Bit3
                ItemData(0) = b0
                ItemData(1) = b1
            End Set
        End Property
        Public Property Number As Byte
            Get
                Return Bits8.FromBinary(ItemData, 2, 1)
            End Get
            Set(value As Byte)
                Dim b2 As Bits8 = ItemData(2)
                Dim b3 As Bits8 = ItemData(3)
                b2.Bit7 = value.Bit8
                b2.Bit6 = value.Bit7
                b2.Bit5 = value.Bit6
                b2.Bit4 = value.Bit5
                b2.Bit3 = value.Bit4
                b2.Bit2 = value.Bit3
                b2.Bit1 = value.Bit2
                b3.Bit8 = value.Bit1
                ItemData(2) = b2
                ItemData(3) = b3
            End Set
        End Property
        ''' <summary>
        ''' Returns the number team member who's holding the item (1-4), or 0 if it's not held
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property HeldBy As Byte
            Get
                Return Bits8.FromBits(ItemData(0).Bit6, ItemData(0).Bit7, ItemData(0).Bit8, 0, 0, 0, 0, 0)
            End Get
        End Property
        Public ReadOnly Property IsTerminator As Boolean
            Get
                Return (ItemData(0) = 0 AndAlso ItemData(1) = 0 AndAlso ItemData(2) = 0 AndAlso ItemData(3) = 0)
            End Get
        End Property
        Public ReadOnly Property IsBox As Boolean
            Get
                Return ItemID > 363 AndAlso ItemID < 400
            End Get
        End Property
        Public Sub New(ItemID As UInt16, Number As Byte)
            ItemData = {0, 0, 0, 0, 0}
            Me.ItemID = ItemID
            Me.Number = Number
        End Sub
        Public Sub New(ItemData As Byte())
            Me.ItemData = ItemData
        End Sub
        Public Overrides Function ToString() As String
            Return FriendlyName.Trim
        End Function
    End Class

    Public Sub New(Save As Byte())
        MyBase.New(Save)
    End Sub

    Public Property HeldMoney As UInt32
        Get
            Return BitConverter.ToUInt16(BitConverter.GetBytes((BitConverter.ToUInt32(RawData(&H990C, 4), 0) << 2)), 1)
        End Get
        Set(value As UInt32)
            Dim b1 = BitConverter.GetBytes(BitConverter.ToUInt32(RawData(&H990C, 4), 0) << 2)
            Dim b2 = BitConverter.GetBytes(value)
            b1(1) = b2(0)
            b1(2) = b2(1)
            b1(3) = b2(2)
            Dim out = BitConverter.GetBytes(BitConverter.ToUInt32(b1, 0) >> 2)
            For x As Integer = 0 To 3
                RawData(&H990C + x) = out(x)
                RawData(&H990C + &HC800 + x) = out(x)
            Next
        End Set
    End Property

    Public Property SpEpisode_HeldMoney As UInt32
        Get
            Return BitConverter.ToUInt16(BitConverter.GetBytes((BitConverter.ToUInt32(RawData(&H990F, 4), 0) << 2)), 1)
        End Get
        Set(value As UInt32)
            Dim b1 = BitConverter.GetBytes(BitConverter.ToUInt32(RawData(&H990F, 4), 0) << 2)
            Dim b2 = BitConverter.GetBytes(value)
            b1(1) = b2(0)
            b1(2) = b2(1)
            b1(3) = b2(2)
            Dim out = BitConverter.GetBytes(BitConverter.ToUInt32(b1, 0) >> 2)
            For x As Integer = 0 To 3
                RawData(&H990F + x) = out(x)
                RawData(&H990F + &HC800 + x) = out(x)
            Next
        End Set
    End Property

    ' ''' <summary>
    ' ''' Gets or sets the player's money in storage.  Max of 16,580,607 that will be lowered in game to 9,999,999 once you withdraw any amount of money.
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Property StoredMoney As UInt32
    '    Get
    '        Dim raw As Byte() = (GenericArrayOperations(Of Byte).CopyOfRange(RawData, &H9915, &H991F))
    '        Return BitConverter.ToUInt32(BitOperations.ShiftRightPMD({raw(0), raw(1), raw(2), 0, 0}, 6, 0, 4), 0)
    '    End Get
    '    Set(value As UInt32)
    '        Dim b1 = BitConverter.GetBytes(BitConverter.ToUInt32(GenericArrayOperations(Of Byte).CopyOfRange(RawData, &H9915, &H991A), 0))
    '        Dim b2 = BitConverter.GetBytes(value)

    '        Array.Reverse(b2)
    '        Array.Resize(b2, 5)
    '        Array.Reverse(b2)

    '        b2 = BitOperations.EncodeBytes(b2, 6, 32)
    '        'nope: b2 = BitOperations.ShiftLeftPMD(b2, 32, 6, b2.Length)

    '        Dim out = b2
    '        For x As Integer = 0 To 3
    '            RawData(&H9915 + x) = b2(x)
    '            RawData(&H9915 + &HC800 + x) = b2(x)
    '        Next
    '    End Set
    'End Property

    Public Property AdventuresHad As Integer
        Get
            Return BitConverter.ToInt32(RawData(&H8B70, 4), 0)
        End Get
        Set(value As Integer)
            Dim buffer As Byte() = BitConverter.GetBytes(value)
            RawData(&H8B70) = buffer(0)
            RawData(&H8B70 + 1) = buffer(1)
            RawData(&H8B70 + 2) = buffer(2)
            RawData(&H8B70 + 3) = buffer(3)
            RawData(&H8B70 + CurrentOffsets.BackupSaveStart) = buffer(0)
            RawData(&H8B70 + 1 + CurrentOffsets.BackupSaveStart) = buffer(1)
            RawData(&H8B70 + 2 + CurrentOffsets.BackupSaveStart) = buffer(2)
            RawData(&H8B70 + 3 + CurrentOffsets.BackupSaveStart) = buffer(3)
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the type of window frame used in the game.  Must be 1-5.
    ''' Set is untested.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property WindowFrameType As Byte
        Get
            Return (RawData(&H995F) >> 5) + 1
        End Get
        Set(value As Byte)
            'RawData(&H995F) = RawData(&H995F) - (WindowFrameType << 5)
            RawData(&H995F) = RawData(&H995F) - (value - 1 << 5)
        End Set
    End Property
    Public Overrides Function DefaultSaveID() As String
        Return GameStrings.SkySave
    End Function
End Class