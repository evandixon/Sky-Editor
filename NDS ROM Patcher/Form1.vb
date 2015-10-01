Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Web.Script.Serialization
Imports ICSharpCode.SharpZipLib.Zip

Public Class Form1

    Public Class ModJson
        Public Property ToAdd As List(Of String)
        Public Property ToDelete As List(Of String)
        Public Property ToRename As Dictionary(Of String, String)
        Public Property ToUpdate As List(Of String)
        Public Property Name As String
        Public Property Author As String
        Public Property Description As String
        Public Property DependenciesBefore As List(Of String)
        Public Property DependenciesAfter As List(Of String)
        Public Property UpdateUrl As String
        Public Sub New()
            ToAdd = New List(Of String)
            ToDelete = New List(Of String)
            ToRename = New Dictionary(Of String, String)
            ToUpdate = New List(Of String)
            DependenciesBefore = New List(Of String)
            DependenciesAfter = New List(Of String)
        End Sub
    End Class

    Public Class FilePatcher
        Public Property FilePath As String
        Public Property CreatePatchProgram As String
        ''' <summary>
        ''' Arguments for the CreatePatchProgram.
        ''' {0} is a placeholder for the original file, {1} is a placeholder for the updated file, and {2} is a placeholder for the output patch file.
        ''' </summary>
        ''' <returns></returns>
        Public Property CreatePatchArguments As String
        Public Property ApplyPatchProgram As String
        ''' <summary>
        ''' Arguments for the ApplyPatchProgram.
        ''' {0} is a placeholder for the original file, {1} is a placeholder for the patch file, and {2} is a placeholder for the output file.
        ''' </summary>
        ''' <returns></returns>
        Public Property ApplyPatchArguments As String
        Public Property PatchExtension As String
    End Class

    Public Class ModFile
        Public Property ModDetails As ModJson
        Public Property Name As String
        Public Property Patched As Boolean
        Public Property Filename As String
        Public Async Function ApplyPatch(currentDirectory As String, ROMDirectory As String, patchers As List(Of FilePatcher)) As Task
            Dim renameTemp = IO.Path.Combine(currentDirectory, "Tools/renametemp")
            If ModDetails.ToAdd IsNot Nothing Then
                For Each file In ModDetails.ToAdd
                    IO.File.Copy(IO.Path.Combine(Filename, "Files", file.Trim("\")), IO.Path.Combine(ROMDirectory, file.Trim("\")), True)
                Next
            End If

            If ModDetails.ToUpdate IsNot Nothing Then
                For Each file In ModDetails.ToUpdate
                    If IO.File.Exists(IO.Path.Combine(ROMDirectory, file.TrimStart("\"))) Then
                        Dim patches = IO.Directory.GetFiles(IO.Path.GetDirectoryName(IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "Files", file.Trim("\"))), IO.Path.GetFileName(file.Trim("\")) & "*")
                        'Hopefully we only have 1 patch, but if there's more than 1 patch, apply them all.
                        For Each patchFile In patches
                            Dim possiblePatchers As New List(Of FilePatcher) ' = (From p In patchers Where p.PatchExtension = IO.Path.GetExtension(patchFile) Select p).ToList
                            For Each p As FilePatcher In patchers
                                If "." & p.PatchExtension = IO.Path.GetExtension(patchFile) Then
                                    possiblePatchers.Add(p)
                                End If
                            Next
                            'If possiblePatchers.Count = 0 Then
                            '   Do nothing, we don't have the tools to deal with this patch
                            If possiblePatchers.Count >= 1 Then
                                Dim tempFilename As String = IO.Path.Combine(currentDirectory, "Tools", "tempFile")
                                'If there's 1 possible patcher, great.  If there's more than one, then multiple programs have the same extension, which is their fault.  Only using the first one because we don't need to apply the same patch multiple times.
                                Await RunProgram(IO.Path.Combine(currentDirectory, "Tools", "Patchers", possiblePatchers(0).ApplyPatchProgram), String.Format(possiblePatchers(0).ApplyPatchArguments, IO.Path.Combine(ROMDirectory, file.TrimStart("\")), patchFile, tempFilename))
                                IO.File.Copy(tempFilename, IO.Path.Combine(ROMDirectory, file.TrimStart("\")), True)
                                IO.File.Delete(tempFilename)
                                End If
                        Next
                    End If
                Next
            End If

            If ModDetails.ToRename IsNot Nothing Then
                'Create temporary directory
                If Not IO.Directory.Exists(renameTemp) Then
                    IO.Directory.CreateDirectory(renameTemp)
                End If

                'Move to a temporary directory (so swapping files works)
                For Each file In ModDetails.ToRename
                    MoveFile(IO.Path.Combine(ROMDirectory, file.Key.Trim("\")), IO.Path.Combine(renameTemp, file.Key.Trim("\")), True)
                Next

                'Rename the things
                For Each file In ModDetails.ToRename
                    MoveFile(IO.Path.Combine(renameTemp, file.Key.Trim("\")), IO.Path.Combine(ROMDirectory, file.Value.Trim("\")), True)
                Next
            End If

            If ModDetails.ToDelete IsNot Nothing Then
                For Each file In ModDetails.ToDelete
                    If IO.File.Exists(IO.Path.Combine(ROMDirectory, file.Trim("\"))) Then
                        IO.File.Delete(IO.Path.Combine(ROMDirectory, file.Trim("\")))
                    End If
                Next
            End If

            If IO.Directory.Exists(renameTemp) Then IO.Directory.Delete(renameTemp, True)

            Patched = True
        End Function
        Public Sub New(Filename As String)
            Dim j As New JavaScriptSerializer
            Me.ModDetails = j.Deserialize(Of ModJson)(IO.File.ReadAllText(Filename))
            Me.Name = Me.ModDetails.Name
            Me.Patched = False
            Me.Filename = Filename
        End Sub
    End Class
    Dim is3dsMode As Boolean
    Private Async Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        If IO.File.Exists("Tools/ctrtool.exe") AndAlso IO.File.Exists("Tools/3DS Builder.exe") Then
            is3dsMode = True
            OpenFileDialog1.Filter = "3DS Files (*.3ds)|*.3ds|All Files (*.*)|*.*"
        Else
            is3dsMode = False
            OpenFileDialog1.Filter = "NDS Files (*.nds)|*.nds|All Files (*.*)|*.*"
        End If
        Dim args = Environment.GetCommandLineArgs
        If args.Length >= 3 Then
            Await PatchROM(args(1), args(2))
            Me.Close()
        ElseIf args.Length >= 2
            Await PatchROM(args(1), Nothing)
            Me.Close()
        Else
            'Show the GUI
            ListAvailableMods()
        End If
    End Sub
    Private Async Function ApplyPatch(Mods As List(Of ModFile), ModFile As ModFile, currentDirectory As String, ROMDirectory As String, patchers As List(Of FilePatcher)) As Task
        If Not ModFile.Patched Then
            'Patch depencencies
            If ModFile.ModDetails.DependenciesBefore IsNot Nothing Then
                For Each item In ModFile.ModDetails.DependenciesBefore
                    Dim q = From m In Mods Where m.Name = item AndAlso Not String.IsNullOrEmpty(m.Name)

                    For Each d In q
                        Await ApplyPatch(Mods, d, currentDirectory, ROMDirectory, patchers)
                    Next
                Next
            End If
            Await ModFile.ApplyPatch(currentDirectory, ROMDirectory, patchers)
            'Patch dependencies
            If ModFile.ModDetails.DependenciesBefore IsNot Nothing Then
                For Each item In ModFile.ModDetails.DependenciesAfter
                    Dim q = From m In Mods Where m.Name = item AndAlso Not String.IsNullOrEmpty(m.Name)

                    For Each d In q
                        Await ApplyPatch(Mods, d, currentDirectory, ROMDirectory, patchers)
                    Next
                Next
            End If
        End If
    End Function
    ''' <summary>
    ''' Patches the NDS ROM at the given SourceFilename and saves it to DestinationFilename.  Opens a SaveFileDialog if DestinationFilename is null.
    ''' </summary>
    ''' <param name="SourceFilename">Path of the vanilla ROM to patch.</param>
    ''' <param name="DestinationFilename">Path to save the patched ROM to.  If null, a SaveFileDialog will be shown.</param>
    Public Async Function PatchROM(SourceFilename As String, Optional DestinationFilename As String = Nothing) As Task
        Dim currentDirectory = Environment.CurrentDirectory 'IO.Path.GetDirectoryName(Environment.GetCommandLineArgs(0))
        Dim ROMDirectory = IO.Path.Combine(currentDirectory, "Tools/ndstemp")

        Dim modTempDirectory = IO.Path.Combine(currentDirectory, "Tools/modstemp")

        btnPatch.Enabled = False

        If is3dsMode Then
            statusLabel1.Text = "Extracting the 3DS ROM"
            If Not IO.Directory.Exists(ROMDirectory) Then IO.Directory.CreateDirectory(ROMDirectory)
            Dim exHeaderPath = IO.Path.Combine(ROMDirectory, "DecryptedExHeader.bin")
            Dim exefsPath = IO.Path.Combine(ROMDirectory, "DecryptedExeFS.bin")
            Dim romfsPath = IO.Path.Combine(ROMDirectory, "DecryptedRomFS.bin")
            Dim romfsDir = IO.Path.Combine(ROMDirectory, "romfs")
            Dim exefsDir = IO.Path.Combine(ROMDirectory, "exefs")
            'Unpack portions
            Await RunCTRTool($"-p --exheader=""{exHeaderPath}"" ""{SourceFilename}""")
            Await RunCTRTool($"-p --exefs=""{exefsPath}"" ""{SourceFilename}""")
            Await RunCTRTool($"-p --romfs=""{romfsPath}"" ""{SourceFilename}""")
            'Unpack romfs
            Await RunCTRTool($"-t romfs --romfsdir=""{romfsDir}"" ""{romfsPath}""")
            'Unpack exefs
            Await RunCTRTool($"-t exefs --exefsdir=""{exefsDir}"" ""{exefsPath}"" --decompresscode")
            IO.File.Delete(exefsPath)
            IO.File.Delete(romfsPath)
        Else
            'Extract the NDS ROM
            statusLabel1.Text = "Extracting the NDS ROM"
            ToolStripProgressBar1.Value = 0
            If Not IO.Directory.Exists(ROMDirectory) Then
                IO.Directory.CreateDirectory(ROMDirectory)
            End If
            Await RunProgram(IO.Path.Combine(currentDirectory, "Tools/ndstool.exe"), String.Format("-v -x ""{0}"" -9 ""{1}/arm9.bin"" -7 ""{1}/arm7.bin"" -y9 ""{1}/y9.bin"" -y7 ""{1}/y7.bin"" -d ""{1}/data"" -y ""{1}/overlay"" -t ""{1}/banner.bin"" -h ""{1}/header.bin""", SourceFilename, ROMDirectory))
        End If

        'Unpack the mods
        statusLabel1.Text = "Extracting the mods"
        ToolStripProgressBar1.Value = 20
        For Each item In IO.Directory.GetFiles(IO.Path.Combine(currentDirectory, "Mods"), "*.dsmod", IO.SearchOption.TopDirectoryOnly)
            Dim z As New FastZip
            If Not IO.Directory.Exists(IO.Path.Combine(modTempDirectory, IO.Path.GetFileNameWithoutExtension(item))) Then
                IO.Directory.CreateDirectory(IO.Path.Combine(modTempDirectory, IO.Path.GetFileNameWithoutExtension(item)))
            End If
            z.ExtractZip(item, IO.Path.Combine(modTempDirectory, IO.Path.GetFileNameWithoutExtension(item)), ".*")
        Next

        'Apply the mods
        statusLabel1.Text = "Applying the mods"
        ToolStripProgressBar1.Value = 40
        Dim j As New JavaScriptSerializer
        Dim patchers = j.Deserialize(Of List(Of FilePatcher))(IO.File.ReadAllText(IO.Path.Combine(currentDirectory, "Tools", "patchers.json")))
        Dim mods As New List(Of ModFile)
        For Each item In IO.Directory.GetDirectories(modTempDirectory, "*", IO.SearchOption.TopDirectoryOnly)
            mods.Add(New ModFile(item & "\mod.json"))
        Next

        For Each item In mods
            Await ApplyPatch(mods, item, currentDirectory, ROMDirectory, patchers)
        Next

        'Repack the ROM
        statusLabel1.Text = "Repacking the ROM"
        ToolStripProgressBar1.Value = 80
        If is3dsMode Then
            Dim exeFS As String = IO.Path.Combine(ROMDirectory, "exefs")
            Dim romFS As String = IO.Path.Combine(ROMDirectory, "romfs")
            Dim exHeader As String = IO.Path.Combine(ROMDirectory, "DecryptedExHeader.bin")
            Dim output As String = IO.Path.Combine(currentDirectory, "PatchedROM.3ds")
            Await RunProgram(IO.Path.Combine(currentDirectory, "Tools/3DS Builder.exe"),
                             $"""{exeFS}"" ""{romFS}"" ""{exHeader}"" ""{output}""")
        Else
            Await RunProgram(IO.Path.Combine(currentDirectory, "Tools/ndstool.exe"),
                                                  String.Format("-c ""{0}"" -9 ""{1}/arm9.bin"" -7 ""{1}/arm7.bin"" -y9 ""{1}/y9.bin"" -y7 ""{1}/y7.bin"" -d ""{1}/data"" -y ""{1}/overlay"" -t ""{1}/banner.bin"" -h ""{1}/header.bin""", IO.Path.Combine(currentDirectory, "PatchedROM.nds"), ROMDirectory))
        End If

        If is3dsMode Then
            'Save the ROM
            statusLabel1.Text = "Saving the ROM"
            ToolStripProgressBar1.Value = 100
            If String.IsNullOrEmpty(DestinationFilename) Then
                Dim o As New SaveFileDialog
                o.Filter = "3DS Files (*.3ds)|*.3ds|All Files (*.*)|*.*"
ShowSaveDialog3DS: If o.ShowDialog = DialogResult.OK Then
                    IO.File.Copy(IO.Path.Combine(currentDirectory, "PatchedROM.3ds"), o.FileName)
                Else
                    If MessageBox.Show("Are you sure you want to cancel the patching process?", "DS ROM Patcher", MessageBoxButtons.YesNo) = DialogResult.No Then
                        GoTo ShowSaveDialog3DS
                    End If
                End If
            Else
                IO.File.Copy(IO.Path.Combine(currentDirectory, "PatchedROM.3ds"), DestinationFilename, True)
            End If

            'Clean Up
            statusLabel1.Text = "Cleaning up"
            ToolStripProgressBar1.Value = 100
            If IO.Directory.Exists(modTempDirectory) Then IO.Directory.Delete(modTempDirectory, True)

            IO.Directory.Delete(ROMDirectory, True)
            IO.File.Delete(IO.Path.Combine(currentDirectory, "PatchedROM.3ds"))
        Else
            'Save the ROM
            statusLabel1.Text = "Saving the ROM"
            ToolStripProgressBar1.Value = 100
            If String.IsNullOrEmpty(DestinationFilename) Then
                Dim o As New SaveFileDialog
                o.Filter = "NDS Files (*.nds)|*.nds|All Files (*.*)|*.*"
ShowSaveDialogNDS: If o.ShowDialog = DialogResult.OK Then
                    IO.File.Copy(IO.Path.Combine(currentDirectory, "PatchedROM.nds"), o.FileName)
                Else
                    If MessageBox.Show("Are you sure you want to cancel the patching process?", "DS ROM Patcher", MessageBoxButtons.YesNo) = DialogResult.No Then
                        GoTo ShowSaveDialogNDS
                    End If
                End If
            Else
                IO.File.Copy(IO.Path.Combine(currentDirectory, "PatchedROM.nds"), DestinationFilename, True)
            End If

            'Clean Up
            statusLabel1.Text = "Cleaning up"
            ToolStripProgressBar1.Value = 100
            If IO.Directory.Exists(modTempDirectory) Then IO.Directory.Delete(modTempDirectory, True)

            IO.Directory.Delete(ROMDirectory, True)
            IO.File.Delete(IO.Path.Combine(currentDirectory, "PatchedROM.nds"))
        End If


        statusLabel1.Text = "Ready"
        ToolStripProgressBar1.Value = 100
        btnPatch.Enabled = True
    End Function
    ''' <summary>
    ''' Runs the specified program, capturing console output.
    ''' Returns true when the program exits.
    ''' </summary>
    ''' <param name="Filename"></param>
    ''' <param name="Arguments"></param>
    ''' <remarks></remarks>
    Public Shared Async Function RunProgram(Filename As String, Arguments As String) As Task
        'WriteLine(String.Format("Executing {0} {1}", Filename, Arguments))
        Dim p As New Process()
        p.StartInfo.FileName = Filename
        p.StartInfo.Arguments = Arguments
        p.StartInfo.RedirectStandardOutput = True
        p.StartInfo.UseShellExecute = False
        p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
        p.StartInfo.CreateNoWindow = True
        p.StartInfo.WorkingDirectory = IO.Path.GetDirectoryName(Filename)
        p.Start()
        p.BeginOutputReadLine()
        Await WaitForProcess(p)
        p.Dispose()
        ' WriteLine(String.Format("""{0}"" finished running.", p.StartInfo.FileName))
    End Function
    Public Shared Async Function RunCTRTool(Arguments) As Task
        Await RunProgram("Tools/ctrtool.exe", Arguments)
    End Function
    Private Shared Async Function WaitForProcess(p As Process) As Task
        Await Task.Run(Sub()
                           p.WaitForExit()
                       End Sub)
    End Function
    Public Shared Sub MoveFile(OriginalFilename As String, NewFilename As String, Overwrite As Boolean)
        IO.File.Copy(OriginalFilename, NewFilename, Overwrite)
        IO.File.Delete(OriginalFilename)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If OpenFileDialog1.ShowDialog = DialogResult.OK Then
            txtFilename.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Async Sub btnPatch_Click(sender As Object, e As EventArgs) Handles btnPatch.Click
        Await PatchROM(txtFilename.Text)
    End Sub

    Private Sub ListAvailableMods()
        Dim currentDirectory = IO.Path.GetDirectoryName(Environment.GetCommandLineArgs(0))
        If IO.Directory.Exists(IO.Path.Combine(currentDirectory, "Mods")) Then
            For Each item In IO.Directory.GetFiles(IO.Path.Combine(currentDirectory, "Mods"), "*.dsmod*", IO.SearchOption.TopDirectoryOnly)
                clbMods.Items.Add(IO.Path.GetFileNameWithoutExtension(item), Not item.ToLower.EndsWith(".disabled"))
            Next
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If Button2.Tag = True Then
            Button2.Tag = False
            Button2.Text = "Advanced"
            Me.Size = New Size(456, 130)
            GroupBox1.Visible = False
        Else
            Button2.Tag = True
            Button2.Text = "Simple"
            Me.Size = New Size(456, 407)
            GroupBox1.Visible = True
        End If
    End Sub
End Class