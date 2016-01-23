Imports CodeFiles

Public Class LuaCodeFile
    Inherits CodeFile

    Public Overrides Function GetCodeHighlightRules() As HighlightDefinition
        Dim out As New HighlightDefinition

        out.Name = "Lua Rules"
        out.NamedHighlightColors.Add(New HighlightingStyle With {.Name = "String", .Foreground = "#A31515", .Background = "Transparent", .FontStyle = "Normal", .FontWeight = "Normal", .Underline = False})
        out.NamedHighlightColors.Add(New HighlightingStyle With {.Name = "Number", .Foreground = "#111111", .Background = "Transparent", .FontStyle = "Normal", .FontWeight = "Normal", .Underline = False})
        out.NamedHighlightColors.Add(New HighlightingStyle With {.Name = "Keyword", .Foreground = "#0000FF", .Background = "Transparent", .FontStyle = "Normal", .FontWeight = "Bold", .Underline = False})

        out.Rules.Add(New HighlightRule With {.CaseSensitive = False, .ColorName = "String", .Regex = "(\"".*?\"")"})
        out.Rules.Add(New HighlightRule With {.CaseSensitive = False, .ColorName = "Number", .Regex = "\b([0-9]+)\b"})
        For Each item In "and break do else elseif end false for function if in local nil not or repeat return then true until while".Split(" ")
            out.Rules.Add(New HighlightRule With {.CaseSensitive = False, .ColorName = "Keyword", .Regex = "\b" & item & "\b"})
        Next

        Return out
    End Function
End Class
