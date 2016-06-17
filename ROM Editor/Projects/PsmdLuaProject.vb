Imports System.Collections.Concurrent
Imports System.Text.RegularExpressions
Imports CodeFiles
Imports ROMEditor.FileFormats.PSMD
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Utilities
Imports SkyEditor.Core.Windows
Imports SkyEditor.Core.Windows.Processes
Namespace Projects
    Public Class PsmdLuaProject
        Inherits GenericModProject
        Implements CodeFiles.ICodeProject

        Public Overrides Function GetCustomFilePatchers() As IEnumerable(Of FilePatcher)
            Dim patchers = New List(Of FilePatcher)
            If patchers Is Nothing Then
                patchers = New List(Of FilePatcher)
            End If
            Dim MSPatcher As New FilePatcher()
            With MSPatcher
                .CreatePatchProgram = "MessageFARCPatcher.exe"
                .CreatePatchArguments = "-c ""{0}"" ""{1}"" ""{2}"""
                .ApplyPatchProgram = "MessageFARCPatcher.exe"
                .ApplyPatchArguments = "-a ""{0}"" ""{1}"" ""{2}"""
                .MergeSafe = True
                .PatchExtension = "msgFarcT5"
                .FilePath = ".*message_?.*\.bin"
            End With
            patchers.Add(MSPatcher)
            Return patchers
        End Function

        Private Property LanguageIDDictionary As ConcurrentDictionary(Of UInteger, Boolean)

        ''' <summary>
        ''' Gets the task that loads the Language file IDs, to avoid duplicate keys.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property LanguageLoadTask As Task
            Get
                Return _languageLoadTask
            End Get
        End Property
        Dim _languageLoadTask As Task

        Public Function IsLanguageLoaded() As Boolean
            Return LanguageLoadTask IsNot Nothing AndAlso LanguageLoadTask.IsCompleted
        End Function

        Public Async Function GetNewLanguageID() As Task(Of UInteger)
            'Note: with the current system of logging every ID in use, excessively large numbers of IDs in use will cause the computer to run out of memory.
            'If this happens, we won't be able to store the IDs in memory, and we will instead need to check everything one file at a time.
            'Chances are this won't be a problem.  GTI contains something over 5,000,000 strings.  Just the IDs will take about 20MB of RAM, which should be easy to come by.
            'This could only be a problem because the range of IDs is 0 to UInt32.MaxValue, and besides storage space, there's no technical limitations to having that many strings.

            Dim newID As Integer = 0

            If LanguageLoadTask Is Nothing Then
                LoadLanguageIDs()
            End If

            Await LanguageLoadTask

            'Validate the ID
