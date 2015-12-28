Namespace MenuActions
    Public Class ImportSdf
        Inherits SkyEditorBase.MenuAction
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
            MyBase.New("SDF/Import", "/", True)
            Me.AlwaysVisible = True

            FolderBrowserDialog1 = New Forms.FolderBrowserDialog
            FolderBrowserDialog1.Description = SkyEditorBase.PluginHelper.GetLanguageItem("Please select your SD Card")
        End Sub
    End Class
End Namespace

