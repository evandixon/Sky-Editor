Imports SkyEditorBase

Namespace Mods
    Public Class pk3DSMod
        Inherits GenericMod

        Public Overrides Function SupportedGameCodes() As IEnumerable(Of String)
            Return {GameStrings.PokemonXCode, GameStrings.PokemonYCode, GameStrings.ORCode, GameStrings.ASCode}
        End Function

        Public Overrides Sub Initialize(CurrentProject As ProjectOld)
            MyBase.Initialize(CurrentProject)

            IO.File.Copy(PluginHelper.GetResourceName("pk3DS.exe"), IO.Path.Combine(ModDirectory, "pk3DS.exe"))
            CurrentProject.CreateDirectory("Mods/" & IO.Path.GetFileNameWithoutExtension(Filename))
            CurrentProject.OpenFile(IO.Path.Combine(ModDirectory, "pk3DS.exe"), "Mods/" & IO.Path.GetFileNameWithoutExtension(Filename) & "/pk3DS.exe", False)

            IO.File.WriteAllText(IO.Path.Combine(ModDirectory, "config.ini"), "RawFiles")
        End Sub
    End Class
End Namespace

