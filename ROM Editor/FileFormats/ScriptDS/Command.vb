Namespace FileFormats.ScriptDS
    Public Class Command
        Public Property CommandID As UInt16
        Public Property Params As List(Of UInt16)
        Public Function GetScript(CommandDefinitions As Dictionary(Of Integer, CommandDefinition), File As SSB) As String
            Dim d = CommandDefinitions(CommandID)
            Return d.GetScript(File, Params)
        End Function
        Public Function GetBytes() As List(Of Byte)
            Dim out As New List(Of Byte)

            out.AddRange(BitConverter.GetBytes(CommandID))
            For Each item In Params
                out.AddRange(BitConverter.GetBytes(item))
            Next

            Return out
        End Function
        Public Sub New(RawData As Byte())
            CommandID = BitConverter.ToUInt16(RawData, 0)
            Dim p As New List(Of UInt16)
            For count = 2 To RawData.Length - 1 Step 2
                p.Add(BitConverter.ToUInt16(RawData, count))
            Next
            Params = p
        End Sub
    End Class

End Namespace
