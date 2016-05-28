Namespace UI
    Public Interface IHiearchyItem
        Property Children As ObservableCollection(Of IHiearchyItem)
        ReadOnly Property Name As String
        ReadOnly Property Prefix As String
    End Interface
End Namespace