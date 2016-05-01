Namespace FileFormats.Explorers.Script.Commands
    Public Class ImagePos
        Inherits RawCommand

        <CommandParameter(0)> Public Property C As UInt16
        <CommandParameter(1)> Public Property R As UInt16
        <CommandParameter(2)> Public Property X As UInt16
        <CommandParameter(3)> Public Property Y As UInt16

        Public Overrides Function ToString() As String
            Return String.Format(My.Resources.SsbCommandNames.ImagePos, C, R, X, Y)
        End Function
    End Class

End Namespace
