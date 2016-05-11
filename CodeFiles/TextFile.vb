Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.IO
Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Public Class TextFile
    Implements iCreatableFile
    Implements IOpenableFile
    Implements iOnDisk
    Implements iGenericFile
    Implements ISavableAs
    Implements iNamed
    Implements iModifiable
    Implements ITextFile
    Implements IContainer(Of String)

    Public Sub New()
        Text = ""
    End Sub

    Public Property Filename As String Implements iOnDisk.Filename

    Public ReadOnly Property Name As String Implements iNamed.Name
        Get
            If Filename Is Nothing Then
                Return _name
            Else
                Return IO.Path.GetFileName(Filename)
            End If
        End Get
    End Property

    Public Property Text As String Implements ITextFile.Text, IContainer(Of String).Item

    Dim _name As String

    Public Event FileSaved As iSavable.FileSavedEventHandler Implements iSavable.FileSaved
    Public Event Modified As iModifiable.ModifiedEventHandler Implements iModifiable.Modified

    Public Sub CreateFile(Name As String) Implements iCreatableFile.CreateFile
        Text = ""
        _name = Name
    End Sub

    Public Function OpenFile(Filename As String, Provider As IOProvider) As Task Implements IOpenableFile.OpenFile
        Text = Provider.ReadAllText(Filename)
        Me.Filename = Filename
        Return Task.CompletedTask
    End Function

    Public Sub RaiseModified() Implements iModifiable.RaiseModified
        RaiseEvent Modified(Me, New EventArgs)
    End Sub

    Public Sub Save() Implements iSavable.Save
        IO.File.WriteAllText(Me.Filename, Text)
        RaiseEvent FileSaved(Me, New EventArgs)
    End Sub

    Public Sub Save(Filename As String) Implements ISavableAs.Save
        IO.File.WriteAllText(Filename, Text)
        RaiseEvent FileSaved(Me, New EventArgs)
    End Sub

    Public Function GetDefaultExtension() As String Implements ISavableAs.GetDefaultExtension
        Return ".txt"
    End Function
End Class
