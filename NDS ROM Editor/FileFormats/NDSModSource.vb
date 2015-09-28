Imports System.Text
Imports SkyEditorBase

Namespace FileFormats
    Public Class NDSModSourceContainer
        Public Property ModName As String
        Public Property Author As String
        Public Property Description As String
        Public Property UpdateURL As String
        Public Property DependenciesBefore As List(Of String)
        Public Property DependenciesAfter As List(Of String)
        Public Property Version As String
    End Class
    Public Class NDSModSource
        Inherits ObjectFile(Of NDSModSourceContainer)
        Public Sub New(Filename As String)
            MyBase.New(Filename)
        End Sub
        Public Sub New()
            MyBase.New
        End Sub
        Public Overrides Function DefaultExtension() As String
            Return ".ndsmodsrc"
        End Function
    End Class
    Public Class KaomadoFixNDSMod
        Inherits NDSModSource
        Public Sub New()
            MyBase.New()
        End Sub
        Public Sub New(Filename As String)
            MyBase.New(Filename)
        End Sub
    End Class
End Namespace
