Imports ROMEditor.FileFormats.Explorers
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Utilities
Imports SkyEditorBase

Namespace Projects
    Public Class SkyBackModProject
        Inherits GenericModProject

        Public Overrides Function GetFilesToCopy(Solution As Solution, BaseRomProjectName As String) As IEnumerable(Of String)
            Return {IO.Path.Combine("data", "BACK")}
        End Function

        Public Overrides Function GetSupportedGameCodes() As IEnumerable(Of String)
            Return {GameStrings.SkyCode}
        End Function

        Protected Overrides Async Function Initialize() As Task
            Await MyBase.Initialize

            Dim projectDir = GetRootDirectory()
            Dim sourceDir = GetRawFilesDir()

            Dim BACKdir As String = IO.Path.Combine(projectDir, "Backgrounds")
            Me.CreateDirectory("Backgrounds")
            Dim backFiles = IO.Directory.GetFiles(IO.Path.Combine(sourceDir, "Data", "BACK"), "*.bgp")
            Dim f As New AsyncFor(My.Resources.Language.LoadingConvertingBackgrounds)

            Await f.RunForEach(Async Function(Item As String) As Task
                                   Using b As New BGP
                                       Await b.OpenFile(Item, CurrentPluginManager.CurrentIOProvider)
                                       Dim image = Await b.GetImage
                                       Dim newFilename = IO.Path.Combine(BACKdir, IO.Path.GetFileNameWithoutExtension(Item) & ".bmp")
                                       If Not IO.Directory.Exists(IO.Path.GetDirectoryName(newFilename)) Then
                                           IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(newFilename))
                                       End If
                                       image.Save(newFilename, Drawing.Imaging.ImageFormat.Bmp)
                                       IO.File.Copy(newFilename, newFilename & ".original")
                                       Await Me.AddExistingFile("Backgrounds", newFilename, CurrentPluginManager.CurrentIOProvider)
                                   End Using
                               End Function, backFiles)
        End Function

        Protected Overrides Async Function DoBuild() As Task
            'Convert BACK
            Dim projectDir = GetRootDirectory()
            Dim rawDir = GetRawFilesDir()
            If IO.Directory.Exists(IO.Path.Combine(projectDir, "Backgrounds")) Then
                For Each background In IO.Directory.GetFiles(IO.Path.Combine(projectDir, "Backgrounds"), "*.bmp")
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
                        Dim img = BGP.ConvertFromBitmap(Drawing.Bitmap.FromFile(background))
                        img.Save(IO.Path.Combine(rawDir, "Data", "BACK", IO.Path.GetFileNameWithoutExtension(background) & ".bgp"), CurrentPluginManager.CurrentIOProvider)
                        img.Dispose()
                        Await BGP.RunCompress(IO.Path.Combine(rawDir, "Data", "BACK", IO.Path.GetFileNameWithoutExtension(background) & ".bgp"))
                    End If

                Next
            End If
            'Cleanup
            '-Data/Back/Decompressed
            If IO.Directory.Exists(IO.Path.Combine(rawDir, "Data", "BACK", "Decompressed")) Then
                IO.Directory.Delete(IO.Path.Combine(rawDir, "Data", "BACK", "Decompressed"), True)
            End If

            Await MyBase.DoBuild
        End Function
    End Class
End Namespace

