Imports SkyEditorBase.Interfaces
Namespace SkyEditorWindows
    Public Class GameTypeSelector
        Implements iGameTypeSelector
        Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
            DialogResult = True
            Me.Close()
        End Sub

        Public Property SelectedGame As String Implements iGameTypeSelector.SelectedGame
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
        Public Sub AddGames(Games As Dictionary(Of String, Type).KeyCollection) Implements iGameTypeSelector.AddGames
            For Each item In Games
                cbGame.Items.Add(item)
            Next
        End Sub

        Private Sub GameTypeSelector_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            UiHelper.TranslateForm(Me)
            Me.Title = PluginHelper.GetLanguageItem("File Type Selector")
        End Sub
        Public Shadows Function ShowDialog() As Boolean Implements iGameTypeSelector.ShowDialog
            Return MyBase.ShowDialog
        End Function
    End Class
End Namespace