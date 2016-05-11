Imports System.Collections.ObjectModel
Imports System.Windows.Controls

Namespace Explorers
    Public Class SsbScriptToolbar
        Public Property Target As ListView
        Public Property ScriptFile As FileFormats.Explorers.Script.SSB

        Protected Sub btnTest_OnClick(sender As Object, e As RoutedEventArgs) Handles btnTest.Click
            Dim newItem As New SSBScriptEditor.ObjectContainer
            Dim newCmd = ScriptFile.CreateCommand((From i In ScriptFile.CurrentCommandInfo Where i.CommandType.Equals(GetType(FileFormats.Explorers.Script.Commands.MonologueCommand))).First)
            DirectCast(newCmd, FileFormats.Explorers.Script.Commands.MonologueCommand).Line.English = "Insertion test"
            newItem.Item = newCmd
            DirectCast(Target.ItemsSource, ObservableCollection(Of SSBScriptEditor.ObjectContainer)).Insert(Math.Max(Target.SelectedIndex, 0), newItem)
        End Sub
    End Class
End Namespace

