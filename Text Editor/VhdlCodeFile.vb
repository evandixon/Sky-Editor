Imports AurelienRibon.Ui.SyntaxHighlightBox

Public Class VhdlCodeFile
    Inherits CodeFile

    Public Overrides ReadOnly Property CodeHighlighter As IHighlighter
        Get
            Return HighlighterManager.Instance.Highlighters("VHDL")
        End Get
    End Property
End Class
