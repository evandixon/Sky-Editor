Imports System.Windows.Controls
Imports ROMEditor.FileFormats.ScriptDS
Imports SkyEditorBase
Imports SkyEditorBase.Interfaces
Imports SkyEditorWPF.UI

Public Class SSBScriptEditor
    Inherits ObjectControl
    Public Overrides Sub RefreshDisplay()
        With GetEditingObject(Of SSB)()
            txtScript.Text = .GetText
        End With
    End Sub

    Public Overrides Sub UpdateObject()
        With GetEditingObject(Of SSB)()

        End With
    End Sub

    Private Sub OnLoaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Me.Header = PluginHelper.GetLanguageItem("Script")
    End Sub

    Public Overrides Function GetSupportedTypes() As IEnumerable(Of Type)
        Return {GetType(SSB)}
    End Function

    Public Overrides Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer
        Return 1
    End Function

End Class
