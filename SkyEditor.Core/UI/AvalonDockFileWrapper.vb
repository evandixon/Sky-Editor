Imports System.Windows.Input
Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Utilities

Namespace UI
    Public Class AvalonDockFileWrapper
        Implements INotifyPropertyChanged

        Public Sub New()
            IsFileModified = False
            CloseCommand = New RelayCommand(AddressOf OnClosed)
        End Sub
        Public Sub New(file As Object)
            Me.New
            Me.File = file
        End Sub

        Public Event CloseCommandExecuted(sender As Object, e As EventArgs)
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Public Property File As Object
            Get
                Return _file
            End Get
            Set(value As Object)
                If TypeOf _file Is ISavable Then
                    RemoveHandler DirectCast(_file, ISavable).FileSaved, AddressOf File_OnSaved
                End If
                If TypeOf _file Is INotifyPropertyChanged Then
                    RemoveHandler DirectCast(_file, INotifyPropertyChanged).PropertyChanged, AddressOf File_OnModified
                End If
                If TypeOf _file Is INotifyModified Then
                    RemoveHandler DirectCast(_file, INotifyModified).Modified, AddressOf File_OnModified
                End If

                _file = value

                IsFileModified = False

                If TypeOf _file Is ISavable Then
                    AddHandler DirectCast(_file, ISavable).FileSaved, AddressOf File_OnSaved
                End If
                If TypeOf _file Is INotifyPropertyChanged Then
                    AddHandler DirectCast(_file, INotifyPropertyChanged).PropertyChanged, AddressOf File_OnModified
                End If
                If TypeOf _file Is INotifyModified Then
                    AddHandler DirectCast(_file, INotifyModified).Modified, AddressOf File_OnModified
                End If
            End Set
        End Property
        Dim _file As Object

        Public ReadOnly Property Title As String
            Get
                Dim out As String
                If TypeOf File Is iNamed Then
                    out = DirectCast(File, iNamed).Name
                Else
                    out = ReflectionHelpers.GetTypeFriendlyName(File.GetType)
                End If
                If IsFileModified Then
                    Return "* " & out
                Else
                    Return out
                End If
            End Get
        End Property

        Public Property IsFileModified As Boolean
            Get
                Return _isFileModified
            End Get
            Set(value As Boolean)
                If Not _isFileModified = value Then
                    _isFileModified = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(IsFileModified)))

                    'Title is dependant on this property, so notify that it changed too
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Title)))
                End If
            End Set
        End Property
        Dim _isFileModified As Boolean

        Public ReadOnly Property Tooltip As String
            Get
                Return ""
            End Get
        End Property

        Public ReadOnly Property CloseCommand As ICommand

        Protected Overridable Function OnClosed() As Task
            RaiseEvent CloseCommandExecuted(Me, New EventArgs)
            Return Task.FromResult(0)
        End Function

        Private Sub File_OnSaved(sender As Object, e As EventArgs)
            IsFileModified = False
        End Sub

        Private Sub File_OnModified(sender As Object, e As EventArgs)
            IsFileModified = True
        End Sub
    End Class
End Namespace

