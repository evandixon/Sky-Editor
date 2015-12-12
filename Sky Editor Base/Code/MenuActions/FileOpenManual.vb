Imports System.Threading.Tasks

Namespace MenuActions
    Public Class FileOpenManual
        Inherits MenuAction
        Private WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            Dim _manager = PluginManager.GetInstance
            OpenFileDialog1.Filter = _manager.IOFiltersString
            If OpenFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                Dim w As New SkyEditorWindows.GameTypeSelector()
                Dim games As New Dictionary(Of String, Type)
                For Each item In _manager.OpenableFiles
                    games.Add(PluginHelper.GetLanguageItem(item.Name), item)
                Next
                w.AddGames(games.Keys)
                If w.ShowDialog Then
                    PluginHelper.RequestFileOpen(_manager.OpenFile(OpenFileDialog1.FileName, games(w.SelectedGame)), True)
                End If
            End If
            Return Task.CompletedTask
        End Function

        Public Sub New()
            MyBase.New({PluginHelper.GetLanguageItem("_File"), PluginHelper.GetLanguageItem("_Open"), PluginHelper.GetLanguageItem("Open (Let me _Choose Game)")})
            AlwaysVisible = True
            OpenFileDialog1 = New Forms.OpenFileDialog
        End Sub
    End Class
End Namespace

