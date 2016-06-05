Imports System.IO
Imports ROMEditor.FileFormats.Explorers.Script.Commands
Imports SkyEditor.Core
Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.IO

Namespace FileFormats.Explorers.Script
    Partial Public Class SSB
        Implements INotifyModified
        Implements IOpenableFile
        Implements ISavableAs
        Implements IDetectableFileType
        Implements IOnDisk
        Implements iNamed

        ''' <summary>
        ''' Raised when the file starts saving
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Public Event FileSaving(sender As Object, e As EventArgs)

        ''' <summary>
        ''' Raised after the file has saved
        ''' </summary>
        Public Event FileSaved As ISavable.FileSavedEventHandler Implements ISavable.FileSaved

        ''' <summary>
        ''' Raised when the file is modified
        ''' </summary>
        Public Event Modified As INotifyModified.ModifiedEventHandler Implements INotifyModified.Modified

        Public Sub New()
            GotoTargetCommands = New List(Of RawCommand)
            CurrentCommandInfo = GetSkyCommandInfo()
        End Sub

#Region "Properties"
        Public Property Commands As New List(Of LogicalCommand)
        Public Property Constants As New List(Of String)
        Public Property English As New List(Of String)
        Public Property French As New List(Of String)
        Public Property German As New List(Of String)
        Public Property Italian As New List(Of String)
        Public Property Spanish As New List(Of String)
        Public Property isMultiLang As Boolean

        Public Property Filename As String Implements IOnDisk.Filename

        Public ReadOnly Property Name As String Implements iNamed.Name
            Get
                Return Path.GetFileName(Filename)
            End Get
        End Property

        ''' <summary>
        ''' A list of the indexes of the constants that have not been referenced by a known Command type.
        ''' We need to leave these in their current index in order to keep things from breaking.
        ''' </summary>
        ''' <returns></returns>
        Private Property UnreferencedConstantIndexes As List(Of Integer)

        ''' <summary>
        ''' A list of the indexes of the strings that have not been referenced by a known Command type.
        ''' We need to leave these in their current index in order to keep things from breaking.
        ''' </summary>
        ''' <returns></returns>
        Private Property UnreferencedStringIndexes As List(Of Integer)

        ''' <summary>
        ''' A list of the indexes of constants that have been temporarily cleared and can be used for other strings.
        ''' </summary>
        ''' <returns></returns>
        Private Property AvailableConstantIndexes As List(Of Integer)

        ''' <summary>
        ''' A list of the indexes of strings that have been temporarily cleared and can be used for other strings.
        ''' </summary>
        ''' <returns></returns>
        Private Property AvailableStringIndexes As List(Of Integer)

        ''' <summary>
        ''' A list of references to all commands with a GotoTarget property.
        ''' </summary>
        ''' <returns></returns>
        Private Property GotoTargetCommands As List(Of RawCommand)
#End Region

#Region "Functions"

