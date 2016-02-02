Imports System.Windows.Media
Imports CodeFiles
Imports ICSharpCode.AvalonEdit.CodeCompletion
Imports ICSharpCode.AvalonEdit.Document
Imports ICSharpCode.AvalonEdit.Editing

Public Class AutoCompleteData
    Implements ICompletionData

    Public ReadOnly Property Content As Object Implements ICompletionData.Content
        Get
            Return Me.Text
        End Get
    End Property

    Public ReadOnly Property Description As Object Implements ICompletionData.Description

    Public ReadOnly Property Image As ImageSource Implements ICompletionData.Image
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property Priority As Double Implements ICompletionData.Priority

    Public ReadOnly Property Text As String Implements ICompletionData.Text

    Public Sub Complete(textArea As TextArea, completionSegment As ISegment, insertionRequestEventArgs As EventArgs) Implements ICompletionData.Complete
        textArea.Document.Replace(completionSegment, Me.Text)
    End Sub

    Public Sub New(Info As FunctionDocumentation, SeparatorChars As List(Of Char))
        Dim part As New Text.StringBuilder
        For Each c In Info.FunctionName.ToCharArray
            If SeparatorChars.Contains(c) Then
                part = New Text.StringBuilder
            Else
                part.Append(c)
            End If
        Next
        Me.Text = part.ToString
        Me.Description = Info.FunctionDescription
    End Sub
End Class
