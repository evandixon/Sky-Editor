Imports System.Drawing
Imports SkyEditorBase.Utilities

Module Module1

    Sub Main()
        'for all raw img's
        For Each item In IO.Directory.GetFiles("C:\TFS\Sky Editor\skyeditor\Sky Editor Base\bin\Debug\Resources\Plugins\ROMEditor\Current\data\BACK\Decompressed\Converted", "*.rawimg") '"C:\Users\Evan\Desktop\sky\data\RESCUE", "*.bin")
            Dim file As Byte() = IO.File.ReadAllBytes(item)
            Dim imageData As Byte() = GenericArrayOperations(Of Byte).CopyOfRange(file, &HC40, file.Length - 1)
            Dim mapData As Byte() = GenericArrayOperations(Of Byte).CopyOfRange(file, &H420, &HA1F + 1)
            Dim palData As Byte() = GenericArrayOperations(Of Byte).CopyOfRange(file, &H20, &H420)
            Dim chunks As New List(Of Byte())
            For count As Integer = 0 To imageData.Count - 1 Step 32
                'Dim i = (ChunkToImage(GenericArrayOperations(Of Byte).CopyOfRange(imageData, count, count + 31), palData))
                'chunks.Add(i)
                'i.Save("C:\Users\Evan\Desktop\sky\data\BACK\temp\" & count / 32 & ".png")
                If imageData.Count >= count + 31 Then
                    chunks.Add(GenericArrayOperations(Of Byte).CopyOfRange(imageData, count, count + 31))
                End If
            Next
            Dim outputBitmap As Bitmap = ProcessMapping(mapData, chunks, palData)
            outputBitmap.Save(item.Replace(".rawimg", ".png"))
        Next

        'for just one
        'Const filename As String = "s09p06a"
        'Dim file As Byte() = IO.File.ReadAllBytes("C:\Users\Evan\Desktop\sky\data\BACK\" & filename & ".rawimg")
        'Dim imageData As Byte() = GenericArrayOperations(Of Byte).CopyOfRange(file, &HC40, file.Length - 1)
        'Dim mapData As Byte() = GenericArrayOperations(Of Byte).CopyOfRange(file, &H420, &HA1F + 1)
        'Dim palData As Byte() = GenericArrayOperations(Of Byte).CopyOfRange(file, &H20, &H420)
        ''For count As Integer = 0 To 500 Step 2 '713 Step 2
        ''    Dim b8 = New SkyEditorBase.Bits8(mapData(count * 2 + 1))
        ''    Dim index As Integer = BitConverter.ToUInt16({mapData(count), (mapData(count + 1) Or &HFC) - &HFC}, 0)
        ''    Dim flipX As Boolean = If(b8.Bit3, True, False)
        ''    Dim flipY As Boolean = If(b8.Bit4, True, False)
        ''    Dim ExtraData As Integer = (mapData(count + 1) >> 4 Or &HF0) - &HF0
        ''    Console.WriteLine("{0} - Flip X: {1}, Flip Y: {2}, Extra Data: {3}", count, flipX, flipY, ExtraData)
        ''Next
        'Dim chunks As New List(Of Byte())
        'For count As Integer = 0 To imageData.Count - 1 Step 32
        '    'Dim i = (ChunkToImage(GenericArrayOperations(Of Byte).CopyOfRange(imageData, count, count + 31), palData))
        '    'chunks.Add(i)
        '    'i.Save("C:\Users\Evan\Desktop\sky\data\BACK\temp\" & count / 32 & ".png")
        '    chunks.Add(GenericArrayOperations(Of Byte).CopyOfRange(imageData, count, count + 31))
        'Next
        'Dim outputBitmap As Bitmap = ProcessMapping(mapData, chunks, palData)
        'outputBitmap.Save("C:\Users\Evan\Desktop\sky\data\BACK\" & filename & ".png")
        'Console.ReadLine()
    End Sub

    Function ProcessMapping(Data As Byte(), Chunks As List(Of Byte()), PalData As Byte()) As Bitmap
        Dim i As New System.Drawing.Bitmap(256, 192)
        Dim g As Graphics = Graphics.FromImage(i)
        Dim dataIndex As Integer = 0
        For y As Integer = 0 To 23
            For x As Integer = 0 To 31
                Dim index As Integer = BitConverter.ToUInt16({Data(dataIndex * 2), (Data(dataIndex * 2 + 1) Or &HFC) - &HFC}, 0) 'Data(dataIndex * 2)
                'If Chunks.Count >= index - 1 AndAlso index > 0 Then
                Dim b8 = New Bits8(Data(dataIndex * 2 + 1))
                Dim palette As Integer = ((Data(dataIndex * 2 + 1) >> 4 Or &HF0) - &HF0)
                If dataIndex > 300 And dataIndex < 400 Then
                    Console.WriteLine(dataIndex & " " & palette)
                End If
                If Chunks.Count > index Then
                    Dim icopy = ChunkToImage(Chunks(index - 1), PalData, palette) 'Chunks(index - 1).Clone
                    If b8.Bit3 Then
                        icopy.RotateFlip(RotateFlipType.RotateNoneFlipX)
                    End If
                    If b8.Bit4 Then
                        icopy.RotateFlip(RotateFlipType.RotateNoneFlipY)
                    End If
                    g.DrawImage(icopy, New Point(x * 8, y * 8))
                End If
                'Else
                ' Console.WriteLine(index)
                'End If
                dataIndex += 1
            Next
        Next
        Return i
    End Function

    Function ChunkToImage(Data As Byte(), PalData As Byte(), PalIndex As Byte) As Bitmap
        Dim i As New System.Drawing.Bitmap(8, 8)
        Dim g As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(i)
        Dim colors As New List(Of Byte)
        For Each b In Data
            colors.Add(((b) Or &HF0) - &HF0)
            colors.Add(((b >> 4) Or &HF0) - &HF0)
        Next
        Dim colorIndex = 0
        For y As Byte = 0 To 7
            For x As Byte = 0 To 7
                g.FillRectangle(Pen(colors(colorIndex), PalData, PalIndex), x, y, 1, 1)
                colorIndex += 1
            Next
        Next
        g.Save()
        Return i
    End Function

    Function Pen(Color As Byte, palData As Byte(), PalNumber As Byte) As Brush
        Dim r = palData(PalNumber * 64 + Color * 4 + 0)
        Dim g = palData(PalNumber * 64 + Color * 4 + 1)
        Dim b = palData(PalNumber * 64 + Color * 4 + 2)
        'Dim out As New SolidBrush(System.Drawing.Color.FromArgb(255, Color * 16, Color * 16, Color * 16))
        Dim out As New SolidBrush(System.Drawing.Color.FromArgb(255, r, g, b))
        Return out
    End Function

    Public Class GenericArrayOperations(Of T)
        Public Shared Function CopyOfRange(ByteArr As T(), Index As Integer, EndPoint As Integer) As T()
            Dim output(Math.Max(Math.Min(EndPoint, ByteArr.Length) - Index, 0)) As T
            For x As Integer = 0 To output.Length - 1
                output(x) = ByteArr(x + Index)
            Next
            Return output
        End Function
    End Class

End Module
