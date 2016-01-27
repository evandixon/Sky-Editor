Imports System.Web.Script.Serialization
Imports DS_ROM_Patcher

Public Class NDSPatcherCore
    Inherits PatcherCore
    Public Overrides Sub PromptFilePath()
        Dim o As New OpenFileDialog
        o.Filter = "NDS ROM Files (*.nds)|*.nds|All Files (*.*)|*.*"
        If o.ShowDialog = DialogResult.OK Then
            SelectedFilename = o.FileName
        End If
    End Sub

    Public Overrides Async Function RunPatch(Mods As IEnumerable(Of ModJson), Optional DestinationPath As String = Nothing) As Task
        Dim currentDirectory = Environment.CurrentDirectory
        Dim ROMDirectory = IO.Path.Combine(currentDirectory, "Tools/ndstemp")
        Dim modTempDirectory = IO.Path.Combine(currentDirectory, "Tools/modstemp")

        'Extract the NDS ROM
        If IO.File.Exists(SelectedFilename) Then
            RaiseProgressChanged(0, "Extracting the NDS ROM...")
            If Not IO.Directory.Exists(ROMDirectory) Then
                IO.Directory.CreateDirectory(ROMDirectory)
            End If
            Await ProcessHelper.RunProgram(IO.Path.Combine(currentDirectory, "Tools/ndstool.exe"), String.Format("-v -x ""{0}"" -9 ""{1}/arm9.bin"" -7 ""{1}/arm7.bin"" -y9 ""{1}/y9.bin"" -y7 ""{1}/y7.bin"" -d ""{1}/data"" -y ""{1}/overlay"" -t ""{1}/banner.bin"" -h ""{1}/header.bin""", SelectedFilename, ROMDirectory))
        Else
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

        Dim j As New JavaScriptSerializer
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
        Await ProcessHelper.RunProgram(IO.Path.Combine(currentDirectory, "Tools/ndstool.exe"),
                                                  String.Format("-c ""{0}"" -9 ""{1}/arm9.bin"" -7 ""{1}/arm7.bin"" -y9 ""{1}/y9.bin"" -y7 ""{1}/y7.bin"" -d ""{1}/data"" -y ""{1}/overlay"" -t ""{1}/banner.bin"" -h ""{1}/header.bin""", IO.Path.Combine(currentDirectory, "PatchedROM.nds"), ROMDirectory))

        'Save the ROM
        RaiseProgressChanged(2 / 3, "Saving the ROM...")
        If DestinationPath Is Nothing Then
            Dim o As New SaveFileDialog
            o.Filter = "NDS Files (*.nds)|*.nds|All Files (*.*)|*.*"
ShowSaveDialogNDS: If o.ShowDialog = DialogResult.OK Then
                IO.File.Copy(IO.Path.Combine(currentDirectory, "PatchedROM.nds"), o.FileName, True)
            Else
                If MessageBox.Show("Are you sure you want to cancel the patching process?", "DS ROM Patcher", MessageBoxButtons.YesNo) = DialogResult.No Then
                    GoTo ShowSaveDialogNDS
                End If
            End If
        Else
            IO.File.Copy(IO.Path.Combine(currentDirectory, "PatchedROM.nds"), DestinationPath, True)
        End If

        RaiseProgressChanged(1, "Ready")

        'Cleanup
        If IO.Directory.Exists(ROMDirectory) Then IO.Directory.Delete(ROMDirectory, True)
        If IO.File.Exists(IO.Path.Combine(currentDirectory, "PatchedROM.nds")) Then IO.File.Delete(IO.Path.Combine(currentDirectory, "PatchedROM.nds"))
    End Function

    Public Overrides Function SupportsMod(ModToCheck As ModJson) As Boolean
        Return True
    End Function
End Class
