Imports SkyEditor.skyjed.buffer
Imports SkyEditor.skyjed.save
Imports SkyEditor.skyjed.util
Imports SkyEditorBase
Imports SkyEditorBase.Utilities

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
            Return GameStrings.TDSave
        End Get
    End Property
End Class
