Imports System.Deployment.Application
Imports System.IO
Imports System.Reflection

Public Class EnvironmentPaths
    ''' <summary>
    ''' Gets the directory used to store extensions.
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function GetExtensionDirectory() As String
        Return Path.Combine(GetRootResourceDirectory, "Extensions")
    End Function

    ''' <summary>
    ''' Combines the given path with your plugin's resource directory.
    ''' </summary>
    ''' <param name="Path"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetResourceName(Path As String) As String
        Return GetResourceName(Path, Assembly.GetCallingAssembly.GetName.Name)
    End Function

    Public Shared Function GetResourceName(resoucePath As String, pluginName As String, Optional ThrowIfCantCreateDirectory As Boolean = False) As String
        Dim fullPath = Path.Combine(GetResourceDirectory(pluginName), resoucePath)
        Dim baseDir = Path.GetDirectoryName(fullPath)
        Try
            If Not Directory.Exists(baseDir) Then
                Directory.CreateDirectory(baseDir)
            End If
        Catch ex As UnauthorizedAccessException
            If ThrowIfCantCreateDirectory Then
                Throw ex
            End If
        End Try
        Return fullPath
    End Function
    ''' <summary>
    ''' Returns your plugin's resource directory as managed by Sky Editor.
    ''' It will be created if it does not exist.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetResourceDirectory() As String
        Return GetResourceDirectory(Assembly.GetCallingAssembly.GetName.Name)
    End Function

    ''' <summary>
    ''' Gets the resource directory for the plugin with the given assembly name.
    ''' </summary>
    ''' <param name="AssemblyName"></param>
    ''' <param name="ThrowIfCantCreateDirectory"></param>
    ''' <returns></returns>
    Public Shared Function GetResourceDirectory(AssemblyName As String, Optional ThrowIfCantCreateDirectory As Boolean = False) As String
        Dim baseDir = Path.Combine(GetRootResourceDirectory, "Extensions", "Plugins", "Development", AssemblyName)
        If Directory.Exists(baseDir) Then
            Return baseDir
        ElseIf Directory.Exists(Path.Combine(Environment.CurrentDirectory, AssemblyName)) Then
            Return Path.Combine(Environment.CurrentDirectory, AssemblyName)
        Else
            Try
                Directory.CreateDirectory(baseDir)
            Catch ex As UnauthorizedAccessException
                If ThrowIfCantCreateDirectory Then
                    Throw ex
                End If
            End Try
            Return baseDir
        End If
    End Function

    ''' <summary>
    ''' Returns a the path of the root resource directory and creates it if it doesn't exist.
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function GetRootResourceDirectory(Optional ThrowIfCantCreateDirectory As Boolean = False) As String
        Dim d As String
        If ApplicationDeployment.IsNetworkDeployed Then
            'I'm choosing not to verify if the folder exists because I'm already going to check below.
            d = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create)
        Else
            d = Path.Combine(Environment.CurrentDirectory, "Resources")
            If Not Directory.Exists(d) Then
                Try
                    Directory.CreateDirectory(d)
                Catch ex As UnauthorizedAccessException
                    If ThrowIfCantCreateDirectory Then
                        Throw ex
                    End If
                End Try
            End If
        End If
        Return d
    End Function

    ''' <summary>
    ''' Gets the full path for the application's settings file.
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function GetSettingsFilename() As String
        Return Path.Combine(EnvironmentPaths.GetRootResourceDirectory, "settings.json")
    End Function

    ''' <summary>
    ''' Gets absolute paths of all the assemblies in the development plugin directory.
    ''' Not all of these are guarenteed to be supported plugins.
    ''' </summary>
    ''' <returns></returns>
    <Obsolete> Public Shared Function GetPluginAssemblies() As List(Of String)
        Dim FromFolder = Path.Combine(GetRootResourceDirectory, "Extensions", "Plugins", "Development")
        Dim assemblyPaths As New List(Of String)
        If Directory.Exists(FromFolder) Then
            For Each item In Directory.GetFiles(FromFolder, "*.dll")
                If Not assemblyPaths.Contains(item) Then
                    assemblyPaths.Add(item)
                End If
            Next
            For Each item In Directory.GetFiles(FromFolder, "*.exe")
                If Not assemblyPaths.Contains(item) Then
                    assemblyPaths.Add(item)
                End If
            Next
        Else
            'If the "proper" plugin directory doesn't exist, we'll also load from the working directory (because plugins often reference SkyEditor.exe, which will be copied to the same directory).
            For Each item In Directory.GetFiles(Environment.CurrentDirectory, "*.dll")
                If Not assemblyPaths.Contains(item) Then
                    assemblyPaths.Add(item)
                End If
            Next
            For Each item In Directory.GetFiles(Environment.CurrentDirectory, "*.exe")
                If Not assemblyPaths.Contains(item) Then
                    assemblyPaths.Add(item)
                End If
            Next
        End If
        Return assemblyPaths
    End Function
End Class
