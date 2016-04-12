Imports System.Web.Script.Serialization
Imports DS_ROM_Patcher

Public Class ThreeDSPatcherCore
    Inherits PatcherCore

    Public Overrides Sub PromptFilePath()
        Dim o As New OpenFileDialog
        o.Filter = "Supported Files (*.3ds;*.3dz;*.cxi;romfs.bin)|*.3ds;*.3dz;romfs.bin|3DS DS Roms (*.3ds;*.3dz)|*.3ds;*.3dz|CXI Files (*.cxi)|*.cxi|Braindump romfs (romfs.bin)|romfs.bin|All Files (*.*)|*.*"
        If o.ShowDialog = DialogResult.OK Then
            SelectedFilename = o.FileName
        End If
    End Sub

    Public Overrides Async Function RunPatch(Mods As IEnumerable(Of ModJson), Optional DestinationPath As String = Nothing) As Task
        Dim args = Environment.GetCommandLineArgs
        Dim currentDirectory = Environment.CurrentDirectory
        Dim ROMDirectory = IO.Path.Combine(currentDirectory, "Tools", "ndstemp")
        Dim modTempDirectory = IO.Path.Combine(currentDirectory, "Tools", "modstemp")
        Dim modsDirectory = IO.Path.Combine(currentDirectory, "mods")
        Dim output As String = IO.Path.Combine(currentDirectory, "Tools", "PatchedROM.3ds")
        Dim outputCia As String = IO.Path.Combine(currentDirectory, "Tools", "PatchedROM.cia")
        Dim j As New JavaScriptSerializer
        Dim info As ModpackInfo = j.Deserialize(Of ModpackInfo)(IO.File.ReadAllText(IO.Path.Combine(currentDirectory, "Mods", "Modpack Info")))

        Dim hansMode As Boolean = args.Contains("-hans")

        'Extract the NDS ROM
        If IO.File.Exists(SelectedFilename) Then
            RaiseProgressChanged(0, "Extracting the 3DS ROM...")
            If Not IO.Directory.Exists(ROMDirectory) Then
                IO.Directory.CreateDirectory(ROMDirectory)
            End If

            'Is the input a romfs.bin?
            If SelectedFilename.ToLower.EndsWith(".romfs") OrElse SelectedFilename.ToLower.EndsWith("romfs.bin") Then
                hansMode = True
                Dim exefsPath = IO.Path.Combine(ROMDirectory, "DecryptedExeFS.bin")
                Dim romfsPath = IO.Path.Combine(ROMDirectory, "DecryptedRomFS.bin")
                Dim romfsDir = IO.Path.Combine(ROMDirectory, "romfs")
                Dim exefsDir = IO.Path.Combine(ROMDirectory, "exefs")
                'It's a romfs.bin file

                If Not IO.Directory.Exists(ROMDirectory) Then
                    IO.Directory.CreateDirectory(ROMDirectory)
                End If

                IO.File.Copy(SelectedFilename, romfsPath, True)

                'Unpack exefs
                Dim exefsSource As String = IO.Path.Combine(IO.Path.GetDirectoryName(SelectedFilename), "exefs.bin")
                If IO.File.Exists(exefsSource) Then
                    IO.File.Copy(exefsSource, exefsPath, True)

                    If Not IO.Directory.Exists(exefsDir) Then
                        IO.Directory.CreateDirectory(exefsDir)
                    End If
                    Await ProcessHelper.RunCTRTool($"-t exefs --exefsdir=""{exefsDir}"" ""{exefsPath}"" --decompresscode")
                End If

                'Unpack romfs
                If Not IO.Directory.Exists(romfsDir) Then
                    IO.Directory.CreateDirectory(romfsDir)
                End If

                Await ProcessHelper.RunCTRTool($"-t romfs --romfsdir=""{romfsDir}"" ""{romfsPath}""")
            Else
                Dim DestinationDirectory = ROMDirectory
                Dim Filename = SelectedFilename
                'We're dealing with a .3DS file
                '(Basically these variables are formatted as so: [item][bin/dir][path/task])
                'Define paths
                Dim exHeaderPath = IO.Path.Combine(DestinationDirectory, "DecryptedExHeader.bin")
                Dim exefsBinPath = IO.Path.Combine(DestinationDirectory, "DecryptedExeFS.bin")
                Dim romfsBinPath = IO.Path.Combine(DestinationDirectory, "DecryptedRomFS.bin")
                Dim romfsDirPath = IO.Path.Combine(DestinationDirectory, "Romfs")
                Dim exefsDirPath = IO.Path.Combine(DestinationDirectory, "Exefs")
                Dim manualBinPath = IO.Path.Combine(DestinationDirectory, "DecryptedManual.bin")
                Dim manualDirPath = IO.Path.Combine(DestinationDirectory, "Manual")
                Dim dlPlayBinPath = IO.Path.Combine(DestinationDirectory, "DecryptedDownloadPlay.bin")
                Dim dlPlayDirPath = IO.Path.Combine(DestinationDirectory, "DownloadPlay")
                Dim n3dsUpdateBinPath = IO.Path.Combine(DestinationDirectory, "DecryptedN3DSUpdate.bin")
                Dim n3dsUpdateDirPath = IO.Path.Combine(DestinationDirectory, "N3DSUpdate")
                Dim o3dsUpdateBinPath = IO.Path.Combine(DestinationDirectory, "DecryptedO3DSUpdate.bin")
                Dim o3dsUpdateDirPath = IO.Path.Combine(DestinationDirectory, "O3DSUpdate")
                Dim onlineHeaderBinPath = IO.Path.Combine(DestinationDirectory, "OnlineHeader.bin")

                'Unpack portions
                Dim exheaderTask = ProcessHelper.RunCTRTool($"-p --ncch=0 --exheader=""{exHeaderPath}"" ""{Filename}""")
                Dim exefsBinTask = ProcessHelper.RunCTRTool($"-p --ncch=0 --exefs=""{exefsBinPath}"" ""{Filename}""")
                Dim romfsBinTask = ProcessHelper.RunCTRTool($"-p --ncch=0 --romfs=""{romfsBinPath}"" ""{Filename}""")
                Dim manualBinTask = ProcessHelper.RunCTRTool($"-p --ncch=1 --romfs=""{manualBinPath}"" ""{Filename}"" --decompresscode")
                Dim dlPlayBinTask = ProcessHelper.RunCTRTool($"-p --ncch=2 --romfs=""{dlPlayBinPath}"" ""{Filename}"" --decompresscode")
                Dim n3dsUpdateBinTask = ProcessHelper.RunCTRTool($"-p --ncch=6 --romfs=""{n3dsUpdateBinPath}"" ""{Filename}"" --decompresscode")
                Dim o3dsUpdateBinTask = ProcessHelper.RunCTRTool($"-p --ncch=7 --romfs=""{o3dsUpdateBinPath}"" ""{Filename}"" --decompresscode")

                'Save online header
                Using f As New GenericFile
                    f.OpenFile(SelectedFilename)
                    IO.File.WriteAllBytes(onlineHeaderBinPath, f.RawData(&H1200, &H2E00))
                End Using


                'Unpack romfs
                Await romfsBinTask
                Dim romfsDirTask = ProcessHelper.RunCTRTool($"-t romfs --romfsdir=""{romfsDirPath}"" ""{romfsBinPath}""")

                'Unpack exefs
                Await exefsBinTask
                Dim exefsDirTask = ProcessHelper.RunCTRTool($"-t exefs --exefsdir=""{exefsDirPath}"" ""{exefsBinPath}"" --decompresscode")

                'Unpack manual
                Await manualBinTask
                Dim manualDirTask = ProcessHelper.RunCTRTool($"-t romfs --romfsdir=""{manualDirPath}"" ""{manualBinPath}""")

                'Unpack n3ds update
                Await n3dsUpdateBinTask
                Dim n3dsUpdateDirTask = ProcessHelper.RunCTRTool($"-t romfs --romfsdir=""{n3dsUpdateDirPath}"" ""{n3dsUpdateBinPath}""")

                'Unpack o3ds update
                Await o3dsUpdateBinTask
                Dim o3dsUpdateDirTask = ProcessHelper.RunCTRTool($"-t romfs --romfsdir=""{o3dsUpdateDirPath}"" ""{o3dsUpdateBinPath}""")

                'Unpack download play
                Await dlPlayBinTask
                Dim dlPlayDirTask = ProcessHelper.RunCTRTool($"-t romfs --romfsdir=""{dlPlayDirPath}"" ""{dlPlayBinPath}""")


                Await romfsDirTask
                Await exefsDirTask
                Await manualDirTask
                Await n3dsUpdateDirTask
                Await dlPlayDirTask

                DeleteFile(exefsBinPath)
                DeleteFile(romfsBinPath)
                DeleteFile(manualBinPath)
                DeleteFile(dlPlayBinPath)
                DeleteFile(n3dsUpdateBinPath)
                DeleteFile(o3dsUpdateBinPath)
            End If
        ElseIf IO.Directory.Exists(SelectedFilename) Then
            RaiseProgressChanged(0, "Copying Files...")
            Dim tasks As New List(Of Task)
            For Each item In IO.Directory.GetFiles(SelectedFilename, "*", IO.SearchOption.AllDirectories)
                Dim item2 = item
                tasks.Add(Task.Run(Sub()
                                       Dim dest As String = item.Replace(SelectedFilename, ROMDirectory)
                                       If Not IO.Directory.Exists(IO.Path.GetDirectoryName(dest)) Then
                                           IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(dest))
                                       End If
                                       IO.File.Copy(item, dest, True)
                                   End Sub))
            Next
            Await Task.WhenAll(tasks)
        End If

        'Apply the Mods
        Const RepackMessage As String = "Repacking the ROM..."
        RaiseProgressChanged(1 / 3, RepackMessage)

        Dim patchers = j.Deserialize(Of List(Of FilePatcher))(IO.File.ReadAllText(IO.Path.Combine(currentDirectory, "Tools", "patchers.json")))
        Dim modFiles As New List(Of ModFile)
        For Each item In Mods
            modFiles.Add(New ModFile(item.Filename))
        Next

        For Each item In modFiles
            Await ModFile.ApplyPatch(modFiles, item, currentDirectory, ROMDirectory, patchers)
        Next

        'Repack the ROM
        RaiseProgressChanged(2 / 3, "Repacking the ROM...")

        Dim destination As String = Nothing

        If Not hansMode AndAlso DestinationPath Is Nothing Then
            If MessageBox.Show("Would you like to output to HANS?  (Say no to output to .3DS)", "DS ROM Patcher", MessageBoxButtons.YesNo) = DialogResult.Yes Then '(Say no to output to .3DS or .CIA)
                hansMode = True
            End If
        End If

        If hansMode Then
            If DestinationPath Is Nothing Then
                Dim d As New FolderBrowserDialog
                d.Description = "Please select the root of your SD card."
