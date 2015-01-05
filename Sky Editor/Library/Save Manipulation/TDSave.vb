Imports SkyEditor.skyjed.buffer
Imports SkyEditor.skyjed.save
Imports SkyEditor.skyjed.util
Imports SkyEditorBase

Partial Class TDSave
    Inherits MDSaveBase
    ''' <summary>
    ''' Gets or sets the save file's Team Name.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TeamName As String
        Get
            Return StringUtilities.PMDEncodingToString(GenericArrayOperations(Of Byte).CopyOfRange(RawData, CurrentOffsets.TeamNameStart, CurrentOffsets.TeamNameStart + 9))
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
    Public Class TDItem
        'Public Property BoxContents As SkyItem
        '    Get
        '        Dim count As Bit8 = Bit8.FromBits(ItemData(3).Bit1, ItemData(3).Bit2, ItemData(3).Bit3, ItemData(3).Bit4, ItemData(3).Bit5, ItemData(3).Bit6, ItemData(3).Bit7, 0)
        '        Dim id As UInt16 = BitConverter.ToUInt16({Bit8.FromBinary(ItemData, 2, 1), (Bit8.FromBits(ItemData(2).Bit8, ItemData(1).Bit1, ItemData(1).Bit2, 0, 0, 0, 0, 0))}, 0)
        '        Return New SkyItem(id, count)
        '    End Get
        '    Set(value As SkyItem)
        '        Dim idBuffer As Byte() = BitConverter.GetBytes(value.ItemID)
        '        Dim countBuffer As Byte() = BitConverter.GetBytes(value.Number)
        '        Dim b1 As Bit8 = ItemData(1)
        '        Dim b2 As Bit8 = ItemData(2)
        '        Dim b3 As Bit8 = ItemData(3)
        '        b1.Bit2 = idBuffer(1).Bit3
        '        b1.Bit1 = idBuffer(1).Bit2
        '        b2.Bit8 = idBuffer(1).Bit1
        '        b2.Bit7 = idBuffer(0).Bit8
        '        b2.Bit6 = idBuffer(0).Bit7
        '        b2.Bit5 = idBuffer(0).Bit6
        '        b2.Bit4 = idBuffer(0).Bit5
        '        b2.Bit3 = idBuffer(0).Bit4
        '        b2.Bit2 = idBuffer(0).Bit3
        '        b2.Bit1 = idBuffer(0).Bit2
        '        b3.Bit8 = idBuffer(0).Bit1
        '        b3.Bit7 = countBuffer(0).Bit7
        '        b3.Bit6 = countBuffer(0).Bit6
        '        b3.Bit5 = countBuffer(0).Bit5
        '        b3.Bit4 = countBuffer(0).Bit4
        '        b3.Bit3 = countBuffer(0).Bit3
        '        b3.Bit2 = countBuffer(0).Bit2
        '        b3.Bit1 = countBuffer(0).Bit1
        '        ItemData(1) = b1
        '        ItemData(2) = b2
        '        ItemData(3) = b3
        '    End Set
        'End Property
        Public ReadOnly Property FriendlyName As String
            Get
                Dim out As String = ""
                If Lists.SkyItemNames.ContainsKey(ItemID) Then
                    out = Lists.TDItemNames(ItemID)
                    'If (Number > 0) AndAlso Not IsBox Then
                    '    out = out & " (" & Number & ")"
                    'End If
                    'If HeldBy > 0 Then
                    '    out = "[Held By: " & HeldBy & "] " & out
                    'End If
                    'If IsBox Then
                    '    If Not BoxContents.ItemID = 0 Then
                    '        out = out & " [Item: " & BoxContents.ToString & "]"
                    '    Else
                    '        out = out & " [Empty]"
                    '    End If
                    'End If
                End If
                If Not Lists.TDItemNames.ContainsKey(ItemID) OrElse Settings.DebugMode Then
                    out = "[Item " & ItemID & "] (" & Number & ") " & out
                End If
                Return out.Trim
            End Get
        End Property
        Public Property ItemData As Byte()
        Public Property ItemID As UInt16
            Get
                Return BitConverter.ToUInt16({Bit8.FromBinary(ItemData, 0, 6), (Bit8.FromBits(ItemData(0).Bit3, ItemData(0).Bit4, ItemData(0).Bit5, 0, 0, 0, 0, 0))}, 0)
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
                Return Bit8.FromBinary(ItemData, 2, 1)
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
            End Set
        End Property
        ' ''' <summary>
        ' ''' Returns the number team member who's holding the item (1-4), or 0 if it's not held
        ' ''' </summary>
        ' ''' <value></value>
        ' ''' <returns></returns>
        ' ''' <remarks></remarks>
        'Public ReadOnly Property HeldBy As Byte
        '    Get
        '        Return Bit8.FromBits(ItemData(0).Bit6, ItemData(0).Bit7, ItemData(0).Bit8, 0, 0, 0, 0, 0)
        '    End Get
        'End Property
        Public ReadOnly Property IsTerminator As Boolean
            Get
                Return (ItemData(0) = 0 AndAlso ItemData(1) = 0 AndAlso ItemData(2) = 0 AndAlso ItemData(3) = 0)
            End Get
        End Property
        'Public ReadOnly Property IsBox As Boolean
        '    Get
        '        Return ItemID > 363 AndAlso ItemID < 400
        '    End Get
        'End Property
        Public Sub New(ItemID As UInt16, Number As Byte)
            ItemData = {0, 0, 0, 0, 0}
            Me.ItemID = ItemID
            Me.Number = Number
        End Sub
        Public Sub New(Buf As BooleanBuffer)

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
#Region "Offsets"
    Public Const HeldItemsOffset As Integer = &H8B71
