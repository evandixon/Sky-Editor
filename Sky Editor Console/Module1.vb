Imports SkyEditorBase

Module Module1

    Sub Main()
        Dim manager As PluginManager = PluginManager.GetInstance
        manager.LoadPlugins(New ConsoleCoreMod)

        PluginHelper.ShowConsole()
        ConsoleModule.ConsoleMain(manager).Wait()
    End Sub

End Module
