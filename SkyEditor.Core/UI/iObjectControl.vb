Imports System.Reflection

Namespace UI
    Public Interface IObjectControl
        ''' <summary>
        ''' Raised when the IObjectControl's Header has been changed
        ''' </summary>
        Event HeaderUpdated(sender As Object, e As HeaderUpdatedEventArgs)

        ''' <summary>
        ''' Raised when the IObjectControl's IsModified property has changed.
        ''' </summary>
        Event IsModifiedChanged(sender As Object, e As EventArgs)

        ''' <summary>
        ''' Returns the value of the Header.  Only used when the iObjectControl is behaving as a tab.
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property Header As String

        ''' <summary>
        ''' The object this control is intended to edit.
        ''' </summary>
        ''' <returns></returns>
        Property EditingObject As Object

        ''' <summary>
        ''' Whether or not the EditingObject has been modified without saving.
        ''' Set to true when the user changes anything in the GUI.
        ''' Set to false when the object is saved, or if the user undoes every change.
        ''' </summary>
        ''' <returns></returns>
        Property IsModified As Boolean

        ''' <summary>
        ''' Updates the current IObjectControl's reference to the current plugin manager
        ''' </summary>
        ''' <param name="manager">Instance of the current plugin manager</param>
        Sub SetPluginManager(manager As PluginManager)

        ''' <summary>
        ''' Returns an IEnumerable of every type that the iObjectControl is programmed to handle.
        ''' EditingObject will be of one of these types.
        ''' </summary>
        ''' <returns></returns>
        Function GetSupportedTypes() As IEnumerable(Of Type)

        ''' <summary>
        ''' Returns whether or not the iObjectControl supports the given object.
        ''' Obj will be of one of the types in GetSupportedTypes, but this function gives the iObjectControl more control over what objects it will edit.
        ''' Should return true if there is no situation exists where a given object of a supported type is not supported.
        ''' </summary>
        ''' <param name="Obj"></param>
        ''' <returns></returns>
        Function SupportsObject(Obj As Object) As Boolean

        ''' <summary>
        ''' Determines whether or not this iObjectControl should be used for the given object if another control exists for it.
        ''' If false, this will be used if SupportsObject(Obj) is true.
        ''' If true, this will only be used if no other iObjectControl can edit the given object.
        ''' 
        ''' If multiple backup controls are present, GetSortOrder will be used to determine which iObjectControl is used.
        ''' </summary>
        ''' <param name="Obj"></param>
        ''' <returns></returns>
        Function IsBackupControl(Obj As Object) As Boolean

        ''' <summary>
        ''' Returns the sort order of this control when editing the given type.
        ''' Note: The returned value is context-specific.  Higher values make a Control more likely to be used, but lower values make tabs appear higher in the list of tabs.
        ''' Note: Negative values will result in the control not being used if there are other controls with positive values.
        ''' </summary>
        ''' <param name="CurrentType">Type of the EditingObject to get a sort order for.</param>
        ''' <param name="IsTab">Whether or not the iObjectControl is registered to behave as a Tab or a Control.</param>
        ''' <returns></returns>
        Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer
    End Interface

End Namespace
