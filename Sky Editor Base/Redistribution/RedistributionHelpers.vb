Imports System.IO
Imports System.Reflection
Imports System.Threading.Tasks
Imports ICSharpCode.SharpZipLib.Zip
Imports SkyEditor.Core.Extensions.Plugins
Imports SkyEditorBase.Interfaces
Imports SkyEditorBase.Utilities

Namespace Redistribution
    'Legacy code to deal with the old manor of handling plugins, some of which will still be used for plugin development.
    Public Class RedistributionHelpers
        Public Shared Event ApplicationRestartRequested(sender As Object, e As EventArgs)

        ''' <summary>
        ''' Runs the PrepareForDistribution method in all plugins, deleting any files that aren't distribution safe.
        ''' </summary>
        ''' <param name="Manager"></param>
        Public Shared Sub PrepareForDistribution(Manager As PluginManager)
            PluginHelper.Writeline("Preparing for distribution...")
            For Each item In Manager.Plugins
                item.PrepareForDistribution()
            Next
            PluginHelper.Writeline("Distribution preparation complete.")
        End Sub

        Private Shared Function GetAssemblyDependencies(SourceAssembly As Assembly) As List(Of String)
            Dim out As New List(Of String)
            Dim devAssemblyPaths = PluginHelper.GetPluginAssemblies

            'Get the Sky Editor Plugin's resource directory
            Dim resourceDirectory = IO.Path.Combine(IO.Path.GetDirectoryName(SourceAssembly.Location), IO.Path.GetFileNameWithoutExtension(SourceAssembly.Location))
            If IO.Directory.Exists(resourceDirectory) Then
                out.Add(resourceDirectory)
            End If

            'Get regional resources
            Dim resourcesName = IO.Path.GetFileNameWithoutExtension(SourceAssembly.Location) & ".resources.dll"
            For Each item In IO.Directory.GetDirectories(IO.Path.GetDirectoryName(SourceAssembly.Location))
                If IO.File.Exists(IO.Path.Combine(item, resourcesName)) Then
                    out.Add(IO.Path.Combine(item, resourcesName))
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
        Public Shared Async Function PackPlugins(Plugins As IEnumerable(Of SkyEditorPlugin), DestinationFilename As String, Info As Extensions.ExtensionInfo) As Task
            Dim tempDir = IO.Path.Combine(Environment.CurrentDirectory, "PackageTemp" & Guid.NewGuid.ToString)
            Dim ToCopy As New List(Of String)
            For Each plugin In Plugins
                Dim plgAssembly = plugin.GetType.Assembly
                Dim filename = IO.Path.GetFileNameWithoutExtension(plgAssembly.Location)

                'Prepare the plugin for distribution
                Dim plg = (From p In PluginManager.GetInstance.Plugins Where p.GetType.Assembly.Location = plgAssembly.Location).FirstOrDefault
                If plg IsNot Nothing Then
                    plg.PrepareForDistribution()
                Else
                    'Then the assembly isn't currently loaded.  In this case, we'll load it and tell it to prepare for distribution.
                    Using reflector As New Utilities.AssemblyReflectionManager
                        reflector.LoadAssembly(plgAssembly.Location, "PackPlugin")
                        reflector.Reflect(plgAssembly.Location, Function(CurrentAssembly As Assembly, Args() As Object) As Object
                                                                    For Each result In From t In CurrentAssembly.GetTypes Where Utilities.ReflectionHelpers.IsOfType(t, GetType(SkyEditorPlugin)) AndAlso t.GetConstructor({}) IsNot Nothing
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
            Await Utilities.FileSystem.ReCreateDirectory(tempDir)
            For Each file In ToCopy
                If IO.File.Exists(file) Then
                    IO.File.Copy(file, file.Replace(IO.Path.GetDirectoryName(file), tempDir), True)
                Else
                    'It's probably a directory.
                    If IO.Directory.Exists(file) Then
                        Await FileSystem.CopyDirectory(file, file.Replace(IO.Path.GetDirectoryName(file), tempDir))
                        'Else
                        'Guess not.  Do nothing.
                    End If
                End If
            Next

            'Create the extension info file
            Info.ExtensionTypeName = GetType(Extensions.PluginExtensionType).AssemblyQualifiedName
            Info.IsEnabled = True
            For Each item In Plugins
                Info.ExtensionFiles.Add(IO.Path.GetFileName(item.GetType.Assembly.Location))
            Next
            Info.Save(IO.Path.Combine(tempDir, "info.skyext"))

            'Then zip it
            Utilities.Zip.Zip(tempDir, DestinationFilename)
            Await Utilities.FileSystem.DeleteDirectory(tempDir)
        End Function

        ''' <summary>
        ''' Deletes files and directories scheduled for deletion.
        ''' </summary>
        Public Shared Async Function DeleteScheduledFiles() As Task
            If Not IO.File.Exists(IO.Path.Combine(PluginHelper.RootResourceDirectory, "todelete.txt")) Then
                IO.File.WriteAllText(IO.Path.Combine(PluginHelper.RootResourceDirectory, "todelete.txt"), "")
            End If
            For Each item In IO.File.ReadAllLines(IO.Path.Combine(PluginHelper.RootResourceDirectory, "todelete.txt"))
                If IO.File.Exists(item) Then
                    IO.File.Delete(item)
                ElseIf IO.Directory.Exists(item) Then
                    Await Utilities.FileSystem.DeleteDirectory(item)
                End If
            Next
            IO.File.WriteAllText(IO.Path.Combine(PluginHelper.RootResourceDirectory, "todelete.txt"), "")
        End Function

        ''' <summary>
        ''' Schedules a file or directory for deletion.
        ''' </summary>
        ''' <param name="Path"></param>
        Public Shared Sub ScheduleDelete(Path As String)
            If Not IO.File.Exists(IO.Path.Combine(PluginHelper.RootResourceDirectory, "todelete.txt")) Then
                IO.File.WriteAllText(IO.Path.Combine(PluginHelper.RootResourceDirectory, "todelete.txt"), "")
            End If
            IO.File.AppendAllLines(IO.Path.Combine(PluginHelper.RootResourceDirectory, "todelete.txt"), {Path})
        End Sub

        ''' <summary>
        ''' Restarts the application.
        ''' </summary>
        Public Shared Sub RequestRestartProgram()
            RaiseEvent ApplicationRestartRequested(Nothing, New EventArgs)
        End Sub

    End Class

End Namespace