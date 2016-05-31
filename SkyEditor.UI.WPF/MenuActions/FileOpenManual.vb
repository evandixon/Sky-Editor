Imports System.Reflection
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI
Imports SkyEditor.Core.Utilities

Namespace MenuActions
    Public Class FileOpenManual
        Inherits MenuAction
        Private WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
        Public Overrides Async Function DoAction(Targets As IEnumerable(Of Object)) As Task
            OpenFileDialog1.Filter = CurrentPluginManager.CurrentIOUIManager.IOFiltersString
            If OpenFileDialog1.ShowDialog = DialogResult.OK Then
                Dim w As New FileTypeSelector()
                Dim games As New Dictionary(Of String, TypeInfo)
                For Each item In IOHelper.GetOpenableFileTypes(CurrentPluginManager)
                    games.Add(ReflectionHelpers.GetTypeFriendlyName(item), item)
                Next
                w.SetFileTypeSource(games)
                If w.ShowDialog Then
                    CurrentPluginManager.CurrentIOUIManager.OpenFile(Await IOHelper.OpenFile(OpenFileDialog1.FileName, w.SelectedFileType, CurrentPluginManager), True)
                End If
            End If
        End Function

        Public Sub New()
            MyBase.New({My.Resources.Language.MenuFile, My.Resources.Language.MenuFileOpen, My.Resources.Language.MenuFileOpenManual})
            AlwaysVisible = True
            OpenFileDialog1 = New OpenFileDialog
            SortOrder = 1.22
        End Sub
    End Class
End Namespace

