Imports SkyEditor.Core.Windows
Imports SkyEditor.Core.Windows.Processes

Module ffmpeg
    Async Function ConvertToMp3(InputFilename As String, OutputFilename As String) As Task
        Await ConsoleApp.RunProgram(EnvironmentPaths.GetResourceName("ffmpeg/ffmpeg.exe"), $"-i ""{InputFilename}"" -ar 48000 -acodec libmp3lame ""{OutputFilename}""")
    End Function
End Module