#End Region
    Public Property HeldItems As skyjed.save.TDHeldItemStorage
        Get
            Dim Offset As Integer = HeldItemsOffset
            Dim ItemLength As Integer = 31
            Dim buf = RawDataBits
            Return New TDHeldItemStorage(buf.seek(Offset * 8).view(ItemLength * 48))
        End Get
        Set(value As skyjed.save.TDHeldItemStorage)
            Dim Offset As Integer = HeldItemsOffset
            Dim ItemLength As Integer = 31
            Dim buf = RawDataBits
            value.store(buf.seek(Offset * 8).view(ItemLength * 48))
            RawDataBits = buf
        End Set
    End Property
    Public Property ActivePokemon As skyjed.save.ActivePkmnTD
        Get
            Dim Offset As Integer = &H83CB * 8
            Dim ItemLength As Integer = 544
            Dim buf = RawDataBits
            Return New ActivePkmnTD(buf.seek(Offset).view(ItemLength * 4))
        End Get
        Set(value As skyjed.save.ActivePkmnTD)
            Dim Offset As Integer = &H83CB * 8
            Dim ItemLength As Integer = 544
            Dim buf = RawDataBits
            value.store(buf.seek(Offset).view(ItemLength * 4))
            RawDataBits = buf
        End Set
    End Property
    Public Property StoredPokemon As skyjed.save.PkmnStorageTD
        Get
            Dim Offset As Integer = &H460 * 8 + 3
            Dim ItemLength As Integer = 388
            Dim buf = RawDataBits
            Return New PkmnStorageTD(buf.seek(Offset).view(ItemLength * 550))
        End Get
        Set(value As skyjed.save.PkmnStorageTD)
            Dim Offset As Integer = &H460 * 8 + 3
            Dim ItemLength As Integer = 388
            Dim buf = RawDataBits
            value.store(buf.seek(Offset).view(ItemLength * 550))
            RawDataBits = buf
        End Set
    End Property
    '    Public ReadOnly Property HeldItems As List(Of TDItem)
    '        'Get
    '        '    Dim out As New List(Of TDItem)
    '        '    For count As Integer = HeldItemsOffset To HeldItemsOffset + (50 * 4) Step 4
    '        '        Dim i As New TDItem({RawData(count + 3), RawData(count + 2), RawData(count + 1), RawData(count)})
    '        '        If Not i.IsTerminator Then
    '        '            out.Add(i)
    '        '        Else
    '        '            Exit For
    '        '        End If
    '        '    Next
    '        '    Return out
    '        'End Get
    '        Get
    '            Dim out As New List(Of TDItem)
    '            Dim Offset As Integer = HeldItemsOffset
    '            Dim ItemLength As Integer = 32
    '            Dim buf = RawDataBits
    '            buf.seek(Offset)
    '            out.Add(New TDItem(buf.getBytes(ItemLength)))
    '            'For i As Byte = 0 To 6
    '            '    'Dim item1 As New TDItem({RawData(Offset + (i * 33) + 3), RawData(Offset + (i * 33) + 2), RawData(Offset + (i * 33) + 1), RawData(Offset + (i * 33))})
    '            '    'If Not item1.IsTerminator Then
    '            '    '    out.Add(item1)
    '            '    'Else
    '            '    '    GoTo ReturnSpEp
    '            '    'End If
    '            '    For j As Byte = 0 To 7
    '            '        If Not (i = 6 AndAlso j > 1) Then
    '            '            Dim itemRaw As Byte()
    '            '            itemRaw = {RawData(Offset + 0 + i * 31 + j * 4), RawData(Offset + 1 + i * 31 + j * 4), RawData(Offset + 2 + i * 31 + j * 4), RawData(Offset + 3 + i * 31 + j * 4), RawData(Offset + 4 + i * 31 + j * 4), 0}
    '            '            itemRaw = BitOperations.ShiftLeftPMD(itemRaw, (j), 0, 4)
    '            '            Array.Resize(itemRaw, 4)
    '            '            Array.Reverse(itemRaw)
    '            '            Dim item2 As New TDItem(itemRaw)
    '            '            If Not item2.IsTerminator Then
    '            '                out.Add(item2)
    '            '            Else
    '            '                GoTo ReturnSpEp
    '            '            End If
    '            '        End If
    '            '    Next
    '            '    'If i < 6 Then
    '            '    '    Dim itemRaw8 As Byte() = {Bit8.FromBinary(RawData, Offset + i * 33 + 28, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 28, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 1 + 28, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 2 + 28, 0), Bit8.FromBinary(RawData, Offset + 1 + i * 33 + 3 + 28, 0)}
    '            '    '    Array.Reverse(itemRaw8)
    '            '    '    Dim item8 As New TDItem(itemRaw8)
    '            '    '    If Not item8.IsTerminator Then
    '            '    '        out.Add(item8)
    '            '    '    Else
    '            '    '        Exit For
    '            '    '    End If
    '            '    'End If
    '            'Next
    'ReturnSpEp: Return out
    '        End Get
    '    End Property
    <Obsolete()>
    Public Shared Function FromBase(Base As GenericSave) As TDSave
        Return New TDSave(Base.RawData)
    End Function
    <Obsolete>
    Public Function ToBase() As GenericSave
        Return DirectCast(Me, GenericSave)
    End Function

    Public Overrides ReadOnly Property SaveID As String
        Get
            Return GameConstants.TDSave
        End Get
    End Property
End Class
