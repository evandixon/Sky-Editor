Imports System.Text.RegularExpressions
Imports ROMEditor.Projects
Imports SkyEditor.Core.Utilities
Imports SkyEditorBase
Namespace MenuActions
    Public Class PsmdSoundtrackMenuAction
        Inherits MenuAction

        Public Overrides Function SupportedTypes() As IEnumerable(Of Type)
            Return {GetType(BaseRomProject)}
        End Function

        Public Overrides Function SupportsObject(Obj As Object) As Boolean
            If TypeOf Obj Is BaseRomProject Then
                Dim psmd As New Regex(GameStrings.PSMDCode)
                Return DirectCast(Obj, BaseRomProject).RomSystem = "3DS" AndAlso psmd.IsMatch(DirectCast(Obj, BaseRomProject).GameCode)
            Else
                Return False
            End If
        End Function

        Public Overrides Async Function DoAction(Targets As IEnumerable(Of Object)) As Task
            For Each Project As BaseRomProject In Targets
                Dim sourceDir As String = IO.Path.Combine(Project.GetRawFilesDir, "romfs", "sound", "stream")
                Dim destDir As String = IO.Path.Combine(Project.GetRootDirectory, "Soundtrack")

                'Todo: do error checks on input file
                Dim trackNames As New Dictionary(Of String, String)
                If IO.File.Exists(PluginHelper.GetResourceName("PSMD English Soundtrack.txt")) Then
                    Dim lines = IO.File.ReadAllLines(PluginHelper.GetResourceName("PSMD English Soundtrack.txt"))
                    For Each item In lines
                        Dim parts = item.Split("=".ToCharArray, 2)
                        If parts.Count = 2 Then
                            trackNames.Add(parts(0), parts(1))
                        End If
                    Next
                End If

                If Not IO.Directory.Exists(destDir) Then
                    IO.Directory.CreateDirectory(destDir)
                End If

                For Each item In IO.Directory.GetFiles(destDir)
                    IO.File.Delete(item)
                Next

                PluginHelper.SetLoadingStatus(My.Resources.Language.ConvertingStreams)

                Dim f As New AsyncFor(My.Resources.Language.ConvertingStreams)
                Await f.RunForEach(Async Function(Item As String) As Task
                                       Dim source = IO.Path.Combine(sourceDir, Item) & ".dspadpcm.bcstm"

                                       'Create the wav
                                       Dim destinationWav = source.Replace(sourceDir, destDir).Replace("dspadpcm.bcstm", "wav")

                                       Dim filename = IO.Path.GetFileNameWithoutExtension(destinationWav)

                                       If trackNames.ContainsKey(filename) Then
                                           destinationWav = destinationWav.Replace(filename, trackNames(filename).Replace(":", "").Replace("é", "e"))
                                       End If

                                       For Each c In "!?,".ToCharArray
                                           destinationWav = destinationWav.Replace(c, "")
                                       Next

                                       Dim destinationMp3 = destinationWav.Replace(".wav", ".mp3")

                                       Await vgmstream.RunVGMStream(source, destinationWav)

                                       'Convert to mp3
                                       Await ffmpeg.ConvertToMp3(destinationWav, destinationMp3)

                                       IO.File.Delete(destinationWav)

                                       ' Dim m = IdSharp.AudioInfo.AudioFile.Create(destinationMp3, True)
                                       Dim t As New IdSharp.Tagging.ID3v2.ID3v2Tag(destinationMp3)
                                       t.Album = My.Resources.Language.PSMDSoundTrackAlbum
                                       t.Artist = My.Resources.Language.PSMDSoundTrackArtist
                                       t.Year = 2015
                                       Dim filenameParts = trackNames(filename).Split(" ".ToCharArray, 2)
                                       If filenameParts.Count = 2 Then
                                           If IsNumeric(filenameParts(0)) Then
                                               t.TrackNumber = CInt(filenameParts(0))
                                           End If

                                           t.Title = filenameParts(1)
                                       End If
                                       t.Save(destinationMp3)
                                   End Function, trackNames.Keys)
            Next
        End Function

        Public Sub New()
            MyBase.New({My.Resources.Language.MenuUtilities, My.Resources.Language.MenuUtilitiesExportSoundtrack})
            SortOrder = 4.1
        End Sub
    End Class

End Namespace
