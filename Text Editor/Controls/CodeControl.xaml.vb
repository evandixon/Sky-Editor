Imports System.Windows
Imports System.Windows.Controls

Namespace Controls
    Public Class CodeControl
        Inherits SkyEditorBase.ObjectControl(Of CodeFile)
        Public Overrides Sub RefreshDisplay()
            Dim f As New Windows.Documents.FlowDocument
            f.MaxPageWidth = Double.PositiveInfinity
            'Todo: set a better page width.  Having the page width longer than the content makes the horizontal scrollbar tiny, and having it too small basically enables word wrap.
            'Since this control will edit code, defaulting to something that disables word wrap
            f.PageWidth = Int16.MaxValue 'I would use something bigger like Integer.MaxValue, or even better: Double.PositiveInfinity, but apparently it "isn't a valid value".
            Dim p As New Windows.Documents.Paragraph(New Windows.Documents.Run(EditingItem.Text))
            f.Blocks.Add(p)
            txtCode.Document = f
        End Sub
        Public Overrides Sub UpdateObject()
            Dim f As Windows.Documents.FlowDocument = txtCode.Document
            Dim b As Windows.Documents.Paragraph = f.Blocks(0)
            Dim text As New Text.StringBuilder
            For Each item In b.Inlines
                text.Append(DirectCast(item, Windows.Documents.Run).Text)
            Next
            EditingItem.Text = text.ToString
        End Sub

        Private Sub CodeControl_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Dim f As New Windows.Documents.FlowDocument
            f.MaxPageWidth = Double.PositiveInfinity
            f.PageWidth = Int16.MaxValue
            Dim p As New Windows.Documents.Paragraph(New Windows.Documents.Run(""))
            f.Blocks.Add(p)
        End Sub

        Private Sub txtText_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtCode.TextChanged
            RaiseModified()
        End Sub

        Public Overrides Function UsagePriority(Type As Type) As Integer
            Return 1
        End Function
    End Class
End Namespace