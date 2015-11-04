Imports System.Security.Cryptography
Imports System.Text.RegularExpressions
Imports System.Web.Script.Serialization
Imports System.Windows.Forms
Imports ROMEditor.Roms
Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Public Class GenericNDSModProject
    Inherits Project

    Public Event NDSModAdded(sender As Object, e As NDSModAddedEventArgs)
    Public Event NDSModBuilding(sender As Object, e As NDSModBuildingEventArgs)

#Region "Child Classes"
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
#End Region

    Public Overrides Async Sub Initialize()
        MyBase.Initialize()
        Dim o As New OpenFileDialog
        o.Filter = PluginHelper.GetLanguageItem("Nintendo DS Roms") & " (*.nds)|*.nds|All Files (*.*)|*.*"
        If o.ShowDialog = DialogResult.OK Then
            OpenFile(o.FileName, "BaseRom.nds")
            Dim romDirectory = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "BaseRom RawFiles")
            Dim sky = DirectCast(Files("BaseRom.nds"), iPackedRom)
            Await sky.Unpack(romDirectory)
            CreateDirectory("Mods")
        Else
            MessageBox.Show(PluginHelper.GetLanguageItem("Project initialization failed.  You must supply a base ROM."))
        End If
    End Sub

    Private Async Sub NDSRomProject_FileAdded(sender As Object, e As EventArguments.FileAddedEventArguments) Handles Me.FileAdded
        If TypeOf e.File.Value Is Mods.GenericMod Then
            Dim romDirectory = IO.Path.Combine(IO.Path.GetDirectoryName(e.File.Value.OriginalFilename), IO.Path.GetFileNameWithoutExtension(e.File.Value.OriginalFilename), "RawFiles")

            Dim m = DirectCast(e.File.Value, Mods.GenericMod)

            If m.FilesToCopy.Count > 0 Then
                Dim a As New Utilities.AsyncFor(PluginHelper.GetLanguageItem("Copying files", "Copying files..."))
                Await a.RunForEach(Sub(Item As String)
                                       Dim source As String = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "BaseRom RawFiles", Item)
                                       If IO.File.Exists(source) Then
                                           Dim dest As String = IO.Path.Combine(romDirectory, Item)
                                           If Not IO.Directory.Exists(IO.Path.GetDirectoryName(dest)) Then
                                               IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(dest))
                                           End If
                                           IO.File.Copy(source, dest, True)
                                       ElseIf IO.Directory.Exists(source)
                                           For Each f In IO.Directory.GetFiles(source, "*", IO.SearchOption.AllDirectories)
                                               Dim dest As String = f.Replace(IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "BaseRom RawFiles"), romDirectory)
                                               If Not IO.Directory.Exists(IO.Path.GetDirectoryName(dest)) Then
                                                   IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(dest))
                                               End If
                                               IO.File.Copy(f, dest, True)
                                           Next
                                       End If
                                   End Sub, m.FilesToCopy)
            Else
                Await PluginHelper.CopyDirectory(IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "BaseRom RawFiles"), romDirectory)
            End If

            Await m.InitializeAsync(Me)

            PluginHelper.SetLoadingStatusFinished()
        End If
    End Sub

    Protected Overridable Function BuildMod(e As NDSModBuildingEventArgs) As Task
        Return Task.CompletedTask
    End Function

    Public Overrides Async Sub Build()
        PluginHelper.StartLoading(PluginHelper.GetLanguageItem("Building mod pack."))
        MyBase.Build()
        If IO.Directory.Exists(IO.Path.Combine(IO.Path.GetDirectoryName(Filename), PluginHelper.GetLanguageItem("ModPack Files"))) Then
            PluginHelper.DeleteDirectory(IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "ModPack Files"))
            'IO.Directory.Delete(IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "ModPack Files"), True)
        End If
        Dim j As New JavaScriptSerializer
        Dim patchers As New List(Of FilePatcher)

        'Dim baseRomFiles As String = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), PluginHelper.GetLanguageItem("BaseRom RawFiles"))
        'Dim modPackFiles As String = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), PluginHelper.GetLanguageItem("ModPack Files"))
        'Dim modPackFilesMods As String = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), PluginHelper.GetLanguageItem("ModPack Files"), PluginHelper.GetLanguageItem("Mods"))
        'Dim modPackFilesTools As String = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), PluginHelper.GetLanguageItem("ModPack Files"), PluginHelper.GetLanguageItem("Tools"))
        'Dim modPackFilesToolsPatchers As String = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), PluginHelper.GetLanguageItem("ModPack Files"), PluginHelper.GetLanguageItem("Tools"), PluginHelper.GetLanguageItem("Patchers"))

        Dim baseRomFiles As String = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "BaseRom RawFiles")
        Dim modPackFiles As String = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "ModPack Files")
        Dim modPackFilesMods As String = IO.Path.Combine(modPackFiles, "Mods")
        Dim modPackFilesTools As String = IO.Path.Combine(modPackFiles, "Tools")
        Dim modPackFilesToolsPatchers As String = IO.Path.Combine(modPackFilesTools, "Patchers")

        'Build mods
        Dim modFiles = GetFiles(GetType(Mods.GenericMod)) 'IO.Directory.GetFiles(IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "Mods"), "*.ndsmodsrc")
        For count = 0 To modFiles.Count - 1
            PluginHelper.StartLoading(String.Format(PluginHelper.GetLanguageItem("BuildingModProgress", "Building mod {0} of {1}"), count + 1, modFiles.Count))
            Dim ndsmod = DirectCast(modFiles(count), Mods.GenericMod)
            'Dim rawFilesDir As String = IO.Path.Combine(IO.Path.GetDirectoryName(ndsmod.OriginalFilename), IO.Path.GetFileNameWithoutExtension(ndsmod.OriginalFilename), PluginHelper.GetLanguageItem("RawFiles"))
            'Dim modFilesDir As String = IO.Path.Combine(IO.Path.GetDirectoryName(ndsmod.OriginalFilename), IO.Path.GetFileNameWithoutExtension(ndsmod.OriginalFilename), PluginHelper.GetLanguageItem("ModFiles"))
            'Dim modFilesFilesDir As String = IO.Path.Combine(IO.Path.GetDirectoryName(ndsmod.OriginalFilename), IO.Path.GetFileNameWithoutExtension(ndsmod.OriginalFilename), PluginHelper.GetLanguageItem("ModFiles"), PluginHelper.GetLanguageItem("Files"))
            'Dim modFilesToolsDir As String = IO.Path.Combine(IO.Path.GetDirectoryName(ndsmod.OriginalFilename), IO.Path.GetFileNameWithoutExtension(ndsmod.OriginalFilename), PluginHelper.GetLanguageItem("ModFiles"), PluginHelper.GetLanguageItem("Tools"))

            Dim rawFilesDir As String = IO.Path.Combine(IO.Path.GetDirectoryName(ndsmod.OriginalFilename), IO.Path.GetFileNameWithoutExtension(ndsmod.OriginalFilename), "RawFiles")
            Dim modFilesDir As String = IO.Path.Combine(IO.Path.GetDirectoryName(ndsmod.OriginalFilename), IO.Path.GetFileNameWithoutExtension(ndsmod.OriginalFilename), "ModFiles")
            Dim modFilesFilesDir As String = IO.Path.Combine(modFilesDir, "Files")
            Dim modFilesToolsDir As String = IO.Path.Combine(modFilesDir, "Tools")


            Await ndsmod.BuildAsync(Me)

            'Create the mod
            '-Analyze files (find out what's changed)
            Dim sourceFiles As New Dictionary(Of String, Byte())
            Using hash = MD5.Create
                For Each file In IO.Directory.GetFiles(baseRomFiles, "*", IO.SearchOption.AllDirectories)
                    sourceFiles.Add(file.Replace(baseRomFiles, "").ToLower, hash.ComputeHash(IO.File.OpenRead(file)))
                Next
            End Using

            Dim destFiles As New Dictionary(Of String, Byte())
            Using hash = MD5.Create
                For Each file In IO.Directory.GetFiles(rawFilesDir, "*", IO.SearchOption.AllDirectories)
                    destFiles.Add(file.Replace(rawFilesDir, "").ToLower, hash.ComputeHash(IO.File.OpenRead(file)))
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
                        If ndsmod.SupportsAdd Then actions.ToAdd.Add(item)
                    End If
                End If
            Next

            If ndsmod.SupportsDelete Then
                For Each item In sourceFiles.Keys
                    Dim existsDest As Boolean = destFiles.ContainsKey(item)
                    If Not existsDest Then
                        'Possible actions: delete (rename would have been detected in above iteration)
                        actions.ToDelete.Add(item)
                    End If
                Next
            End If

            'If Not IO.Directory.Exists(IO.Path.Combine(IO.Path.GetDirectoryName(ndsmod), IO.Path.GetFileNameWithoutExtension(ndsmod), "ModFiles")) Then
            '    IO.Directory.CreateDirectory(IO.Path.Combine(IO.Path.GetDirectoryName(ndsmod), IO.Path.GetFileNameWithoutExtension(ndsmod), "ModFiles"))
            'End If

            Try
                If ndsmod.ContainedObject.DependenciesBefore IsNot Nothing Then actions.DependenciesBefore.AddRange(ndsmod.ContainedObject.DependenciesBefore)
                If ndsmod.ContainedObject.DependenciesAfter IsNot Nothing Then actions.DependenciesAfter.AddRange(ndsmod.ContainedObject.DependenciesAfter)
                actions.Name = ndsmod.ContainedObject.ModName
                actions.Author = ndsmod.ContainedObject.Author
                actions.Description = ndsmod.ContainedObject.Description
                actions.UpdateUrl = ndsmod.ContainedObject.UpdateURL
            Catch ex As Exception
                MessageBox.Show(PluginHelper.GetLanguageItem("The modsrc file is invalid and will not be used."))
                actions.DependenciesBefore = Nothing
                actions.DependenciesAfter = Nothing
                actions.Name = ""
                actions.Author = ""
                actions.Description = ""
                actions.UpdateUrl = ""
            End Try

            '-Copy and write files
            If IO.Directory.Exists(modFilesDir) Then
                PluginHelper.DeleteDirectory(modFilesDir)
            End If
            IO.Directory.CreateDirectory(modFilesDir)

            IO.File.WriteAllText(IO.Path.Combine(modFilesDir, "mod.json"), j.Serialize(actions))


            For Each item In actions.ToAdd
                'Todo: remove item from toAdd if no longer exists
                If IO.File.Exists(IO.Path.Combine(rawFilesDir, item)) Then
                    If Not IO.Directory.Exists(IO.Path.GetDirectoryName(IO.Path.Combine(modFilesDir, item))) Then
                        IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(IO.Path.Combine(modFilesDir, item)))
                    End If
                    IO.File.Copy(IO.Path.Combine(rawFilesDir, item), IO.Path.Combine(modFilesDir, item), True)
                End If
            Next


            For Each item In actions.ToUpdate
                Dim patchMade As Boolean = False
                'Detect and use appropriate patching program
                For Each patcher In Me.CustomFilePatchers
                    Dim reg As New Regex(patcher.FilePath, RegexOptions.IgnoreCase)
                    If reg.IsMatch(item) Then
                        patchers.Add(patcher)
                        If Not IO.Directory.Exists(IO.Path.Combine(modFilesFilesDir, item.Trim("\"))) Then
                            IO.Directory.CreateDirectory(IO.Path.Combine(modFilesFilesDir, item.Trim("\")))
                        End If

                        Dim oldF As String = IO.Path.Combine(baseRomFiles, item.Trim("\"))
                        Dim newF As String = IO.Path.Combine(rawFilesDir, item.Trim("\"))
                        Dim patchFile As String = IO.Path.Combine(modFilesFilesDir, item.Trim("\") & "." & patcher.PatchExtension.Trim("*").Trim("."))

                        Await PluginHelper.RunProgram(IO.Path.Combine(PluginHelper.GetResourceDirectory, patcher.CreatePatchProgram), String.Format(patcher.CreatePatchArguments, oldF, newF, patchFile), False)
                        patchMade = True
                        Exit For
                    End If
                Next
                If Not patchMade Then
                    'Use xdelta for all other file types
                    If Not IO.Directory.Exists(IO.Path.GetDirectoryName(IO.Path.Combine(modFilesFilesDir, item.Trim("\")))) Then
                        IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(IO.Path.Combine(modFilesFilesDir, item.Trim("\"))))
                    End If
                    Dim oldFile As String = IO.Path.Combine(baseRomFiles, item.Trim("\"))
                    Dim oldFileTemp As String = IO.Path.Combine(PluginHelper.GetResourceName("xdelta"), "oldFile.bin")
                    Dim newFile As String = IO.Path.Combine(rawFilesDir, item.Trim("\"))
                    Dim newFileTemp As String = IO.Path.Combine(PluginHelper.GetResourceName("xdelta"), "newFile.bin")
                    Dim deltaFile As String = IO.Path.Combine(modFilesFilesDir, item.Trim("\") & ".xdelta")
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
            If Not IO.Directory.Exists(modFilesToolsDir) Then
                IO.Directory.CreateDirectory(modFilesToolsDir)
            End If
            For Each item In patchers
                IO.File.Copy(IO.Path.Combine(PluginHelper.GetResourceDirectory, item.ApplyPatchProgram), IO.Path.Combine(modFilesToolsDir, IO.Path.GetFileName(item.ApplyPatchProgram)), True)
            Next

            '-Zip Mod
            If Not IO.Directory.Exists(modPackFilesMods) Then
                IO.Directory.CreateDirectory(modPackFilesMods)
            End If
            SkyEditorBase.Utilities.Zip.Zip(modFilesDir, IO.Path.Combine(modPackFilesMods, IO.Path.GetFileNameWithoutExtension(ndsmod.OriginalFilename) & ".dsmod"))

        Next

        PluginHelper.StartLoading(PluginHelper.GetLanguageItem("Copying files", "Copying files..."))
        'Copy Patcher programs for all file formats
        If Not IO.Directory.Exists(modPackFilesTools) Then
            IO.Directory.CreateDirectory(modPackFilesTools)
        End If

        '-Copy xdelta
        'IO.File.Copy(PluginHelper.GetResourceName("xdelta/xdelta3.exe"), IO.Path.Combine(toolsDir, "xdelta3.exe"), True)
        '-Ensure xdelta is registered as a patching program
        Dim xdelta As New FilePatcher
        xdelta.ApplyPatchProgram = "xdelta\xdelta3.exe"
        xdelta.ApplyPatchArguments = "-d -n -s ""{0}"" ""{1}"" ""{2}"""
        xdelta.MergeSafe = False
        xdelta.PatchExtension = "xdelta"
        patchers.Add(xdelta)
        '-Copy patchers
        IO.File.WriteAllText(IO.Path.Combine(modPackFilesTools, "patchers.json"), j.Serialize(patchers))
        For Each item In patchers
            If Not IO.Directory.Exists(IO.Path.GetDirectoryName(IO.Path.Combine(modPackFilesToolsPatchers, item.ApplyPatchProgram))) Then
                IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(IO.Path.Combine(modPackFilesToolsPatchers, item.ApplyPatchProgram)))
            End If
            IO.File.Copy(IO.Path.Combine(PluginHelper.GetResourceDirectory, item.ApplyPatchProgram), IO.Path.Combine(modPackFilesToolsPatchers, item.ApplyPatchProgram), True)
            '--Copy Dependencies
            If item.ApplyPatchDependencies IsNot Nothing Then
                For Each d In item.ApplyPatchDependencies
                    If Not IO.Directory.Exists(IO.Path.GetDirectoryName(IO.Path.Combine(modPackFilesToolsPatchers, d.Value))) Then
                        IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(IO.Path.Combine(modPackFilesToolsPatchers, d.Value)))
                    End If
                    IO.File.Copy(IO.Path.Combine(PluginHelper.GetResourceDirectory, d.Key), IO.Path.Combine(modPackFilesToolsPatchers, d.Value), True)
                Next
            End If
        Next

        CopyPatcherProgram()

        '-Zip it
        Utilities.Zip.Zip(modPackFiles, IO.Path.Combine(IO.Path.GetDirectoryName(Filename), IO.Path.GetFileNameWithoutExtension(Filename) & ".zip"))

        'Apply patch
        PluginHelper.StartLoading(PluginHelper.GetLanguageItem("Applying patch", "Applying patch..."))

        Await ApplyPatchAsync()

        PluginHelper.StopLoading()
    End Sub
    Public Overrides Sub Run()
        MyBase.Run()
        If IO.File.Exists(IO.Path.Combine(IO.Path.GetDirectoryName(Filename), OutputRomFilename)) Then
            DeSmuMe.RunDeSmuMe(IO.Path.Combine(IO.Path.GetDirectoryName(Filename), OutputRomFilename))
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
    Public Overrides Function CreatableFiles(InternalPath As String, Manager As PluginManager) As IList(Of Type)
        If InternalPath.ToLower = "mods/" Then
            Return NDSModRegistry.GetMods(Files(BaseRomFilename).GetType)
        Else
            Return New List(Of Type)
        End If
    End Function
    ''' <summary>
    ''' Returns a dictionary that maps regular expressions of paths or files in a ROM to patching programs.
    ''' The value of the dictionary must be the filename (not the path) of a program in the ROMEditor resource directory.  Use a semi-colon (;) to separate arguments for making a patch, and another semi-colon to separate arguments for applying a patch.
    ''' Example: the key "\.kao" and value "kao.exe;-c {input1} {input2} {output};-a {input1} {input2} {output}" makes patches for all modified .kao files by running "kao.exe -c {input1} {input2} {output}", and applies patches using "kao.exe -a {input1} {input2} {output}"
    ''' </summary>
    ''' <returns></returns>
    Public Overridable Function CustomFilePatchers() As List(Of FilePatcher)
        'Todo: make this plugin-able
        Dim patchers = New List(Of FilePatcher)
        If patchers Is Nothing Then
            patchers = New List(Of FilePatcher)
        End If
        Dim LSPatcher As New FilePatcher()
        With LSPatcher
            .CreatePatchProgram = "LanguageStringPatcher.exe"
            .CreatePatchArguments = "-c ""{0}"" ""{1}"" ""{2}"""
            .ApplyPatchProgram = "LanguageStringPatcher.exe"
            .ApplyPatchArguments = "-a ""{0}"" ""{1}"" ""{2}"""
            .MergeSafe = True
            .PatchExtension = "textstrlsp"
            .FilePath = ".*text_.\.str"
        End With
        patchers.Add(LSPatcher)
        Return patchers
    End Function

    Public Overridable Function BaseRomFilename() As String
        Return "BaseRom.nds"
    End Function
    Public Overridable Function OutputRomFilename() As String
        Return "PatchedRom.nds"
    End Function

    Public Overridable Sub CopyPatcherProgram()
        Dim toolsDir = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "ModPack Files", "Tools")
        '-Copy ndstool
        IO.File.Copy(PluginHelper.GetResourceName("ndstool.exe"), IO.Path.Combine(toolsDir, "ndstool.exe"), True)

        '-Copy patching wizard
        IO.File.Copy(PluginHelper.GetResourceName("DSPatcher.exe"), IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "ModPack Files", "DSPatcher.exe"), True)
        IO.File.Copy(PluginHelper.GetResourceName("ICSharpCode.SharpZipLib.dll"), IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "ModPack Files", "ICSharpCode.SharpZipLib.dll"), True)
    End Sub
    Public Overridable Async Function ApplyPatchAsync() As Task
        If Not PluginHelper.IsMethodOverridden(Me.GetType.GetMethod("ApplyPatch")) Then
            Await PluginHelper.RunProgram(IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "ModPack Files", "DSPatcher.exe"), String.Format("""{0}"" ""{1}""", IO.Path.Combine(IO.Path.GetDirectoryName(Filename), BaseRomFilename), IO.Path.Combine(IO.Path.GetDirectoryName(Filename), OutputRomFilename)), False)

        Else
            Await Task.Run(New Action(Sub()
                                          ApplyPatch()
                                      End Sub))
        End If
    End Function
    Public Overridable Sub ApplyPatch()
    End Sub

End Class