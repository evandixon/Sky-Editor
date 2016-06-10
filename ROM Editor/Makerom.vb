Imports SkyEditor.Core.Windows
Imports SkyEditor.Core.Windows.Processes
Public Module Makerom
    Enum Format
        NCCH
        CXI
        CFA
        CCI
        CIA
    End Enum
    Async Function CreateCXI(RsfFilename As String, OutputFilename As String) As Task
        Await ConsoleApp.RunProgram(EnvironmentPaths.GetResourceName("makerom.exe"), $"-o ""{OutputFilename}"" -rsf ""{RsfFilename}"" -target t ")
    End Function
    Async Function CreateCCI(OutputFilename As String) As Task
        Await ConsoleApp.RunProgram(EnvironmentPaths.GetResourceName("makerom.exe"), $"-f cia -o ""{OutputFilename}"" -target t ")
    End Function
End Module
