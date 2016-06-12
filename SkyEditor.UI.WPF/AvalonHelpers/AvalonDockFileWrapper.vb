Imports System.ComponentModel
Imports System.Windows.Input
Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI
Imports SkyEditor.Core.Utilities

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

