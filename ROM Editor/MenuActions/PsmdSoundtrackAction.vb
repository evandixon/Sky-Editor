Imports ROMEditor.Projects
Imports SkyEditorBase
Namespace MenuActions
    Public Class PsmdSoundtrackMenuAction
        Inherits MenuAction

        Public Overrides Function SupportedTypes() As IEnumerable(Of Type)
            Return {GetType(BaseRomProject)}
        End Function

        Public Overrides Function SupportsObject(Obj As Object) As Boolean
            If TypeOf Obj Is BaseRomProject Then
                Return DirectCast(Obj, BaseRomProject).RomSystem = "3DS" AndAlso DirectCast(Obj, BaseRomProject).GameCode = GameStrings.PSMDCode
            Else
                Return False
            End If
        End Function

        Public Overrides Async Function DoAction(Targets As IEnumerable(Of Object)) As Task
            For Each Project As BaseRomProject In Targets
                Console.WriteLine("Starting conversion...")

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
                Console.WriteLine("Track names: " & trackNames.Count)
                If trackNames.Count = 0 Then
                    For Each item In IO.File.ReadAllLines(PluginHelper.GetResourceName("PSMD English Soundtrack.txt"))
                        Console.WriteLine(item)
                    Next
                End If

                If Not IO.Directory.Exists(destDir) Then
                    IO.Directory.CreateDirectory(destDir)
                End If

                For Each item In IO.Directory.GetFiles(destDir)
                    IO.File.Delete(item)
                Next

                PluginHelper.SetLoadingStatus(PluginHelper.GetLanguageItem("Converting streams..."))

                Dim f As New SkyEditorBase.Utilities.AsyncFor(PluginHelper.GetLanguageItem("Converting streams..."))
                Await f.RunForEach(Async Function(Item As String) As Task
                                       Console.WriteLine("Converting " & Item & " to wav.")
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
                                       Console.WriteLine("Converting " & Item & " to mp3.")
                                       Await ffmpeg.ConvertToMp3(destinationWav, destinationMp3)

                                       IO.File.Delete(destinationWav)

                                       Console.WriteLine("Tagging " & Item)

                                       ' Dim m = IdSharp.AudioInfo.AudioFile.Create(destinationMp3, True)
                                       Dim t As New IdSharp.Tagging.ID3v2.ID3v2Tag(destinationMp3)
                                       t.Album = "Pokémon Super Mystery Dungeon"
                                       t.Artist = "Chunsoft"
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

                Console.WriteLine("Conversion complete!")
            Next
        End Function

        Public Sub New()
            MyBase.New({PluginHelper.GetLanguageItem("Utilities"), PluginHelper.GetLanguageItem("Export Soundtrack")})
        End Sub
    End Class

End Namespace
