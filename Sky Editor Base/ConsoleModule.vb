Imports System.Reflection

Module ConsoleModule
    Sub ConsoleMain()
        Console.WriteLine(String.Format(PluginHelper.GetLanguageItem("Sky Editor {0}"), Assembly.GetCallingAssembly.GetName.Version.ToString))
        Console.WriteLine()
        Using manager = PluginManager.GetInstance
            ConsoleMain(manager)
        End Using
    End Sub

    Sub ConsoleMain(Manager As PluginManager)
        While True
            Dim line = Console.ReadLine()
            Dim cmdParts = line.Split(" ".ToCharArray, 2)
            Dim cmd = cmdParts(0).ToLower
            Dim arg = ""
            If cmdParts.Length > 1 Then
                arg = cmdParts(1)
            End If
            If cmd = "exit" Then
                Exit While
            ElseIf cmd = "help" Then
                Console.WriteLine(PluginHelper.GetLanguageItem("The following commands are available:"))
                Dim commands As New List(Of String)(Manager.ConsoleCommandList.Keys)
                commands.Sort()
                For Each item In commands
                    Console.WriteLine(item)
                Next
            ElseIf Manager.ConsoleCommandList.Keys.Contains(cmd) Then
                Manager.ConsoleCommandList(cmd).Invoke(Manager, arg)
            ElseIf Manager.ConsoleCommandAsyncList.Keys.Contains(cmd)
                Dim t = Manager.ConsoleCommandAsyncList(cmd).Invoke(Manager, arg)
                t.Wait()
            Else
                Console.WriteLine(String.Format("""{0}"" is not a recognizable command.", cmd))
            End If
        End While
    End Sub
End Module
