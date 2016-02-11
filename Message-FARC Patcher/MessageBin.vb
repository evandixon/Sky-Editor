''' <summary>
''' Models a .bin file in the message directory of PMD: GTI and PSMD.
''' </summary>
''' <remarks>Credit to psy_commando for researching the format.</remarks>
Public Class MessageBin
        Inherits Sir0

    Public Class StringEntry
        Public Property Entry As Byte()
        Public Property Hash As UInteger
        Public Property Unknown As UInteger
        Public Function GetStringBytes() As Byte()
            Return Entry
        End Function
    End Class

    ''' <summary>
    ''' Matches string hashes to the strings contained in the file.
    ''' </summary>
    ''' <returns>The games' scripts refer to the strings by this hash.</returns>
    Public Property Strings As List(Of StringEntry)

    Public Overrides Sub OpenFile(Filename As String)
        MyBase.OpenFile(Filename)

        ProcessData()
    End Sub
    Private Sub ProcessData()
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
            Dim stringData As New List(Of Byte)
            Dim doEnd As Boolean = False
            Do
                cRaw = RawData(stringPointer + j * 2, 2)
                If (Utilities.GenericArrayOperations(Of Byte).ArraysEqual(cRaw, {0, 0})) Then
                    doEnd = True
                End If
                stringData.AddRange(cRaw)
                j += 1
            Loop Until doEnd

            Strings.Add(New StringEntry With {.Hash = stringHash, .Entry = stringData.ToArray, .Unknown = unk})
        Next
    End Sub

    Public Overrides Sub Save()
        Me.RelativePointers.Clear()
        'Sir0 header pointers
        Me.RelativePointers.Add(4)
        Me.RelativePointers.Add(4)

        'Generate sections
        Dim stringSection As New List(Of Byte)
        Dim infoSection As New List(Of Byte)
        For Each item In From s In Strings Order By s.Hash Ascending
            infoSection.AddRange(BitConverter.GetBytes(16 + stringSection.Count))
            infoSection.AddRange(BitConverter.GetBytes(item.Hash))
            infoSection.AddRange(BitConverter.GetBytes(item.Unknown))
            stringSection.AddRange(item.GetStringBytes)
        Next

        'Add pointers
        Me.RelativePointers.Add(stringSection.Count + 8)
        For count = 0 To Strings.Count - 2
            Me.RelativePointers.Add(&HC)
        Next

        'Write sections to file
        Me.Length = 16 + stringSection.Count + infoSection.Count
        Me.RawData(16, stringSection.Count) = stringSection.ToArray
        Me.RawData(16 + stringSection.Count, infoSection.Count) = infoSection.ToArray

        'Update header
        Dim headerBytes As New List(Of Byte)
        headerBytes.AddRange(BitConverter.GetBytes(Strings.Count))
        headerBytes.AddRange(BitConverter.GetBytes(16 + stringSection.Count))
        Me.Header = headerBytes.ToArray
        Me.RelativePointers.Add(&H10)

        'Let the general SIR0 stuff happen
        MyBase.Save()
    End Sub

    Public Sub New()
        MyBase.New
        Strings = New List(Of StringEntry)
    End Sub
    Public Sub New(RawData As Byte())
        MyBase.New(RawData)
        Strings = New List(Of StringEntry)
        ProcessData()
    End Sub
End Class