ShowFolderDialog3DS: If d.ShowDialog = DialogResult.OK Then
                    destination = d.SelectedPath
                Else
                    If MessageBox.Show("Are you sure you want to cancel the patching process?", "DS ROM Patcher", MessageBoxButtons.YesNo) = DialogResult.No Then
                        GoTo ShowSaveDialog3DS
                    End If
                End If
            Else
                destination = DestinationPath
            End If

            If destination IsNot Nothing Then
                Dim romfsDir = IO.Path.Combine(ROMDirectory, "romfs")
                Dim romfsPath = IO.Path.Combine(ROMDirectory, "romfsRepacked.bin")
                Dim romfsTrimmedPath = romfsPath 'IO.Path.Combine(ROMDirectory, "romfsRepackedTrimmed.bin")
                Dim hansName As String = info.Name.Replace(" ", "").Replace("é", "e")
                Dim shortName As String = info.ShortName

                'Repack romfs
                Await ProcessHelper.RunProgram(IO.Path.Combine(currentDirectory, "Tools/3DS Builder.exe"),
                                     $"-romfs ""{romfsDir}"" ""{romfsPath}""")

                'Apparently this isn't needed anymore, although I haven't been able to successfully test this
                ''Trim the first part of the romfs
                'Const HansRomfsTrim As Integer = &H1000
                'Using source As New IO.FileStream(romfsPath, IO.FileMode.Open, IO.FileAccess.ReadWrite)
                '    Using dest As New IO.FileStream(romfsTrimmedPath, IO.FileMode.OpenOrCreate, IO.FileAccess.ReadWrite)
                '        dest.SetLength(source.Length - HansRomfsTrim)
                '        source.Seek(HansRomfsTrim, IO.SeekOrigin.Begin)
                '        dest.Seek(0, IO.SeekOrigin.Begin)
                '        source.CopyTo(dest)
                '        dest.Flush()
                '    End Using
                'End Using

                'Copy the files
                If Not IO.Directory.Exists(destination) Then
                    IO.Directory.CreateDirectory(destination)
                End If

                If Not IO.Directory.Exists(IO.Path.Combine(destination, "hans")) Then
                    IO.Directory.CreateDirectory(IO.Path.Combine(destination, "hans"))
                End If

                IO.File.Copy(romfsTrimmedPath, IO.Path.Combine(destination, "hans", shortName & ".romfs"), True)

                If IO.File.Exists(IO.Path.Combine(ROMDirectory, "exefs", "code.bin")) Then
                    IO.File.Copy(IO.Path.Combine(ROMDirectory, "exefs", "code.bin"), IO.Path.Combine(destination, "hans", shortName & ".code"), True)
                End If

                If Not IO.Directory.Exists(IO.Path.Combine(destination, "3ds")) Then
                    IO.Directory.CreateDirectory(IO.Path.Combine(destination, "3ds"))
                End If

                'Copy smdh
                Dim iconExists As Boolean = False
                If IO.File.Exists(IO.Path.Combine(modsDirectory, "Modpack.smdh")) Then
                    iconExists = True
                    IO.File.Copy(IO.Path.Combine(modsDirectory, "Modpack.smdh"), IO.Path.Combine(destination, "3ds", hansName & ".smdh"), True)
                End If

                'Write hans shortcut
                Dim shortcut As New Text.StringBuilder
                shortcut.AppendLine("<shortcut>")
                shortcut.AppendLine("	<executable>/3ds/hans/hans.3dsx</executable>")
                If iconExists Then
                    shortcut.AppendLine($"	<icon>/3ds/{hansName}.smdh</icon>")
                End If
                shortcut.AppendLine($"	<arg>-f/3ds/hans/titles/{shortName}.txt</arg>")
                shortcut.AppendLine("</shortcut>")
                shortcut.AppendLine("<targets selectable=""false"">")
                shortcut.AppendLine($"	<title mediatype=""2"">{info.GameCode}</title>")
                shortcut.AppendLine($"	<title mediatype=""1"">{info.GameCode}</title>")
                shortcut.AppendLine("</targets>")
                IO.File.WriteAllText(IO.Path.Combine(destination, "3ds", hansName & ".xml"), shortcut.ToString)

                'Write hans title settings
                Dim preset As New Text.StringBuilder
                preset.Append("region : -1")
                preset.Append(vbLf)
                preset.Append("language : -1")
                preset.Append(vbLf)
                preset.Append("clock : 0")
                preset.Append(vbLf)
                preset.Append("romfs : 0")
                preset.Append(vbLf)
                preset.Append("code : 0")
                preset.Append(vbLf)
                preset.Append("nim_checkupdate : 1")
                preset.Append(vbLf)
                If Not IO.Directory.Exists(IO.Path.Combine(destination, "3ds", "hans", "titles")) Then
                    IO.Directory.CreateDirectory(IO.Path.Combine(destination, "3ds", "hans", "titles"))
                End If
                IO.File.WriteAllText(IO.Path.Combine(destination, "3ds", "hans", "titles", shortName & ".txt"), preset.ToString)
                RaiseProgressChanged(1, "Ready")
            Else
                RaiseProgressChanged(1, "Patching canceled by user")
            End If

        Else '.3DS mode
            'Choose an output file
            If DestinationPath Is Nothing Then