#Region "IO"
        Public Async Function OpenFile(Filename As String, Provider As IOProvider) As Task Implements IOpenableFile.OpenFile
            Me.Filename = Filename
            Using f As New GenericFile
                f.IsReadOnly = True
                Await f.OpenFile(Filename, Provider)

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
                Dim groupDefinitions As New List(Of CommandGroup)
                Dim groupPointerDictionary As New Dictionary(Of Integer, List(Of CommandGroup))
                Dim groupOffset = f.Position
                For count = 0 To numGroups - 1
                    Dim ptrScript As Integer = &HC + f.NextUInt16 * 2
                    Dim type As Integer = f.NextUInt16
                    Dim unknownGroupData As Integer = f.NextUInt16

                    Dim g = New CommandGroup With {.Type = type, .Unknown = unknownGroupData}
                    groupDefinitions.Add(g)

                    'Log the groups by command pointer so we can later update the CommandNumber
                    If Not groupPointerDictionary.ContainsKey(ptrScript) Then
                        groupPointerDictionary.Add(ptrScript, New List(Of CommandGroup))
                    End If
                    groupPointerDictionary(ptrScript).Add(g)
                Next

                'Note the current position as the start of the command block
                Dim commandOffset = f.Position

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

                Dim rawCommands As New List(Of RawCommand)
                Dim commandLocations As New Dictionary(Of Integer, Integer) 'Key: index of the command in RawCommands. Value: index of the start of the Command, in Words.  
                Dim currentCommandStart As Integer = commandOffset
                Dim gotoPhysicalPointers As New List(Of Integer)
                While currentCommandStart < dataWordLength * 2 + dataBlockOffset '(currentCommandStart + numWordsData)
                    'Update the relevant group's pointer
                    If groupPointerDictionary.ContainsKey(currentCommandStart) Then
                        For Each item In groupPointerDictionary(currentCommandStart)
                            item.CommandNumber = rawCommands.Count
                        Next
                    End If

                    'Log where the command is located
                    If isMultiLang Then
                        commandLocations.Add(rawCommands.Count,
                                                (currentCommandStart - &H12) / 2) 'Index of the command in the file - header, then converted to Words, instead of bytes
                    Else
                        commandLocations.Add(rawCommands.Count,
                                                (currentCommandStart - &HC) / 2) 'Index of the command in the file - header, then converted to Words, instead of bytes
                    End If


                    'Read the command
                    Dim commandID As UInt16 = f.UInt16(currentCommandStart)
                    Dim paramSize As Integer = (From d In CurrentCommandInfo Where d.CommandID = commandID Select d.ParameterCount).First
                    Dim params As New List(Of UInt16)
                    For i = 1 To paramSize
                        params.Add(f.UInt16(currentCommandStart + i * 2))
                    Next
                    Dim newCmd = CreateCommand(commandID, params)
                    rawCommands.Add(newCmd)
                    currentCommandStart += paramSize * 2 + 2

                    'Determine if the given command has a goto target
                    If TypeOf newCmd Is RawCommand AndAlso GotoTargetCommands.Contains(newCmd) Then
                        'Then alter the relevant property to reference the label
                        For Each prop In newCmd.GetType.GetProperties
                            For Each attribute In prop.GetCustomAttributes(True)
                                If TypeOf attribute Is CommandParameterAttribute AndAlso prop.PropertyType.Equals(GetType(GotoTarget)) Then
                                    'We have an attribute on a GotoTarget.  Let's read the physical pointer, and log it so we can add a label later

                                    Dim physical = newCmd.Params(DirectCast(attribute, CommandParameterAttribute).Index)
                                    If Not gotoPhysicalPointers.Contains(physical) Then
                                        gotoPhysicalPointers.Add(physical)
                                    End If
                                End If
                            Next
                        Next
                    End If
                End While

                'Process pass 1 part 1 - processing Goto statements and Groups
                Dim processPass1 As New List(Of LogicalCommand)
                Dim gotoLabelIndexes As New Dictionary(Of Integer, Integer) 'Key: index of the target command in RawCommands. Value: index of the goto label in processPass1.
                Dim groupIndex = 0
                Dim gotoIndex = 0
                '-Add group and goto labels as logical commands
                For count = 0 To rawCommands.Count - 1
                    Dim count2 = count
                    Dim group = (From g In groupDefinitions Where g.CommandNumber = count2).FirstOrDefault
                    If group IsNot Nothing Then
                        'Then this is the start of a group
                        'Add a group start label
                        processPass1.Add(New Commands.GroupLabel With {.GroupIndex = groupIndex, .Type = group.Type, .Unknown = group.Unknown})
                        groupIndex += 1
                    End If

                    'If this command was referenced by a goto statement
                    If gotoPhysicalPointers.Contains(commandLocations(count)) Then
                        'Then add a goto label
                        processPass1.Add(New Commands.GotoLabel With {.Name = GetPass1GotoLabelName(gotoIndex)})
                        gotoLabelIndexes.Add(count, processPass1.Count - 1)
                        gotoIndex += 1
                    End If
                    'Add the original command
                    processPass1.Add(rawCommands(count))
                Next
                '-Change goto statements to point to a goto label, instead of a Word index
                For Each item In processPass1
                    If TypeOf item Is RawCommand AndAlso GotoTargetCommands.Contains(item) Then
                        'Then alter the relevant property to reference the label
                        For Each prop In item.GetType.GetProperties
                            For Each attribute In prop.GetCustomAttributes(True)
                                If TypeOf attribute Is CommandParameterAttribute AndAlso prop.PropertyType.Equals(GetType(GotoTarget)) Then
                                    'We have an attribute on a GotoTarget.  Let's set the value now.

                                    'Get the raw address of the target
                                    Dim rawAddress = DirectCast(item, RawCommand).Params(DirectCast(attribute, CommandParameterAttribute).Index)

                                    'Get the index of the command in RawCommands
                                    Dim rawCommandIndex = (From i In commandLocations Where i.Value = rawAddress Select i.Key).First

                                    'Get the label index
                                    Dim labelIndex As Integer = gotoLabelIndexes(rawCommandIndex)
                                    Dim target As New GotoTarget
                                    target.LabelName = GetPass1GotoLabelName(labelIndex)

                                    'Set the property
                                    prop.SetValue(item, target)
                                End If
                            Next
                        Next
                    End If
                Next

                Commands = processPass1

                'Todo: Pass 2 - Convert certain series of Goto statements into more human readable structures like If/ElseIf statements, Loops, Etc.
            End Using
        End Function

        Private Function GetPass1GotoLabelName(LabelIndex As Integer) As String
            Return $"Goto-{LabelIndex}"
        End Function

        Public Sub Save(Filename As String, provider As IOProvider) Implements ISavableAs.Save
            RaiseEvent FileSaving(Me, New EventArgs)

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

            'Todo when implemented in Open:
            'Command saving pass 2 - processing If/ElseIf, Loops, etc. into Goto statements.

            'Command saving pass 1 - processing Goto statements and Groups
            Dim rawCommands As New List(Of RawCommand)
            Dim rawCommandPointerTmp As Integer = 0
            Dim groupDefinitions As New List(Of CommandGroup)
            Dim gotoLabels As New Dictionary(Of String, Integer) 'Key: name of the label.  Value: index of the command it logically points to
            Dim gotoRawPointers As New Dictionary(Of Integer, Integer) 'Key: index of the command in rawCommands.  Value: Physical pointer
            Dim gotoCommandData As New Dictionary(Of RawCommand, Reflection.PropertyInfo)
            For count = 0 To Commands.Count - 1
                If TypeOf Commands(count) Is RawCommand Then
                    'If this is a raw command, then add it.
                    rawCommands.Add(Commands(count))

                    'Update the pointer tmp variable so we can know the physical location of commands that are pointed to
                    rawCommandPointerTmp += 1 + DirectCast(Commands(count), RawCommand).Params.Count 'Add the appropriate number of words

                    'Check to see if this has a GotoTarget parameter
                    For Each prop In Commands(count).GetType.GetProperties
                        For Each attribute In prop.GetCustomAttributes(True)
                            If TypeOf attribute Is CommandParameterAttribute AndAlso prop.PropertyType.Equals(GetType(GotoTarget)) Then
                                'If it does, log it so we can update it after we've processed all the labels
                                gotoCommandData.Add(Commands(count), prop)
                            End If
                        Next
                    Next
                ElseIf TypeOf Commands(count) Is GotoLabel Then
                    'If this is a goto label, log that its name and where it points, so we can update any goto statements
                    With DirectCast(Commands(count), GotoLabel)
                        gotoLabels.Add(.Name, rawCommands.Count)
                        gotoRawPointers.Add(rawCommands.Count, rawCommandPointerTmp)
                    End With
                    'Todo: maybe save the label names somewhere so they can be read later
                ElseIf TypeOf Commands(count) Is GroupLabel Then
                    'If this is a group definition, log it for future use
                    Dim g = DirectCast(Commands(count), GroupLabel)
                    groupDefinitions.Add(New CommandGroup With {.CommandNumber = rawCommands.Count, .Type = g.Type, .Unknown = g.Unknown})

                End If
            Next

            'Command saving pass 1 part 2 - update goto statements
            For Each item In gotoCommandData
                For Each attribute In item.Value.GetCustomAttributes(True)
                    If TypeOf attribute Is CommandParameterAttribute Then
                        With DirectCast(attribute, CommandParameterAttribute)
                            'Read the GotoTarget to get the label name, then get the index of the command it logically points to 
                            Dim logicalPointer = gotoLabels(DirectCast(item.Value.GetValue(item.Key), GotoTarget).LabelName)
                            Dim physicalPointer = gotoRawPointers(logicalPointer)
                            item.Key.Params(.Index) = physicalPointer
                        End With
                    End If
                Next
            Next

            'Commands saving pass 0 - converting commands and groups to byte code.
            Dim groupsSection As New List(Of Byte)
            Dim commandsSection As New List(Of Byte)
            Dim commandOffset = 2 + 3 * groupDefinitions.Count
            For count = 0 To rawCommands.Count - 1
                'Add the command to commandSection
                Dim item = rawCommands(count)

                Dim buffer = GetCommandBytes(item)
                commandsSection.AddRange(buffer)

                'Add the group if there's a group pointing to this command
                Dim c = count
                Dim group = (From g In groupDefinitions Where g.CommandNumber = c).FirstOrDefault

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
            data.AddRange(BitConverter.GetBytes(CUShort(groupDefinitions.Count))) 'numGroups
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

            provider.WriteAllBytes(Filename, out.ToArray)
            RaiseEvent FileSaved(Me, New EventArgs)
        End Sub

        Public Sub Save(provider As IOProvider) Implements ISavable.Save
            Save(Me.Filename, provider)
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

                'Replace null strings with empty strings, so they can still be encoded in the file
                If item Is Nothing Then
                    item = String.Empty
                End If

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

        Public Function IsOfType(File As GenericFile) As Task(Of Boolean) Implements IDetectableFileType.IsOfType
            'Todo: actually look at the file contents to verify its integrity
            Return Task.FromResult(File.OriginalFilename.ToLower.EndsWith(".ssb"))
        End Function

        Public Sub RaiseModified()
            RaiseEvent Modified(Me, New EventArgs)
        End Sub
#End Region


    End Class
End Namespace

