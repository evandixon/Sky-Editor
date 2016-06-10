Imports System.IO
Imports SkyEditor.Core
Imports SkyEditor.Core.ConsoleCommands

Module Module1

    Sub Main()
        Try
            Dim manager As New PluginManager
            manager.LoadCore(New ConsoleCoreMod)

            Console.WriteLine("Sky Editor Console has successfully loaded.")
            ConsoleHelper.RunConsole(manager).Wait()
        Catch ex As Exception
            Console.WriteLine(ex.ToString)
            File.WriteAllText("error.txt", ex.ToString)
            Console.WriteLine("Error details have been written to error.txt")
            Console.WriteLine("Press enter to exit...")
            Console.ReadLine()
        End Try
    End Sub

End Module
