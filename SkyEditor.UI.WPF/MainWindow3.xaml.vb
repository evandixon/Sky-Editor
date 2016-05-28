Imports System.Collections.ObjectModel
Imports System.Collections.Specialized
Imports System.Globalization
Imports System.Reflection
Imports System.Windows
Imports System.Windows.Input
Imports SkyEditor.Core
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Settings
Imports SkyEditor.Core.UI
Imports SkyEditor.UI.WPF
Imports Xceed.Wpf.AvalonDock.Layout.Serialization

Public Class MainWindow3
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.Title = String.Format(CultureInfo.InvariantCulture, My.Resources.Language.MainTitle, My.Resources.Language.VersionPrefix, Assembly.GetExecutingAssembly.GetName.Version.ToString)
        SaveLayoutCommand = New RelayCommand(AddressOf SaveLayout, AddressOf CanSaveLayout)
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

    Public Property SaveLayoutCommand As ICommand
    Public Property LoadLayoutCommand As ICommand

    Private Function CanSaveLayout() As Boolean
        Return True
    End Function

    Private Function CanLoadLayout() As Boolean
        Return CurrentPluginManager.CurrentIOProvider.FileExists("AvalonDock.Layout.config")
    End Function

    Private Function SaveLayout() As Task
        Dim layoutSerializer As New XmlLayoutSerializer(dockingManager)
        layoutSerializer.Serialize("AvalonDock.Layout.config")
        Return Task.CompletedTask
    End Function

    Private Function LoadLayout() As Task
        Dim layoutSerializer As New XmlLayoutSerializer(dockingManager)
        layoutSerializer.Deserialize("AvalonDock.Layout.config")

        'Todo: handle tool windows as described in AvalonDock's example project:
        '//Here I've implemented the LayoutSerializationCallback just to show
        '// a way to feed layout desarialization with content loaded at runtime
        '//Actually I could in this case let AvalonDock to attach the contents
        '//from current layout using the content ids
        '//LayoutSerializationCallback should anyway be handled to attach contents
        '//Not currently loaded
        'layoutSerializer.LayoutSerializationCallback += (s, e) =>
        '    {
        '        //if (e.Model.ContentId == FileStatsViewModel.ToolContentId)
        '        //    e.Content = Workspace.This.FileStats;
        '        //else if (!string.IsNullOrWhiteSpace(e.Model.ContentId) &&
        '        //    File.Exists(e.Model.ContentId))
        '        //    e.Content = Workspace.This.Open(e.Model.ContentId);
        '    };

        Return Task.CompletedTask
    End Function

End Class
