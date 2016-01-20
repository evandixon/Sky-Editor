Namespace Interfaces
    Public Interface iObjectControl
        ''' <summary>
        ''' Raised when the iObjectControl's Header has been changed
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Event HeaderUpdated(sender As Object, e As EventArguments.HeaderUpdatedEventArgs)

        ''' <summary>
        ''' Raised when the iObjectControl's IsModified property has changed.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
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
        ''' Returns an IEnumerable of every type that the iObjectControl is programmed to handle.
        ''' EditingObject will be of one of these types.
        ''' </summary>
        ''' <returns></returns>
        Function GetSupportedTypes() As IEnumerable(Of Type)

        ''' <summary>
        ''' Returns the sort order of this control when editing the given type.
        ''' Note: The returned value is context-specific.  Higher values make a Control more likely to be used, but lower values make tabs appear higher in the list of tabs.
        ''' </summary>
        ''' <param name="CurrentType">Type of the EditingObject to get a sort order for.</param>
        ''' <param name="IsTab">Whether or not the iObjectControl is registered to behave as a Tab or a Control.</param>
        ''' <returns></returns>
        Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer
    End Interface

End Namespace
