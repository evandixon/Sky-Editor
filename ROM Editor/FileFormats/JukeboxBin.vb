Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Namespace FileFormats
    Public Class JukeboxBin
        Implements iOpenableFile

        Public Class JukeboxEntry
            Public Property Filename As String
            Public Property Unk2 As String
            Public Property Unk3 As String
            Public Property UnlockCriteria As String
            Public Property unk5 As Integer
            Public Property unk6 As Integer
            Public Property unk7 As Integer
            Public Property unk8 As Integer
            Public Property unk9 As Integer
        End Class

        Public Property Entries As List(Of JukeboxEntry)


        Public Sub OpenFile(Filename As String) Implements iOpenableFile.OpenFile
            Using f As New GenericFile(Filename, True)
                Dim subHeaderPointer = f.Int32(4)
                Dim jukeboxPointerOffset = f.Int32(subHeaderPointer + 0)
                Dim numEntries = f.Int32(subHeaderPointer + 4)

                For count = 0 To numEntries - 1
                    Dim filenamePointer As Integer = f.Int32(jukeboxPointerOffset + count * 36 + 0)
                    Dim u2 As Integer = f.Int32(jukeboxPointerOffset + count * 36 + 4)
                    Dim u3 As Integer = f.Int32(jukeboxPointerOffset + count * 36 + 8)
                    Dim unlockPointer As Integer = f.Int32(jukeboxPointerOffset + count * 36 + 12)
                    Dim unk5 As Integer = f.Int32(jukeboxPointerOffset + count * 36 + 16)
                    Dim unk6 As Integer = f.Int32(jukeboxPointerOffset + count * 36 + 20)
                    Dim unk7 As Integer = f.Int32(jukeboxPointerOffset + count * 36 + 24)
                    Dim unk8 As Integer = f.Int32(jukeboxPointerOffset + count * 36 + 28)
                    Dim unk9 As Integer = f.Int32(jukeboxPointerOffset + count * 36 + 32)

                    Dim e As New JukeboxEntry
                    e.Filename = f.ReadUnicodeString(filenamePointer)
                    e.Unk2 = f.ReadUnicodeString(u2)
                    e.Unk3 = f.ReadUnicodeString(u3)
                    e.UnlockCriteria = f.ReadUnicodeString(unlockPointer)
                    e.unk5 = unk5
                    e.unk6 = unk6
                    e.unk7 = unk7
                    e.unk8 = unk8
                    e.unk9 = unk9

                    Entries.Add(e)
                Next
            End Using
        End Sub

        Public Sub New()
            MyBase.New
            Entries = New List(Of JukeboxEntry)
        End Sub
    End Class

End Namespace
