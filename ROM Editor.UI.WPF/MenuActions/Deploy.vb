Imports SkyEditorBase

Namespace MenuActions
    Public Class Deploy
        Inherits MenuAction

        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            Throw New NotImplementedException()
        End Function

        Public Sub New()
            MyBase.New("Testing/Deploy")
        End Sub
    End Class

End Namespace
