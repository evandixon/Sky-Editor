Imports SkyEditorBase

Module vgmstream
    ''' <summary>
    ''' Runs vgmstream's test.exe with the given options.
    ''' Converts supported stream files to .wav files.
    ''' </summary>
    ''' <param name="Input">Input filename.  Must be a format that vgmstream supports.</param>
    ''' <param name="Output">Ouput filename of the wav that will be created.</param>
    ''' <param name="LoopCount">Number of times the sound stream should loop.  Defaults to 2 times.</param>
    ''' <param name="FadeTime">Number of seconds to take when fading.  Defaults to 10 seconds.</param>
    ''' <param name="FadeDelay">Number of seconds to delay before fading.  Defaults to 0 seconds.</param>
    ''' <returns></returns>
    Sub RunVGMStreamSync(Input As String, Output As String, Optional LoopCount As Decimal? = Nothing, Optional FadeTime As Decimal? = Nothing, Optional FadeDelay As Decimal? = Nothing)
        Dim filename = PluginHelper.GetResourceName("vgmstream\test.exe")
        Dim arguments As New Text.StringBuilder

        arguments.Append($"-o ""{Output}"" ")

        If LoopCount IsNot Nothing Then
            arguments.Append($"-l {LoopCount} ")
        End If

        If FadeTime IsNot Nothing Then
            arguments.Append($"-f {FadeTime} ")
        End If

        If FadeDelay IsNot Nothing Then
            arguments.Append($"-d {FadeDelay} ")
        End If

        arguments.Append($"""{Input}""")

        PluginHelper.RunProgramSync(filename, arguments.ToString, False)
    End Sub
End Module
