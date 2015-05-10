Imports SkyEditorBase

Module DSIconTool
    Sub ExtractIcon(RomPath As String, OutputPath As String)
        Dim romDirectory As String = PluginHelper.GetResourceDirectory
        Dim extractTask = SkyEditorBase.PluginHelper.RunProgram(IO.Path.Combine(romDirectory, "DSIconTool.exe"),
                                              String.Format("{0} {1}",
                                                            RomPath, OutputPath))
        extractTask.Wait()
    End Sub
End Module