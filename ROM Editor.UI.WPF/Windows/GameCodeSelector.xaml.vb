Imports System.Windows

Public Class GameCodeSelector

    Public Property Presets As Dictionary(Of String, String)

    Public ReadOnly Property SelectedGameCode As String
        Get
            Return txtGameCode.Text
        End Get
    End Property

    Private Sub GameCodeSelector_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        cbPreset.Items.Clear()
        For Each item In From p In Presets.Keys Order By p Ascending
            cbPreset.Items.Add(item)
        Next
    End Sub

    Private Sub btnUse_Click(sender As Object, e As Windows.RoutedEventArgs) Handles btnUse.Click
        If cbPreset.SelectedValue IsNot Nothing AndAlso Presets.ContainsKey(cbPreset.SelectedValue) Then
            txtGameCode.Text = Presets(cbPreset.SelectedValue)
        End If
    End Sub

    Private Sub btnOK_Click(sender As Object, e As Windows.RoutedEventArgs) Handles btnOK.Click
        DialogResult = True
        Me.Close()
    End Sub


End Class
