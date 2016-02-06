﻿Imports System.Windows.Controls
Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Public Class MessageBinEditor
    Implements iObjectControl

    Public Sub RefreshDisplay()
        With GetEditingObject(Of ROMEditor.FileFormats.MessageBin)()
            lstEntries.Items.Clear()
            For Each item In .Strings
                lstEntries.Items.Add(item)
            Next
            If lstEntries.Items.Count > 0 Then
                lstEntries.SelectedIndex = 0
            End If
        End With
        IsModified = False
    End Sub

    Public Sub UpdateObject()
        With GetEditingObject(Of ROMEditor.FileFormats.MessageBin)()
            .Strings.Clear()
            For Each item In lstEntries.Items
                .Strings.Add(item)
            Next
            If lstEntries.SelectedItem IsNot Nothing Then
                lstEntries.Items(lstEntries.SelectedIndex) = placeEntry.ObjectToEdit
            End If
        End With
    End Sub

    Private Sub lstEntries_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lstEntries.SelectionChanged
        Dim current As FileFormats.MessageBin.StringEntry = placeEntry.ObjectToEdit
        If e.RemovedItems IsNot Nothing AndAlso e.RemovedItems.Count > 0 AndAlso current IsNot Nothing Then
            lstEntries.Items(lstEntries.Items.IndexOf(e.RemovedItems(0))) = current
        End If
        If e.AddedItems IsNot Nothing AndAlso e.AddedItems.Count > 0 Then
            placeEntry.ObjectToEdit = e.AddedItems(0)
        End If
    End Sub

    Public Function GetSupportedTypes() As IEnumerable(Of Type) Implements iObjectControl.GetSupportedTypes
        Return {GetType(ROMEditor.FileFormats.MessageBin)} '{GetType(Mods.ModSourceContainer)}
    End Function

    Public Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer Implements iObjectControl.GetSortOrder
        Return 1
    End Function

    Private Sub NDSModSrcEditor_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Me.Header = PluginHelper.GetLanguageItem("Message")
    End Sub

#Region "IObjectControl Support"
    Public Function SupportsObject(Obj As Object) As Boolean Implements iObjectControl.SupportsObject
        Return True
    End Function

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

    Private Sub btnAdd_Click(sender As Object, e As RoutedEventArgs) Handles btnAdd.Click
        Dim entry As New FileFormats.MessageBin.StringEntry
        entry.Entry = ""

        Dim pendingID As UInteger = 1 'Set an arbitrary ID
RedoCheck:
        'And make sure nothing else has the same ID
        For Each item As FileFormats.MessageBin.StringEntry In lstEntries.Items
            If item.Hash = pendingID Then
                'If something else is using the ID, then increment 1 by and start the collision check again
                pendingID += 1
                GoTo RedoCheck
            End If
        Next

        entry.Hash = pendingID
        lstEntries.Items.Add(entry)
    End Sub
#End Region
End Class
