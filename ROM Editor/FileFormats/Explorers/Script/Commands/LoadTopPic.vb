Namespace FileFormats.Explorers.Script.Commands
    Public Class LoadTopPic
        Inherits RawCommand

        <CommandParameter(0)> Public Property PictureID As UInt16

        Public Overrides Function ToString() As String
            Dim output As String
            If IsEoS Then
                If CommandID = &H17 Then
                    output = My.Resources.SsbCommandNames.LoadTopPic
                Else
                    output = My.Resources.SsbCommandNames.LoadTopPic2
                End If
            Else
                If CommandID = &H13 Then
                    output = My.Resources.SsbCommandNames.LoadTopPic
                Else
                    output = My.Resources.SsbCommandNames.LoadTopPic2
                End If
            End If

            Return String.Format(output, PictureID)
        End Function
    End Class

End Namespace
