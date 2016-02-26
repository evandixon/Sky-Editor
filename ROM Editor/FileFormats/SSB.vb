Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Namespace FileFormats
    Partial Class SSB
        Implements iOpenableFile

        Public Class CommandDefinition
            Public Property CommandID As Integer

            ''' <summary>
            ''' The length in 16 bit words of the command's parameters.
            ''' </summary>
            ''' <returns></returns>
            Public Property Length As Integer

            Public Property CommandName As String
                Get
                    If _commandName Is Nothing Then
                        Return "raw." & Conversion.Hex(CommandID).PadLeft(2, "0"c)
                    Else
                        Return _commandName
                    End If
                End Get
                Set(value As String)

                End Set
            End Property
            Dim _commandName As String

            Public Sub New(CommandID As Integer, ParamLength As Integer)
                Me.CommandID = CommandID
                Me.Length = ParamLength
            End Sub
        End Class

        Public Class Command
            Public Property CommandID As UInt16
            Public Property Params As List(Of UInt16)
            Public Sub New(RawData As Byte())
                CommandID = BitConverter.ToUInt16(RawData, 0)
                Dim p As New List(Of UInt16)
                For count = 2 To RawData.Length - 1 Step 2
                    p.Add(BitConverter.ToUInt16(RawData, count))
                Next
                Params = p
            End Sub
        End Class

        Public Class CommandGroup
            ''' <summary>
            ''' Points to a command in SSB.Commands
            ''' </summary>
            ''' <returns></returns>
            Public Property CommandNumber As Integer
            Public Property Type As Integer
            Public Property Unknown As Integer
        End Class

        Protected Shared Function GetSkyCommandDefinitionsDictionary() As Dictionary(Of Integer, CommandDefinition)
            Dim out As New Dictionary(Of Integer, CommandDefinition)
            For Each item In GetSkyCommandDefinitions()
                out.Add(item.CommandID, item)
            Next
            Return out
        End Function

        Public Property Groups As New List(Of CommandGroup)

        Public Property Commands As New List(Of Command)

        Public Property Strings As New Dictionary(Of Integer, String)

        Public Property Constants As New Dictionary(Of Integer, String)
        Public Property English As New Dictionary(Of Integer, String)
        Public Property French As New Dictionary(Of Integer, String)
        Public Property German As New Dictionary(Of Integer, String)
        Public Property Italian As New Dictionary(Of Integer, String)
        Public Property Spanish As New Dictionary(Of Integer, String)

        Public Sub OpenFile(Filename As String) Implements iOpenableFile.OpenFile
            Using f As New GenericFile(Filename, True)
                f.Position = 0
                Dim numConstants As Integer = f.NextUInt16
                Dim numStrings As Integer = f.NextUInt16

                'In Words
                'Absolute tring table offset = stringTableOffset * 2 + Header Size
                Dim stringTableOffset As Integer = f.NextUInt16
                Dim sizeConstants As Integer = f.NextUInt16 'In Words
                Dim sizeEnglish As Integer = f.NextUInt16
                Dim sizeFrench As Integer = 0
                Dim sizeGerman As Integer = 0
                Dim sizeItalian As Integer = 0
                Dim sizeSpanish As Integer = 0

                Dim isMultiLang As Boolean = False
                Dim dataBlockOffset As Integer = f.Position + 2

                'Todo: correct this invalid check
                If f.Length > (dataBlockOffset + f.Int16(dataBlockOffset) * 2 + sizeEnglish * 2 + f.Int16(dataBlockOffset) * 2) Then
                    'Then it's probably a multi-lang script.
                    isMultiLang = True
                End If

                If isMultiLang Then
                    sizeFrench = f.NextUInt16
                    sizeGerman = f.NextUInt16
                    sizeItalian = f.NextUInt16
                    sizeSpanish = f.NextUInt16
                    dataBlockOffset = f.Position
                Else
                    'Unknown value, possibly for an unused string table
                    f.Position += 2
                End If

                Dim dataWordLength = f.NextUInt16
                Dim numGroups = f.NextUInt16

                'Read Groups
                Dim groupPointerDictionary As New Dictionary(Of Integer, List(Of CommandGroup))
                Dim groupOffset = f.Position
                For count = 0 To numGroups - 1
                    Dim ptrScript As Integer = &HC + (f.NextUInt16) * 2
                    Dim type As Integer = f.Int16(f.NextUInt16)
                    Dim unknownGroupData As Integer = f.Int16(f.NextUInt16)

                    Dim g = New CommandGroup With {.Type = type, .Unknown = unknownGroupData}
                    Groups.Add(g)

                    'Log the groups by command pointer so we can later update the CommandNumber
                    If Not groupPointerDictionary.ContainsKey(ptrScript) Then
                        groupPointerDictionary.Add(ptrScript, New List(Of CommandGroup))
                    End If
                    groupPointerDictionary(ptrScript).Add(g)
                Next

                'Load commands
                Dim commandOffset = f.Position
                Dim currentCommandStart As Integer = commandOffset
                Dim commandDefs = GetSkyCommandDefinitionsDictionary()
                While currentCommandStart < dataWordLength * 2 + dataBlockOffset '(currentCommandStart + numWordsData)
                    'Update the relevant group's pointer
                    If groupPointerDictionary.ContainsKey(currentCommandStart) Then
                        For Each item In groupPointerDictionary(currentCommandStart)
                            item.CommandNumber = Commands.Count
                        Next
                    End If

                    'Read the command
                    Dim commandID As Integer = f.Int16(currentCommandStart)
                    Dim paramSize As Integer = commandDefs(commandID).Length * 2
                    Dim buffer(paramSize + 1) As Byte 'Large enough for the params and the 2 byte command id
                    Dim commandIDBytes = BitConverter.GetBytes(commandID)
                    buffer(0) = commandIDBytes(0)
                    buffer(1) = commandIDBytes(1)
                    For i = 2 To paramSize + 1
                        buffer(i) = f.RawData(currentCommandStart + i)
                    Next
                    Commands.Add(New Command(buffer))
                    currentCommandStart += paramSize + 2
                End While

                'Load Strings

                Dim stringStart = dataBlockOffset + stringTableOffset * 2
                LoadStringList(numConstants, stringStart, f, Constants)

                stringStart += sizeConstants * 2
                LoadStringList(numStrings, stringStart, f, English)

                If isMultiLang Then
                    stringStart = stringStart + sizeEnglish * 2
                    LoadStringList(numStrings, stringStart, f, French)

                    stringStart += sizeFrench * 2
                    LoadStringList(numStrings, stringStart, f, German)

                    stringStart += sizeGerman * 2
                    LoadStringList(numStrings, stringStart, f, Italian)

                    stringStart += sizeSpanish * 2
                    LoadStringList(numStrings, stringStart, f, Spanish)
                End If

                Console.Write("")
            End Using
        End Sub

        Private Sub LoadStringList(numStrings As Integer, stringStart As Integer, f As GenericFile, currentDictionary As Dictionary(Of Integer, String))
            Dim e = Text.Encoding.GetEncoding("Windows-1252")
            Dim currentString As New Text.StringBuilder
            Dim stringPosition As Integer = stringStart + 2 * numStrings
            Dim currentStringNumber = 1
            For count = 0 To numStrings - 1
                Dim c As Byte
                Do
                    c = f.RawData(stringPosition)
                    If c = 0 Then
                        currentDictionary.Add(currentStringNumber, currentString.ToString)
                        currentString = New Text.StringBuilder
                        currentStringNumber += 1
                    Else
                        currentString.Append(e.GetString({c}))
                    End If
                    stringPosition += 1
                Loop Until c = 0
            Next
        End Sub

        Private Sub OldOpenFile(Filename As String)
            Using f As New GenericFile(Filename, True)
                Dim numConstants As Integer = f.Int16(&H0)
                Dim numStrings As Integer = f.Int16(&H2)
                'Unknown: might not represent data as labled
                Dim stringTableDataWordOffset As Integer = f.Int16(&H4)
                Dim sizeConst As Integer = f.Int16(&H6)
                Dim sizeString As Integer = f.Int16(&H8)
                Dim unkown As Integer = f.Int16(&HA)
                'Start data partition
                Dim numWordsData As Integer = f.Int16(&HC)
                Dim numGroups As Integer = f.Int16(&HE)

                Dim groupPointerDictionary As New Dictionary(Of Integer, List(Of CommandGroup))

                'Load groups
                For count = 0 To numGroups - 1
                    Dim offset = count * 6 + &H10
                    Dim ptrScript As Integer = &HC + (f.Int16(offset + 0)) * 2
                    Dim type As Integer = f.Int16(offset + 2)
                    Dim unknownGroupData As Integer = f.Int16(offset + 2)

                    Dim g = New CommandGroup With {.Type = type, .Unknown = unkown}
                    Groups.Add(g)

                    'Log the groups by command pointer so we can later update the CommandNumber
                    If Not groupPointerDictionary.ContainsKey(ptrScript) Then
                        groupPointerDictionary.Add(ptrScript, New List(Of CommandGroup))
                    End If
                    groupPointerDictionary(ptrScript).Add(g)
                Next

                'Load commands
                Dim commandOffset = numGroups * 6 + &H10
                Dim currentCommandStart As Integer = commandOffset
                Dim dataLength As Integer = (numWordsData - numGroups * 3 - 2) * 2
                Dim commandDefs = GetSkyCommandDefinitionsDictionary()
                While currentCommandStart < dataLength + commandOffset '(currentCommandStart + numWordsData)
                    'Update the relevant group's pointer
                    If groupPointerDictionary.ContainsKey(currentCommandStart) Then
                        For Each item In groupPointerDictionary(currentCommandStart)
                            item.CommandNumber = Commands.Count
                        Next
                    End If

                    'Read the command
                    Dim commandID As Integer = f.Int16(currentCommandStart)
                    Dim paramSize As Integer = commandDefs(commandID).Length * 2
                    Dim buffer(paramSize + 1) As Byte 'Large enough for the params and the 2 byte command id
                    Dim commandIDBytes = BitConverter.GetBytes(commandID)
                    buffer(0) = commandIDBytes(0)
                    buffer(1) = commandIDBytes(1)
                    For i = 2 To paramSize + 1
                        buffer(i) = f.RawData(currentCommandStart + i)
                    Next
                    Commands.Add(New Command(buffer))
                    currentCommandStart += paramSize + 2
                End While

                'Load strings
                Dim stringTableOffset As Integer = (stringTableDataWordOffset - 1) * 2 + &HC
                Dim numStringTableEntries As Integer = f.Int16(stringTableOffset)
                Dim currentString As New Text.StringBuilder
                Dim currentStringOffset = stringTableOffset + 2
                Dim currentStringStart = currentStringOffset
                Dim e = Text.Encoding.GetEncoding("Windows-1252")
                'Read the null terminated strings
                While currentStringOffset < f.Length
                    If f.RawData(currentStringOffset) = 0 Then
                        Strings.Add(currentStringStart, currentString.ToString)
                        currentString = New Text.StringBuilder
                        currentStringStart = currentStringOffset + 1
                    Else
                        currentString.Append(e.GetString({f.RawData(currentStringOffset)}))
                    End If
                    currentStringOffset += 1
                End While
                Console.Write("")
            End Using
        End Sub
    End Class
End Namespace

