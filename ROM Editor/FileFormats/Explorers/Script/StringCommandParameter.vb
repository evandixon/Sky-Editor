Namespace FileFormats.Explorers.Script
    ''' <summary>
    ''' The parameter of a command that stores a game string.
    ''' </summary>
    Public Class StringCommandParameter
        ''' <summary>
        ''' The default value of the string.
        ''' 
        ''' In US and European games, this is English value of the string.
        ''' In Japanese games, this stores the Japanese value.
        ''' </summary>
        ''' <returns></returns>
        Public Property English As String

        Public Overrides Function ToString() As String
            Return English
        End Function
    End Class
End Namespace

