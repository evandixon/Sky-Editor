Imports System
Imports SkyEditor.skyjed.buffer
Imports SkyEditor.skyjed.util
Imports SkyEditorBase

Namespace skyjed.save


    Public Class QuickSaveSlot

        Private Const MAGIC_OFFSET As Integer = &H20
        Private Const MAGIC_BYTELENGTH As Integer = 8
        Private Shared ReadOnly MAGIC_DATA() As Byte = {&H82, &HDD, &H82, &HB3, &H82, &HAB, &H97, &H6C}
        'Private Shared ReadOnly Property MAGIC_DATA As Byte()
        '    Get
        '        Return StringUtilities.StringToPMDEncoding("POKE_DUN_SORA")
        '    End Get
        'End Property

        Private Shared ReadOnly POKEMON_Q_STORAGE_OFFSET As Integer = &H3170 * 8
        Private Const POKEMON_Q_STORAGE_LENGTH As Integer = 20 * 8 * 429

        ' private (public for debugging)
        Public split_data() As Boolean
        Public buf As BooleanBuffer

        Public pkmnQStorage As PkmnQStorage

        Public Sub New(ByVal data() As Byte)
            'If calcChecksum(data) <> BitConverterLE.readInt32(data, 0) Then
            '    Console.Error.WriteLine("WARNING: checksum mismatch")
            'End If
            split_data = BitConverterLE.splitBits(data)
            buf = New BooleanBufferArray(split_data)
            data = Nothing ' do not use later

            If Not Array.Equals(MAGIC_DATA, buf.seek(MAGIC_OFFSET).getBytes(MAGIC_BYTELENGTH)) Then
                Console.Error.WriteLine("WARNING: invalid magic")
            End If

            pkmnQStorage = New PkmnQStorage(buf.seek(POKEMON_Q_STORAGE_OFFSET).view(POKEMON_Q_STORAGE_LENGTH))
        End Sub

        Public Overridable Function toByteA() As Byte()
            buf.seek(MAGIC_OFFSET).putBytes(MAGIC_DATA)

            pkmnQStorage.store(buf.seek(POKEMON_Q_STORAGE_OFFSET).view(POKEMON_Q_STORAGE_LENGTH))

            Dim data() As Byte = BitConverterLE.packBits(split_data)
            'Sky Editor will handle this...  BitConverterLE.writeInt32(calcChecksum(data), data, 0)
            Return data
        End Function

        Private Shared Function calcChecksum(ByVal data() As Byte) As Integer
            Dim chksum As Integer = 0
            For i As Integer = 4 To data.Length - 1 Step 4
                chksum += BitConverterLE.readInt32(data, i)
            Next i
            Return chksum
        End Function

        Public Shared Function checkMagic(ByVal data() As Byte) As Boolean
            Return Array.Equals(MAGIC_DATA, GenericArrayOperations(Of Byte).CopyOfRange(data, MAGIC_OFFSET \ 8, MAGIC_OFFSET \ 8 + MAGIC_BYTELENGTH)) ' offset is in bits we need bytes here
        End Function

    End Class

End Namespace