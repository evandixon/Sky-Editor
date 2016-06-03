Imports System.Threading.Tasks
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI
Imports SkyEditor.Core.Utilities

Namespace MenuActions
    Public Class FileNewFile
        Inherits MenuAction

        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            Dim w As New NewFileWindow()
            Dim games As New Dictionary(Of String, Type)
            For Each item In IOHelper.GetCreatableFileTypes(CurrentPluginManager)
                games.Add(ReflectionHelpers.GetTypeFriendlyName(item), item)
            Next
            w.SetGames(games)
            If w.ShowDialog Then
                Dim file As Object = IOHelper.CreateNewFile(w.SelectedName, w.SelectedType)
                CurrentPluginManager.CurrentIOUIManager.OpenFile(file, True)
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

