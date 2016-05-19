Namespace ConsoleCommands
    ''' <summary>
    ''' An IConsoleProvider that does nothing, existing to allow running ConsoleCommands outside of an environment with a console.
    ''' </summary>
    Public Class DummyConsoleProvider
        Implements IConsoleProvider

        Public Property BackgroundColor As ConsoleColor Implements IConsoleProvider.BackgroundColor
            Get
                Return ConsoleColor.Black
            End Get
            Set(value As ConsoleColor)
            End Set
        End Property

        Public Property ForegroundColor As ConsoleColor Implements IConsoleProvider.ForegroundColor
            Get
                Return ConsoleColor.White
            End Get
            Set(value As ConsoleColor)
            End Set
        End Property

        Public Sub Write(value() As Char) Implements IConsoleProvider.Write
        End Sub

        Public Sub Write(value As String) Implements IConsoleProvider.Write
        End Sub

        Public Sub Write(value As Object) Implements IConsoleProvider.Write
        End Sub

        Public Sub Write(value As Boolean) Implements IConsoleProvider.Write
        End Sub

        Public Sub Write(format As String, ParamArray arg() As Object) Implements IConsoleProvider.Write
        End Sub

        Public Sub Write(value() As Char, index As Integer, count As Integer) Implements IConsoleProvider.Write
        End Sub

        Public Sub WriteLine() Implements IConsoleProvider.WriteLine
        End Sub

        Public Sub WriteLine(value() As Char) Implements IConsoleProvider.WriteLine
        End Sub

        Public Sub WriteLine(value As Object) Implements IConsoleProvider.WriteLine
        End Sub

        Public Sub WriteLine(value As String) Implements IConsoleProvider.WriteLine
        End Sub

        Public Sub WriteLine(format As String, ParamArray arg() As Object) Implements IConsoleProvider.WriteLine
        End Sub

        Public Sub WriteLine(value() As Char, index As Integer, count As Integer) Implements IConsoleProvider.WriteLine
        End Sub

        Public Function Read() As Integer Implements IConsoleProvider.Read
            Return 0
        End Function

        Public Function ReadLine() As String Implements IConsoleProvider.ReadLine
            Return String.Empty
        End Function
    End Class
End Namespace

