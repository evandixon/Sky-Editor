Imports System.Reflection
Imports ROMEditor.FileFormats.PSMD
Imports SkyEditor.Core.UI

Namespace MenuActions
    Public Class CteImageImport
        Inherits MenuAction
        Private WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
        Public Overrides Function SupportedTypes() As IEnumerable(Of TypeInfo)
            Return {GetType(CteImage).GetTypeInfo}
        End Function
        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            For Each item As CteImage In Targets
                If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
                    item.ContainedImage = Drawing.Bitmap.FromFile(OpenFileDialog1.FileName)
                End If
            Next
            Return Task.CompletedTask
        End Function
        Public Sub New()
            MyBase.New({My.Resources.Language.MenuImage, My.Resources.Language.MenuImageImport})
            OpenFileDialog1 = New Windows.Forms.OpenFileDialog
            OpenFileDialog1.Filter = $"{My.Resources.Language.PNGImages} (*.png)|*.png|{My.Resources.Language.BitmapImages} (*.bmp)|*.bmp|{My.Resources.Language.AllFiles} (*.*)|*.*"
            SortOrder = 4.2
        End Sub
    End Class
End Namespace

