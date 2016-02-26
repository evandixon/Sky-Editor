Imports SkyEditorBase

Namespace Projects
    Public Class ExplorersScriptModProject
        Inherits GenericModProject
        Public Overrides Function GetRawFilesDir() As String
            Return GetRootDirectory()
        End Function
        Public Overrides Function GetFilesToCopy(Solution As Solution, BaseRomProjectName As String) As IEnumerable(Of String)
            Return {IO.Path.Combine("data", "script")}
        End Function
        Public Overrides Function GetSupportedGameCodes() As IEnumerable(Of String)
            Return {GameStrings.SkyCode}
        End Function

        Public Overrides Async Function Initialize(Solution As Solution) As Task
            Await MyBase.Initialize(Solution)

            Dim projectDir = GetRootDirectory()
            Dim sourceDir = GetRawFilesDir()

            Dim scriptFiles = IO.Directory.GetFiles(IO.Path.Combine(sourceDir, "Data", "SCRIPT"), "*", IO.SearchOption.AllDirectories)
            Dim f2 As New Utilities.AsyncFor(PluginHelper.GetLanguageItem("Adding Files..."))
            Await f2.RunForEachSync(Async Function(Item As String) As Task
                                        Dim d = IO.Path.GetDirectoryName(Item).Replace(projectDir, "")
                                        Me.CreateDirectory(d)
                                        Await Me.AddExistingFile(d, Item, False)
                                    End Function, scriptFiles)

            PluginHelper.SetLoadingStatusFinished()
        End Function

    End Class

End Namespace
