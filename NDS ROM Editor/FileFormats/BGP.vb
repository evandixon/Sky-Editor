Imports System.Drawing
Imports SkyEditorBase
Imports SkyEditorBase.Utilities

Namespace FileFormats
    Public Class BGP
        Inherits DecomressedFile
        Private Class Palette
            Public Property Colors As Generic.List(Of Color)
            Public Function ToBytes() As Byte()
                Dim paletteData(&H400 - 1) As Byte
                For count As Integer = 0 To paletteData.Length - 1 Step 4
                    Dim colorNum As Integer = count / 4
                    If Colors(colorNum) = Nothing Then
                        Colors(colorNum) = Color.Black
                    End If
                    paletteData(count + 0) = Colors(colorNum).R
                    paletteData(count + 1) = Colors(colorNum).G
                    paletteData(count + 2) = Colors(colorNum).B
                    paletteData(count + 3) = &H80
                Next
                Return paletteData
            End Function
            Public Function GetPalateOffset(LocalPalette As Generic.List(Of Color)) As Byte
                Dim out As Integer = -1
                For i As Integer = 0 To 15
                    Dim part = Colors.GetRange(i * 16, 16)
                    Dim isMatch As Boolean = True
                    Dim missing As New List(Of Color)
                    Dim nullOffsets As New List(Of Integer)
                    Dim numMatching As Integer
                    For count As Integer = 0 To part.Count - 1
                        If part(count) = Nothing Then
                            nullOffsets.Add(count)
                        End If
                    Next
                    For Each Color In LocalPalette
                        If Not part.Contains(Color) Then
                            isMatch = False
                            missing.Add(Color)
                        Else
                            numMatching += 1
                        End If
                    Next
                    If isMatch Then
                        out = i
                        'ElseIf missing.Count > numMatching OrElse i = 0 Then
                        'skip
                    ElseIf nullOffsets.Count >= missing.Count Then
                        For count As Integer = 0 To missing.Count - 1
                            part(nullOffsets(count)) = missing(count)
                        Next
                        For count As Integer = 0 To part.Count - 1
                            Colors(i * 16 + count) = part(count)
                        Next
                        out = i
                    Else
                        'go to the next palette offset and try again
                    End If
                    If out > -1 Then Exit For
                Next
                'If out = -1 Then out = 0
                If out = -1 Then
                    Throw New BadImageFormatException("There was not enough space in the palette for all the colors.")
                End If
                'double check this is the right palette
                'Dim isMatch2 As Boolean = True
                'Dim part2 = Colors.GetRange(out * 16, 16)
                'For Each Color In LocalPalette
                '    If Not part2.Contains(Color) Then
                '        isMatch2 = False
                '        Exit For
                '    End If
                'Next
                'If Not isMatch2 Then
                '    Throw New Exception
                'End If
                Return out
            End Function
            Public Function GetLocalPalette(PaletteIndex As Byte) As Palette
                Dim part = Colors.GetRange(PaletteIndex * 16, 16)
                Dim out As New Palette
                out.Colors = part
                Return out
            End Function
            Public Sub New()
                Colors = New List(Of Color)
                For count As Integer = 0 To 255
                    Colors.Add(Nothing)
                Next
            End Sub
        End Class
        Private Class NumberOfItemsOrderer
            Implements IComparer(Of List(Of Color))
            Public Function Compare(x As List(Of Color), y As List(Of Color)) As Integer Implements IComparer(Of List(Of Color)).Compare
                Return y.Count - x.Count
            End Function
        End Class

