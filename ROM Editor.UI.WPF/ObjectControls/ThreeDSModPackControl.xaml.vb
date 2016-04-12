Imports SkyEditorBase
Imports SkyEditorBase.Interfaces
Imports SkyEditorWPF.UI

Namespace ObjectControls
    Public Class ThreeDSModPackControl
        Inherits ObjectControl
        Public Overrides Sub RefreshDisplay()
            With GetEditingObject(Of Projects.DSModPackProject)() '(Of Mods.ModSourceContainer)()
                chb3DS.IsChecked = .Output3DSFile
                chbCIA.IsChecked = .OutputCIAFile
                chbHANS.IsChecked = .OutputHans
            End With
            IsModified = False
        End Sub

        Public Overrides Sub UpdateObject()
            With GetEditingObject(Of Projects.DSModPackProject)()
                .Output3DSFile = chb3DS.IsChecked
                .OutputCIAFile = chbCIA.IsChecked
                .OutputHans = chbHANS.IsChecked
            End With
        End Sub

        Private Sub ThreeDSModPackControl_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Header = My.Resources.Language.ThreeDSBuildOptions
        End Sub

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of Type)
            Return {GetType(Projects.DSModPackProject)}
        End Function

        Public Overrides Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer
            Return 2
        End Function


        Private Sub chb_Checked(sender As Object, e As RoutedEventArgs) Handles chb3DS.Checked, chbCIA.Checked, chbHANS.Checked
            IsModified = True
        End Sub

    End Class

End Namespace
