Imports SkyEditor.Core.ConsoleCommands
Imports SkyEditorBase

Namespace ConsoleCommands
    Public Class ImportFunctionList
        Inherits ConsoleCommand

        Public Overrides Sub Main(Arguments() As String)
            Dim currentPrefix As String = ""
            Dim file As New CodeFiles.CodeExtraDataFile
            For Each line In IO.File.ReadAllLines(Arguments(0))
                'Ignore blank lines
                If Not String.IsNullOrEmpty(line) Then
                    'If the line is ========================================================== or equivalent, we're in another namespace
                    If String.IsNullOrEmpty(line.Trim("=")) Then
                        currentPrefix = ""
                    ElseIf line.Trim.StartsWith("MenuLuaFunc_") OrElse line.Trim.StartsWith("ScriptFunc_") Then
                        'Set the current prefix
                        currentPrefix = line.Trim.Replace("MenuLuaFunc_", "").Replace("ScriptFunc_", "")
                    Else
                        If Not String.IsNullOrEmpty(currentPrefix) Then
                            Dim d As New CodeFiles.FunctionDocumentation
                            d.FunctionName = currentPrefix & ":" & FullTrim(line)
                            file.Database.Add(d)
                        End If
                    End If
                End If
            Next
            file.AutoCompleteChars.Clear()
            file.AutoCompleteChars.Add(":")
            file.Save(PluginHelper.GetResourceName("Code/psmdLuaInfo-English.fdd"), PluginManager.GetInstance.CurrentIOProvider)
        End Sub
        Private Function FullTrim(Input As String) As String
            Dim current As String = Input
            While Not current = SimpleTrim(current)
                current = SimpleTrim(current)
            End While
            Return current
        End Function
        Private Function SimpleTrim(Input As String) As String
            Return Input.Trim.Trim("""!@#$%^&*()_+.,;<>=".ToCharArray).TrimStart("1234567890".ToCharArray)
        End Function
    End Class
End Namespace

