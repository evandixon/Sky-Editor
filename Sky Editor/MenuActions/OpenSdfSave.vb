Imports System.Windows.Forms
Imports SkyEditorBase

Namespace MenuActions
    Public Class OpenSdfSave
        Inherits SkyEditorBase.MenuAction

        Dim dialog As FolderBrowserDialog

        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            If dialog.ShowDialog Then
                PluginHelper.RequestFileOpen(PluginManager.GetInstance.OpenObject(dialog.SelectedPath), True)
            End If
            Return Task.CompletedTask
        End Function

        Public Sub New()
            MyBase.New("_SDF/_Open", "/"c, True)
            AlwaysVisible = True

            dialog = New FolderBrowserDialog
        End Sub
    End Class
End Namespace

