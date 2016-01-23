Public MustInherit Class CodeFile
    Inherits TextFile
    Public MustOverride Function GetCodeHighlightRules() As HighlightDefinition
End Class
