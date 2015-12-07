Imports System.ComponentModel
Imports System.Reflection
Imports System.Web.Script.Serialization
Imports ICSharpCode.SharpZipLib.Zip

Public Class Form2
    Private Property is3dsMode As Boolean
    Private WithEvents core As PatcherCore
    Private Property Mods As List(Of ModJson)
    Private Async Sub Form2_Load(sender As Object, e As EventArgs) Handles Me.Load
        If IO.File.Exists("Tools/ctrtool.exe") Then
            is3dsMode = True
            Me.Text = String.Format("{0} Patcher v{1}", "3DS", Assembly.GetExecutingAssembly.GetName.Version.ToString)
            core = New ThreeDSPatcherCore
        Else
            is3dsMode = False
            Me.Text = String.Format("{0} Patcher v{1}", "NDS", Assembly.GetExecutingAssembly.GetName.Version.ToString)
            core = New NDSPatcherCore
        End If

        'Unpack Mods
        Dim currentDirectory = Environment.CurrentDirectory
        Dim modTempDirectory = IO.Path.Combine(currentDirectory, "Tools/modstemp")

        If Not IO.Directory.Exists(modTempDirectory) Then
            IO.Directory.CreateDirectory(modTempDirectory)
        End If

        Dim modFiles = IO.Directory.GetFiles(IO.Path.Combine(currentDirectory, "Mods"), "*.dsmod", IO.SearchOption.TopDirectoryOnly)
        Dim completed As Integer = 0

        lblStatus.Text = "Unpacking Mods..."
        For Each item In modFiles
            pbProgress.Value = completed / modFiles.Count

            Dim z As New FastZip
            If Not IO.Directory.Exists(IO.Path.Combine(modTempDirectory, IO.Path.GetFileNameWithoutExtension(item))) Then
                IO.Directory.CreateDirectory(IO.Path.Combine(modTempDirectory, IO.Path.GetFileNameWithoutExtension(item)))
            End If
            z.ExtractZip(item, IO.Path.Combine(modTempDirectory, IO.Path.GetFileNameWithoutExtension(item)), ".*")

            completed += 1
        Next

        'Load Mods
        Dim modDirs = IO.Directory.GetDirectories(modTempDirectory, "*", IO.SearchOption.TopDirectoryOnly)
        Dim total As Integer = modDirs.Count
        Dim j As New JavaScriptSerializer
        Mods = New List(Of ModJson)

        lblStatus.Text = "Opening Mods..."
        completed = 0

        For Each item In modDirs
            pbProgress.Value = completed / total

            Dim jsonFile = IO.Path.Combine(item, "mod.json")
            If IO.File.Exists(jsonFile) Then
                Dim m = j.Deserialize(Of ModJson)(IO.File.ReadAllText(jsonFile))
                m.Filename = jsonFile
                Mods.Add(m)
                completed += 1
            Else
                total -= 1
            End If
        Next

        lblStatus.Text = "Ready"

        Dim args = Environment.GetCommandLineArgs
        If args.Count > 2 Then
            btnBrowse.Enabled = False
            btnPatch.Enabled = False
            core.SelectedFilename = args(1)
            For Each item In Mods
                If core.SupportsMod(item) Then
                    chbMods.Items.Add(item, True)
                End If
            Next

            Dim items As New List(Of ModJson)
            For Each item In chbMods.CheckedItems
                items.Add(item)
            Next
            Await core.RunPatch(items, args(2))

            Me.Close()
        End If
    End Sub

    Private Sub btnBrowse_Click(sender As Object, e As EventArgs) Handles btnBrowse.Click
        core.PromptFilePath()
        txtInput.Text = core.SelectedFilename
        'Display supported mods
        For Each item In Mods
            If core.SupportsMod(item) Then
                chbMods.Items.Add(item, True)
            End If
        Next
    End Sub

    Private Sub chbMods_SelectedIndexChanged(sender As Object, e As EventArgs) Handles chbMods.SelectedIndexChanged
        If chbMods.SelectedIndex > -1 Then
            txtDescription.Text = DirectCast(chbMods.SelectedItem, ModJson).GetDescription
        End If
    End Sub

    Private Async Sub btnPatch_Click(sender As Object, e As EventArgs) Handles btnPatch.Click
        Dim items As New List(Of ModJson)
        For Each item In chbMods.CheckedItems
            items.Add(item)
        Next
        Await core.RunPatch(items)
    End Sub

    Private Sub Form2_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Dim currentDirectory = Environment.CurrentDirectory
        Dim modTempDirectory = IO.Path.Combine(currentDirectory, "Tools/modstemp")
        If IO.Directory.Exists(modTempDirectory) Then
            For Each item In IO.Directory.GetDirectories(modTempDirectory, "*", IO.SearchOption.TopDirectoryOnly)
                IO.Directory.Delete(item, True)
            Next
        End If
    End Sub

    Private Sub core_ProgressChanged(sender As Object, e As PatcherCore.ProgressChangedEventArgs) Handles core.ProgressChanged
        If InvokeRequired Then
            Invoke(Sub()
                       lblStatus.Text = e.Message
                       pbProgress.Value = e.Progress
                   End Sub)
        Else
            lblStatus.Text = e.Message
            pbProgress.Value = e.Progress
        End If
    End Sub
End Class