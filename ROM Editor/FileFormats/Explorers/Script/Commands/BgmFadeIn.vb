Namespace FileFormats.Explorers.Script.Commands
    ''' <summary>
    ''' Fades in background music in BGM 1 or BGM 2.
    ''' </summary>
    Public Class BgmFadeIn
        Inherits RawCommand

        <CommandParameter(0)> Public Property SongID As UInt16
        <CommandParameter(1)> Public Property Frames As UInt16
        <CommandParameter(2)> Public Property Volume As UInt16

        Public Overrides Function ToString() As String
            Dim output As String
            If IsEoS Then
                If CommandID = &H21 Then
                    output = My.Resources.SsbCommandNames.BgmFadeIn
                Else
                    output = My.Resources.SsbCommandNames.Bgm2FadeIn
                End If
            Else
                If CommandID = &H1C Then
                    output = My.Resources.SsbCommandNames.BgmFadeIn
                Else
                    output = My.Resources.SsbCommandNames.Bgm2FadeIn
                End If
            End If

            Return String.Format(output, SongID, Frames, Volume)
        End Function
    End Class
End Namespace

