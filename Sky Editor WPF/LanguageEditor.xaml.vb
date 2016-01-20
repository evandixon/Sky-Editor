Imports SkyEditorBase.Interfaces
Imports SkyEditorBase.Language

Public Class LanguageEditor
    Inherits UserControl
    Implements iObjectControl
    Public Sub RefreshDisplay()
        cbLanguages.Items.Clear()

        GetEditingObject(Of LanguageManager).LoadAllLanguages()

        For Each item In GetEditingObject(Of LanguageManager).Languages.Keys
            cbLanguages.Items.Add(item)
        Next

        If cbLanguages.Items.Count > 0 Then
            cbLanguages.SelectedIndex = 0
        End If
    End Sub

    Private Sub cbLanguages_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbLanguages.SelectionChanged
        'Save existing values
        If e.RemovedItems IsNot Nothing AndAlso e.RemovedItems.Count > 0 AndAlso GetEditingObject(Of LanguageManager).Languages.ContainsKey(e.RemovedItems(0)) Then
            GetEditingObject(Of LanguageManager).Languages(e.RemovedItems(0)).ContainedObject.Items.Clear()
            For Each item In lbItems.Items
                GetEditingObject(Of LanguageManager).Languages(e.RemovedItems(0)).ContainedObject.Items.Add(item)
            Next
        End If

        'Load new values
        If GetEditingObject(Of LanguageManager).Languages.ContainsKey(cbLanguages.SelectedItem) Then
            lbItems.Items.Clear()
            For Each item In GetEditingObject(Of LanguageManager).Languages(cbLanguages.SelectedItem).ContainedObject.Items
                lbItems.Items.Add(item)
            Next
        End If
    End Sub

    Private Sub lbItems_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lbItems.SelectionChanged
        'Save existing values
        If e.RemovedItems IsNot Nothing AndAlso e.RemovedItems.Count > 0 Then
            With DirectCast(e.RemovedItems(0), SkyEditorBase.Language.LanguageItem)
                If Not .Value = txtValue.Text Then
                    .Value = txtValue.Text
                    IsModified = True
                End If
            End With
        End If

        'Load new values
        If lbItems.SelectedItem IsNot Nothing Then
            With DirectCast(lbItems.SelectedItem, LanguageItem)
                txtKey.Text = .Key
                txtValue.Text = .Value
            End With
        End If
    End Sub

    Public Sub UpdateObject()
        If lbItems.SelectedItem IsNot Nothing Then
            With DirectCast(lbItems.SelectedItem, LanguageItem)
                .Value = txtValue.Text
            End With
        End If

        If cbLanguages.SelectedItem IsNot Nothing AndAlso GetEditingObject(Of LanguageManager).Languages.ContainsKey(cbLanguages.SelectedItem) Then
            GetEditingObject(Of LanguageManager).Languages(cbLanguages.SelectedItem).ContainedObject.Items.Clear()
            For Each item In lbItems.Items
                GetEditingObject(Of LanguageManager).Languages(cbLanguages.SelectedItem).ContainedObject.Items.Add(item)
            Next
        End If
    End Sub

    Private Sub txtValue_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtValue.TextChanged
        If lbItems.SelectedValue IsNot Nothing AndAlso Not DirectCast(lbItems.SelectedItem, LanguageItem).Value = txtValue.Text Then
            DirectCast(lbItems.SelectedItem, LanguageItem).Value = txtValue.Text
            IsModified = True
        End If
    End Sub

    Private Sub btnNewLang_Click(sender As Object, e As RoutedEventArgs) Handles btnNewLang.Click
        Dim newDialog As New SkyEditorWindows.NewNameWindow(PluginHelper.GetLanguageItem("NewLanguageMessage", "What is the name of the language you want to add?"), PluginHelper.GetLanguageItem("NewLanguageTitle"))
        If newDialog.ShowDialog Then
            If Not GetEditingObject(Of LanguageManager).Languages.ContainsKey(newDialog.SelectedName) Then
                GetEditingObject(Of LanguageManager).EnsureLanguageLoaded(newDialog.SelectedName)
                cbLanguages.Items.Add(newDialog.SelectedName)
            End If
        End If
    End Sub

    Private Sub btnAddLangItem_Click(sender As Object, e As RoutedEventArgs) Handles btnAddLangItem.Click
        Dim addDialog As New AddLanguageItem
        For Each item In lbItems.Items
            addDialog.CurrentLanguageItems.Add(item)
        Next
        addDialog.DefaultLanguageItems.AddRange(GetEditingObject(Of LanguageManager).Languages(SettingsManager.Instance.Settings.DefaultLanguage).ContainedObject.Items)
        If addDialog.ShowDialog Then
            Dim newItem = addDialog.SelectedItem.Clone
            lbItems.Items.Add(newItem)
            lbItems.SelectedItem = newItem
        End If
    End Sub

    Public Function GetSupportedTypes() As IEnumerable(Of Type) Implements iObjectControl.GetSupportedTypes
        Return {GetType(LanguageManager)}
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