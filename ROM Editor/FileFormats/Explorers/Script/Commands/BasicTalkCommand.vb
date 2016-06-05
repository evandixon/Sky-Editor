Namespace FileFormats.Explorers.Script.Commands
    Public Class BasicTalkCommand
        Inherits RawCommand
        Implements ISingleStringParamCommand
        <CommandParameter(0)> Public Property Line As StringCommandParameter Implements ISingleStringParamCommand.Line

    End Class

End Namespace
