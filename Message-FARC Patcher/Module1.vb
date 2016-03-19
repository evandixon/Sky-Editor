Imports System.Web.Script.Serialization

Module Module1

    Sub Main()
        Dim Version = Environment.Version.ToString(2)
        Console.WriteLine($"Pokemon Super Mystery Dungeon Message FARC Type 5 Patcher v{Version}")
        Console.WriteLine()
        Dim args = Environment.GetCommandLineArgs
        If args.Length >= 4 Then
            Dim mode As String = args(1)
            Select Case mode.ToLower
                Case "-c"
                    Dim inputFile As String = args(2)
                    Dim editedFile As String = args(3)
                    Dim patchFile As String = args(4)

                    Dim sourceFarc As New FarcF5
                    sourceFarc.OpenFile(inputFile)

                    Dim editedFarc As New FarcF5
                    editedFarc.OpenFile(editedFile)

                    'Dim report As New Text.StringBuilder

                    Dim patches As New PatchList
                    Dim u = Text.UnicodeEncoding.Unicode

                    For Each edited In editedFarc.Header.FileData
                        Dim sourceDataItemHeader = (From d In sourceFarc.Header.FileData Where d.FilenamePointer = edited.FilenamePointer).FirstOrDefault
                        If sourceDataItemHeader IsNot Nothing Then
                            Using editedMsg As New MessageBin(editedFarc.GetFileData(edited.Index))
                                Using sourceMsg As New MessageBin(sourceFarc.GetFileData(sourceDataItemHeader.Index))
                                    'We've now opened two corresponding message bin files from the source and target FARC files.
                                    'Now to check to see if any strings are different, using a compare similar to outside these using statements

                                    For Each editedStringEntry In editedMsg.Strings
                                        Dim sourceStringEntry As MessageBin.StringEntry = Nothing '= (From s In sourceMsg.Strings Where s.Hash = editedStringEntry.Hash).FirstOrDefault
                                        For Each item In sourceMsg.Strings
                                            If item.Hash = editedStringEntry.Hash Then
                                                sourceStringEntry = item
                                                Exit For
                                            End If
                                        Next
                                        If sourceStringEntry IsNot Nothing Then
                                            'We've found corresponding strings.  NOW for the actual compare

                                            If Not Utilities.GenericArrayOperations(Of Byte).ArraysEqual(sourceStringEntry.Entry, editedStringEntry.Entry) Then
                                                'Two string entries are different.  Log it.

                                                patches.Patches.Add(New PatchList.PatchItem With {.FarcFileHash = edited.FilenamePointer, .StringID = editedStringEntry.Hash, .StringData = editedStringEntry.Entry})

                                                'report.AppendLine(String.Format("Difference in file {0}, entry {1}:", edited.FilenamePointer, sourceStringEntry.Hash))
                                                'report.AppendLine("Source: ")
                                                'report.AppendLine(u.GetString(sourceStringEntry.Entry))
                                                'report.AppendLine()
                                                'report.AppendLine("Edited: ")
                                                'report.AppendLine(u.GetString(editedStringEntry.Entry))
                                                'report.AppendLine("-----------------------------------")
                                            End If
                                        Else
                                            'There's an entry in the edited file that's not in the source.

                                            patches.Patches.Add(New PatchList.PatchItem With {.FarcFileHash = edited.FilenamePointer, .StringID = editedStringEntry.Hash, .StringData = editedStringEntry.Entry})
                                            'report.AppendLine(String.Format("Addition to file {0}, entry {1}:", edited.FilenamePointer, editedStringEntry.Hash))
                                            'report.AppendLine("New Entry: ")
                                            'report.AppendLine(u.GetString(editedStringEntry.Entry))
                                            'report.AppendLine("-----------------------------------")
                                        End If
                                    Next


                                End Using
                            End Using
                        Else
                            'This means we're going to add the WHOLE message file

                            Using editedMsg As New MessageBin(editedFarc.GetFileData(edited.FilenamePointer))
                                'report.AppendLine(String.Format("New file {0}", edited.FilenamePointer))
                                For Each editedStringEntry In editedMsg.Strings
                                    patches.Patches.Add(New PatchList.PatchItem With {.FarcFileHash = edited.FilenamePointer, .StringID = editedStringEntry.Hash, .StringData = editedStringEntry.Entry})
                                    'report.AppendLine(String.Format("Addition {0}:", editedStringEntry.Hash))
                                    'report.AppendLine(u.GetString(editedStringEntry.Entry))
                                    'report.AppendLine("-----------------------------------")
                                Next
                                'report.AppendLine("-----------------------------------")
                            End Using
                        End If
                    Next

                    Dim j As New JavaScriptSerializer
                    IO.File.WriteAllText(patchFile, j.Serialize(patches))
                    'IO.File.WriteAllText("report.txt", report.ToString)
                    sourceFarc.Dispose()
                    editedFarc.Dispose()
                Case "-a"
                    Dim inputFile As String = args(2)
                    Dim patchFile As String = args(3)
                    Dim editedFile As String = args(4)

                    Dim patches As PatchList
                    Dim j As New JavaScriptSerializer
                    patches = j.Deserialize(Of PatchList)(IO.File.ReadAllText(patchFile))

                    If Not IO.Directory.Exists("farcExtractTemp") Then
                        IO.Directory.CreateDirectory("farcExtractTemp")
                    End If

                    Using sourceFarc As New FarcF5
                        sourceFarc.OpenFile(inputFile)
                        sourceFarc.Extract("farcExtractTemp")
                    End Using

                    Dim patchesSorted As New Dictionary(Of UInteger, List(Of PatchList.PatchItem))

                    'Sort the patches based on the FARC file, so we only open as many files as we need to (ie. not reopening the same file 30 times)
                    For Each item In patches.Patches
                        If Not patchesSorted.ContainsKey(item.FarcFileHash) Then
                            patchesSorted.Add(item.FarcFileHash, New List(Of PatchList.PatchItem))
                        End If
                        patchesSorted(item.FarcFileHash).Add(item)
                    Next

                    'Apply the patches
                    For Each item In patchesSorted.Keys
                        Using msg As New MessageBin
                            msg.OpenFile(IO.Path.Combine("farcExtractTemp", Conversion.Hex(item).PadLeft(8, "0"c)))

                            For Each patch In patchesSorted(item)
                                Dim target = (From m In msg.Strings Where m.Hash = patch.StringID).FirstOrDefault
                                If target Is Nothing Then
                                    msg.Strings.Add(New MessageBin.StringEntry With {.Hash = patch.StringID, .Entry = patch.StringData})
                                Else
                                    target.Entry = patch.StringData
                                End If
                            Next

                            msg.Save()
                        End Using
                    Next

                    'Repack the farc
                    FarcF5.Pack("farcExtractTemp", editedFile)

                    IO.Directory.Delete("farcExtractTemp", True)
                Case Else
                    PrintUsage()
            End Select
        Else
            PrintUsage()
        End If
    End Sub

    Sub PrintUsage()
        Console.WriteLine("Usage:")
        Console.WriteLine("To create a patch: MessageFarcPatcher.exe -c <originalFile> <editedFile> <patchFile>")
        Console.WriteLine("To apply a patch:  MessageFarcPatcher.exe -a <originalFile> <patchFile> <editedFile>")
    End Sub

End Module
