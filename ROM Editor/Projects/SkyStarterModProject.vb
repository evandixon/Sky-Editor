Imports ROMEditor.FileFormats
Imports ROMEditor.FileFormats.Explorers
Imports SkyEditor.Core.IO
Imports SkyEditorBase

Namespace Projects
    Public Class SkyStarterModProject
        Inherits GenericModProject

        Public Overrides Function GetFilesToCopy(Solution As SolutionOld, BaseRomProjectName As String) As IEnumerable(Of String)
            Return {IO.Path.Combine("overlay", "overlay_0013.bin"),
                    IO.Path.Combine("data", "MESSAGE", "text_e.str"),
                    IO.Path.Combine("data", "MESSAGE", "text_f.str"),
                    IO.Path.Combine("data", "MESSAGE", "text_s.str"),
                    IO.Path.Combine("data", "MESSAGE", "text_i.str"),
                    IO.Path.Combine("data", "MESSAGE", "text_g.str"),
                    IO.Path.Combine("data", "MESSAGE", "text_j.str")}
        End Function
        Public Overrides Function GetSupportedGameCodes() As IEnumerable(Of String)
            Return {GameStrings.SkyCode}
        End Function

        Public Overrides Function GetCustomFilePatchers() As IEnumerable(Of FilePatcher)
            Dim patchers = New List(Of FilePatcher)
            If patchers Is Nothing Then
                patchers = New List(Of FilePatcher)
            End If
            Dim LSPatcher As New FilePatcher()
            With LSPatcher
                .CreatePatchProgram = "LanguageStringPatcher.exe"
                .CreatePatchArguments = "-c ""{0}"" ""{1}"" ""{2}"""
                .ApplyPatchProgram = "LanguageStringPatcher.exe"
                .ApplyPatchArguments = "-a ""{0}"" ""{1}"" ""{2}"""
                .MergeSafe = True
                .PatchExtension = "textstrlsp"
                .FilePath = ".*text_.\.str"
            End With
            patchers.Add(LSPatcher)
            Return patchers
        End Function

        Public Overrides Async Function Initialize(Solution As SolutionOld) As Task
            Await MyBase.Initialize(Solution)

            Dim rawDir = GetRawFilesDir()
            Dim projDir = GetRootDirectory()

            'Convert Languages
            PluginHelper.SetLoadingStatus(My.Resources.Language.LoadingConvertingLanguages)
            Dim languageDictionary As New Dictionary(Of String, String)
            languageDictionary.Add("text_e.str", "English")
            languageDictionary.Add("text_f.str", "Français")
            languageDictionary.Add("text_s.str", "Español")
            languageDictionary.Add("text_i.str", "Italiano")
            languageDictionary.Add("text_g.str", "Deutsche") 'German
            languageDictionary.Add("text_j.str", "日本語") 'Japanese

            If Not IO.Directory.Exists(IO.Path.Combine(projDir, "Languages")) Then
                IO.Directory.CreateDirectory(IO.Path.Combine(projDir, "Languages"))
            End If
            For Each item In languageDictionary
                If IO.File.Exists(IO.Path.Combine(rawDir, "Data", "MESSAGE", item.Key)) Then
                    Using langString = New LanguageString()
                        Await langString.OpenFile(IO.Path.Combine(rawDir, "Data", "MESSAGE", item.Key), New SkyEditor.Core.Windows.IOProvider)
                        Dim langList As New ObjectFile(Of List(Of String))(New SkyEditor.Core.Windows.IOProvider)
                        langList.ContainedObject = langString.Items
                        langList.Save(IO.Path.Combine(projDir, "Languages", item.Value), PluginManager.GetInstance.CurrentIOProvider)
                    End Using
                End If
            Next

            'Convert Personality Test
            PluginHelper.SetLoadingStatus(My.Resources.Language.LoadingConvertingPersonalityTest)
            Dim overlay13 As New Overlay13(IO.Path.Combine(rawDir, "Overlay", "overlay_0013.bin"))
            Dim personalityTest As New ObjectFile(Of PersonalityTestContainer)(New SkyEditor.Core.Windows.IOProvider)
            personalityTest.ContainedObject = New PersonalityTestContainer(overlay13)
            personalityTest.Save(IO.Path.Combine(projDir, "Starter Pokemon"), PluginManager.GetInstance.CurrentIOProvider)
            Await Me.AddExistingFile("", IO.Path.Combine(projDir, "Starter Pokemon"))
            PluginHelper.SetLoadingStatusFinished()
        End Function

        Public Overrides Async Function Build(Solution As SolutionOld) As Task
            Dim rawDir = GetRawFilesDir()
            Dim projDir = GetRootDirectory()

            'Convert Personality Test
            Dim personalityTest As ObjectFile(Of PersonalityTestContainer) = Nothing
            If IO.File.Exists(IO.Path.Combine(projDir, "Starter Pokemon")) Then
                Dim overlay13 As New Overlay13(IO.Path.Combine(rawDir, "Overlay", "overlay_0013.bin"))
                personalityTest = New ObjectFile(Of PersonalityTestContainer)(New SkyEditor.Core.Windows.IOProvider, IO.Path.Combine(projDir, "Starter Pokemon"))
                personalityTest.ContainedObject.UpdateOverlay(overlay13)
                overlay13.Save()
            End If


            'Convert Languages
            Dim languageDictionary As New Dictionary(Of String, String)
            languageDictionary.Add("text_e.str", "English")
            languageDictionary.Add("text_f.str", "Français")
            languageDictionary.Add("text_s.str", "Español")
            languageDictionary.Add("text_i.str", "Italiano")
            languageDictionary.Add("text_g.str", "Deutsche") 'German
            languageDictionary.Add("text_j.str", "日本語") 'Japanese
            For Each item In languageDictionary
                If IO.File.Exists(IO.Path.Combine(projDir, "Languages", item.Value)) Then
                    Dim langFile As New ObjectFile(Of List(Of String))(New SkyEditor.Core.Windows.IOProvider, IO.Path.Combine(projDir, "Languages", item.Value))
                    Using langString As New LanguageString
                        langString.CreateFile("")
                        langString.Items = langFile.ContainedObject

                        If personalityTest IsNot Nothing Then
                            langString.UpdatePersonalityTestResult(personalityTest.ContainedObject)
                        End If

                        langString.Save(IO.Path.Combine(rawDir, "Data", "MESSAGE", item.Key), PluginManager.GetInstance.CurrentIOProvider)
                    End Using
                End If
            Next

            Await MyBase.Build(Solution)
        End Function
    End Class

End Namespace
