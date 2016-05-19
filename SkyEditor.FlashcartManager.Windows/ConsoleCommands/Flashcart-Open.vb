Imports SkyEditor.Core.ConsoleCommands

Namespace ConsoleCommands
    Public Class Flashcart_Open
        Inherits ConsoleCommandAsync

        Public Overrides Async Function MainAsync(Arguments() As String) As Task
            Const defaultFilename As String = "flashcart.skyfc"
            If Not Arguments.Length < 1 Then
                Dim filename As String = Nothing

                If CurrentPluginManager.CurrentIOProvider.FileExists(Arguments(0)) Then
                    filename = Arguments(0)
                ElseIf CurrentPluginManager.CurrentIOProvider.DirectoryExists(Arguments(0)) AndAlso CurrentPluginManager.CurrentIOProvider.FileExists(IO.Path.Combine(Arguments(0), defaultFilename)) Then
                    filename = IO.Path.Combine(Arguments(0), defaultFilename)
                Else
                    Console.WriteLine("Path doesn't exist.")
                    Exit Function
                End If

                Dim cart = Await Flashcart.OpenFlashcart(filename, CurrentPluginManager)
                Console.WriteLine("Name: " & cart.Name)
                Console.WriteLine("{0} libraries.", cart.Libraries.Count)

                Dim doExit As Boolean = False
                While Not doExit
                    Console.WriteLine("Please select a number:")
                    Console.WriteLine("0. Save & Exit")
                    Console.WriteLine("1. Create an NDS Library")
                    Console.WriteLine("2. List contents of all libraries")

                    Select Case Console.Read
                        Case AscW("0")
                            Console.WriteLine()
                            Await cart.Save(CurrentPluginManager.CurrentIOProvider)
                            doExit = True
                        Case AscW("1")
                            Console.WriteLine()
                            Console.WriteLine("Enter the name of the library:")
                            Dim name = Console.ReadLine
                            Console.WriteLine("Enter the relative path of the library (blank for root path):")
                            Dim relativePath = Console.ReadLine
                            Dim l As New NDSFlashcartLibrary(name, relativePath, cart.GetRootPath)
                            cart.Libraries.Add(l)
                            Console.WriteLine("Library added.")
                        Case AscW("2")
                            Console.WriteLine()
                            Console.WriteLine("Libraries:")
                            For Each library In cart.Libraries
                                Console.WriteLine("-{0} (~/{1})", library.Name, library.RelativePath)
                                For Each item In Await library.GetContents(CurrentPluginManager)
                                    Console.WriteLine("--{0}", item.ToString)
                                Next
                            Next
                    End Select
                End While
            Else
                Console.WriteLine("Usage: Flashcart-Open <filename|parentDirectory>")
            End If
        End Function

        Public Overrides ReadOnly Property CommandName As String
            Get
                Return "Flashcart-Open"
            End Get
        End Property
    End Class
End Namespace

