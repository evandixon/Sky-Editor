Imports SkyEditor.Core.IO

Namespace FileFormats.PSMD
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Credit to Andibad for research.  https://projectpokemon.org/forums/showthread.php?46904-Pokemon-Super-Mystery-Dungeon-And-PMD-GTI-Research-And-Utilities&amp;p=211199&amp;viewfull=1#post211199
    ''' </remarks>
    Public Class ActHitCountTableDataInfo
        Implements IOpenableFile

        Public Class ActHitCountTableDataInfoEntry
            ''' <summary>
            ''' Whether or not the move will be repeated until the first miss.
            ''' </summary>
            ''' <returns></returns>
            Public Property RepeatUntilMiss As Boolean
            Public Property HitCountMinimum As Byte
            Public Property HitCountMaximum As Byte
            Public Property HitChance2 As UInt16
            Public Property HitChance3 As UInt16
            Public Property HitChance4 As UInt16
            Public Property HitChance5 As UInt16
            Public Sub New(RawData As Byte())
                RepeatUntilMiss = (RawData(0) = 1)
                HitCountMinimum = RawData(1)
                HitCountMaximum = RawData(2)
                HitChance2 = BitConverter.ToUInt16(RawData, 4)
                HitChance3 = BitConverter.ToUInt16(RawData, 6)
                HitChance4 = BitConverter.ToUInt16(RawData, 8)
                HitChance5 = BitConverter.ToUInt16(RawData, 10)
            End Sub
        End Class

        Public Property Entries As List(Of ActHitCountTableDataInfoEntry)

        Public Function OpenFile(Filename As String, Provider As IOProvider) As Task Implements IOpenableFile.OpenFile
            Using f As New SkyEditor.Core.Windows.GenericFile
                f.EnableInMemoryLoad = True
                f.OpenFile(Filename)

                For count = 0 To ((f.Length / 16) - 1)
                    Entries.Add(New ActHitCountTableDataInfoEntry(f.RawData(count * 16, 16)))
                Next
            End Using
            Return Task.CompletedTask
        End Function

        Public Sub New()
            MyBase.New
            Entries = New List(Of ActHitCountTableDataInfoEntry)
        End Sub
    End Class
End Namespace

