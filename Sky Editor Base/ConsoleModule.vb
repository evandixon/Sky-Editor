Imports System.Reflection

Module ConsoleModule
    Sub ConsoleMain()
        Console.WriteLine(String.Format(PluginHelper.GetLanguageItem("Sky Editor {0}"), Assembly.GetCallingAssembly.GetName.Version.ToString))
        Console.WriteLine()
        Using manager = PluginManager.GetInstance
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
                    Dim commands As New List(Of String)(manager.ConsoleCommandList.Keys)
                    commands.Sort()
                    For Each item In commands
                        Console.WriteLine(item)
                    Next
                ElseIf manager.ConsoleCommandList.Keys.Contains(cmd) Then
                    manager.ConsoleCommandList(cmd).Invoke(manager, arg)
                Else
                    Console.WriteLine(String.Format("""{0}"" is not a recognizable command.", cmd))
                End If
            End While
        End Using
    End Sub
End Module
