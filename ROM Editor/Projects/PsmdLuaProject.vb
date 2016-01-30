Imports SkyEditorBase
Namespace Projects
    Public Class PsmdLuaProject
        Inherits GenericModProject
        Public Overrides Async Function Initialize(Solution As Solution) As Task
            Await MyBase.Initialize(Solution)

            Dim scriptSource As String = IO.Path.Combine(Me.GetRawFilesDir, "romfs", "script")
            Dim scriptDestination As String = IO.Path.Combine(Me.GetRootDirectory)
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
                                        Dim d = IO.Path.GetDirectoryName(Item).Replace(scriptDestination, "")
                                        Me.CreateDirectory(d)
                                        Await Me.AddExistingFile(d, Item, False)
                                    End Function, filesToOpen)
            PluginHelper.SetLoadingStatusFinished()

        End Function

        Public Overrides Async Function Build(Solution As Solution) As Task
            Dim scriptDestination As String = IO.Path.Combine(Me.GetRawFilesDir, "romfs", "script")
            Dim scriptSource As String = IO.Path.Combine(Me.GetRootDirectory)

            Dim toCompile = From d In IO.Directory.GetFiles(scriptSource, "*.lua", IO.SearchOption.AllDirectories) Where Not d.StartsWith(scriptDestination) Select d

            Dim f As New Utilities.AsyncFor(PluginHelper.GetLanguageItem("Compiling Scripts..."))
            Await f.RunForEach(Async Function(Item As String) As Task
                                   Dim sourceText = IO.File.ReadAllText(Item)
                                   Dim sourceOrig = IO.File.ReadAllText(Item & ".original")

                                   If Not sourceText = sourceOrig Then
                                       Dim dest = Item.Replace(scriptSource, scriptDestination)
                                       Await PluginHelper.RunProgram(PluginHelper.GetResourceName("lua/luac5.1.exe"), $"-o ""{dest}"" ""{Item}""")
                                   End If
                               End Function, toCompile)
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
