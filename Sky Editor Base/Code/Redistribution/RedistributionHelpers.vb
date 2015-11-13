Imports System.IO
Imports ICSharpCode.SharpZipLib.Zip
Imports System.Reflection
Imports System.Web.Script.Serialization
Imports SkyEditorBase.Utilities

Namespace Redistribution
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
                    PluginNames.Add(IO.Path.GetFileName(ReflectionHelpers.GetAssemblyFileName(item, Manager.PluginFolder)))
                Next
            Else
                PluginNames.Add(PluginName)
            End If
            For Each item In PluginNames
                Dim tempDir = IO.Path.Combine(Environment.CurrentDirectory, "PackageTemp")
                Dim ToCopy As New List(Of String)
                ToCopy.Add(PluginHelper.GetResourceDirectory(item.Replace(".dll", "").Replace(".exe", "")))
                ToCopy.Add(IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins", item))
                If Manager.PluginFiles.ContainsKey(item.Replace(".dll", "").Replace(".exe", "")) Then
                    ToCopy.AddRange(Manager.PluginFiles(item.Replace(".dll", "").Replace(".exe", "")))
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
                            'Else
                            'Guess not.
                        End If
                    End If
                Next
                Dim z As New FastZip
                z.CreateZip(IO.Path.Combine(Environment.CurrentDirectory, "PluginDist", item.Replace(".dll", "").Replace(".exe", "") & ".zip"), tempDir, True, ".*", ".*")
                PluginHelper.Writeline("Packed plugin " & item, PluginHelper.LineType.Message)
            Next
            Dim l = Language.LanguageManager.Instance
            l.LoadAllLanguages()
            l.SaveAll()
            For Each item In l.Languages.Keys
                IO.File.Copy(l.Languages(item).Filename, IO.Path.Combine(Environment.CurrentDirectory, "PluginDist", item & ".language"), True)
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
                For Each item In IO.Directory.GetFiles(PluginHelper.PluginsToInstallDirectory, "*_plg.zip")
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
                z.ExtractZip(IO.Path.Combine(PluginHelper.PluginsToInstallDirectory, IO.Path.GetFileName(item)).Replace(".exe", ".zip").Replace(".dll", ".zip"), plgDir, FastZip.Overwrite.Always, Nothing, ".*", ".*", True)
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
        Public Shared Sub InstallPendingPlugins()
            'Dim a = GetAssemblies()
            Dim PluginNames As New List(Of String)
            For Each item In IO.Directory.GetFiles(PluginHelper.PluginsToInstallDirectory, "*_plg.zip")
                PluginNames.Add(IO.Path.GetFileName(item))
            Next
            For Each item In PluginNames
                Dim install As Boolean = True
                'For Each assembly In a
                '    If IO.Path.GetFileName(assembly) = item.Replace(".zip", ".dll") OrElse IO.Path.GetFileName(assembly) = item.Replace(".exe", ".dll") Then
                '        install = False
                '        Exit For
                '    End If
                'Next
                If install Then
                    Try
                        UnpackPlugins(item.Replace(".zip", ".dll").Replace(".exe", ".dll"))
                    Catch ex As ICSharpCode.SharpZipLib.Zip.ZipException
                        'Let's assume the zip is bad and delete it.
                        'Because this will happen anyway, we simply don't extract anything.
                    End Try
                End If
                IO.File.Delete(IO.Path.Combine(PluginHelper.PluginsToInstallDirectory, item))
            Next
            For Each item In IO.Directory.GetFiles(PluginHelper.PluginsToInstallDirectory, "*.language")
                IO.File.Copy(item, IO.Path.Combine(PluginHelper.RootResourceDirectory, "Languages", IO.Path.GetFileNameWithoutExtension(item) & ".json"), True)
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
            Windows.Forms.Application.Restart()
            'Application.Current.Shutdown()
            'Application.Current.Shutdown(1)
        End Sub
        Public Shared Function RequiredPlugins(Plugins As List(Of PluginInfo)) As List(Of PluginInfo)
            Dim pending As New List(Of PluginInfo)
            For Each p In Plugins
                For Each d In RequiredPlugins(p.Dependencies)
                    If Not pending.Contains(d) Then
                        pending.Add(d)
                    End If
                Next
                If Not pending.Contains(p) Then pending.Add(p)
            Next
            Return pending
        End Function
        Public Shared Function UpdatePlugins(Manager As PluginManager, Plugins As List(Of PluginInfo)) As Boolean
            Dim c As New Net.WebClient
            Dim restart As Boolean = False
            For Each p In RequiredPlugins(Plugins)
                Dim assemblyName As String = Nothing
                Dim exists As Boolean = False
                Dim outdated As Boolean = True

                If p.Type = PluginInfo.PluginType.Code Then
                    For Each item In Manager.Assemblies
                        If ReflectionHelpers.GetAssemblyFileName(item, Manager.PluginFolder).Replace(".dll", "").Replace(".exe", "") = p.Name Then
                            exists = True
                            assemblyName = ReflectionHelpers.GetAssemblyFileName(item, Manager.PluginFolder)
                            If ReflectionHelpers.GetAssemblyVersion(item).CompareTo(p.GetVersion) >= 0 Then
                                outdated = False
                            End If
                        End If
                    Next
                ElseIf p.Type = PluginInfo.PluginType.Language
                    With Language.LanguageManager.Instance
                        .EnsureLanguageLoaded(p.Name)
                        If CInt(p.VersionString) > .Languages(p.Name).ContainedObject.Revision Then
                            outdated = False
                        End If
                    End With
                End If

                If outdated Then
                    If exists AndAlso assemblyName IsNot Nothing Then
                        PluginHelper.Writeline("Deleting plugin " & assemblyName)
                        DeletePlugin(Manager, assemblyName)
                    End If
                    PluginHelper.Writeline("Downloading plugin " & p.Name)
                    c.DownloadFile(p.DownloadUrl, IO.Path.Combine(PluginHelper.PluginsToInstallDirectory, IO.Path.GetFileName(p.DownloadUrl)))
                    restart = True
                End If
            Next
            Return restart
        End Function
        Public Shared Function ParsePluginInfo(PluginInfoText As String) As List(Of PluginInfo)
            Dim j As New JavaScriptSerializer
            Return j.Deserialize(Of List(Of PluginInfo))(PluginInfoText)
        End Function
        Public Shared Function SerializePluginInfo(Plugins As List(Of PluginInfo)) As String
            Dim j As New JavaScriptSerializer
            Return j.Serialize(Plugins)
        End Function
        Public Shared Function GetPluginInfo(InfoUrl As String) As List(Of PluginInfo)
            Dim c As New Net.WebClient
            c.Encoding = New Text.UTF8Encoding
            Return ParsePluginInfo(c.DownloadString(InfoUrl))
        End Function
        Public Shared Function GeneratePluginInfoString(Manager As PluginManager, WebDirectory As String) As String
            Dim plugins As New List(Of PluginInfo)
            For Each item In Manager.Assemblies
                Dim info As New PluginInfo
                info.Type = PluginInfo.PluginType.Code
                info.VersionString = ReflectionHelpers.GetAssemblyVersion(item).ToString
                info.Name = ReflectionHelpers.GetAssemblyFileName(item, Manager.PluginFolder).Replace(".exe", "").Replace(".dll", "")
                info.Dependencies = New List(Of PluginInfo)
                info.DownloadUrl = IO.Path.Combine(WebDirectory, ReflectionHelpers.GetAssemblyFileName(item, Manager.PluginFolder).Replace(".exe", "").Replace(".dll", "") & ".zip").Replace("\", "/")
                plugins.Add(info)
            Next
            Dim l = Language.LanguageManager.Instance
            l.LoadAllLanguages()
            For Each item In l.Languages.Keys
                Dim info As New PluginInfo
                info.Type = PluginInfo.PluginType.Language
                info.VersionString = l.Languages(item).ContainedObject.Revision.ToString
                info.Name = item
                info.Dependencies = New List(Of PluginInfo)
                info.DownloadUrl = IO.Path.Combine(WebDirectory, item & ".language").Replace("\", "/")
                plugins.Add(info)
            Next
            Return SerializePluginInfo(plugins)
        End Function
        Public Shared Sub GeneratePluginDownloadDir(Manager As PluginManager, WebDirectory As String)
            PackPlugins(Manager, Nothing)
            IO.File.WriteAllText(IO.Path.Combine(Environment.CurrentDirectory, "PluginDist", "plugins.json"), GeneratePluginInfoString(Manager, SettingsManager.Instance.Settings.PluginUpdateUrl.Replace("plugins.json", "")))
        End Sub
        Public Shared Function DownloadAllPlugins(Manager As PluginManager, Url As String) As Boolean
            Return UpdatePlugins(Manager, GetPluginInfo(Url))
        End Function
        Public Shared Sub PackageAll(Manager As PluginManager, Optional Argument As String = Nothing)
            If Not IO.Directory.Exists(IO.Path.Combine(Environment.CurrentDirectory, "PluginDist")) Then
                IO.Directory.CreateDirectory(IO.Path.Combine(Environment.CurrentDirectory, "PluginDist"))
            End If
            For Each item In IO.Directory.GetFiles(IO.Path.Combine(Environment.CurrentDirectory, "PluginDist"), "*", SearchOption.AllDirectories)
                Console.WriteLine("Deleting " & item)
                IO.File.Delete(item)
            Next
            PrepareForDistribution(Manager, Nothing)
            PackPlugins(Manager, Nothing)
            GeneratePluginDownloadDir(Manager, Nothing)
            PluginHelper.Writeline(PluginHelper.GetLanguageItem("All plugins packaged to:"))
            PluginHelper.Writeline(IO.Path.Combine(Environment.CurrentDirectory, "PluginDist"))
        End Sub
    End Class

End Namespace