Imports System.IO
Imports System.Reflection
Imports System.Text.RegularExpressions
Imports ROMEditor.Projects
Imports SkyEditor.Core.UI
Imports SkyEditor.Core.Utilities
Imports SkyEditorBase
Imports TagLib

Namespace MenuActions
    Public Class PsmdSoundtrackMenuAction
        Inherits MenuAction

        Private Class FileAbstraction
            Implements TagLib.File.IFileAbstraction
            Implements IDisposable

            Private Filestream As IO.FileStream

            Public ReadOnly Property Name As String Implements TagLib.File.IFileAbstraction.Name
                Get
                    Return Filestream.Name
                End Get
            End Property

            Public ReadOnly Property ReadStream As Stream Implements TagLib.File.IFileAbstraction.ReadStream
                Get
                    Return Filestream
                End Get
            End Property

            Public ReadOnly Property WriteStream As Stream Implements TagLib.File.IFileAbstraction.WriteStream
                Get
                    Return Filestream
                End Get
            End Property

            Public Sub CloseStream(stream As Stream) Implements TagLib.File.IFileAbstraction.CloseStream
                stream.Close()
            End Sub

            Public Sub New(Filename As String)
                Filestream = IO.File.Open(Filename, IO.FileMode.Open, IO.FileAccess.ReadWrite)
            End Sub

#Region "IDisposable Support"
            Private disposedValue As Boolean ' To detect redundant calls

            ' IDisposable
            Protected Overridable Sub Dispose(disposing As Boolean)
                If Not Me.disposedValue Then
                    If disposing Then
                        ' TODO: dispose managed state (managed objects).
                        Filestream.Dispose()
                    End If

                    ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                    ' TODO: set large fields to null.
                End If
                Me.disposedValue = True
            End Sub

            ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
            'Protected Overrides Sub Finalize()
            '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            '    Dispose(False)
            '    MyBase.Finalize()
            'End Sub

            ' This code added by Visual Basic to correctly implement the disposable pattern.
            Public Sub Dispose() Implements IDisposable.Dispose
                ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
                Dispose(True)
                ' TODO: uncomment the following line if Finalize() is overridden above.
                ' GC.SuppressFinalize(Me)
            End Sub
#End Region
        End Class

        Public Overrides Function SupportedTypes() As IEnumerable(Of TypeInfo)
            Return {GetType(BaseRomProject).GetTypeInfo}
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

                                       'Add the tag
                                       Using abs As New FileAbstraction(destinationMp3)
                                           Dim t As New TagLib.Mpeg.AudioFile(abs)
                                           With t.Tag
                                               .Album = My.Resources.Language.PSMDSoundTrackAlbum
                                               .AlbumArtists = {My.Resources.Language.PSMDSoundTrackArtist}
                                               .Year = 2015
                                               Dim filenameParts = trackNames(filename).Split(" ".ToCharArray, 2)
                                               If filenameParts.Count = 2 Then
                                                   If IsNumeric(filenameParts(0)) Then
                                                       .Track = CInt(filenameParts(0))
                                                   End If

                                                   .Title = filenameParts(1)
                                               End If
                                           End With
                                           t.Save()
                                       End Using
                                   End Function, trackNames.Keys)
            Next
            PluginHelper.SetLoadingStatusFinished()
        End Function

        Public Sub New()
            MyBase.New({My.Resources.Language.MenuUtilities, My.Resources.Language.MenuUtilitiesExportSoundtrack})
            SortOrder = 4.1
        End Sub
    End Class

End Namespace
