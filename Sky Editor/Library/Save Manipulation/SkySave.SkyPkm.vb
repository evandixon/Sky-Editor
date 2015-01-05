Imports SkyEditorBase

Partial Class SkySave
    Public Class SkyPkm
        Public Property RawData As Byte()
        Public ReadOnly Property sRawData As Byte()
            Get
                Dim out2(RawData.Length - 1) As Byte
                For x As Integer = 0 To out2.Length - 1
                    out2(x) = RawData(x)
                Next
                Return out2
            End Get
        End Property
        Public Sub New(RawData As Byte())
            Me.RawData = RawData
        End Sub
        Public Property ID As UInt16
            Get
                ' Dim idraw = BitOperations.ShiftRightPMD(RawData, 1, 1, 2)
                Dim out = (BitConverter.ToUInt16({RawData(1), RawData(2)}, 0) Or 63488) - 63488
                If out > 600 Then
                    Return out Mod 600
                Else
                    Return out
                End If
                'Return out
            End Get
            Set(value As UInt16)
                Dim originalBytes As Byte() = BitConverter.GetBytes((BitConverter.ToUInt16({RawData(1), RawData(2)}, 0) Or 63488) - 63488)
                If IsFemale AndAlso Not value > 600 Then value += 600
                Dim valueBytes As Byte() = BitConverter.GetBytes(value)
                RawData(2) = (RawData(2) Or 248) - 248
                RawData(2) = (RawData(2) Or valueBytes(1))
                RawData(1) = valueBytes(1)
            End Set
        End Property
        Public Property IsFemale As Boolean
            Get
                Return (ID > 600)
            End Get
            Set(value As Boolean)
                Dim idToWrite As UInt16 = ID
                Dim originalBytes As Byte() = BitConverter.GetBytes((BitConverter.ToUInt16({RawData(1), RawData(2)}, 0) Or 63488) - 63488)
                If value Then idToWrite += 600
                Dim valueBytes As Byte() = BitConverter.GetBytes(idToWrite)
                RawData(2) = (RawData(2) Or 248) - 248
                RawData(2) = (RawData(2) Or valueBytes(1))
                RawData(1) = valueBytes(1)
            End Set
        End Property
        Public Property Name As String
            Get
                Return EncodedString(35, 2, 10)
            End Get
            Set(value As String)
                EncodedString(35, 2, 10) = value
            End Set
        End Property
        Public Property Level As Byte
            Get
                Return RawData(0) >> 1
            End Get
            Set(value As Byte)
                RawData(0) = (Math.Min(value, 128) << 1)
            End Set
        End Property
        ''' <summary>
        ''' TODO: Move this elsewhere
        ''' </summary>
        ''' <param name="Offset"></param>
        ''' <param name="StartBit"></param>
        ''' <param name="StringLength"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property EncodedString(Offset As Integer, StartBit As Integer, StringLength As Integer) As String
            Get
                Dim out As String = ""
                Dim buffer As Byte() = BitOperations.ShiftRightPMD(GenericArrayOperations(Of Byte).CopyOfRange(_RawData, Offset, Offset + StringLength), StartBit, 0, StringLength)
                For Each b In buffer 'SubByteArr(_rawData, CurrentOffsets.TeamNameStart, CurrentOffsets.TeamNameStart + 7)
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
            End Get
            Set(value As String)
                Dim buffer As Byte() = StringUtilities.StringToPMDEncoding(value)
                Array.Resize(buffer, 10)
                Array.Reverse(buffer)
                Array.Resize(buffer, 11)
                Array.Reverse(buffer)
                buffer = GenericArrayOperations(Of Byte).CopyOfRange(buffer, StartBit, StringLength * 8)
                For x As Integer = 0 To 10
                    If buffer.Length > x Then
                        _RawData(Offset + x) = buffer(x)
                    Else
                        _RawData(Offset + x) = 0
                    End If
                Next
            End Set
        End Property
        Public Overrides Function ToString() As String
            If Lists.SkyPokemon.ContainsKey(ID) Then
                Return Name & " (Lvl. " & Level & " " & Lists.SkyPokemon(ID) & ")"
            Else
                Return Name & " (Lvl. " & Level & " Pokemon " & ID & ")"
            End If
        End Function
        Public ReadOnly Property IsValid As Boolean
            Get
                Return Not ID = 0
            End Get
        End Property
        Public Property MetAt As Byte
            Get
                Return BitConverter.GetBytes((BitConverter.ToUInt16({RawData(2), RawData(3)}, 0) >> 3))(0)
            End Get
            Set(value As Byte)
                Dim bytesToWrite As Byte() = BitConverter.GetBytes(value << 3)
                RawData(3) = ((RawData(3) Or 7) - 7) Or bytesToWrite(1)
                RawData(2) = ((RawData(2) Or 248) - 248) Or bytesToWrite(0)
            End Set
        End Property
        Public Property MetFloor As Byte
            Get
                Return BitConverter.GetBytes((BitConverter.ToUInt16({RawData(3), RawData(4)}, 0) >> 3))(0)
            End Get
            Set(value As Byte)
                Dim bytesToWrite As Byte() = BitConverter.GetBytes(value << 3)
                RawData(4) = ((RawData(4) Or 7) - 7) Or bytesToWrite(1)
                RawData(3) = ((RawData(3) Or 248) - 248) Or bytesToWrite(0)
            End Set
        End Property
        Public Property IQ As UInt16
            Get
                Return (((BitConverter.ToUInt32({RawData(5), RawData(6), RawData(7), 0}, 0) >> 2) Or &HFC0000) - &HFC0000) >> 7
            End Get
            Set(value As UInt16)
                'Dim bytesToWrite As Byte() = BitConverter.GetBytes(value << 3)
                'RawData(7) = ((RawData(7) Or 3) - 3) Or bytesToWrite(1)
                'RawData(6) = ((RawData(6) Or 252) - 252) Or bytesToWrite(0)
                'Dim originalBytes As Byte() = BitConverter.GetBytes((BitConverter.ToUInt16({RawData(1), RawData(2)}, 0) Or 64512) - 64512)
                'Dim valueBytes As Byte() = BitConverter.GetBytes(value << 3)
                'RawData(7) = (RawData(2) Or 252) - 252
                'RawData(7) = (RawData(2) Or valueBytes(1))
                'RawData(6) = valueBytes(1)
                Dim valueToWrite As Integer = value << 7
                valueToWrite = ((valueToWrite Or &HFC0000) - &HFC0000)
                valueToWrite = valueToWrite << 2
                Dim bytesToWrite As Byte() = BitConverter.GetBytes(valueToWrite)
                RawData(7) = bytesToWrite(2)
                RawData(6) = bytesToWrite(1)
                RawData(5) = bytesToWrite(0)
            End Set
        End Property
        Public Property MaxHP As UInt16
            Get
                Return (((BitConverter.ToUInt32({RawData(7), RawData(8), RawData(9), 0}, 0) >> 3) Or &H1FFC00) - &H1FFC00)
            End Get
            Set(value As UInt16)
                Dim valueToWrite As Integer = value
                valueToWrite = ((valueToWrite Or &H1FFC00) - &H1FFC00)
                valueToWrite = valueToWrite << 3
                Dim bytesToWrite As Byte() = BitConverter.GetBytes(valueToWrite)
                RawData(9) = bytesToWrite(2)
                RawData(8) = bytesToWrite(1)
                RawData(7) = bytesToWrite(0)
            End Set
        End Property
        Public Property Attack As Byte
            Get
                Return BitConverter.GetBytes(BitConverter.ToUInt16({RawData(8), RawData(9)}, 0) >> 5)(0)
            End Get
            Set(value As Byte)
                Dim bytesToWrite As Byte() = BitConverter.GetBytes(value << 5)
                RawData(9) = ((RawData(9) Or 224) - 224) Or bytesToWrite(1)
                RawData(8) = ((RawData(8) Or 31) - 31) Or bytesToWrite(0)
            End Set
        End Property
        Public Property SpAttack As Byte
            Get
                Return BitConverter.GetBytes(BitConverter.ToUInt16({RawData(9), RawData(10)}, 0) >> 5)(0)
            End Get
            Set(value As Byte)
                Dim bytesToWrite As Byte() = BitConverter.GetBytes(value << 5)
                RawData(10) = ((RawData(10) Or 224) - 224) Or bytesToWrite(1)
                RawData(9) = ((RawData(9) Or 31) - 31) Or bytesToWrite(0)
            End Set
        End Property
        Public Property Defense As Byte
            Get
                Return BitConverter.GetBytes(BitConverter.ToUInt16({RawData(10), RawData(11)}, 0) >> 5)(0)
            End Get
            Set(value As Byte)
                Dim bytesToWrite As Byte() = BitConverter.GetBytes(value << 5)
                RawData(11) = ((RawData(11) Or 224) - 224) Or bytesToWrite(1)
                RawData(10) = ((RawData(10) Or 31) - 31) Or bytesToWrite(0)
            End Set
        End Property
        Public Property SpDefense As Byte
            Get
                Return BitConverter.GetBytes(BitConverter.ToUInt16({RawData(11), RawData(12)}, 0) >> 5)(0)
            End Get
            Set(value As Byte)
                Dim bytesToWrite As Byte() = BitConverter.GetBytes(value << 5)
                RawData(12) = ((RawData(12) Or 224) - 224) Or bytesToWrite(1)
                RawData(11) = ((RawData(11) Or 31) - 31) Or bytesToWrite(0)
            End Set
        End Property
        Public Property Experience As UInt32
            Get
                Return ((BitConverter.ToUInt32({RawData(12), RawData(13), RawData(14), RawData(15)}, 0) >> 5) Or &HFF000000) - &HFF000000
            End Get
            Set(value As UInt32)
                Dim bytesToWrite As Byte() = BitConverter.GetBytes(value << 5)
                RawData(12) = ((RawData(12) Or 224) - 224) Or bytesToWrite(1)
                RawData(11) = ((RawData(11) Or 31) - 31) Or bytesToWrite(0)
            End Set
        End Property
        ''IQ Data: 73 bits
        Public Property Attack1 As SkyAttack
            Get
                Dim t = Bit8.FromBinary(RawData, 0, 213, 4)
                Dim out = BitOperations.ShiftRightPMD(BitConverter.GetBytes(BitConverter.ToUInt64({RawData(24), RawData(25), RawData(26), RawData(27), RawData(28), 0, 0, 0}, 0) << 6), 4, 1, 3)
                Return New SkyAttack(out)
            End Get
            Set(value As SkyAttack)
                Throw New NotImplementedException
            End Set
        End Property
        Public Property Attack2 As SkyAttack
            Get
                Return New SkyAttack(Bit8.FromBinary(RawData, 0, 194, 3))
                'Return New SkyAttack(BitOperations.ShiftRightPMD(BitConverter.GetBytes(BitConverter.ToUInt64({RawData(26), RawData(27), RawData(28), RawData(29), RawData(30), 0, 0, 0}, 0) << 6), 1, 1, 4))
            End Get
            Set(value As SkyAttack)
                Throw New NotImplementedException
            End Set
        End Property
        Public Function GetPkmFileData() As Byte()
            Return BitOperations.ShiftLeftPMD(RawData, 6, 0, RawData.Length)
        End Function
        Public Sub FromPkmFileData(Data As Byte())
            RawData = BitOperations.ShiftRightPMD(Data, 6, 0, Data.Length)
        End Sub
    End Class
    Public Class SkyAttack
        Public Property RawData As Byte()
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="RawData">Byte array that contains at least 21 bits of data.</param>
        ''' <remarks></remarks>
        Public Sub New(RawData As Byte())
            Me.RawData = RawData
        End Sub
    End Class
    Public Property Pokemon As List(Of SkyPkm)
        Get
            Dim out As New List(Of SkyPkm)
            For count As Integer = 0 To 720 - 1
                ' out.Add(New SkyPkm(Bit8.FromBinary(RawData, &H464, count * 362, 46)))
                out.Add(New SkyPkm(BitOperations.ShiftRightPMD(RawData, (count * 362) Mod 8, &H464 + ((count * 362) \ 8), 46)))
            Next
            Return out
        End Get
        Set(value As List(Of SkyPkm))
            For count As Integer = 0 To 720 - 1
                If value.Count > count Then
                    'Write pkm
                    Dim bytesToWrite As Byte() = BitOperations.ShiftLeftPMD(value(count).RawData, (count * 362) Mod 8, 0, 46)
                    For x As Byte = 0 To bytesToWrite.Length - 1
                        If x = 0 Then
                            RawData(&H464 + ((count * 362) \ 8) + x) = (((RawData(&H464 + ((count * 362) \ 8) + x) Or (2 ^ ((count * 362) Mod 8))) - (2 ^ ((count * 362) Mod 8)) Or bytesToWrite(x) << (count * 362) Mod 8))
                        Else
                            RawData(&H464 + ((count * 362) \ 8) + x) = bytesToWrite(x)
                        End If
                    Next
                Else
                    For x As Integer = 0 To 45
                        RawData(&H464 + ((count * 362) \ 8)) = 0
                    Next
                End If
            Next
        End Set
    End Property
End Class
