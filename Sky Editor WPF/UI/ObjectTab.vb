Imports SkyEditor.Core.EventArguments
Imports SkyEditor.Core.Interfaces
Namespace UI
    Public Class ObjectTab
        Inherits TabItem

        Public Property ContainedObjectControl As iObjectControl
            Get
                If TypeOf Me.Content Is iObjectControl Then
                    Return Me.Content
                Else
                    Return Nothing
                End If
            End Get
            Set(value As iObjectControl)
                If Me.Content IsNot Nothing AndAlso TypeOf Me.Content Is iObjectControl Then
                    RemoveHandler DirectCast(Me.Content, iObjectControl).HeaderUpdated, AddressOf OnContentHeaderChanged
                End If

                If TypeOf value Is UserControl Then
                    Me.Content = value
                End If

                If value.Header IsNot Nothing Then
                    Me.Header = value.Header
                Else
                    Me.Header = PluginHelper.GetTypeName(value.GetType)
                End If

                If Me.Content IsNot Nothing AndAlso TypeOf Me.Content Is iObjectControl Then
                    AddHandler DirectCast(Me.Content, iObjectControl).HeaderUpdated, AddressOf OnContentHeaderChanged
                End If
            End Set
        End Property

        Private Sub OnContentHeaderChanged(sender As Object, e As HeaderUpdatedEventArgs)
            Me.Header = e.NewValue
        End Sub

        Public Sub New()
            Margin = New Thickness(0, 0, 0, 0)
        End Sub
        Public Sub New(ContainedObjectControl As iObjectControl)
            Me.New
            Me.ContainedObjectControl = ContainedObjectControl
        End Sub
    End Class

End Namespace
