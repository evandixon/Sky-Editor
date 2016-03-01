Namespace FileFormats.ScriptDS
    Public Class DecimalParamCommandDefinition
        Inherits CommandDefinition
        Public Overrides Function GetScript(File As SSB, Params As List(Of UInt16)) As String
            Dim out As New Text.StringBuilder
            out.Append(CommandName)
            out.Append("(")
            If Params.Count > 0 Then
                out.Append(Params(0))
            End If
            For count = 1 To Params.Count - 1
                out.Append(",")
                out.Append(Params(count))
            Next
            out.Append(")")
            Return out.ToString
        End Function
        Public Sub New(CommandID As Integer, NumParams As Integer, CommandName As String)
            MyBase.New(CommandID, NumParams, CommandName)
        End Sub
    End Class
End Namespace

