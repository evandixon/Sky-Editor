Imports SkyEditor.Core.ConsoleCommands
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI
''' <summary>
''' A variant of SkyEditorPlugin that controls how the plugin manager loads other plugins.
''' </summary>
Public MustInherit Class CoreSkyEditorPlugin
    Inherits SkyEditorPlugin

    ''' <summary>
    ''' Creates an instance of an IO Provider.
    ''' </summary>
    ''' <returns></returns>
    Public MustOverride Function GetIOProvider() As IOProvider

    ''' <summary>
    ''' Creates an instance of an ISettingsProvider.
    ''' </summary>
    ''' <returns></returns>
    Public MustOverride Function GetSettingsProvider(manager As PluginManager) As ISettingsProvider

    Public Overridable Function GetConsoleProvider() As IConsoleProvider
        Return New DummyConsoleProvider
    End Function

    Public Overridable Function GetIOUIManager(manager As PluginManager) As IOUIManager
        Return New IOUIManager(manager)
    End Function

    ''' <summary>
    ''' Gets the full path of the directory used to store extensions.
    ''' </summary>
    ''' <returns></returns>
    Public MustOverride Function GetExtensionDirectory() As String

    ''' <summary>
    ''' Gets whether or not to enable dynamicaly loading plugins.
    ''' </summary>
    ''' <returns></returns>
    Public Overridable Function IsPluginLoadingEnabled() As Boolean
        Return False
    End Function

    ''' <summary>
    ''' Loads the assembly at the given path into the current AppDomain and returns it.
    ''' </summary>
    ''' <param name="assemblyPath">Full path of the assembly to load.</param>
    ''' <returns></returns>
    ''' <exception cref="NotSupportedException">Thrown when the current platform does not support loading assemblies from a specific path.</exception>
    ''' <exception cref="BadImageFormatException">Thrown when the assembly is not a valid .Net assembly.</exception>
    Public Overridable Function LoadAssembly(assemblyPath As String) As Reflection.Assembly
        Throw New NotSupportedException
    End Function

End Class

