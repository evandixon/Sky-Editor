Imports System.Windows.Forms
Imports ROMEditor.Roms

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
End Class
