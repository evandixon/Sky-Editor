Imports System.Drawing
Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.IO

Namespace FileFormats.PSMD
    Public Class CteImage
        Inherits GenericFile
        Implements IOpenableFile
        Implements INotifyModified
        Implements IDetectableFileType

        Public Event ContainedImageUpdated(sender As Object, e As EventArgs)
        Public Event Modified As INotifyModified.ModifiedEventHandler Implements INotifyModified.Modified

        Public Property ContainedImage As Bitmap
            Get
                Return _containedImage
            End Get
            Set(value As Bitmap)
                _containedImage = value
                RaiseModified()
                RaiseEvent ContainedImageUpdated(Me, New EventArgs)
            End Set
        End Property
        Dim _containedImage As Bitmap

        Private Property ImageFormat As Integer

        ''' <summary>
        ''' Gets the length in bits of each pixel.
        ''' </summary>
        ''' <returns></returns>
        Private Property PixelLength As Integer

        Private Property Width As Integer

        Private Property Height As Integer

        Public Overrides Async Function OpenFile(Filename As String, Provider As IOProvider) As Task Implements IOpenableFile.OpenFile
            Await MyBase.OpenFile(Filename, Provider)
            ImageFormat = BitConverter.ToInt32(RawData(&H4, 4), 0)
            Width = BitConverter.ToInt32(RawData(&H8, 4), 0)
            Height = BitConverter.ToInt32(RawData(&HC, 4), 0)
            PixelLength = BitConverter.ToInt32(RawData(&H10, 4), 0)
            Dim dataStart = BitConverter.ToInt32(RawData(&H18, 4), 0)
            Dim dataLength = Width * Height * PixelLength

            Dim image As New Bitmap(Width, Height)
            Dim g = Graphics.FromImage(image)
            Dim pixelIndex = 0
            For y As Integer = (Height / 8) - 1 To 0 Step -1
                For x As Integer = 0 To (Width / 8) - 1
                    g.DrawImage(DrawTile(pixelIndex, PixelLength, dataStart, 8), New Point(x * 8, y * 8))
                    pixelIndex += 64
                Next
            Next

            g.Save()
            ' image.RotateFlip(RotateFlipType.Rotate270FlipNone)

            ContainedImage = image
        End Function
        Private Function GetColor(PixelIndex As Integer, PixelLength As Integer, DataStart As Integer) As Color
            If PixelLength = &H20 Then '32 bit
                Dim data = RawData(PixelIndex * (PixelLength / 8) + DataStart, 4)
                Return Color.FromArgb(data(0), data(3), data(2), data(1))
            ElseIf PixelLength = &H18 Then
                Dim data = RawData(PixelIndex * (PixelLength / 8) + DataStart, 3)
                Return Color.FromArgb(255, data(2), data(1), data(0))
            ElseIf PixelLength = 8 Then
                Dim data = RawData(PixelIndex * (PixelLength / 8) + DataStart)
                Dim a = ((data And &HF0) >> 4) * 17
                Dim c = ((data And &HF)) * 17
                Return Color.FromArgb(a, c, c, c)
            Else
                Throw New NotImplementedException("Unsupported pixel length: " & PixelLength)
            End If
        End Function
        Private Function DrawTile(PixelIndex As Integer, PixelLength As Integer, DataStart As Integer, TileSize As Integer) As Bitmap
            If TileSize = 2 Then
                Dim output As New Bitmap(2, 2)
                output.SetPixel(0, 1, GetColor(PixelIndex + 0, PixelLength, DataStart))
                output.SetPixel(1, 1, GetColor(PixelIndex + 1, PixelLength, DataStart))
                output.SetPixel(0, 0, GetColor(PixelIndex + 2, PixelLength, DataStart))
                output.SetPixel(1, 0, GetColor(PixelIndex + 3, PixelLength, DataStart))
                Return output
            Else
                Dim output As New Bitmap(TileSize, TileSize)
                Dim g = Graphics.FromImage(output)
                Dim half As Integer = TileSize / 2
                Dim childPixelCount As Integer = (TileSize / 2) ^ 2
                g.DrawImage(DrawTile(PixelIndex + childPixelCount * 0, PixelLength, DataStart, TileSize / 2), New Point(0, half))
                g.DrawImage(DrawTile(PixelIndex + childPixelCount * 1, PixelLength, DataStart, TileSize / 2), New Point(half, half))
                g.DrawImage(DrawTile(PixelIndex + childPixelCount * 2, PixelLength, DataStart, TileSize / 2), New Point(0, 0))
                g.DrawImage(DrawTile(PixelIndex + childPixelCount * 3, PixelLength, DataStart, TileSize / 2), New Point(half, 0))
                g.Save()
                Return output
            End If
        End Function

        Public Overrides Sub Save(Destination As String, provider As IOProvider)
            Dim empty64(64) As Byte
            Dim dataLength = Width * Height * PixelLength
            Length = dataLength + &H80
            RawData(0) = 0
            RawData(1) = &H63
            RawData(2) = &H74
            RawData(3) = &H65
            RawData(4, 4) = BitConverter.GetBytes(ImageFormat)
            RawData(8, 4) = BitConverter.GetBytes(Width)
            RawData(&HC, 4) = BitConverter.GetBytes(Height)
            RawData(&H10, 4) = BitConverter.GetBytes(PixelLength)
            RawData(&H14, 4) = BitConverter.GetBytes(0)
            RawData(&H18, 4) = BitConverter.GetBytes(&H80)
            RawData(&H1C, 64) = empty64
            'Todo: write pixel data
            Dim offset = &H80
            For y As Integer = (Height / 8) - 1 To 0 Step -1
                For x As Integer = 0 To (Width / 8) - 1
                    RawData(offset, (PixelLength * 64) / 8) = GetTileData(x * 8, y * 8, 8, PixelLength).ToArray
                    offset += (PixelLength * 64) / 8
                Next
            Next
            MyBase.Save(Destination, provider)
        End Sub

        Private Function GetColorData(Color As Color, PixelLength As Integer) As Byte()
            If PixelLength = &H20 Then
                Return {Color.A, Color.B, Color.G, Color.R}
            ElseIf PixelLength = &H18 Then
                Return {Color.B, Color.G, Color.R}
            ElseIf PixelLength = &H8 Then
                'Return {255 - ((Color.B + Color.G + Color.R) / 3)}
                Throw New NotImplementedException("Unsupported pixel length: " & PixelLength)
            Else
                Throw New NotImplementedException("Unsupported pixel length: " & PixelLength)
            End If
        End Function

        Private Function GetTileData(OffsetX As Integer, OffsetY As Integer, TileSize As Integer, PixelLength As Integer) As IEnumerable(Of Byte)
            If TileSize = 2 Then
                Dim out As New List(Of Byte)
                out.AddRange(GetColorData(ContainedImage.GetPixel(OffsetX + 0, OffsetY + 1), PixelLength))
                out.AddRange(GetColorData(ContainedImage.GetPixel(OffsetX + 1, OffsetY + 1), PixelLength))
                out.AddRange(GetColorData(ContainedImage.GetPixel(OffsetX + 0, OffsetY + 0), PixelLength))
                out.AddRange(GetColorData(ContainedImage.GetPixel(OffsetX + 1, OffsetY + 0), PixelLength))
                Return out
            Else
                Dim out As New List(Of Byte)
                Dim half = TileSize / 2
                out.AddRange(GetTileData(OffsetX + 0, OffsetY + half, half, PixelLength))
                out.AddRange(GetTileData(OffsetX + half, OffsetY + half, half, PixelLength))
                out.AddRange(GetTileData(OffsetX + 0, OffsetY + 0, half, PixelLength))
                out.AddRange(GetTileData(OffsetX + half, OffsetY + 0, half, PixelLength))
                Return out
            End If
        End Function

        Public Sub RaiseModified() Implements INotifyModified.RaiseModified
            RaiseEvent Modified(Me, New EventArgs)
        End Sub

        Public Function IsOfType(File As GenericFile) As Task(Of Boolean) Implements IDetectableFileType.IsOfType
            Return Task.FromResult(File.RawData(0) = 0 AndAlso File.RawData(1) = &H63 AndAlso File.RawData(2) = &H74 AndAlso File.RawData(3) = &H65)
        End Function
    End Class
End Namespace