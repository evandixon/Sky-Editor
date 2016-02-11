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

        Public Overrides Function GetFilesToCopy() As IEnumerable(Of String)
            Return {IO.Path.Combine("romfs", "script"),
                IO.Path.Combine("romfs", "message_en.bin"),
                IO.Path.Combine("romfs", "message_fr.bin"),
                IO.Path.Combine("romfs", "message_ge.bin"),
                IO.Path.Combine("romfs", "message_it.bin"),
                IO.Path.Combine("romfs", "message_sp.bin"),
                IO.Path.Combine("romfs", "message_us.bin"),
                IO.Path.Combine("romfs", "message.bin")}
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
    End Class

End Namespace
