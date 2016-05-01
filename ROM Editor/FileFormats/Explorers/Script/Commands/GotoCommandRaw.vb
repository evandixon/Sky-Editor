Namespace FileFormats.Explorers.Script.Commands
    ''' <summary>
    ''' Represents a compiled Goto command, pointing to the address of a specific command.
    ''' </summary>
    Public Class GotoCommandRaw
        Inherits RawCommand

        <CommandParameter(0)> Public Property Target As GotoTarget
    End Class

End Namespace
