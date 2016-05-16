Imports System.Reflection
Imports System.Threading.Tasks
Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI

Namespace MenuActions
    Public Class FileSaveAs
        Inherits MenuAction
        Private WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
        Public Overrides Function SupportedTypes() As IEnumerable(Of TypeInfo)
            Return {GetType(ISavableAs).GetTypeInfo}
        End Function
        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            For Each item As ISavableAs In Targets
                SaveFileDialog1.Filter = PluginManager.GetInstance.IOFiltersStringSaveAs(IO.Path.GetExtension(item.GetDefaultExtension))
                If SaveFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                    item.Save(SaveFileDialog1.FileName, PluginManager.GetInstance.CurrentIOProvider)
                End If
            Next
            Return Task.CompletedTask
        End Function
        Public Sub New()
            MyBase.New({My.Resources.Language.MenuFile, My.Resources.Language.MenuFileSave, My.Resources.Language.MenuFileSaveFileAs})
            SaveFileDialog1 = New Forms.SaveFileDialog
            SortOrder = 1.32
        End Sub
    End Class
End Namespace

