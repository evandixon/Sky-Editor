Imports System.Threading.Tasks
Imports SkyEditor.Core.UI

Namespace MenuActions
    Public Class FileNewFile
        Inherits MenuAction

        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            Dim _manager = PluginManager.GetInstance
            Dim w As New UI.NewFileWindow()
            Dim games As New Dictionary(Of String, Type)
            For Each item In _manager.GetCreatableFiles
                games.Add(PluginHelper.GetTypeName(item), item)
            Next
            w.AddGames(games.Keys)
            If w.ShowDialog Then
                Dim file As Object = _manager.CreateNewFile(w.SelectedName, games(w.SelectedGame))
                PluginHelper.RequestFileOpen(file, True)
            End If
            Return Task.CompletedTask
        End Function

        Public Sub New()
            MyBase.New({My.Resources.Language.MenuFile, My.Resources.Language.MenuFileNew, My.Resources.Language.MenuFileNewFile})
            AlwaysVisible = True
            SortOrder = 1.11
        End Sub
    End Class
End Namespace

