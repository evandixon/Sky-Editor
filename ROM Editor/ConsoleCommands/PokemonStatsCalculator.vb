Imports SkyEditorBase

Namespace ConsoleCommands
    Public Class PokemonStatsCalculator
        Inherits ConsoleCommandAsync

        Public Overrides Function MainAsync(Arguments() As String) As Task
            If Arguments.Length > 0 Then
                If IO.Directory.Exists(Arguments(0)) Then
                    Dim data As New PsmdDir
                    data.OpenFile(Arguments(0))

                    Dim doExit As Boolean = False
                    While Not doExit
                        Dim command As String = Console.ReadLine
                        Dim parts As String() = command.Split(" ".ToCharArray, 2)
                        Select Case parts(0).ToLower
                            Case "stat"
                                Dim params = parts(1).Split(" ")
                                If params.Length < 2 Then
                                    Console.WriteLine("Usage: stat <id> <level>")
                                Else
                                    Dim pkm = (From d In data.PokemonInfo.Entries Where d.EntryNumber = CInt(params(0))).FirstOrDefault
                                    If pkm IsNot Nothing Then
                                        Dim xpTable = data.PokemonExpTable.Entries(pkm.ExpTableNumber)
                                        Dim hp As Integer = pkm.BaseHP
                                        Dim atk As Integer = pkm.BaseAttack
                                        Dim def As Integer = pkm.BaseDefense
                                        Dim spd As Integer = pkm.BaseSpeed
                                        Dim spAtk As Integer = pkm.BaseSpAttack
                                        Dim spDef As Integer = pkm.BaseSpDefense
                                        Console.WriteLine("---------------")
                                        Console.WriteLine("Pokemon: " & data.PokemonNames(pkm.EntryNumber))
                                        Console.WriteLine("Experience table number: " & pkm.ExpTableNumber)
                                        Console.WriteLine("Level: 0")
                                        Console.WriteLine("HP: " & hp)
                                        Console.WriteLine("Attack: " & atk)
                                        Console.WriteLine("Defense: " & def)
                                        Console.WriteLine("Sp. Attack: " & spAtk)
                                        Console.WriteLine("Sp. Defense: " & spDef)
                                        Console.WriteLine("Speed: " & spd)
                                        Console.WriteLine("---------------")
                                        For count = 0 To CInt(params(1)) - 1
                                            Console.WriteLine("Level " & count + 1)
                                            hp += xpTable(count).AddedHP
                                            atk += xpTable(count).AddedAttack
                                            def += xpTable(count).AddedDefense
                                            spd += xpTable(count).AddedSpeed
                                            spAtk += xpTable(count).AddedSpAttack
                                            spDef += xpTable(count).AddedSpDefense
                                            Console.WriteLine("+HP    = " & xpTable(count).AddedHP)
                                            Console.WriteLine("+ATK   = " & xpTable(count).AddedAttack)
                                            Console.WriteLine("+DEF   = " & xpTable(count).AddedDefense)
                                            Console.WriteLine("+spATK = " & xpTable(count).AddedSpeed)
                                            Console.WriteLine("+spDEF = " & xpTable(count).AddedSpAttack)
                                            Console.WriteLine("+SPEED = " & xpTable(count).AddedSpDefense)
                                            Console.WriteLine("Experience: " & xpTable(count).Exp)
                                        Next
                                        Console.WriteLine("---------------")
                                        Console.WriteLine("Pokemon: " & data.PokemonNames(pkm.EntryNumber))
                                        Console.WriteLine("Level: " & CInt(params(1)))
                                        Console.WriteLine("Experience: " & xpTable(CInt(params(1) - 1)).Exp)
                                        Console.WriteLine("HP: " & hp)
                                        Console.WriteLine("Attack: " & atk)
                                        Console.WriteLine("Defense: " & def)
                                        Console.WriteLine("Sp. Attack: " & spAtk)
                                        Console.WriteLine("Sp. Defense: " & spDef)
                                        Console.WriteLine("Speed: " & spd)
                                        Console.WriteLine("---------------")
                                    Else
                                        Console.WriteLine("Can't find Pokemon")
                                    End If
                                End If
                            Case "pkm"
                                Dim result As String = "Pokemon ID not found"
                                For count = 0 To data.PokemonNames.Count - 1
                                    If data.PokemonNames(count).ToLower = parts(1).ToLower Then
                                        result = $"{parts(1)}'s ID is {count} (first occurrance)"
                                        Exit For
                                    End If
                                Next
                                Console.WriteLine(result)
                            Case "exit"
                                doExit = True
                            Case Else
                                Console.WriteLine("Unknown command")
                        End Select
                    End While
                Else
                    Console.WriteLine("Directory doesn't exist")
                End If
            Else
                Console.WriteLine("Usage: PokemonStatsCalculator <directory>")
            End If
            Return Task.CompletedTask
        End Function
    End Class
End Namespace