Validate:
            For Each item In LanguageIDDictionary.Keys
                If item = newID AndAlso LanguageIDDictionary(item) = True Then
                    'Then this ID is in use.  Increment by 1 and try again
                    newID += 1
                    GoTo Validate
                End If
            Next

            'the ID must be valid.  Let's register it.
            LanguageIDDictionary(newID) = True

            Return newID
        End Function

        Private Sub LoadLanguageIDs()
            'Load language IDs
            Dim dir = IO.Path.Combine(Me.GetRootDirectory, "Languages")
            If IO.Directory.Exists(dir) Then
                Dim langDirs = IO.Directory.GetDirectories(dir)
                Dim f1 As New AsyncFor(My.Resources.Language.LoadingLanguages)
                f1.BatchSize = langDirs.Length
                _languageLoadTask = f1.RunForEach(Async Function(Item As String)
                                                      Dim lang = IO.Path.GetFileNameWithoutExtension(Item)

                                                      Dim f2 As New AsyncFor
                                                      Await f2.RunForEach(Async Function(File As String) As Task
                                                                              Using msg As New MessageBin(True)
                                                                                  Await msg.OpenFileOnlyIDs(File, CurrentPluginManager.CurrentIOProvider)

                                                                                  For Each entry In msg.Strings
                                                                                      'If LanguageIDs(lang).Contains(entry.Hash) Then
                                                                                      '    'Todo: throw an error of some sort
                                                                                      'Else
                                                                                      LanguageIDDictionary(entry.Hash) = True
                                                                                      'End If
                                                                                  Next
                                                                              End Using
                                                                          End Function, IO.Directory.GetFiles(Item))
                                                  End Function, langDirs)
            End If
        End Sub

        Private Async Function StartExtractLanguages() As Task
            'PSMD style
            Dim languageNameRegex As New Text.RegularExpressions.Regex(".*message_?(.*)\.bin", RegexOptions.IgnoreCase)
            Dim languageFileNames = IO.Directory.GetFiles(IO.Path.Combine(Me.GetRawFilesDir, "romfs"), "message*.bin", IO.SearchOption.TopDirectoryOnly)
            Dim f As New AsyncFor
            AddHandler f.LoadingStatusChanged, Sub(sender As Object, e As LoadingStatusChangedEventArgs)
                                                   Me.BuildProgress = e.Progress
                                               End Sub
            f.RunSynchronously = True
            Await f.RunForEach(Async Function(item As String) As Task
                                   Dim lang = "jp"

                                   Dim match = languageNameRegex.Match(item)
                                   If match.Success AndAlso Not String.IsNullOrEmpty(match.Groups(1).Value) Then
                                       lang = match.Groups(1).Value
                                   End If

                                   Dim destDir = IO.Path.Combine(Me.GetRootDirectory, "Languages", lang)
                                   Await FileSystem.ReCreateDirectory(destDir, CurrentPluginManager.CurrentIOProvider)

                                   Dim farc As New FarcF5
                                   Await farc.OpenFile(item, CurrentPluginManager.CurrentIOProvider)
                                   Await farc.Extract(destDir)
                               End Function, languageFileNames)

            'GTI style
            Dim languageDirNameRegex As New Text.RegularExpressions.Regex(".*message_?(.*)", RegexOptions.IgnoreCase)
            Dim languageDirFilenames = IO.Directory.GetDirectories(IO.Path.Combine(Me.GetRawFilesDir, "romfs"), "message*", IO.SearchOption.TopDirectoryOnly)
            Dim f2 As New AsyncFor
            AddHandler f2.LoadingStatusChanged, Sub(sender As Object, e As LoadingStatusChangedEventArgs)
                                                    Me.BuildProgress = e.Progress
                                                End Sub
            f.RunSynchronously = True
            Await f2.RunForEach(Async Function(item As String) As Task
                                    Dim lang = "en"

                                    Dim match = languageDirNameRegex.Match(item)
                                    If match.Success AndAlso Not String.IsNullOrEmpty(match.Groups(1).Value) Then
                                        lang = match.Groups(1).Value
                                    End If

                                    Dim destDir = IO.Path.Combine(Me.GetRootDirectory, "Languages", lang)
                                    Await FileSystem.ReCreateDirectory(destDir, CurrentPluginManager.CurrentIOProvider)
                                    Await FileSystem.CopyDirectory(item, destDir, CurrentPluginManager.CurrentIOProvider)
                                End Function, languageDirFilenames)
        End Function

        Private _languageExtractTask As Task
        ''' <summary>
        ''' Extracts the language files.
        ''' If called multiple times, only extracts once.
        ''' </summary>
        ''' <returns></returns>
        Protected Function ExtractLanguages() As Task
            If _languageExtractTask Is Nothing Then
                _languageExtractTask = StartExtractLanguages()
            End If
            Return _languageExtractTask
        End Function

        Private Async Sub GenericModProject_ProjectOpened(sender As Object, e As EventArgs) Handles Me.ProjectOpened
            'Fix bug from Beta 1
            'Re-extract the language files if they don't exist
            If IO.Directory.Exists(IO.Path.Combine(Me.GetRootDirectory, "Languages")) AndAlso IO.Directory.GetFiles(IO.Path.Combine(Me.GetRootDirectory, "Languages"), "*", IO.SearchOption.AllDirectories).Length = 0 Then
                Await ExtractLanguages()
            End If
        End Sub

        Protected Overrides Async Function Initialize() As Task
            Await MyBase.Initialize

            Me.BuildStatusMessage = My.Resources.Language.LoadingExtractingLanguages
            Me.BuildProgress = 0
            Me.IsBuildProgressIndeterminate = False

            Await ExtractLanguages()

            Me.BuildStatusMessage = My.Resources.Language.LoadingDecompilingScripts
            Me.BuildProgress = 0

            Dim scriptSource As String = IO.Path.Combine(Me.GetRawFilesDir, "romfs", "script")
            Dim scriptDestination As String = IO.Path.Combine(Me.GetRootDirectory, "script")
            Dim filesToOpen As New List(Of String)

            Dim f As New AsyncFor

            AddHandler f.LoadingStatusChanged, Sub(sender As Object, e As LoadingStatusChangedEventArgs)
                                                   Me.BuildProgress = e.Progress
                                               End Sub

            'f.BatchSize = Environment.ProcessorCount * 2

            Await f.RunForEach(Async Function(Item As String) As Task
                                   Dim dest = Item.Replace(scriptSource, scriptDestination)
                                   If Not IO.Directory.Exists(IO.Path.GetDirectoryName(dest)) Then
                                       IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(dest))
                                   End If

                                   Await unluac.DecompileToFile(Item, dest)
                                   IO.File.Copy(dest, dest & ".original")
                                   filesToOpen.Add(dest)

                                   'Add the file to the project
                                   'Dim d = IO.Path.GetDirectoryName(dest).Replace(scriptDestination, "script")
                                   'Me.CreateDirectory(d)
                                   'Await Me.AddExistingFile(d, Item, False)
                               End Function, IO.Directory.GetFiles(scriptSource, "*.lua", IO.SearchOption.AllDirectories))

            'Me.BuildStatusMessage = My.Resources.Language.LoadingAddingFiles
            'Me.BuildProgress = 0
            'Dim f2 As New AsyncFor()
            'f2.RunSynchronously = True

            'AddHandler f2.LoadingStatusChanged, Sub(sender As Object, e As LoadingStatusChangedEventArgs)
            '                                        Me.BuildProgress = e.Progress
            '                                    End Sub

            'Await f2.RunForEach(Async Function(Item As String) As Task

            '                        Me.CreateDirectory(d)
            '                        Await Me.AddExistingFile(d, Item, CurrentPluginManager.CurrentIOProvider)
            '                    End Function, filesToOpen)

            Dim batchAdd As New List(Of Project.AddExistingFileBatchOperation)
            For Each item In filesToOpen
                Dim d = IO.Path.GetDirectoryName(item).Replace(scriptDestination, "script")
                batchAdd.Add(New Project.AddExistingFileBatchOperation With {.ParentPath = d, .ActualFilename = item})
            Next
            Await Me.RecreateRootWithExistingFiles(batchAdd, CurrentPluginManager.CurrentIOProvider)

            BuildProgress = 1
            Me.BuildStatusMessage = My.Resources.Language.Complete
        End Function

        Protected Overrides Async Function DoBuild() As Task
            Dim farcMode As Boolean = False

            If IO.Directory.GetFiles(IO.Path.Combine(Me.GetRawFilesDir, "romfs"), "message*").Length > 0 Then
                farcMode = True
            End If

            If farcMode Then
                Await Task.Run(Async Function() As Task
                                   Dim dirs = IO.Directory.GetDirectories(IO.Path.Combine(Me.GetRootDirectory, "Languages"))
                                   Me.BuildStatusMessage = My.Resources.Language.LoadingBuildingLanguageFiles
                                   For count = 0 To dirs.Length - 1
                                       Me.BuildProgress = count / dirs.Length
                                       Dim newFilename As String = "message_" & IO.Path.GetFileNameWithoutExtension(dirs(count)) & ".bin"
                                       Dim newFilePath As String = IO.Path.Combine(IO.Path.Combine(Me.GetRawFilesDir, "romfs", newFilename.Replace("_jp", "")))
                                       Await FarcF5.Pack(dirs(count), newFilePath, CurrentPluginManager.CurrentIOProvider)
                                   Next
                                   Me.BuildProgress = 1
                               End Function)
            Else
                'Then we're in GTI directory mode
                Await Task.Run(Async Function() As Task
                                   Dim dirs = IO.Directory.GetDirectories(IO.Path.Combine(Me.GetRootDirectory, "Languages"))
                                   Me.BuildStatusMessage = My.Resources.Language.LoadingBuildingLanguageFiles
                                   For count = 0 To dirs.Length - 1
                                       Me.BuildProgress = count / dirs.Length
                                       Dim newFilename As String = "message_" & IO.Path.GetFileNameWithoutExtension(dirs(count))
                                       Dim newFilePath As String = IO.Path.Combine(IO.Path.Combine(Me.GetRawFilesDir, "romfs", newFilename.Replace("_en", "")))
                                       Await FileSystem.CopyDirectory(dirs(count), newFilePath, CurrentPluginManager.CurrentIOProvider)
                                   Next
                                   Me.BuildProgress = 1
                               End Function)
            End If

            Dim scriptDestination As String = IO.Path.Combine(Me.GetRawFilesDir, "romfs", "script")
            Dim scriptSource As String = IO.Path.Combine(Me.GetRootDirectory, "script")

            Dim toCompile = From d In IO.Directory.GetFiles(scriptSource, "*.lua", IO.SearchOption.AllDirectories) Where Not d.StartsWith(scriptDestination) Select d

            Me.BuildStatusMessage = My.Resources.Language.LoadingCompilingScripts
            Dim f As New AsyncFor
            Dim onProgressChanged = Sub(sender As Object, e As LoadingStatusChangedEventArgs)
                                        Me.BuildProgress = e.Progress
                                    End Sub
            AddHandler f.LoadingStatusChanged, onProgressChanged
            Await f.RunForEach(Async Function(Item As String) As Task
                                   Dim sourceText = IO.File.ReadAllText(Item)
                                   Dim sourceOrig = IO.File.ReadAllText(Item & ".original")

                                   If Not sourceText = sourceOrig Then
                                       Dim dest = Item.Replace(scriptSource, scriptDestination)
                                       Await ConsoleApp.RunProgram(EnvironmentPaths.GetResourceName("lua/luac5.1.exe"), $"-o ""{dest}"" ""{Item}""")
                                   End If
                               End Function, toCompile)
            RemoveHandler f.LoadingStatusChanged, onProgressChanged
            Await MyBase.DoBuild
        End Function

        Public Overrides Function GetFilesToCopy(Solution As Solution, BaseRomProjectName As String) As IEnumerable(Of String)
            Dim project As Project = Solution.GetProjectsByName(BaseRomProjectName).FirstOrDefault
            If project IsNot Nothing AndAlso TypeOf project Is BaseRomProject Then
                Dim code = DirectCast(project, BaseRomProject).GameCode
                Dim psmd As New Regex(GameStrings.PSMDCode)
                Dim gti As New Regex(GameStrings.GTICode)
                If psmd.IsMatch(code) Then
                    Return {IO.Path.Combine("romfs", "script"),
                            IO.Path.Combine("romfs", "message_en.bin"),
                            IO.Path.Combine("romfs", "message_fr.bin"),
                            IO.Path.Combine("romfs", "message_ge.bin"),
                            IO.Path.Combine("romfs", "message_it.bin"),
                            IO.Path.Combine("romfs", "message_sp.bin"),
                            IO.Path.Combine("romfs", "message_us.bin"),
                            IO.Path.Combine("romfs", "message.bin")}
                ElseIf gti.IsMatch(code) Then
                    Return {IO.Path.Combine("romfs", "script"),
                            IO.Path.Combine("romfs", "message_fr"),
                            IO.Path.Combine("romfs", "message_ge"),
                            IO.Path.Combine("romfs", "message_it"),
                            IO.Path.Combine("romfs", "message_sp"),
                            IO.Path.Combine("romfs", "message")}
                Else
                    Return {IO.Path.Combine("romfs", "script")}
                End If
            Else
                Return {IO.Path.Combine("romfs", "script")}
            End If
        End Function

        Public Overrides Function GetSupportedGameCodes() As IEnumerable(Of String)
            Return {GameStrings.GTICode, GameStrings.PSMDCode}
        End Function

        Public Function GetExtraData(Code As CodeFile) As CodeExtraData Implements ICodeProject.GetExtraData
            Dim filenameTemplate = EnvironmentPaths.GetResourceName("Code/psmdLuaInfo-{0}.fdd")
            Dim filenameCurrent = String.Format(filenameTemplate, "English") 'SettingsManager.Instance.Settings.CurrentLanguage)
            ' Dim filenameDefault = String.Format(filenameTemplate, SettingsManager.Instance.Settings.DefaultLanguage)
            If IO.File.Exists(filenameCurrent) Then
                Return New CodeExtraDataFile(filenameCurrent, CurrentPluginManager.CurrentIOProvider)
                'ElseIf IO.File.Exists(filenameDefault) Then
                '    Return New CodeExtraDataFile(filenameDefault)
            Else
                Return New CodeExtraDataFile
            End If
        End Function

        Public Sub New()
            MyBase.New
            LanguageIDDictionary = New ConcurrentDictionary(Of UInteger, Boolean)
        End Sub
    End Class

End Namespace
