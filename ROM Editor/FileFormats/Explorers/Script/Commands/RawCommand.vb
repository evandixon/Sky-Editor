Namespace FileFormats.Explorers.Script.Commands
    Public Class RawCommand
        Public Property CommandID As UInt16
        ''' <summary>
        ''' The parameters for the command if there are no properties with the appropriate attribute.
        ''' In any case, this must be the proper size for the command.
        ''' </summary>
        ''' <returns></returns>
        Public Property Params As List(Of UInt16)

        Public Overrides Function ToString() As String
            Return $"Raw {CommandID}"
        End Function
    End Class
End Namespace

