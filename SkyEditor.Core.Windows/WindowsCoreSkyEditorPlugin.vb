Imports System.Reflection
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

    Public Overrides Function IsPluginLoadingEnabled() As Boolean
        Return True
    End Function

    Public Overrides Function LoadAssembly(assemblyPath As String) As Assembly
        Return Assembly.LoadFrom(assemblyPath)
    End Function
End Class
