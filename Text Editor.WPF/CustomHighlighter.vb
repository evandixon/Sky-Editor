Imports System.Text.RegularExpressions
Imports System.Windows
Imports System.Windows.Media
Imports AurelienRibon.Ui.SyntaxHighlightBox

''' <remarks>
''' A slight modification to AurelienRibon.Ui.SyntaxHighlightBox.HighlighterManager.XmlHighlighter, reimplemented because it was set to Private in the library.
''' On 12-12-2015 4:47 CST, Copied from http://syntaxhighlightbox.codeplex.com/SourceControl/latest#AurelienRibon.Ui.SyntaxHighlightBox/src/HighlighterManager.cs
''' Credit to ArelienRibon for writing the original class, and the library.
''' </remarks>
Public Class CustomHighlighter
    Implements IHighlighter
    Public Property wordsRules As List(Of HighlightWordsRule)
    Public Property lineRules As List(Of HighlightLineRule)
    Public Property regexRules As List(Of AdvancedHighlightRule)

    Public Sub New()
        wordsRules = New List(Of HighlightWordsRule)()
        lineRules = New List(Of HighlightLineRule)()
        regexRules = New List(Of AdvancedHighlightRule)()
    End Sub

    Public Sub New(root As XElement)
        Me.New
        For Each elem As XElement In root.Elements()
            Select Case elem.Name.ToString()
                Case "HighlightWordsRule"
                    wordsRules.Add(New HighlightWordsRule(elem))
                    Exit Select
                Case "HighlightLineRule"
                    lineRules.Add(New HighlightLineRule(elem))
                    Exit Select
                Case "AdvancedHighlightRule"
                    regexRules.Add(New AdvancedHighlightRule(elem))
                    Exit Select
            End Select
        Next
    End Sub

    Public Function Highlight(text As FormattedText, previousBlockCode As Integer) As Integer Implements IHighlighter.Highlight
        '
        ' WORDS RULES
        '
        Dim wordsRgx As New Regex("[a-zA-Z_][a-zA-Z0-9_]*")
        For Each m As Match In wordsRgx.Matches(text.Text)
            For Each rule As HighlightWordsRule In wordsRules
                For Each word As String In rule.Words
                    If rule.Options.IgnoreCase Then
                        If m.Value.Equals(word, StringComparison.InvariantCultureIgnoreCase) Then
                            text.SetForegroundBrush(rule.Options.Foreground, m.Index, m.Length)
                            text.SetFontWeight(rule.Options.FontWeight, m.Index, m.Length)
                            text.SetFontStyle(rule.Options.FontStyle, m.Index, m.Length)
                        End If
                    Else
                        If m.Value = word Then
                            text.SetForegroundBrush(rule.Options.Foreground, m.Index, m.Length)
                            text.SetFontWeight(rule.Options.FontWeight, m.Index, m.Length)
                            text.SetFontStyle(rule.Options.FontStyle, m.Index, m.Length)
                        End If
                    End If
                Next
            Next
        Next

        '
        ' REGEX RULES
        '
        For Each rule As AdvancedHighlightRule In regexRules
            Dim regexRgx As New Regex(rule.Expression)
            For Each m As Match In regexRgx.Matches(text.Text)
                text.SetForegroundBrush(rule.Options.Foreground, m.Index, m.Length)
                text.SetFontWeight(rule.Options.FontWeight, m.Index, m.Length)
                text.SetFontStyle(rule.Options.FontStyle, m.Index, m.Length)
            Next
        Next

        '
        ' LINES RULES
        '
        For Each rule As HighlightLineRule In lineRules
            Dim lineRgx As New Regex(Regex.Escape(rule.LineStart) + ".*")
            For Each m As Match In lineRgx.Matches(text.Text)
                text.SetForegroundBrush(rule.Options.Foreground, m.Index, m.Length)
                text.SetFontWeight(rule.Options.FontWeight, m.Index, m.Length)
                text.SetFontStyle(rule.Options.FontStyle, m.Index, m.Length)
            Next
        Next

        Return -1
    End Function

    ''' <summary>
    ''' A set of words and their RuleOptions.
    ''' </summary>
    Public Class HighlightWordsRule
        Public Property Words() As List(Of String)
            Get
                Return m_Words
            End Get
            Private Set
                m_Words = Value
            End Set
        End Property
        Private m_Words As List(Of String)
        Public Property Options() As RuleOptions
            Get
                Return m_Options
            End Get
            Private Set
                m_Options = Value
            End Set
        End Property
        Private m_Options As RuleOptions

        Public Sub New(rule As XElement)
            Words = New List(Of String)()
            Options = New RuleOptions(rule)

            Dim wordsStr As String = rule.Element("Words").Value
            Dim words__1 As String() = Regex.Split(wordsStr, "\s+")

            For Each word As String In words__1
                If Not String.IsNullOrWhiteSpace(word) Then
                    Words.Add(word.Trim())
                End If
            Next
        End Sub
    End Class

    ''' <summary>
    ''' A line start definition and its RuleOptions.
    ''' </summary>
    Public Class HighlightLineRule
        Public Property LineStart() As String
            Get
                Return m_LineStart
            End Get
            Private Set
                m_LineStart = Value
            End Set
        End Property
        Private m_LineStart As String
        Public Property Options() As RuleOptions
            Get
                Return m_Options
            End Get
            Private Set
                m_Options = Value
            End Set
        End Property
        Private m_Options As RuleOptions

        Public Sub New(rule As XElement)
            LineStart = rule.Element("LineStart").Value.Trim()
            Options = New RuleOptions(rule)
        End Sub
    End Class

    ''' <summary>
    ''' A regex and its RuleOptions.
    ''' </summary>
    Public Class AdvancedHighlightRule
        Public Property Expression() As String
            Get
                Return m_Expression
            End Get
            Private Set
                m_Expression = Value
            End Set
        End Property
        Private m_Expression As String
        Public Property Options() As RuleOptions
            Get
                Return m_Options
            End Get
            Private Set
                m_Options = Value
            End Set
        End Property
        Private m_Options As RuleOptions

        Public Sub New(rule As XElement)
            Expression = rule.Element("Expression").Value.Trim()
            Options = New RuleOptions(rule)
        End Sub
    End Class

    ''' <summary>
    ''' A set of options liked to each rule.
    ''' </summary>
    Public Class RuleOptions
        Public Property IgnoreCase() As Boolean
            Get
                Return m_IgnoreCase
            End Get
            Private Set
                m_IgnoreCase = Value
            End Set
        End Property
        Private m_IgnoreCase As Boolean
        Public Property Foreground() As Brush
            Get
                Return m_Foreground
            End Get
            Private Set
                m_Foreground = Value
            End Set
        End Property
        Private m_Foreground As Brush
        Public Property FontWeight() As FontWeight
            Get
                Return m_FontWeight
            End Get
            Private Set
                m_FontWeight = Value
            End Set
        End Property
        Private m_FontWeight As FontWeight
        Public Property FontStyle() As FontStyle
            Get
                Return m_FontStyle
            End Get
            Private Set
                m_FontStyle = Value
            End Set
        End Property
        Private m_FontStyle As FontStyle

        Public Sub New(rule As XElement)
            Dim ignoreCaseStr As String = rule.Element("IgnoreCase").Value.Trim()
            Dim foregroundStr As String = rule.Element("Foreground").Value.Trim()
            Dim fontWeightStr As String = rule.Element("FontWeight").Value.Trim()
            Dim fontStyleStr As String = rule.Element("FontStyle").Value.Trim()

            IgnoreCase = Boolean.Parse(ignoreCaseStr)
            Foreground = DirectCast(New BrushConverter().ConvertFrom(foregroundStr), Brush)
            FontWeight = DirectCast(New FontWeightConverter().ConvertFrom(fontWeightStr), FontWeight)
            FontStyle = DirectCast(New FontStyleConverter().ConvertFrom(fontStyleStr), FontStyle)
        End Sub
    End Class
End Class
'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================