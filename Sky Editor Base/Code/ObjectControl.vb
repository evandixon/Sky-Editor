Imports SkyEditorBase.Interfaces
Public MustInherit Class ObjectControl
    Inherits UserControl
    Public Sub New()
        MyBase.New()
    End Sub
    ''' <summary>
    ''' Called when the EditingObject is being changed to something else, but before it has actually been changed.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Public Event EditingObjectChanging(sender As Object, e As EventArgs)

    ''' <summary>
    ''' Called when the EditingObject has been changed.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Public Event EditingObjectChanged(sender As Object, e As EventArgs)

    Public Overridable Property EditingObject As Object
        Get
            Return _editingObject
        End Get
        Set(value As Object)
            RaiseEvent EditingObjectChanging(Me, New EventArgs)
            _editingObject = value
            RaiseEvent EditingObjectChanged(Me, New EventArgs)
        End Set
    End Property
    Dim _editingObject As Object

    Public Overridable Sub RefreshDisplay()

    End Sub

    Public Overridable Sub UpdateObject()

    End Sub

    ''' <summary>
    ''' Calls RaiseModified on the editing object if it implements iModifiable.
    ''' </summary>
    Public Sub RaiseModified()
        If TypeOf EditingObject Is iModifiable Then DirectCast(EditingObject, iModifiable).RaiseModified()
    End Sub

    Overridable ReadOnly Property SupportedTypes As Type()
        Get
            Return {}
        End Get
    End Property
    ''' <summary>
    ''' Priority for this control to be used in relevant circumstances.  Higher values are more likely to be used.
    ''' Useless for ObjectTabs or EditorTabs.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Overridable Function UsagePriority(Type As Type) As Integer
        Return 0
    End Function
End Class
Public MustInherit Class ObjectControl(Of T)
    Inherits ObjectControl
    Public Sub New()
        MyBase.New()
    End Sub

    Public Property EditingItem As T
        Get
            If EditingObject IsNot Nothing AndAlso ObjectFile(Of T).IsObjectFile(Me.EditingObject.GetType) Then
                Return DirectCast(Me.EditingObject, ObjectFile(Of T)).ContainedObject
            Else
                Return DirectCast(MyBase.EditingObject, T)
            End If
        End Get
        Set(value As T)
            If ObjectFile(Of T).IsObjectFile(Me.EditingObject) Then
                'TODO: raise events before and after next line
                DirectCast(Me.EditingObject, ObjectFile(Of T)).ContainedObject = value
            Else
                Me.EditingObject = value
            End If
        End Set
    End Property

    Public Overrides ReadOnly Property SupportedTypes As Type()
        Get
            Return {GetType(T)}
        End Get
    End Property
End Class