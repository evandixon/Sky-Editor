Imports SkyEditor.Core.UI

Namespace MenuActions
    Public Class ImportSdf
        Inherits MenuAction
        Implements IDisposable

        Dim FolderBrowserDialog1 As Windows.Forms.FolderBrowserDialog
        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            If FolderBrowserDialog1.ShowDialog = Forms.DialogResult.OK Then
                Dim source As New SdfSaveDataDirectory(IO.Path.Combine(FolderBrowserDialog1.SelectedPath, "filer", "UserSaveData"))
                Dim dest As New SdfSaveDataDirectory(SkyEditorBase.PluginHelper.GetResourceName("SDF"))
                source.MoveSaves(dest)
            End If
            Return Task.CompletedTask
        End Function

        Public Sub New()
            MyBase.New({My.Resources.Language.MenuDev, My.Resources.Language.MenuDevImportSDF})
            Me.AlwaysVisible = False

            FolderBrowserDialog1 = New Forms.FolderBrowserDialog
            FolderBrowserDialog1.Description = My.Resources.Language.PleaseSelectSD
            DevOnly = True
            SortOrder = 10.7
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    FolderBrowserDialog1.Dispose()
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

