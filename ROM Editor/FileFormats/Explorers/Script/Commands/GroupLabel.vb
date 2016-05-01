Namespace FileFormats.Explorers.Script.Commands
    ''' <summary>
    ''' The start of a group.
    ''' </summary>
    Public Class GroupLabel
        Inherits LogicalCommand

        Public Property GroupIndex As UShort
        Public Property Type As UShort
        Public Property Unknown As UShort

        Public Overrides Function ToString() As String
            Return String.Format(My.Resources.SsbCommandNames.GroupLabel, GroupIndex, Type, Unknown)
        End Function
    End Class
End Namespace

