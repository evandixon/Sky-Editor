Imports System.Collections.ObjectModel
Imports System.Collections.Specialized
Imports System.ComponentModel
Imports System.Windows
Imports SkyEditor.Core
Imports Xceed.Wpf.AvalonDock
Imports Xceed.Wpf.AvalonDock.Layout

'Credit to Ashley Davis for original implementation.
' http://www.codeproject.com/Articles/239342/AvalonDock-and-MVVM </remarks>
Public Class AvalonDockManager

#Region "Dependency Properties"
    Shared Sub New()
        'Initialize dependency properties
        PanesProperty = DependencyProperty.Register("Panes", GetType(IList), GetType(AvalonDockManager), New FrameworkPropertyMetadata(AddressOf Document_PropertyChanged))
        DocumentsProperty = DependencyProperty.Register("Documents", GetType(IList), GetType(AvalonDockManager), New FrameworkPropertyMetadata(AddressOf Pane_PropertyChanged))
        ActiveDocumentProperty = DependencyProperty.Register("ActiveDocument", GetType(Object), GetType(AvalonDockManager), New FrameworkPropertyMetadata(Nothing, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, AddressOf ActiveDocumentPane_PropertyChanged))
        ActivePaneProperty = DependencyProperty.Register("ActivePane", GetType(Object), GetType(AvalonDockManager), New FrameworkPropertyMetadata(Nothing, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, AddressOf ActiveDocumentPane_PropertyChanged))
        IsPaneVisibleProperty = DependencyProperty.RegisterAttached("IsPaneVisible", GetType(Object), GetType(AvalonDockManager), New FrameworkPropertyMetadata(True, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, AddressOf IsPaneVisible_PropertyChanged))
    End Sub

    Public Shared ReadOnly Property PanesProperty As DependencyProperty
    Public Shared ReadOnly Property DocumentsProperty As DependencyProperty
    Public Shared ReadOnly Property ActiveDocumentProperty As DependencyProperty
    Public Shared ReadOnly Property ActivePaneProperty As DependencyProperty
    'Attached
    Public Shared ReadOnly Property IsPaneVisibleProperty As DependencyProperty

    Private Shared Sub Document_PropertyChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim manager As AvalonDockManager = d

        'Deal with the previous value of the property
        If e.OldValue IsNot Nothing Then
            'Remove the old panels from AvalonDock
            Dim oldPanels As IList = e.OldValue
            manager.RemovePanels(oldPanels)

            If TypeOf oldPanels Is INotifyCollectionChanged Then
                RemoveHandler DirectCast(oldPanels, INotifyCollectionChanged).CollectionChanged, AddressOf manager.Document_CollectionChanged
            End If
        End If

        'Deal with new values
        If e.NewValue IsNot Nothing Then
            Dim newPanels As IList = e.NewValue
            manager.AddDocumentPanels(newPanels)

            If TypeOf newPanels Is INotifyCollectionChanged Then
                AddHandler DirectCast(newPanels, INotifyCollectionChanged).CollectionChanged, AddressOf manager.Document_CollectionChanged
            End If
        End If
    End Sub

    Private Shared Sub Pane_PropertyChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim manager As AvalonDockManager = d

        'Deal with the previous value of the property
        If e.OldValue IsNot Nothing Then
            'Remove the old panels from AvalonDock
            Dim oldPanels As IList = e.OldValue
            manager.RemovePanels(oldPanels)

            If TypeOf oldPanels Is INotifyPropertyChanged Then
                RemoveHandler DirectCast(oldPanels, INotifyCollectionChanged).CollectionChanged, AddressOf manager.Pane_CollectionChanged
            End If
        End If

        'Deal with new values
        If e.NewValue IsNot Nothing Then
            Dim newPanels As IList = e.NewValue
            manager.AddPanePanels(newPanels)

            If TypeOf newPanels Is INotifyCollectionChanged Then
                AddHandler DirectCast(newPanels, INotifyCollectionChanged).CollectionChanged, AddressOf manager.Pane_CollectionChanged
            End If
        End If
    End Sub

    Private Shared Sub ActiveDocumentPane_PropertyChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim c As AvalonDockManager = d
        Dim layout As LayoutContent = Nothing
        If e.NewValue IsNot Nothing AndAlso c.contentMap.TryGetValue(e.NewValue, layout) Then
            layout.IsActive = True
        End If
    End Sub

    Private Shared Sub IsPaneVisible_PropertyChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim avalonDockContent As LayoutContent = Nothing
        If TypeOf d Is LayoutContent Then
            avalonDockContent = d
        End If
        If avalonDockContent IsNot Nothing Then
            Dim isVisible = DirectCast(e.NewValue, Boolean)
            If isVisible Then
                avalonDockContent.IsActive = True 'Todo: make sure this is the correct property
            Else
                avalonDockContent.Close()
            End If
        End If
    End Sub

    Public Shared Sub SetIsPaneVisible(element As UIElement, value As Boolean)
        element.SetValue(IsPaneVisibleProperty, value)
    End Sub

    Public Shared Function GetIsPaneVisible(element As UIElement)
        Return element.GetValue(IsPaneVisibleProperty)
    End Function
