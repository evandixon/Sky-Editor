Imports AurelienRibon.Ui.SyntaxHighlightBox

Public Class LuaCodeFile
    Inherits CodeFile
    Dim _highlighter As CustomHighlighter
    Public Overrides ReadOnly Property CodeHighlighter As IHighlighter
        Get
            Return _highlighter
        End Get
    End Property
    Public Sub New()
        MyBase.New
        _highlighter = New CustomHighlighter
        'TODO: add rules for lua syntax
    End Sub
End Class
