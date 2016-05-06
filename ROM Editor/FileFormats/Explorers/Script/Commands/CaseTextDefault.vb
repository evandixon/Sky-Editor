Namespace FileFormats.Explorers.Script.Commands
    Public Class CaseTextDefault
        Inherits RawCommand

        <CommandParameter(0)> Public Property Text As StringCommandParameter

        Public Overrides Function ToString() As String
            Return String.Format(My.Resources.SsbCommandNames.CaseTextDefault, Text.English)
        End Function
    End Class
End Namespace

