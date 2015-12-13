Imports SkyEditorBase

Namespace MenuActions
    Public Class CteImageImport
        Inherits MenuAction
        Private WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
        Public Overrides Function SupportedTypes() As IEnumerable(Of Type)
            Return {GetType(FileFormats.CteImage)}
        End Function
        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            For Each item As FileFormats.CteImage In Targets
                If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
                    item.ContainedImage = Drawing.Bitmap.FromFile(OpenFileDialog1.FileName)
                End If
            Next
            Return Task.CompletedTask
        End Function
        Public Sub New()
            MyBase.New({PluginHelper.GetLanguageItem("_Image"), PluginHelper.GetLanguageItem("_Import Image")})
            OpenFileDialog1 = New Windows.Forms.OpenFileDialog
            OpenFileDialog1.Filter = "PNG Images (*.png)|*.png|Bitmap Images (*.bmp)|*.bmp|All Files (*.*)|*.*"
        End Sub
    End Class
End Namespace

