Imports SkyEditor.Core.Windows
Imports SkyEditor.Core.Windows.Processes
Imports SkyEditorBase

Public Module romTool3ds
    <Obsolete("Untested")> Async Function CompressCodeBin(SourceFilename As String, OutputFilename As String) As Task
        Await ConsoleApp.RunProgram(EnvironmentPaths.GetResourceName("rom_tool.exe"), $"-zvf ""{SourceFilename}"" --compress-type blz --compress-out ""{OutputFilename}""")
    End Function
    <Obsolete("Untested")> Async Function BuildExefs(ExefsDirectory As String, ExheaderPath As String, OutputFilename As String) As Task
        Await ConsoleApp.RunProgram(EnvironmentPaths.GetResourceName("rom_tool.exe"), $"-cvtf exefs ""{ExefsDirectory}"" --exefs-dir ""{OutputFilename}"" --header ""{ExheaderPath}""")
    End Function
    <Obsolete("Untested")> Async Function BuildCxi(NcchHeaderFilename As String, ExheaderFilename As String, ExefsFilename As String, RomfsFilename As String, OutputFilename As String) As Task
        Await ConsoleApp.RunProgram(EnvironmentPaths.GetResourceName("rom_tool.exe"), $"-cvtf cxi ""{OutputFilename}"" --header ""{NcchHeaderFilename}"" --exh ""{ExheaderFilename}"" --exefs ""{ExefsFilename}"" --romfs ""{RomfsFilename}""")
    End Function
End Module
