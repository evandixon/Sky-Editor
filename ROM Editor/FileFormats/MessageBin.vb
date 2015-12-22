Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Namespace FileFormats
    ''' <summary>
    ''' Models a .bin file in the message directory of PMD: GTI and PSMD.
    ''' </summary>
    ''' <remarks>Credit to psy_commando for researching the format.</remarks>
    Public Class MessageBin
        Implements iOpenableFile

        ''' <summary>
        ''' Matches string hashes to the strings contained in the file.
        ''' </summary>
        ''' <returns>The games' scripts refer to the strings by this hash.</returns>
        Public Property Strings As Dictionary(Of Integer, String)

        Public Sub OpenFile(Filename As String) Implements iOpenableFile.OpenFile
            Using f As New GenericFile(Filename, True)
                Dim subHeaderPointer As Integer = BitConverter.ToInt32(f.RawData(&H4, 4), 0)
                Dim pointerOffsetListPointer As Integer = BitConverter.ToInt32(f.RawData(&H8, 4), 0)

                Dim stringCount As Integer = BitConverter.ToInt32(f.RawData(subHeaderPointer + &H0, 4), 0)
                Dim stringInfoPointer As Integer = BitConverter.ToInt32(f.RawData(subHeaderPointer + &H4, 4), 0)

                For i = 0 To stringCount - 1
                    Dim stringPointer As Integer = BitConverter.ToInt32(f.RawData(stringInfoPointer + i * 12 + &H0, 4), 0)
                    Dim stringHash As Integer = BitConverter.ToInt32(f.RawData(stringInfoPointer + i * 12 + &H4, 4), 0)
                    Dim unk1 As UInt16 = BitConverter.ToUInt16(f.RawData(stringInfoPointer + i * 12 + &H8, 2), 0)
                    Dim unk2 As UInt16 = BitConverter.ToUInt16(f.RawData(stringInfoPointer + i * 12 + &HA, 2), 0)

                    Dim s As New Text.StringBuilder()
                    Dim e = Text.UnicodeEncoding.Unicode

                    'Parse the null-terminated UTF-16 string
                    Dim j As Integer = 0
                    Dim cRaw As Byte()
                    Do
                        cRaw = f.RawData(stringPointer + j * 2, 2)
                        Dim c = e.GetString(cRaw)
                        'TODO: parse escape characters, as described in these posts:
                        'http://projectpokemon.org/forums/showthread.php?46904-Pokemon-Super-Mystery-Dungeon-And-PMD-GTI-Research-And-Utilities&p=211018&viewfull=1#post211018
                        'http://projectpokemon.org/forums/showthread.php?46904-Pokemon-Super-Mystery-Dungeon-And-PMD-GTI-Research-And-Utilities&p=210986&viewfull=1#post210986
                        s.Append(c)
                        j += 1
                    Loop Until (SkyEditorBase.Utilities.GenericArrayOperations(Of Byte).ArraysEqual(cRaw, {0, 0}))

                    Strings.Add(stringHash, s.ToString)
                Next
            End Using
        End Sub

        Public Sub New()
            MyBase.New
            Strings = New Dictionary(Of Integer, String)
        End Sub
    End Class
End Namespace

