Namespace FileFormats.CTR
    Public Class Level3Header
        Public Property HeaderLength As UInteger
        Public Property DirectoryHashKeyTableOffset As UInteger
        Public Property DirectoryHashKeyTableLength As UInteger
        Public Property DirectoryMetadataTableOffset As UInteger
        Public Property DirectoryMetadataTableLength As UInteger
        Public Property FileHashKeyTableOffset As UInteger
        Public Property FileHashKeyTableLength As UInteger
        Public Property FileMetadataTableOffset As UInteger
        Public Property FileMetadataTableLength As UInteger
        Public Property FileDataOffset As UInteger

        Public Function GetBytes() As Byte()
            Dim out As New List(Of Byte)

            out.AddRange(BitConverter.GetBytes(HeaderLength))
            out.AddRange(BitConverter.GetBytes(DirectoryHashKeyTableOffset))
            out.AddRange(BitConverter.GetBytes(DirectoryHashKeyTableLength))
            out.AddRange(BitConverter.GetBytes(DirectoryMetadataTableOffset))
            out.AddRange(BitConverter.GetBytes(DirectoryMetadataTableLength))
            out.AddRange(BitConverter.GetBytes(FileHashKeyTableOffset))
            out.AddRange(BitConverter.GetBytes(FileHashKeyTableLength))
            out.AddRange(BitConverter.GetBytes(FileMetadataTableOffset))
            out.AddRange(BitConverter.GetBytes(FileMetadataTableLength))
            out.AddRange(BitConverter.GetBytes(FileDataOffset))

            Return out.ToArray
        End Function

        ''' <summary>
        ''' Creates a new instance of Level3Header
        ''' </summary>
        ''' <param name="RawData">The raw data representing the header.</param>
        Public Sub New(RawData As Byte())
            If RawData Is Nothing Then
                Throw New ArgumentNullException(NameOf(RawData))
            End If
            If RawData.Length < &H28 Then
                Throw New ArgumentException("RawData must be at least 0x28 bytes long.", NameOf(RawData))
            End If

            HeaderLength = BitConverter.ToUInt32(RawData, &H0)
            DirectoryHashKeyTableOffset = BitConverter.ToUInt32(RawData, &H4)
            DirectoryHashKeyTableLength = BitConverter.ToUInt32(RawData, &H8)
            DirectoryMetadataTableOffset = BitConverter.ToUInt32(RawData, &HC)
            DirectoryMetadataTableLength = BitConverter.ToUInt32(RawData, &H10)
            FileHashKeyTableOffset = BitConverter.ToUInt32(RawData, &H14)
            FileHashKeyTableLength = BitConverter.ToUInt32(RawData, &H18)
            FileMetadataTableOffset = BitConverter.ToUInt32(RawData, &H1C)
            FileMetadataTableLength = BitConverter.ToUInt32(RawData, &H20)
            FileDataOffset = BitConverter.ToUInt32(RawData, &H24)
        End Sub
    End Class
End Namespace

