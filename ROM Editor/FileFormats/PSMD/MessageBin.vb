Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.Utilities
Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Namespace FileFormats.PSMD
    ''' <summary>
    ''' Models a .bin file in the message directory of PMD: GTI and PSMD.
    ''' </summary>
    ''' <remarks>Credit to psy_commando for researching the format.</remarks>
    Public Class MessageBin
        Inherits Sir0
        Implements iOpenableFile
        Implements ComponentModel.INotifyPropertyChanged
        Implements iModifiable

        Public Class EntryAddedEventArgs
            Inherits EventArgs

            Public Property NewID As UInteger
        End Class

        ''' <summary>
        ''' Matches string hashes to the strings contained in the file.
        ''' </summary>
        ''' <returns>The games' scripts refer to the strings by this hash.</returns>
        Public Property Strings As ObservableCollection(Of MessageBinStringEntry) ' Dictionary(Of Integer, String)

        Public Sub AddBlankEntry(ID As UInteger)
            Dim newEntry = New MessageBinStringEntry With {.Hash = ID}
            AddHandler newEntry.PropertyChanged, AddressOf Entry_PropertyChanged
            Strings.Add(newEntry)
            RaiseModified()
            RaiseEvent EntryAdded(Me, New EntryAddedEventArgs With {.NewID = ID})
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Strings)))
        End Sub

        Public Overrides Sub CreateFile(Name As String, FileContents() As Byte)
            MyBase.CreateFile(Name, FileContents)

            ProcessData()
        End Sub

        Public Overrides Sub OpenFile(Filename As String) Implements iOpenableFile.OpenFile
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
                Dim doEnd As Boolean = False
                Do
                    cRaw = RawData(stringPointer + j * 2, 2)

                    'TODO: parse escape characters, as described in these posts:
                    'http://projectpokemon.org/forums/showthread.php?46904-Pokemon-Super-Mystery-Dungeon-And-PMD-GTI-Research-And-Utilities&p=211018&viewfull=1#post211018
                    'http://projectpokemon.org/forums/showthread.php?46904-Pokemon-Super-Mystery-Dungeon-And-PMD-GTI-Research-And-Utilities&p=210986&viewfull=1#post210986

                    If cRaw(1) >= 128 Then 'Most significant bit is set
                        s.Append("\")
                        s.Append(Conversion.Hex(cRaw(1)).PadLeft(2, "0"c))
                        s.Append(Conversion.Hex(cRaw(0)).PadLeft(2, "0"c))
                        j += 1
                    Else
                        Dim c = e.GetString(cRaw)

                        If (GenericArrayOperations(Of Byte).ArraysEqual(cRaw, {0, 0})) Then
                            doEnd = True
                        Else
                            s.Append(c)
                            j += 1
                        End If
                    End If

                Loop Until doEnd

                Dim newEntry = New MessageBinStringEntry With {.Hash = stringHash, .Entry = s.ToString.Trim, .Unknown = unk, .Pointer = stringPointer}
                AddHandler newEntry.PropertyChanged, AddressOf Entry_PropertyChanged
                Strings.Add(newEntry)
            Next
        End Sub

        Public Sub OpenFileOnlyIDs(Filename As String)
            MyBase.OpenFile(Filename)

            Dim stringCount As Integer = BitConverter.ToInt32(Header, 0)
            Dim stringInfoPointer As Integer = BitConverter.ToInt32(Header, 4)

            For i = 0 To stringCount - 1
                Dim stringPointer As Integer = BitConverter.ToInt32(RawData(stringInfoPointer + i * 12 + &H0, 4), 0)
                Dim stringHash As UInteger = BitConverter.ToUInt32(RawData(stringInfoPointer + i * 12 + &H4, 4), 0)
                Dim unk As UInt32 = BitConverter.ToUInt32(RawData(stringInfoPointer + i * 12 + &H8, 4), 0)

                'We're skipping reading the string, since this function only loads the IDs

                Dim newEntry = New MessageBinStringEntry With {.Hash = stringHash, .Entry = "", .Unknown = unk}
                AddHandler newEntry.PropertyChanged, AddressOf Entry_PropertyChanged
                Strings.Add(newEntry)
            Next
        End Sub

        Private Sub Entry_PropertyChanged(sender As Object, e As EventArgs)
            If Strings.Contains(sender) Then
                RaiseModified()
            End If
        End Sub

        Public Overrides Sub Save(Destination As String)
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
            MyBase.Save(Destination)
        End Sub

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
        Public Event EntryAdded(sender As Object, e As EntryAddedEventArgs)
        Public Event Modified As iModifiable.ModifiedEventHandler Implements iModifiable.Modified

        Public Sub New()
            MyBase.New
            Strings = New ObservableCollection(Of MessageBinStringEntry)
        End Sub

        Public Sub New(OpenReadOnly As Boolean)
            Me.New
            IsReadOnly = OpenReadOnly
            Strings = New ObservableCollection(Of MessageBinStringEntry)
        End Sub

        Public Sub RaiseModified() Implements iModifiable.RaiseModified
            RaiseEvent Modified(Me, New EventArgs)
        End Sub
    End Class
End Namespace