#End Region

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        disableClosingEvent = False
        UpdateActiveContent()
    End Sub

    Public Event AvalonDockLoaded As EventHandler(Of EventArgs)
    Public Event DocumentClosing As EventHandler(Of DocumentClosingEventArgs)

    Public Property Panes As IList
    Public Property Documents As IList
    Public Property ActiveDocument As Object
    Public Property ActivePane As Object
    Public Property PluginManager As PluginManager
    Private Property disableClosingEvent As Boolean

    ''' <summary>
    ''' Maps view-model objects to Avalondock LayoutContent objects.
    ''' </summary>
    ''' <returns></returns>
    Private Property contentMap As Dictionary(Of Object, LayoutContent)

    ''' <summary>
    ''' Adds panels to Avalondock as documents
    ''' </summary>
    Private Sub AddDocumentPanels(panels As IEnumerable)
        For Each item In panels
            AddPanel(item, True)
        Next
    End Sub

    ''' <summary>
    ''' Adds panels to Avalondock as documents
    ''' </summary>
    Private Sub AddPanePanels(panels As IEnumerable)
        For Each item In panels
            AddPanel(item, False)
        Next
    End Sub

    ''' <summary>
    ''' Adds a panel to Avalondock
    ''' </summary>
    Private Sub AddPanel(viewModel As Object, isDocument As Boolean)
        If isDocument Then
            'Create a layout document to display the content
            Dim document = New DocumentTab(viewModel, PluginManager)

            AddHandler document.Closed, AddressOf layoutContent_Closed

            'Todo: add content to the main control
            Throw New NotImplementedException

            document.IsActive = True
        Else
            Throw New NotImplementedException
        End If
    End Sub

    ''' <summary>
    ''' Removes panels from Avalondock
    ''' </summary>
    Private Sub RemovePanels(panels As IEnumerable)
        For Each item In panels
            RemovePanel(item)
        Next
    End Sub

    ''' <summary>
    ''' Removes a panel from Avalondock
    ''' </summary>
    Private Sub RemovePanel(panel As Object)
        'Look up the content in the content map
        Dim content As LayoutContent = Nothing
        If contentMap.TryGetValue(panel, content) Then
            disableClosingEvent = True

            Try
                'The content was still in the map, and therefore still open, so close it
                content.Close()
            Finally
                disableClosingEvent = False
            End Try
        End If
    End Sub

    Private Sub ResetDocuments()
        For Each item In contentMap
            If TypeOf item.Value Is LayoutDocument Then
                RemovePanel(item.Key)
            End If
        Next
    End Sub

    Private Sub ResetPanes()
        For Each item In contentMap
            If TypeOf item.Value Is LayoutAnchorable Then
                RemovePanel(item.Key)
            End If
        Next
    End Sub

    Private Sub UpdateActiveContent()
        If TypeOf DockingManager.ActiveContent Is DocumentTab Then
            ActiveDocument = DirectCast(DockingManager.ActiveContent, DocumentTab).Document
        ElseIf TypeOf DockingManager.ActiveContent Is LayoutAnchorable Then
            Throw New NotImplementedException
        End If
    End Sub

#Region "Event Handlers"
    Private Sub layoutContent_Closed(sender As Object, e As EventArgs)
        Dim layout As LayoutContent = sender
        Dim content = layout.Content

        'Remove view model from content map
        contentMap.Remove(content)

        'Remove event handler
        RemoveHandler layout.Closed, AddressOf layoutContent_Closed

        If TypeOf layout Is LayoutDocument Then
            Documents.Remove(content)
            'Active document has closed, clear it
            If ActiveDocument = content Then
                ActiveDocument = Nothing
            End If
        ElseIf TypeOf layout Is LayoutAnchorable Then
            Throw New NotImplementedException
        End If
    End Sub

    Private Sub Document_CollectionChanged(sender As Object, e As NotifyCollectionChangedEventArgs)
        If e.Action = NotifyCollectionChangedAction.Reset Then
            ResetDocuments()
        Else
            If e.OldItems IsNot Nothing Then
                RemovePanels(e.OldItems)
            End If
            If e.NewItems IsNot Nothing Then
                AddDocumentPanels(e.NewItems)
            End If
        End If
    End Sub

    Private Sub Pane_CollectionChanged(sender As Object, e As NotifyCollectionChangedEventArgs)
        If e.Action = NotifyCollectionChangedAction.Reset Then
            ResetPanes()
        Else
            If e.OldItems IsNot Nothing Then
                RemovePanels(e.OldItems)
            End If
            If e.NewItems IsNot Nothing Then
                AddPanePanels(e.NewItems)
            End If
        End If
    End Sub

    Private Sub DockingManager_ActiveContentChanged(sender As Object, e As EventArgs) Handles DockingManager.ActiveContentChanged
        UpdateActiveContent()
    End Sub
#End Region

End Class
