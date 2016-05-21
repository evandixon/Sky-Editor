Imports SkyEditor.Core.Windows
Imports SkyEditorBase

Public Module romTool3ds
    <Obsolete("Untested")> Async Function CompressCodeBin(SourceFilename As String, OutputFilename As String, Optional ShowLoadingMessage As Boolean = True) As Task
        Await PluginHelper.RunProgram(EnvironmentPaths.GetResourceName("rom_tool.exe"), $"-zvf ""{SourceFilename}"" --compress-type blz --compress-out ""{OutputFilename}""", ShowLoadingMessage)
    End Function
    <Obsolete("Untested")> Async Function BuildExefs(ExefsDirectory As String, ExheaderPath As String, OutputFilename As String, Optional ShowLoadingMessage As Boolean = True) As Task
        Await PluginHelper.RunProgram(EnvironmentPaths.GetResourceName("rom_tool.exe"), $"-cvtf exefs ""{ExefsDirectory}"" --exefs-dir ""{OutputFilename}"" --header ""{ExheaderPath}""", ShowLoadingMessage)
    End Function
    <Obsolete("Untested")> Async Function BuildCxi(NcchHeaderFilename As String, ExheaderFilename As String, ExefsFilename As String, RomfsFilename As String, OutputFilename As String, Optional ShowLoadingMessage As Boolean = True) As Task
        Await PluginHelper.RunProgram(EnvironmentPaths.GetResourceName("rom_tool.exe"), $"-cvtf cxi ""{OutputFilename}"" --header ""{NcchHeaderFilename}"" --exh ""{ExheaderFilename}"" --exefs ""{ExefsFilename}"" --romfs ""{RomfsFilename}""", ShowLoadingMessage)
    End Function
End Module
