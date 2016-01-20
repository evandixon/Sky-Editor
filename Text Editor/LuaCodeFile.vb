Imports AurelienRibon.Ui.SyntaxHighlightBox

Public Class LuaCodeFile
    Inherits CodeFile
    Dim _highlighter As CustomHighlighter
    Public Overrides ReadOnly Property CodeHighlighter As IHighlighter
        Get
            Dim out = <Syntax name="VHDL">
                          <HighlightWordsRule name="Language Keywords">
                              <Words>
                              break do end else elseif function if local nil not or repeat return then until while
                          </Words>
                              <IgnoreCase>true</IgnoreCase>
                              <Foreground>#0000FF</Foreground>
                              <FontWeight>Bold</FontWeight>
                              <FontStyle>Normal</FontStyle>
                          </HighlightWordsRule>
                          <AdvancedHighlightRule name="Numbers">
                              <Expression>\b([0-9]+)\b</Expression>
                              <IgnoreCase>false</IgnoreCase>
                              <Foreground>#000000</Foreground>
                              <FontWeight>Normal</FontWeight>
                              <FontStyle>Normal</FontStyle>
                          </AdvancedHighlightRule>
                          <AdvancedHighlightRule name="Strings">
                              <Expression>(\".*?\")</Expression>
                              <IgnoreCase>false</IgnoreCase>
                              <Foreground>#A31515</Foreground>
                              <FontWeight>Normal</FontWeight>
                              <FontStyle>Normal</FontStyle>
                          </AdvancedHighlightRule>
                      </Syntax>
            Return New CustomHighlighter(out)
        End Get
    End Property
    Public Sub New()
        MyBase.New
    End Sub
End Class
