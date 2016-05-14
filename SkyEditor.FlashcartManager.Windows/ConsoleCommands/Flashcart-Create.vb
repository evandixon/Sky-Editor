Imports SkyEditor.Core.ConsoleCommands

Namespace ConsoleCommands
    Public Class Flashcart_Create
        Inherits ConsoleCommandAsync

        Public Overrides Async Function MainAsync(Arguments() As String) As Task
            If Not Arguments.Length < 2 Then
                Dim c = Await Flashcart.CreateFlashcart(Arguments(0), Arguments(1), CurrentPluginManager.CurrentIOProvider)
                Await c.Save(CurrentPluginManager.CurrentIOProvider)
            Else
                Console.WriteLine("Usage: Flashcart-Create <filename> <name>")
            End If
        End Function

        Public Overrides ReadOnly Property CommandName As String
            Get
                Return "Flashcart-Create"
            End Get
        End Property
    End Class
End Namespace

