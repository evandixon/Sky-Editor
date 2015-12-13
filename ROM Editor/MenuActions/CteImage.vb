Imports System.Drawing
Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Namespace FileFormats
    Public Class CteImage
        Inherits GenericFile
        Implements iOpenableFile

        Public Property ContainedImage As Bitmap

        Private Sub iOpenableFile_OpenFile(Filename As String) Implements iOpenableFile.OpenFile
            MyBase.OpenFile(Filename)
            Dim width = BitConverter.ToInt32(RawData(&H8, 4), 0)
            Dim height = BitConverter.ToInt32(RawData(&HC, 4), 0)
            Dim pixelLength = BitConverter.ToInt32(RawData(&H10, 4), 0)
            Dim dataStart = BitConverter.ToInt32(RawData(&H18, 4), 0)
            Dim dataLength = width * height * pixelLength

            Dim image As New Bitmap(height, width)
            Dim g = Graphics.FromImage(image)
            Dim pixelIndex = 0
            For y As Integer = 0 To (height / 8) - 1 ' To 0 Step -1
                For x As Integer = 0 To (width / 8) - 1
                    g.DrawImage(DrawTile(pixelIndex, pixelLength, dataStart, 8), New Point(y * 8, x * 8))
                    pixelIndex += 64
                Next
            Next

            g.Save()
            image.RotateFlip(RotateFlipType.Rotate270FlipNone)

            'For x As Integer = 0 To width - 1
            '    For y As Integer = height - 1 To 0 Step -1
            '        image.SetPixel(x, y, GetColor(pixelIndex, pixelLength, dataStart))
            '        pixelIndex += 1
            '    Next
            'Next
            ContainedImage = image

            'debug
            image.Save(OriginalFilename & ".png", Imaging.ImageFormat.Png)
        End Sub
        Public Function GetColor(PixelIndex As Integer, PixelLength As Integer, DataStart As Integer) As Color
            If PixelLength = &H20 Then '32 bit
                Dim data = RawData(PixelIndex * (PixelLength / 8) + DataStart, 4)
                Return Color.FromArgb(data(0), data(1), data(2), data(3))
            ElseIf PixelLength = &H18
                Dim data = RawData(PixelIndex * (PixelLength / 8) + DataStart, 3)
                Return Color.FromArgb(255, data(0), data(1), data(2))
            End If
        End Function
        Public Function DrawTile(PixelIndex As Integer, PixelLength As Integer, DataStart As Integer, TileSize As Integer) As Bitmap
            If TileSize = 2 Then
                Dim output As New Bitmap(2, 2)
                output.SetPixel(0, 0, GetColor(PixelIndex + 0, PixelLength, DataStart))
                output.SetPixel(0, 1, GetColor(PixelIndex + 1, PixelLength, DataStart))
                output.SetPixel(1, 0, GetColor(PixelIndex + 2, PixelLength, DataStart))
                output.SetPixel(1, 1, GetColor(PixelIndex + 3, PixelLength, DataStart))
                Return output
            Else
                Dim output As New Bitmap(TileSize, TileSize)
                Dim g = Graphics.FromImage(output)
                Dim half As Integer = TileSize / 2
                Dim childPixelCount As Integer = (TileSize / 2) ^ 2
                g.DrawImage(DrawTile(PixelIndex + childPixelCount * 0, PixelLength, DataStart, TileSize / 2), New Point(0, 0))
                g.DrawImage(DrawTile(PixelIndex + childPixelCount * 1, PixelLength, DataStart, TileSize / 2), New Point(0, half))
                g.DrawImage(DrawTile(PixelIndex + childPixelCount * 2, PixelLength, DataStart, TileSize / 2), New Point(half, 0))
                g.DrawImage(DrawTile(PixelIndex + childPixelCount * 3, PixelLength, DataStart, TileSize / 2), New Point(half, half))
                g.Save()
                Return output
            End If
        End Function

        Public Sub New()
            MyBase.New
        End Sub
    End Class
End Namespace