Imports System.Text
Imports ROMEditor.Roms
Imports SkyEditorBase

Namespace FileFormats
    Public Class LanguageString
        Inherits GenericFile
        Public Property Items As List(Of String)


        'Public Sub New(Filename As String)
        '    MyBase.New(Filename)
        'End Sub

        Public Sub New(Filename As String)
            MyBase.New(Filename)
            Dim bytes = IO.File.ReadAllBytes(Filename)

            Items = New List(Of String)

            Dim offset1 As UInt32 = BitConverter.ToUInt32(bytes, 0)
            Dim e = Encoding.GetEncoding("Windows-1252")
            'Loop through each entry
            For count As Integer = 0 To offset1 - 5 Step 4
                Dim startOffset As UInteger = BitConverter.ToUInt32(bytes, count)
                Items.Add("")
                Dim endOffset As UInteger = startOffset
                Dim s As New StringBuilder
                'Read the null-terminated string
                While bytes(endOffset) <> 0
                    s.Append(e.GetString({RawData(endOffset)}))
                    endOffset += 1
                End While
                Items(count / 4) = s.ToString
            Next
        End Sub

        Public Overrides Sub PreSave()
            MyBase.PreSave()
            'Generate File
            Dim e = Encoding.GetEncoding("Windows-1252")
            Dim offsets As New List(Of UInt32)
            For i As UInt32 = 0 To Items.Count - 1
                offsets.Add(0)
            Next
            Dim stringdataBytes As New List(Of Byte)
            For count As Integer = 0 To Items.Count - 1
                Dim offset As UInt32 = offsets.Count * 4 + stringdataBytes.Count
                offsets(count) = offset
                Dim strBytes = e.GetBytes(Item(count).Replace(vbCrLf, vbCr))
                For Each s In strBytes
                    stringdataBytes.Add(s)
                Next
                stringdataBytes.Add(0)
            Next
            Dim offsetBytes As New List(Of Byte)
            For Each offset In offsets
                Dim t = BitConverter.GetBytes(offset)
                offsetBytes.Add(t(0))
                offsetBytes.Add(t(1))
                offsetBytes.Add(t(2))
                offsetBytes.Add(t(3))
            Next

            Dim totalData As New List(Of Byte)
            For Each b In offsetBytes
                totalData.Add(b)
            Next
            For Each b In stringdataBytes
                totalData.Add(b)
            Next
            'Write buffer to stream
            Length = totalData.Count
            RawData(0, totalData.Count) = totalData.ToArray
        End Sub
        Default Public Property Item(Index As UInteger) As String
            Get
                Return Items(Index)
            End Get
            Set(value As String)
                Items(Index) = value
            End Set
        End Property
        Public Const ItemNameStartUS As Integer = 6773

        'Default Public Property Item(Index As UInteger) As String
        '    Get
        '        Dim startOffset As UInteger = BitConverter.ToUInt32(RawData, Index * 4)
        '        Dim endOffset As UInteger = BitConverter.ToUInt32(RawData, (Index + 1) * 4) - 2

        '        Return ""
        '    End Get
        '    Set(value As String)
        '        Dim startOffset As UInteger = BitConverter.ToUInt32(RawData, Index * 4)
        '        Dim endOffset As UInteger = BitConverter.ToUInt32(RawData, (Index + 1) * 4) - 2
        '        Dim e As New Text.UTF7Encoding
        '        Dim valueBytes = e.GetBytes(value)
        '        For count As Integer = 0 To Math.Min(valueBytes.Length - 1, endOffset - startOffset - 1)
        '            RawData(startOffset + count) = valueBytes(count)
        '        Next
        '    End Set
        'End Property

    End Class
End Namespace