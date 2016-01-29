Imports SkyEditorBase

Namespace Roms
    Public Class Cxi3DSRom
        Inherits Generic3DSRom
        Public Overrides Function IsOfType(File As GenericFile) As Boolean
            If File.Length > 104 Then
                Dim e As New System.Text.ASCIIEncoding
                Return e.GetString(File.RawData(&H100, 4)) = "NCCH"
            Else
                Return False
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

        Public Sub New(Filename As String)
            MyBase.New(Filename)
        End Sub
    End Class
End Namespace