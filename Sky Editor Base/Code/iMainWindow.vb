Public Interface iMainWindow
    Sub AddMenuItem(Menu As MenuItem)
    Sub AddTabItem(Tab As TabItem)
    Sub ClearTabItems()
    Function GetTabItems() As ItemCollection

    Event OnKeyPress(sender As Object, e As KeyEventArgs)
End Interface
