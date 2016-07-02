Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Utilities

Public Class TextFile
    Implements ICreatableFile
    Implements IOpenableFile
    Implements IOnDisk
    Implements ISavableAs
    Implements INamed
    Implements INotifyModified
    Implements ITextFile
    Implements IContainer(Of String)

    Public Sub New()
        Text = ""
    End Sub

    Public Property Filename As String Implements IOnDisk.Filename

    Public ReadOnly Property Name As String Implements INamed.Name
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

    Public Event FileSaved As ISavable.FileSavedEventHandler Implements ISavable.FileSaved
    Public Event Modified As INotifyModified.ModifiedEventHandler Implements INotifyModified.Modified

    Public Sub CreateFile(Name As String) Implements ICreatableFile.CreateFile
        Text = ""
        _name = Name
    End Sub

    Public Function OpenFile(Filename As String, Provider As IOProvider) As Task Implements IOpenableFile.OpenFile
        Text = Provider.ReadAllText(Filename)
        Me.Filename = Filename
        Return Task.CompletedTask
    End Function

    Public Sub Save(provider As IOProvider) Implements ISavable.Save
        provider.WriteAllText(Me.Filename, Text)
        RaiseEvent FileSaved(Me, New EventArgs)
    End Sub

    Public Sub Save(Filename As String, provider As IOProvider) Implements ISavableAs.Save
        provider.WriteAllText(Filename, Text)
        RaiseEvent FileSaved(Me, New EventArgs)
    End Sub

    Public Function GetDefaultExtension() As String Implements ISavableAs.GetDefaultExtension
        Return ".txt"
    End Function
End Class
