'Imports SkyCharsetRes = skyjed.resources.SkyCharsetRes

Namespace skyjed.util

    Public Class SkyCharConv

        Private Const NAME_LENGTH As Integer = 10
        Private Const EnableTrash As Boolean = False

        Public Shared Function decode(ByVal buf() As Byte) As String
            'Dim tmp(NAME_LENGTH - 1) As Char
            'For i As Integer = 0 To buf.Length - 1
            '    If buf(i) = 0 Then
            '        Return New String(ArrayUtils.copyOfRange(tmp, 0, i))
            '    End If
            '    Try
            '        tmp(i) = Lists.StringEncoding(buf(i)) 'SkyCharsetRes.byte2char(buf(i))
            '    Catch ex As Exception
            '        tmp(i) = "-"
            '    End Try
            'Next i
            'Return New String(tmp).Trim
            Dim tmp As New Text.StringBuilder
            For Each b In buf
                If (b = 0 AndAlso Not EnableTrash) Then
                    Return tmp.ToString
                ElseIf (b = 0 AndAlso EnableTrash) OrElse Not Lists.StringEncoding.ContainsKey(b) Then
                    tmp.Append("\00")
                ElseIf Lists.StringEncoding(b) = "\" Then
                    tmp.Append("\\")
                Else
                    tmp.Append(Lists.StringEncoding(b))
                End If
            Next
            Return tmp.ToString
        End Function

        Public Shared Function encode(ByVal str As String) As Byte()
            'Dim buf(NAME_LENGTH - 1) As Byte
            'For i As Integer = 0 To str.Length - 1
            '    If Lists.StringEncodingInverse.Keys.Contains(str.Chars(i)) Then
            '        buf(i) = Lists.StringEncodingInverse(str.Chars(i)) 'SkyCharsetRes.char2byte(str.Chars(i))
            '    End If
            'Next i
            'Return buf
            Dim buf(NAME_LENGTH - 1) As Byte

            Dim i As Integer = 0
            Dim j As Integer = 0
            While i < buf.Length AndAlso (i + j) < str.Length
                If str.Chars(i + j) = "\" Then
                    If str.Chars(i + j + 1) = "\" Then
                        buf(i) = Lists.StringEncodingInverse("\")
                        j += 1
                    Else
                        Dim raw As Byte = Convert.ToByte(str.Chars(i + j + 1) & str.Chars(i + j + 2), 16)
                        buf(i) = raw
                        j += 2
                    End If
                Else
                    buf(i) = Lists.StringEncodingInverse(str.Chars(i + j))
                End If
                i += 1
            End While

            Return buf
        End Function

    End Class

End Namespace