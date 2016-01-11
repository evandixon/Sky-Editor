Namespace Language
    Public Class LanguageItemList
        Implements IEnumerable(Of LanguageItem)
        Public Property Items As List(Of LanguageItem)
        Public Property Revision As Integer
        Public Sub New()
            MyBase.New
            Items = New List(Of LanguageItem)
            Revision = 0
        End Sub

        Public Function GetEnumerator() As IEnumerator(Of LanguageItem) Implements IEnumerable(Of LanguageItem).GetEnumerator
            Return Items.GetEnumerator
        End Function

        Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return Items.GetEnumerator
        End Function
    End Class
End Namespace

