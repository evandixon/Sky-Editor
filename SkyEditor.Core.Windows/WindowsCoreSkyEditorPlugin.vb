Imports System.Reflection
Imports SkyEditor.Core
Imports SkyEditor.Core.ConsoleCommands

Public MustInherit Class WindowsCoreSkyEditorPlugin
    Inherits CoreSkyEditorPlugin

    Public Overrides Function GetIOProvider() As SkyEditor.Core.IO.IOProvider
        Return New WindowsIOProvider
    End Function

    Public Overrides Function GetConsoleProvider() As IConsoleProvider
        Return New WindowsConsoleProvider
    End Function

    Public Overrides Sub Load(manager As PluginManager)
        MyBase.Load(manager)

        manager.RegisterTypeRegister(GetType(ConsoleCommands.ConsoleCommandAsync))
    End Sub

    Public Overrides Function IsPluginLoadingEnabled() As Boolean
        Return True
    End Function

    Public Overrides Function LoadAssembly(assemblyPath As String) As Assembly
        ''First, check to see if we already loaded it
        'Dim name As AssemblyName = AssemblyName.GetAssemblyName(assemblyPath)
        'Dim q1 = From a In AppDomain.CurrentDomain.GetAssemblies Where a.FullName = name.FullName

        'If q1.Any Then
        '    'If we did, then there's no point in loading it again.  In some cases, it could cause more problems
        '    Return q1.First
        'Else
        '    'If we didn't, then load it
        If WindowsReflectionHelpers.IsSupportedPlugin(assemblyPath) Then
            Return Assembly.LoadFrom(assemblyPath)
        Else
            Return Nothing
        End If

        'End If
    End Function


End Class
