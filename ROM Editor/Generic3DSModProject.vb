Imports System.Windows.Forms
Imports ROMEditor.Roms
Imports SkyEditorBase

Public Class Generic3DSModProject
    Inherits GenericNDSModProject
    Public Overrides Async Sub Initialize()
        Dim o As New OpenFileDialog
        o.Filter = "3DS DS Roms (*.3ds;*.3dz)|*.3ds;*.3dz|All Files (*.*)|*.*"
        If o.ShowDialog = DialogResult.OK Then
            OpenFile(o.FileName, "BaseRom.3ds")
            Dim romDirectory = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "BaseRom RawFiles")
            Dim sky = DirectCast(Files("BaseRom.3ds"), iPackedRom)
            Await sky.Unpack(romDirectory)
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
        Await PluginHelper.RunProgram(IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "ModPack Files", "DSPatcher.exe"), String.Format("""{0}"" ""{1}""", IO.Path.Combine(IO.Path.GetDirectoryName(Filename), BaseRomFilename), IO.Path.Combine(IO.Path.GetDirectoryName(Filename), OutputRomFilename)), False)
    End Function
    Public Overrides Function CanRun() As Boolean
        Return False
    End Function
End Class
