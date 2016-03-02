Imports System.Threading.Tasks

Namespace MenuActions
    Public Class FileNewFile
        Inherits MenuAction

        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            Dim _manager = PluginManager.GetInstance
            Dim w As New UI.NewFileWindow()
            Dim games As New Dictionary(Of String, Type)
            For Each item In _manager.GetCreatableFiles
                games.Add(PluginHelper.GetLanguageItem(item.Name), item)
            Next
            w.AddGames(games.Keys)
            If w.ShowDialog Then
                Dim file As Object = _manager.CreateNewFile(w.SelectedName, games(w.SelectedGame))
                PluginHelper.RequestFileOpen(file, True)
            End If
            Return Task.CompletedTask
        End Function

        Public Sub New()
            MyBase.New({PluginHelper.GetLanguageItem("_File"), PluginHelper.GetLanguageItem("_New"), PluginHelper.GetLanguageItem("_File")})
            AlwaysVisible = True
            SortOrder = 1.11
        End Sub
    End Class
End Namespace

