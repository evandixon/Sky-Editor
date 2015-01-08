Namespace Windows
    Public Class GameTypeSelector

        Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
            DialogResult = True
            Me.Close()
        End Sub

        Public Property SelectedGame As String
            Get
                Return cbGame.SelectedItem
            End Get
            Set(value As String)
                cbGame.SelectedItem = value
            End Set
        End Property
        Public Sub ResetGames()
            cbGame.Items.Clear()
        End Sub
        Public Sub AddGames(Games As Dictionary(Of String, Type).KeyCollection)
            For Each item In Games
                cbGame.Items.Add(item)
            Next
        End Sub

        Private Sub GameTypeSelector_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            btnOK.Content = PluginHelper.GetLanguageItem("OK")
            lbGame.Content = PluginHelper.GetLanguageItem("Game")
            lbGameSelectorQuestion.Content = PluginHelper.GetLanguageItem("GameSelectorQuestion", "What game is this save for?")
            Me.Title = PluginHelper.GetLanguageItem("GameSelectorTitle", "Game Selector")
        End Sub
    End Class
End Namespace

