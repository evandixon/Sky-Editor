Imports System.Collections.ObjectModel
Imports System.Collections.Specialized
Imports System.Windows
Imports SkyEditor.Core
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Settings
Imports SkyEditor.Core.UI
Imports SkyEditor.UI.WPF

Public Class MainWindow3
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub OnIOUIManagerFileClosing(sender As Object, e As FileClosingEventArgs)
        If e.File.IsFileModified Then
            e.Cancel = Not (MessageBox.Show(My.Resources.Language.DocumentCloseConfirmation, My.Resources.Language.MainTitle, MessageBoxButton.YesNo) = MessageBoxResult.Yes)
        End If
    End Sub

    Public Property CurrentPluginManager As PluginManager
        Get
            Return _currentPluginManager
        End Get
        Set(value As PluginManager)
            If _currentPluginManager?.CurrentIOUIManager IsNot Nothing Then
                RemoveHandler _currentPluginManager.CurrentIOUIManager.FileClosing, AddressOf OnIOUIManagerFileClosing
            End If

            _currentPluginManager = value

            If _currentPluginManager?.CurrentIOUIManager IsNot Nothing Then
                AddHandler _currentPluginManager.CurrentIOUIManager.FileClosing, AddressOf OnIOUIManagerFileClosing
            End If
        End Set
    End Property
    Dim _currentPluginManager As PluginManager

    Public Property OpenFiles As ObservableCollection(Of AvalonDockFileWrapper)

End Class
