Imports SkyEditorBase.Interfaces
Imports Xceed.Wpf.AvalonDock.Layout

Public Class DocumentTab
    Inherits LayoutDocument
    Implements IDisposable
    Private _manager As PluginManager
    Private WithEvents _document As Object

#Region "Properties"
    Public Property Document As Object
        Get
            If TypeOf Me.Content Is iObjectControl Then
                _document = DirectCast(Me.Content, iObjectControl).EditingObject()
            ElseIf TypeOf Me.Content Is TabControl Then
                For Each item In DirectCast(Me.Content, TabControl).Items
                    If TypeOf item.Content Is iObjectControl Then
                        _document = DirectCast(item.Content, iObjectControl).EditingObject()
                    End If
                Next
            End If
            Return _document
        End Get
        Set(value As Object)
            'If _document IsNot Nothing AndAlso TypeOf _document Is iModifiable Then
            '    RemoveHandler DirectCast(_document, iModifiable).Modified, AddressOf File_FileModified
            'End If
            If _document IsNot Nothing AndAlso TypeOf _document Is iSavable Then
                RemoveHandler DirectCast(_document, iSavable).FileSaved, AddressOf _file_FileSaved
            End If

            If value IsNot Nothing Then
                Dim tabs = _manager.GetRefreshedTabs(value, {GetType(UserControl)})
                Dim ucTabs = (From t In tabs Where Utilities.ReflectionHelpers.IsOfType(t, GetType(UserControl))).ToList
                Dim count = ucTabs.Count '- (From t In ucTabs Where t.GetSortOrder(value.GetType, True) < 0).Count
                If count > 1 Then
                    Dim tabControl As New TabControl
                    tabControl.TabStripPlacement = Controls.Dock.Left
                    For Each item In UiHelper.GenerateObjectTabs(ucTabs)
                        tabControl.Items.Add(item)
                        AddHandler item.ContainedObjectControl.IsModifiedChanged, AddressOf File_FileModified
                    Next
                    Me.Content = tabControl

                ElseIf count = 1 Then
                    Dim control = ucTabs.First '(From t In ucTabs Where t.GetSortOrder(value.GetType, True) >= 0).First
                    Me.Content = control
                    AddHandler control.IsModifiedChanged, AddressOf File_FileModified
                Else
                    'Nothing is registered to edit this object.
                    Dim label As New Label
                    label.Content = String.Format(PluginHelper.GetLanguageItem("There are no UserControls for this object of type ""{0}""."), value.GetType.FullName)
                    Me.Content = label
                End If
            End If
            _document = value

            If TypeOf _document Is iSavable Then
                AddHandler DirectCast(_document, iSavable).FileSaved, AddressOf _file_FileSaved
            End If
        End Set
    End Property

    ''' <summary>
    ''' Gets whether or not the file has been modified since it was last opened or saved.
    ''' </summary>
    ''' <returns></returns>
    Public Property IsModified As Boolean
        Get
            Return _isModified
        End Get
        Private Set(value As Boolean)
            _isModified = value
            UpdateTitle()
            If value = False Then
                If TypeOf Me.Content Is iObjectControl Then
                    DirectCast(Me.Content, iObjectControl).IsModified = False
                ElseIf TypeOf Me.Content Is TabControl Then
                    For Each item As TabItem In DirectCast(Me.Content, TabControl).Items
                        If TypeOf item.Content Is iObjectControl Then
                            DirectCast(item.Content, iObjectControl).IsModified = False
                        End If
                    Next
                End If
            End If
        End Set
    End Property
    Dim _isModified As Boolean

    Private Property DisposeOnExit As Boolean
#End Region

#Region "Events"
    Public Event FileModified(sender As Object, e As EventArgs)
    Public Event FileSaved(sender As Object, e As EventArgs)
#End Region

#Region "Event Handlers"
    Private Sub DocumentTab_Closing(sender As Object, e As ComponentModel.CancelEventArgs) Handles Me.Closing
        If IsModified Then
            If MessageBox.Show(PluginHelper.GetLanguageItem("Document Close Notification", "Are you sure you want to close this file?  Any unsaved changes will be lost."), PluginHelper.GetLanguageItem("Sky Editor"), MessageBoxButton.YesNo) = MessageBoxResult.No Then
                e.Cancel = True
            Else
                If TypeOf _document Is IDisposable Then
                    PluginHelper.RaiseFileClosed(Me, New EventArguments.FileClosedEventArgs With {.File = Me._document, .WillDispose = True})
                    DirectCast(_document, IDisposable).Dispose()
                Else
                    PluginHelper.RaiseFileClosed(Me, New EventArguments.FileClosedEventArgs With {.File = Me._document, .WillDispose = False})
                End If
            End If
        Else
            If TypeOf _document Is IDisposable Then
                PluginHelper.RaiseFileClosed(Me, New EventArguments.FileClosedEventArgs With {.File = Me._document, .WillDispose = True})
                DirectCast(_document, IDisposable).Dispose()
            Else
                PluginHelper.RaiseFileClosed(Me, New EventArguments.FileClosedEventArgs With {.File = Me._document, .WillDispose = False})
            End If
        End If
        If TypeOf Me.Content Is TabControl Then
            For Each item As TabItem In (DirectCast(Me.Content, TabControl)).Items
                If TypeOf item.Content Is IDisposable Then
                    DirectCast(item.Content, IDisposable).Dispose()
                End If
            Next
        ElseIf TypeOf Me.Content Is IDisposable Then
            DirectCast(Me.Content, IDisposable).Dispose()
        End If
    End Sub
    Private Sub File_FileModified(sender As Object, e As EventArgs)
        If TypeOf sender Is iObjectControl Then
            IsModified = DirectCast(sender, iObjectControl).IsModified
        Else
            IsModified = True
        End If
        RaiseEvent FileModified(sender, e)
    End Sub
    Private Sub _file_FileSaved(sender As Object, e As EventArgs)
        IsModified = False
        RaiseEvent FileSaved(sender, e)
    End Sub
#End Region

#Region "Methods"
    Public Sub SaveFile(Filename As String)
        If TypeOf Document Is ISavableAs Then
            DirectCast(Document, ISavableAs).Save(Filename)
        End If
    End Sub
    Public Sub SaveFile()
        If TypeOf Document Is iSavable Then
            DirectCast(Document, iSavable).Save()
        End If
    End Sub

    Private Sub UpdateTitle()
        If _document IsNot Nothing Then
            If TypeOf _document Is iNamed Then
                Me.Title = DirectCast(_document, iNamed).Name
            Else
                Me.Title = _document.ToString
            End If
            If IsModified Then
                Me.Title = "* " & Me.Title
            End If
        End If
    End Sub
#End Region

#Region "Constructors"
    Public Sub New()
        Me.CanClose = True
        Me.IsModified = False
    End Sub
    Public Sub New(File As Object, Manager As PluginManager, Optional DisposeOnExit As Boolean = False)
        Me.New()
        _manager = Manager
        Me.Document = File
        UpdateTitle()
        Me.DisposeOnExit = DisposeOnExit
    End Sub

#End Region

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                If _document IsNot Nothing Then
                    If TypeOf _document Is IDisposable Then
                        DirectCast(_document, IDisposable).Dispose()
                    End If
                End If
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
