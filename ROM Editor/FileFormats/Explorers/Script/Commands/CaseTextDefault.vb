Namespace FileFormats.Explorers.Script.Commands
    Public Class CaseTextDefault
        Inherits RawCommand
        Implements ISingleStringParamCommand

        <CommandParameter(0)> Public Property Text As StringCommandParameter Implements ISingleStringParamCommand.Line

        Public Overrides Function ToString() As String
            Return String.Format(My.Resources.SsbCommandNames.CaseTextDefault, Text.English)
        End Function
    End Class
End Namespace

