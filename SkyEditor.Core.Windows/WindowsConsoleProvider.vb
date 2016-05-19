Imports SkyEditor.Core.ConsoleCommands

Public Class WindowsConsoleProvider
    Implements IConsoleProvider

    Public Property BackgroundColor As ConsoleColor Implements IConsoleProvider.BackgroundColor
        Get
            Return Console.BackgroundColor
        End Get
        Set(value As ConsoleColor)
            Console.BackgroundColor = value
        End Set
    End Property

    Public Property ForegroundColor As ConsoleColor Implements IConsoleProvider.ForegroundColor
        Get
            Return Console.ForegroundColor
        End Get
        Set(value As ConsoleColor)
            Console.ForegroundColor = value
        End Set
    End Property

    Public Sub Write(value() As Char) Implements IConsoleProvider.Write
        Console.Write(value)
    End Sub

    Public Sub Write(value As String) Implements IConsoleProvider.Write
        Console.Write(value)
    End Sub

    Public Sub Write(value As Object) Implements IConsoleProvider.Write
        Console.Write(value)
    End Sub

    Public Sub Write(value As Boolean) Implements IConsoleProvider.Write
        Console.Write(value)
    End Sub

    Public Sub Write(format As String, ParamArray arg() As Object) Implements IConsoleProvider.Write
        Console.Write(format, arg)
    End Sub

    Public Sub Write(value() As Char, index As Integer, count As Integer) Implements IConsoleProvider.Write
        Console.Write(value, index, count)
    End Sub

    Public Sub WriteLine() Implements IConsoleProvider.WriteLine
        Console.WriteLine()
    End Sub

    Public Sub WriteLine(value() As Char) Implements IConsoleProvider.WriteLine
        Console.WriteLine(value)
    End Sub

    Public Sub WriteLine(value As Object) Implements IConsoleProvider.WriteLine
        Console.WriteLine(value)
    End Sub

    Public Sub WriteLine(value As String) Implements IConsoleProvider.WriteLine
        Console.WriteLine(value)
    End Sub

    Public Sub WriteLine(format As String, ParamArray arg() As Object) Implements IConsoleProvider.WriteLine
        Console.WriteLine(format, arg)
    End Sub

    Public Sub WriteLine(value() As Char, index As Integer, count As Integer) Implements IConsoleProvider.WriteLine
        Console.WriteLine(value, index, count)
    End Sub

    Public Function Read() As Integer Implements IConsoleProvider.Read
        Return Console.Read
    End Function

    Public Function ReadLine() As String Implements IConsoleProvider.ReadLine
        Return Console.ReadLine
    End Function
End Class
