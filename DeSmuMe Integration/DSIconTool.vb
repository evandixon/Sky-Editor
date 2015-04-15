Imports SkyEditorBase

Module DSIconTool
    Sub ExtractIcon(RomPath As String, OutputPath As String)
        Dim romDirectory As String = PluginHelper.GetResourceDirectory
        SkyEditorBase.PluginHelper.RunProgram(IO.Path.Combine(romDirectory, "dsicontool.exe"),
                                              String.Format("""{0}"" ""{1}""",
                                                            RomPath, OutputPath))
    End Sub
End Module