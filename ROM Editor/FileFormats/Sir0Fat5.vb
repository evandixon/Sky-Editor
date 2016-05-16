Imports SkyEditor.Core.IO

Namespace FileFormats
    Public Class Sir0Fat5
        Inherits Sir0
        Implements IOpenableFile

        Public Class FileInfo
            Public Property Index As Integer
            Public Property DataOffset As Integer
            Public Property DataLength As Integer
            Public Property Filename As String
            Public Property FilenamePointer As UInteger
            Public Overrides Function ToString() As String
                If Filename IsNot Nothing Then
                    Return Filename
                Else
                    Return MyBase.ToString
                End If
            End Function
        End Class

        Protected Property FileCount As Integer
        Protected Property DataOffset As Integer
        Protected Property Sir0Fat5Type As Integer
        Public Property FileData As List(Of FileInfo)
        Public Overrides Sub CreateFile(Name As String, FileContents() As Byte)
            MyBase.CreateFile(Name, FileContents)

            If FileContents.Length > 0 Then
                ProcessData()
            End If
        End Sub

        Public Overrides Async Function OpenFile(Filename As String, Provider As IOProvider) As Task Implements IOpenableFile.OpenFile
            Await MyBase.OpenFile(Filename, Provider)

            ProcessData()
        End Function
        Private Sub ProcessData()
            FileData = New List(Of FileInfo)
            DataOffset = BitConverter.ToInt32(Header, 0)
            FileCount = BitConverter.ToInt32(Header, 4)
            Sir0Fat5Type = BitConverter.ToInt32(Header, 8)

            For count = 0 To FileCount - 1
                Dim info As New FileInfo
                info.Index = count
                Dim filenameOffset = Me.UInt32(DataOffset + count * 12 + 0)
                info.DataOffset = Me.Int32(DataOffset + count * 12 + 4)
                info.DataLength = Me.Int32(DataOffset + count * 12 + 8)
                info.FilenamePointer = filenameOffset
                If Sir0Fat5Type = 0 Then
                    'We're inferring the length based on the offset of the next filename
                    Dim filenameLength = Me.Int32(DataOffset + (count + 1) * 12 + 0) - filenameOffset
                    info.Filename = Me.ReadUnicodeString(filenameOffset, filenameLength / 2)
                Else
                    info.Filename = Hex(filenameOffset).PadLeft(8, "0"c)
                End If
                FileData.Add(info)
            Next
        End Sub

        Public Overrides Sub Save(Destination As String, provider As IOProvider)
            'Only works for files without filenames

            'Reset pointers
            Me.RelativePointers.Clear()
            Me.RelativePointers.Add(4)
            Me.RelativePointers.Add(4)

            'Generate data
            Dim data As New List(Of Byte)
            For Each item In FileData
                data.AddRange(BitConverter.GetBytes(item.FilenamePointer))
                data.AddRange(BitConverter.GetBytes(item.DataOffset))
                data.AddRange(BitConverter.GetBytes(item.DataLength))
            Next

            'Write data
            Me.Length = 16 + data.Count
            Me.RawData(16, data.Count) = data.ToArray

            'Generate header, and let the base class write it
            Dim headerData As New List(Of Byte)
            headerData.AddRange(BitConverter.GetBytes(&H10)) 'Data offset
            headerData.AddRange(BitConverter.GetBytes(FileData.Count))
            headerData.AddRange(BitConverter.GetBytes(1)) 'Marks that we're not actually using filenames

            Me.Header = headerData.ToArray


            Me.RelativePointers.Add(data.Count + 8)
            MyBase.Save(Destination, provider)
        End Sub


        Public Sub New()
            FileData = New List(Of FileInfo)
        End Sub

    End Class
End Namespace

