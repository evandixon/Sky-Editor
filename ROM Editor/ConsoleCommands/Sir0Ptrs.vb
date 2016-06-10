Imports SkyEditor.Core.ConsoleCommands

Namespace ConsoleCommands
    Public Class Sir0Ptrs
        Inherits ConsoleCommandAsync

        Public Overrides Async Function MainAsync(Arguments() As String) As Task
            If Arguments.Length > 0 Then
                If IO.File.Exists(Arguments(0)) Then
                    Dim pointers As New Dictionary(Of UInt32, UInt32)
                    Using f As New FileFormats.Sir0
                        f.IsReadOnly = True
                        Await f.OpenFile(Arguments(0), CurrentPluginManager.CurrentIOProvider)
                        Dim offset As UInt32 = 0
                        For Each item In f.RelativePointers
                            offset += item
                            pointers.Add(offset, f.UInt32(offset))
                            Console.WriteLine($"{Conversion.Hex(offset)}: {Conversion.Hex(f.UInt32(offset))}")
                        Next


                        'Dim q = From p In pointers Select $"{Conversion.Hex(p.Key)}: {Conversion.Hex(p.Value)}"

                        Dim s As New Text.StringBuilder
                        Dim i As Integer = 0
                        While i < f.Length - 4
                            If pointers.ContainsKey(i) Then
                                s.AppendLine($"{Conversion.Hex(i)}: [Pointer] {Conversion.Hex(pointers(i))}")
                                i += 4
                            ElseIf pointers.ContainsValue(i) Then
                                Dim ref = (From p In pointers Where p.Value = i Select p.Key).First
                                s.AppendLine($"{Conversion.Hex(i)}: {Conversion.Hex(f.UInt32(i))} [Referenced at {Conversion.Hex(ref)}]")
                                i += 4
                            Else
                                s.AppendLine($"{Conversion.Hex(i)}: {Conversion.Hex(f.UInt32(i))}")
                                i += 4
                            End If
                        End While
                        IO.File.WriteAllText(Arguments(0) & ".txt", s.ToString)
                    End Using
                Else
                    Console.WriteLine("File doesn't exist")
                End If
            Else
                Console.WriteLine("Usage: sir0ptrs <filename>")
            End If
        End Function
    End Class

End Namespace
