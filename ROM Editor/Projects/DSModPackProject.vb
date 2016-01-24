Imports SkyEditorBase
Namespace Projects
    Public Class DSModPackProject
        Inherits Project
        Public Overridable Function GetModsDir() As String
            Return IO.Path.Combine(GetModPackDir, "Mods")
        End Function
        Public Overridable Function GetToolsDir() As String
            Return IO.Path.Combine(GetModPackDir, "Tools")
        End Function
        Public Overridable Function GetPatchersDir() As String
            Return IO.Path.Combine(GetModPackDir, "Tools", "Patchers")
        End Function
        Public Overridable Function GetModPackDir() As String
            Return IO.Path.Combine(IO.Path.GetDirectoryName(Me.Filename), "Modpack Files")
        End Function
        Public Overridable Function OutputDir() As String
            Return IO.Path.Combine(IO.Path.GetDirectoryName(Me.Filename), "Output")
        End Function

        Public Overridable Function GetBaseRomFilename(Solution As Solution) As String
            Dim baseRomProject As BaseRomProject = Solution.GetProjectsByName(Solution.Setting("BaseRomProject")).FirstOrDefault
            Return baseRomProject.GetProjectItemByPath("/BaseRom").GetFilename
        End Function

        Public Overrides Async Function Build(Solution As Solution) As Task
            Const patcherVersion As String = "alpha 4"
            Dim modpackDir = GetModPackDir()
            Dim modpackModsDir = GetModsDir()
            Dim modpackToolsDir = GetToolsDir()
            Dim modpackToolsPatchersDir = GetPatchersDir()


            If Not IO.Directory.Exists(modpackDir) Then
                IO.Directory.CreateDirectory(modpackDir)
            End If
            If Not IO.Directory.Exists(modpackModsDir) Then
                IO.Directory.CreateDirectory(modpackModsDir)
            End If
            If Not IO.Directory.Exists(modpackToolsDir) Then
                IO.Directory.CreateDirectory(modpackToolsDir)
            End If
            If Not IO.Directory.Exists(modpackToolsPatchersDir) Then
                IO.Directory.CreateDirectory(modpackToolsPatchersDir)
            End If
            If Not IO.Directory.Exists(OutputDir) Then
                IO.Directory.CreateDirectory(OutputDir)
            End If

            Dim patchers As New List(Of FilePatcher)
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
            IO.File.WriteAllText(IO.Path.Combine(modpackToolsDir, "patchers.json"), Utilities.Json.Serialize(patchers))
            For Each item In patchers
                If Not IO.Directory.Exists(IO.Path.GetDirectoryName(IO.Path.Combine(GetPatchersDir, item.ApplyPatchProgram))) Then
                    IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(IO.Path.Combine(GetPatchersDir, item.ApplyPatchProgram)))
                End If
                IO.File.Copy(IO.Path.Combine(PluginHelper.GetResourceDirectory, item.ApplyPatchProgram), IO.Path.Combine(GetPatchersDir, item.ApplyPatchProgram), True)
                '--Copy Dependencies
                If item.ApplyPatchDependencies IsNot Nothing Then
                    For Each d In item.ApplyPatchDependencies
                        If Not IO.Directory.Exists(IO.Path.GetDirectoryName(IO.Path.Combine(GetPatchersDir, d.Value))) Then
                            IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(IO.Path.Combine(GetPatchersDir, d.Value)))
                        End If
                        IO.File.Copy(IO.Path.Combine(PluginHelper.GetResourceDirectory, d.Key), IO.Path.Combine(GetPatchersDir, d.Value), True)
                    Next
                End If
            Next

            CopyPatcherProgram(Solution)

            IO.File.WriteAllText(IO.Path.Combine(modpackModsDir, "Modpack Info"), Utilities.Json.Serialize(Me.Setting("ModpackInfo")))

            '-Zip it
            Utilities.Zip.Zip(modpackDir, IO.Path.Combine(OutputDir, Me.Setting("ModName") & " " & Me.Setting("ModVersion") & "-" & patcherVersion & ".zip"))

            'Apply patch
            PluginHelper.StartLoading(PluginHelper.GetLanguageItem("Applying patch", "Applying patch..."))

            Await ApplyPatchAsync(Solution)
        End Function

        Public Overridable Sub CopyPatcherProgram(Solution As Solution)
            Select Case Solution.Setting("System")
                Case "3DS"
                    '-Copy ctrtool
                    IO.File.Copy(PluginHelper.GetResourceName("ctrtool.exe"), IO.Path.Combine(GetToolsDir, "ctrtool.exe"), True)
                    IO.File.Copy(PluginHelper.GetResourceName("3DS Builder.exe"), IO.Path.Combine(GetToolsDir, "3DS Builder.exe"), True)
                Case "NDS"
                    '-Copy ndstool
                    IO.File.Copy(PluginHelper.GetResourceName("ndstool.exe"), IO.Path.Combine(GetToolsDir, "ndstool.exe"), True)
                Case Else
                    PluginHelper.Writeline("Unknown system.  Not copying appropriate tools.", PluginHelper.LineType.Error)
            End Select

            '-Copy patching wizard
            IO.File.Copy(PluginHelper.GetResourceName("DSPatcher.exe"), IO.Path.Combine(GetModPackDir, "DSPatcher.exe"), True)
            IO.File.Copy(PluginHelper.GetResourceName("ICSharpCode.SharpZipLib.dll"), IO.Path.Combine(GetModPackDir, "ICSharpCode.SharpZipLib.dll"), True)
        End Sub

        Public Overridable Async Function ApplyPatchAsync(Solution As Solution) As Task
            Select Case Solution.Setting("System")
                Case "3DS"
                    Await PluginHelper.RunProgram(IO.Path.Combine(GetModPackDir, "DSPatcher.exe"), String.Format("""{0}"" ""{1}""", GetBaseRomFilename(Solution), IO.Path.Combine(OutputDir, "PatchedRom.3ds")), False)
                Case "NDS"
                    Await PluginHelper.RunProgram(IO.Path.Combine(GetModPackDir, "DSPatcher.exe"), String.Format("""{0}"" ""{1}""", GetBaseRomFilename(Solution), IO.Path.Combine(OutputDir, "PatchedRom.nds")), False)
            End Select
        End Function
    End Class

End Namespace
