Imports System.IO
Imports System.Reflection
Imports System.Threading.Tasks
Imports SkyEditor
Imports SkyEditor.Core
Imports SkyEditor.Core.Extensions
Imports SkyEditor.Core.Utilities
Imports SkyEditor.Core.Windows

Namespace Redistribution
    'Legacy code to deal with the old manor of handling plugins, some of which will still be used for plugin development.
    Public Class RedistributionHelpers
        Public Shared Event ApplicationRestartRequested(sender As Object, e As EventArgs)

        ''' <summary>
        ''' Runs the PrepareForDistribution method in all plugins, deleting any files that aren't distribution safe.
        ''' </summary>
        ''' <param name="Manager"></param>
        Public Shared Sub PrepareForDistribution(Manager As PluginManager)
            For Each item In Manager.Plugins
                item.PrepareForDistribution()
            Next
        End Sub

        Private Shared Function GetAssemblyDependencies(SourceAssembly As Assembly) As List(Of String)
            Dim out As New List(Of String)
            Dim devAssemblyPaths = EnvironmentPaths.GetPluginAssemblies

            'Get the Sky Editor Plugin's resource directory
            Dim resourceDirectory = Path.Combine(Path.GetDirectoryName(SourceAssembly.Location), Path.GetFileNameWithoutExtension(SourceAssembly.Location))
            If Directory.Exists(resourceDirectory) Then
                out.Add(resourceDirectory)
            End If

            'Get regional resources
            Dim resourcesName = Path.GetFileNameWithoutExtension(SourceAssembly.Location) & ".resources.dll"
            For Each item In Directory.GetDirectories(Path.GetDirectoryName(SourceAssembly.Location))
                If File.Exists(Path.Combine(item, resourcesName)) Then
                    out.Add(Path.Combine(item, resourcesName))
                End If
            Next

            'Look at the dependencies
            For Each reference In SourceAssembly.GetReferencedAssemblies
                Dim isLocal As Boolean = False
                'Try to find the filename of this reference
                For Each source In devAssemblyPaths
                    Dim name = AssemblyName.GetAssemblyName(source)
                    If reference.FullName = name.FullName Then
                        If Not out.Contains(source) Then
                            out.Add(source)
                            isLocal = True
                            Exit For
                        End If
                    End If
                Next

                If isLocal Then
                    'Try to find the references of this reference
                    Dim q = (From a In AppDomain.CurrentDomain.GetAssemblies Where a.FullName = reference.FullName).FirstOrDefault

                    If q IsNot Nothing Then
                        out.AddRange(GetAssemblyDependencies(q))
                    Else
                        'Then this reference isn't in the app domain.
                        'Let's try to find the assembly.
                        'Todo: it would be optimal to do this in another Appdomain, but since this assembly would be loaded if needed, there's no real harm
                        For Each source In devAssemblyPaths
                            Dim name = AssemblyName.GetAssemblyName(source)
                            If reference.FullName = name.FullName Then
                                out.AddRange(GetAssemblyDependencies(Assembly.LoadFrom(source)))
                            End If
                        Next
                    End If
                End If
            Next

            Return out
        End Function

        ''' <summary>
        ''' Packs the given plugin into a zip file.
        ''' </summary>
        ''' <param name="Plugins">Definitions of the plugins to pack.</param>
        ''' <param name="DestinationFilename">File path of the zip to create.</param>
        ''' <returns></returns>
        Public Shared Async Function PackPlugins(Plugins As IEnumerable(Of SkyEditorPlugin), DestinationFilename As String, Info As ExtensionInfo, manager As PluginManager) As Task
            Dim tempDir = Path.Combine(Environment.CurrentDirectory, "PackageTemp" & Guid.NewGuid.ToString)
            Dim ToCopy As New List(Of String)
            For Each plugin In Plugins
                Dim plgAssembly = plugin.GetType.Assembly
                Dim filename = Path.GetFileNameWithoutExtension(plgAssembly.Location)

                'Prepare the plugin for distribution
                Dim plg = (From p In manager.Plugins Where p.GetType.Assembly.Location = plgAssembly.Location).FirstOrDefault
                If plg IsNot Nothing Then
                    plg.PrepareForDistribution()
                Else
                    'Then the assembly isn't currently loaded.  In this case, we'll load it and tell it to prepare for distribution.
                    Using reflector As New AssemblyReflectionManager
                        reflector.LoadAssembly(plgAssembly.Location, "PackPlugin")
                        reflector.Reflect(plgAssembly.Location, Function(CurrentAssembly As Assembly, Args() As Object) As Object
                                                                    For Each result In From t In CurrentAssembly.GetTypes Where ReflectionHelpers.IsOfType(t, GetType(SkyEditorPlugin).GetTypeInfo) AndAlso t.GetConstructor({}) IsNot Nothing
                                                                        Dim def As SkyEditorPlugin = result.GetConstructor({}).Invoke({})
                                                                        def.PrepareForDistribution()
                                                                    Next
                                                                    Return Nothing
                                                                End Function)
                    End Using
                End If

                'Find the files we should pack
                'ToCopy.Add(PluginFilename.Replace(".dll", "").Replace(".exe", ""))
                ToCopy.Add(plgAssembly.Location)
                'If Manager.PluginFiles.ContainsKey(filename) Then
                '    ToCopy.AddRange(Manager.PluginFiles(filename))
                'End If

                'Try to detect dependencies.
                For Each item In GetAssemblyDependencies(plgAssembly)
                    If Not ToCopy.Contains(item) Then
                        ToCopy.Add(item)
                    End If
                Next
            Next

            'Copy temporary files
            Await Core.Utilities.FileSystem.ReCreateDirectory(tempDir, manager.CurrentIOProvider)
            For Each filePath In ToCopy
                If File.Exists(filePath) Then
                    File.Copy(filePath, filePath.Replace(Path.GetDirectoryName(filePath), tempDir), True)
                Else
                    'It's probably a directory.
                    If Directory.Exists(filePath) Then
                        Await Core.Utilities.FileSystem.CopyDirectory(filePath, filePath.Replace(Path.GetDirectoryName(filePath), tempDir), manager.CurrentIOProvider)
                        'Else
                        'Guess not.  Do nothing.
                    End If
                End If
            Next

            'Create the extension info file
            Info.ExtensionTypeName = GetType(PluginExtensionType).AssemblyQualifiedName
            Info.IsEnabled = True
            For Each item In Plugins
                Info.ExtensionFiles.Add(Path.GetFileName(item.GetType.Assembly.Location))
            Next
            Info.Save(Path.Combine(tempDir, "info.skyext"), manager.CurrentIOProvider)

            'Then zip it
            Core.Utilities.Zip.Zip(tempDir, DestinationFilename)
            Await Core.Utilities.FileSystem.DeleteDirectory(tempDir, manager.CurrentIOProvider)
        End Function

        ''' <summary>
        ''' Restarts the application.
        ''' </summary>
        Public Shared Sub RequestRestartProgram()
            RaiseEvent ApplicationRestartRequested(Nothing, New EventArgs)
        End Sub

    End Class

End Namespace