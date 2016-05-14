Imports System.Threading.Tasks
Imports SkyEditor.Core.ConsoleCommands
Imports SkyEditorBase.Redistribution

Namespace ConsoleCommands
    Public Class GeneratePluginExtensions
        Inherits ConsoleCommandAsync

        Public Overrides Async Function MainAsync(Arguments() As String) As Task
            Dim manager = PluginManager.GetInstance
            For Each item In manager.Plugins
                Dim a = item.GetType.Assembly
                If Not manager.IsAssemblyDependant(a) Then
                    Dim info As New SkyEditorBase.Extensions.ExtensionInfo
                    info.Name = item.PluginName
                    info.Author = item.PluginAuthor
                    info.Version = item.GetType.Assembly.GetName.Version.ToString
                    Dim path = IO.Path.Combine("PluginsTest", a.GetName.Name & ".zip")
                    If Not IO.Directory.Exists(IO.Path.GetDirectoryName(path)) Then
                        IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(path))
                    End If
                    Await RedistributionHelpers.PackPlugins({item}, path, info)
                End If
            Next
        End Function
    End Class

End Namespace
