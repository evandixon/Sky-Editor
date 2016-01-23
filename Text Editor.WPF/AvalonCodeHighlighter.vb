Imports System.Text.RegularExpressions
Imports System.Windows
Imports System.Windows.Media
Imports CodeFiles
Imports ICSharpCode.AvalonEdit.Document
Imports ICSharpCode.AvalonEdit.Highlighting

Public Class AvalonCodeHighlighter
    Implements ICSharpCode.AvalonEdit.Highlighting.IHighlightingDefinition


    Public Property MainRuleSet As HighlightingRuleSet Implements IHighlightingDefinition.MainRuleSet

    Public ReadOnly Property Name As String Implements IHighlightingDefinition.Name

    Public ReadOnly Property NamedHighlightingColors As IEnumerable(Of HighlightingColor) Implements IHighlightingDefinition.NamedHighlightingColors

    Public ReadOnly Property Properties As IDictionary(Of String, String) Implements IHighlightingDefinition.Properties

    Public Function GetNamedColor(name As String) As HighlightingColor Implements IHighlightingDefinition.GetNamedColor
        Return (From c In NamedHighlightingColors Where c.Name = name Select c).FirstOrDefault
    End Function

    Public Function GetNamedRuleSet(name As String) As HighlightingRuleSet Implements IHighlightingDefinition.GetNamedRuleSet
        Return Nothing
    End Function

    ''' <summary>
    ''' Adds the given HighlightDefinition as a RuleSet.
    ''' </summary>
    ''' <param name="RuleSetName">Name of the ruleset.  Pass in Nothing if this is the default ruleset.</param>
    ''' <param name="Definition"></param>
    Public Sub AddRuleSet(RuleSetName As String, Definition As HighlightDefinition)
        'Named colors
        Dim namedColors = New List(Of HighlightingColor)
        For Each item In Definition.NamedHighlightColors
            Dim c As New HighlightingColor()
            c.Name = item.Name
            Dim b As New Windows.Media.Color
            c.Foreground = New SimpleHighlightingBrush(ColorConverter.ConvertFromString(item.Foreground))
            c.Background = New SimpleHighlightingBrush(ColorConverter.ConvertFromString(item.Background))
            c.FontWeight = New FontWeightConverter().ConvertFrom(item.FontWeight)
            c.FontStyle = New FontStyleConverter().ConvertFrom(item.FontStyle)
            c.Underline = item.Underline
            namedColors.Add(c)
        Next
        DirectCast(NamedHighlightingColors, List(Of HighlightingColor)).AddRange(namedColors)

        'Rulesets
        'Dim ruleset = New HighlightingRuleSet
        For Each item In Definition.Rules
            Dim r As New HighlightingRule
            r.Color = GetNamedColor(item.ColorName)
            If item.CaseSensitive Then
                r.Regex = New Regex(item.Regex, RegexOptions.Compiled)
            Else
                r.Regex = New Regex(item.Regex, RegexOptions.Compiled Or RegexOptions.IgnoreCase)
            End If
            MainRuleSet.Rules.Add(r)
        Next
        'If RuleSetName Is Nothing Then
        '    MainRuleSet = ruleset
        'Else

        'End If

        'Todo: properties
    End Sub

    Public Sub New(Definition As HighlightDefinition)
        Name = Definition.Name
        NamedHighlightingColors = New List(Of HighlightingColor)
        Properties = New Dictionary(Of String, String)
        MainRuleSet = New HighlightingRuleSet
        AddRuleSet(Nothing, Definition)
    End Sub
End Class
