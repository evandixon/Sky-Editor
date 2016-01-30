Imports CodeFiles

Public Class DebugExtraData
    Inherits CodeExtraData

    Public Overrides Function GetAutoCompleteChars() As IEnumerable(Of Char)
        Return {} '{"."c, ":"c}
    End Function

    Public Overrides Function GetAutoCompleteData(CurrentWord As String) As IEnumerable(Of FunctionDocumentation)
        Dim out As New List(Of FunctionDocumentation)

        'Dim alpha As New FunctionDocumentation
        'alpha.FunctionName = "Alpha"
        'alpha.FunctionDescription = "A test function that does nothing but demonstrate auto-complete documentation."
        'alpha.ReturnTypeName = "Void"
        'alpha.Remarks = "Insert remarks here."
        'out.Add(alpha)

        'Dim beta As New FunctionDocumentation
        'beta.FunctionName = "Beta"
        'beta.FunctionDescription = "A test function that does nothing but demonstrate auto-complete documentation."
        'beta.ReturnTypeName = "Void"
        'beta.Remarks = "Insert remarks here."
        'out.Add(beta)

        'Dim others As New FunctionDocumentation
        'others.FunctionName = "Others"
        'others.FunctionDescription = "A test function that does nothing but demonstrate auto-complete documentation."
        'others.ReturnTypeName = "Void"
        'others.Remarks = "Insert remarks here."
        'out.Add(others)

        'Dim omega As New FunctionDocumentation
        'omega.FunctionName = "Omega"
        'omega.FunctionDescription = "A test function that does nothing but demonstrate auto-complete documentation."
        'omega.ReturnTypeName = "Void"
        'omega.Remarks = "Insert remarks here."
        'out.Add(omega)

        Return out
    End Function

    Public Overrides Function GetDocumentation(FunctionName As String) As FunctionDocumentation
        Dim test As New FunctionDocumentation
        test.FunctionName = "Test"
        test.FunctionDescription = "A test function that does nothing but demonstrate auto-complete documentation."
        test.ReturnTypeName = "Void"
        test.Remarks = "Insert remarks here."
        Return test
    End Function
End Class
