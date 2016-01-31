Imports System.Reflection
Imports System.Threading.Tasks

Public Module ConsoleModule
    Async Function ConsoleMain(Manager As PluginManager) As Task
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
            ElseIf Manager.GetConsoleCommands.Keys.Contains(cmd, StringComparer.InvariantCultureIgnoreCase) Then
                'Todo: split arg on spaces, while respecting quotation marks
                Dim args As New List(Of String)
                For Each item As Text.RegularExpressions.Match In paramRegex.Matches(arg)
                    args.Add(item.Value)
                Next
                Dim command = (From c In Manager.GetConsoleCommands Where String.Compare(c.Key, cmd, True, Globalization.CultureInfo.InvariantCulture) = 0 Select c.Value).FirstOrDefault '(cmd).MainAsync(args.ToArray)
                Try
                    Await command.MainAsync(args.ToArray)
                Catch ex As Exception
                    Console.WriteLine(ex.ToString)
                End Try
            Else
                Console.WriteLine(String.Format("""{0}"" is not a recognizable command.", cmd))
            End If
        End While
    End Function
End Module
