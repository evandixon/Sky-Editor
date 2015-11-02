Namespace Language
    Public Class LanguageFile
        Inherits ObjectFile(Of List(Of LanguageItem))
        Public Sub New()
            MyBase.New()
        End Sub
        Public Sub New(Filename As String)
            MyBase.New(Filename)
        End Sub
    End Class

End Namespace
