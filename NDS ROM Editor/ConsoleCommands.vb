Imports SkyEditorBase
Imports ROMEditor.Roms

Public Class ConsoleCommands
    'Public Shared Sub ROMHeader(Manager As PluginManager, Argument As String)
    '    PluginHelper.Writeline(DirectCast(Manager.Save, GenericNDSRom).GameTitle)
    'End Sub
    'Public Shared Async Sub UnPack(Manager As PluginManager, Argument As String)
    '    Await DirectCast(Manager.Save, GenericNDSRom).Unpack()
    'End Sub
    'Public Shared Async Sub RePack(Manager As PluginManager, Argument As String)
    '    Await DirectCast(Manager.Save, GenericNDSRom).RePack(Argument)
    'End Sub
    ' ''' <summary>
    ' ''' Makes lots of test ROMs each with only one kind of BGM.
    ' ''' Do not actually run, this doesn't know how to handle skipping 109 and 194 to 198 yet.
    ' ''' </summary>
    ' ''' <param name="Manager"></param>
    ' ''' <param name="Argument"></param>
    ' ''' <remarks></remarks>
    'Public Shared Sub EoSTestMusic(Manager As PluginManager, Argument As String)
    '    Dim e As New System.Text.ASCIIEncoding
    '    Dim romDirectory As String = IO.Path.Combine(PluginHelper.GetResourceName("Test EoS Music Locations"))
    '    Dim soundDir As String = IO.Path.Combine(PluginHelper.GetResourceName(Manager.CurrentSaveName & "\data\SOUND\BGM\"))
    '    Dim report As String = ""
    '    If Not IO.Directory.Exists(romDirectory) Then
    '        IO.Directory.CreateDirectory(romDirectory)
    '    End If
    '    For count As Integer = 1 To 201
    '        UnPack(Manager, Argument)
    '        Dim b = IO.File.ReadAllBytes(soundDir & String.Format("bgm{0}.smd", count.ToString.PadLeft(4, "0")))
    '        report &= count & ": " & e.GetString(b, &H20, 15) & vbCrLf
    '        IO.File.WriteAllText(romDirectory & "/eos.txt", report)
    '        For count2 As Integer = 0 To 201
    '            If Not count = count2 Then
    '                PluginHelper.Writeline(String.Format("Copying {0} to {1}", count, count2))
    '                IO.File.Copy(soundDir & String.Format("bgm{0}.smd", count.ToString.PadLeft(4, "0")), soundDir & String.Format("bgm{0}.smd", count2.ToString.PadLeft(4, "0")), True)
    '                IO.File.Copy(soundDir & String.Format("bgm{0}.swd", count.ToString.PadLeft(4, "0")), soundDir & String.Format("bgm{0}.swd", count2.ToString.PadLeft(4, "0")), True)
    '            End If
    '        Next
    '        RePack(Manager, romDirectory & "\" & count.ToString.PadLeft(4, "0") & ".nds")
    '    Next
    'End Sub
    'Public Shared Async Sub ExplorersExtractBGP(Manager As PluginManager, Argument As String)
    '    If Not TypeOf Manager.Save Is SkyNDSRom Then
    '        PluginHelper.Writeline("Save should be of type 'SkyNDSRom'")
    '        Exit Sub
    '    End If
    '    Dim dir As String = IO.Path.Combine(PluginHelper.GetResourceName(Manager.CurrentSaveName & "\data\BACK"))
    '    For Each file In IO.Directory.GetFiles(dir, "*.bgp")
    '        PluginHelper.Writeline("Converting " & file)
    '        Dim img = Await FileFormats.BGP.FromFilename(file)
    '        Dim i = Await img.GetImage
    '        i.Save(file.Replace(".bgp", ".png"))
    '    Next
    'End Sub
    'Public Shared Async Function KaomadoPatch(Manager As PluginManager, Argument As String) As Task
    '    Dim s = DirectCast(Manager.Save, SkyNDSRom)
    '    Dim x = Await s.GetPortraitsFile
    '    For Each directory In IO.Directory.GetDirectories(x.UnpackDirectory)
    '        Dim faces = {"0000_STANDARD.png",
    '                     "0002_GRIN.png",
    '                     "0004_PAINED.png",
    '                     "0006_ANGRY.png",
    '                     "0008_WORRIED.png",
    '                     "0010_SAD.png",
    '                     "0012_CRYING.png",
    '                     "0014_SHOUTING.png",
    '                     "0016_TEARY_EYED.png",
    '                     "0018_DETERMINED.png",
    '                     "0020_JOYOUS.png",
    '                     "0022_INSPIRED.png",
    '                     "0024_SURPRISED.png",
    '                     "0026_DIZZY.png",
    '                     "0032_SIGH.png",
    '                     "0034_STUNNED.png"}

    '        For count As Integer = 1 To faces.Length - 1
    '            If Not IO.File.Exists(IO.Path.Combine(directory, faces(count))) Then
    '                PluginHelper.Writeline(String.Format("Copying {0} to {1}", IO.Path.Combine(directory, faces(0)), IO.Path.Combine(directory, faces(count))))
    '                IO.File.Copy(IO.Path.Combine(directory, faces(0)), IO.Path.Combine(directory, faces(count)))
    '            End If
    '        Next
    '    Next
    '    Await x.Save()
    'End Function
    'Public Shared Async Sub PmdLanguage(Manager As PluginManager, Argument As String)
    '    PluginHelper.Writeline((Await DirectCast(Manager.Save, SkyNDSRom).GetLanguageString)(CUInt(Argument)))
    'End Sub
End Class