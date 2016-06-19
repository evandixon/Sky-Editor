Module ffmpeg
    Async Function ConvertToMp3(InputFilename As String, OutputFilename As String) As Task
        Using external As New ExternalProgramManager
            Await external.RunFFMpeg($"-i ""{InputFilename}"" -ar 48000 -acodec libmp3lame ""{OutputFilename}""")
        End Using
    End Function
End Module
