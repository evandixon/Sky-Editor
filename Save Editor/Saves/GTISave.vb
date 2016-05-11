Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.IO
Imports SkyEditorBase.Interfaces

Namespace Saves
    Public Class GTISave
        Inherits SdfSave
        Implements IContainer(Of GTIGameData)

        Public Const GTIMiniTitleID As String = "00000ba8"

        Public Property GameData As GTIGameData Implements IContainer(Of GTIGameData).Item

        Public Sub New()
            MyBase.New
        End Sub

        Public Overrides Async Function OpenFile(SdfDirectory As String, Provider As IOProvider) As Task
            Await MyBase.OpenFile(SdfDirectory, Provider)

            GameData = New GTIGameData
            Await GameData.OpenFile(Me.GetFilePath("game_data"), Provider)
        End Function
    End Class

End Namespace
