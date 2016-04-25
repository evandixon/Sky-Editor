Namespace FileFormats.PSMD
    Public Class FlowData
        Inherits Sir0

        Public Property Strings As List(Of String)

        Public Property Data1 As List(Of ULong)
        Public Property Data2 As List(Of ULong)

        Public Overrides Sub OpenFile(Filename As String)
            MyBase.OpenFile(Filename)

            Dim numEntries1 As UInteger = Me.UInt32(&H18)
            Dim entryPtr1 As UInteger = Me.UInt32(&H1C)
            Dim numEntries2 As UInteger = Me.UInt32(&H20)
            Dim entryPtr2 As UInteger = Me.UInt32(&H24)

            For count As UInteger = 0 To numEntries1 - 1
                Dim data As New List(Of Byte)
                Dim len = Me.UInt32(entryPtr1 + count * 8)
                Dim ptr = Me.UInt32(entryPtr1 + count * 8 + 4)
                For i As UInteger = 0 To len - 1
                    data.Add(Me.RawData(ptr + i))
                Next
                While data.Count < 8
                    data.Add(0)
                End While
                Data1.Add(BitConverter.ToUInt64(data.ToArray, 0))
            Next

            For count As UInteger = 0 To numEntries2 - 1
                Dim data As New List(Of Byte)
                Dim len = Me.UInt32(entryPtr2 + count * 8)
                Dim ptr = Me.UInt32(entryPtr2 + count * 8 + 4)
                For i As UInteger = 0 To len - 1
                    data.Add(Me.RawData(ptr + i))
                Next
                While data.Count < 8
                    data.Add(0)
                End While
                Data2.Add(BitConverter.ToUInt64(data.ToArray, 0))
            Next

            'Debug
            Dim d1 As New List(Of String)
            Dim d2 As New List(Of String)
            For Each item In Data1
                d1.Add(Conversion.Hex(item))
            Next
            For Each item In Data2
                d2.Add(Conversion.Hex(item))
            Next
            Dim s1 As New Text.StringBuilder
            For Each item In d1
                s1.AppendLine(item)
            Next
            Dim s2 As New Text.StringBuilder
            For Each item In d2
                s2.AppendLine(item)
            Next
            Console.WriteLine()
        End Sub

        Public Sub New()
            MyBase.New
            Data1 = New List(Of ULong)
            Data2 = New List(Of ULong)
            Me.ResizeFileOnLoad = False
        End Sub
    End Class

End Namespace
