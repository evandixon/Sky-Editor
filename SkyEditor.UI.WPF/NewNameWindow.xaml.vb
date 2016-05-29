Imports System.Windows

Public Class NewNameWindow
    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        DialogResult = True
        Me.Close()
    End Sub

    Public Property SelectedName As String
        Get
            Return txtName.Text
        End Get
        Set(value As String)
            txtName.Text = value
        End Set
    End Property

    Public Shadows Function ShowDialog() As Boolean
        Return MyBase.ShowDialog
    End Function

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Public Sub New(Message As String, Title As String)
        Me.New
        Me.Title = Title
        lbGameSelectorQuestion.Content = Message
    End Sub
End Class