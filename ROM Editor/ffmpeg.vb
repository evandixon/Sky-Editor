Module ffmpeg
    Sub ConvertToMp3(InputFilename As String, OutputFilename As String)
        SkyEditorBase.PluginHelper.RunProgramSync(SkyEditorBase.PluginHelper.GetResourceName("ffmpeg/ffmpeg.exe"), $"-i ""{InputFilename}"" -ar 48000 -acodec libmp3lame ""{OutputFilename}""", False)
    End Sub
End Module
