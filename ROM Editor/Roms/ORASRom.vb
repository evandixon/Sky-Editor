Namespace Roms
    Public Class ORASRom
        Inherits Generic3DSRom
        Public Sub New()
            MyBase.New(False)
        End Sub

        Public Sub New(Filename As String)
            MyBase.New(Filename)
        End Sub
    End Class
End Namespace


