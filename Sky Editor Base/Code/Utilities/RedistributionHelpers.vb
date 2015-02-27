Imports System.IO
Imports ICSharpCode.SharpZipLib.Zip

Public Class RedistributionHelpers
    Public Shared Sub PrepareForDistribution(Manager As PluginManager, Argument As String)
        PluginHelper.Writeline("Preparing for distribution...")
        For Each item In Manager.Plugins
            item.PrepareForDistribution()
        Next
        PluginHelper.Writeline("Distribution preparation complete.")
    End Sub
    ''' <summary>
    ''' Packages the program and its Resources folder into a single zip file.
    ''' </summary>
    ''' <param name="Manager"></param>
    ''' <param name="ArchiveName"></param>
    ''' <remarks></remarks>
    Public Shared Sub PackProgram(Manager As PluginManager, ArchiveName As String)
        Dim blacklist As String() = {"DeSmuMe Integration_plg.pdb",
                                     "DeSmuMe Integration_plg.xml",
                                     "ICSharpCode.SharpZipLib.dll",
                                     "ROMEditor_plg.pdb",
                                     "ROMEditor_plg.xml",
                                     "SkyEditor.exe",
                                     "SkyEditor.pdb",
                                     "SkyEditor.xml",
                                     "SkyEditor_plg.dll.config",
                                     "SkyEditor_plg.pdb",
                                     "SkyEditor_plg.xml"}

        If Not IO.Directory.Exists(IO.Path.Combine(Environment.CurrentDirectory, "PackageTemp")) Then
            IO.Directory.CreateDirectory(IO.Path.Combine(Environment.CurrentDirectory, "PackageTemp"))
        Else
            IO.Directory.Delete(IO.Path.Combine(Environment.CurrentDirectory, "PackageTemp"), True)
            IO.Directory.CreateDirectory(IO.Path.Combine(Environment.CurrentDirectory, "PackageTemp"))
        End If
        If Not IO.Directory.Exists(IO.Path.Combine(Environment.CurrentDirectory, "Sky Editor Archives")) Then
            IO.Directory.CreateDirectory(IO.Path.Combine(Environment.CurrentDirectory, "Sky Editor Archives"))
        End If
        IO.File.Copy(IO.Path.Combine(Environment.CurrentDirectory, "SkyEditor.exe"), IO.Path.Combine(Environment.CurrentDirectory, "PackageTemp", "SkyEditor.exe"), True)
        IO.File.Copy(IO.Path.Combine(Environment.CurrentDirectory, "ICSharpCode.SharpZipLib.dll"), IO.Path.Combine(Environment.CurrentDirectory, "PackageTemp", "ICSharpCode.SharpZipLib.dll"), True)
        For Each File In IO.Directory.GetFiles(IO.Path.Combine(Environment.CurrentDirectory, "Resources"), "*", SearchOption.AllDirectories)
            If Not IO.Directory.Exists(IO.Path.GetDirectoryName(File.Replace(Environment.CurrentDirectory, IO.Path.Combine(Environment.CurrentDirectory, "PackageTemp")))) Then
                IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(File.Replace(Environment.CurrentDirectory, IO.Path.Combine(Environment.CurrentDirectory, "PackageTemp"))))
            End If
            If Not blacklist.Contains(IO.Path.GetFileName(File)) Then
                IO.File.Copy(File, File.Replace(Environment.CurrentDirectory, IO.Path.Combine(Environment.CurrentDirectory, "PackageTemp")), True)
            End If
        Next
        Dim z As New FastZip
        z.CreateZip(IO.Path.Combine(Environment.CurrentDirectory, "Sky Editor Archives", ArchiveName & ".zip"), IO.Path.Combine(Environment.CurrentDirectory, "PackageTemp"), True, ".*", ".*")

    End Sub
    ''' <summary>
    ''' Packs all plugins into one archive per plugin, or only one if PluginName is not nothing.
    ''' </summary>
    ''' <param name="Manager"></param>
    ''' <param name="PluginName">Name of the plugin to pack.  Pass in Nothing to pack all loaded plugins.</param>
    ''' <remarks></remarks>
    Public Shared Sub PackPlugins(Manager As PluginManager, PluginName As String)
        Dim PluginNames As New List(Of String)
        If String.IsNullOrWhiteSpace(PluginName) Then
            For Each item In Manager.Assemblies
                PluginNames.Add(IO.Path.GetFileName(item))
            Next
        Else
            PluginNames.Add(PluginName)
        End If
        For Each item In PluginNames
            Dim tempDir = IO.Path.Combine(Environment.CurrentDirectory, "PackageTemp")
            Dim ToCopy As New List(Of String)
            ToCopy.Add(PluginHelper.GetResourceDirectory(item.Replace(".dll", "").Replace(".exe", "")))
            ToCopy.Add(IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins", item))
            If Manager.PluginFiles.ContainsKey(item) Then
                ToCopy.AddRange(Manager.PluginFiles(item))
            End If
            If IO.Directory.Exists(tempDir) Then
                IO.Directory.Delete(tempDir, True)
            End If
            IO.Directory.CreateDirectory(tempDir)
            For Each file In ToCopy
                If IO.File.Exists(file) Then
                    IO.File.Copy(file, file.Replace(IO.Path.GetDirectoryName(file), tempDir), True)
                Else
                    'It's probably a directory.
                    If IO.Directory.Exists(file) Then
                        My.Computer.FileSystem.CopyDirectory(file, file.Replace(IO.Path.GetDirectoryName(file), tempDir), True)
                    End If
                End If
            Next
            Dim z As New FastZip
            z.CreateZip(IO.Path.Combine(Environment.CurrentDirectory, item.Replace(".dll", "").Replace(".exe", "") & ".zip"), tempDir, True, ".*", ".*")
            PluginHelper.Writeline("Packed plugin " & item, PluginHelper.LineType.Message)
        Next
    End Sub
    ''' <summary>
    ''' Unpacks all plugins into the appropriate folder, or only one if PluginName is not nothing.
    ''' </summary>
    ''' <param name="PluginName">Name of the plugin to pack.  Pass in Nothing to pack all loaded plugins.</param>
    ''' <remarks></remarks>
    Public Shared Sub UnpackPlugins(PluginName As String)
        Dim PluginNames As New List(Of String)
        If String.IsNullOrWhiteSpace(PluginName) Then
            For Each item In IO.Directory.GetFiles(Environment.CurrentDirectory, "*_plg.zip")
                PluginNames.Add(IO.Path.GetFileName(item))
            Next
        Else
            PluginNames.Add(PluginName)
        End If
        For Each item In PluginNames
            Dim z As New FastZip
            Dim plgDir As String = IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins")
            If Not IO.Directory.Exists(plgDir) Then
                IO.Directory.CreateDirectory(plgDir)
            End If
            z.ExtractZip(IO.Path.Combine(Environment.CurrentDirectory, IO.Path.GetFileName(item)).Replace(".exe", ".zip").Replace(".dll", ".zip"), plgDir, ".*")
            PluginHelper.Writeline("Unpacked plugin " & item, PluginHelper.LineType.Message)
        Next
    End Sub
    Public Shared Sub DeletePlugin(Manager As PluginManager, AssemblyName As String)
        Dim toDelete As New List(Of String)
        toDelete.Add(IO.Path.Combine(Manager.PluginFolder, AssemblyName))
        toDelete.Add(PluginHelper.GetResourceDirectory(AssemblyName.Replace(".exe", "").Replace(".dll", "")))
        If Manager.PluginFiles.ContainsKey(AssemblyName) Then
            toDelete.AddRange(Manager.PluginFiles(AssemblyName))
        End If
        For Each item In toDelete
            ScheduleDelete(item)
        Next
        RestartProgram()
        'Manager.ReloadPlugins()
    End Sub
    Public Shared Sub InstallUnknownPlugins()
        Dim a = GetAssemblies()
        Dim PluginNames As New List(Of String)
        For Each item In IO.Directory.GetFiles(Environment.CurrentDirectory, "*_plg.zip")
            PluginNames.Add(IO.Path.GetFileName(item))
        Next
        For Each item In PluginNames
            Dim install As Boolean = True
            For Each assembly In a
                If IO.Path.GetFileName(assembly) = item.Replace(".zip", ".dll") OrElse IO.Path.GetFileName(assembly) = item.Replace(".exe", ".dll") Then
                    install = False
                    Exit For
                End If
            Next
            If install Then
                UnpackPlugins(item.Replace(".zip", ".dll").Replace(".exe", ".dll"))
            End If
        Next
    End Sub
    Public Shared Function GetAssemblies(PluginDirectory As String) As List(Of String)
        Dim assemblies As New List(Of String)
        If Not IO.Directory.Exists(PluginDirectory) Then
            IO.Directory.CreateDirectory(PluginDirectory)
        End If
        assemblies.AddRange(IO.Directory.GetFiles(PluginDirectory, "*_plg.dll"))
        assemblies.AddRange(IO.Directory.GetFiles(PluginDirectory, "*_plg.exe"))
        Return assemblies
    End Function
    Public Shared Function GetAssemblies() As List(Of String)
        Return GetAssemblies(IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins"))
    End Function
    Public Shared Sub DeleteScheduledFiles()
        If Not IO.File.Exists(IO.Path.Combine(PluginHelper.RootResourceDirectory, "todelete.txt")) Then
            IO.File.WriteAllText(IO.Path.Combine(PluginHelper.RootResourceDirectory, "todelete.txt"), "")
        End If
        For Each item In IO.File.ReadAllLines(IO.Path.Combine(PluginHelper.RootResourceDirectory, "todelete.txt"))
            If IO.File.Exists(item) Then
                IO.File.Delete(item)
            ElseIf IO.Directory.Exists(item) Then
                My.Computer.FileSystem.DeleteDirectory(item, FileIO.DeleteDirectoryOption.DeleteAllContents)
            End If
        Next
        IO.File.WriteAllText(IO.Path.Combine(PluginHelper.RootResourceDirectory, "todelete.txt"), "")
    End Sub
    Public Shared Sub ScheduleDelete(Path As String)
        If Not IO.File.Exists(IO.Path.Combine(PluginHelper.RootResourceDirectory, "todelete.txt")) Then
            IO.File.WriteAllText(IO.Path.Combine(PluginHelper.RootResourceDirectory, "todelete.txt"), "")
        End If
        IO.File.AppendAllLines(IO.Path.Combine(PluginHelper.RootResourceDirectory, "todelete.txt"), {Path})
    End Sub
    Public Shared Sub RestartProgram()
        'Windows.Forms.Application.Restart()
        'Application.Current.Shutdown()
        Application.Current.Shutdown(1)
    End Sub
End Class
