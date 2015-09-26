﻿Namespace Interfaces
    ''' <summary>
    ''' Marks an object that supports raising an event when modified.
    ''' </summary>
    Public Interface iModifiable
        Event Modified(sender As Object, e As EventArgs)
        Sub RaiseModified()
    End Interface

End Namespace
