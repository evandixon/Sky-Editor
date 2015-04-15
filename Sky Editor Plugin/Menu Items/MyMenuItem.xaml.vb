Imports SkyEditorBase
Imports System.Windows.Input

Public Class MyMenuItem
    Private manager As PluginManager
    Private Sub OnKeyPress(sender As Object, e As KeyEventArgs)
        If e.Key = Key.F3 Then
            'Todo: do something if someone pressed F3
        End If
    End Sub
    Private Sub menuSayHi_Click(sender As Object, e As Windows.RoutedEventArgs) Handles menuSayHi.Click
        'Todo: Add handler for click event
    End Sub
    Public Sub New(Manager As PluginManager)
        InitializeComponent()
        'In order to do much, you'll need a reference to the plugin manager.
        'It's up to you whether or not to store it, or put it in the constructor in the first place,
        'but it's recommended to get access to important data in the form, and to register event handlers.
        'Do not rely on the form being the menu item's parent's parent, as layout could change in the future.
        Me.manager = Manager

        'Using the plugin manager, you can handle certain form events.
        AddHandler Manager.Window.OnKeyPress, AddressOf OnKeyPress

        'As always, it's best to add translation support for your plugin.
        PluginHelper.TranslateForm(Me)
    End Sub
End Class