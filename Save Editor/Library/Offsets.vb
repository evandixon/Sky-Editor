''' <summary>
''' Interface for use in GenericSave
''' </summary>
''' <remarks></remarks>
Public Interface Offsets
    ReadOnly Property TeamNameStart As Integer
    ReadOnly Property BackupSaveStart As Integer
    ReadOnly Property ChecksumEnd As Integer
    ReadOnly Property QuicksaveStart As Integer?
    ''' <summary>
    ''' Specifies the beginning offset of the data used to calculate the quicksave checksum
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property QuicksaveChecksumStart As Integer
    ''' <summary>
    ''' Specifies the ending offset of the data used to calculate the quicksave checksum
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property QuicksaveChecksumEnd As Integer
End Interface
Public Class SkyOffsets
    Implements Offsets

    Public ReadOnly Property TeamNameStart As Integer Implements Offsets.TeamNameStart
        Get
            Return &H994E
        End Get
    End Property

    Public ReadOnly Property BackupSaveStart As Integer Implements Offsets.BackupSaveStart
        Get
            Return &HC800
        End Get
    End Property

    Public ReadOnly Property ChecksumEnd As Integer Implements Offsets.ChecksumEnd
        Get
            Return &HB65A
        End Get
    End Property

    Public ReadOnly Property QuicksaveChecksumEnd As Integer Implements Offsets.QuicksaveChecksumEnd
        Get
            Return &H1E7FF
        End Get
    End Property

    Public ReadOnly Property QuicksaveChecksumStart As Integer Implements Offsets.QuicksaveChecksumStart
        Get
            Return &H19004
        End Get
    End Property

    Public ReadOnly Property QuicksaveStart As Integer? Implements Offsets.QuicksaveStart
        Get
            Return &H19000
        End Get
    End Property
End Class
Public Class TDOffsets
    Implements Offsets

    Public ReadOnly Property TeamNameStart As Integer Implements Offsets.TeamNameStart
        Get
            Return &H96F7
        End Get
    End Property

    Public ReadOnly Property BackupSaveStart As Integer Implements Offsets.BackupSaveStart
        Get
            Return &H10000
        End Get
    End Property

    Public ReadOnly Property ChecksumEnd As Integer Implements Offsets.ChecksumEnd
        Get
            Return &HDC7B
        End Get
    End Property

    Public ReadOnly Property QuicksaveChecksumEnd As Integer Implements Offsets.QuicksaveChecksumEnd
        Get
            Return &H2E0FF
        End Get
    End Property

    Public ReadOnly Property QuicksaveChecksumStart As Integer Implements Offsets.QuicksaveChecksumStart
        Get
            Return &H2E004
        End Get
    End Property

    Public ReadOnly Property QuicksaveStart As Integer? Implements Offsets.QuicksaveStart
        Get
            Return &H2E000
        End Get
    End Property
End Class
Public Class RBOffsets
    Implements Offsets

    Public ReadOnly Property BackupSaveStart As Integer Implements Offsets.BackupSaveStart
        Get
            Return &H6000
        End Get
    End Property

    Public ReadOnly Property ChecksumEnd As Integer Implements Offsets.ChecksumEnd
        Get
            Return &H57D0
        End Get
    End Property

    Public ReadOnly Property QuicksaveChecksumEnd As Integer Implements Offsets.QuicksaveChecksumEnd
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property QuicksaveChecksumStart As Integer Implements Offsets.QuicksaveChecksumStart
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property QuicksaveStart As Integer? Implements Offsets.QuicksaveStart
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property TeamNameStart As Integer Implements Offsets.TeamNameStart
        Get
            Return &H4EC8
        End Get
    End Property
End Class
Public Class RBOffsetsEU
    Implements Offsets

    Public ReadOnly Property BackupSaveStart As Integer Implements Offsets.BackupSaveStart
        Get
            Return &H6000
        End Get
    End Property

    Public ReadOnly Property ChecksumEnd As Integer Implements Offsets.ChecksumEnd
        Get
            Return &H57D0
        End Get
    End Property

    Public ReadOnly Property QuicksaveChecksumEnd As Integer Implements Offsets.QuicksaveChecksumEnd
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property QuicksaveChecksumStart As Integer Implements Offsets.QuicksaveChecksumStart
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property QuicksaveStart As Integer? Implements Offsets.QuicksaveStart
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property TeamNameStart As Integer Implements Offsets.TeamNameStart
        Get
            Return &H4EC8
        End Get
    End Property
End Class
Public Class OffsetManager
    ''' <summary>
    ''' Gets the offset definition using the save file, only for Pokemon Mystery Dungeon: Red/Blue Rescue Team and Explorers of Time/Darkness/Sky
    ''' </summary>
    ''' <param name="RawData"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Obsolete>
    Public Shared Function GetOffsets(RawData As Byte()) As Offsets
        Select Case RawData(&HD)
            Case &H54
                Return New TDOffsets
            Case &H53
                Return New SkyOffsets
            Case Else
                If Not (RawData(4) = &H50 AndAlso RawData(5) = &H4F AndAlso RawData(6) = &H4B AndAlso RawData(7) = &H45) Then
                    Return New RBOffsets
                Else
                    Return New SkyOffsets
                End If
        End Select
    End Function
    Public Shared Function GetOffsets(Save As MDSaveBase) As Offsets
        If TypeOf Save Is SkySave Then
            Return New SkyOffsets
        ElseIf TypeOf Save Is TDSave Then
            Return New TDOffsets
        ElseIf TypeOf Save Is RBSave Then
            Return New RBOffsets
        ElseIf TypeOf Save Is RBSaveEU Then
            Return New RBOffsetsEU
        Else
            Return New SkyOffsets
        End If
    End Function
End Class