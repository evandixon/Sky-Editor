Imports SkyEditorBase

Namespace Mods
    Public Class PMD3DLuaMod
        Inherits GenericMod

        Public Overrides Async Function InitializeAsync(CurrentProject As Project) As Task
            Dim scriptSource As String = IO.Path.Combine(Me.ROMDirectory, "romfs", "script")
            Dim scriptDestination As String = IO.Path.Combine(Me.ModDirectory, "Scripts")


            PluginHelper.SetLoadingStatus(PluginHelper.GetLanguageItem("Decompiling Scripts..."))
            For Each item In IO.Directory.GetFiles(scriptSource, "*.lua", IO.SearchOption.AllDirectories)
                Dim unlua As New unluac(item)
                Dim script As String = Await unlua.Decompile
                Dim dest = item.Replace(scriptSource, scriptDestination)
                If Not IO.Directory.Exists(IO.Path.GetDirectoryName(dest)) Then
                    IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(dest))
                End If
                IO.File.WriteAllText(dest, script)
                IO.File.WriteAllText(dest & ".original", script)
            Next
        End Function

        Public Overrides Async Function BuildAsync(CurrentProject As Project) As Task
            Dim scriptDestination As String = IO.Path.Combine(Me.ROMDirectory, "romfs", "script")
            Dim scriptSource As String = IO.Path.Combine(Me.ModDirectory, "Scripts")


            PluginHelper.SetLoadingStatus(PluginHelper.GetLanguageItem("Compiling Scripts..."))
            'Todo: queue the tasks, then await them all
            For Each item In IO.Directory.GetFiles(scriptSource, "*.lua", IO.SearchOption.AllDirectories)
                Dim sourceText = IO.File.ReadAllText(item)
                Dim sourceOrig = IO.File.ReadAllText(item & ".original")

                If Not sourceText = sourceOrig Then
                    Dim dest = item.Replace(scriptSource, scriptDestination)
                    Await PluginHelper.RunProgram(PluginHelper.GetResourceName("lua/luac5.1.exe"), $"-o ""{dest}"" ""{item}""")
                End If
            Next
        End Function

        Public Overrides Function SupportedGameCodes() As IEnumerable(Of Type)
            Return {GetType(Roms.GatesToInfinityRom), GetType(Roms.PSMDRom)}
        End Function

        Public Overrides Function FilesToCopy() As IEnumerable(Of String)
            Return {IO.Path.Combine("romfs", "script")}
        End Function
    End Class
End Namespace

