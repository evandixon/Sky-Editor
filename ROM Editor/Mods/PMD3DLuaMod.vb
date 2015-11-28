Imports SkyEditorBase

Namespace Mods
    Public Class PMD3DLuaMod
        Inherits GenericMod

        Public Overrides Function InitializeAsync(CurrentProject As Project) As Task
            Return MyBase.InitializeAsync(CurrentProject)
        End Function

        Public Overrides Function SupportedGameCodes() As IEnumerable(Of Type)
            Return {GetType(Roms.GatesToInfinityRom)}
        End Function

        Public Overrides Function FilesToCopy() As IEnumerable(Of String)
            Return {IO.Path.Combine("romfs", "script")}
        End Function
    End Class
End Namespace

