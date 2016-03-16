Imports SkyEditorBase.Interfaces

Namespace Saves
    Public Class GTISave
        Inherits SdfSave
        Implements iContainer(Of GTIGameData)

        Public Property GameData As GTIGameData Implements iContainer(Of GTIGameData).Item

        Public Sub New()
            MyBase.New
        End Sub

        Public Overrides Sub OpenFile(SdfDirectory As String)
            MyBase.OpenFile(SdfDirectory)

            GameData = New GTIGameData
            GameData.OpenFile(Me.GetFilePath("game_data"))
        End Sub
    End Class

End Namespace
