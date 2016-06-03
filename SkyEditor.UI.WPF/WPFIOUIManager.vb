Imports SkyEditor.Core
Imports SkyEditor.UI.WPF.AvalonHelpers

Public Class WPFIOUIManager
    Inherits IOUIManager

    Public Sub New(manager As PluginManager)
        MyBase.New(manager)
        AvalonDockLayout = New AvalonDockLayoutViewModel(manager)
    End Sub

    Public Property AvalonDockLayout As AvalonDockLayoutViewModel
End Class
