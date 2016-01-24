Imports SkyEditorBase
Namespace Projects
    Public Class PsmdLuaProject
        Inherits GenericModProject
        Public Overrides Async Function Initialize(Solution As Solution) As Task
            Await MyBase.Initialize(Solution)

            Dim scriptSource As String = IO.Path.Combine(Me.GetRawFilesDir, "romfs", "script")
            Dim scriptDestination As String = IO.Path.Combine(Me.GetRootDirectory, "Scripts")
            Dim filesToOpen As New List(Of String)

            Dim f As New Utilities.AsyncFor(PluginHelper.GetLanguageItem("Decompiling Scripts..."))
            'Await f.RunForEachSync(Async Function(Item As String) As Task
            '                           Dim unlua As New unluac(Item)
            '                           Dim script As String = Await unlua.Decompile
            '                           Dim dest = Item.Replace(scriptSource, scriptDestination)
            '                           If Not IO.Directory.Exists(IO.Path.GetDirectoryName(dest)) Then
            '                               IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(dest))
            '                           End If
            '                           IO.File.WriteAllText(dest, script)
            '                           IO.File.WriteAllText(dest & ".original", script)

            '                           filesToOpen.Add(dest)
            '                       End Function, IO.Directory.GetFiles(scriptSource, "*.lua", IO.SearchOption.AllDirectories))
            Dim toConvert = IO.Directory.GetFiles(scriptSource, "*.lua", IO.SearchOption.AllDirectories)
            Dim done As Integer = 0
            For Each item In toConvert
                PluginHelper.SetLoadingStatus(String.Format(PluginHelper.GetLanguageItem("Converting Scripts... {0} of {1}"), done, toConvert.Length), done / toConvert.Length)
                Dim unlua As New unluac(item)
                Dim script As String = Await unlua.Decompile
                Dim dest = item.Replace(scriptSource, scriptDestination)
                If Not IO.Directory.Exists(IO.Path.GetDirectoryName(dest)) Then
                    IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(dest))
                End If
                IO.File.WriteAllText(dest, script)
                IO.File.WriteAllText(dest & ".original", script)

                filesToOpen.Add(dest)
                done += 1
            Next

            PluginHelper.SetLoadingStatusFinished()

            Me.CreateDirectory("", "Scripts")
            For Each item In filesToOpen

                Dim pathParts = item.ToLower.Replace(Me.GetRootDirectory.ToLower, "").Replace("\", "/").TrimStart("/").Split("/")
                Dim parentPath As New Text.StringBuilder
                For count = 0 To pathParts.Length - 3
                    parentPath.Append(pathParts(count))
                    parentPath.Append("/")
                Next
                Dim immediateParentDir = pathParts(pathParts.Length - 2)
                Dim parentPathString = parentPath.ToString.TrimEnd("/")
                'ensure the directory exists
                CreateDirectory(parentPathString, immediateParentDir)

                Await Me.AddExistingFile(parentPathString & "/" & immediateParentDir, item)

                ''Create project directory if it doesn't exist
                'Dim projDir = "Mods/" & IO.Path.GetFileNameWithoutExtension(Filename) & "/Scripts" & item.Replace(scriptDestination, "").Replace("\", "/").Replace(IO.Path.GetFileName(item), "")
                'If Not CurrentProject.Files.ContainsKey(projDir) Then
                '    CurrentProject.CreateDirectory(projDir)
                'End If
                ''Add file to project
                'CurrentProject.OpenFile(item, "Mods/" & IO.Path.GetFileNameWithoutExtension(Filename) & "/Scripts" & item.Replace(scriptDestination, "").Replace("\", "/"), False)
            Next

        End Function

        Public Overrides Async Function Build(Solution As Solution) As Task
            Dim scriptDestination As String = IO.Path.Combine(Me.GetRawFilesDir, "romfs", "script")
            Dim scriptSource As String = IO.Path.Combine(Me.GetRootDirectory, "Scripts")

            Dim f As New Utilities.AsyncFor(PluginHelper.GetLanguageItem("Compiling Scripts..."))
            Await f.RunForEach(Async Function(Item As String) As Task
                                   Dim sourceText = IO.File.ReadAllText(Item)
                                   Dim sourceOrig = IO.File.ReadAllText(Item & ".original")

                                   If Not sourceText = sourceOrig Then
                                       Dim dest = Item.Replace(scriptSource, scriptDestination)
                                       Await PluginHelper.RunProgram(PluginHelper.GetResourceName("lua/luac5.1.exe"), $"-o ""{dest}"" ""{Item}""")
                                   End If
                               End Function, IO.Directory.GetFiles(scriptSource, "*.lua", IO.SearchOption.AllDirectories))
            Await MyBase.Build(Solution)
        End Function

        Public Overrides Function GetFilesToCopy() As IEnumerable(Of String)
            Return {IO.Path.Combine("romfs", "script")}
        End Function

        Public Overrides Function GetSupportedGameCodes() As IEnumerable(Of String)
            Return {GameStrings.GTICode, GameStrings.PSMDCode}
        End Function
    End Class

End Namespace
