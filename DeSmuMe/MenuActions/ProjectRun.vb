Imports SkyEditorBase

Namespace MenuActions
    Public Class ProjectRun
        Inherits MenuAction

        Public Overrides Function SupportedTypes() As IEnumerable(Of Type)
            Return {GetType(ROMEditor.Projects.DSModPackProject)}
        End Function

        Public Overrides Function SupportsObject(Obj As Object) As Boolean
            Return TypeOf Obj Is ROMEditor.Projects.DSModPackProject AndAlso IO.File.Exists(IO.Path.Combine(DirectCast(Obj, ROMEditor.Projects.DSModPackProject).OutputDir, "PatchedRom.nds"))
        End Function

        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            For Each item As ROMEditor.Projects.DSModPackProject In Targets
                Dim romPath As String = IO.Path.Combine(item.OutputDir, "PatchedRom.nds")
                If IO.File.Exists(romPath) Then
                    DeSmuMe.RunDeSmuMe(romPath)
                End If
            Next
            Return Task.CompletedTask
        End Function

        Public Sub New()
            MyBase.New({PluginHelper.GetLanguageItem("_Project"), PluginHelper.GetLanguageItem("_Run")})
            SortOrder = 2.2
        End Sub
    End Class
End Namespace

