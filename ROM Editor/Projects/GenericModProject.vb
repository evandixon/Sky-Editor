Imports System.Security.Cryptography
Imports System.Text.RegularExpressions
Imports SkyEditorBase
Namespace Projects
    Public Class GenericModProject
        Inherits Project

#Region "Project Settings"
        Public Property ModName As String
            Get
                Return Setting("ModName")
            End Get
            Set(value As String)
                Setting("ModName") = value
            End Set
        End Property

        Public Property ModVersion As String
            Get
                Return Setting("ModVersion")
            End Get
            Set(value As String)
                Setting("ModVersion") = value
            End Set
        End Property

        Public Property ModAuthor As String
            Get
                Return Setting("ModAuthor")
            End Get
            Set(value As String)
                Setting("ModAuthor") = value
            End Set
        End Property

        Public Property ModDescription As String
            Get
                Return Setting("ModDescription")
            End Get
            Set(value As String)
                Setting("ModDescription") = value
            End Set
        End Property

        Public Property Homepage As String
            Get
                Return Setting("Homepage")
            End Get
            Set(value As String)
                Setting("Homepage") = value
            End Set
        End Property

        Public Property ModDependenciesBefore As List(Of String)
            Get
                Return Setting("Dependencies-Before")
            End Get
            Set(value As List(Of String))
                Setting("Dependencies-Before") = value
            End Set
        End Property

        Public Property ModDependenciesAfter As List(Of String)
            Get
                Return Setting("Dependencies-After")
            End Get
            Set(value As List(Of String))
                Setting("Dependencies-After") = value
            End Set
        End Property
#End Region

#Region "Project Type Properties"
        Public Overrides Function CanCreateDirectory(Path As String) As Boolean
            Return False
        End Function

        Public Overrides Function CanCreateFile(Path As String) As Boolean
            Return False
        End Function

        Public Overrides Function CanDeleteDirectory(Path As String) As Boolean
            Return False
        End Function

        Public Overrides Function CanAddExistingFile(Path As String) As Boolean
            Return False
        End Function

        Public Overrides Function CanDeleteFile(FilePath As String) As Boolean
            Return False
        End Function

        Public Overridable Function GetSupportedGameCodes() As IEnumerable(Of String)
            Return {".*"}
        End Function

        Public Overrides Function CanBuild(Solution As Solution) As Boolean
            Return True
        End Function
