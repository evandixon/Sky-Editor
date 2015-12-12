Imports System.Windows.Forms
Imports ROMEditor.Roms
Imports SkyEditorBase

Public Class Generic3DSModProject
    Inherits GenericNDSModProject
    Public Overrides Async Sub Initialize()
        Dim o As New OpenFileDialog
        o.Filter = "Supported Files (*.3ds;*.3dz;romfs.bin)|*.3ds;*.3dz;romfs.bin|3DS DS Roms (*.3ds;*.3dz)|*.3ds;*.3dz|Braindump romfs (romfs.bin)|romfs.bin|All Files (*.*)|*.*"
        If o.ShowDialog = DialogResult.OK Then
            Dim info = New ObjectFile(Of ModpackInfo)(IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "Modpack Info"))
            info.Save()
            AddFile("Modpack Info", info)

            If IO.Path.GetFileName(o.FileName).ToLower = "romfs.bin" Then
                Dim romDirectory = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "BaseRom RawFiles")
                Dim romfsDir = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "BaseRom RawFiles", "romfs")
                Dim exefsDir = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "BaseRom RawFiles", "exefs")

                If Not IO.Directory.Exists(romDirectory) Then
                    IO.Directory.CreateDirectory(romDirectory)
                End If

                PluginHelper.SetLoadingStatus(PluginHelper.GetLanguageItem("Copying romfs"))
                Await Task.Run(New Action(Sub()
                                              OpenFile(o.FileName, "romfs.bin")
                                          End Sub))

                Dim exefsSource As String = IO.Path.Combine(IO.Path.GetDirectoryName(o.FileName), "exefs.bin")
                If IO.File.Exists(exefsSource) Then
                    PluginHelper.SetLoadingStatus(PluginHelper.GetLanguageItem("Copying exefs"))
                    Await Task.Run(New Action(Sub()
                                                  OpenFile(exefsSource, "exefs.bin")
                                              End Sub))

                    Dim exefs As String = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "exefs.bin")

                    If Not IO.Directory.Exists(exefsDir) Then
                        IO.Directory.CreateDirectory(exefsDir)
                    End If
                    Await Generic3DSRom.RunCtrTool($"-t exefs --exefsdir=""{exefsDir}"" ""{exefs}"" --decompresscode")
                End If

                'Unpack romfs
                If Not IO.Directory.Exists(romfsDir) Then
                    IO.Directory.CreateDirectory(romfsDir)
                End If

                Dim romfs As String = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "romfs.bin")
                Await Generic3DSRom.RunCtrTool($"-t romfs --romfsdir=""{romfsDir}"" ""{romfs}""")
            Else
                OpenFile(o.FileName, "BaseRom.3ds")
                Dim romDirectory = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "BaseRom RawFiles")
                Dim sky = DirectCast(Files("BaseRom.3ds"), iPackedRom)
                Await sky.Unpack(romDirectory)
            End If

            CreateDirectory("Mods")
        Else
            MessageBox.Show("Project initialization failed.  You must supply a base ROM.")
        End If
    End Sub
    Public Overrides Function BaseRomFilename() As String
        Return "BaseRom.3ds"
    End Function
    Public Overrides Function OutputRomFilename() As String
        Return "PatchedRom.3ds"
    End Function
    Public Overrides Sub CopyPatcherProgram()
        Dim toolsDir = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "ModPack Files", "Tools")
        '-Copy ctrtool
        IO.File.Copy(PluginHelper.GetResourceName("ctrtool.exe"), IO.Path.Combine(toolsDir, "ctrtool.exe"), True)
        IO.File.Copy(PluginHelper.GetResourceName("3DS Builder.exe"), IO.Path.Combine(toolsDir, "3DS Builder.exe"), True)

        '-Copy patching wizard
        IO.File.Copy(PluginHelper.GetResourceName("DSPatcher.exe"), IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "ModPack Files", "DSPatcher.exe"), True)
        IO.File.Copy(PluginHelper.GetResourceName("ICSharpCode.SharpZipLib.dll"), IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "ModPack Files", "ICSharpCode.SharpZipLib.dll"), True)
    End Sub
    Public Overrides Async Function ApplyPatchAsync() As Task
        If IO.File.Exists(BaseRomFilename) Then
            Await PluginHelper.RunProgram(IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "ModPack Files", "DSPatcher.exe"), String.Format("""{0}"" ""{1}""", IO.Path.Combine(IO.Path.GetDirectoryName(Filename), BaseRomFilename), IO.Path.Combine(IO.Path.GetDirectoryName(Filename), OutputRomFilename)), False)
        Else
            MessageBox.Show("Unable to find BaseRom.3DS.  Patch will not be tested.  If you are using a romfs made with Braindump, support for this is coming soon.")
        End If
    End Function
    Public Overrides Function CanRun() As Boolean
        Return False
    End Function
    Public Sub New()
        MyBase.New
    End Sub
End Class