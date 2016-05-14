Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.UI

Namespace UI
    Public Class ObjectControlPlaceholder
        Inherits UserControl
        Implements IDisposable

        ''' <summary>
        ''' Raised when the contained object raises its Modified event, if it implements iModifiable
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>

        Public Event Modified(sender As Object, e As EventArgs)

        Dim _object As Object
        Public Property ObjectToEdit As Object
            Get
                If Content Is Nothing Then
                    Return Nothing
                Else
                    Return DirectCast(Content, IObjectControl).EditingObject
                End If
            End Get
            Set(value As Object)
                If _object IsNot Nothing AndAlso TypeOf _object Is iModifiable Then
                    RemoveHandler DirectCast(_object, iModifiable).Modified, AddressOf OnModified
                End If
                _object = value
                If TypeOf value Is iModifiable Then
                    AddHandler DirectCast(value, iModifiable).Modified, AddressOf OnModified
                End If
                Dim objControl = SkyEditor.Core.UI.UIHelper.GetObjectControl(value, {GetType(UserControl)}, PluginManager.GetInstance)
                If objControl IsNot Nothing Then
                    Content = objControl
                    objControl.EditingObject = value
                Else
                    'Todo: display a "missing control" message?
                End If
            End Set
        End Property

        Private Sub ObjectControlPlaceholder_DataContextChanged(sender As Object, e As DependencyPropertyChangedEventArgs) Handles Me.DataContextChanged
            ObjectToEdit = e.NewValue
        End Sub

        Private Sub OnModified(sender As Object, e As EventArgs)
            RaiseEvent Modified(sender, e)
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    If _object IsNot Nothing AndAlso TypeOf _object Is IDisposable Then
                        DirectCast(_object, IDisposable).Dispose()
                    End If
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            ' TODO: uncomment the following line if Finalize() is overridden above.
            ' GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class
End Namespace
