Imports System.Text
Imports SkyEditorBase

Namespace FileFormats
    Public Class LanguageString
        Inherits SkyEditorBase.GenericFile
        Public Property Items As List(Of String)
        Public Sub New(Rom As GenericNDSRom)
            MyBase.New(PluginHelper.GetResourceName(Rom.Name & "\data\message\text_e.str"))
            Items = New List(Of String)
            RawData = IO.File.ReadAllBytes(Filename)
            Dim offset1 As UInt32 = BitConverter.ToUInt32(RawData, 0)
            Dim e = Encoding.GetEncoding("Windows-1252") '"shift_jis")
            For count As Integer = 0 To offset1 - 5 Step 4
                Dim startOffset As UInteger = BitConverter.ToUInt32(RawData, count)
                Items.Add("")
                Dim endOffset As UInteger = startOffset
                While RawData(endOffset) <> 0
                    Items(count / 4) &= e.GetString({RawData(endOffset)})
                    endOffset += 1
                End While
            Next
        End Sub

        Public Sub Save()
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
            IO.File.WriteAllBytes(Filename, totalData.ToArray)
        End Sub
        Default Public Property Item(Index As UInteger) As String
            Get
                Return Items(Index)
            End Get
            Set(value As String)
                Items(Index) = value
            End Set
        End Property

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