Imports SkyEditor.Core.Windows
Imports SkyEditorBase

Public Module Makerom
    Enum Format
        NCCH
        CXI
        CFA
        CCI
        CIA
    End Enum
    Async Function CreateCXI(RsfFilename As String, OutputFilename As String, Optional ShowLoadingWindow As Boolean = True) As Task
        Await PluginHelper.RunProgram(EnvironmentPaths.GetResourceName("makerom.exe"), $"-o ""{OutputFilename}"" -rsf ""{RsfFilename}"" -target t ", ShowLoadingWindow)
    End Function
    Async Function CreateCCI(OutputFilename As String, Optional ShowLoadingWindow As Boolean = True) As Task
        Await PluginHelper.RunProgram(EnvironmentPaths.GetResourceName("makerom.exe"), $"-f cia -o ""{OutputFilename}"" -target t ", ShowLoadingWindow)
    End Function
End Module
