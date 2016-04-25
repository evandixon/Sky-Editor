Namespace FileFormats.Explorers.Script
    Public Class CommandDefinition
        Public Property CommandID As Integer

        ''' <summary>
        ''' The length in 16 bit words of the command's parameters.
        ''' </summary>
        ''' <returns></returns>
        Public Property Length As Integer

        Public Property CommandName As String
            Get
                If _commandName Is Nothing Then
                    Return "raw." & Conversion.Hex(CommandID).PadLeft(4, "0"c)
                Else
                    Return _commandName
                End If
            End Get
            Set(value As String)
                _commandName = value
            End Set
        End Property
        Dim _commandName As String

        Public Overridable Function GetScript(File As SSB, Params As List(Of UInt16)) As String
            Dim out As New Text.StringBuilder
            out.Append(CommandName)
            out.Append("(")
            If Params.Count > 0 Then
                out.Append(Conversion.Hex(Params(0)).PadLeft(4, "0"c))
            End If
            For count = 1 To Params.Count - 1
                out.Append(",0x")
                out.Append(Conversion.Hex(Params(count)).PadLeft(4, "0"c))
            Next
            out.Append(")")
            Return out.ToString
        End Function

        Public Sub New(CommandID As Integer, ParamLength As Integer)
            Me.CommandID = CommandID
            Me.Length = ParamLength
        End Sub
        Public Sub New(CommandID As Integer, ParamLength As Integer, CommandName As String)
            Me.CommandID = CommandID
            Me.Length = ParamLength
            Me.CommandName = CommandName
        End Sub
    End Class

End Namespace
