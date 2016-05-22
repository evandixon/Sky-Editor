Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.Utilities

Namespace UI
    Public Class AvalonDockFileWrapper
        Public Property File As Object
        Public ReadOnly Property Title As String
            Get
                If TypeOf File Is iNamed Then
                    Return DirectCast(File, iNamed).Name
                Else
                    Return ReflectionHelpers.GetTypeFriendlyName(File.GetType)
                End If
            End Get
        End Property
        Public ReadOnly Property Tooltip As String
            Get
                Return ""
            End Get
        End Property
        Public Sub New(file As Object)
            Me.File = file
        End Sub
    End Class
End Namespace

