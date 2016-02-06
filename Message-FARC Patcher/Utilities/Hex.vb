Namespace Utilities
    Public Class Hex
        ''' <summary>
        ''' Returns whether or not the given Input is a hex string.
        ''' </summary>
        ''' <param name="Input"></param>
        ''' <returns></returns>
        Public Shared Function IsHex(Input As String) As Boolean
            Dim output As Boolean = True
            For Each item In Input
                Dim upper = item.ToString.ToUpper
                If Not (IsNumeric(item) OrElse upper = "A" OrElse upper = "B" OrElse upper = "C" OrElse upper = "D" OrElse upper = "E" OrElse upper = "F") Then
                    output = False
                    Exit For
                End If
            Next
            Return output
        End Function
    End Class
End Namespace