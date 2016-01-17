Imports System.Threading.Tasks

Namespace MenuActions
    Public Class FileSaveAs
        Inherits MenuAction
        Private WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
        Public Overrides Function SupportedTypes() As IEnumerable(Of Type)
            Return {GetType(Interfaces.iSavable)}
        End Function
        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            For Each item As Interfaces.iSavable In Targets
                SaveFileDialog1.Filter = PluginManager.GetInstance.IOFiltersStringSaveAs(IO.Path.GetExtension(item.DefaultExtension))
                If SaveFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                    item.Save(SaveFileDialog1.FileName)
                End If
            Next
            Return Task.CompletedTask
        End Function
        Public Sub New()
            MyBase.New({PluginHelper.GetLanguageItem("_File"), PluginHelper.GetLanguageItem("_Save"), PluginHelper.GetLanguageItem("Save File _As...")})
            SaveFileDialog1 = New Forms.SaveFileDialog
        End Sub
    End Class
End Namespace