#Region "Private Image Processing"
        Private ReadOnly Property ImageData As Byte()
            Get
                Return RawData(&HC40, Length - &HC40)
            End Get
        End Property
        Private ReadOnly Property MapData As Byte()
            Get
                Return RawData(&H420, &H600)
            End Get
        End Property
        Private ReadOnly Property PaletteData As Byte()
            Get
                Return RawData(&H20, &H400)
            End Get
        End Property
        Private ReadOnly Property ChunkData As List(Of Byte())
            Get
                Dim chunks As New List(Of Byte())
                For count As Integer = 0 To ImageData.Count - 1 Step 32
                    chunks.Add(GenericArrayOperations(Of Byte).CopyOfRange(ImageData, count, count + 31))
                Next
                Return chunks
            End Get
        End Property
        Private Function ChunkToImage(Data As Byte(), PalData As Byte(), PalIndex As Byte) As Bitmap
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
                    g.FillRectangle(GetBrush(colors(colorIndex), PalData, PalIndex), x, y, 1, 1)
                    colorIndex += 1
                Next
            Next
            g.Save()
            Return i
        End Function

        Private Function GetBrush(Color As Byte, palData As Byte(), PalNumber As Byte) As Brush
            Dim r = palData(PalNumber * 64 + Color * 4 + 0)
            Dim g = palData(PalNumber * 64 + Color * 4 + 1)
            Dim b = palData(PalNumber * 64 + Color * 4 + 2)
            Dim out As New SolidBrush(System.Drawing.Color.FromArgb(255, r, g, b))
            Return out
        End Function
        Private Function ProcessMapping(MapData As Byte(), Chunks As List(Of Byte()), PalData As Byte()) As Bitmap
            Dim i As New System.Drawing.Bitmap(256, 192)
            Dim g As Graphics = Graphics.FromImage(i)
            Dim dataIndex = 0
            For y = 0 To 23
                For x = 0 To 31
                    Dim index As Integer = BitConverter.ToUInt16({MapData(dataIndex * 2), (MapData(dataIndex * 2 + 1) Or &HFC) - &HFC}, 0) 'Data(dataIndex * 2)
                    If Chunks.Count >= index - 1 AndAlso index > 0 Then
                        Dim b8 = New Bits8(MapData(dataIndex * 2 + 1))
                        Dim palette As Integer = ((MapData(dataIndex * 2 + 1) >> 4 Or &HF0) - &HF0)
                        Dim icopy = ChunkToImage(Chunks(index - 1), PalData, palette) 'Chunks(index - 1).Clone
                        If b8.Bit3 Then
                            icopy.RotateFlip(RotateFlipType.RotateNoneFlipX)
                        End If
                        If b8.Bit4 Then
                            icopy.RotateFlip(RotateFlipType.RotateNoneFlipY)
                        End If
                        g.DrawImage(icopy, New Point(x * 8, y * 8))
                    Else
                        Console.WriteLine(index)
                    End If
                    dataIndex += 1
                Next
            Next
            Return i
        End Function
