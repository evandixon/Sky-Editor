Namespace FileFormats.Explorers.Script.Commands
    Public Class MonologueCommand
        Inherits RawCommand
        Implements ISingleStringParamCommand

        <CommandParameter(0)> Public Property Line As StringCommandParameter Implements ISingleStringParamCommand.Line

        Public Overrides Function ToString() As String
            Return $"Monologue ""{Line}"""
        End Function

        Public Sub New()
            MyBase.New
        End Sub

    End Class

End Namespace
