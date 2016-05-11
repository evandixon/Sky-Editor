Imports SkyEditor.Core.IO

Namespace FileFormats.PSMD
    Public Class WazaDataInfo
        Implements IOpenableFile

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

        Public Function OpenFile(Filename As String, Provider As IOProvider) As Task Implements IOpenableFile.OpenFile
            Const entryLength = 18
            Using f As New SkyEditor.Core.Windows.GenericFile
                f.EnableInMemoryLoad = True
                f.OpenFile(Filename)

                For count = 0 To ((f.Length / entryLength) - 1)
                    Entries.Add(New WazaDataInfoEntry(f.RawData(count * entryLength, entryLength)))
                Next
            End Using
            Return Task.completedtask
        End Function

        Public Sub New()
            Entries = New List(Of WazaDataInfoEntry)
        End Sub

    End Class

End Namespace
