Namespace FileFormats.Explorers.Script.Commands
    ''' <summary>
    ''' An unknown command in an SSB script.
    ''' </summary>
    Public Class RawCommand
        Inherits LogicalCommand
        Public Property CommandID As UInt16
        ''' <summary>
        ''' The parameters for the command if there are no properties with the appropriate attribute.
        ''' In any case, this must be the proper size for the command.
        ''' </summary>
        ''' <returns></returns>
        Public Property Params As List(Of UInt16)

        ''' <summary>
        ''' Gets or sets whether or not this command is a command for Explorers of Sky.  If false, it's for Explorers of Time/Darkness.
        ''' </summary>
        ''' <returns></returns>
        Public Property IsEoS As Boolean

        Public Overrides Function ToString() As String
            Return $"Raw {CommandID}"
        End Function
    End Class
End Namespace

