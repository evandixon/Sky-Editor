Imports System.Reflection
Imports System.Windows

Public Class FileTypeSelector
    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        DialogResult = True
        Me.Close()
    End Sub

    Public Property SelectedFileType As TypeInfo
        Get
            Return cbGame.SelectedValue
        End Get
        Set(value As TypeInfo)
            cbGame.SelectedValue = value
        End Set
    End Property

    Public Sub SetFileTypeSource(Games As Dictionary(Of String, TypeInfo))
        cbGame.ItemsSource = Games
    End Sub

    Public Shadows Function ShowDialog() As Boolean
        Return MyBase.ShowDialog
    End Function
End Class