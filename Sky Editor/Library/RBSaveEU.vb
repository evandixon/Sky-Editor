Imports SkyEditor.skyjed.save
Imports SkyEditorBase
Imports System
Imports System.Collections.Generic
Imports SkyEditorBase.Utilities

Public Class RBSaveEU
    Inherits MDSaveBase

    '&h67: Base type?
    'Pikachu: 2210fb27 00000000
    'Meowth: 2210fb27 00000001
    'Eevee: 2210fb27 00000002
    'Skitty: 2210fb27 00000003
    'Squirtle: 2210fb27 00000004
    'Totodile: 2210fb27 00000005
    'Mudkip: 2210fb27 00000006
    'Psyduck: 2210fb27 00000007
    'Charmander: 2210fb27 00000008
    'Torchic: 2210fb27 00000009
    'Cyndaquil: 2210fb27 0000000A
    'Cubone: 2210fb27 0000000B
    'Machop: 2210fb27 0000000C
    'Bulbasaur: 2210fb27 000000OD
    'Chikorita: 2210fb27 000000OE
    'Treeko: 2210fb27 000000OF

    Public Property BaseType As Byte
        Get
            Return RawData(&H67)
        End Get
        Set(value As Byte)
            RawData(&H67) = value
        End Set
    End Property

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
    Public Property HeldMoney As Integer
        Get
            Const offset As Integer = &H4E6C
            Return BitConverter.ToInt32({RawData(offset), RawData(offset + 1), RawData(offset + 2), 0}, 0)
        End Get
        Set(value As Integer)
            Const offset As Integer = &H4E6C
            Dim bytes() = BitConverter.GetBytes(value)
            For count As Byte = 0 To 2
                RawData(offset + count) = bytes(0 + count)
            Next
        End Set
    End Property
    Public Property StoredMoney As Integer
        Get
            Const offset As Integer = &H4E6F
            Return BitConverter.ToInt32({RawData(offset), RawData(offset + 1), RawData(offset + 2), 0}, 0)
        End Get
        Set(value As Integer)
            Const offset As Integer = &H4E6F
            Dim bytes() = BitConverter.GetBytes(value)
            For count As Byte = 0 To 2
                RawData(offset + count) = bytes(0 + count)
            Next
        End Set
    End Property
    Public Property RescuePoints As Integer
        Get
            Const offset As Integer = &H4ED3
            Return BitConverter.ToInt16({RawData(offset), RawData(offset + 1)}, 0)
        End Get
        Set(value As Integer)
            Const offset As Integer = &H4ED3
            Dim bytes() = BitConverter.GetBytes(value)
            For count As Byte = 0 To 1
                RawData(offset + count) = bytes(0 + count)
            Next
        End Set
    End Property
#Region "Offsets"
    Public Const HeldItemsOffset As Integer = &H4CF0
    Public Const StoredItemsOffsetBits As Integer = &H4D2B * 8 - 2
#End Region
    Public Property HeldItems As skyjed.save.RBHeldItemStorage
        Get
            Dim Offset As Integer = HeldItemsOffset
            Dim ItemLength As Integer = 23
            Dim buf = RawDataBits
            Return New RBHeldItemStorage(buf.seek(Offset * 8).view(ItemLength * 20))
        End Get
        Set(value As skyjed.save.RBHeldItemStorage)
            Dim Offset As Integer = HeldItemsOffset
            Dim ItemLength As Integer = 23
            Dim buf = RawDataBits
            value.store(buf.seek(Offset * 8).view(ItemLength * 20))
            RawDataBits = buf
        End Set
    End Property
    Public Property AncientRelicPokemon As skyjed.save.PkmnStorageRB
        Get
            Dim Offset As Integer = &H3DD9 * 8 - 1
            Dim ItemLength As Integer = 323
            Dim buf = RawDataBits
            Return New PkmnStorageRB(buf.seek(Offset).view(ItemLength * 6), 6)
        End Get
        Set(value As skyjed.save.PkmnStorageRB)
            Dim Offset As Integer = &H3DD9 * 8
            Dim ItemLength As Integer = 323
            Dim buf = RawDataBits
            value.store(buf.seek(Offset).view(ItemLength * 6))
            RawDataBits = buf
        End Set
    End Property
    Public Property StoredPokemon As skyjed.save.PkmnStorageRB
        Get
            Dim Offset As Integer = (&H5B7 * 8 + 3) - (323 * 3)
            Dim ItemLength As Integer = 323
            Dim buf = RawDataBits
            Return New PkmnStorageRB(buf.seek(Offset).view(ItemLength * 407), 407)
            'Dim Offset As Integer = &H9CD * 8 + 1
            'Dim ItemLength As Integer = 347
            'Dim buf = RawDataBits
            'Return New PkmnStorageRB(buf.seek(Offset).view(ItemLength * 6), 378)
        End Get
        Set(value As skyjed.save.PkmnStorageRB)
            Dim Offset As Integer = &H5B7 * 8 + 3 - (323 * 3)
            Dim ItemLength As Integer = 323
            Dim buf = RawDataBits
            value.store(buf.seek(Offset).view(ItemLength * 407))
            RawDataBits = buf
        End Set
    End Property
    Public Property StoredItemCounts As Integer()
        Get
            Dim out(239) As Integer
            Dim buf = RawDataBits
            Dim block = buf.seek(StoredItemsOffsetBits).view(2400)
            For count As Integer = 0 To 238
                out(count) = block.getInt(10)
            Next
            Return out
        End Get
        Set(value As Integer())
            Throw New NotImplementedException
        End Set
    End Property
    Public Class RBFriendAreaOffsetDefinition
        Public Property Index As Integer
        Public Property AreaName As String
        Public Property Length As Integer
        Public Property CurrentPokemon As Integer = 0
        Public Sub New(Index As Integer, AreaName As String, Length As Integer)
            Me.Index = Index
            Me.AreaName = AreaName
            Me.Length = Length
        End Sub
        Public Shared Function FromLine(Line As String, Index As Integer) As RBFriendAreaOffsetDefinition
            Dim parts = Line.Split(":")
            Return New RBFriendAreaOffsetDefinition(Index, parts(0).Trim, parts(1).Trim)
        End Function
        Public Shared Function FromLines(Lines As String) As List(Of RBFriendAreaOffsetDefinition)
            Dim out As New List(Of RBFriendAreaOffsetDefinition)
            Dim offset As Integer = 0
            For Each Line In Lines.Split(vbCrLf)
                If Not Line.Trim.StartsWith("#") Then
                    out.Add(FromLine(Line.Trim, offset))
                    offset += out.Last.Length
                End If
            Next
            Return out
        End Function
        Public Overrides Function ToString() As String
            Return AreaName & " (" & CurrentPokemon & "/" & Length & ")"
        End Function
    End Class
    Public Shared Function FromBase(ByVal Base As MDSaveBase) As RBSaveEU
        Return New RBSaveEU(Base.RawData)
    End Function
    Public Function ToBase() As GenericSave
        Return DirectCast(Me, GenericSave)
    End Function

    Public Overrides ReadOnly Property SaveID As String
        Get
            Return GameStrings.RBSave
        End Get
    End Property
End Class