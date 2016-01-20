Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Controls.Primitives
Imports System.Windows.Input

Public Class AutoCompletePopup
    Inherits Popup
    Public ReadOnly Property SelectedItem As String
        Get
            Return lstMethodsSelection.SelectedItem?.ToString
        End Get
    End Property
    Public Property ItemsSource As IEnumerable(Of String)
        Get
            Return _itemsSource
        End Get
        Set(value As IEnumerable(Of String))
            _itemsSource = value
            ApplyFilter("")
        End Set
    End Property
    Dim _itemsSource As IEnumerable(Of String)

    Public Sub ApplyFilter(CurrentText As String)
        lstMethodsSelection.Items.Clear()
        For Each item In From s In ItemsSource Where s.ToLower.StartsWith(CurrentText.ToLower) Select s
            lstMethodsSelection.Items.Add(item)
        Next
        If lstMethodsSelection.Items.Count > 0 Then
            lstMethodsSelection.SelectedIndex = 0
        End If
    End Sub
End Class
