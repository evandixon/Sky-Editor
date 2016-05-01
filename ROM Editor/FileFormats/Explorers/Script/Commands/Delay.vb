Namespace FileFormats.Explorers.Script.Commands

    Public Class Delay
        Inherits RawCommand

        <CommandParameter(0)> Public Property Frames As UInt16

        Public Overrides Function ToString() As String
            Return String.Format(My.Resources.SsbCommandNames.Delay, Frames)
        End Function
    End Class

End Namespace
