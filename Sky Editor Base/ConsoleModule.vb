Imports System.Reflection

Public Module ConsoleModule
    Sub ConsoleMain(Manager As PluginManager)
        While True
            Dim paramRegex As New Text.RegularExpressions.Regex("(\"".*?\"")|(\S+)", Text.RegularExpressions.RegexOptions.Compiled)
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
                Dim commands As New List(Of String)(Manager.GetConsoleCommands.Keys)
                commands.Sort()
                For Each item In commands
                    Console.WriteLine(item)
                Next
            ElseIf Manager.GetConsoleCommands.Keys.Contains(cmd) Then
                'Todo: split arg on spaces, while respecting quotation marks
                Dim args As New List(Of String)
                For Each item As Text.RegularExpressions.Match In paramRegex.Matches(arg)
                    args.Add(item.Value)
                Next
                Dim t = Manager.GetConsoleCommands(cmd).MainAsync(args.ToArray)
                t.Wait()
            Else
                Console.WriteLine(String.Format("""{0}"" is not a recognizable command.", cmd))
            End If
        End While
    End Sub
End Module
