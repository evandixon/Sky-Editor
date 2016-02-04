Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Namespace FileFormats
    ''' <summary>
    ''' Models a .bin file in the message directory of PMD: GTI and PSMD.
    ''' </summary>
    ''' <remarks>Credit to psy_commando for researching the format.</remarks>
    Public Class MessageBin
        Inherits Sir0
        Implements iOpenableFile

        Public Class StringEntry
            Public Property Entry As String
            Public Property Hash As UInteger
            Public Property Unknown As UInteger
            Public Overrides Function ToString() As String
                Return BitConverter.ToInt32(BitConverter.GetBytes(Hash), 0).ToString & ": " & Entry
            End Function
        End Class

        ''' <summary>
        ''' Matches string hashes to the strings contained in the file.
        ''' </summary>
        ''' <returns>The games' scripts refer to the strings by this hash.</returns>
        Public Property Strings As List(Of StringEntry) ' Dictionary(Of Integer, String)

        Public Overrides Sub OpenFile(Filename As String) Implements iOpenableFile.OpenFile
            MyBase.OpenFile(Filename)

            Dim stringCount As Integer = BitConverter.ToInt32(Header, 0)
            Dim stringInfoPointer As Integer = BitConverter.ToInt32(Header, 4)

            For i = 0 To stringCount - 1
                Dim stringPointer As Integer = BitConverter.ToInt32(RawData(stringInfoPointer + i * 12 + &H0, 4), 0)
                Dim stringHash As UInteger = BitConverter.ToUInt32(RawData(stringInfoPointer + i * 12 + &H4, 4), 0)
                Dim unk As UInt32 = BitConverter.ToUInt32(RawData(stringInfoPointer + i * 12 + &H8, 4), 0)

                Dim s As New Text.StringBuilder()
                Dim e = Text.UnicodeEncoding.Unicode

                'Parse the null-terminated UTF-16 string
                Dim j As Integer = 0
                Dim cRaw As Byte()
                Dim doEnd As Boolean = False
                Do
                    cRaw = RawData(stringPointer + j * 2, 2)
                    Dim c = e.GetString(cRaw)
                    'TODO: parse escape characters, as described in these posts:
                    'http://projectpokemon.org/forums/showthread.php?46904-Pokemon-Super-Mystery-Dungeon-And-PMD-GTI-Research-And-Utilities&p=211018&viewfull=1#post211018
                    'http://projectpokemon.org/forums/showthread.php?46904-Pokemon-Super-Mystery-Dungeon-And-PMD-GTI-Research-And-Utilities&p=210986&viewfull=1#post210986
                    If (SkyEditorBase.Utilities.GenericArrayOperations(Of Byte).ArraysEqual(cRaw, {0, 0})) Then
                        doEnd = True
                    Else
                        s.Append(c)
                        j += 1
                    End If
                Loop Until doEnd

                Strings.Add(New StringEntry With {.Hash = stringHash, .Entry = s.ToString.Trim, .Unknown = unk})
            Next
        End Sub

        Public Overrides Sub Save(Destination As String)
            MyBase.Save(Destination)
        End Sub

        Public Sub New()
            MyBase.New
            Strings = New List(Of StringEntry)
        End Sub
    End Class
End Namespace

