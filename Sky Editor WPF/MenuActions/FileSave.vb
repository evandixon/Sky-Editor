Imports System.Threading.Tasks
Imports SkyEditorBase.Interfaces

Namespace MenuActions
    Public Class FileSave
        Inherits MenuAction
        Private WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
        Public Overrides Function SupportedTypes() As IEnumerable(Of Type)
            Return {GetType(Interfaces.iSavable)}
        End Function
        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            For Each item As Interfaces.iSavable In Targets
                If TypeOf item Is iOnDisk AndAlso String.IsNullOrEmpty(DirectCast(item, iOnDisk).Filename) Then
                    If TypeOf item Is iOnDisk Then
                        SaveFileDialog1.Filter = PluginManager.GetInstance.IOFiltersStringSaveAs(IO.Path.GetExtension(DirectCast(item, iOnDisk).Filename))
                    Else
                        SaveFileDialog1.Filter = PluginManager.GetInstance.IOFiltersString(IsSaveAs:=True) 'Todo: use default extension
                    End If
                    If SaveFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                        item.Save(SaveFileDialog1.FileName)
                    End If
                Else
                    item.Save()
                End If
            Next
            Return Task.CompletedTask
        End Function
        Public Sub New()
            MyBase.New({PluginHelper.GetLanguageItem("_File"), PluginHelper.GetLanguageItem("_Save"), PluginHelper.GetLanguageItem("Save _File")})
            SaveFileDialog1 = New Forms.SaveFileDialog
        End Sub
    End Class
End Namespace

