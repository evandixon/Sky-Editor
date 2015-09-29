Imports ROMEditor.FileFormats
Imports SkyEditorBase

Namespace Mods
    Public Class KaomadoNDSMod
        Inherits GenericNDSMod
        Public Sub New()
            MyBase.New()
        End Sub

        Public Overrides Function FilesToCopy() As IEnumerable(Of String)
            Return {IO.Path.Combine("Data", "FONT", "kaomado.kao")}
        End Function

        Public Overrides Async Function InitializeAsync(CurrentProject As Project) As Task
            Dim portraitDir = IO.Path.Combine(ModDirectory, "Pokemon", "Portraits")
            If Not IO.Directory.Exists(portraitDir) Then IO.Directory.CreateDirectory(portraitDir)
            Dim k As New Kaomado(IO.Path.Combine(ROMDirectory, "data", "FONT", "kaomado.kao"))
            Await Kaomado.RunUnpack(IO.Path.Combine(ROMDirectory, "data", "FONT", "kaomado.kao"), portraitDir)
            Await k.ApplyMissingPortraitFix(portraitDir)
        End Function

        Public Overrides Async Function BuildAsync(CurrentProject As Project) As Task
            'Convert portraits
            If IO.Directory.Exists(IO.Path.Combine(ModDirectory, "Pokemon", "Portraits")) Then
                Await Kaomado.RunPack(IO.Path.Combine(ROMDirectory, "data", "FONT", "kaomado.kao"), IO.Path.Combine(ModDirectory, "Pokemon", "Portraits"))
            End If
        End Function

        Public Sub New(Filename As String)
            MyBase.New(Filename)
        End Sub
    End Class
End Namespace
