Imports System.Windows
Imports SkyEditorBase

Namespace Mods
    Public Class GtiImageBg
        Inherits GenericMod
        Public Sub New()
            MyBase.New()
        End Sub

        Public Overrides Function FilesToCopy() As IEnumerable(Of String)
            Return {IO.Path.Combine("romfs", "bg")}
        End Function

        Public Overrides Function FilesToArchive() As IEnumerable(Of String)
            Return {"Backgrounds"}
        End Function

        Public Overrides Sub Initialize(CurrentProject As Project)
            'Convert BACK
            Dim BACKdir As String = IO.Path.Combine(ModDirectory, "Backgrounds")
            CurrentProject.CreateDirectory("Mods/" & IO.Path.GetFileNameWithoutExtension(OriginalFilename) & "/Backgrounds/")
            Dim backFiles = IO.Directory.GetFiles(IO.Path.Combine(ROMDirectory, "romfs", "bg"), "*.img")
            For count = 0 To backFiles.Count - 1
                PluginHelper.StartLoading(PluginHelper.GetLanguageItem("Converting backgrounds..."), count / backFiles.Count)
                Dim item = backFiles(count)
                Using b As New FileFormats.CteImage(item)
                    Try
                        Dim image = b.ContainedImage
                        Dim newFilename = IO.Path.Combine(BACKdir, IO.Path.GetFileNameWithoutExtension(item) & ".bmp")
                        If Not IO.Directory.Exists(IO.Path.GetDirectoryName(newFilename)) Then
                            IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(newFilename))
                        End If
                        image.Save(newFilename, Drawing.Imaging.ImageFormat.Bmp)
                        IO.File.Copy(newFilename, newFilename & ".original")
                        CurrentProject.OpenFile(newFilename, "Mods/" & IO.Path.GetFileNameWithoutExtension(OriginalFilename) & "/Backgrounds/" & IO.Path.GetFileName(newFilename), False)
                    Catch ex As BadImageFormatException
                        MessageBox.Show(String.Format(PluginHelper.GetLanguageItem("BadImageFormatConversion", "Unable to convert image {0}.  Bad image format."), IO.Path.GetFileNameWithoutExtension(b.OriginalFilename)))
                    End Try
                End Using
            Next
            PluginHelper.StopLoading()
        End Sub

        Public Overrides Sub Build(CurrentProject As Project)
            'Convert BACK
            If IO.Directory.Exists(IO.Path.Combine(ModDirectory, "Backgrounds")) Then
                For Each background In IO.Directory.GetFiles(IO.Path.Combine(ModDirectory, "Backgrounds"), "*.bmp")
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
                        Dim img As New FileFormats.CteImage(IO.Path.Combine(ROMDirectory, "romfs", "bg", IO.Path.GetFileNameWithoutExtension(background) & ".img"))
                        img.ContainedImage = Drawing.Image.FromFile(background)
                        img.Save()
                        img.Dispose()
                    End If

                Next
            End If
        End Sub

        Public Overrides Function SupportedGameCodes() As IEnumerable(Of Type)
            Return {GetType(Roms.GatesToInfinityRom)}
        End Function

        Public Sub New(Filename As String)
            MyBase.New(Filename)
        End Sub
    End Class
End Namespace