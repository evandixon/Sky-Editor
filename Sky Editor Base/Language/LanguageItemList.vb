Namespace Language
    Public Class LanguageItemList

        Public Property Items As List(Of LanguageItem)
        Public Property Revision As Integer
        Public Sub New()
            MyBase.New
            Items = New List(Of LanguageItem)
            Revision = 0
        End Sub
    End Class
End Namespace

