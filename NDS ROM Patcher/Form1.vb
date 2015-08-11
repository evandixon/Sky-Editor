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
        Public Property FilePath As Regex
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

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim args = Environment.GetCommandLineArgs
        If args.Length >= 3 Then
            PatchROM(args(1), args(2))
        ElseIf args.Length >= 2
            PatchROM(args(1), Nothing)
        End If
    End Sub
    ''' <summary>
    ''' Patches the NDS ROM at the given SourceFilename and saves it to DestinationFilename.  Opens a SaveFileDialog if DestinationFilename is null.
    ''' </summary>
    ''' <param name="SourceFilename">Path of the vanilla ROM to patch.</param>
    ''' <param name="DestinationFilename">Path to save the patched ROM to.  If null, a SaveFileDialog will be shown.</param>
    Public Async Sub PatchROM(SourceFilename As String, Optional DestinationFilename As String = Nothing)
        Dim currentDirectory = IO.Path.GetDirectoryName(Environment.GetCommandLineArgs(0))
        Dim ROMDirectory = IO.Path.Combine(currentDirectory, "Tools/ndstemp")
        Dim renameTemp = IO.Path.Combine(currentDirectory, "Tools/renametemp")
        Dim modTempDirectory = IO.Path.Combine(currentDirectory, "Tools/modstemp")

        'Extract the NDS ROM
        If Not IO.Directory.Exists(ROMDirectory) Then
            IO.Directory.CreateDirectory(ROMDirectory)
        End If
        Await RunProgram(IO.Path.Combine(currentDirectory, "Tools/ndstool.exe"), String.Format("-v -x ""{0}"" -9 ""{1}/arm9.bin"" -7 ""{1}/arm7.bin"" -y9 ""{1}/y9.bin"" -y7 ""{1}/y7.bin"" -d ""{1}/data"" -y ""{1}/overlay"" -t ""{1}/banner.bin"" -h ""{1}/header.bin""", SourceFilename, ROMDirectory))

        'Unpack the mods
        For Each item In IO.Directory.GetFiles(IO.Path.Combine(currentDirectory, "Mods"), "*.ndsmod", IO.SearchOption.TopDirectoryOnly)
            Dim z As New FastZip
            If Not IO.Directory.Exists(IO.Path.Combine(modTempDirectory, IO.Path.GetFileNameWithoutExtension(item))) Then
                IO.Directory.CreateDirectory(IO.Path.Combine(modTempDirectory, IO.Path.GetFileNameWithoutExtension(item)))
            End If
            z.ExtractZip(item, IO.Path.Combine(modTempDirectory, IO.Path.GetFileNameWithoutExtension(item)), ".*")
        Next

        'Apply the mods
        Dim j As New JavaScriptSerializer
        Dim patchers = j.Deserialize(Of List(Of FilePatcher))(IO.File.ReadAllText(IO.Path.Combine(currentDirectory, "Tools", "patchers.json")))
        For Each item In IO.Directory.GetDirectories(modTempDirectory, "*", IO.SearchOption.TopDirectoryOnly)
            Dim currentMod = j.Deserialize(Of ModJson)(IO.File.ReadAllText(IO.Path.Combine(item, "mod.json")))

            If currentMod.ToAdd IsNot Nothing Then
                For Each file In currentMod.ToAdd
                    IO.File.Copy(IO.Path.Combine(item, "Files", file.Trim("\")), IO.Path.Combine(ROMDirectory, file.Trim("\")), True)
                Next
            End If

            If currentMod.ToUpdate IsNot Nothing Then
                For Each file In currentMod.ToUpdate
                    Dim patches = IO.Directory.GetFiles(IO.Path.GetDirectoryName(IO.Path.Combine(ROMDirectory, file.Trim("\"))), IO.Path.GetFileName(file.Trim("\")) & "*")
                    'Hopefully we only have 1 patch, but if there's more than 1 patch, apply them all.
                    For Each patchFile In patches
                        Dim possiblePatchers = (From p In patchers Where p.PatchExtension = IO.Path.GetExtension(patchFile) Select p).ToList
                        'If possiblePatchers.Count = 0 Then
                        '   Do nothing, we don't have the tools to deal with this patch
                        If possiblePatchers.Count >= 1 Then
                            'If there's 1 possible patcher, great.  If there's more than one, then multiple programs have the same extension, which is their fault.  Only using the first one because we don't need to apply the same patch multiple times.
                            Await RunProgram(IO.Path.Combine(currentDirectory, "Tools", possiblePatchers(0).ApplyPatchProgram), possiblePatchers(0).ApplyPatchArguments)
                        End If
                    Next
                Next
            End If

            If currentMod.ToRename IsNot Nothing Then
                'Create temporary directory
                If Not IO.Directory.Exists(renameTemp) Then
                    IO.Directory.CreateDirectory(renameTemp)
                End If

                'Move to a temporary directory (so swapping files works)
                For Each file In currentMod.ToRename
                    MoveFile(IO.Path.Combine(ROMDirectory, file.Key.Trim("\")), IO.Path.Combine(renameTemp, file.Key.Trim("\")), True)
                Next

                'Rename the things
                For Each file In currentMod.ToRename
                    MoveFile(IO.Path.Combine(renameTemp, file.Key.Trim("\")), IO.Path.Combine(ROMDirectory, file.Value.Trim("\")), True)
                Next
            End If

            If currentMod.ToDelete IsNot Nothing Then
                For Each file In currentMod.ToDelete
                    If IO.File.Exists(IO.Path.Combine(ROMDirectory, file.Trim("\"))) Then
                        IO.File.Delete(IO.Path.Combine(ROMDirectory, file.Trim("\")))
                    End If
                Next
            End If

        Next
        'Repack the ROM
        Await RunProgram(IO.Path.Combine(currentDirectory, "Tools/ndstool.exe"),
                                                  String.Format("-c ""{0}"" -9 ""{1}/arm9.bin"" -7 ""{1}/arm7.bin"" -y9 ""{1}/y9.bin"" -y7 ""{1}/y7.bin"" -d ""{1}/data"" -y ""{1}/overlay"" -t ""{1}/banner.bin"" -h ""{1}/header.bin""", IO.Path.Combine(currentDirectory, "PatchedROM.nds"), ROMDirectory))
        'Save the ROM
        If String.IsNullOrEmpty(DestinationFilename) Then
            Dim o As New SaveFileDialog
            o.Filter = "NDS Files (*.nds)|*.nds|All Files (*.*)|*.*"
ShowSaveDialog: If o.ShowDialog = DialogResult.OK Then
                IO.File.Copy(IO.Path.Combine(currentDirectory, "PatchedROM.nds"), o.FileName)
            Else
                If MessageBox.Show("Are you sure you want to cancel the patching process?", "NDS ROM Patcher", MessageBoxButtons.YesNo) = DialogResult.No Then
                    GoTo ShowSaveDialog
                End If
            End If
        Else
            IO.File.Copy(IO.Path.Combine(currentDirectory, "PatchedROM.nds"), DestinationFilename)
        End If


        'Clean Up
        IO.Directory.Delete(modTempDirectory, True)
        If IO.Directory.Exists(renameTemp) Then IO.Directory.Delete(renameTemp, True)
        IO.Directory.Delete(modTempDirectory, True)
        IO.File.Delete(IO.Path.Combine(currentDirectory, "PatchedROM.nds"))
    End Sub
    ''' <summary>
    ''' Runs the specified program, capturing console output.
    ''' Returns true when the program exits.
    ''' </summary>
    ''' <param name="Filename"></param>
    ''' <param name="Arguments"></param>
    ''' <remarks></remarks>
    Public Shared Async Function RunProgram(Filename As String, Arguments As String) As Task(Of Boolean)
        'WriteLine(String.Format("Executing {0} {1}", Filename, Arguments))
        Dim p As New Process()
        p.StartInfo.FileName = Filename
        p.StartInfo.Arguments = Arguments
        p.StartInfo.RedirectStandardOutput = True
        p.StartInfo.UseShellExecute = False
        p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
        p.StartInfo.CreateNoWindow = True
        p.Start()
        p.BeginOutputReadLine()
        Await WaitForProcess(p)
        p.Dispose()
        ' WriteLine(String.Format("""{0}"" finished running.", p.StartInfo.FileName))
        Return True
    End Function
    Private Shared Async Function WaitForProcess(p As Process) As Task(Of Boolean)
        Return Await Task.Run(Function()
                                  p.WaitForExit()
                                  Return True
                              End Function)
    End Function
    Public Sub MoveFile(OriginalFilename As String, NewFilename As String, Overwrite As Boolean)
        IO.File.Copy(OriginalFilename, NewFilename, Overwrite)
        IO.File.Delete(OriginalFilename)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If OpenFileDialog1.ShowDialog = DialogResult.OK Then
            txtFilename.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Sub btnPatch_Click(sender As Object, e As EventArgs) Handles btnPatch.Click
        PatchROM(txtFilename.Text)
    End Sub
End Class
