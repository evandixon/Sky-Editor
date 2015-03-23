Public Interface iMainWindow
    Sub AddMenuItem(Menu As MenuItem)
    Function GetMenuItems() As ItemCollection
    Sub RemoveMenuItem(Menu As MenuItem)
    Sub AddTabItem(SaveName As String, Tab As TabItem)
    Sub ClearTabItems()
    Function GetTabItems() As ItemCollection

    Event OnKeyPress(sender As Object, e As KeyEventArgs)
End Interface
