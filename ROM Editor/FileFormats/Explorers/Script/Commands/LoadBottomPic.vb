Namespace FileFormats.Explorers.Script.Commands
    Public Class LoadBottomPic
        Inherits RawCommand
        <CommandParameter(0)> Public Property PictureID As UInt16
        Public Overrides Function ToString() As String
            Return String.Format(My.Resources.SsbCommandNames.LoadBottomPic, PictureID)
        End Function
    End Class
End Namespace

