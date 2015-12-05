Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Web.Script.Serialization
Imports ICSharpCode.SharpZipLib.Zip

Public Class Form1


    Dim is3dsMode As Boolean
    Private Async Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        If IO.File.Exists("Tools/ctrtool.exe") Then
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
            Await ProcessHelper.RunCTRTool($"-p --exheader=""{exHeaderPath}"" ""{SourceFilename}""")
            Await ProcessHelper.RunCTRTool($"-p --exefs=""{exefsPath}"" ""{SourceFilename}""")
            Await ProcessHelper.RunCTRTool($"-p --romfs=""{romfsPath}"" ""{SourceFilename}""")
            'Unpack romfs
            Await ProcessHelper.RunCTRTool($"-t romfs --romfsdir=""{romfsDir}"" ""{romfsPath}""")
            'Unpack exefs
            Await ProcessHelper.RunCTRTool($"-t exefs --exefsdir=""{exefsDir}"" ""{exefsPath}"" --decompresscode")
            IO.File.Delete(exefsPath)
            IO.File.Delete(romfsPath)
        Else
            'Extract the NDS ROM
            statusLabel1.Text = "Extracting the NDS ROM"
            ToolStripProgressBar1.Value = 0
            If Not IO.Directory.Exists(ROMDirectory) Then
                IO.Directory.CreateDirectory(ROMDirectory)
            End If
            Await ProcessHelper.RunProgram(IO.Path.Combine(currentDirectory, "Tools/ndstool.exe"), String.Format("-v -x ""{0}"" -9 ""{1}/arm9.bin"" -7 ""{1}/arm7.bin"" -y9 ""{1}/y9.bin"" -y7 ""{1}/y7.bin"" -d ""{1}/data"" -y ""{1}/overlay"" -t ""{1}/banner.bin"" -h ""{1}/header.bin""", SourceFilename, ROMDirectory))
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
            Await ModFile.ApplyPatch(mods, item, currentDirectory, ROMDirectory, patchers)
        Next

        'Repack the ROM
        statusLabel1.Text = "Repacking the ROM"
        ToolStripProgressBar1.Value = 80
        If is3dsMode Then
            Dim exeFS As String = IO.Path.Combine(ROMDirectory, "exefs")
            Dim romFS As String = IO.Path.Combine(ROMDirectory, "romfs")
            Dim exHeader As String = IO.Path.Combine(ROMDirectory, "DecryptedExHeader.bin")
            Dim output As String = IO.Path.Combine(currentDirectory, "PatchedROM.3ds")
            Await ProcessHelper.RunProgram(IO.Path.Combine(currentDirectory, "Tools/3DS Builder.exe"),
                             $"""{exeFS}"" ""{romFS}"" ""{exHeader}"" ""{output}""")
        Else
            Await ProcessHelper.RunProgram(IO.Path.Combine(currentDirectory, "Tools/ndstool.exe"),
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