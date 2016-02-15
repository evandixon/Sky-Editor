Imports SkyEditorBase.Language
Namespace UI
    Public Class AddLanguageItem
        Public Sub New()

            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            DefaultLanguageItems = New List(Of LanguageItem)
            CurrentLanguageItems = New List(Of LanguageItem)

            Me.Title = PluginHelper.GetLanguageItem("Add Language Item")
            UiHelper.TranslateForm(Me)
        End Sub
        Public ReadOnly Property SelectedItem As LanguageItem
            Get
                Return ComboBox.SelectedItem
            End Get
        End Property
        Public Property DefaultLanguageItems As List(Of LanguageItem)
        Public Property CurrentLanguageItems As List(Of LanguageItem)
        Private Sub button_Click(sender As Object, e As RoutedEventArgs) Handles button.Click
            DialogResult = True
            Me.Close()
        End Sub
        Public Shadows Function ShowDialog() As Boolean
            ComboBox.Items.Clear()
            For Each item In DefaultLanguageItems
                If Not CurrentLanguageItems.Contains(item) Then
                    ComboBox.Items.Add(item)
                End If
            Next
            If ComboBox.Items.Count > 0 Then
                ComboBox.SelectedIndex = 0
                Return MyBase.ShowDialog
            Else
                MessageBox.Show(PluginHelper.GetLanguageItem("NoMoreLanguageItemsMessage", "There are no more language items you can add."))
                Return False
            End If
        End Function
    End Class
End Namespace
