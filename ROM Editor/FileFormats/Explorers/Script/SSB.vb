Imports ROMEditor.FileFormats.Explorers.Script.Commands
Imports SkyEditor.Core
Imports SkyEditor.Core.Interfaces
Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Namespace FileFormats.Explorers.Script
    Partial Public Class SSB
        Implements iOpenableFile
        Implements ISavableAs
        Implements iDetectableFileType
        Implements iOnDisk
        Implements iNamed

        Public Event FileSaved As iSavable.FileSavedEventHandler Implements iSavable.FileSaved

#Region "Properties"
        Public Property Groups As New List(Of CommandGroup)
        Public Property Commands As New ObjectModel.ObservableCollection(Of RawCommand)
        Public Property Constants As New List(Of String)
        Public Property English As New List(Of String)
        Public Property French As New List(Of String)
        Public Property German As New List(Of String)
        Public Property Italian As New List(Of String)
        Public Property Spanish As New List(Of String)
        Public Property isMultiLang As Boolean

        Public Property Filename As String Implements iOnDisk.Filename

        Public ReadOnly Property Name As String Implements iNamed.Name
            Get
                Return IO.Path.GetFileName(Filename)
            End Get
        End Property
#End Region

#Region "Functions"

#Region "IO"
        Public Sub OpenFile(Filename As String) Implements iOpenableFile.OpenFile
            Me.Filename = Filename
            Using f As New SkyEditor.Core.Windows.GenericFile
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

                'Load Strings
                ''Given this implementation, it's important to load the strings first, because the commands below directly reference these strings.
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

                UnreferencedConstantIndexes = New List(Of Integer)
                For count = 0 To Constants.Count - 1
                    UnreferencedConstantIndexes.Add(count)
                Next

                UnreferencedStringIndexes = New List(Of Integer)
                For count = 0 To English.Count - 1
                    UnreferencedStringIndexes.Add(count)
                Next

                'Load commands
                Dim commandOffset = f.Position
                Dim currentCommandStart As Integer = commandOffset
                Dim commandDefs = GetSkyCommandParamLengths()
                While currentCommandStart < dataWordLength * 2 + dataBlockOffset '(currentCommandStart + numWordsData)
                    'Update the relevant group's pointer
                    If groupPointerDictionary.ContainsKey(currentCommandStart) Then
                        For Each item In groupPointerDictionary(currentCommandStart)
                            item.CommandNumber = Commands.Count
                        Next
                    End If

                    'Read the command
                    Dim commandID As UInt16 = f.UInt16(currentCommandStart)
                    Dim paramSize As Integer = commandDefs(commandID)
                    Dim params As New List(Of UInt16)
                    For i = 1 To paramSize
                        params.Add(f.UInt16(currentCommandStart + i * 2))
                    Next
                    Commands.Add(CreateSkyCommand(commandID, params))
                    currentCommandStart += paramSize * 2 + 2
                End While

            End Using
        End Sub

        Public Sub Save(Filename As String) Implements ISavableAs.Save
            'Preprocess the constants and strings
            'The function that converts commands into byte arrays will add strings to the proper tables where appropriate
            'However, not all commands are known, and some could be referencing strings or constants we're not aware of.
            'We need to keep those in place just in case, and we can remove the rest.

            AvailableConstantIndexes = New List(Of Integer)
            AvailableStringIndexes = New List(Of Integer)

            '-Remove items until the first unreferenced string
            Dim constantRemove As Boolean = True
            For count = Constants.Count - 1 To 0 Step -1
                If constantRemove Then
                    'If we can remove constants
                    If UnreferencedConstantIndexes.Contains(count) Then
                        constantRemove = False
                    Else
                        Constants.RemoveAt(count)
                    End If
                Else
                    'If not, clear the referenced ones to save space
                    If Not UnreferencedConstantIndexes.Contains(count) Then
                        Constants(count) = ""
                        AvailableConstantIndexes.Add(count)
                    End If
                End If
            Next
            '-Do the same with the other languages
            Dim stringRemove As Boolean = True
            For count = English.Count - 1 To 0 Step -1
                If stringRemove Then
                    'If we can remove strings
                    If UnreferencedStringIndexes.Contains(count) Then
                        stringRemove = False
                    Else
                        English.RemoveAt(count)
                        If isMultiLang Then
                            French.RemoveAt(count)
                            German.RemoveAt(count)
                            Italian.RemoveAt(count)
                            Spanish.RemoveAt(count)
                        End If
                    End If
                Else
                    'If not, clear the referenced ones to save space
                    If Not UnreferencedStringIndexes.Contains(count) Then
                        English(count) = ""
                        If isMultiLang Then
                            French(count) = ""
                            German(count) = ""
                            Italian(count) = ""
                            Spanish(count) = ""
                        End If
                        AvailableStringIndexes.Add(count)
                    End If
                End If
            Next

            'Commands and groups
            Dim groupsSection As New List(Of Byte)
            Dim commandsSection As New List(Of Byte)
            Dim commandOffset = 2 + 3 * Groups.Count
            For count = 0 To Commands.Count - 1
                'Add the command to commandSection
                Dim item = Commands(count)

                Dim buffer = GetSkyCommandBytes(item)
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
            header.AddRange(BitConverter.GetBytes(CUShort(Constants.Count)))
            header.AddRange(BitConverter.GetBytes(CUShort(English.Count)))
            'Offset const
            If Constants.Count = 0 Then
                'Point to the first string table
                Dim offset As UShort = data.Count / 2
                header.AddRange(BitConverter.GetBytes(offset))
            Else
                'Point to the first constant in the constant table, skipping the pointers in the same table.
                'I don't know why, but that's just how the game works.
                Dim offset As UShort = data.Count / 2 + Constants.Count * 2
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

        Public Sub Save() Implements iSavable.Save
            Save(Me.Filename)
        End Sub

        Public Function GetDefaultExtension() As String Implements ISavableAs.GetDefaultExtension
            Return ".ssb"
        End Function
