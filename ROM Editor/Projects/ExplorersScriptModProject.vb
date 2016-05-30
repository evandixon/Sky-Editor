Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Utilities
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

        Protected Overrides Async Function Initialize() As Task
            Await MyBase.Initialize

            Dim projectDir = GetRootDirectory()
            Dim sourceDir = GetRawFilesDir()

            Dim scriptFiles = IO.Directory.GetFiles(IO.Path.Combine(sourceDir, "Data", "SCRIPT"), "*", IO.SearchOption.AllDirectories)
            Dim f2 As New AsyncFor(My.Resources.Language.LoadingAddingFiles)
            f2.RunSynchronously = True
            Await f2.RunForEach(Async Function(Item As String) As Task
                                    Dim d = IO.Path.GetDirectoryName(Item).Replace(projectDir, "")
                                    Me.CreateDirectory(d)
                                    Await Me.AddExistingFile(d, Item, CurrentPluginManager.CurrentIOProvider)
                                End Function, scriptFiles)

            PluginHelper.SetLoadingStatusFinished()
        End Function

    End Class

End Namespace
