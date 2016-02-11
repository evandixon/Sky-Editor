Namespace FileFormats.CTR
    Public Class RomFSHeader

#Region "Physical Properties"
        Public Property MagicStr As String
        Public Property MagicNum As UInteger
        Public Property MasterHashSize As UInteger
        Public Property Level1LogicalOffset As ULong
        Public Property Level1HashDataSize As ULong
        Public Property Level1BlockSizeLog2 As UInteger
        Public Property Reserved1 As UInteger
        Public Property Level2LogicalOffset As ULong
        Public Property Level2HashDataSize As ULong
        Public Property Level2BlockSizeLog2 As UInteger
        Public Property Reserved2 As UInteger
        Public Property Level3LogicalOffset As ULong
        Public Property Level3HashDataSize As ULong
        Public Property Level3BlockSizeLog2 As UInteger
        Public Property Reserved3 As UInteger
        Public Property Reserved4 As UInteger
        Public Property InfoSize As UInteger?
#End Region


#Region "Logical Properties"
        Public Property Level1HashBlockSize As ULong
            Get
                Return 1 << Level1BlockSizeLog2
            End Get
            Set(value As ULong)
                Level1BlockSizeLog2 = Math.Log(value, 2)
            End Set
        End Property

        Public Property Level2HashBlockSize As ULong
            Get
                Return 1 << Level2BlockSizeLog2
            End Get
            Set(value As ULong)
                Level2BlockSizeLog2 = Math.Log(value, 2)
            End Set
        End Property

        Public Property Level3HashBlockSize As ULong
            Get
                Return 1 << Level3BlockSizeLog2
            End Get
            Set(value As ULong)
                Level3BlockSizeLog2 = Math.Log(value, 2)
            End Set
        End Property

        Public ReadOnly Property BodyOffset As ULong
            Get

            End Get
        End Property

        Public ReadOnly Property BodySize As ULong
            Get
                Return Level3HashDataSize
            End Get
        End Property
#End Region


        Public Function GetBytes() As Byte()
            Dim out As New List(Of Byte)

            out.AddRange({&H49, &H56, &H46, &H43}) 'Todo: set magic string
            out.AddRange(BitConverter.GetBytes(MagicNum))
            out.AddRange(BitConverter.GetBytes(MasterHashSize))
            out.AddRange(BitConverter.GetBytes(Level1LogicalOffset))
            out.AddRange(BitConverter.GetBytes(Level1HashDataSize))
            out.AddRange(BitConverter.GetBytes(Level1BlockSizeLog2))
            out.AddRange(BitConverter.GetBytes(Reserved1))
            out.AddRange(BitConverter.GetBytes(Level2LogicalOffset))
            out.AddRange(BitConverter.GetBytes(Level2HashDataSize))
            out.AddRange(BitConverter.GetBytes(Level2BlockSizeLog2))
            out.AddRange(BitConverter.GetBytes(Reserved2))
            out.AddRange(BitConverter.GetBytes(Level3LogicalOffset))
            out.AddRange(BitConverter.GetBytes(Level3HashDataSize))
            out.AddRange(BitConverter.GetBytes(Level3BlockSizeLog2))
            out.AddRange(BitConverter.GetBytes(Reserved3))
            out.AddRange(BitConverter.GetBytes(Reserved4))

            If InfoSize.HasValue Then
                out.AddRange(BitConverter.GetBytes(InfoSize.Value))
            End If

            Return out.ToArray
        End Function

        Public Sub New(RawData As Byte())
            If RawData Is Nothing Then
                Throw New ArgumentNullException(NameOf(RawData))
            End If
            If RawData.Length < &H58 Then
                Throw New ArgumentException("RawData must be at least 0x58 bytes long.", NameOf(RawData))
            End If
            'Load properties
            MagicStr = Text.Encoding.ASCII.GetString(RawData, 0, 4)
            MagicNum = BitConverter.ToUInt32(RawData, &H4)
            MasterHashSize = BitConverter.ToUInt32(RawData, &H8)
            Level1LogicalOffset = BitConverter.ToUInt64(RawData, &HC) 'ULong
            Level1HashDataSize = BitConverter.ToUInt64(RawData, &H14) 'ULong
            Level1BlockSizeLog2 = BitConverter.ToUInt32(RawData, &H1C)
            Reserved1 = BitConverter.ToUInt32(RawData, &H20)
            Level2LogicalOffset = BitConverter.ToUInt64(RawData, &H24) 'ULong
            Level2HashDataSize = BitConverter.ToUInt64(RawData, &H2C) 'ULong
            Level2BlockSizeLog2 = BitConverter.ToUInt32(RawData, &H34)
            Reserved2 = BitConverter.ToUInt32(RawData, &H38)
            Level3LogicalOffset = BitConverter.ToUInt64(RawData, &H3C) 'ULong
            Level3HashDataSize = BitConverter.ToUInt64(RawData, &H44) 'ULong
            Level3BlockSizeLog2 = BitConverter.ToUInt32(RawData, &H4C)
            Reserved3 = BitConverter.ToUInt32(RawData, &H50)
            Reserved4 = BitConverter.ToUInt32(RawData, &H54)

            'Load info size, if supplied
            If RawData.Length >= &H5C Then
                InfoSize = BitConverter.ToUInt32(RawData, &H58)
            End If
        End Sub
    End Class

End Namespace
