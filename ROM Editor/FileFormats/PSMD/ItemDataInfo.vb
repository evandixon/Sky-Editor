Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.Windows
Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Namespace FileFormats.PSMD
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Credit to Andibad for research.  https://projectpokemon.org/forums/showthread.php?46904-Pokemon-Super-Mystery-Dungeon-And-PMD-GTI-Research-And-Utilities&amp;p=211199&amp;viewfull=1#post211199
    ''' </remarks>

    Public Class ItemDataInfo
        Implements iOpenableFile
        Public Class ItemDataInfoEntry
            Public Property BuyPrice As UInt16
            Public Property SellPrice As UInt16
            Public Sub New(RawData As Byte())
                BuyPrice = BitConverter.ToUInt16(RawData, 2)
                SellPrice = BitConverter.ToUInt16(RawData, 4)
            End Sub
        End Class
        Public Property Entries As List(Of ItemDataInfoEntry)

        Public Sub OpenFile(Filename As String) Implements iOpenableFile.OpenFile
            Const entryLength = &H24
            Using f As New GenericFile
                f.EnableInMemoryLoad = True
                f.OpenFile(Filename)

                For count = 0 To ((f.Length / entryLength) - 1)
                    Entries.Add(New ItemDataInfoEntry(f.RawData(count * entryLength, entryLength)))
                Next
            End Using
        End Sub

        Public Sub New()
            Entries = New List(Of ItemDataInfoEntry)
        End Sub
    End Class
End Namespace