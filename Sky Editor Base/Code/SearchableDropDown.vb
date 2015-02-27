Public Class SearchableDropDown
    Inherits ComboBox
    Private DefaultValue As String
    Private DefaultIndex As Integer

    Private Sub SearchableDropDown_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        If Me.Items.Contains(Me.Text) Then
            DefaultValue = Me.SelectedValue
            DefaultIndex = Me.SelectedIndex
            Me.Background = New SolidColorBrush(Windows.Media.Color.FromRgb(229, 229, 229))
            Me.FontStyle = FontStyles.Normal
        Else
            Me.Background = New SolidColorBrush(Windows.Media.Color.FromRgb(255, 0, 0))
            Me.FontStyle = FontStyles.Oblique
        End If
    End Sub

    Private Sub SearchableDropDown_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Me.IsEditable = True
    End Sub
    ''' <summary>
    ''' Gets the current value.  If the current text is not a valid item, returns the previous valid item.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LastSafeValue As String
        Get
            If Me.Items.Contains(Me.Text) Then
                Return Me.Text
            Else
                Return DefaultValue
            End If
        End Get
        Set(value As String)
            Me.SelectedValue = value
        End Set
    End Property
    Public Property LastSafeIndex As Integer
        Get
            If Me.Items.Contains(Me.Text) Then
                Return Me.SelectedIndex
            Else
                Return DefaultIndex
            End If
        End Get
        Set(value As Integer)
            Me.SelectedIndex = value
        End Set
    End Property

    Private Sub SearchableDropDown_TextInput(sender As Object, e As TextCompositionEventArgs) Handles Me.TextInput
    
    End Sub
End Class
