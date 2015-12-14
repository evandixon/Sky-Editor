Imports SkyEditorBase

Namespace Mods
    Public Class PMD3DLuaMod
        Inherits GenericMod

        Public Overrides Async Function InitializeAsync(CurrentProject As Project) As Task
            Dim scriptSource As String = IO.Path.Combine(Me.ROMDirectory, "romfs", "script")
            Dim scriptDestination As String = IO.Path.Combine(Me.ModDirectory, "Scripts")
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

            PluginHelper.SetLoadingStatus(PluginHelper.GetLanguageItem("Opening files..."))

            CurrentProject.EnableRaisingEvents = False
            For Each item In filesToOpen
                'Create project directory if it doesn't exist
                Dim projDir = "Mods/" & IO.Path.GetFileNameWithoutExtension(OriginalFilename) & "/Scripts" & item.Replace(scriptDestination, "").Replace("\", "/").Replace(IO.Path.GetFileName(item), "")
                If Not CurrentProject.Files.ContainsKey(projDir) Then
                    CurrentProject.CreateDirectory(projDir)
                End If
                'Add file to project
                CurrentProject.OpenFile(item, "Mods/" & IO.Path.GetFileNameWithoutExtension(OriginalFilename) & "/Scripts" & item.Replace(scriptDestination, "").Replace("\", "/"), False)
            Next
            CurrentProject.EnableRaisingEvents = True
            PluginHelper.SetLoadingStatusFinished()
        End Function

        Public Overrides Async Function BuildAsync(CurrentProject As Project) As Task
            Dim scriptDestination As String = IO.Path.Combine(Me.ROMDirectory, "romfs", "script")
            Dim scriptSource As String = IO.Path.Combine(Me.ModDirectory, "Scripts")

            Dim f As New Utilities.AsyncFor(PluginHelper.GetLanguageItem("Compiling Scripts..."))
            Await f.RunForEach(Async Function(Item As String) As Task
                                   Dim sourceText = IO.File.ReadAllText(Item)
                                   Dim sourceOrig = IO.File.ReadAllText(Item & ".original")

                                   If Not sourceText = sourceOrig Then
                                       Dim dest = Item.Replace(scriptSource, scriptDestination)
                                       Await PluginHelper.RunProgram(PluginHelper.GetResourceName("lua/luac5.1.exe"), $"-o ""{dest}"" ""{Item}""")
                                   End If
                               End Function, IO.Directory.GetFiles(scriptSource, "*.lua", IO.SearchOption.AllDirectories))
        End Function

        Public Overrides Function SupportedGameTypes() As IEnumerable(Of Type)
            Return {GetType(Roms.GatesToInfinityRom), GetType(Roms.PSMDRom)}
        End Function

        Public Overrides Function FilesToCopy() As IEnumerable(Of String)
            Return {IO.Path.Combine("romfs", "script")}
        End Function
    End Class
End Namespace

