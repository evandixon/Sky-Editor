﻿Module ffmpeg
    Async Function ConvertToMp3(InputFilename As String, OutputFilename As String) As Task
        Await SkyEditorBase.PluginHelper.RunProgram(SkyEditorBase.PluginHelper.GetResourceName("ffmpeg/ffmpeg.exe"), $"-i ""{InputFilename}"" -ar 48000 -acodec libmp3lame ""{OutputFilename}""", False)
    End Function
End Module