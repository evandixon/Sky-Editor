Imports SkyEditor.Core.UI

Namespace IO
    Public Class FileClosingEventArgs
        Public Property Cancel As Boolean
        Public Property File As AvalonDockFileWrapper
        Public Sub New()
            Me.Cancel = False
        End Sub
    End Class
End Namespace

