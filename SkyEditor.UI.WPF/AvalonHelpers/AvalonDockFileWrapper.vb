Imports System.ComponentModel
Imports SkyEditor.Core.UI

Namespace AvalonHelpers
    Public Class AvalonDockFileWrapper
        Inherits GenericViewModel
        Implements INotifyPropertyChanged

        Public Sub New()
            MyBase.New
        End Sub
        Public Sub New(file As Object)
            MyBase.New(file)
        End Sub

        Public ReadOnly Property Tooltip As String
            Get
                Return ""
            End Get
        End Property

    End Class
End Namespace

