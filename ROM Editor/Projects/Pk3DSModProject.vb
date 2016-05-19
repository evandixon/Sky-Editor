Imports SkyEditor.Core.IO
Imports SkyEditorBase

Namespace Projects
    Public Class Pk3DSModProject
        Inherits GenericModProject
        Public Overrides Function GetSupportedGameCodes() As IEnumerable(Of String)
            Return {GameStrings.PokemonXCode, GameStrings.PokemonYCode, GameStrings.ORCode, GameStrings.ASCode}
        End Function

        Public Overrides Async Function Initialize(Solution As Solution) As Task
            Await MyBase.Initialize(Solution)
            IO.File.Copy(PluginHelper.GetResourceName("pk3DS.exe"), IO.Path.Combine(GetRootDirectory, "pk3DS.exe"))
            Await Me.AddExistingFile("", IO.Path.Combine(GetRootDirectory, "pk3DS.exe"), CurrentPluginManager.CurrentIOProvider)
            IO.File.WriteAllText(IO.Path.Combine(GetRootDirectory, "config.ini"), IO.Path.GetFileName(Me.GetRawFilesDir))
        End Function
    End Class
End Namespace

