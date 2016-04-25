Imports ROMEditor.FileFormats.PSMD
Imports SkyEditorBase

Namespace MenuActions
    Public Class CteImageExport
        Inherits MenuAction
        Private WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
        Public Overrides Function SupportedTypes() As IEnumerable(Of Type)
            Return {GetType(CteImage)}
        End Function
        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            For Each item As CteImage In Targets
                If SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
                    Dim format As System.Drawing.Imaging.ImageFormat
                    Select Case SaveFileDialog1.FilterIndex
                        Case 0
                            format = Drawing.Imaging.ImageFormat.Png
                        Case 1
                            format = Drawing.Imaging.ImageFormat.Bmp
                        Case Else
                            format = Drawing.Imaging.ImageFormat.Png
                    End Select
                    item.ContainedImage.Save(SaveFileDialog1.FileName, format)
                End If
            Next
            Return Task.CompletedTask
        End Function
        Public Sub New()
            MyBase.New({My.Resources.Language.MenuImage, My.Resources.Language.MenuImageExport})
            SaveFileDialog1 = New Windows.Forms.SaveFileDialog
            SaveFileDialog1.Filter = $"{My.Resources.Language.PNGImages} (*.png)|*.png|{My.Resources.Language.BitmapImages} (*.bmp)|*.bmp"
            SortOrder = 4.1
        End Sub
    End Class
End Namespace

