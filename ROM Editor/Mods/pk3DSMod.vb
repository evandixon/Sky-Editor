Imports SkyEditorBase

Namespace Mods
    Public Class pk3DSMod
        Inherits GenericMod

        Public Overrides Function SupportedGameCodes() As IEnumerable(Of Type)
            Return {GetType(Roms.XYRom), GetType(Roms.ORASRom)}
        End Function

        Public Overrides Sub Initialize(CurrentProject As Project)
            MyBase.Initialize(CurrentProject)

            IO.File.Copy(PluginHelper.GetResourceName("pk3DS.exe"), IO.Path.Combine(ModDirectory, "pk3DS.exe"))
            CurrentProject.CreateDirectory("Mods/" & IO.Path.GetFileNameWithoutExtension(OriginalFilename))
            CurrentProject.OpenFile(IO.Path.Combine(ModDirectory, "pk3DS.exe"), "Mods/" & IO.Path.GetFileNameWithoutExtension(OriginalFilename) & "/pk3DS.exe", False)

            IO.File.WriteAllText(IO.Path.Combine(ModDirectory, "config.ini"), "RawFiles")
        End Sub
    End Class
End Namespace

