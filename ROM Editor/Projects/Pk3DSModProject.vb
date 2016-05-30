Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Windows
Imports SkyEditorBase

Namespace Projects
    Public Class Pk3DSModProject
        Inherits GenericModProject
        Public Overrides Function GetSupportedGameCodes() As IEnumerable(Of String)
            Return {GameStrings.PokemonXCode, GameStrings.PokemonYCode, GameStrings.ORCode, GameStrings.ASCode}
        End Function

        Protected Overrides Async Function Initialize() As Task
            Await MyBase.Initialize
            IO.File.Copy(EnvironmentPaths.GetResourceName("pk3DS.exe"), IO.Path.Combine(GetRootDirectory, "pk3DS.exe"))
            Await Me.AddExistingFile("", IO.Path.Combine(GetRootDirectory, "pk3DS.exe"), CurrentPluginManager.CurrentIOProvider)
            IO.File.WriteAllText(IO.Path.Combine(GetRootDirectory, "config.ini"), IO.Path.GetFileName(Me.GetRawFilesDir))
        End Function
    End Class
End Namespace

