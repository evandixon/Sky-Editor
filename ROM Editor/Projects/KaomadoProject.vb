Imports ROMEditor.FileFormats
Imports ROMEditor.FileFormats.Explorers
Imports SkyEditor.Core.IO
Imports SkyEditorBase

Namespace Projects
    Public Class KaomadoProject
        Inherits GenericModProject

        Public Overrides Function GetSupportedGameCodes() As IEnumerable(Of String)
            Return {GameStrings.SkyCode}
        End Function

        Public Overrides Function GetFilesToCopy(Solution As Solution, BaseRomProjectName As String) As IEnumerable(Of String)
            Return {IO.Path.Combine("Data", "FONT", "kaomado.kao")}
        End Function

        Protected Overrides Async Function Initialize() As Task
            Await MyBase.Initialize

            'Start loading
            Me.BuildProgress = 0
            Me.IsBuildProgressIndeterminate = True
            Me.BuildStatusMessage = My.Resources.Language.LoadingUnpacking

            'Unpack
            Dim rootDir = GetRootDirectory()
            Dim portraitDir = IO.Path.Combine(rootDir, "Pokemon", "Portraits")
            If Not IO.Directory.Exists(portraitDir) Then
                IO.Directory.CreateDirectory(portraitDir)
            End If
            Dim k As New Kaomado(IO.Path.Combine(GetRawFilesDir, "data", "FONT", "kaomado.kao"))
            Await Kaomado.RunUnpack(IO.Path.Combine(GetRawFilesDir, "data", "FONT", "kaomado.kao"), portraitDir)
            Await k.ApplyMissingPortraitFix(portraitDir)

            'Add files to project
            Dim toAdd As New List(Of AddExistingFileBatchOperation)
            For Each item In IO.Directory.GetFiles(portraitDir, "*", IO.SearchOption.AllDirectories)
                toAdd.Add(New AddExistingFileBatchOperation With {.ActualFilename = item, .ParentPath = IO.Path.GetDirectoryName(item).Replace(portraitDir, IO.Path.Combine("Pokemon", "Portraits"))})
            Next
            Await Me.RecreateRootWithExistingFiles(toAdd, CurrentPluginManager.CurrentIOProvider)

            'Stop loading
            Me.BuildProgress = 1
            Me.IsBuildProgressIndeterminate = False
            Me.BuildStatusMessage = My.Resources.Language.Complete

        End Function

        Protected Overrides Async Function DoBuild() As Task
            'Start loading
            Me.BuildProgress = 0
            Me.IsBuildProgressIndeterminate = True
            Me.BuildStatusMessage = My.Resources.Language.LoadingPacking

            'Pack
            If IO.Directory.Exists(IO.Path.Combine(GetRootDirectory, "Pokemon", "Portraits")) Then
                Await Kaomado.RunPack(IO.Path.Combine(GetRawFilesDir, "data", "FONT", "kaomado.kao"), IO.Path.Combine(GetRootDirectory, "Pokemon", "Portraits"))
            End If

            'Stop loading
            Me.BuildProgress = 1
            Me.IsBuildProgressIndeterminate = False
            Me.BuildStatusMessage = My.Resources.Language.Complete

            'Build the mod
            Await MyBase.DoBuild
        End Function
    End Class
End Namespace

