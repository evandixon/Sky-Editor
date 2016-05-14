Imports SkyEditor.Core

Public MustInherit Class WindowsCoreSkyEditorPlugin
    Inherits CoreSkyEditorPlugin

    Public Overrides Function GetIOProvider() As SkyEditor.Core.IO.IOProvider
        Return New IOProvider
    End Function

    Public Overrides Sub Load(manager As PluginManager)
        MyBase.Load(manager)

        manager.RegisterTypeRegister(GetType(ConsoleCommands.ConsoleCommandAsync))
    End Sub
End Class
