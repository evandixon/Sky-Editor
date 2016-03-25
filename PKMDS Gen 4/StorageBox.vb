Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Runtime.InteropServices
Imports PokemonDSLib.PokemonLib

<Serializable()>
<StructLayout(LayoutKind.Sequential)>
Public Class StorageBox

    Public Number As PCBoxes
    Public Name As String
    Public StoredPokemon() As Pokemon

    <NonSerialized>
    Public Wallpaper As mWallpaper

    Public Sub New()
        Number = 0
        Name = String.Empty
        StoredPokemon = Nothing
        Wallpaper = New mWallpaper
    End Sub

    Public Sub New(ByVal mNumber As PCBoxes, ByVal mName As String, ByVal mPKM() As Pokemon, ByVal _Wallpaper As Byte)
        Number = mNumber
        Name = mName
        StoredPokemon = mPKM
        Wallpaper = New mWallpaper(_Wallpaper)
    End Sub

    Public ReadOnly Property PokemonCount() As Byte
        Get
            PokemonCount = 0
            For Each pkm As Pokemon In StoredPokemon
                If pkm.Species.ID > 0 And pkm.Species.ID < 494 Then PokemonCount += 1
            Next
        End Get
    End Property

    Public Sub Grid(ByVal g As Graphics, Optional ByVal Scale As Integer = 1)

        Dim IMG As Bitmap = New Bitmap(6, 5)
        Dim pkm As New Pokemon
        For i As Integer = 0 To 29
            pkm = StoredPokemon(i)
            With pkm.Species
                If .ID > 0 And .ID < 494 Then
                    IMG.SetPixel(i Mod 6, Math.Floor(i / 6), pkm.BaseStats.Color)
                Else
                    IMG.SetPixel(i Mod 6, Math.Floor(i / 6), Color.Transparent)
                End If
            End With
        Next

        ' destination rectangle
        Dim rectDst As New Rectangle()

        rectDst.X = 0
        rectDst.Y = 0
        rectDst.Width = IMG.Width * Scale
        rectDst.Height = IMG.Height * Scale

        ' source rectangle
        Dim rectSrc As New Rectangle()

        rectSrc.X = 0
        rectSrc.Y = 0
        rectSrc.Width = IMG.Width
        rectSrc.Height = IMG.Height

        ' draw (part of the image)
        'Dim g As Graphics = PB.CreateGraphics 'e.Graphics
        g.InterpolationMode = InterpolationMode.NearestNeighbor
        g.PixelOffsetMode = PixelOffsetMode.Half
        g.DrawImage(IMG, rectDst, rectSrc, GraphicsUnit.Pixel)

    End Sub

End Class