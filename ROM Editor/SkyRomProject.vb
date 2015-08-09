Imports System.Security.Cryptography
Imports System.Web.Script.Serialization
Imports System.Windows.Forms
Imports ROMEditor
Imports ROMEditor.Roms
Imports SkyEditorBase

Public Class SkyRomProject
    Inherits GenericNDSModProject

    Private Async Sub SkyRomProject_NDSModAdded(sender As Object, e As NDSModAddedEventArgs) Handles Me.NDSModAdded
        Dim romDirectory = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "Mods", IO.Path.GetFileNameWithoutExtension(e.InternalName), "RawFiles")
        Dim sky = DirectCast(Files("BaseRom.nds"), SkyNDSRom)

        'Convert BACK
        Dim BACKdir As String = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "Mods", IO.Path.GetFileNameWithoutExtension(e.InternalName), "Backgrounds")
        CreateDirectory("Mods/" & IO.Path.GetFileNameWithoutExtension(e.InternalName) & "/Backgrounds/")
        For Each item In IO.Directory.GetFiles(IO.Path.Combine(romDirectory, "Data", "BACK"), "*.bgp")
            Dim b As New FileFormats.BGP(item)
            Dim image = Await b.GetImage
            Dim newFilename = IO.Path.Combine(BACKdir, IO.Path.GetFileNameWithoutExtension(item) & ".bmp")
            If Not IO.Directory.Exists(IO.Path.GetDirectoryName(newFilename)) Then
                IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(newFilename))
            End If
            image.Save(newFilename, Drawing.Imaging.ImageFormat.Bmp)
            IO.File.Copy(newFilename, newFilename & ".original")
            OpenFile(newFilename, "Mods/" & IO.Path.GetFileNameWithoutExtension(e.InternalName) & "/Backgrounds/" & IO.Path.GetFileName(newFilename), False)
        Next
    End Sub

    Private Sub SkyRomProject_NDSModBuilding(sender As Object, e As NDSModBuildingEventArgs) Handles Me.NDSModBuilding
        'Convert BACK
        For Each background In IO.Directory.GetFiles(IO.Path.Combine(IO.Path.GetDirectoryName(e.NDSModSourceFilename), IO.Path.GetFileNameWithoutExtension(e.NDSModSourceFilename), "Backgrounds"), "*.bmp")
            Dim includeInPack As Boolean

            If IO.File.Exists(background & ".original") Then
                Using bmp As New IO.FileStream(background, IO.FileMode.Open)
                    Using orig As New IO.FileStream(background & ".original", IO.FileMode.Open)
                        Dim equal As Boolean = (bmp.Length = orig.Length)
                        While equal
                            Dim b = bmp.ReadByte
                            Dim o = orig.ReadByte
                            equal = (b = o)
                            If b = -1 OrElse o = -1 Then
                                Exit While
                            End If
                        End While
                        includeInPack = Not equal
                    End Using
                End Using
            Else
                includeInPack = True
            End If

            If includeInPack Then
                Dim bgp = FileFormats.BGP.ConvertFromBitmap(Drawing.Bitmap.FromFile(background))
                bgp.Save(IO.Path.Combine(IO.Path.GetDirectoryName(e.NDSModSourceFilename), IO.Path.GetFileNameWithoutExtension(e.NDSModSourceFilename), "RawFiles", "Data", "BACK", IO.Path.GetFileNameWithoutExtension(background) & ".bgp"))
            End If

        Next
        'Cleanup
        '-Data/Back/Decompressed
        If IO.Directory.Exists(IO.Path.Combine(IO.Path.GetDirectoryName(e.NDSModSourceFilename), IO.Path.GetFileNameWithoutExtension(e.NDSModSourceFilename), "RawFiles", "Data", "BACK", "Decompressed")) Then
            IO.Directory.Delete(IO.Path.Combine(IO.Path.GetDirectoryName(e.NDSModSourceFilename), IO.Path.GetFileNameWithoutExtension(e.NDSModSourceFilename), "RawFiles", "Data", "BACK", "Decompressed"), True)
        End If
    End Sub
End Class