ShowSaveDialog3DS: Dim s As New SaveFileDialog
                s.Filter = "3DS Files (*.3ds)|*.3ds|All Files (*.*)|*.*"
                If s.ShowDialog = DialogResult.OK Then
                    destination = s.FileName
                Else
                    If MessageBox.Show("Are you sure you want to cancel the patching process?", "DS ROM Patcher", MessageBoxButtons.YesNo) = DialogResult.No Then
                        GoTo ShowSaveDialog3DS
                    End If
                End If
            Else
                destination = DestinationPath
            End If

            'If output file chosen, repack then copy
            If destination IsNot Nothing Then
                'Output to .3DS first
                Dim exeFS As String = IO.Path.Combine(ROMDirectory, "exefs")
                Dim romFS As String = IO.Path.Combine(ROMDirectory, "romfs")
                Dim exHeader As String = IO.Path.Combine(ROMDirectory, "DecryptedExHeader.bin")
                Dim onlineHeader As String = IO.Path.Combine(ROMDirectory, "OnlineHeader.bin")

                'To save lots of time, we're NOT going to compress code.bin
                'Because of this, we must update the exHeader to not expect a compressed code.bin
                Using f As New IO.FileStream(exHeader, IO.FileMode.Open, IO.FileAccess.ReadWrite)
                    f.Seek(&HD, IO.SeekOrigin.Begin)
                    Dim sciD = f.ReadByte

                    sciD = sciD And &HFE 'We want to set bit 1 to 0 to avoid using a compressed code.bin


                    If destination.ToLower.EndsWith(".cia") Then
                        'If we're going to build a cia later, let's go ahead and update the exheader.
                        sciD = sciD Or 2
                    End If


                    f.Seek(&HD, IO.SeekOrigin.Begin)
                    f.WriteByte(sciD)
                    f.Flush()
                End Using

                'Run 3DS Builder
                Await ProcessHelper.RunProgram(IO.Path.Combine(currentDirectory, "Tools/3DS Builder.exe"),
                                     $"""{exeFS}"" ""{romFS}"" ""{exHeader}"" ""{output}""", True) 'Add -compressCode to compress code.bin

                'Add the online header back
                If IO.File.Exists(onlineHeader) Then
                    Using f As New GenericFile
                        f.OpenFile(output)
                        f.RawData(&H1200, &H2E00) = IO.File.ReadAllBytes(onlineHeader)
                    End Using
                End If


                'If destination.ToLower.EndsWith(".cia") Then
                '    Await CiaConversion.ConvertToCia(output, outputCia)
                '    IO.File.Delete(output)
                '    IO.File.Copy(outputCia, destination, True)
                'Else
                IO.File.Copy(output, destination, True)
                'End If

                RaiseProgressChanged(1, "Ready")
            Else
                RaiseProgressChanged(1, "Patching canceled by user")
            End If
        End If

        'Cleanup
        If Not IO.File.Exists("nocleanup") Then
            If IO.Directory.Exists(ROMDirectory) Then IO.Directory.Delete(ROMDirectory, True)
            If IO.File.Exists(output) Then IO.File.Delete(output)
            If IO.File.Exists(outputCia) Then IO.File.Delete(outputCia)
        End If
    End Function

    Public Overrides Function SupportsMod(ModToCheck As ModJson) As Boolean
        Return True
    End Function

    ''' <summary>
    ''' Deletes the given file if it exists, and does nothing if it does not exist.
    ''' </summary>
    ''' <param name="Filename">Full path of the file to delete.</param>
    Public Shared Sub DeleteFile(Filename As String)
        If IO.File.Exists(Filename) Then
            IO.File.Delete(Filename)
        End If
    End Sub
End Class
