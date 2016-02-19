﻿Imports System.Windows.Controls
Imports SkyEditorBase
Imports SkyEditorBase.Interfaces
Imports SkyEditorWPF
Imports SkyEditorWPF.UI

Public Class PsmdLuaLangIntegration
    Implements iObjectControl
    Implements IDisposable

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.Header = PluginHelper.GetLanguageItem("Message")
        btnAdd.Content = PluginHelper.GetLanguageItem("Add")
    End Sub

    Public Sub RefreshDisplay()
        With PluginManager.GetInstance.GetOpenedFileProject(GetEditingObject) 'GetEditingObject(Of CodeFiles.LuaCodeFile)()
            Dim messageFiles As New Dictionary(Of String, FileFormats.MessageBin)
            For Each item In IO.Directory.GetDirectories(IO.Path.Combine(.GetRootDirectory, "Languages"), "*", IO.SearchOption.TopDirectoryOnly)
                Dim msgfile = New FileFormats.MessageBin
                Dim filename = IO.Path.Combine(item, IO.Path.GetFileNameWithoutExtension(GetEditingObject(Of CodeFiles.LuaCodeFile).Filename))

                Dim exists As Boolean = False
                If IO.File.Exists(filename) Then
                    exists = True
                ElseIf IO.File.Exists(filename & ".bin") Then
                    filename &= ".bin"
                    exists = True
                End If

                If exists Then
                    msgfile.OpenFile(filename)
                    messageFiles.Add(IO.Path.GetFileName(item), msgfile)
                End If
            Next

            tcTabs.Items.Clear()
            For Each item In messageFiles
                Dim t As New TabItem
                t.Header = item.Key
                Dim p As New ObjectControlPlaceholder
                AddHandler p.Modified, AddressOf Me.OnModified
                t.Content = p
                p.ObjectToEdit = item.Value
                tcTabs.Items.Add(t)
            Next
        End With
        IsModified = False
    End Sub

    Public Sub UpdateObject()
        For Each item As TabItem In tcTabs.Items
            DirectCast(DirectCast(item.Content, ObjectControlPlaceholder).ObjectToEdit, FileFormats.MessageBin).Save()
        Next
    End Sub

    Public Function GetSupportedTypes() As IEnumerable(Of Type) Implements iObjectControl.GetSupportedTypes
        Return {GetType(CodeFiles.LuaCodeFile)} '{GetType(Mods.ModSourceContainer)}
    End Function

    Public Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer Implements iObjectControl.GetSortOrder
        Return 1
    End Function

    Public Function SupportsObject(Obj As Object) As Boolean Implements iObjectControl.SupportsObject
        Return PluginManager.GetInstance.GetOpenedFileProject(Obj) IsNot Nothing
    End Function

    Private Sub OnModified(sender As Object, e As EventArgs)
        IsModified = True
    End Sub

    Private Async Sub btnAdd_Click(sender As Object, e As RoutedEventArgs) Handles btnAdd.Click
        Dim p As Projects.PsmdLuaProject = PluginManager.GetInstance.GetOpenedFileProject(GetEditingObject)
        Dim oldText As String = btnAdd.Content
        If Not p.IsLanguageLoaded Then
            btnAdd.IsEnabled = False
            btnAdd.Content = String.Format(PluginHelper.GetLanguageItem("LoadingButtonText", "{0} (Loading)"), oldText)
        End If
        Dim id As UInteger = Await p.GetNewLanguageID
        For Each item As TabItem In tcTabs.Items
            DirectCast(DirectCast(item.Content, ObjectControlPlaceholder).ObjectToEdit, FileFormats.MessageBin).AddBlankEntry(id)
        Next
        btnAdd.IsEnabled = True
        btnAdd.Content = oldText
    End Sub

#Region "IObjectControl Support"

    Public Function IsBackupControl(Obj As Object) As Boolean Implements iObjectControl.IsBackupControl
        Return False
    End Function

    ''' <summary>
    ''' Called when Header is changed.
    ''' </summary>
    Public Event HeaderUpdated As iObjectControl.HeaderUpdatedEventHandler Implements iObjectControl.HeaderUpdated

    ''' <summary>
    ''' Called when IsModified is changed.
    ''' </summary>
    Public Event IsModifiedChanged As iObjectControl.IsModifiedChangedEventHandler Implements iObjectControl.IsModifiedChanged

    ''' <summary>
    ''' Returns the value of the Header.  Only used when the iObjectControl is behaving as a tab.
    ''' </summary>
    ''' <returns></returns>
    Public Property Header As String Implements iObjectControl.Header
        Get
            Return _header
        End Get
        Set(value As String)
            Dim oldValue = _header
            _header = value
            RaiseEvent HeaderUpdated(Me, New EventArguments.HeaderUpdatedEventArgs(oldValue, value))
        End Set
    End Property
    Dim _header As String

    ''' <summary>
    ''' Returns the current EditingObject, after casting it to type T.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <returns></returns>
    Protected Function GetEditingObject(Of T)() As T
        Return PluginHelper.Cast(Of T)(_editingObject)
    End Function

    ''' <summary>
    ''' Returns the current EditingObject.
    ''' It is recommended to use GetEditingObject(Of T), since it returns iContainter(Of T).Item if the EditingObject implements that interface.
    ''' </summary>
    ''' <returns></returns>
    Protected Function GetEditingObject() As Object
        Return _editingObject
    End Function

    ''' <summary>
    ''' The way to get the EditingObject from outside this class.  Refreshes the display on set, and updates the object on get.
    ''' Calling this from inside this class could result in a stack overflow, especially if called from UpdateObject, so use GetEditingObject or GetEditingObject(Of T) instead.
    ''' </summary>
    ''' <returns></returns>
    Public Property EditingObject As Object Implements iObjectControl.EditingObject
        Get
            UpdateObject()
            Return _editingObject
        End Get
        Set(value As Object)
            _editingObject = value
            RefreshDisplay()
        End Set
    End Property
    Dim _editingObject As Object

    ''' <summary>
    ''' Whether or not the EditingObject has been modified without saving.
    ''' Set to true when the user changes anything in the GUI.
    ''' Set to false when the object is saved, or if the user undoes every change.
    ''' </summary>
    ''' <returns></returns>
    Public Property IsModified As Boolean Implements iObjectControl.IsModified
        Get
            Return _isModified
        End Get
        Set(value As Boolean)
            Dim oldValue As Boolean = _isModified
            _isModified = value
            If Not oldValue = _isModified Then
                RaiseEvent IsModifiedChanged(Me, New EventArgs)
            End If
        End Set
    End Property
    Dim _isModified As Boolean


#End Region

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                If tcTabs IsNot Nothing Then
                    For Each item As TabItem In tcTabs.Items
                        If item.Content IsNot Nothing AndAlso TypeOf item.Content Is ObjectControlPlaceholder Then
                            DirectCast(item.Content, ObjectControlPlaceholder).Dispose()
                        End If
                    Next
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