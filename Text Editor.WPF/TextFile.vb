Imports SkyEditorBase
Imports SkyEditorBase.Interfaces
Imports Text_Editor

Public Class TextFile
    Implements iCreatableFile
    Implements iOpenableFile
    Implements iOnDisk
    Implements iGenericFile
    Implements iSavable
    Implements iNamed
    Implements iModifiable
    Implements iTextFile

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

    Public Property Text As String Implements iTextFile.Text

    Dim _name As String

    Public Event FileSaved As iSavable.FileSavedEventHandler Implements iSavable.FileSaved
    Public Event Modified As iModifiable.ModifiedEventHandler Implements iModifiable.Modified

    Public Sub CreateFile(Name As String) Implements iCreatableFile.CreateFile
        Text = ""
        _name = Name
    End Sub

    Public Sub OpenFile(Filename As String) Implements iOpenableFile.OpenFile
        Text = IO.File.ReadAllText(Filename)
        Me.Filename = Filename
    End Sub

    Public Sub RaiseModified() Implements iModifiable.RaiseModified
        RaiseEvent Modified(Me, New EventArgs)
    End Sub

    Public Sub Save() Implements iSavable.Save
        IO.File.WriteAllText(Me.Filename, Text)
        RaiseEvent FileSaved(Me, New EventArgs)
    End Sub

    Public Sub Save(Filename As String) Implements iSavable.Save
        IO.File.WriteAllText(Filename, Text)
        RaiseEvent FileSaved(Me, New EventArgs)
    End Sub

    Public Function DefaultExtension() As String Implements iSavable.DefaultExtension
        Return ".txt"
    End Function
End Class
