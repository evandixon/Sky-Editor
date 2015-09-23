Imports System.Security.Cryptography
Imports System.Text.RegularExpressions
Imports System.Web.Script.Serialization
Imports System.Windows.Forms
Imports ROMEditor.Roms
Imports SkyEditorBase

Public Class GenericNDSModProject
    Inherits Project

    Public Event NDSModAdded(sender As Object, e As NDSModAddedEventArgs)
    Public Event NDSModBuilding(sender As Object, e As NDSModBuildingEventArgs)

    Public Class NDSModAddedEventArgs
        Inherits EventArgs
        Public Property InternalName As String
        Public Property File As GenericFile
    End Class

    Public Class NDSModBuildingEventArgs
        Public Property NDSModSourceFilename As String
    End Class

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
        ''' <summary>
        ''' Regular expression used to identify which file paths this FilePatcher will create and apply patches for.
        ''' </summary>
        ''' <returns></returns>
        Public Property FilePath As String
        ''' <summary>
        ''' Path of the program to create a patch, relative to ROMEditor's plugin directory.
        ''' </summary>
        ''' <returns></returns>
        Public Property CreatePatchProgram As String
        ''' <summary>
        ''' Arguments for the CreatePatchProgram.
        ''' {0} is a placeholder for the original file, {1} is a placeholder for the updated file, and {2} is a placeholder for the output patch file.
        ''' </summary>
        ''' <returns></returns>
        Public Property CreatePatchArguments As String
        ''' <summary>
        ''' Path of the patcher program, relative to ROMEditor's plugin directory.
        ''' Will be placed in the Tools directory of a mod pack, and any dependencies should be supplied by FilePatcher.ApplyPatchDependencies.
        ''' </summary>
        ''' <returns></returns>
        Public Property ApplyPatchProgram As String
        ''' <summary>
        ''' Arguments for the ApplyPatchProgram.
        ''' {0} is a placeholder for the original file, {1} is a placeholder for the patch file, and {2} is a placeholder for the output file.
        ''' </summary>
        ''' <returns></returns>
        Public Property ApplyPatchArguments As String
        ''' <summary>
        ''' Extension of the patch file.
        ''' While the FilePath regex will likely be used to identify which patcher to use to apply the patch, the extension should be unique to the patcher.
        ''' </summary>
        ''' <returns></returns>
        Public Property PatchExtension As String
        ''' <summary>
        ''' Specifies whether or not multple patches can be applied to the same file.
        ''' If false, any two mods that patch the same file will be incompatible.
        ''' </summary>
        ''' <returns></returns>
        Public Property MergeSafe As Boolean
        ''' <summary>
        ''' A dictionary of paths of files needed by the patcher.
        ''' The key is the path of the file relative to ROMEditor's plugin directory.  The value is the path it should be placed in, relative to the mod pack's Tools directory (where the patcher will be copied).
        ''' </summary>
        ''' <returns></returns>
        Public Property ApplyPatchDependencies As IDictionary(Of String, String)
    End Class

    Public Overrides Async Sub Initialize()
        MyBase.Initialize()
        Dim o As New OpenFileDialog
        o.Filter = "Nintendo DS Roms (*.nds)|*.nds|All Files (*.*)|*.*"
        If o.ShowDialog = DialogResult.OK Then
            OpenFile(o.FileName, "BaseRom.nds")
            Dim romDirectory = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "BaseRom RawFiles")
            Dim sky = DirectCast(Files("BaseRom.nds"), SkyNDSRom)
            Await sky.Unpack(romDirectory)
            CreateDirectory("Mods")
        Else
            MessageBox.Show("Project initialization failed.  You must supply a base NDS ROM.")
        End If
    End Sub

    Private Sub SkyRomProject_FileAdded(sender As Object, File As KeyValuePair(Of String, SkyEditorBase.GenericFile)) Handles Me.FileAdded
        If File.Key.ToLower.EndsWith(".ndsmodsrc") Then
            PluginHelper.StartLoading("Creating NDS mod...")
            Dim romDirectory = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "Mods", IO.Path.GetFileNameWithoutExtension(File.Key), "RawFiles")

            Dim sourcePath As String = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "BaseRom RawFiles")
            Dim files = IO.Directory.GetFiles(sourcePath, "*", IO.SearchOption.AllDirectories)
            For count = 0 To files.Count - 1
                PluginHelper.StartLoading("Copying files...", count / (files.Length - 1))
                Dim item = files(count)
                If Not IO.Directory.Exists(IO.Path.GetDirectoryName(item.Replace(sourcePath, romDirectory))) Then
                    IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(item.Replace(sourcePath, romDirectory)))
                End If
                IO.File.Copy(item, item.Replace(sourcePath, romDirectory))
            Next

            PluginHelper.StartLoading("Creating NDS mod...")
            Dim e As New NDSModAddedEventArgs
            e.InternalName = File.Key
            e.File = File.Value
            RaiseEvent NDSModAdded(Me, e)

            PluginHelper.Writeline("Done!")
            PluginHelper.StopLoading()
        End If
    End Sub

    Public Overrides Async Sub Build()
        PluginHelper.StartLoading("Building mod pack.")
        MyBase.Build()
        If IO.Directory.Exists(IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "ModPack Files")) Then
            PluginHelper.DeleteDirectory(IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "ModPack Files"))
            'IO.Directory.Delete(IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "ModPack Files"), True)
        End If
        Dim j As New JavaScriptSerializer
        Dim patchers As New List(Of FilePatcher)

        'Build mods
        Dim modFiles = IO.Directory.GetFiles(IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "Mods"), "*.ndsmodsrc")
        For count = 0 To modFiles.Length - 1
            PluginHelper.StartLoading(String.Format("Building mod {0} of {1}", count + 1, modFiles.Length))
            Dim ndsmod = modFiles(count)
            Dim modsrc = New FileFormats.NDSModSource(ndsmod)

            Dim e As New NDSModBuildingEventArgs
            e.NDSModSourceFilename = ndsmod
            RaiseEvent NDSModBuilding(Me, e)

            'Create the mod
            '-Analyze files (find out what's changed)
            Dim sourceFiles As New Dictionary(Of String, Byte())
            Using hash = MD5.Create
                For Each file In IO.Directory.GetFiles(IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "BaseRom RawFiles"), "*", IO.SearchOption.AllDirectories)
                    sourceFiles.Add(file.Replace(IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "BaseRom RawFiles"), ""), hash.ComputeHash(IO.File.OpenRead(file)))
                Next
            End Using

            Dim destFiles As New Dictionary(Of String, Byte())
            Using hash = MD5.Create
                For Each file In IO.Directory.GetFiles(IO.Path.Combine(IO.Path.GetDirectoryName(ndsmod), IO.Path.GetFileNameWithoutExtension(ndsmod), "RawFiles"), "*", IO.SearchOption.AllDirectories)
                    destFiles.Add(file.Replace(IO.Path.Combine(IO.Path.GetDirectoryName(ndsmod), IO.Path.GetFileNameWithoutExtension(ndsmod), "RawFiles"), ""), hash.ComputeHash(IO.File.OpenRead(file)))
                Next
            End Using

            Dim actions As New ModJson
            For Each item In destFiles.Keys
                Dim originalFilename As String = ""
                Dim existsSource As Boolean = sourceFiles.ContainsKey(item)
                If existsSource Then
                    'Possible actions: rename, update, none
                    If Utilities.GenericArrayOperations(Of Byte).ArraysEqual(sourceFiles(item), destFiles(item)) Then
                        'Do Nothing
                    Else
                        'Possible actions: update, rename
                        If DictionaryContainsValue(sourceFiles, destFiles(item)) Then
                            actions.ToRename.Add(item, (From f In sourceFiles Where Utilities.GenericArrayOperations(Of Byte).ArraysEqual(f.Value, destFiles(item)) Take 1 Select f.Key).ToList(0))
                        Else
                            actions.ToUpdate.Add(item)
                        End If
                    End If
                Else
                    'Possible actions: add, rename
                    If DictionaryContainsValue(sourceFiles, destFiles(item)) Then
                        actions.ToRename.Add(item, (From f In sourceFiles Where Utilities.GenericArrayOperations(Of Byte).ArraysEqual(f.Value, destFiles(item)) Take 1 Select f.Key).ToList(0))
                    Else
                        actions.ToAdd.Add(item)
                    End If
                End If
            Next
            For Each item In sourceFiles.Keys
                Dim existsDest As Boolean = destFiles.ContainsKey(item)
                If Not existsDest Then
                    'Possible actions: delete (rename would have been detected in above iteration)
                    actions.ToDelete.Add(item)
                End If
            Next
            'If Not IO.Directory.Exists(IO.Path.Combine(IO.Path.GetDirectoryName(ndsmod), IO.Path.GetFileNameWithoutExtension(ndsmod), "ModFiles")) Then
            '    IO.Directory.CreateDirectory(IO.Path.Combine(IO.Path.GetDirectoryName(ndsmod), IO.Path.GetFileNameWithoutExtension(ndsmod), "ModFiles"))
            'End If

            Try
                actions.DependenciesBefore.AddRange(modsrc.Settings("DependenciesBefore").Split(";"))
                actions.DependenciesAfter.AddRange(modsrc.Settings("DependenciesAfter").Split(";"))
                actions.Name = modsrc.Settings("Name")
                actions.Author = modsrc.Settings("Author")
                actions.Description = modsrc.Settings("Description")
                actions.UpdateUrl = modsrc.Settings("UpdateUrl")
            Catch ex As Exception
                MessageBox.Show("The ndsmodsrc file is invalid and will not be used.")
                actions.DependenciesBefore = Nothing
                actions.DependenciesAfter = Nothing
                actions.Name = ""
                actions.Author = ""
                actions.Description = ""
                actions.UpdateUrl = ""
            End Try


            '-Copy and write files
            If IO.Directory.Exists(IO.Path.Combine(IO.Path.GetDirectoryName(ndsmod), IO.Path.GetFileNameWithoutExtension(ndsmod), "ModFiles")) Then
                PluginHelper.DeleteDirectory(IO.Path.Combine(IO.Path.GetDirectoryName(ndsmod), IO.Path.GetFileNameWithoutExtension(ndsmod), "ModFiles"))
            End If
            IO.Directory.CreateDirectory(IO.Path.Combine(IO.Path.GetDirectoryName(ndsmod), IO.Path.GetFileNameWithoutExtension(ndsmod), "ModFiles"))

            IO.File.WriteAllText(IO.Path.Combine(IO.Path.GetDirectoryName(ndsmod), IO.Path.GetFileNameWithoutExtension(ndsmod), "ModFiles", "mod.json"), j.Serialize(actions))

            For Each item In actions.ToAdd
                If Not IO.Directory.Exists(IO.Path.GetDirectoryName(IO.Path.Combine(IO.Path.GetDirectoryName(ndsmod), IO.Path.GetFileNameWithoutExtension(ndsmod), "ModFiles", item))) Then
                    IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(IO.Path.Combine(IO.Path.GetDirectoryName(ndsmod), IO.Path.GetFileNameWithoutExtension(ndsmod), "ModFiles", item)))
                End If
                IO.File.Copy(IO.Path.Combine(IO.Path.GetDirectoryName(ndsmod), IO.Path.GetFileNameWithoutExtension(ndsmod), "RawFiles", item), IO.Path.Combine(IO.Path.GetDirectoryName(ndsmod), IO.Path.GetFileNameWithoutExtension(ndsmod), "ModFiles", item))
            Next

            For Each item In actions.ToUpdate
                Dim patchMade As Boolean = False
                'Detect and use appropriate patching program
                For Each patcher In Me.CustomFilePatchers
                    Dim reg As New Regex(patcher.FilePath, RegexOptions.IgnoreCase)
                    If reg.IsMatch(item) Then
                        patchers.Add(patcher)
                        If Not IO.Directory.Exists(IO.Path.GetDirectoryName(IO.Path.Combine(IO.Path.GetDirectoryName(ndsmod), IO.Path.GetFileNameWithoutExtension(ndsmod), "ModFiles", "Files", item.Trim("\")))) Then
                            IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(IO.Path.Combine(IO.Path.GetDirectoryName(ndsmod), IO.Path.GetFileNameWithoutExtension(ndsmod), "ModFiles", "Files", item.Trim("\"))))
                        End If

                        Dim oldF As String = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "BaseRom RawFiles", item.Trim("\"))
                        Dim newF As String = IO.Path.Combine(IO.Path.GetDirectoryName(ndsmod), IO.Path.GetFileNameWithoutExtension(ndsmod), "RawFiles", item.Trim("\"))
                        Dim patchFile As String = IO.Path.Combine(IO.Path.GetDirectoryName(ndsmod), IO.Path.GetFileNameWithoutExtension(ndsmod), "ModFiles", "Files", item.Trim("\") & "." & patcher.PatchExtension.Trim("*").Trim("."))

                        Await PluginHelper.RunProgram(IO.Path.Combine(PluginHelper.GetResourceDirectory, patcher.CreatePatchProgram), String.Format(patcher.CreatePatchArguments, oldF, newF, patchFile), False)
                        patchMade = True
                        Exit For
                    End If
                Next
                If Not patchMade Then
                    'Use xdelta for all other file types
                    If Not IO.Directory.Exists(IO.Path.GetDirectoryName(IO.Path.Combine(IO.Path.GetDirectoryName(ndsmod), IO.Path.GetFileNameWithoutExtension(ndsmod), "ModFiles", "Files", item.Trim("\")))) Then
                        IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(IO.Path.Combine(IO.Path.GetDirectoryName(ndsmod), IO.Path.GetFileNameWithoutExtension(ndsmod), "ModFiles", "Files", item.Trim("\"))))
                    End If
                    Dim oldFile As String = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "BaseRom RawFiles", item.Trim("\"))
                    Dim oldFileTemp As String = IO.Path.Combine(PluginHelper.GetResourceName("xdelta"), "oldFile.bin")
                    Dim newFile As String = IO.Path.Combine(IO.Path.GetDirectoryName(ndsmod), IO.Path.GetFileNameWithoutExtension(ndsmod), "RawFiles", item.Trim("\"))
                    Dim newFileTemp As String = IO.Path.Combine(PluginHelper.GetResourceName("xdelta"), "newFile.bin")
                    Dim deltaFile As String = IO.Path.Combine(IO.Path.GetDirectoryName(ndsmod), IO.Path.GetFileNameWithoutExtension(ndsmod), "ModFiles", "Files", item.Trim("\") & ".xdelta")
                    Dim deltaFileTemp As String = IO.Path.Combine(PluginHelper.GetResourceName("xdelta"), "patch.xdelta")
                    IO.File.Copy(oldFile, oldFileTemp, True)
                    IO.File.Copy(newFile, newFileTemp, True)
                    Dim path = IO.Path.Combine(PluginHelper.GetResourceDirectory, "xdelta", "xdelta3.exe")
                    Await PluginHelper.RunProgram(IO.Path.Combine(PluginHelper.GetResourceDirectory, "xdelta", "xdelta3.exe"), String.Format("-e -s ""{0}"" ""{1}"" ""{2}""", "oldFile.bin", "newFile.bin", "patch.xdelta"), False)
                    IO.File.Copy(deltaFileTemp, deltaFile)
                    IO.File.Delete(deltaFileTemp)
                    IO.File.Delete(oldFileTemp)
                    IO.File.Delete(newFileTemp)
                End If
            Next
            '-Copy Patcher programs for non-standard file formats
            If Not IO.Directory.Exists(IO.Path.Combine(IO.Path.GetDirectoryName(ndsmod), IO.Path.GetFileNameWithoutExtension(ndsmod), "ModFiles", "Tools")) Then
                IO.Directory.CreateDirectory(IO.Path.Combine(IO.Path.GetDirectoryName(ndsmod), IO.Path.GetFileNameWithoutExtension(ndsmod), "ModFiles", "Tools"))
            End If
            For Each item In patchers
                IO.File.Copy(IO.Path.Combine(PluginHelper.GetResourceDirectory, item.ApplyPatchProgram), IO.Path.Combine(IO.Path.GetDirectoryName(ndsmod), IO.Path.GetFileNameWithoutExtension(ndsmod), "ModFiles", "Tools", IO.Path.GetFileName(item.ApplyPatchProgram)))
            Next

            '-Zip Mod
            If Not IO.Directory.Exists(IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "ModPack Files", "Mods")) Then
                IO.Directory.CreateDirectory(IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "ModPack Files", "Mods"))
            End If
            SkyEditorBase.Utilities.Zip.Zip(IO.Path.Combine(IO.Path.GetDirectoryName(ndsmod), IO.Path.GetFileNameWithoutExtension(ndsmod), "ModFiles"), IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "ModPack Files", "Mods", IO.Path.GetFileNameWithoutExtension(ndsmod) & ".ndsmod"))
        Next

        PluginHelper.StartLoading("Copying files...")
        'Copy Patcher programs for all file formats
        Dim toolsDir = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "ModPack Files", "Tools")
        If Not IO.Directory.Exists(IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "ModPack Files", "Tools")) Then
            IO.Directory.CreateDirectory(IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "ModPack Files", "Tools"))
        End If
        '-Copy ndstool
        IO.File.Copy(PluginHelper.GetResourceName("ndstool.exe"), IO.Path.Combine(toolsDir, "ndstool.exe"), True)
        '-Copy xdelta
        'IO.File.Copy(PluginHelper.GetResourceName("xdelta/xdelta3.exe"), IO.Path.Combine(toolsDir, "xdelta3.exe"), True)
        '-Ensure xdelta is registered as a patching program
        Dim xdelta As New FilePatcher
        xdelta.ApplyPatchProgram = "xdelta\xdelta3.exe"
        xdelta.ApplyPatchArguments = "-d -s ""{0}"" ""{1}"" ""{2}"""
        xdelta.MergeSafe = False
        xdelta.PatchExtension = "xdelta"
        patchers.Add(xdelta)
        '-Copy patchers
        IO.File.WriteAllText(IO.Path.Combine(toolsDir, "patchers.json"), j.Serialize(patchers))
        For Each item In patchers
            If Not IO.Directory.Exists(IO.Path.GetDirectoryName(IO.Path.Combine(toolsDir, "Patchers", item.ApplyPatchProgram))) Then
                IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(IO.Path.Combine(toolsDir, "Patchers", item.ApplyPatchProgram)))
            End If
            IO.File.Copy(IO.Path.Combine(PluginHelper.GetResourceDirectory, item.ApplyPatchProgram), IO.Path.Combine(toolsDir, "Patchers", item.ApplyPatchProgram), True)
            '--Copy Dependencies
            If item.ApplyPatchDependencies IsNot Nothing Then
                For Each d In item.ApplyPatchDependencies
                    If Not IO.Directory.Exists(IO.Path.GetDirectoryName(IO.Path.Combine(toolsDir, "Patchers", d.Value))) Then
                        IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(IO.Path.Combine(toolsDir, "Patchers", d.Value)))
                    End If
                    IO.File.Copy(IO.Path.Combine(PluginHelper.GetResourceDirectory, d.Key), IO.Path.Combine(toolsDir, "Patchers", d.Value), True)
                Next
            End If
        Next
        '-Copy patching wizard
        IO.File.Copy(PluginHelper.GetResourceName("NDSPatcher.exe"), IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "ModPack Files", "NDSPatcher.exe"), True)
        IO.File.Copy(PluginHelper.GetResourceName("ICSharpCode.SharpZipLib.dll"), IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "ModPack Files", "ICSharpCode.SharpZipLib.dll"), True)
        '-Zip it
        Utilities.Zip.Zip(IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "ModPack Files"), IO.Path.Combine(IO.Path.GetDirectoryName(Filename), IO.Path.GetFileNameWithoutExtension(Filename) & ".zip"))

        'Apply patch
        PluginHelper.StartLoading("Applying patch...")
        Await PluginHelper.RunProgram(IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "ModPack Files", "NDSPatcher.exe"), String.Format("""{0}"" ""{1}""", IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "BaseRom.nds"), IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "PatchedRom.nds")), False)

        PluginHelper.StopLoading()
    End Sub
    Public Overrides Sub Run()
        MyBase.Run()
        If IO.File.Exists(IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "PatchedRom.nds")) Then
            DeSmuMe.RunDeSmuMe(IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "PatchedRom.nds"))
        End If
    End Sub
    Private Function DictionaryContainsValue(Dictionary As Dictionary(Of String, Byte()), Value As Byte()) As Boolean
        Dim out As Boolean = False
        For Each item In Dictionary
            If Utilities.GenericArrayOperations(Of Byte).ArraysEqual(item.Value, Value) Then
                out = True
                Exit For
            End If
        Next
        Return out
    End Function
    Public Overrides Function CanCreateDirectory(InternalPath As String) As Boolean
        Return False
    End Function
    Public Overrides Function CreatableFiles(InternalPath As String, Manager As PluginManager) As IList(Of String)
        If InternalPath.ToLower = "mods/" Then
            Return New List(Of String)({GameStrings.NDSModSourceFile})
        Else
            Return New List(Of String)
        End If
    End Function
    ''' <summary>
    ''' Returns a dictionary that maps regular expressions of paths or files in a ROM to patching programs.
    ''' The value of the dictionary must be the filename (not the path) of a program in the ROMEditor resource directory.  Use a semi-colon (;) to separate arguments for making a patch, and another semi-colon to separate arguments for applying a patch.
    ''' Example: the key "\.kao" and value "kao.exe;-c {input1} {input2} {output};-a {input1} {input2} {output}" makes patches for all modified .kao files by running "kao.exe -c {input1} {input2} {output}", and applies patches using "kao.exe -a {input1} {input2} {output}"
    ''' </summary>
    ''' <returns></returns>
    Public Overridable Function CustomFilePatchers() As List(Of FilePatcher)
        Return New List(Of FilePatcher)
    End Function
End Class
