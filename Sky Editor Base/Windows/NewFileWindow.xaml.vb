Namespace SkyEditorWindows
    Public Class NewFileWindow
        Inherits Window

        Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
            DialogResult = True
            Me.Close()
        End Sub

        Public Property SelectedGame As String
            Get
                Return cbType.LastSafeValue
            End Get
            Set(value As String)
                cbType.SelectedItem = value
            End Set
        End Property

        Public ReadOnly Property SelectedName As String
            Get
                Return txtName.text
            End Get
        End Property

        Public Sub ResetGames()
            cbType.Items.Clear()
        End Sub
        Public Sub AddGames(Games As Dictionary(Of String, Type).KeyCollection)
            For Each item In Games
                cbType.Items.Add(item)
            Next
            If cbType.Items.Count > 0 Then cbType.SelectedIndex = 0
        End Sub

        Private Sub GameTypeSelector_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Title = PluginHelper.GetLanguageItem("New File")
            PluginHelper.TranslateForm(Me)
        End Sub
        Public Shadows Function ShowDialog() As Boolean
            Return MyBase.ShowDialog
        End Function
    End Class

End Namespace
