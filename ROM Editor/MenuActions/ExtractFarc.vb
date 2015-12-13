Imports SkyEditorBase

Namespace MenuActions
    Public Class ExtractFarc
        Inherits MenuAction
        Public Overrides Function SupportedTypes() As IEnumerable(Of Type)
            Return {GetType(FileFormats.Farc)}
        End Function
        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            For Each item As FileFormats.Farc In Targets
                item.ExtractContents()
            Next
            Return Task.CompletedTask
        End Function
        Public Sub New()
            MyBase.New("Testing/ExtractFarc")
        End Sub
    End Class
End Namespace

