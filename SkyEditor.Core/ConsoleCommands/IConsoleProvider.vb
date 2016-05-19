Namespace ConsoleCommands
    Public Interface IConsoleProvider
        Property BackgroundColor As ConsoleColor
        Property ForegroundColor As ConsoleColor
        Function Read() As Integer
        Function ReadLine() As String
        Sub Write(value As Boolean)
        Sub Write(value As Char())
        Sub Write(value As Char(), index As Integer, count As Integer)
        Sub Write(value As Object)
        Sub Write(value As String)
        Sub Write(format As String, ParamArray arg As Object())
        Sub WriteLine()
        Sub WriteLine(value As String)
        Sub WriteLine(value As Char())
        Sub WriteLine(value As Char(), index As Integer, count As Integer)
        Sub WriteLine(value As Object)
        Sub WriteLine(format As String, ParamArray arg As Object())
    End Interface
End Namespace

