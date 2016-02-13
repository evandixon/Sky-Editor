Imports System.Collections.Concurrent
Imports System.Text.RegularExpressions
Imports CodeFiles
Imports SkyEditorBase
Namespace Projects
    Public Class PsmdLuaProject
        Inherits GenericModProject
        Implements CodeFiles.ICodeProject

        'Public Overrides Function GetCustomFilePatchers() As IEnumerable(Of FilePatcher)
        '    Dim patchers = New List(Of FilePatcher)
        '    If patchers Is Nothing Then
        '        patchers = New List(Of FilePatcher)
        '    End If
        '    Dim MSPatcher As New FilePatcher()
        '    With MSPatcher
        '        .CreatePatchProgram = "MessageFARCPatcher.exe"
        '        .CreatePatchArguments = "-c ""{0}"" ""{1}"" ""{2}"""
        '        .ApplyPatchProgram = "MessageFARCPatcher.exe"
        '        .ApplyPatchArguments = "-a ""{0}"" ""{1}"" ""{2}"""
        '        .MergeSafe = True
        '        .PatchExtension = "msgFarcT5"
        '        .FilePath = ".*message_?.*\.bin"
        '    End With
        '    patchers.Add(MSPatcher)
        '    Return patchers
        'End Function

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
                Dim f1 As New Utilities.AsyncFor(PluginHelper.GetLanguageItem("Loading languages"))
                _languageLoadTask = f1.RunForEach(Async Function(Item As String)
                                                      Dim lang = IO.Path.GetFileNameWithoutExtension(Item)

                                                      Dim f2 As New Utilities.AsyncFor
                                                      Await f2.RunForEach(Sub(File As String)
                                                                              Using msg As New FileFormats.MessageBin(True)
                                                                                  msg.OpenFileOnlyIDs(File)

                                                                                  For Each entry In msg.Strings
                                                                                      'If LanguageIDs(lang).Contains(entry.Hash) Then
                                                                                      '    'Todo: throw an error of some sort
                                                                                      'Else
                                                                                      LanguageIDDictionary(entry.Hash) = True
                                                                                      'End If
                                                                                  Next
                                                                              End Using
                                                                          End Sub, IO.Directory.GetFiles(Item))
                                                  End Function, langDirs, langDirs.Count)
            End If
        End Sub

        Private Sub GenericModProject_ProjectOpened(sender As Object, e As EventArgs) Handles Me.ProjectOpened
            'LoadLanguageIDs()
        End Sub

        Public Overrides Async Function Initialize(Solution As Solution) As Task
            Await MyBase.Initialize(Solution)

            PluginHelper.SetLoadingStatus(PluginHelper.GetLanguageItem("Extracting Language Files..."))
            Dim languageNameRegex As New Text.RegularExpressions.Regex(".*message_?(.*)\.bin", RegexOptions.IgnoreCase)
            Dim languageFileNames = IO.Directory.GetFiles(IO.Path.Combine(Me.GetRawFilesDir, "romfs"), "message*.bin", IO.SearchOption.TopDirectoryOnly)
            For Each item In languageFileNames
                Dim lang = "jp"

                Dim match = languageNameRegex.Match(item)
                If match.Success AndAlso Not String.IsNullOrEmpty(match.Groups(1).Value) Then
                    lang = match.Groups(1).Value
                End If

                Dim destDir = IO.Path.Combine(Me.GetRootDirectory, "Languages", lang)
                Await Utilities.FileSystem.ReCreateDirectory(destDir)

                Dim farc As New FileFormats.FarcF5
                farc.OpenFile(item)
                Await farc.Extract(destDir)
            Next

            Dim languageDirNameRegex As New Text.RegularExpressions.Regex(".*message_?(.*)", RegexOptions.IgnoreCase)
            Dim languageDirFilenames = IO.Directory.GetDirectories(IO.Path.Combine(Me.GetRawFilesDir, "romfs"), "message*", IO.SearchOption.TopDirectoryOnly)
            For Each item In languageDirFilenames
                Dim lang = "en"

                Dim match = languageDirNameRegex.Match(item)
                If match.Success AndAlso Not String.IsNullOrEmpty(match.Groups(1).Value) Then
                    lang = match.Groups(1).Value
                End If

                Dim destDir = IO.Path.Combine(Me.GetRootDirectory, "Languages", lang)
                Await Utilities.FileSystem.ReCreateDirectory(destDir)
                Await Utilities.FileSystem.CopyDirectory(item, destDir, True)
            Next

            Dim scriptSource As String = IO.Path.Combine(Me.GetRawFilesDir, "romfs", "script")
            Dim scriptDestination As String = IO.Path.Combine(Me.GetRootDirectory, "script")
            Dim filesToOpen As New List(Of String)

            Dim f As New Utilities.AsyncFor(PluginHelper.GetLanguageItem("Decompiling Scripts..."))
            Await f.RunForEach(Async Function(Item As String) As Task
                                   Dim unlua As New unluac(Item)
                                   Dim script As String = Await unlua.Decompile
                                   Dim dest = Item.Replace(scriptSource, scriptDestination)
                                   If Not IO.Directory.Exists(IO.Path.GetDirectoryName(dest)) Then
                                       IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(dest))
                                   End If
                                   IO.File.WriteAllText(dest, script)
                                   IO.File.WriteAllText(dest & ".original", script)

                                   filesToOpen.Add(dest)
                               End Function, IO.Directory.GetFiles(scriptSource, "*.lua", IO.SearchOption.AllDirectories))

            Dim f2 As New Utilities.AsyncFor(PluginHelper.GetLanguageItem("Adding Files..."))
            Await f2.RunForEachSync(Async Function(Item As String) As Task
                                        Dim d = IO.Path.GetDirectoryName(Item).Replace(scriptDestination, "script")
                                        Me.CreateDirectory(d)
                                        Await Me.AddExistingFile(d, Item, False)
                                    End Function, filesToOpen)

            PluginHelper.SetLoadingStatusFinished()
        End Function

        Public Overrides Async Function Build(Solution As Solution) As Task
            Dim farcMode As Boolean = False

            If IO.Directory.GetFiles(IO.Path.Combine(Me.GetRawFilesDir, "romfs"), "message*").Length > 0 Then
                farcMode = True
            End If

            If farcMode Then
                Await Task.Run(Async Function() As Task
                                   Dim dirs = IO.Directory.GetDirectories(IO.Path.Combine(Me.GetRootDirectory, "Languages"))
                                   Me.BuildStatusMessage = PluginHelper.GetLanguageItem("Building language files")
                                   For count = 0 To dirs.Length - 1
                                       Me.BuildProgress = count / dirs.Length
                                       Dim newFilename As String = "message_" & IO.Path.GetFileNameWithoutExtension(dirs(count)) & ".bin"
                                       Dim newFilePath As String = IO.Path.Combine(IO.Path.Combine(Me.GetRawFilesDir, "romfs", newFilename.Replace("_jp", "")))
                                       Await FileFormats.FarcF5.Pack(dirs(count), newFilePath)
                                   Next
                                   Me.BuildProgress = 1
                               End Function)
            Else
                'Then we're in GTI directory mode
                Await Task.Run(Async Function() As Task
                                   Dim dirs = IO.Directory.GetDirectories(IO.Path.Combine(Me.GetRootDirectory, "Languages"))
                                   Me.BuildStatusMessage = PluginHelper.GetLanguageItem("Building language files")
                                   For count = 0 To dirs.Length - 1
                                       Me.BuildProgress = count / dirs.Length
                                       Dim newFilename As String = "message_" & IO.Path.GetFileNameWithoutExtension(dirs(count))
                                       Dim newFilePath As String = IO.Path.Combine(IO.Path.Combine(Me.GetRawFilesDir, "romfs", newFilename.Replace("_en", "")))
                                       Await Utilities.FileSystem.CopyDirectory(dirs(count), newFilePath)
                                   Next
                                   Me.BuildProgress = 1
                               End Function)
            End If

            Dim scriptDestination As String = IO.Path.Combine(Me.GetRawFilesDir, "romfs", "script")
            Dim scriptSource As String = IO.Path.Combine(Me.GetRootDirectory, "script")

            Dim toCompile = From d In IO.Directory.GetFiles(scriptSource, "*.lua", IO.SearchOption.AllDirectories) Where Not d.StartsWith(scriptDestination) Select d

            Dim f As New Utilities.AsyncFor(PluginHelper.GetLanguageItem("Compiling Scripts..."))
            f.SetLoadingStatus = False
            f.SetLoadingStatusOnFinish = False
            Dim onProgressChanged = Sub(sender As Object, e As EventArguments.LoadingStatusChangedEventArgs)
                                        Me.BuildStatusMessage = e.Message
                                        Me.BuildProgress = e.Progress
                                    End Sub
            AddHandler f.LoadingStatusChanged, onProgressChanged
            Await f.RunForEach(Async Function(Item As String) As Task
                                   Dim sourceText = IO.File.ReadAllText(Item)
                                   Dim sourceOrig = IO.File.ReadAllText(Item & ".original")

                                   If Not sourceText = sourceOrig Then
                                       Dim dest = Item.Replace(scriptSource, scriptDestination)
                                       Await PluginHelper.RunProgram(PluginHelper.GetResourceName("lua/luac5.1.exe"), $"-o ""{dest}"" ""{Item}""", False)
                                   End If
                               End Function, toCompile)
            RemoveHandler f.LoadingStatusChanged, onProgressChanged
            Await MyBase.Build(Solution)
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
            Dim filenameTemplate = PluginHelper.GetResourceName("Code/psmdLuaInfo-{0}.fdd")
            Dim filenameCurrent = String.Format(filenameTemplate, SettingsManager.Instance.Settings.CurrentLanguage)
            Dim filenameDefault = String.Format(filenameTemplate, SettingsManager.Instance.Settings.DefaultLanguage)
            If IO.File.Exists(filenameCurrent) Then
                Return New CodeExtraDataFile(filenameCurrent)
            ElseIf IO.File.Exists(filenameDefault) Then
                Return New CodeExtraDataFile(filenameDefault)
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
