Imports SkyEditor.skyjed.buffer
Imports SkyEditor.skyjed.util
Public Enum GameType
    RB = 1
    TD = 2
    Sky = 4
End Enum
Public Class GenericSave
#Region "Generic/Helper Methods"
    ''' <summary>
    ''' Depricated; moved to BitOperations.SubByteArr
    ''' </summary>
    ''' <param name="ByteArr"></param>
    ''' <param name="Index"></param>
    ''' <param name="EndPoint"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function SubByteArr(ByteArr As Byte(), Index As Integer, EndPoint As Integer) As Byte()
        Return BitOperations.SubByteArr(ByteArr, Index, EndPoint)
    End Function

    ''' <summary>
    ''' Depricated; use the RawData property.  For now, there's no functional difference.
    ''' </summary>
    ''' <remarks></remarks>
    Friend _rawData As Byte()
    ''' <summary>
    ''' The raw bytes of the save file.  Most properties should access this in order to access the save
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RawData As Byte()
        Get
            Return _rawData
        End Get
        Set(value As Byte())
            _rawData = value
        End Set
    End Property
    Public Property RawDataBits As BooleanBufferArray
        Get
            Dim split_data = BitConverterLE.splitBits(RawData)
            Return New BooleanBufferArray(split_data)
        End Get
        Set(value As BooleanBufferArray)
            RawData = BitConverterLE.packBits(value.GetSplitBytes)
        End Set
    End Property
    Friend Function CurrentOffsets() As Offsets
        Return OffsetManager.GetOffsets(_rawData)
    End Function

    Public Sub New(Save As Byte())
        _rawData = Save
    End Sub

    Public Sub FixChecksum()
        'First checksum
        Dim words As New List(Of UInt32)
        For count As Integer = 4 To CurrentOffsets.ChecksumEnd Step 4
            words.Add(BitConverter.ToUInt32(_rawData, count))
        Next
        Dim sum As UInt64 = 0
        For Each item In words
            sum += item
        Next
        Dim buffer() As Byte = BitConverter.GetBytes(sum)
        For x As Byte = 0 To 3
            _rawData(x) = buffer(x)
            _rawData(x + CurrentOffsets.BackupSaveStart) = buffer(x)
        Next
        'Quicksave checksum
        If CurrentOffsets.QuicksaveStart.HasValue Then
            words = New List(Of UInt32)
            For count As Integer = CurrentOffsets.QuicksaveChecksumStart To CurrentOffsets.QuicksaveChecksumEnd Step 4
                words.Add(BitConverter.ToUInt32(_rawData, count))
            Next
            sum = 0
            For Each item In words
                sum += item
            Next
            buffer = BitConverter.GetBytes(sum)
            For x As Byte = 0 To 3
                _rawData(x + CurrentOffsets.QuicksaveStart) = buffer(x)
            Next
        End If
    End Sub
    ''' <summary>
    ''' Returns the raw data of the save file, after fixing the checksum.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetBytes() As Byte()
        FixChecksum()
        Return _rawData
    End Function
#End Region

    ''' <summary>
    ''' Gets or sets the save file's Team Name.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TeamName As String
        Get
            Return StringUtilities.PMDEncodingToString(BitOperations.SubByteArr(_rawData, CurrentOffsets.TeamNameStart, CurrentOffsets.TeamNameStart + 9))
        End Get
        Set(value As String)
            Dim buffer As Byte() = StringUtilities.StringToPMDEncoding(value)
            For x As Integer = 0 To 9
                If buffer.Length > x Then
                    _rawData(CurrentOffsets.TeamNameStart + x) = buffer(x)
                    _rawData(CurrentOffsets.TeamNameStart + CurrentOffsets.BackupSaveStart + x) = buffer(x)
                Else
                    RawData(CurrentOffsets.TeamNameStart + x) = 0
                End If
            Next
        End Set
    End Property

    ''' <summary>
    ''' Specifies whether or not the save is an Explorers of Time/Darkness save.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IsTDSave As Boolean
        Get
            Return (_rawData(&HD) = &H54)
        End Get
    End Property

    ''' <summary>
    ''' Specifies whether or not the save is an Explorers of Sky save.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IsSkySave As Boolean
        Get
            Return (_rawData(&HD) = &H53)
        End Get
    End Property

    Public ReadOnly Property IsRBSave As Boolean
        Get
            Return (TypeOf CurrentOffsets() Is RBOffsets)
        End Get
    End Property

End Class