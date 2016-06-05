Imports SkyEditor.Core.Windows
Imports SkyEditor.Core.Windows.Processes
Imports SkyEditorBase

Public Module DSIconTool
    Sub ExtractIcon(RomPath As String, OutputPath As String)
        Dim romDirectory As String = EnvironmentPaths.GetResourceDirectory
        'Dim extractTask = SkyEditorBase.PluginHelper.RunProgram(IO.Path.Combine(romDirectory, "DSIconTool.exe"),
        '                                      String.Format("{0} {1}",
        '                                                    RomPath, OutputPath))
        'extractTask.Wait()
        ConsoleApp.RunProgram(IO.Path.Combine(romDirectory, "DSIconTool.exe"),
                                              String.Format("{0} {1}",
                                                            RomPath, OutputPath))
    End Sub
End Module