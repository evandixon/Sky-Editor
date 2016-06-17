Imports SkyEditor.UI.WPF

Public Class MessageBinEntryEditor
    Inherits ObjectControl

    Public Overrides Sub RefreshDisplay()
        'With GetEditingObject(Of ROMEditor.FileFormats.MessageBinStringEntry)()
        '    txtRaw.Text = .Entry
        'End With
        'txtRaw.SetBinding(New DependencyProperty(), "Entry")
        Me.DataContext = GetEditingObject()
        IsModified = False
    End Sub

    Public Overrides Sub UpdateObject()
        'With GetEditingObject(Of ROMEditor.FileFormats.MessageBinStringEntry)()
        '    .Entry = txtRaw.Text
        'End With
    End Sub

    Public Overrides Function GetSupportedTypes() As IEnumerable(Of Type)
        Return {} '{GetType(ROMEditor.FileFormats.MessageBinStringEntry)} '{GetType(Mods.ModSourceContainer)}
    End Function

    Public Overrides Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer
        Return 1
    End Function

    Private Sub NDSModSrcEditor_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Me.Header = My.Resources.Language.Message
    End Sub

End Class
