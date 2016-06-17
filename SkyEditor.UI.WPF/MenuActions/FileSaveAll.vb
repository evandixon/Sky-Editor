Imports System.Reflection
Imports System.Windows.Forms
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI

Namespace MenuActions
    Public Class FileSaveAll
        Inherits MenuAction
        Private WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
        Public Overrides Function SupportedTypes() As IEnumerable(Of TypeInfo)
            Return {GetType(Solution).GetTypeInfo, GetType(ISavable).GetTypeInfo}
        End Function
        Public Overrides Function SupportsObjects(Objects As IEnumerable(Of Object)) As Boolean
            Dim hasProject = From o In Objects Where TypeOf o Is Solution
            Dim hasSavable = From o In Objects Where TypeOf o Is ISavable

            Return hasProject.Any AndAlso hasSavable.Any
        End Function
        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            For Each item In Targets
                If TypeOf item Is Solution Then
                    DirectCast(item, Solution).SaveAllProjects(CurrentPluginManager.CurrentIOProvider)
                ElseIf TypeOf item Is ISavable Then

                    If TypeOf item Is IOnDisk AndAlso String.IsNullOrEmpty(DirectCast(item, IOnDisk).Filename) Then
                        If TypeOf item Is IOnDisk Then
                            SaveFileDialog1.Filter = CurrentPluginManager.CurrentIOUIManager.IOFiltersStringSaveAs(IO.Path.GetExtension(DirectCast(item, IOnDisk).Filename))
                        Else
                            SaveFileDialog1.Filter = CurrentPluginManager.CurrentIOUIManager.IOFiltersString(IsSaveAs:=True) 'Todo: use default extension
                        End If
                        If SaveFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                            item.Save(SaveFileDialog1.FileName)
                        End If
                    Else
                        item.Save()
                    End If
                End If
            Next
            Return Task.CompletedTask
        End Function
        Public Sub New()
            MyBase.New({My.Resources.Language.MenuFile, My.Resources.Language.MenuFileSave, My.Resources.Language.MenuFileSaveAll})
            SaveFileDialog1 = New SaveFileDialog
            SortOrder = 1.34
        End Sub
    End Class
End Namespace

