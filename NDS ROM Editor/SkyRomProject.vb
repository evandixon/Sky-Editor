Imports ROMEditor
Imports ROMEditor.FileFormats
Imports ROMEditor.Mods
Imports ROMEditor.Roms
Imports SkyEditorBase

Public Class SkyRomProject
    Inherits GenericNDSModProject

    Private Async Function LoadKaomadoFixMod(modDirectory As String, romDirectory As String) As Task

    End Function

    Private Async Function LoadGeneralMod(modDirectory As String, romDirectory As String, modFilename As String, internalPath As String) As Task

    End Function

    Private Async Sub SkyRomProject_NDSModAdded(sender As Object, e As NDSModAddedEventArgs) Handles Me.NDSModAdded
        Dim romDirectory = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "Mods", IO.Path.GetFileNameWithoutExtension(e.InternalName), "RawFiles")
        Dim modDirectory = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "Mods", IO.Path.GetFileNameWithoutExtension(e.InternalName))
        Dim internalPath = "Mods/" & IO.Path.GetFileNameWithoutExtension(e.InternalName)
        Dim sky = DirectCast(Files("BaseRom.nds"), SkyNDSRom)

        If TypeOf e.File Is KaomadoNDSMod Then
            Await LoadKaomadoFixMod(modDirectory, romDirectory)
        Else
            Await LoadGeneralMod(modDirectory, romDirectory, e.InternalName, internalPath)
        End If
    End Sub
    Private Async Function BuildModAsync(e As NDSModBuildingEventArgs) As Task
        Dim romDirectory = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "Mods", IO.Path.GetFileNameWithoutExtension(IO.Path.GetFileNameWithoutExtension(e.NDSModSourceFilename)), "RawFiles")
        Dim modDirectory = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "Mods", IO.Path.GetFileNameWithoutExtension(IO.Path.GetFileNameWithoutExtension(e.NDSModSourceFilename)))
        'Convert BACK
        If IO.Directory.Exists(IO.Path.Combine(IO.Path.GetDirectoryName(e.NDSModSourceFilename), IO.Path.GetFileNameWithoutExtension(e.NDSModSourceFilename), "Backgrounds")) Then
            For Each background In IO.Directory.GetFiles(IO.Path.Combine(IO.Path.GetDirectoryName(e.NDSModSourceFilename), IO.Path.GetFileNameWithoutExtension(e.NDSModSourceFilename), "Backgrounds"), "*.bmp")
                Dim includeInPack As Boolean

                If IO.File.Exists(background & ".original") Then
                    Using bmp As New IO.FileStream(background, IO.FileMode.Open)
                        Using orig As New IO.FileStream(background & ".original", IO.FileMode.Open)
                            Dim equal As Boolean = (bmp.Length = orig.Length)
                            While equal
                                Dim b = bmp.ReadByte
                                Dim o = orig.ReadByte
                                equal = (b = o)
                                If b = -1 OrElse o = -1 Then
                                    Exit While
                                End If
                            End While
                            includeInPack = Not equal
                        End Using
                    End Using
                Else
                    includeInPack = True
                End If

                If includeInPack Then
                    Dim bgp = FileFormats.BGP.ConvertFromBitmap(Drawing.Bitmap.FromFile(background))
                    bgp.Save(IO.Path.Combine(IO.Path.GetDirectoryName(e.NDSModSourceFilename), IO.Path.GetFileNameWithoutExtension(e.NDSModSourceFilename), "RawFiles", "Data", "BACK", IO.Path.GetFileNameWithoutExtension(background) & ".bgp"))
                    bgp.Dispose()
                End If

            Next
        End If

        'Convert Personality Test
        Dim personalityTest As ObjectFile(Of PersonalityTestContainer) = Nothing
        If IO.File.Exists(IO.Path.Combine(modDirectory, "Starter Pokemon")) Then
            Dim overlay13 As New FileFormats.Overlay13(IO.Path.Combine(romDirectory, "Overlay", "overlay_0013.bin"))
            personalityTest = New ObjectFile(Of PersonalityTestContainer)(IO.Path.Combine(modDirectory, "Starter Pokemon"))
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
            If IO.File.Exists(IO.Path.Combine(modDirectory, "Languages", item.Value)) Then
                Using langFile As New ObjectFile(Of List(Of String))(IO.Path.Combine(modDirectory, "Languages", item.Value))
                    Using langString As New FileFormats.LanguageString
                        langString.Items = langFile.ContainedObject

                        If personalityTest IsNot Nothing Then
                            langString.UpdatePersonalityTestResult(personalityTest.ContainedObject)
                        End If

                        langString.Save(IO.Path.Combine(romDirectory, "Data", "MESSAGE", item.Key))
                    End Using
                End Using
            End If
        Next

        If personalityTest IsNot Nothing Then personalityTest.Dispose()

        'Copy Items
        Dim item_p_path As String = IO.Path.Combine(romDirectory, "Data", "BALANCE", "item_p.bin")
        Dim item_s_p_path As String = IO.Path.Combine(romDirectory, "Data", "BALANCE", "item_s_p.bin")

        If IO.File.Exists(IO.Path.Combine(modDirectory, "Items", "Item Definitions")) Then
            IO.File.Copy(IO.Path.Combine(modDirectory, "Items", "Item Definitions"), item_p_path, True)
        End If
        If IO.File.Exists(IO.Path.Combine(modDirectory, "Items", "Exclusive Item Rarity")) Then
            IO.File.Copy(IO.Path.Combine(modDirectory, "Items", "Exclusive Item Rarity"), item_s_p_path, True)
        End If






        'Cleanup
        '-Data/Back/Decompressed
        If IO.Directory.Exists(IO.Path.Combine(IO.Path.GetDirectoryName(e.NDSModSourceFilename), IO.Path.GetFileNameWithoutExtension(e.NDSModSourceFilename), "RawFiles", "Data", "BACK", "Decompressed")) Then
            IO.Directory.Delete(IO.Path.Combine(IO.Path.GetDirectoryName(e.NDSModSourceFilename), IO.Path.GetFileNameWithoutExtension(e.NDSModSourceFilename), "RawFiles", "Data", "BACK", "Decompressed"), True)
        End If
    End Function
    Protected Overrides Async Function BuildMod(e As NDSModBuildingEventArgs) As Task
        Await MyBase.BuildMod(e)
        Await BuildModAsync(e)
    End Function

    Public Overrides Function CustomFilePatchers() As List(Of FilePatcher)
        Dim patchers = MyBase.CustomFilePatchers
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
    Public Overrides Function CreatableFiles(InternalPath As String, Manager As PluginManager) As IList(Of Type)
        Dim l = MyBase.CreatableFiles(InternalPath, Manager)
        If InternalPath.ToLower = "mods/" Then
            l.Add(GetType(KaomadoNDSMod))
        End If
        Return l
    End Function
End Class