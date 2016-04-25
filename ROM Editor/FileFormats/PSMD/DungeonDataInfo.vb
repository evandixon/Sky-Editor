Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Namespace FileFormats.PSMD
    Public Class DungeonDataInfo
        Implements iOpenableFile
        Private Const EntryLength As Integer = &H18

        Public Class DungeonDataInfoEntry
            Private Property RawData As Byte()
            Public ReadOnly Property DungeonID As UInt16
                Get
                    Return BitConverter.ToUInt16(RawData, 0)
                End Get
            End Property
            Private ReadOnly Property Unknown As UInt16
                Get
                    Return BitConverter.ToUInt16(RawData, 4)
                End Get
            End Property
            Public Function ToBytes() As Byte()
                Return RawData
            End Function
            Public Sub New(RawData As Byte())
                Me.RawData = RawData
            End Sub
        End Class

        Public Property Entries As List(Of DungeonDataInfoEntry)

        Public Sub OpenFile(Filename As String) Implements iOpenableFile.OpenFile
            Using f As New GenericFile
                f.OpenFile(Filename)

                Dim numEntries = Math.Floor(f.Length / EntryLength)

                For count = 0 To numEntries - 1
                    Entries.Add(New DungeonDataInfoEntry(f.RawData(count * EntryLength, EntryLength)))
                Next
            End Using
        End Sub

        Public Sub New()
            Entries = New List(Of DungeonDataInfoEntry)
        End Sub


    End Class
End Namespace
