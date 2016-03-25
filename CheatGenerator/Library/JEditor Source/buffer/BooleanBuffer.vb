Imports SkyEditor.skyjed.util

Namespace skyjed.buffer

    Public MustInherit Class BooleanBuffer

        Protected Friend __position As Integer
        Protected Friend __size As Integer

        Protected Friend Sub New(ByVal size As Integer)
            __position = 0
            __size = size
        End Sub

        Public Overridable Function position() As Integer
            Return __position
        End Function

        Public Overridable Function size() As Integer
            Return __size
        End Function

        Public Overridable Function remaining() As Integer
            Return __size - __position
        End Function

        Public Overridable Function reset() As BooleanBuffer
            __position = 0
            Return Me
        End Function

        Public Overridable Function seek(ByVal new_position As Integer) As BooleanBuffer
            __position = new_position
            Return Me
        End Function

        Public Overridable Function skip(ByVal length As Integer) As BooleanBuffer
            __position += length
            Return Me
        End Function

        '/ returns view of this buffer from current position and advances position
        Public Overridable Function view(ByVal length As Integer) As BooleanBuffer
            Dim ret As BooleanBuffer = New BooleanBufferView(Me, __position, length)
            __position += length
            Return ret
        End Function

        Public Overridable Function [get]() As Boolean
            If remaining() = 0 Then
                Throw New BufferUnderflowException()
            End If
            Dim tempVar As Integer = __position
            __position += 1
            Return aget(tempVar)
        End Function

        Public Overridable Function [get](ByVal length As Integer) As Boolean()
            If remaining() < length Then
                Throw New BufferUnderflowException()
            End If
            Dim ret(length - 1) As Boolean
            For i As Integer = 0 To length - 1
                ret(i) = [get]()
            Next i
            Return ret
        End Function

        Public Overridable ReadOnly Property [Byte] As Byte
            Get
                Return BitConverterLE.packBits([get](8))(0)
            End Get
        End Property

        Public Overridable Function getBytes(ByVal length As Integer) As Byte()
            Return BitConverterLE.packBits([get](length * 8))
        End Function

        Public Overridable Function getInt(ByVal num_bits As Integer) As Integer
            Return BitConverterLE.packBitsInt([get](num_bits))
        End Function

        Public Overridable Function getInts(ByVal length As Integer, ByVal num_bits As Integer) As Integer()
            Return BitConverterLE.packBitsInt([get](length * num_bits), num_bits)
        End Function

        Public Overridable Function getLong(ByVal num_bits As Integer) As Long
            Return BitConverterLE.packBitsLong([get](num_bits))
        End Function

        Public Overridable Function getLongs(ByVal length As Integer, ByVal num_bits As Integer) As Long()
            Return BitConverterLE.packBitsLong([get](length * num_bits), num_bits)
        End Function

        Public Overridable Sub put(ByVal b As Boolean)
            If remaining() = 0 Then
                Throw New System.OverflowException()
            End If
            aput(__position, b)
            __position += 1
        End Sub

        Public Overridable Sub put(ByVal src() As Boolean)
            If remaining() < src.Length Then
                Throw New System.OverflowException()
            End If
            For i As Integer = 0 To src.Length - 1
                put(src(i))
            Next i
        End Sub

        Public Overridable Sub putByte(ByVal val As Byte)
            Dim buf(0) As Byte
            buf(0) = val
            put(BitConverterLE.splitBits(buf))
        End Sub

        Public Overridable Sub putBytes(ByVal vals() As Byte)
            put(BitConverterLE.splitBits(vals))
        End Sub

        Public Overridable Sub putInt(ByVal val As Integer, ByVal num_bits As Integer)
            put(BitConverterLE.splitBitsInt(val, num_bits))
        End Sub

        Public Overridable Sub putInts(ByVal vals() As Integer, ByVal num_bits As Integer)
            put(BitConverterLE.splitBitsInt(vals, num_bits))
        End Sub

        Public Overridable Sub putLong(ByVal val As Long, ByVal num_bits As Integer)
            put(BitConverterLE.splitBitsLong(val, num_bits))
        End Sub

        Public Overridable Sub putLongs(ByVal vals() As Long, ByVal num_bits As Integer)
            put(BitConverterLE.splitBitsLong(vals, num_bits))
        End Sub

        Protected Friend MustOverride Function aget(ByVal index As Integer) As Boolean
        Protected Friend MustOverride Sub aput(ByVal index As Integer, ByVal b As Boolean)

    End Class

End Namespace