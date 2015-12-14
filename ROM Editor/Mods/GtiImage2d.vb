Imports System.Windows
Imports SkyEditorBase

Namespace Mods
    Public Class GtiImage2d
        Inherits GenericMod
        Public Sub New()
            MyBase.New()
        End Sub

        Public Overrides Function FilesToCopy() As IEnumerable(Of String)
            Return {IO.Path.Combine("romfs", "bg"), IO.Path.Combine("romfs", "font"), IO.Path.Combine("romfs", "image_2d")}
        End Function

        Public Overrides Function FilesToArchive() As IEnumerable(Of String)
            Return {"bg", "font", "image_2d"}
        End Function

        Public Overrides Sub Initialize(CurrentProject As Project)
            'Convert BACK
            Dim BACKdir As String = ModDirectory
            CurrentProject.CreateDirectory("Mods/" & IO.Path.GetFileNameWithoutExtension(OriginalFilename) & "/Backgrounds/")
            Dim backFiles = IO.Directory.GetFiles(IO.Path.Combine(ROMDirectory, "romfs"), "*.img", IO.SearchOption.AllDirectories)
            CurrentProject.EnableRaisingEvents = False
            For count = 0 To backFiles.Count - 1
                PluginHelper.StartLoading(String.Format(PluginHelper.GetLanguageItem("Converting backgrounds... ({0} of {1})"), count, backFiles.Count), count / backFiles.Count)
                Dim item = backFiles(count)
                Try
                    Using b As New FileFormats.CteImage(item)
                        Dim image = b.ContainedImage
                        Dim newFilename = IO.Path.Combine(BACKdir, IO.Path.GetDirectoryName(item).Replace(ROMDirectory, "").Replace("\romfs", "").Trim("\"), IO.Path.GetFileNameWithoutExtension(item) & ".bmp")
                        If Not IO.Directory.Exists(IO.Path.GetDirectoryName(newFilename)) Then
                            IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(newFilename))
                        End If
                        image.Save(newFilename, Drawing.Imaging.ImageFormat.Bmp)
                        IO.File.Copy(newFilename, newFilename & ".original")

                        Dim internalDir = "Mods/" & IO.Path.GetFileNameWithoutExtension(OriginalFilename) & IO.Path.GetDirectoryName(item).Replace(ROMDirectory, "").Replace("\romfs", "")
                        If Not CurrentProject.Files.ContainsKey(internalDir) Then
                            CurrentProject.CreateDirectory(internalDir)
                        End If
                        CurrentProject.OpenFile(newFilename, internalDir & IO.Path.GetFileName(newFilename), False)
                    End Using
                Catch ne As NotImplementedException
                    'Do nothing, simply don't convert the file for now.
                    'We will, however, log it.
                    PluginHelper.Writeline(String.Format("Error converting {0}: ", IO.Path.GetFileNameWithoutExtension(item)) & ne.Message, PluginHelper.LineType.Warning)
                Catch ex As BadImageFormatException
                    MessageBox.Show(String.Format(PluginHelper.GetLanguageItem("BadImageFormatConversion", "Unable to convert image {0}.  Bad image format."), IO.Path.GetFileNameWithoutExtension(item)))
                End Try
            Next
            CurrentProject.EnableRaisingEvents = True
            PluginHelper.StopLoading()
        End Sub

        Public Overrides Sub Build(CurrentProject As Project)
            'Convert BACK
            If IO.Directory.Exists(ModDirectory) Then
                For Each background In IO.Directory.GetFiles(ModDirectory, "*.bmp", IO.SearchOption.AllDirectories)
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
                        Dim img As New FileFormats.CteImage(IO.Path.Combine(ROMDirectory, "romfs", IO.Path.GetDirectoryName(background).Replace(ModDirectory, ""), IO.Path.GetFileNameWithoutExtension(background) & ".img"))
                        img.ContainedImage = Drawing.Image.FromFile(background)
                        img.Save()
                        img.Dispose()
                    End If

                Next
            End If
        End Sub

        Public Overrides Function SupportedGameTypes() As IEnumerable(Of Type)
            Return {GetType(Roms.GatesToInfinityRom)}
        End Function

        Public Sub New(Filename As String)
            MyBase.New(Filename)
        End Sub
    End Class
End Namespace