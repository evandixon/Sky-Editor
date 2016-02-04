Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Namespace FileFormats
    Public Class Sir0Fat5
        Inherits Sir0
        Implements iOpenableFile

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

        Public Overrides Sub OpenFile(Filename As String) Implements iOpenableFile.OpenFile
            MyBase.OpenFile(Filename)

            ProcessData()
        End Sub
        Private Sub ProcessData()
            FileData = New List(Of FileInfo)
            DataOffset = BitConverter.ToInt32(Header, 0)
            FileCount = BitConverter.ToInt32(Header, 4)
            Sir0Fat5Type = BitConverter.ToInt32(Header, 8)

            For count = 0 To FileCount - 1
                Dim info As New FileInfo
                info.Index = count
                Dim filenameOffset = Me.UInt(DataOffset + count * 12 + 0)
                info.DataOffset = Me.Int(DataOffset + count * 12 + 4)
                info.DataLength = Me.Int(DataOffset + count * 12 + 8)
                info.FilenamePointer = filenameOffset
                If Sir0Fat5Type = 0 Then
                    'We're inferring the length based on the offset of the next filename
                    Dim filenameLength = Me.Int(DataOffset + (count + 1) * 12 + 0) - filenameOffset
                    info.Filename = Me.ReadUnicodeString(filenameOffset, filenameLength / 2)
                Else
                    info.Filename = Hex(filenameOffset).PadLeft(8, "0"c)
                End If
                FileData.Add(info)
            Next
        End Sub

        Public Function GetBytes() As Byte()
            Dim output As New List(Of Byte)


            Throw New NotImplementedException

        End Function

        Public Sub New()
            FileData = New List(Of FileInfo)
        End Sub

        Public Sub New(RawData As Byte())
            MyBase.New(RawData)
            ProcessData()
        End Sub

    End Class
End Namespace