#End Region

        Private Sub LoadStringList(numStrings As Integer, stringStart As Integer, f As GenericFile, currentDictionary As List(Of String), IsContantTable As Boolean)
            Dim e = Text.Encoding.GetEncoding("Windows-1252")
            Dim stringPosition As Integer = stringStart
            If Not IsContantTable Then
                stringPosition += 2 * numStrings
            End If
            For count = 0 To numStrings - 1
                Dim s = f.ReadNullTerminatedString(stringPosition, e)
                stringPosition += s.Length + 1
                currentDictionary.Add(s)
            Next
        End Sub


        Private Function GenerateStringTable(SourceDictionary As List(Of String), sizeConstantsWords As UShort) As List(Of Byte)
            Dim e = Text.Encoding.GetEncoding("Windows-1252")
            Dim stringSection As New List(Of Byte)
            Dim pointerSection As New List(Of Byte)
            Dim offset As UShort = SourceDictionary.Count * 2 + sizeConstantsWords * 2 'Looks like this is the only thing in Bytes and not Words.
            For Each item In SourceDictionary
                pointerSection.AddRange(BitConverter.GetBytes(offset))
                Dim buffer = e.GetBytes(item.Replace(vbCrLf, vbLf))
                stringSection.AddRange(buffer)
                stringSection.Add(0)
                offset += buffer.Length + 1
            Next

            'Pad the string section so its length is divisible by 2.
            If stringSection.Count Mod 2 = 1 Then
                stringSection.Add(0)
            End If

            Dim out As New List(Of Byte)
            out.AddRange(pointerSection)
            out.AddRange(stringSection)
            Return out
        End Function

        Public Function IsOfType(File As GenericFile) As Boolean Implements iDetectableFileType.IsOfType
            'Todo: actually look at the file contents to verify its integrity
            Return File.OriginalFilename.ToLower.EndsWith(".ssb")
        End Function
#End Region


    End Class
End Namespace

