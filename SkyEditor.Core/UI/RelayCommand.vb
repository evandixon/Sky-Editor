Imports System.Windows.Input

Namespace UI
    ''' <summary>
    ''' A command whose sole purpose is to relay its functionality to other objects by invoking delegates. The default return value for the CanExecute method is 'true'.
    ''' </summary>
    ''' <remarks>Credit to Torsten Tiedt for providing the original source:
    ''' http://www.dotmaniac.net/wpf-karl-shifletts-relaycommand/ </remarks>
    Public Class RelayCommand
        Implements ICommand

#Region "Declarations"
        Private ReadOnly _CanExecute As Func(Of Boolean)
        Private ReadOnly _Execute As Action
#End Region

#Region "Constructors"
        Public Sub New(execute As Action)
            Me.New(execute, Nothing)
        End Sub

        Public Sub New(execute As Action, canExecute As Func(Of Boolean))
            If execute Is Nothing Then
                Throw New ArgumentNullException(NameOf(execute))
            End If
            _Execute = execute
            _CanExecute = canExecute
        End Sub
#End Region

#Region "ICommand"
        Public Custom Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged
            AddHandler(ByVal value As EventHandler)
                If _CanExecute IsNot Nothing Then
                    AddHandler CommandManager.RequerySuggested, value
                End If
            End AddHandler

            RemoveHandler(ByVal value As EventHandler)
                If _CanExecute IsNot Nothing Then
                    RemoveHandler CommandManager.RequerySuggested, value
                End If
            End RemoveHandler

            RaiseEvent(ByVal sender As Object, ByVal e As System.EventArgs)
                'This is the RaiseEvent block
                CommandManager.InvalidateRequerySuggested()
            End RaiseEvent
        End Event

        Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
            If _CanExecute Is Nothing Then
                Return True
            Else
                Return _CanExecute.Invoke
            End If
        End Function

        Public Sub Execute(parameter As Object) Implements ICommand.Execute
            _Execute.Invoke()
        End Sub
#End Region
    End Class
End Namespace
