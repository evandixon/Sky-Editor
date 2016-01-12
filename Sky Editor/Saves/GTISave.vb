Imports SkyEditorBase.Interfaces

Namespace Saves
    Public Class GTISave
        Inherits SdfSave
        Implements iContainer(Of GatesGameData)

        Public Property GameData As GatesGameData Implements iContainer(Of GatesGameData).Item

        Public Sub New()
            MyBase.New
        End Sub

        Public Overrides Sub OpenFile(SdfDirectory As String)
            MyBase.OpenFile(SdfDirectory)

            GameData = New GatesGameData(Me.GetFilePath("game_data"))
        End Sub
    End Class

End Namespace
