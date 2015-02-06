Namespace ARDS
    Public Class CBAHelper
        Public Class Line
            Dim _rawData As String
            Public Property RawData As String
                Get
                    Return _rawData.Trim.Substring(0, 13)
                End Get
                Set(value As String)
                    _rawData = value.Trim.Substring(0, 13)
                End Set
            End Property
            'Private Property Section(Index As Byte) As String
            '    Get
            '        Return RawData.Split(" ")(Index And 1)
            '    End Get
            '    Set(value As String)
            '        If Index = 0 Then
            '            _rawData = value.Trim.Substring(0, 8) & " " & RawData.Split(" ")(1)
            '        Else
            '            _rawData = RawData.Split(" ")(0) & " " & value.Trim.Substring(0, 8)
            '        End If
            '    End Set
            'End Property
            'Public Property Part1 As String
            '    Get
            '        Return Section(0)
            '    End Get
            '    Set(value As String)
            '        Section(1) = value
            '    End Set
            'End Property
            'Public Property Part2 As String
            '    Get
            '        Return Section(1)
            '    End Get
            '    Set(value As String)
            '        Section(1) = value
            '    End Set
            'End Property
            Public Shared Narrowing Operator CType(ByVal x As String) As Line
                Return New Line(x)
            End Operator
            Public Shared Widening Operator CType(ByVal x As Line) As String
                Return x.RawData
            End Operator
            Public Sub New(RawData As String)
                Me.RawData = RawData
            End Sub
            Public Overrides Function ToString() As String
                Return RawData
            End Function

            ''' <summary>
            ''' Gets the If Button Down line of an ARDS code.
            ''' </summary>
            ''' <param name="Buttons">AND combination of ARDS.DSButton</param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Shared Function IfButtonDown(Buttons As UInt16) As String
                Dim b As Byte() = BitConverter.GetBytes(Buttons)
                Return "74000130 " & Conversion.Hex(b(1)).PadLeft(2, "0") & Conversion.Hex(b(0)).PadLeft(2, "0")
            End Function

            ' ''' <summary>
            ' ''' Gets the If Button Not Down line of an ARDS code.
            ' ''' </summary>
            ' ''' <param name="Buttons">AND combination of ARDS.DSButton</param>
            ' ''' <returns></returns>
            ' ''' <remarks></remarks>
            'Public Shared Function IfNotButtonDown(Buttons As UInt16) As String
            '    Dim b As Byte() = BitConverter.GetBytes(Buttons)
            '    Return "A2FFFFA8 " & Conversion.Hex(b(1)).PadLeft(2, "0") & Conversion.Hex(b(0)).PadLeft(2, "0") & "0000"
            'End Function

            ' ''' <summary>
            ' ''' Gets the If Button Down line of an ARDS code using the GBA compatible offset.
            ' ''' X, Y, and NDS_Not_Folded not supported.
            ' ''' </summary>
            ' ''' <param name="Buttons">AND combination of ARDS.DSButton</param>
            ' ''' <returns></returns>
            ' ''' <remarks></remarks>
            'Public Shared Function IfGBAButtonDown(Buttons As UInt16) As String
            '    Dim b As Byte() = BitConverter.GetBytes(Buttons)
            '    Return "92FFFFA8 " & Conversion.Hex(b(1)).PadLeft(2, "0") & Conversion.Hex(b(0)).PadLeft(2, "0") & "0000"
            'End Function

            ' ''' <summary>
            ' ''' Gets the If Button Not Down line of an ARDS code using the GBA compatible offset.
            ' ''' X, Y, and NDS_Not_Folded not supported.
            ' ''' </summary>
            ' ''' <param name="Buttons">AND combination of ARDS.DSButton</param>
            ' ''' <returns></returns>
            ' ''' <remarks></remarks>
            'Public Shared Function IfGBAButtonNotDown(Buttons As UInt16) As String
            '    Dim b As Byte() = BitConverter.GetBytes(Buttons)
            '    Return "A2FFFFA8 " & Conversion.Hex(b(1)).PadLeft(2, "0") & Conversion.Hex(b(0)).PadLeft(2, "0") & "0000"
            'End Function
        End Class
        Public Class Code
            Inherits Generic.List(Of Line)
            Public Overrides Function ToString() As String
                Dim out As String = ""
                For Each Line In Me
                    out = out & Line.ToString & vbCrLf
                Next
                Return out.Trim
            End Function
            Public Sub New()
                MyBase.New()
            End Sub
            Public Sub New(Code As String)
                MyBase.New()
                Code = Code.Trim.Replace(" ", "").Replace(vbCr, "").Replace(vbLf, "")
                If Count Mod 8 = 0 Then
                    For Count As Integer = 0 To Code.Length - 1 Step 16
                        Me.Add(New Line(Code.Substring(Count, Count + 7) & " " & Code.Substring(Count + 8, Count + 15)))
                    Next
                End If
            End Sub
        End Class
    End Class
End Namespace