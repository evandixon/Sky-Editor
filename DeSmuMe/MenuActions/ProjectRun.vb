Imports System.IO
Imports System.Reflection
Imports ROMEditor.Projects
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI

Namespace MenuActions
    Public Class ProjectRun
        Inherits MenuAction

        Public Overrides Function SupportedTypes() As IEnumerable(Of TypeInfo)
            Return {GetType(SolutionNode).GetTypeInfo}
        End Function

        Public Overrides Function SupportsObject(Obj As Object) As Boolean
            Dim out = False
            If TypeOf Obj Is SolutionNode Then
                Dim node = DirectCast(Obj, SolutionNode)
                If Not node.IsDirectory Then
                    Dim proj = DirectCast(node.Project, Project)
                    If TypeOf proj Is DSModPackProject Then
                        Dim mp = DirectCast(proj, DSModPackProject)
                        If CurrentPluginManager.CurrentIOProvider.FileExists(Path.Combine(mp.OutputDir, "PatchedRom.nds")) Then
                            out = True
                        End If
                    End If
                End If
            End If
            Return out
        End Function

        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            For Each item As SolutionNode In Targets
                If SupportsObject(item) Then 'Checks again for type validity and existance of our target file
                    Dim mp = DirectCast(item.Project, DSModPackProject)
                    Dim romPath As String = IO.Path.Combine(mp.OutputDir, "PatchedRom.nds")
                    DeSmuMe.RunDeSmuMe(romPath)
                End If
            Next
            Return Task.CompletedTask
        End Function

        Public Sub New()
            MyBase.New({My.Resources.Language.MenuProjectRun})
            Me.IsContextBased = True
            SortOrder = 2.2
        End Sub
    End Class
End Namespace

