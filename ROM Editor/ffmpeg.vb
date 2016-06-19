Imports SkyEditor.Core.Windows.Processes

Module ffmpeg
    Async Function ConvertToMp3(ffmpegPath As String, InputFilename As String, OutputFilename As String) As Task
        Await ConsoleApp.RunProgram(ffmpegPath, $"-i ""{InputFilename}"" -ar 48000 -acodec libmp3lame ""{OutputFilename}""")
    End Function
End Module
