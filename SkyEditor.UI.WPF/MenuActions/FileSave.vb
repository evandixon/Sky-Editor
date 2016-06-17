Imports System.Reflection
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI

Namespace MenuActions
    Public Class FileSave
        Inherits MenuAction
        Private WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
        Public Overrides Function SupportedTypes() As IEnumerable(Of TypeInfo)
            Return {GetType(ISavable).GetTypeInfo}
        End Function
        'Public Overrides Function SupportsObject(Obj As Object) As Boolean
        '    Return Not TypeOf Obj Is Solution AndAlso Not TypeOf Obj Is Project
        'End Function
        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            For Each item In Targets
                If TypeOf item Is ISavable Then
                    Dim sav = DirectCast(item, ISavable)
                    If TypeOf sav Is ISavableAs Then
                        'Detect if the file has a filename
                        If TypeOf item Is IOnDisk AndAlso String.IsNullOrEmpty(DirectCast(item, IOnDisk).Filename) Then
                            'If it doesn't, then do a SaveAs.
                            If TypeOf item Is IOnDisk Then
                                SaveFileDialog1.Filter = CurrentPluginManager.CurrentIOUIManager.IOFiltersStringSaveAs(IO.Path.GetExtension(DirectCast(item, IOnDisk).Filename))
                            Else
                                SaveFileDialog1.Filter = CurrentPluginManager.CurrentIOUIManager.IOFiltersString(IsSaveAs:=True) 'Todo: use default extension
                            End If

                            If SaveFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                                DirectCast(sav, ISavableAs).Save(SaveFileDialog1.FileName, CurrentPluginManager.CurrentIOProvider)
                            Else
                                sav.Save(CurrentPluginManager.CurrentIOProvider)
                            End If
                        Else
                            sav.Save(CurrentPluginManager.CurrentIOProvider)
                        End If
                    Else
                        sav.Save(CurrentPluginManager.CurrentIOProvider)
                    End If
                Else
                    'The act of getting to this point (calling DocumentTab.Document) forces any changes in the GUI to be applied to the underlying object.
                    'Therefore, we've done all we need.
                End If
            Next
            Return Task.CompletedTask
        End Function
        Public Sub New()
            MyBase.New({My.Resources.Language.MenuFile, My.Resources.Language.MenuFileSave, My.Resources.Language.MenuFileSaveFile})
            SaveFileDialog1 = New SaveFileDialog
            'AlwaysVisible = True
            SortOrder = 1.31
        End Sub
    End Class
End Namespace

