Imports System.Windows.Forms
Imports SkyEditor.Core.UI
Imports SkyEditorBase

Namespace MenuActions
    Public Class OpenSdfSave
        Inherits MenuAction
        Implements IDisposable

        Dim dialog As FolderBrowserDialog

        Public Overrides Async Function DoAction(Targets As IEnumerable(Of Object)) As Task
            If dialog.ShowDialog Then
                PluginHelper.RequestFileOpen(Await PluginManager.GetInstance.OpenObject(dialog.SelectedPath), True)
            End If
        End Function

        Public Sub New()
            MyBase.New({My.Resources.Language.MenuDev, My.Resources.Language.MenuDevOpenSDF})
            AlwaysVisible = False

            dialog = New FolderBrowserDialog
            DevOnly = True
            SortOrder = 10.6
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    dialog.Dispose()
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

