Imports System.Windows

Public Class NewFileWindow
    Inherits Window

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        DialogResult = True
        Me.Close()
    End Sub

    Public Property SelectedType As Type
        Get
            Return cbType.SelectedValue
        End Get
        Set(value As Type)
            cbType.SelectedValue = value
        End Set
    End Property

    Public ReadOnly Property SelectedName As String
        Get
            Return txtName.Text
        End Get
    End Property

    Public Sub SetGames(Games As Dictionary(Of String, Type))
        cbType.ItemsSource = Games
    End Sub

    Public Shadows Function ShowDialog() As Boolean
        Return MyBase.ShowDialog
    End Function
End Class

