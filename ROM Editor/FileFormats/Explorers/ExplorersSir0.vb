Namespace FileFormats.Explorers
    ''' <summary>
    ''' Variant of the SIR0 container that uses 0xAA as the block padding byte.
    ''' </summary>
    Public Class ExplorersSir0
        Inherits Sir0

        Public Sub New()
            MyBase.New
            PaddingByte = &HAA
        End Sub
    End Class

End Namespace
