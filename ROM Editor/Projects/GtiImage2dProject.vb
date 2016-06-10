Imports ROMEditor.FileFormats.PSMD
Imports SkyEditor.Core.EventArguments
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Utilities

Namespace Projects
    Public Class GtiImage2dProject
        Inherits GenericModProject

        Public Overrides Function GetSupportedGameCodes() As IEnumerable(Of String)
            Return {GameStrings.GTICode}
        End Function

        Public Overrides Function GetFilesToCopy(Solution As Solution, BaseRomProjectName As String) As IEnumerable(Of String)
            Return {IO.Path.Combine("romfs", "bg"), IO.Path.Combine("romfs", "font"), IO.Path.Combine("romfs", "image_2d")}
        End Function

        Protected Overrides Async Function Initialize() As Task
            Await MyBase.Initialize
            Dim rawFilesDir = GetRawFilesDir()
            Dim backDir = GetRootDirectory()

            Me.BuildStatusMessage = My.Resources.Language.LoadingConvertingBackgrounds
            Me.IsBuildProgressIndeterminate = False
            Me.BuildProgress = 0

            Dim backFiles = IO.Directory.GetFiles(IO.Path.Combine(rawFilesDir, "romfs"), "*.img", IO.SearchOption.AllDirectories)
            Dim toAdd As New List(Of Project.AddExistingFileBatchOperation)
            Dim f As New AsyncFor
            AddHandler f.LoadingStatusChanged, Sub(sender As Object, e As LoadingStatusChangedEventArgs)
                                                   Me.BuildProgress = e.Progress
                                               End Sub
            Await f.RunForEach(Async Function(Item As String) As Task
                                   Using b As New CteImage
                                       Await b.OpenFile(Item, CurrentPluginManager.CurrentIOProvider)
                                       Dim image = b.ContainedImage
                                       Dim newFilename = IO.Path.Combine(backDir, IO.Path.GetDirectoryName(Item).Replace(rawFilesDir, "").Replace("\romfs", "").Trim("\"), IO.Path.GetFileNameWithoutExtension(Item) & ".bmp")
                                       If Not IO.Directory.Exists(IO.Path.GetDirectoryName(newFilename)) Then
                                           IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(newFilename))
                                       End If
                                       image.Save(newFilename, Drawing.Imaging.ImageFormat.Bmp)
                                       IO.File.Copy(newFilename, newFilename & ".original")

                                       Dim internalDir = IO.Path.GetDirectoryName(Item).Replace(rawFilesDir, "").Replace("\romfs", "")
                                       toAdd.Add(New AddExistingFileBatchOperation With {.ActualFilename = newFilename, .ParentPath = internalDir})
                                   End Using
                               End Function, backFiles)

            Await Me.RecreateRootWithExistingFiles(toAdd, CurrentPluginManager.CurrentIOProvider)

            Me.BuildProgress = 1
            Me.BuildStatusMessage = My.Resources.Language.Complete
        End Function

        Protected Overrides Async Function DoBuild() As Task
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
                    Dim img As New CteImage
                    Await img.OpenFile(IO.Path.Combine(rawFilesDir, "romfs", IO.Path.GetDirectoryName(background).Replace(sourceDir, ""), IO.Path.GetFileNameWithoutExtension(background) & ".img"), CurrentPluginManager.CurrentIOProvider)
                    img.ContainedImage = Drawing.Image.FromFile(background)
                    img.Save(CurrentPluginManager.CurrentIOProvider)
                    img.Dispose()
                End If

            Next
            Await MyBase.DoBuild
        End Function
    End Class

End Namespace
