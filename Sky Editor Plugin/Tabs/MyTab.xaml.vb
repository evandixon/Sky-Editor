Imports SkyEditorBase
Imports System.Windows

Public Class MyTab
    Inherits ObjectTab
    Public Overrides Sub RefreshDisplay()
        'Here you need to assign values from your save to all of your controls
        If TypeOf Me.EditingObject Is MySave Then
            With DirectCast(Me.EditingObject, MySave)
                'MyControl.Text = .MyProperty
            End With
        End If
    End Sub

    Public Overrides ReadOnly Property SupportedTypes As Type()
        Get
            'An array of types that this tab can edit.  Because there is only one example save in this project, only one example save is shown below.
            Return {GetType(MySave)}
        End Get
    End Property

    Public Overrides Sub UpdateObject()

        If TypeOf Me.EditingObject Is MySave Then 'Make sure the save is in the format you're expecting (Supported Games above should ensure this, but in case the tab supports multiple save formats, this is necessary.)
            With DirectCast(Me.EditingObject, MySave) 'Update its properties
                '.MyProperty = MyControl.Text
            End With
            'ElseIf Typeof Save is MyOtherSave Then  'If you have more than one supported save type.
        End If

    End Sub

    Private Sub FormLoaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        'If any of your controls need initialization with code, do it here.

        'You should also add translation your form.
        'Each label, button, menu, and tab item will be translated with this statement.
        'Anything else, you'll have to use PluginHelper.GetLanguageItem
        PluginHelper.TranslateForm(Me)
    End Sub
End Class