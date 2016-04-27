Namespace FileFormats.Explorers.Script.Commands
    Public Class MonologueCommand
        Inherits RawCommand

        <CommandParameter(0)> Public Property Line As StringCommandParameter

        Public Overrides Function ToString() As String
            Return $"Monologue ""{Line}"""
        End Function

    End Class

End Namespace
