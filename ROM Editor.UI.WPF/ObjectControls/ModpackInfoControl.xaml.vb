﻿Imports System.Windows
Imports System.Windows.Controls
Imports ROMEditor
Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Namespace ObjectControls
    Public Class ModpackInfoControl
        Inherits UserControl
        Implements iObjectControl
        Public Sub RefreshDisplay()
            With GetEditingObject(Of ModpackInfo)()
                txtName.Text = .Name
                txtShortName.Text = .ShortName
                txtAuthor.Text = .Author
                txtVersion.Text = .Version
                txtGameCode.Text = .GameCode
            End With
        End Sub

        Public Sub UpdateObject()
            With GetEditingObject(Of ModpackInfo)()
                .Name = txtName.Text
                .ShortName = txtShortName.Text
                .Author = txtAuthor.Text
                .Version = txtVersion.Text
                .GameCode = txtGameCode.Text
            End With
        End Sub

        Private Sub ModpackInfoControl_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            SkyEditorWPF.UiHelper.TranslateForm(Me)
        End Sub

        Private Sub TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtAuthor.TextChanged, txtGameCode.TextChanged, txtName.TextChanged, txtVersion.TextChanged
            RaiseEvent IsModifiedChanged(Me, New EventArgs)
        End Sub

        Public Function GetSupportedTypes() As IEnumerable(Of Type) Implements iObjectControl.GetSupportedTypes
            Return {GetType(ModpackInfo)}
        End Function

        Public Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer Implements iObjectControl.GetSortOrder
            Return 0
        End Function

#Region "IObjectControl Support"
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
    End Class
End Namespace
