Imports SkyEditorBase

Public Class ThreeDSSave
    Inherits GenericSave
    Public Function FindRepeatingCTRXorPad() As Byte()
        Throw New NotImplementedException
        'Dim chunks As New List(Of Byte())
        'For count = 0 To RawData.Length - 1 Step 512
        '    If RawData.Length - count >= 512 Then
        '        chunks.Add(SkyEditorBase.Utilities.GenericArrayOperations(Of Byte).CopyOfRange(Me.RawData, count, count + 512))
        '    End If
        'Next
    End Function
    Public Sub Decrypt(XORPad As IEnumerable(Of Byte))
        Dim decrypted(Length - 1) As Byte
        For count = 0 To decrypted.Length - 1
            decrypted(count) = RawData(count) Xor XORPad(count)
        Next
        Me.RawData(0, Length) = decrypted
    End Sub
    Public Overrides Function DefaultSaveID() As String
        Return GameConstants.ThreeDSSave
    End Function
    Public Sub New(Filename As String)
        Dim newName = PluginHelper.GetResourceName(IO.Path.Combine("Saves", IO.Path.GetFileName(Filename)))
        If Not IO.Directory.Exists(IO.Path.GetDirectoryName(newName)) Then
            IO.Directory.CreateDirectory(newName)
        End If
        IO.File.Copy(Filename, newName)
        Me.Filename = newName
        RawData(0, Length) = IO.File.ReadAllBytes(Filename)
    End Sub
End Class