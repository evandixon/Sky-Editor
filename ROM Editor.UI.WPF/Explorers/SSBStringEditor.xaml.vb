Imports System.Windows.Controls
Imports ROMEditor.FileFormats.Explorers.Script
Imports SkyEditor.UI.WPF

Namespace Explorers
    Public Class SSBStringEditor
        Inherits ObjectControl
        Public Overrides Sub RefreshDisplay()
            With GetEditingObject(Of SSB)()
                Dim editors As New List(Of SSBStringDictionaryEditor)
                editors.Add(New SSBStringDictionaryEditor With {.EditingObject = GetEditingObject(Of SSB).English, .EditingLanguage = "English"})
                AddHandler editors.Last.IsModifiedChanged, AddressOf OnObjModified
                If .isMultiLang Then
                    editors.Add(New SSBStringDictionaryEditor With {.EditingObject = GetEditingObject(Of SSB).French, .EditingLanguage = "Français"})
                    AddHandler editors.Last.IsModifiedChanged, AddressOf OnObjModified
                    editors.Add(New SSBStringDictionaryEditor With {.EditingObject = GetEditingObject(Of SSB).German, .EditingLanguage = "Deutsche"})
                    AddHandler editors.Last.IsModifiedChanged, AddressOf OnObjModified
                    editors.Add(New SSBStringDictionaryEditor With {.EditingObject = GetEditingObject(Of SSB).Italian, .EditingLanguage = "Italiano"})
                    AddHandler editors.Last.IsModifiedChanged, AddressOf OnObjModified
                    editors.Add(New SSBStringDictionaryEditor With {.EditingObject = GetEditingObject(Of SSB).Spanish, .EditingLanguage = "Español"})
                    AddHandler editors.Last.IsModifiedChanged, AddressOf OnObjModified
                End If
                editors.Add(New SSBStringDictionaryEditor With {.EditingObject = GetEditingObject(Of SSB).Constants, .EditingLanguage = My.Resources.Language.Constants})
                AddHandler editors.Last.IsModifiedChanged, AddressOf OnObjModified
                tcTabs.Items.Clear()
                For Each item In editors
                    tcTabs.Items.Add(New TabItem With {.Content = item, .Header = item.EditingLanguage})
                Next
            End With
            IsModified = False
        End Sub

        Private Sub OnObjModified(sender As Object, e As EventArgs)
            IsModified = True
        End Sub

        Private Sub btnAdd_Click(sender As Object, e As RoutedEventArgs) Handles btnAdd.Click
            For Each item In tcTabs.Items
                Dim h = DirectCast(item, TabItem).Header

                ''I originally wanted to exclude Constants, but to make things easier, I'm going to allow adding Constants.
                ''I'll take care to remove unreferenced strings when recompiling
                'If Not h = PluginHelper.GetLanguageItem("Constants") Then

                'It would be a good idea to make sure the ID doesn't conflict with other languages, but this should be OK.
                Dim newID = GetEditingObject(Of SSB).English.Max + 1
                DirectCast(DirectCast(item, TabItem).Content, SSBStringDictionaryEditor).AddItem(newID)

                'End If
            Next
        End Sub

        Public Overrides Sub UpdateObject()
            With GetEditingObject(Of SSB)()
                For Each item As TabItem In tcTabs.Items
                    Dim editor = DirectCast(item.Content, SSBStringDictionaryEditor)
                    editor.UpdateObject()
                Next
            End With
        End Sub

        Private Sub OnLoaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Header = My.Resources.Language.Strings
        End Sub

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of Type)
            Return {GetType(SSB)}
        End Function

        Public Overrides Function IsBackupControl(Obj As Object) As Boolean
            'This is now a backup control, since strings and constants are being merged with the commands
            Return True
        End Function

        Public Overrides Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer
            Return 1
        End Function

    End Class

End Namespace
