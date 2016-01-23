Public MustInherit Class CodeExtraData

    ''' <summary>
    ''' Returns an IEnumerable of Char that can trigger the auto complete popup.
    ''' </summary>
    ''' <returns></returns>
    Public MustOverride Function GetAutoCompleteChars() As IEnumerable(Of Char)

    ''' <summary>
    ''' Returns AutoComplete data for the currently typed word.
    ''' </summary>
    ''' <param name="CurrentWord">Word that is currently typed.</param>
    ''' <returns></returns>
    ''' <remarks>For example, in a .Net CodeExtraData, GetAutoCompleteData("Console.Wr") would return info for "Console.Writeline" and "Console.Write".</remarks>
    Public MustOverride Function GetAutoCompleteData(CurrentWord As String) As IEnumerable(Of FunctionDocumentation)

    ''' <summary>
    ''' Returns documentation for the given function.
    ''' </summary>
    ''' <param name="FunctionName">Name of the function for which to display documentation.</param>
    ''' <returns></returns>
    ''' <remarks>For example, if the CodeExtraData is for .Net, GetDocumentation("System.Console.Writeline") would return </remarks>
    Public MustOverride Function GetDocumentation(FunctionName As String) As FunctionDocumentation

    ''' <summary>
    ''' Returns additional highlight rules for the given code file.
    ''' </summary>
    ''' <returns></returns>
    Public Overridable Function AdditionalHighlightRules() As HighlightDefinition
        Return Nothing
    End Function
End Class
