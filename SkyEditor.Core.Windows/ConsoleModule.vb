Imports System.Reflection
Imports System.Threading.Tasks

Public Module ConsoleModule
    Async Function ConsoleMain(Manager As PluginManager) As Task
        Dim AllCommands As New Dictionary(Of String, SkyEditor.Core.ConsoleCommands.ConsoleCommandAsync)
        For Each item In Manager.GetRegisteredObjects(Of SkyEditor.Core.ConsoleCommands.ConsoleCommandAsync)
            item.CurrentPluginManager = Manager
            AllCommands.Add(item.CommandName, item)
        Next
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
                Console.WriteLine(My.Resources.Language.ConsoleAvailableCommands)
                Dim commands As New List(Of String)(AllCommands.Keys)
                commands.Sort()
                For Each item In commands
                    Console.WriteLine(item)
                Next
            ElseIf AllCommands.Keys.Contains(cmd, StringComparer.InvariantCultureIgnoreCase) Then
                'Todo: split arg on spaces, while respecting quotation marks
                Dim args As New List(Of String)
                For Each item As Text.RegularExpressions.Match In paramRegex.Matches(arg)
                    args.Add(item.Value.Trim(""""))
                Next
                Dim command = (From c In AllCommands Where String.Compare(c.Key, cmd, True, Globalization.CultureInfo.InvariantCulture) = 0 Select c.Value).FirstOrDefault '(cmd).MainAsync(args.ToArray)
                Try
                    Await command.MainAsync(args.ToArray)
                Catch ex As Exception
                    Console.WriteLine(ex.ToString)
                End Try
            Else
                Console.WriteLine(String.Format(My.Resources.Language.ConsoleUnknownCommand, cmd))
            End If
        End While
    End Function
End Module
