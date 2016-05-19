Imports System.Reflection
Imports System.Threading.Tasks
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI

Namespace MenuActions
    Public Class FileOpenManual
        Inherits MenuAction
        Private WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
        Public Overrides Async Function DoAction(Targets As IEnumerable(Of Object)) As Task
            OpenFileDialog1.Filter = CurrentPluginManager.CurrentIOUIManager.IOFiltersString
            If OpenFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                Dim w As New UI.GameTypeSelector()
                Dim games As New Dictionary(Of String, TypeInfo)
                For Each item In IOHelper.GetOpenableFileTypes(CurrentPluginManager)
                    games.Add(PluginHelper.GetTypeName(item), item)
                Next
                w.AddGames(games.Keys)
                If w.ShowDialog Then
                    PluginHelper.RequestFileOpen(Await IOHelper.OpenFile(OpenFileDialog1.FileName, games(w.SelectedGame), CurrentPluginManager), True)
                End If
            End If
        End Function

        Public Sub New()
            MyBase.New({My.Resources.Language.MenuFile, My.Resources.Language.MenuFileOpen, My.Resources.Language.MenuFileOpenManual})
            AlwaysVisible = True
            OpenFileDialog1 = New Forms.OpenFileDialog
            SortOrder = 1.22
        End Sub
    End Class
End Namespace

