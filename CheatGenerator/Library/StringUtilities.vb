Imports SkyEditorBase

Public Class StringUtilities
    Public Shared Function StringToPMDEncoding(Input As String) As Byte()
        Dim out As New List(Of Byte)
        For Each c As Char In Input
            out.Add(Lists.StringEncodingInverse(c))
        Next
        Return out.ToArray
    End Function
    Public Shared Function PMDEncodingToString(Input As Byte()) As String
        Dim out As String = ""
        For Each b In Input
            If b > 0 Then
                If Lists.StringEncoding.Keys.Contains(b) Then
                    out = out & Lists.StringEncoding(b)
                Else
                    out = out & "[" & b.ToString & "]"
                End If
            Else
                Exit For
            End If
        Next
        Return out
    End Function
End Class