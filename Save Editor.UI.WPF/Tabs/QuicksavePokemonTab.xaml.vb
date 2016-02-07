Imports SaveEditor.Saves
Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Namespace Tabs
    Public Class QuicksavePokemonTab
        Inherits UserControl
        Implements iObjectControl
        Public Sub RefreshDisplay()
            lbActivePokemon.Items.Clear()
            For Each apkm In GetEditingObject(Of SkySave)().QuicksavePokemon
                'If apkm.IsValid Then
                lbActivePokemon.Items.Add(apkm)
                'End If
            Next
        End Sub
        Public Sub UpdateObject()
            Dim apkms As New List(Of Saves.SkySave.QuicksavePkm)
            For Each item In lbActivePokemon.Items
                apkms.Add(item)
            Next
            GetEditingObject(Of SkySave).QuicksavePokemon = apkms.ToArray
        End Sub
        Private Sub ActivePokemonTab_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Header = PluginHelper.GetLanguageItem("Quicksave Pokemon")
            btnEditActivePokemon.Content = PluginHelper.GetLanguageItem("Edit")
        End Sub
        Sub RefreshActivePKMDisplay()
            Dim pkms As New List(Of Saves.SkySave.QuicksavePkm)
            For Each p In lbActivePokemon.Items
                pkms.Add(p)
            Next
            lbActivePokemon.Items.Clear()
            For count As Integer = 0 To pkms.Count - 1
                'If pkms(count).ID > 0 Then
                lbActivePokemon.Items.Add(pkms(count))
                'End If
            Next
        End Sub
        Sub ShowActivePkmEditDialog()
            If lbActivePokemon.SelectedIndex > -1 Then
                Dim w As New SkyEditorWPF.ObjectWindow
                w.ObjectToEdit = lbActivePokemon.SelectedItem
                w.ShowDialog()
                lbActivePokemon.SelectedItem = w.ObjectToEdit
                IsModified = True
                RefreshActivePKMDisplay()
            End If
        End Sub
        Private Sub btnEditActivePokemon_Click(sender As Object, e As RoutedEventArgs) Handles btnEditActivePokemon.Click
            ShowActivePkmEditDialog()
        End Sub
        Private Sub lbActivePokemon_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs) Handles lbActivePokemon.MouseDoubleClick
            ShowActivePkmEditDialog()
        End Sub

        Public Function GetSupportedTypes() As IEnumerable(Of Type) Implements iObjectControl.GetSupportedTypes
            Return {GetType(Saves.SkySave)}
        End Function

        Public Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer Implements iObjectControl.GetSortOrder
            Return 6
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

End Namespace