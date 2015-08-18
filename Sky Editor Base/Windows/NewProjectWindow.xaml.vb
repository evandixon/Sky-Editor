Imports System.Windows.Forms

Public Class NewProjectWindow
    Dim _folderBrowser As FolderBrowserDialog
    Private Sub btnOk_Click(sender As Object, e As RoutedEventArgs) Handles btnOk.Click
        DialogResult = True
        Me.Close()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As RoutedEventArgs) Handles btnCancel.Click
        DialogResult = False
        Me.Close()
    End Sub
    Public Sub New(Manager As PluginManager)
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _folderBrowser = New FolderBrowserDialog

        For Each item In Manager.ProjectTypes.Keys
            ddType.Items.Add(item)
        Next
        If ddType.Items.Count > 0 Then ddType.SelectedIndex = 0
    End Sub

    Private Sub btnBrowse_Click(sender As Object, e As RoutedEventArgs) Handles btnBrowse.Click
        If _folderBrowser.ShowDialog = Forms.DialogResult.OK Then
            txtLocation.Text = _folderBrowser.SelectedPath
        End If
    End Sub

    Public Property SelectedName As String
        Get
            Return txtName.Text
        End Get
        Set(value As String)
            txtName.Text = value
        End Set
    End Property

    Public Property SelectedLocation As String
        Get
            Return txtLocation.Text
        End Get
        Set(value As String)
            txtLocation.Text = value
        End Set
    End Property

    Public Property SelectedProjectType As String
        Get
            Return ddType.LastSafeValue
        End Get
        Set(value As String)
            ddType.LastSafeValue = value
        End Set
    End Property
End Class
