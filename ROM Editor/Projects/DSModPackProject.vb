Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Utilities
Imports SkyEditor.Core.Windows
Imports SkyEditor.Core.Windows.Processes

Namespace Projects
    Public Class DSModPackProject
        Inherits Project
        Implements IContainer(Of ModpackInfo)

        Public Property Info As ModpackInfo Implements IContainer(Of ModpackInfo).Item
            Get
                Return Me.Setting("ModpackInfo")
            End Get
            Set(value As ModpackInfo)
                Me.Setting("ModpackInfo") = value
            End Set
        End Property

        Public Property BaseRomProject As String
            Get
                Return Me.Setting("BaseRomProject")
            End Get
            Set(value As String)
                Me.Setting("BaseRomProject") = value
            End Set
        End Property

        Public Property Output3DSFile As Boolean
            Get
                If Me.Setting("Output3DSFile") Is Nothing Then
                    Me.Setting("Output3DSFile") = True
                End If
                Return Me.Setting("Output3DSFile")
            End Get
            Set(value As Boolean)
                Me.Setting("Output3DSFile") = value
            End Set
        End Property

        Public Property OutputCIAFile As Boolean
            Get
                If Me.Setting("OutputCIAFile") Is Nothing Then
                    Me.Setting("OutputCIAFile") = False
                End If
                Return Me.Setting("OutputCIAFile")
            End Get
            Set(value As Boolean)
                Me.Setting("OutputCIAFile") = value
            End Set
        End Property

        Public Property OutputHans As Boolean
            Get
                If Me.Setting("OutputHans") Is Nothing Then
                    Me.Setting("OutputHans") = False
                End If
                Return Me.Setting("OutputHans")
            End Get
            Set(value As Boolean)
                Me.Setting("OutputHans") = value
            End Set
        End Property


        ''' <summary>
        ''' Gets the directory non-project mods are stored in.
        ''' </summary>
        ''' <returns></returns>
        Public Overridable Function GetSourceModsDir() As String
            Return IO.Path.Combine(GetRootDirectory, "Mods")
        End Function

        'Gets the directory mods in the current modpack build are stored in.
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

        ''' <summary>
        ''' Gets the directory the modpack will be outputted to.
        ''' </summary>
        ''' <returns></returns>
        Public Overridable Function OutputDir() As String
            Return IO.Path.Combine(IO.Path.GetDirectoryName(Me.Filename), "Output")
        End Function

        Public Overridable Function GetBaseRomFilename(Solution As Solution) As String
            Dim p As BaseRomProject = Solution.GetProjectsByName(BaseRomProject).FirstOrDefault
            Return p.GetRawFilesDir
            ' Return p.GetProjectItemByPath("/BaseRom").GetFilename
        End Function

        Public Overridable Function GetBaseRomSystem(Solution As Solution) As String
            Dim p As BaseRomProject = Solution.GetProjectsByName(BaseRomProject).FirstOrDefault
            Return p.RomSystem
        End Function
        Public Overridable Function GetBaseGameCode(Solution As Solution) As String
            Dim p As BaseRomProject = Solution.GetProjectsByName(BaseRomProject).FirstOrDefault
            Return p.GameCode
        End Function
        Public Overridable Function GetLocalSmdhPath() As String
            Return IO.Path.Combine(GetRootDirectory, "Modpack.smdh")
        End Function

        Public Overrides Function CanBuild() As Boolean
            Dim p As BaseRomProject = ParentSolution.GetProjectsByName(BaseRomProject).FirstOrDefault
            Return (p IsNot Nothing)
        End Function

        Public Overrides Async Function Build() As Task
            Await DoBuild()
            Await MyBase.Build()
        End Function

        Protected Async Function DoBuild() As Task
            Const patcherVersion As String = "alpha 4"
            Dim modpackDir = GetModPackDir()
            Dim modpackModsDir = GetModsDir()
            Dim modpackToolsDir = GetToolsDir()
            Dim modpackToolsPatchersDir = GetPatchersDir()
            Dim modsSourceDir = GetSourceModsDir()

            Await FileSystem.ReCreateDirectory(modpackDir, CurrentPluginManager.CurrentIOProvider)

            If Not IO.Directory.Exists(modpackModsDir) Then
                IO.Directory.CreateDirectory(modpackModsDir)
            End If
            If Not IO.Directory.Exists(modpackToolsDir) Then
                IO.Directory.CreateDirectory(modpackToolsDir)
            End If
            If Not IO.Directory.Exists(modpackToolsPatchersDir) Then
                IO.Directory.CreateDirectory(modpackToolsPatchersDir)
            End If
            If Not IO.Directory.Exists(modsSourceDir) Then
                IO.Directory.CreateDirectory(modsSourceDir)
            End If

            Await FileSystem.ReCreateDirectory(OutputDir, CurrentPluginManager.CurrentIOProvider)

            'Copy external mods
            For Each item In IO.Directory.GetFiles(modsSourceDir)
                Dim sourceFilename = item
                Dim destFilename = IO.Path.Combine(modpackModsDir, IO.Path.GetFileName(sourceFilename))
                IO.File.Copy(sourceFilename, destFilename, True)
            Next

            'Copy mods from other projects
            For Each item In Me.GetReferences(ParentSolution)
                If TypeOf item Is GenericModProject Then
                    Dim sourceFilename = DirectCast(item, GenericModProject).GetModOutputFilename(BaseRomProject)
                    Dim destFilename = IO.Path.Combine(modpackModsDir, IO.Path.GetFileName(sourceFilename))
                    If IO.File.Exists(sourceFilename) Then
                        IO.File.Copy(sourceFilename, destFilename, True)
                    End If
                End If
            Next

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
            IO.File.WriteAllText(IO.Path.Combine(modpackToolsDir, "patchers.json"), Json.Serialize(patchers))
            For Each item In patchers
                If Not IO.Directory.Exists(IO.Path.GetDirectoryName(IO.Path.Combine(GetPatchersDir, item.ApplyPatchProgram))) Then
                    IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(IO.Path.Combine(GetPatchersDir, item.ApplyPatchProgram)))
                End If
                IO.File.Copy(IO.Path.Combine(EnvironmentPaths.GetResourceDirectory, item.ApplyPatchProgram), IO.Path.Combine(GetPatchersDir, item.ApplyPatchProgram), True)
                '--Copy Dependencies
                If item.ApplyPatchDependencies IsNot Nothing Then
                    For Each d In item.ApplyPatchDependencies
                        If Not IO.Directory.Exists(IO.Path.GetDirectoryName(IO.Path.Combine(GetPatchersDir, d.Value))) Then
                            IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(IO.Path.Combine(GetPatchersDir, d.Value)))
                        End If
                        IO.File.Copy(IO.Path.Combine(EnvironmentPaths.GetResourceDirectory, d.Key), IO.Path.Combine(GetPatchersDir, d.Value), True)
                    Next
                End If
            Next

            CopyPatcherProgram(ParentSolution)

            'Update modpack info
            Me.Info.GameCode = GetBaseGameCode(ParentSolution)
            Me.Info.System = GetBaseRomSystem(ParentSolution)
            If IO.File.Exists(GetLocalSmdhPath) Then
                IO.File.Copy(GetLocalSmdhPath, IO.Path.Combine(modpackModsDir, "Modpack.smdh"), True)
            End If
            IO.File.WriteAllText(IO.Path.Combine(modpackModsDir, "Modpack Info"), Json.Serialize(Me.Info))

            '-Zip it
            Zip.Zip(modpackDir, IO.Path.Combine(OutputDir, Me.Info.Name & " " & Me.Info.Version & "-" & patcherVersion & ".zip"))

            'Apply patch
            Me.BuildProgress = 0.9
            Me.BuildStatusMessage = My.Resources.Language.LoadingApplyingPatch

            Await ApplyPatchAsync(ParentSolution)

            Me.BuildProgress = 1
            Me.BuildStatusMessage = My.Resources.Language.Complete
        End Function

        Public Overridable Sub CopyPatcherProgram(Solution As Solution)
            Using external As New ExternalProgramManager
                Select Case GetBaseRomSystem(Solution)
                    Case "3DS"
                        '-Copy ctrtool
                        IO.File.Copy(external.GetCtrToolPath, IO.Path.Combine(GetToolsDir, "ctrtool.exe"), True)
                        'IO.File.Copy(PluginHelper.GetResourceName("makerom.exe"), IO.Path.Combine(GetToolsDir, "makerom.exe"), True)
                        'IO.File.Copy(PluginHelper.GetResourceName("rom_tool.exe"), IO.Path.Combine(GetToolsDir, "rom_tool.exe"), True)
                        IO.File.Copy(EnvironmentPaths.GetResourceName("3DS Builder.exe"), IO.Path.Combine(GetToolsDir, "3DS Builder.exe"), True)
                    Case "NDS"
                        '-Copy ndstool
                        IO.File.Copy(EnvironmentPaths.GetResourceName("ndstool.exe"), IO.Path.Combine(GetToolsDir, "ndstool.exe"), True)
                    Case Else
                        Throw New NotSupportedException(String.Format(My.Resources.Language.ErrorModPackTypeNotSupported, GetBaseRomSystem(Solution)))
                End Select

                '-Copy patching wizard
                IO.File.Copy(EnvironmentPaths.GetResourceName("DSPatcher.exe"), IO.Path.Combine(GetModPackDir, "DSPatcher.exe"), True)
                IO.File.Copy(EnvironmentPaths.GetResourceName("ICSharpCode.SharpZipLib.dll"), IO.Path.Combine(GetModPackDir, "ICSharpCode.SharpZipLib.dll"), True)
            End Using
        End Sub

        Public Overridable Async Function ApplyPatchAsync(Solution As Solution) As Task
            Select Case GetBaseRomSystem(Solution)
                Case "3DS"
                    If Output3DSFile Then
                        Await ConsoleApp.RunProgram(IO.Path.Combine(GetModPackDir, "DSPatcher.exe"), String.Format("""{0}"" ""{1}""", GetBaseRomFilename(Solution), IO.Path.Combine(OutputDir, "PatchedRom.3ds")))
                    End If
                    If OutputCIAFile Then
                        Await ConsoleApp.RunProgram(IO.Path.Combine(GetModPackDir, "DSPatcher.exe"), String.Format("""{0}"" ""{1}""", GetBaseRomFilename(Solution), IO.Path.Combine(OutputDir, "PatchedRom.cia")))
                    End If
                    If OutputHans Then
                        Await ConsoleApp.RunProgram(IO.Path.Combine(GetModPackDir, "DSPatcher.exe"), String.Format("""{0}"" ""{1}"" -hans", GetBaseRomFilename(Solution), IO.Path.Combine(OutputDir, "Hans SD")))
                    End If
                Case "NDS"
                    Await ConsoleApp.RunProgram(IO.Path.Combine(GetModPackDir, "DSPatcher.exe"), String.Format("""{0}"" ""{1}""", GetBaseRomFilename(Solution), IO.Path.Combine(OutputDir, "PatchedRom.nds")))
            End Select
        End Function
    End Class

End Namespace
