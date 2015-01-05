Public MustInherit Class FileFormatControl
    Inherits Windows.Controls.UserControl

    ''' <summary>
    ''' Called when data should be read.
    ''' </summary>
    ''' <param name="Filename"></param>
    ''' <remarks></remarks>
    Public MustOverride Sub RefreshDisplay(Filename As String)
    ''' <summary>
    ''' Called when data should be saved.
    ''' </summary>
    ''' <remarks></remarks>
    Public MustOverride Sub UpdateFile()

End Class
