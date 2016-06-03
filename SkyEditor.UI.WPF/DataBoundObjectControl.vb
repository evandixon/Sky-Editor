Imports System.ComponentModel
Imports System.Reflection
Imports System.Windows.Controls
Imports SkyEditor.Core
Imports SkyEditor.Core.UI
Imports SkyEditor.Core.Utilities

''' <summary>
''' An IObjectControl that directly supports WPF DataBinding.
''' </summary>
Public Class DataBoundObjectControl
    Inherits UserControl
    Implements IObjectControl
    Implements INotifyPropertyChanged

#Region "Events"
    ''' <summary>
    ''' Raised when Header is changed.
    ''' </summary>
    Public Event HeaderUpdated(sender As Object, e As HeaderUpdatedEventArgs) Implements IObjectControl.HeaderUpdated

    ''' <summary>
    ''' Raised when IsModified is changed.
    ''' </summary>
    Public Event IsModifiedChanged As IObjectControl.IsModifiedChangedEventHandler Implements IObjectControl.IsModifiedChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
#End Region

#Region "Properties"
    ''' <summary>
    ''' Returns the value of the Header.  Only used when the iObjectControl is behaving as a tab.
    ''' </summary>
    ''' <returns></returns>
    Public Property Header As String Implements IObjectControl.Header
        Get
            Return _header
        End Get
        Set(value As String)
            If Not value = _header Then
                Dim oldValue = _header
                _header = value
                RaiseEvent HeaderUpdated(Me, New HeaderUpdatedEventArgs(oldValue, value))
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Header)))
            End If
        End Set
    End Property
    Dim _header As String

    ''' <summary>
    ''' Whether or not the EditingObject has been modified without saving.
    ''' Set to true when the user changes anything in the GUI.
    ''' Set to false when the object is saved, or if the user undoes every change.
    ''' </summary>
    ''' <returns></returns>
    Public Property IsModified As Boolean Implements IObjectControl.IsModified
        Get
            Return _isModified
        End Get
        Set(value As Boolean)
            If Not value = _isModified Then
                Dim oldValue As Boolean = _isModified
                _isModified = value
                RaiseEvent IsModifiedChanged(Me, New EventArgs)
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(IsModified)))
            End If
        End Set
    End Property
    Dim _isModified As Boolean

    ''' <summary>
    ''' The object the control edits
    ''' </summary>
    ''' <returns></returns>
    Public Overridable Property ObjectToEdit As Object Implements IObjectControl.EditingObject
        Get
            Return Me.DataContext
        End Get
        Set(value As Object)
            Me.DataContext = value
        End Set
    End Property

    ''' <summary>
    ''' Returns the sort order of this control when editing the given type.
    ''' Note: The returned value is context-specific.  Higher values make a Control more likely to be used, but lower values make tabs appear higher in the list of tabs.
    ''' </summary>
    ''' <returns></returns>
    Public Property SortOrder As Integer

    ''' <summary>
    ''' The current instance of the plugin manager
    ''' </summary>
    ''' <returns></returns>
    Public Property CurrentPluginManager As PluginManager

    ''' <summary>
    ''' The type of the object to edit
    ''' </summary>
    ''' <returns></returns>
    Public Property TargetType As Type

    ''' <summary>
    ''' If True, this control will not be used if another one exists.
    ''' </summary>
    ''' <returns></returns>
    Public Property IsBackupControl As Boolean
#End Region

    ''' <summary>
    ''' Returns an IEnumeriable of Types that this control can display or edit.
    ''' </summary>
    ''' <returns></returns>
    Public Overridable Function GetSupportedTypes() As IEnumerable(Of Type) Implements IObjectControl.GetSupportedTypes
        If TargetType IsNot Nothing Then
            Return {TargetType}
        Else
            Return {}
        End If
    End Function

    ''' <summary>
    ''' Determines whether or not the control supports the given object.
    ''' The given object will inherit or implement one of the types in GetSupportedTypes.
    ''' </summary>
    ''' <param name="Obj"></param>
    ''' <returns></returns>
    Public Overridable Function SupportsObject(Obj As Object) As Boolean Implements IObjectControl.SupportsObject
        Return True
    End Function

    ''' <summary>
    ''' If True, this control will not be used if another one exists.
    ''' </summary>
    ''' <param name="Obj"></param>
    ''' <returns></returns>
    Public Overridable Function GetIsBackupControl(Obj As Object) As Boolean Implements IObjectControl.IsBackupControl
        Return IsBackupControl
    End Function

    Public Overridable Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer Implements IObjectControl.GetSortOrder
        Return SortOrder
    End Function

    Public Sub SetPluginManager(manager As PluginManager) Implements IObjectControl.SetPluginManager
        CurrentPluginManager = manager
    End Sub

End Class

