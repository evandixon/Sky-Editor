Namespace Language
    Public Class LanguageFile
        Inherits ObjectFile(Of LanguageItemList)
        Public Sub New()
            MyBase.New()
        End Sub
        Public Sub New(Filename As String)
            MyBase.New(Filename)
        End Sub
    End Class

End Namespace
