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
            Dim toAdd As New List(Of AddExistingFileBatchOperation)
            For Each item In scriptFiles
                toAdd.Add(New AddExistingFileBatchOperation With {.ActualFilename = item, .ParentPath = IO.Path.GetDirectoryName(item).Replace(projectDir, "")})
            Next
            Await Me.RecreateRootWithExistingFiles(toAdd, CurrentPluginManager.CurrentIOProvider)

            Me.BuildProgress = 1
            Me.IsBuildProgressIndeterminate = False
            Me.BuildStatusMessage = My.Resources.Language.Complete
        End Function

    End Class

End Namespace
