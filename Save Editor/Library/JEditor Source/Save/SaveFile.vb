Imports SkyEditor.skyjed.util
Imports SkyEditorBase
Imports SkyEditorBase.Utilities

Namespace skyjed.save

    Public Class SaveFile

        Public Const MIN_FILE_LENGTH As Integer = 128 * 1024
        Public Const SLOT_LENGTH As Integer = 46684
        Public Const SLOT2_OFFSET As Integer = 51200
        Public Const QSLOT_OFFSET As Integer = 102400
        Public Const QSLOT_LENGTH As Integer = 22528

        Private data() As Byte
        Public slot1 As SaveSlot
        'public SaveSlot slot2; //TODO split slots
        Public qslot As QuickSaveSlot

        Public Sub New(ByVal data() As Byte)
            Me.data = data
            slot1 = New SaveSlot(GenericArrayOperations(Of Byte).CopyOfRange(data, 0, SLOT_LENGTH))
            'slot2 = new SaveSlot(ArrayUtils.copyFrom(data, SLOT2_OFFSET, SLOT_LENGTH)); //TODO split slots

            Dim qdata() As Byte = GenericArrayOperations(Of Byte).CopyOfRange(data, QSLOT_OFFSET, QSLOT_OFFSET + QSLOT_LENGTH)
            'If QuickSaveSlot.checkMagic(qdata) Then
            qslot = New QuickSaveSlot(qdata)
            'Else
            '    qslot = Nothing
            'End If

        End Sub

        Public Function toByteA() As Byte()
            ArrayUtils.copyInto(data, slot1.toByteA(), 0)
            ArrayUtils.copyInto(data, slot1.toByteA(), SLOT2_OFFSET) 'TODO split slots
            'ArrayUtils.copyInto(data, slot2.toByteA(), SLOT2_OFFSET); //TODO split slots
            If qslot IsNot Nothing Then
                ArrayUtils.copyInto(data, qslot.toByteA(), QSLOT_OFFSET)
            End If
            Return data
        End Function

    End Class

End Namespace