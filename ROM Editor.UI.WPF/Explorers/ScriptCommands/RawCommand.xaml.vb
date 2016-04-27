Imports SkyEditorWPF.UI

Public Class RawCommand
    Inherits DataBoundObjectControl(Of ROMEditor.FileFormats.Explorers.Script.Commands.RawCommand)

    Public Overrides Function IsBackupControl(Obj As Object) As Boolean
        Return True
    End Function
End Class
