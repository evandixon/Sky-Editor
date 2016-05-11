Imports SkyEditor.Core.IO

Namespace Roms
    Public Class Cxi3DSRom
        Inherits Generic3DSRom
        Public Overrides Function IsOfType(File As GenericFile) As Task(Of Boolean)
            If File.Length > 104 Then
                Dim e As New System.Text.ASCIIEncoding
                Return Task.FromResult(e.GetString(File.RawData(&H100, 4)) = "NCCH")
            Else
                Return Task.FromResult(False)
            End If
        End Function

        Public Overrides ReadOnly Property TitleID As String
            Get
                Return Conversion.Hex(BitConverter.ToUInt64(RawData(&H118, 8), 0)).PadLeft(16, "0"c)
            End Get
        End Property

        Public Sub New()
            MyBase.New
        End Sub

    End Class
End Namespace