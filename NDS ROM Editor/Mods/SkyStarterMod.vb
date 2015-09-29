Imports ROMEditor.FileFormats
Imports SkyEditorBase

Namespace Mods
    Public Class SkyStarterMod
        Inherits GenericMod
        Public Sub New()
            MyBase.New()
        End Sub

        Public Overrides Function FilesToCopy() As IEnumerable(Of String)
            Return {IO.Path.Combine("overlay", "overlay_0013.bin"),
                    IO.Path.Combine("data", "MESSAGE", "text_e.str"),
                    IO.Path.Combine("data", "MESSAGE", "text_f.str"),
                    IO.Path.Combine("data", "MESSAGE", "text_s.str"),
                    IO.Path.Combine("data", "MESSAGE", "text_i.str"),
                    IO.Path.Combine("data", "MESSAGE", "text_g.str"),
                    IO.Path.Combine("data", "MESSAGE", "text_j.str")}
        End Function

        Public Overrides Sub Initialize(CurrentProject As Project)
            Dim internalPath = "Mods/" & IO.Path.GetFileNameWithoutExtension(OriginalFilename)

            'Convert Languages
            PluginHelper.StartLoading("Converting languages...")
            CurrentProject.CreateDirectory("Mods/" & IO.Path.GetFileNameWithoutExtension(OriginalFilename) & "/Languages/")
            Dim languageDictionary As New Dictionary(Of String, String)
            languageDictionary.Add("text_e.str", "English")
            languageDictionary.Add("text_f.str", "Frensh")
            languageDictionary.Add("text_s.str", "Spanish")
            languageDictionary.Add("text_i.str", "Italian")
            languageDictionary.Add("text_g.str", "German")
            languageDictionary.Add("text_j.str", "Japanese")
            For Each item In languageDictionary
                If IO.File.Exists(IO.Path.Combine(ROMDirectory, "Data", "MESSAGE", item.Key)) Then
                    Using langString = New FileFormats.LanguageString(IO.Path.Combine(ROMDirectory, "Data", "MESSAGE", item.Key))
                        Using langList As New ObjectFile(Of List(Of String))
                            langList.ContainedObject = langString.Items
                            langList.Save(IO.Path.Combine(ModDirectory, "Languages", item.Value))
                        End Using
                    End Using
                End If
            Next

            'Convert Personality Test
            PluginHelper.StartLoading("Converting Personality Test")
            Dim overlay13 As New FileFormats.Overlay13(IO.Path.Combine(ROMDirectory, "Overlay", "overlay_0013.bin"))
            Using personalityTest As New ObjectFile(Of FileFormats.PersonalityTestContainer)
                personalityTest.ContainedObject = New FileFormats.PersonalityTestContainer(overlay13)
                personalityTest.Save(IO.Path.Combine(ModDirectory, "Starter Pokemon"))
                CurrentProject.OpenFile(IO.Path.Combine(ModDirectory, "Starter Pokemon"), IO.Path.Combine(internalPath, "Starter Pokemon"), False)
            End Using
            PluginHelper.StopLoading()
        End Sub

        Public Overrides Sub Build(CurrentProject As Project)
            'Convert Personality Test
            Dim personalityTest As ObjectFile(Of PersonalityTestContainer) = Nothing
            If IO.File.Exists(IO.Path.Combine(ModDirectory, "Starter Pokemon")) Then
                Dim overlay13 As New FileFormats.Overlay13(IO.Path.Combine(ROMDirectory, "Overlay", "overlay_0013.bin"))
                personalityTest = New ObjectFile(Of PersonalityTestContainer)(IO.Path.Combine(ModDirectory, "Starter Pokemon"))
                personalityTest.ContainedObject.UpdateOverlay(overlay13)
                overlay13.Save()
            End If


            'Convert Languages
            Dim languageDictionary As New Dictionary(Of String, String)
            languageDictionary.Add("text_e.str", "English")
            languageDictionary.Add("text_f.str", "Frensh")
            languageDictionary.Add("text_s.str", "Spanish")
            languageDictionary.Add("text_i.str", "Italian")
            languageDictionary.Add("text_g.str", "German")
            languageDictionary.Add("text_j.str", "Japanese")
            For Each item In languageDictionary
                If IO.File.Exists(IO.Path.Combine(ModDirectory, "Languages", item.Value)) Then
                    Using langFile As New ObjectFile(Of List(Of String))(IO.Path.Combine(ModDirectory, "Languages", item.Value))
                        Using langString As New FileFormats.LanguageString
                            langString.Items = langFile.ContainedObject

                            If personalityTest IsNot Nothing Then
                                langString.UpdatePersonalityTestResult(personalityTest.ContainedObject)
                            End If

                            langString.Save(IO.Path.Combine(ROMDirectory, "Data", "MESSAGE", item.Key))
                        End Using
                    End Using
                End If
            Next

            If personalityTest IsNot Nothing Then personalityTest.Dispose()
        End Sub

        Public Overrides Function SupportedGameCodes() As IEnumerable(Of Type)
            Return {GetType(Roms.SkyNDSRom)}
        End Function

        Public Sub New(Filename As String)
            MyBase.New(Filename)
        End Sub
    End Class

End Namespace
