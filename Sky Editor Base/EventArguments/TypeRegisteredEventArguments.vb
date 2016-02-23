Namespace EventArguments
    Public Class TypeRegisteredEventArgs
        Inherits EventArgs
        ''' <summary>
        ''' The base type into which the RegisteredType inherits or implements.
        ''' </summary>
        ''' <returns></returns>
        Public Property BaseType As Type

        ''' <summary>
        ''' The type that was registered.
        ''' </summary>
        ''' <returns></returns>
        Public Property RegisteredType As Type
    End Class
End Namespace

