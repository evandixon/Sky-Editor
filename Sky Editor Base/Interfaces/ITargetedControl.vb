Namespace Interfaces
    ''' <summary>
    ''' Represents a control that targets one or more objects.
    ''' Unline IObjectControls, ITargetedControls will have one instance that exists for the life of the application.
    ''' Similar to MenuActions, except in Control form.
    ''' </summary>
    Public Interface ITargetedControl
        Enum Pane
            Left
            Right
            Bottom
        End Enum
        Event HeaderChanged(sender As Object, e As EventArguments.HeaderUpdatedEventArgs)
        Event VisibilityChanged(sender As Object, e As EventArguments.VisibilityUpdatedEventArgs)
        Property Header As String
        Property IsVisible As Boolean

        ''' <summary>
        ''' Returns whether or not the ITargetedControl should start, by default, collapsed.
        ''' </summary>
        ''' <returns></returns>
        Function StartCollapsed() As Boolean

        ''' <summary>
        ''' Returns the default pane the ITargetedControl should be located in.
        ''' </summary>
        ''' <returns></returns>
        Function GetDefaultPane() As Pane

        ''' <summary>
        ''' Updates the ITargetedControl's targets.
        ''' Also may affect visibility.
        ''' </summary>
        ''' <param name="Targets"></param>
        Sub UpdateTargets(Targets As IEnumerable(Of Object))
    End Interface

End Namespace