#End Region

        Public Overridable Function GetCustomFilePatchers() As IEnumerable(Of FilePatcher)
            Return {}
        End Function

        ''' <summary>
        ''' Gets the paths of the files or directories to copy on initialization, relative to the the root RawFiles directory.
        ''' If empty, will copy everything.
        ''' </summary>
        ''' <returns></returns>
        Public Overridable Function GetFilesToCopy(Solution As Solution, BaseRomProjectName As String) As IEnumerable(Of String)
            Return {}
        End Function

        Public Overridable Function SupportsAdd() As Boolean
            Return True
        End Function

        Public Overridable Function SupportsDelete() As Boolean
            Return False
        End Function

#Region "File Paths"
        Public Overridable Function GetRawFilesSourceDir(Solution As Solution, SourceProjectName As String) As String
            Dim baseRomProject As BaseRomProject = Solution.GetProjectsByName(SourceProjectName).FirstOrDefault
            Return baseRomProject.GetRawFilesDir
        End Function

        Public Overridable Function GetModRootOutputDir() As String
            Return IO.Path.Combine(GetRootDirectory, "Output")
        End Function

        Public Overridable Function GetModOutputDir(SourceProjectName As String)
            Return IO.Path.Combine(GetModRootOutputDir, SourceProjectName)
        End Function

        Public Overridable Function GetModOutputFilename(SourceProjectName As String)
            Return IO.Path.Combine(GetModOutputDir(SourceProjectName), ModName & ".mod")
        End Function

        Public Overridable Function GetRawFilesDir() As String
            Return IO.Path.Combine(GetRootDirectory, "Raw Files")
        End Function

        Public Overridable Function GetModTempDir() As String
            Return IO.Path.Combine(GetRootDirectory, "Mod Files")
        End Function
#End Region


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

        Public Overridable Async Function Initialize(Solution As Solution) As Task
            If Me.ProjectReferences.Count > 0 Then
                Dim filesToCopy = Me.GetFilesToCopy(Solution, Me.ProjectReferences(0))
                Dim sourceRoot = GetRawFilesSourceDir(Solution, Me.ProjectReferences(0))
                If filesToCopy.Count = 1 Then
                    Dim source As String = IO.Path.Combine(sourceRoot, filesToCopy(0))
                    Dim dest As String = IO.Path.Combine(GetRawFilesDir, filesToCopy(0))
                    If IO.File.Exists(source) Then
                        If Not IO.Directory.Exists(IO.Path.GetDirectoryName(dest)) Then
                            IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(dest))
                        End If
                        IO.File.Copy(source, dest, True)
                    ElseIf IO.Directory.Exists(source) Then
                        Await Utilities.FileSystem.CopyDirectory(source, dest, True)
                    End If
                ElseIf filesToCopy.Count > 0 Then
                    Dim a As New Utilities.AsyncFor(PluginHelper.GetLanguageItem("Copying files", "Copying files..."))
                    Await a.RunForEach(Sub(Item As String)
                                           Dim source As String = IO.Path.Combine(sourceRoot, Item)
                                           If IO.File.Exists(source) Then
                                               Dim dest As String = IO.Path.Combine(GetRawFilesDir, Item)
                                               If Not IO.Directory.Exists(IO.Path.GetDirectoryName(dest)) Then
                                                   IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(dest))
                                               End If
                                               IO.File.Copy(source, dest, True)
                                           ElseIf IO.Directory.Exists(source) Then

                                               For Each f In IO.Directory.GetFiles(source, "*", IO.SearchOption.AllDirectories)
                                                   Dim dest As String = f.Replace(sourceRoot, GetRawFilesDir)
                                                   If Not IO.Directory.Exists(IO.Path.GetDirectoryName(dest)) Then
                                                       IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(dest))
                                                   End If
                                                   IO.File.Copy(f, dest, True)
                                               Next
                                           End If
                                       End Sub, filesToCopy)
                Else
                    Await Utilities.FileSystem.CopyDirectory(sourceRoot, GetRawFilesDir, True)
                End If
                Else
                    'Since there's no source project, we'll leave it up to the user to supply the needed files.
                    If Not IO.Directory.Exists(GetRawFilesDir) Then
                    IO.Directory.CreateDirectory(GetRawFilesDir)
                End If
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Solution"></param>
        ''' <returns></returns>
        ''' <remarks>If this is overridden, do custom work, THEN use MyBase.Build</remarks>
        Public Overrides Async Function Build(Solution As Solution) As Task
            For Each sourceProjectName In Me.ProjectReferences
                Dim sourceRoot = GetRawFilesSourceDir(Solution, sourceProjectName)
                Dim currentFiles = GetRawFilesDir()
                Dim modTemp = GetModTempDir()
                Dim modTempFiles = IO.Path.Combine(modTemp, "Files")
                Dim modTempTools = IO.Path.Combine(modTemp, "Tools")
                Dim modOutput = GetModOutputDir(sourceProjectName)

                Dim patchers As New List(Of FilePatcher)
                Dim actions As New ModJson

                Await Task.Run(Async Function() As Task
                                   Me.BuildProgress = 0
                                   Me.BuildStatusMessage = PluginHelper.GetLanguageItem("Analyzing files")
                                   'Create the mod
                                   '-Find all the files
                                   Dim sourceFiles As New Dictionary(Of String, Byte())
                                   For Each file In IO.Directory.GetFiles(sourceRoot, "*", IO.SearchOption.AllDirectories)
                                       sourceFiles.Add(file.Replace(sourceRoot, "").ToLower, {})
                                   Next


                                   Dim destFiles As New Dictionary(Of String, Byte())
                                   For Each file In IO.Directory.GetFiles(currentFiles, "*", IO.SearchOption.AllDirectories)
                                       Dim part = file.Replace(currentFiles, "").ToLower
                                       If Not file.ToLower.EndsWith(".skyproj") AndAlso Not part.StartsWith("\output") AndAlso Not part.StartsWith("\mod files") Then 'In case the raw files are stored in the project root
                                           destFiles.Add(part, {})
                                       End If
                                   Next

                                   '-Analyze files
                                   '(Only calculate hashes for files that exist on both sides, to save on time
                                   Dim hashToCalcSource As New List(Of String)
                                   Dim hashToCalcDest As New List(Of String)
                                   For Each destFile In destFiles.Keys
                                       For Each sourceFile In sourceFiles.Keys
                                           If String.Equals(destFile, sourceFile, StringComparison.InvariantCultureIgnoreCase) Then
                                               hashToCalcSource.Add(sourceFile)
                                               hashToCalcDest.Add(destFile)
                                           End If
                                       Next
                                   Next

                                   Me.BuildProgress = 0
                                   Me.BuildStatusMessage = PluginHelper.GetLanguageItem("Computing hashes")

                                   Dim tasks As New List(Of Task)
                                   Dim completed As Integer = 0
                                   '-Compute the hashes
                                   For count = 0 To hashToCalcSource.Count - 1
                                       Dim c = count
                                       tasks.Add(Task.Run(New Action(Sub()
                                                                         Using h = MD5.Create
                                                                             Me.BuildProgress = completed / (hashToCalcSource.Count + hashToCalcDest.Count)
                                                                             Using source = IO.File.OpenRead(IO.Path.Combine(sourceRoot, hashToCalcSource(c).TrimStart("\")))
                                                                                 sourceFiles(hashToCalcSource(c)) = h.ComputeHash(source)
                                                                             End Using
                                                                             completed += 1
                                                                         End Using
                                                                     End Sub)))
                                   Next

                                   For count = 0 To hashToCalcDest.Count - 1
                                       Dim c = count
                                       tasks.Add(Task.Run(New Action(Sub()
                                                                         Using h = MD5.Create
                                                                             Me.BuildProgress = completed / (hashToCalcSource.Count + hashToCalcDest.Count)
                                                                             Using dest = IO.File.OpenRead(IO.Path.Combine(currentFiles, hashToCalcDest(c).TrimStart("\")))
                                                                                 destFiles(hashToCalcDest(c)) = h.ComputeHash(dest)
                                                                             End Using
                                                                             completed += 1
                                                                         End Using
                                                                     End Sub)))
                                   Next

                                   Await Task.WhenAll(tasks)



                                   Me.BuildProgress = 0
                                   Me.BuildStatusMessage = PluginHelper.GetLanguageItem("Comparing files")
                                   '-Analyze the differences
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
                                                   actions.ToRename.Add(item, (From s In sourceFiles Where Utilities.GenericArrayOperations(Of Byte).ArraysEqual(s.Value, destFiles(item)) Take 1 Select s.Key).ToList(0))
                                               Else
                                                   actions.ToUpdate.Add(item)
                                               End If
                                           End If
                                       Else
                                           'Possible actions: add, rename
                                           If DictionaryContainsValue(sourceFiles, destFiles(item)) Then
                                               actions.ToRename.Add(item, (From d In sourceFiles Where Utilities.GenericArrayOperations(Of Byte).ArraysEqual(d.Value, destFiles(item)) Take 1 Select d.Key).ToList(0))
                                           Else
                                               If Me.SupportsAdd Then
                                                   actions.ToAdd.Add(item)
                                               End If
                                           End If
                                       End If
                                   Next

                                   If Me.SupportsDelete Then
                                       For Each item In sourceFiles.Keys
                                           Dim existsDest As Boolean = destFiles.ContainsKey(item)
                                           If Not existsDest Then
                                               'Possible actions: delete (rename would have been detected in above iteration)
                                               actions.ToDelete.Add(item)
                                           End If
                                       Next
                                   End If
                               End Function)


                actions.DependenciesBefore = ModDependenciesBefore
                actions.DependenciesAfter = ModDependenciesAfter
                actions.Name = ModName
                actions.Author = ModAuthor
                actions.Description = ModDescription
                actions.UpdateUrl = Homepage

                '-Copy and write files
                Await Utilities.FileSystem.ReCreateDirectory(modTemp)

                IO.File.WriteAllText(IO.Path.Combine(modTemp, "mod.json"), SkyEditorBase.Utilities.Json.Serialize(actions))

                Me.BuildProgress = 0
                Me.BuildStatusMessage = PluginHelper.GetLanguageItem("Generating patch")

                For Each item In actions.ToAdd
                    'Todo: remove item from toAdd if no longer exists
                    If IO.File.Exists(IO.Path.Combine(sourceRoot, item)) Then
                        If Not IO.Directory.Exists(IO.Path.GetDirectoryName(IO.Path.Combine(modTempFiles, item))) Then
                            IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(IO.Path.Combine(modTempFiles, item)))
                        End If
                        IO.File.Copy(IO.Path.Combine(sourceRoot, item), IO.Path.Combine(modTempFiles, item), True)
                    End If
                Next

                Dim f As New Utilities.AsyncFor(PluginHelper.GetLanguageItem("Generating patch"))
                f.SetLoadingStatus = False
                f.SetLoadingStatusOnFinish = False
                Dim onProgressChanged = Sub(sender As Object, e As EventArguments.LoadingStatusChangedEventArgs)
                                            Me.BuildStatusMessage = e.Message
                                            Me.BuildProgress = e.Progress
                                        End Sub
                AddHandler f.LoadingStatusChanged, onProgressChanged

                Await f.RunForEach(Of String)(Async Function(Item As String) As Task
                                                  Dim patchMade As Boolean = False
                                                  'Detect and use appropriate patching program
                                                  For Each patcher In Me.GetCustomFilePatchers
                                                      Dim reg As New Regex(patcher.FilePath, RegexOptions.IgnoreCase)
                                                      If reg.IsMatch(Item) Then
                                                          If patcher IsNot Nothing AndAlso Not patchers.Contains(patcher) Then
                                                              patchers.Add(patcher)
                                                          End If
                                                          If Not IO.Directory.Exists(IO.Path.Combine(modTempFiles, Item.Trim("\"))) Then
                                                              IO.Directory.CreateDirectory(IO.Path.Combine(modTempFiles, Item.Trim("\")))
                                                          End If

                                                          Dim oldF As String = IO.Path.Combine(sourceRoot, Item.Trim("\"))
                                                          Dim newF As String = IO.Path.Combine(currentFiles, Item.Trim("\"))
                                                          Dim patchFile As String = IO.Path.Combine(modTempFiles, Item.Trim("\") & "." & patcher.PatchExtension.Trim("*").Trim("."))

                                                          Await PluginHelper.RunProgram(IO.Path.Combine(PluginHelper.GetResourceDirectory, patcher.CreatePatchProgram), String.Format(patcher.CreatePatchArguments, oldF, newF, patchFile), False)
                                                          patchMade = True
                                                          Exit For
                                                      End If
                                                  Next
                                                  If Not patchMade Then
                                                      'Use xdelta for all other file types
                                                      If Not IO.Directory.Exists(IO.Path.GetDirectoryName(IO.Path.Combine(modTempFiles, Item.Trim("\")))) Then
                                                          IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(IO.Path.Combine(modTempFiles, Item.Trim("\"))))
                                                      End If
                                                      Dim tmpVal As String = Guid.NewGuid.ToString
                                                      Dim oldFile As String = IO.Path.Combine(sourceRoot, Item.Trim("\"))
                                                      Dim oldFileTemp As String = IO.Path.Combine(PluginHelper.GetResourceName("xdelta"), $"oldFile-{tmpVal}.bin")
                                                      Dim newFile As String = IO.Path.Combine(currentFiles, Item.Trim("\"))
                                                      Dim newFileTemp As String = IO.Path.Combine(PluginHelper.GetResourceName("xdelta"), $"newFile-{tmpVal}.bin")
                                                      Dim deltaFile As String = IO.Path.Combine(modTempFiles, Item.Trim("\") & ".xdelta")
                                                      Dim deltaFileTemp As String = IO.Path.Combine(PluginHelper.GetResourceName("xdelta"), $"patch-{tmpVal}.xdelta")
                                                      IO.File.Copy(oldFile, oldFileTemp, True)
                                                      IO.File.Copy(newFile, newFileTemp, True)
                                                      Dim path = IO.Path.Combine(PluginHelper.GetResourceDirectory, "xdelta", "xdelta3.exe")
                                                      Await PluginHelper.RunProgram(IO.Path.Combine(PluginHelper.GetResourceDirectory, "xdelta", "xdelta3.exe"), String.Format("-e -s ""{0}"" ""{1}"" ""{2}""", $"oldFile-{tmpVal}.bin", $"newFile-{tmpVal}.bin", $"patch-{tmpVal}.xdelta"), False)
                                                      IO.File.Copy(deltaFileTemp, deltaFile)
                                                      IO.File.Delete(deltaFileTemp)
                                                      IO.File.Delete(oldFileTemp)
                                                      IO.File.Delete(newFileTemp)
                                                  End If
                                              End Function, actions.ToUpdate)
                '-Copy Patcher programs for non-standard file formats
                'XDelta will be copied with the modpack
                If Not IO.Directory.Exists(modTempTools) Then
                    IO.Directory.CreateDirectory(modTempTools)
                End If
                For Each item In patchers
                    IO.File.Copy(IO.Path.Combine(PluginHelper.GetResourceDirectory, item.ApplyPatchProgram), IO.Path.Combine(modTempTools, IO.Path.GetFileName(item.ApplyPatchProgram)), True)
                Next
                Utilities.Json.SerializeToFile(IO.Path.Combine(modTempTools, "patchers.json"), patchers)

                '-Zip Mod
                If Not IO.Directory.Exists(modOutput) Then
                    IO.Directory.CreateDirectory(modOutput)
                End If
                SkyEditorBase.Utilities.Zip.Zip(modTemp, GetModOutputFilename(sourceProjectName))


            Next
            Me.BuildProgress = 1
            Me.BuildStatusMessage = PluginHelper.GetLanguageItem("Complete")
        End Function

        Public Sub New()
            MyBase.New
            Me.ModDependenciesBefore = New List(Of String)
            Me.ModDependenciesAfter = New List(Of String)
        End Sub
    End Class

End Namespace
