Imports System.Windows.Controls
Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Public Class SSBStringEditor
    Inherits UserControl
    Implements iObjectControl
    Public Sub RefreshDisplay()
        With GetEditingObject(Of FileFormats.SSB)()
            Dim editors As New List(Of SSBStringDictionaryEditor)
            editors.Add(New SSBStringDictionaryEditor With {.EditingObject = GetEditingObject(Of FileFormats.SSB).English, .EditingLanguage = "English"})
            If .isMultiLang Then
                editors.Add(New SSBStringDictionaryEditor With {.EditingObject = GetEditingObject(Of FileFormats.SSB).French, .EditingLanguage = "Français"})
                editors.Add(New SSBStringDictionaryEditor With {.EditingObject = GetEditingObject(Of FileFormats.SSB).German, .EditingLanguage = "Deutsche"})
                editors.Add(New SSBStringDictionaryEditor With {.EditingObject = GetEditingObject(Of FileFormats.SSB).Italian, .EditingLanguage = "Italiano"})
                editors.Add(New SSBStringDictionaryEditor With {.EditingObject = GetEditingObject(Of FileFormats.SSB).Spanish, .EditingLanguage = "Español"})
            End If
            editors.Add(New SSBStringDictionaryEditor With {.EditingObject = GetEditingObject(Of FileFormats.SSB).Constants, .EditingLanguage = PluginHelper.GetLanguageItem("Constants")})
            tcTabs.Items.Clear()
            For Each item In editors
                tcTabs.Items.Add(New TabItem With {.Content = item, .Header = item.EditingLanguage})
            Next
        End With
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As RoutedEventArgs) Handles btnAdd.Click
        For Each item In tcTabs.Items
            Dim h = DirectCast(item, TabItem).Header

            ''I originally wanted to exclude Constants, but to make things easier, I'm going to allow adding Constants.
            ''I'll take care to remove unreferenced strings when recompiling
            'If Not h = PluginHelper.GetLanguageItem("Constants") Then

            'It would be a good idea to make sure the ID doesn't conflict with other languages, but this should be OK.
            Dim newID = GetEditingObject(Of FileFormats.SSB).English.Keys.Max
            DirectCast(DirectCast(item, TabItem).Content, SSBStringDictionaryEditor).AddItem(newID)

            'End If
        Next
    End Sub

    Public Sub UpdateObject()
        With GetEditingObject(Of FileFormats.SSB)()

        End With
    End Sub

    Private Sub OnLoaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Me.Header = PluginHelper.GetLanguageItem("Strings")
    End Sub

    Public Function GetSupportedTypes() As IEnumerable(Of Type) Implements iObjectControl.GetSupportedTypes
        Return {GetType(FileFormats.SSB)}
    End Function

    Public Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer Implements iObjectControl.GetSortOrder
        Return 1
    End Function

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
#End Region
End Class
