Public Interface iModifiable
    Event Modified(sender As Object, e As EventArgs)
    Sub RaiseModified()
End Interface
