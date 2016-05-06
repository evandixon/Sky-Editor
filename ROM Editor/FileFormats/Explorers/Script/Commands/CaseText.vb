Namespace FileFormats.Explorers.Script.Commands
    Public Class CaseText
        Inherits RawCommand
        <CommandParameter(0)> Public Property CaseValue As UInt16
        <CommandParameter(1)> Property Text As StringCommandParameter

        Public Overrides Function ToString() As String
            Return String.Format(My.Resources.SsbCommandNames.CaseText, CaseValue, Text.English)
        End Function
    End Class
End Namespace

