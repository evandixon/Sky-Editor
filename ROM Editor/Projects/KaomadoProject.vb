Imports ROMEditor.FileFormats
Imports ROMEditor.FileFormats.Explorers
Imports SkyEditorBase

Namespace Projects
    Public Class KaomadoProject
        Inherits GenericModProject

        Public Overrides Function GetSupportedGameCodes() As IEnumerable(Of String)
            Return {GameStrings.SkyCode}
        End Function

        Public Overrides Function GetFilesToCopy(Solution As SolutionOld, BaseRomProjectName As String) As IEnumerable(Of String)
            Return {IO.Path.Combine("Data", "FONT", "kaomado.kao")}
        End Function

        Public Overrides Async Function Initialize(Solution As SolutionOld) As Task
            Await MyBase.Initialize(Solution)
            Dim rootDir = GetRootDirectory()
            Dim portraitDir = IO.Path.Combine(rootDir, "Pokemon", "Portraits")
            If Not IO.Directory.Exists(portraitDir) Then
                IO.Directory.CreateDirectory(portraitDir)
            End If
            Me.CreateDirectory("Pokemon/Portraits")
            Dim k As New Kaomado(IO.Path.Combine(GetRawFilesDir, "data", "FONT", "kaomado.kao"))
            Await Kaomado.RunUnpack(IO.Path.Combine(GetRawFilesDir, "data", "FONT", "kaomado.kao"), portraitDir)
            Await k.ApplyMissingPortraitFix(portraitDir)

            ''We don't really need to add the files to the project.  It will save time to skip this.
            'Dim toAdd = IO.Directory.GetFiles(portraitDir, "*", IO.SearchOption.AllDirectories)

            'Dim f As New Utilities.AsyncFor(PluginHelper.GetLanguageItem("Opening Files..."))
            'Await f.RunForEachSync(Function(Item As String) As Task
            '                           Dim d = IO.Path.GetDirectoryName(Item).Replace(rootDir, "")
            '                           Me.CreateDirectory(d)
            '                           Me.AddExistingFile(d, Item)
            '                           Return Task.CompletedTask
            '                       End Function, toAdd)
            'PluginHelper.SetLoadingStatusFinished()
        End Function

        Public Overrides Async Function Build(Solution As SolutionOld) As Task
            If IO.Directory.Exists(IO.Path.Combine(GetRootDirectory, "Pokemon", "Portraits")) Then
                Await Kaomado.RunPack(IO.Path.Combine(GetRawFilesDir, "data", "FONT", "kaomado.kao"), IO.Path.Combine(GetRootDirectory, "Pokemon", "Portraits"))
            End If
            Await MyBase.Build(Solution)
        End Function
    End Class
End Namespace

