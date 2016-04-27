Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.Windows
Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Namespace FileFormats.PSMD
    Public Class WazaDataInfo
        Implements iOpenableFile

        Public Class WazaDataInfoEntry
            ''' <summary>
            ''' The move index in act_data_info.bin
            ''' </summary>
            ''' <returns></returns>
            Public Property ActDataInfoIndex As UInt16
            Public Sub New(RawData As Byte())
                ActDataInfoIndex = BitConverter.ToUInt16(RawData, &HC)
            End Sub
        End Class

        Public Property Entries As List(Of WazaDataInfoEntry)

        Public Sub OpenFile(Filename As String) Implements iOpenableFile.OpenFile
            Const entryLength = 18
            Using f As New GenericFile
                f.EnableInMemoryLoad = True
                f.OpenFile(Filename)

                For count = 0 To ((f.Length / entryLength) - 1)
                    Entries.Add(New WazaDataInfoEntry(f.RawData(count * entryLength, entryLength)))
                Next
            End Using
        End Sub

        Public Sub New()
            Entries = New List(Of WazaDataInfoEntry)
        End Sub

    End Class

End Namespace
