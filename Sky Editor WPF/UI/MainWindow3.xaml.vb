Imports System.Collections.ObjectModel
Imports System.Collections.Specialized
Imports SkyEditor.Core
Imports SkyEditor.Core.Settings
Imports SkyEditor.Core.UI
Imports SkyEditor.UI.WPF

Public Class MainWindow3
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Public Property CurrentPluginManager As PluginManager

    Public Property OpenFiles As ObservableCollection(Of AvalonDockFileWrapper)

End Class