#End Region

        ''' <summary>
        ''' Creates a new BGP DecompressedFile when given a System.Drawing.Bitmap object.
        '''
        ''' Each 8x8 tile must use 1 of 16 palettes, each with 16 colors.
        ''' Will raise a BadImageFormatException if there is not enough room in the palette for all the colors.
        ''' </summary>
        ''' <param name="Image">Image to be converted to a BGP.</param>
        ''' <param name="CheckForTransform">Unimplemented; When set to true, will analyze tiles to determine if one chunk is a transform of another.  Will be much slower, but may save on disk space.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function ConvertFromBitmap(Image As Bitmap, Optional CheckForTransform As Boolean = False) As BGP
            Dim mapData(&H820 - 1) As Byte
            'Dim paletteData(&H400 - 1) As Byte
            Dim chunks(&H7FE0 - 1) As Byte
            'Dim blank(31) As Byte
            Dim paletteMaster As New Palette
            Dim FileOutput As New List(Of Byte)
            Dim localPaletteList As New List(Of List(Of Color))
            For y As Integer = 0 To 23
                For x As Integer = 0 To 31
                    'Dim chunkIndex = (y * 32) + x
                    Dim colors As New Generic.List(Of Color)
                    For y2 As Integer = 0 To 7
                        For x2 As Integer = 0 To 7
                            Dim c As Color = Image.GetPixel((x * 8 + x2), (y * 8 + y2))
                            If Not colors.Contains(c) Then
                                colors.Add(c)
                            End If
                        Next
                    Next
                    localPaletteList.Add(colors)
                Next
            Next
            localPaletteList.Sort(New NumberOfItemsOrderer)
            For Each localPalette In localPaletteList
                paletteMaster.GetPalateOffset(localPalette) 'generate palettes for future finding
            Next
            For y As Integer = 0 To 23
                For x As Integer = 0 To 31
                    Dim chunkIndex = (y * 32) + x

                    Dim colors As New Generic.List(Of Color)
                    For y2 As Integer = 0 To 7
                        For x2 As Integer = 0 To 7
                            Dim c As Color = Image.GetPixel((x * 8 + x2), (y * 8 + y2))
                            If Not colors.Contains(c) Then
                                colors.Add(c)
                            End If
                        Next
                    Next

                    'find palette index
                    Dim paletteOffset As Byte = paletteMaster.GetPalateOffset(colors)
                    'todo: check for transform
                    'create map data
                    Dim mapEntry As Byte() = {0, 0}
                    Dim chunkIndexBytes As Byte() = BitConverter.GetBytes(chunkIndex + 1)
                    mapEntry(0) = chunkIndexBytes(0)
                    mapEntry(1) = ((chunkIndexBytes(1) Or &HFC) - &HFC) Or (((paletteOffset Or &HF0) - &HF0) << 4)
                    mapData(chunkIndex * 2 + 0) = mapEntry(0)
                    mapData(chunkIndex * 2 + 1) = mapEntry(1)
                    'create tile data (bitmap)
                    Dim localPalette As Palette = paletteMaster.GetLocalPalette(paletteOffset)
                    Dim colorList As New Generic.List(Of Byte)
                    For y2 As Integer = 0 To 7
                        For x2 As Integer = 0 To 7
                            Dim ColorIndex As Integer = localPalette.Colors.IndexOf(Image.GetPixel((x * 8 + x2), (y * 8 + y2)))
                            If ColorIndex = -1 Then ColorIndex = 0
                            colorList.Add(CByte(ColorIndex))
                        Next
                    Next
                    For count As Integer = 0 To 31
                        'colors.Add(((b) Or &HF0) - &HF0)
                        'colors.Add(((b >> 4) Or &HF0) - &HF0)
                        chunks(chunkIndex * 32 + count) = (((colorList(count * 2)) Or &HF0) - &HF0) Or ((((colorList(count * 2 + 1)) Or &HF0) - &HF0) << 4)
                    Next
                Next
            Next
            'header
            FileOutput.Add(&H20)
            FileOutput.Add(0)
            FileOutput.Add(0)
            FileOutput.Add(0)
            FileOutput.Add(0)
            FileOutput.Add(4)
            FileOutput.Add(0)
            FileOutput.Add(0)
            FileOutput.Add(&H20)
            FileOutput.Add(&HC)
            FileOutput.Add(0)
            FileOutput.Add(0)
            FileOutput.Add(0)
            FileOutput.Add(&H80)
            FileOutput.Add(0)
            FileOutput.Add(0)
            'header line 2
            FileOutput.Add(&H20)
            FileOutput.Add(&H4)
            FileOutput.Add(0)
            FileOutput.Add(0)
            FileOutput.Add(0)
            FileOutput.Add(&H8)
            FileOutput.Add(0)
            FileOutput.Add(0)
            FileOutput.Add(1)
            FileOutput.Add(0)
            FileOutput.Add(0)
            FileOutput.Add(0)
            FileOutput.Add(0)
            FileOutput.Add(0)
            FileOutput.Add(0)
            FileOutput.Add(0)
            'Palette
            For Each b In paletteMaster.ToBytes
                FileOutput.Add(b)
            Next
            'Map
            For Each b In mapData
                FileOutput.Add(b)
            Next
            'Chunk
            For Each b In chunks
                FileOutput.Add(b)
            Next
            Return New BGP(FileOutput.ToArray)
        End Function

        Dim _image As Bitmap
        ''' <summary>
        ''' Returns the converted image.  Only converts the first time this is called.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Obsolete()>
        Public ReadOnly Property Image As Bitmap
            Get
                If _image Is Nothing Then
                    _image = ProcessMapping(MapData, ChunkData, PaletteData)
                End If
                Return _image
            End Get
        End Property

        Public Async Function GetImage() As Task(Of Bitmap)
            If _image Is Nothing Then
                _image = Await Task.Run(Function()
                                            Return ProcessMapping(MapData, ChunkData, PaletteData)
                                        End Function)
            End If
            Return _image
        End Function
        Public Function GetImageSync() As Bitmap
            Return ProcessMapping(MapData, ChunkData, PaletteData)
        End Function
        Public Async Function GetTempImageURI() As Task(Of String)
            Dim directory As String = PluginHelper.GetResourceName("Temp")
            If Not IO.Directory.Exists(directory) Then
                IO.Directory.CreateDirectory(directory)
            End If
            Dim uri As String = String.Format(directory & "\TempBPG{0}{1}.png", IO.Path.GetFileName(Filename), Guid.NewGuid.ToString)
            If IO.File.Exists(uri) Then
                IO.File.Delete(uri)
            End If
            Dim i = Await GetImage()
            i.Save(uri)
            Return uri
        End Function

        Public ReadOnly Property TempImageURI As String
            Get
                Dim directory As String = PluginHelper.GetResourceName("Temp")
                If Not IO.Directory.Exists(directory) Then
                    IO.Directory.CreateDirectory(directory)
                End If
                Dim uri As String = String.Format(directory & "\TempBPG{0}{1}.png", IO.Path.GetFileName(Filename), Guid.NewGuid.ToString)
                If IO.File.Exists(uri) Then
                    IO.File.Delete(uri)
                End If
                Dim i = GetImageSync()
                i.Save(uri)
                Return uri
            End Get
        End Property

        Public ReadOnly Property ImageName As String
            Get
                Return IO.Path.GetFileNameWithoutExtension(OriginalFilename)
            End Get
        End Property
        
        Public Shared Shadows Async Function FromFilename(Filename As String) As Task(Of BGP)
            Await RunDecompress(Filename)
            Return New BGP(Filename)
        End Function
        Public Sub New(Filename As String)
            MyBase.New(Filename)
        End Sub

        Public Sub New(RawData As Byte())
            MyBase.New(RawData)
        End Sub

        Public Sub New()
            MyBase.New({})
        End Sub
    End Class
End Namespace