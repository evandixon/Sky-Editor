Imports SkyEditorBase

Namespace Projects
    Public Class GtiImage2dProject
        Inherits GenericModProject

        Public Overrides Function GetSupportedGameCodes() As IEnumerable(Of String)
            Return {GameStrings.GTICode}
        End Function

        Public Overrides Function GetFilesToCopy(Solution As Solution, BaseRomProjectName As String) As IEnumerable(Of String)
            Return {IO.Path.Combine("romfs", "bg"), IO.Path.Combine("romfs", "font"), IO.Path.Combine("romfs", "image_2d")}
        End Function

        Public Overrides Async Function Initialize(Solution As Solution) As Task
            Await MyBase.Initialize(Solution)
            Dim rawFilesDir = GetRawFilesDir()
            Dim backDir = GetRootDirectory()

            Dim backFiles = IO.Directory.GetFiles(IO.Path.Combine(rawFilesDir, "romfs"), "*.img", IO.SearchOption.AllDirectories)
            Dim f As New Utilities.AsyncFor(PluginHelper.GetLanguageItem("Converting backgrounds..."))
            Await f.RunForEach(Function(Item As String) As Task
                                   Using b As New FileFormats.CteImage
                                       b.OpenFile(Item)
                                       Dim image = b.ContainedImage
                                       Dim newFilename = IO.Path.Combine(backDir, IO.Path.GetDirectoryName(Item).Replace(rawFilesDir, "").Replace("\romfs", "").Trim("\"), IO.Path.GetFileNameWithoutExtension(Item) & ".bmp")
                                       If Not IO.Directory.Exists(IO.Path.GetDirectoryName(newFilename)) Then
                                           IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(newFilename))
                                       End If
                                       image.Save(newFilename, Drawing.Imaging.ImageFormat.Bmp)
                                       IO.File.Copy(newFilename, newFilename & ".original")

                                       Dim internalDir = IO.Path.GetDirectoryName(Item).Replace(rawFilesDir, "").Replace("\romfs", "")
                                       Me.CreateDirectory(internalDir)
                                       Me.AddExistingFile(internalDir, newFilename)
                                       Return Task.CompletedTask
                                   End Using
                               End Function, backFiles)

            'For count = 0 To backFiles.Count - 1
            '    PluginHelper.StartLoading(String.Format(PluginHelper.GetLanguageItem("Converting backgrounds... ({0} of {1})"), count, backFiles.Count), count / backFiles.Count)
            '    Dim item = backFiles(count)
            '    Using b As New FileFormats.CteImage(item)
            '        Dim image = b.ContainedImage
            '        Dim newFilename = IO.Path.Combine(backDir, IO.Path.GetDirectoryName(item).Replace(rawFilesDir, "").Replace("\romfs", "").Trim("\"), IO.Path.GetFileNameWithoutExtension(item) & ".bmp")
            '        If Not IO.Directory.Exists(IO.Path.GetDirectoryName(newFilename)) Then
            '            IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(newFilename))
            '        End If
            '        image.Save(newFilename, Drawing.Imaging.ImageFormat.Bmp)
            '        IO.File.Copy(newFilename, newFilename & ".original")

            '        Dim internalDir = IO.Path.GetDirectoryName(item).Replace(rawFilesDir, "").Replace("\romfs", "")
            '        Me.CreateDirectory(internalDir)
            '        Await Me.AddExistingFile(internalDir, item)
            '    End Using
            'Next
            PluginHelper.SetLoadingStatusFinished()
        End Function

        Public Overrides Async Function Build(Solution As Solution) As Task
            'Convert BACK
            Dim sourceDir = GetRootDirectory()
            Dim rawFilesDir = GetRawFilesDir()

            For Each background In IO.Directory.GetFiles(GetRootDirectory, "*.bmp", IO.SearchOption.AllDirectories)
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
                    Dim img As New FileFormats.CteImage
                    img.OpenFile(IO.Path.Combine(rawFilesDir, "romfs", IO.Path.GetDirectoryName(background).Replace(sourceDir, ""), IO.Path.GetFileNameWithoutExtension(background) & ".img"))
                    img.ContainedImage = Drawing.Image.FromFile(background)
                    img.Save()
                    img.Dispose()
                End If

            Next
            Await MyBase.Build(Solution)
        End Function
    End Class

End Namespace
