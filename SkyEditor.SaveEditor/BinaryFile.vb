Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.IO

Public Class BinaryFile
    Implements iCreatableFile
    Implements IOpenableFile
    Implements iNamed
    Implements iOnDisk
    Implements ISavableAs

    Public Sub New()
        Bits = New Binary(0)
    End Sub

    Public Overridable Function OpenFile(Filename As String, Provider As IOProvider) As Task Implements IOpenableFile.OpenFile
        Me.Filename = Filename
        Me.CurrentIOProvider = Provider
        Using f As New GenericFile(Provider, Filename, True, True)
            Bits = New Binary(0)
            ProcessRawData(f)
        End Using
        Return Task.FromResult(0)
    End Function

    Private Sub ProcessRawData(File As GenericFile)
        For count As Integer = 0 To File.Length - 1
            Bits.AppendByte(File.RawData(count))
        Next
    End Sub

    Public Property Bits As Binary
    Public Property Filename As String Implements IOnDisk.Filename
    Private Property CurrentIOProvider As IOProvider

    ''' <summary>
    ''' Name of the file.
    ''' </summary>
    ''' <returns></returns>
    Public Property Name As String Implements iNamed.Name
        Get
            If _name Is Nothing Then
                Return IO.Path.GetFileName(Filename)
            Else
                Return _name
            End If
        End Get
        Set(value As String)
            _name = value
        End Set
    End Property
    Dim _name As String

    Protected Overridable Sub FixChecksum()

    End Sub

    Public Sub Save(Destination As String, provider As IOProvider) Implements ISavableAs.Save
        FixChecksum()
        Dim tmp(Math.Ceiling(Bits.Count / 8) - 1) As Byte
        Using f As New GenericFile(provider, tmp)
            For count As Integer = 0 To Math.Ceiling(Bits.Count / 8) - 1
                f.RawData(count) = Bits.Int(count, 0, 8)
            Next
            f.Save(Destination, provider)
        End Using
        RaiseEvent FileSaved(Me, New EventArgs)
    End Sub

    Public Function GetDefaultExtension() As String Implements ISavableAs.GetDefaultExtension
        Return ".sav"
    End Function

    Public Event FileSaved As iSavable.FileSavedEventHandler Implements iSavable.FileSaved

    Public Sub Save(provider As IOProvider) Implements ISavable.Save
        Save(Filename, provider)
    End Sub

    Public Sub CreateFile(Name As String) Implements iCreatableFile.CreateFile
        Me.Name = ""
    End Sub
End Class
