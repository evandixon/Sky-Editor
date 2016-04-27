Imports System.ComponentModel
Imports SkyEditor.Core.Interfaces
Imports SkyEditorBase.Interfaces

Namespace UI
    Public Class PluginUiElement
        Inherits MarshalByRefObject
        Implements INotifyPropertyChanged
        Implements iModifiable

        Public Property IsEnabled As Boolean
            Get
                Return _isEnabled
            End Get
            Set(value As Boolean)
                _isEnabled = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(IsEnabled)))
            End Set
        End Property
        Dim _isEnabled As Boolean

        Public Property Name As String
            Get
                Return _name
            End Get
            Set(value As String)
                _name = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Name)))
            End Set
        End Property
        Dim _name As String

        Public Property Author As String
            Get
                Return _author
            End Get
            Set(value As String)
                _author = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Author)))
            End Set
        End Property
        Dim _author As String

        Public Property Credits As String
            Get
                Return _credits
            End Get
            Set(value As String)
                _credits = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Credits)))
            End Set
        End Property
        Dim _credits As String

        Public Property Filename As String
            Get
                Return _filename
            End Get
            Set(value As String)
                _filename = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Filename)))
            End Set
        End Property
        Dim _filename As String

        Public Property ContainedDefinition As iSkyEditorPlugin

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
        Public Event Modified As iModifiable.ModifiedEventHandler Implements iModifiable.Modified

        Public Sub RaiseModified() Implements iModifiable.RaiseModified
            RaiseEvent Modified(Me, New EventArgs)
        End Sub

        Private Sub PluginUiElement_PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Handles Me.PropertyChanged
            RaiseModified()
        End Sub
    End Class

End Namespace
