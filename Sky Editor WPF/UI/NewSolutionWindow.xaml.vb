Imports System.Windows.Forms
Namespace UI
    Public Class NewSolutionWindow
        Implements IDisposable

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

            For Each item As Solution In Manager.GetRegisteredObjects(GetType(Solution))
                ddType.Items.Add(item)
            Next
            If ddType.Items.Count > 0 Then ddType.SelectedIndex = 0
        End Sub

        Private Sub btnBrowse_Click(sender As Object, e As RoutedEventArgs) Handles btnBrowse.Click
            If _folderBrowser.ShowDialog = Forms.DialogResult.OK Then
                txtLocation.Text = _folderBrowser.SelectedPath
            End If
        End Sub

        Private Sub NewProjectWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Title = PluginHelper.GetLanguageItem("New Solution")
            UiHelper.TranslateForm(Me)
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

        Public Property SelectedSolution As Solution
            Get
                Return ddType.LastSafeValue
            End Get
            Set(value As Solution)
                ddType.LastSafeValue = value
            End Set
        End Property

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    If _folderBrowser IsNot Nothing Then
                        _folderBrowser.Dispose()
                    End If
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            ' TODO: uncomment the following line if Finalize() is overridden above.
            ' GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class

End Namespace
