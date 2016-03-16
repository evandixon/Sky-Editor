Imports SkyEditorBase

Module Module1

    Sub Main()
        Try
            Dim manager As PluginManager = PluginManager.GetInstance
            manager.LoadPlugins(New ConsoleCoreMod)

            PluginHelper.ShowConsole()
            Console.WriteLine("Sky Editor Console has successfully loaded.")
            ConsoleModule.ConsoleMain(manager).Wait()
        Catch ex As Exception
            Console.WriteLine(ex.ToString)
            IO.File.WriteAllText("error.txt", ex.ToString)
            Console.WriteLine("Error details have been written to error.txt")
            Console.WriteLine("Press enter to exit...")
            Console.ReadLine()
        End Try
    End Sub

End Module
