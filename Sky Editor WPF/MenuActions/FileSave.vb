Imports System.Threading.Tasks
Imports SkyEditorBase.Interfaces

Namespace MenuActions
    Public Class FileSave
        Inherits MenuAction
        Private WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
        Public Overrides Function SupportedTypes() As IEnumerable(Of Type)
            Return {GetType(Interfaces.iSavable)}
        End Function
        'Public Overrides Function SupportsObject(Obj As Object) As Boolean
        '    Return Not TypeOf Obj Is Solution AndAlso Not TypeOf Obj Is Project
        'End Function
        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            For Each item In Targets
                If TypeOf item Is iSavable Then
                    Dim sav = DirectCast(item, iSavable)
                    If TypeOf sav Is ISavableAs Then
                        'Detect if the file has a filename
                        If TypeOf item Is iOnDisk AndAlso String.IsNullOrEmpty(DirectCast(item, iOnDisk).Filename) Then
                            'If it doesn't, then do a SaveAs.
                            If TypeOf item Is iOnDisk Then
                                SaveFileDialog1.Filter = PluginManager.GetInstance.IOFiltersStringSaveAs(IO.Path.GetExtension(DirectCast(item, iOnDisk).Filename))
                            Else
                                SaveFileDialog1.Filter = PluginManager.GetInstance.IOFiltersString(IsSaveAs:=True) 'Todo: use default extension
                            End If

                            If SaveFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                                DirectCast(sav, ISavableAs).Save(SaveFileDialog1.FileName)
                            Else
                                sav.Save()
                            End If
                        Else
                            sav.Save()
                        End If
                    Else
                        sav.Save()
                    End If
                Else
                    'The act of getting to this point (calling DocumentTab.Document) forces any changes in the GUI to be applied to the underlying object.
                    'Therefore, we've done all we need.
                End If
            Next
            Return Task.CompletedTask
        End Function
        Public Sub New()
            MyBase.New({PluginHelper.GetLanguageItem("_File"), PluginHelper.GetLanguageItem("_Save"), PluginHelper.GetLanguageItem("Save _File")})
            SaveFileDialog1 = New Forms.SaveFileDialog
            AlwaysVisible = True
        End Sub
    End Class
End Namespace

