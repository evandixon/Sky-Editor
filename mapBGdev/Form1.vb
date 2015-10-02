Public Class Form1
    Dim images As New List(Of Bitmap)
    Dim bpc As SkyEditorBase.GenericFile
    Dim bpl As SkyEditorBase.GenericFile
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        bpc = New SkyEditorBase.GenericFile("C:\Users\Evan\Desktop\Sky Projects\Sky\BaseRom RawFiles\data\MAP_BG\d00p01.bpc")
        bpl = New SkyEditorBase.GenericFile("C:\Users\Evan\Desktop\Sky Projects\Sky\BaseRom RawFiles\data\MAP_BG\d00p01.bpl")
        NumericUpDown1_ValueChanged(sender, e)
    End Sub

    Private Function MakeLargeTile(TileXCount As Integer, TileYCount As Integer, Tiles As List(Of Byte())) As Bitmap
        Dim i As New Bitmap(TileXCount * 8, TileYCount * 8)
        Dim g = Graphics.FromImage(i)
        For y = 0 To TileYCount - 1
            For x = 0 To TileXCount - 1
                g.DrawImage(ChunkToImage(Tiles(y * (TileXCount) + x)), New Rectangle(8 * x, 8 * y, 8, 8))
            Next
        Next
        g.Save()
        Return i
    End Function

    Private Function ChunkToImage(Data As Byte()) As Bitmap
        Dim i As New System.Drawing.Bitmap(8, 8)
        Dim colors As New List(Of Byte)
        For Each b In Data
            colors.Add(((b) Or &HF0) - &HF0)
            colors.Add(((b >> 4) Or &HF0) - &HF0)
        Next
        Dim colorIndex = 0
        For y As Byte = 0 To 7
            For x As Byte = 0 To 7
                Dim c As Color = Color.FromArgb(bpl.RawData(colors(colorIndex) * 4), bpl.RawData(colors(colorIndex) * 4 + 1), bpl.RawData(colors(colorIndex) * 4 + 2))
                i.SetPixel(x, y, c)

                colorIndex += 1
            Next
        Next
        Return i
    End Function

    Private Sub NumericUpDown1_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown1.ValueChanged
        Dim offset = 0
        Dim source = MakeLargeTile(2, 2, New List(Of Byte())({bpc.RawData(offset + 32 * (NumericUpDown1.Value + 0), 32),
                                                              bpc.RawData(offset + 32 * (NumericUpDown1.Value + 1), 32),
                                                              bpc.RawData(offset + 32 * (NumericUpDown1.Value + 2), 32),
                                                              bpc.RawData(offset + 32 * (NumericUpDown1.Value + 3), 32),
                                                              bpc.RawData(offset + 32 * (NumericUpDown1.Value + 4), 32),
                                                              bpc.RawData(offset + 32 * (NumericUpDown1.Value + 5), 32),
                                                              bpc.RawData(offset + 32 * (NumericUpDown1.Value + 6), 32),
                                                              bpc.RawData(offset + 32 * (NumericUpDown1.Value + 7), 32),
                                                              bpc.RawData(offset + 32 * (NumericUpDown1.Value + 8), 32),
                                                              bpc.RawData(offset + 32 * (NumericUpDown1.Value + 9), 32),
                                                              bpc.RawData(offset + 32 * (NumericUpDown1.Value + 10), 32),
                                                              bpc.RawData(offset + 32 * (NumericUpDown1.Value + 11), 32),
                                                              bpc.RawData(offset + 32 * (NumericUpDown1.Value + 12), 32),
                                                              bpc.RawData(offset + 32 * (NumericUpDown1.Value + 13), 32),
                                                              bpc.RawData(offset + 32 * (NumericUpDown1.Value + 14), 32),
                                                              bpc.RawData(offset + 32 * (NumericUpDown1.Value + 15), 32),
                                                              bpc.RawData(offset + 32 * (NumericUpDown1.Value + 16), 32)}))
        Dim i As New Bitmap(256, 256)
        Dim g = Graphics.FromImage(i)
        g.InterpolationMode = Drawing2D.InterpolationMode.NearestNeighbor
        g.DrawImage(source, New Rectangle(0, 0, 256, 256))
        PictureBox1.Image = i
    End Sub
End Class
