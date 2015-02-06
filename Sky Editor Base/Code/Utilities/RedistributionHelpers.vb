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
End Class
