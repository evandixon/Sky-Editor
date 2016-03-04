Imports SkyEditorBase

Namespace MenuActions
    Public Class ExtractNDS
        Inherits MenuAction
        Public Overrides Function SupportedTypes() As IEnumerable(Of Type)
            Return {GetType(Roms.GenericNDSRom)}
        End Function

        Public Overrides Async Function DoAction(Targets As IEnumerable(Of Object)) As Task
            For Each item As Roms.GenericNDSRom In Targets
                Await item.ExtractFiles("extractNDStest")
            Next
        End Function
        Public Sub New()
            MyBase.New("Testing/Extract NDS")
        End Sub
    End Class

End Namespace
