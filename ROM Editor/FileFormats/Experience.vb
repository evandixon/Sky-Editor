Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Namespace FileFormats
    Public Class Experience
        Implements iOpenableFile

        Public Class ExperienceEntry
            Public Property Exp As UInteger
            Public Property AddedHP As Byte
            Public Property AddedAttack As Byte
            Public Property AddedDefense As Byte
            Public Property AddedSpAttack As Byte
            Public Property AddedSpDefense As Byte
            Public Property AddedSpeed As Byte
            Public Sub New(RawData As Byte())
                Exp = BitConverter.ToUInt32(RawData, 0)
                AddedHP = RawData(5)
                AddedAttack = RawData(6)
                AddedDefense = RawData(7)
                AddedSpAttack = RawData(8)
                AddedSpDefense = RawData(9)
                AddedSpeed = RawData(10)
            End Sub
        End Class

        Public Property Entries As Dictionary(Of Integer, List(Of ExperienceEntry))

        Public Sub OpenFile(Filename As String) Implements iOpenableFile.OpenFile
            Const entryLength = &HC
            Const tableLength = &H4C0
            Using f As New GenericFile
                f.IsReadOnly = True
                f.OpenFile(Filename)
                For tableCount = 0 To (f.Length / tableLength)
                    Dim localEntries As New List(Of ExperienceEntry)
                    For entryCount = 0 To 99 '100 entries
                        localEntries.Add(New ExperienceEntry(f.RawData(tableCount * tableLength + entryCount * entryLength, entryLength)))
                    Next
                    Entries.Add(tableCount, localEntries)
                Next

            End Using
        End Sub

        Public Sub New()
            Entries = New Dictionary(Of Integer, List(Of ExperienceEntry))
        End Sub
    End Class

End Namespace
