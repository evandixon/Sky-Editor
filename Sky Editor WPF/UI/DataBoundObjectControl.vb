Imports SkyEditor.Core.EventArguments
Imports SkyEditor.Core.Interfaces

Namespace UI
    ''' <summary>
    ''' An IObjectControl that directly supports WPF DataBinding.
    ''' </summary>
    Public Class DataBoundObjectControl
        Inherits UserControl
        Implements iObjectControl

        ''' <summary>
        ''' Returns an IEnumeriable of Types that this control can display or edit.
        ''' </summary>
        ''' <returns></returns>
        Public Overridable Function GetSupportedTypes() As IEnumerable(Of Type) Implements iObjectControl.GetSupportedTypes
            If DataContext IsNot Nothing Then
                Return {DataContext.GetType}
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
        Public Overridable Function SupportsObject(Obj As Object) As Boolean Implements iObjectControl.SupportsObject
            Return True
        End Function

        ''' <summary>
        ''' If True, this control will not be used if another one exists.
        ''' </summary>
        ''' <param name="Obj"></param>
        ''' <returns></returns>
        Public Overridable Function IsBackupControl(Obj As Object) As Boolean Implements iObjectControl.IsBackupControl
            Return False
        End Function

        Public Overridable Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer Implements iObjectControl.GetSortOrder
            Return 0
        End Function

        ''' <summary>
        ''' Called when Header is changed.
        ''' </summary>
        Public Event HeaderUpdated(sender As Object, e As HeaderUpdatedEventArgs) Implements iObjectControl.HeaderUpdated

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
                RaiseEvent HeaderUpdated(Me, New HeaderUpdatedEventArgs(oldValue, value))
            End Set
        End Property
        Dim _header As String

        ''' <summary>
        ''' Returns the sort order of this control when editing the given type.
        ''' Note: The returned value is context-specific.  Higher values make a Control more likely to be used, but lower values make tabs appear higher in the list of tabs.
        ''' </summary>
        ''' <returns></returns>
        Public Property SortOrder As Integer

        Public Overridable Property EditingObject As Object Implements iObjectControl.EditingObject
            Get
                Return Me.DataContext
            End Get
            Set(value As Object)
                Me.DataContext = value
            End Set
        End Property

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
    End Class
    ''' <summary>
    ''' An IObjectControl that directly supports WPF DataBinding.
    ''' </summary>
    Public Class DataBoundObjectControl(Of T)
        Inherits DataBoundObjectControl

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of Type)
            Return {GetType(T)}
        End Function
    End Class
End Namespace

