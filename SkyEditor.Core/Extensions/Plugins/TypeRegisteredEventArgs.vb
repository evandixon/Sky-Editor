Imports System.Reflection

Namespace Extensions.Plugins
    Public Class TypeRegisteredEventArgs
        Inherits EventArgs
        ''' <summary>
        ''' The base type into which the RegisteredType inherits or implements.
        ''' </summary>
        ''' <returns></returns>
        Public Property BaseType As TypeInfo

        ''' <summary>
        ''' The type that was registered.
        ''' </summary>
        ''' <returns></returns>
        Public Property RegisteredType As TypeInfo
    End Class
End Namespace

