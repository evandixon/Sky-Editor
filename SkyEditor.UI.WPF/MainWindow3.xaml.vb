Imports System.ComponentModel
Imports System.Globalization
Imports System.Reflection
Imports System.Windows
Imports SkyEditor.Core
Imports SkyEditor.Core.IO
Imports SkyEditor.UI.WPF.Settings

Public Class MainWindow3
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.Title = String.Format(CultureInfo.InvariantCulture, My.Resources.Language.MainTitle, My.Resources.Language.VersionPrefix, Assembly.GetExecutingAssembly.GetName.Version.ToString)
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Currently, this window only supports plugin managers that have a WPFIOUIManager as the CurrentIOUIManager</remarks>
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

    Private Sub OnIOUIManagerFileClosing(sender As Object, e As FileClosingEventArgs)
        If e.File.IsFileModified Then
            e.Cancel = Not (MessageBox.Show(My.Resources.Language.DocumentCloseConfirmation, My.Resources.Language.MainTitle, MessageBoxButton.YesNo) = MessageBoxResult.Yes)
        End If
    End Sub

    Private Sub MainWindow3_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        With CurrentPluginManager.CurrentSettingsProvider
            Dim height = .GetMainWindowHeight
            Dim width = .GetMainWindowWidth
            Dim isMax = .GetMainWindowIsMaximized

            If height.HasValue Then
                Me.Height = height.Value
            End If

            If width.HasValue Then
                Me.Width = width.Value
            End If

            If isMax Then
                Me.WindowState = WindowState.Maximized
            Else
                Me.WindowState = WindowState.Normal
            End If

        End With
    End Sub

    Private Sub MainWindow3_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        With CurrentPluginManager.CurrentSettingsProvider
            If Not Me.WindowState = WindowState.Maximized Then
                'Setting width and height while maximized results in the window being the same size when restored
                .SetMainWindowHeight(Me.Height)
                .SetMainWindowWidth(Me.Width)
            End If

            .SetMainWindowIsMaximized(Me.WindowState = WindowState.Maximized)
            .Save(CurrentPluginManager.CurrentIOProvider)
        End With
    End Sub
End Class
