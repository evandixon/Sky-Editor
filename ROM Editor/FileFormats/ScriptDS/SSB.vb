Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Namespace FileFormats.ScriptDS
    Partial Public Class SSB
        Implements iOpenableFile
        Implements ISavableAs
        Implements iDetectableFileType
        Implements iOnDisk
        Implements iNamed

        Protected Shared Function GetSkyCommandDefinitionsDictionary() As Dictionary(Of Integer, CommandDefinition)
            Dim out As New Dictionary(Of Integer, CommandDefinition)
            For Each item In GetSkyCommandDefinitions()
                out.Add(item.CommandID, item)
            Next
            Return out
        End Function

        Public Property Groups As New List(Of CommandGroup)
        Public Property Commands As New List(Of Command)
        Public Property Constants As New Dictionary(Of Integer, String)
        Public Property English As New Dictionary(Of Integer, String)
        Public Property French As New Dictionary(Of Integer, String)
        Public Property German As New Dictionary(Of Integer, String)
        Public Property Italian As New Dictionary(Of Integer, String)
        Public Property Spanish As New Dictionary(Of Integer, String)
        Public Property isMultiLang As Boolean

        Public Property Filename As String Implements iOnDisk.Filename

        Public ReadOnly Property Name As String Implements iNamed.Name
            Get
                Return IO.Path.GetFileName(Filename)
            End Get
        End Property

        Public Function GetText() As String
            Dim out As New Text.StringBuilder
            Dim defs = GetSkyCommandDefinitionsDictionary()

            For count = 0 To Commands.Count - 1
                Dim c = count
                Dim q = From g In Groups Where g.CommandNumber = c

                If q.Any Then
                    Dim item = q.First
                    out.AppendLine($"Group{Conversion.Hex(Groups.IndexOf(item)).PadLeft(2, "0"c)}Type{Conversion.Hex(item.Type).PadLeft(2, "0"c)}Unk{Conversion.Hex(item.Unknown).PadLeft(2, "0"c)}:")
                End If
                out.Append("    ")
                out.AppendLine(Commands(count).GetScript(defs, Me))
            Next

            Return out.ToString
        End Function

        Public Sub OpenFile(Filename As String) Implements iOpenableFile.OpenFile
            Me.Filename = Filename
            Using f As New GenericFile
                f.IsReadOnly = True
                f.OpenFile(Filename)

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

                Dim dataBlockOffset As Integer = f.Position + 2

                'Todo: correct this invalid check
                If f.Length > (dataBlockOffset + f.Int16(dataBlockOffset) * 2 + sizeEnglish * 2 + f.Int16(dataBlockOffset) * 2) Then
                    'Then it's probably a multi-lang script.
                    isMultiLang = True
                Else
                    isMultiLang = False
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
                    Dim ptrScript As Integer = &HC + f.NextUInt16 * 2
                    Dim type As Integer = f.NextUInt16
                    Dim unknownGroupData As Integer = f.NextUInt16

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
                LoadStringList(numConstants, stringStart, f, Constants, True)

                stringStart = dataBlockOffset + stringTableOffset * 2 + sizeConstants * 2
                LoadStringList(numStrings, stringStart, f, English, False)

                If isMultiLang Then
                    stringStart = stringStart + sizeEnglish * 2
                    LoadStringList(numStrings, stringStart, f, French, False)

                    stringStart += sizeFrench * 2
                    LoadStringList(numStrings, stringStart, f, German, False)

                    stringStart += sizeGerman * 2
                    LoadStringList(numStrings, stringStart, f, Italian, False)

                    stringStart += sizeItalian * 2
                    LoadStringList(numStrings, stringStart, f, Spanish, False)
                End If

                Console.Write("")
            End Using
        End Sub

        Private Sub LoadStringList(numStrings As Integer, stringStart As Integer, f As GenericFile, currentDictionary As Dictionary(Of Integer, String), IsContantTable As Boolean)
            Dim e = Text.Encoding.GetEncoding("Windows-1252")
            Dim stringPosition As Integer = stringStart
            If Not IsContantTable Then
                stringPosition += 2 * numStrings
            End If
            For count = 0 To numStrings - 1
                Dim s = f.ReadNullTerminatedString(stringPosition, e)
                stringPosition += s.Length + 1
                currentDictionary.Add(count + 1, s)
            Next
        End Sub

        Public Event FileSaved As iSavable.FileSavedEventHandler Implements iSavable.FileSaved


        Public Function IsOfType(File As GenericFile) As Boolean Implements iDetectableFileType.IsOfType
            'Todo: actually look at the file contents to verify its integrity
            Return File.OriginalFilename.ToLower.EndsWith(".ssb")
        End Function

        Public Sub Save(Filename As String) Implements ISavableAs.Save
            'Commands and groups
            Dim groupsSection As New List(Of Byte)
            Dim commandsSection As New List(Of Byte)
            Dim commandOffset = 2 + 3 * Groups.Count
            For count = 0 To Commands.Count - 1
                'Add the command to commandSection
                Dim item = Commands(count)
                Dim buffer = item.GetBytes
                commandsSection.AddRange(buffer)

                'Add the group if there's a group pointing to this command
                Dim c = count
                Dim group = (From g In Groups Where g.CommandNumber = c).FirstOrDefault

                If group IsNot Nothing Then
                    groupsSection.AddRange(BitConverter.GetBytes(CUShort(commandOffset)))
                    groupsSection.AddRange(BitConverter.GetBytes(group.Type))
                    groupsSection.AddRange(BitConverter.GetBytes(group.Unknown))
                End If

                'Increment the commandOffset so future groups will function properly
                commandOffset += (buffer.Count / 2)
            Next

            'The data section
            Dim data As New List(Of Byte)
            data.AddRange(BitConverter.GetBytes(CUShort((groupsSection.Count + commandsSection.Count) / 2 + 2))) 'The number of words in groups and commands, plus the words for data length and numGroups.
            data.AddRange(BitConverter.GetBytes(CUShort(Groups.Count))) 'numGroups
            data.AddRange(groupsSection)
            data.AddRange(commandsSection)

            'Generate string tables
            Dim constantsTable As List(Of Byte) = GenerateStringTable(Constants, 0)
            Dim englishTable As List(Of Byte) = GenerateStringTable(English, constantsTable.Count / 2)
            Dim frenchTable As List(Of Byte) = GenerateStringTable(French, constantsTable.Count / 2)
            Dim germanTable As List(Of Byte) = GenerateStringTable(German, constantsTable.Count / 2)
            Dim italianTable As List(Of Byte) = GenerateStringTable(Italian, constantsTable.Count / 2)
            Dim spanishTable As List(Of Byte) = GenerateStringTable(Spanish, constantsTable.Count / 2)

            Dim header As New List(Of Byte)
            header.AddRange(BitConverter.GetBytes(CUShort(Constants.Keys.Count)))
            header.AddRange(BitConverter.GetBytes(CUShort(English.Keys.Count)))
            'Offset const
            If Constants.Keys.Count = 0 Then
                'Point to the first string table
                Dim offset As UShort = data.Count / 2
                header.AddRange(BitConverter.GetBytes(offset))
            Else
                'Point to the first constant in the constant table, skipping the pointers in the same table.
                'I don't know why, but that's just how the game works.
                Dim offset As UShort = data.Count / 2 + Constants.Keys.Count * 2
                header.AddRange(BitConverter.GetBytes(offset))
            End If
            header.AddRange(BitConverter.GetBytes(CUShort(constantsTable.Count / 2)))
            header.AddRange(BitConverter.GetBytes(CUShort(englishTable.Count / 2)))
            If isMultiLang Then
                header.AddRange(BitConverter.GetBytes(CUShort(frenchTable.Count / 2)))
                header.AddRange(BitConverter.GetBytes(CUShort(germanTable.Count / 2)))
                header.AddRange(BitConverter.GetBytes(CUShort(italianTable.Count / 2)))
                header.AddRange(BitConverter.GetBytes(CUShort(spanishTable.Count / 2)))
            Else
                'Todo: figure out this unknown value
                header.AddRange({0, 0})
            End If

            'Combine everything
            Dim out As New List(Of Byte)
            out.AddRange(header)
            out.AddRange(data)
            out.AddRange(constantsTable)
            out.AddRange(englishTable)
            out.AddRange(frenchTable)
            out.AddRange(germanTable)
            out.AddRange(italianTable)
            out.AddRange(spanishTable)
            IO.File.WriteAllBytes(Filename, out.ToArray)
            RaiseEvent FileSaved(Me, New EventArgs)
        End Sub
        Private Function GenerateStringTable(SourceDictionary As Dictionary(Of Integer, String), sizeConstantsWords As UShort) As List(Of Byte)
            Dim e = Text.Encoding.GetEncoding("Windows-1252")
            Dim stringSection As New List(Of Byte)
            Dim pointerSection As New List(Of Byte)
            Dim offset As UShort = SourceDictionary.Values.Count * 2 + sizeConstantsWords * 2 'Looks like this is the only thing in Bytes and not Words.
            For Each item In SourceDictionary.Values
                pointerSection.AddRange(BitConverter.GetBytes(offset))
                Dim buffer = e.GetBytes(item.Replace(vbCrLf, vbLf))
                stringSection.AddRange(buffer)
                stringSection.Add(0)
                offset += buffer.Length + 1
            Next
            Dim out As New List(Of Byte)
            out.AddRange(pointerSection)
            out.AddRange(stringSection)
            Return out
        End Function

        Public Function DefaultExtension() As String Implements ISavableAs.DefaultExtension
            Return ".ssb"
        End Function

        Public Sub Save() Implements iSavable.Save
            Save(Me.Filename)
        End Sub
    End Class
End Namespace

