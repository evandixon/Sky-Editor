Namespace IO
    ''' <summary>
    ''' Marks an object that supports raising an event when modified.
    ''' </summary>
    Public Interface INotifyModified
        Event Modified(sender As Object, e As EventArgs)
    End Interface

End Namespace
