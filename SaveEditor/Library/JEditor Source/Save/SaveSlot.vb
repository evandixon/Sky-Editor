Imports System
Imports SkyEditor.skyjed.buffer
Imports SkyEditor.skyjed.buffer.BooleanBuffer
Imports SkyEditor.skyjed.buffer.BooleanBufferArray
Imports SkyEditor.skyjed.util.BitConverterLE
Imports SkyEditor.skyjed.util.SkyCharConv
Imports SkyEditor.skyjed.util

'Imports SkyEditor.skyjed.util.SkyStringTrash

Namespace skyjed.save

    Public Class SaveSlot

        Private Const MAGIC_OFFSET As Integer = &H20
        Private Const MAGIC_BYTELENGTH As Integer = 13
        Private Shared ReadOnly Property MAGIC_DATA As Byte()
            Get
                Return StringUtilities.StringToPMDEncoding("POKE_DUN_SORA")
            End Get
        End Property
        Private Shared ReadOnly PKMN_STORAGE_OFFSET As Integer = &H464 * 8
        Private Const PKMN_STORAGE_LENGTH As Integer = 555 * 362
        Private Shared ReadOnly ACTIVE_PKMN_OFFSET As Integer = &H83D9 * 8 + 1
        Private Const ACTIVE_PKMN_LENGTH As Integer = 4 * 546
        Private Shared ReadOnly ACTIVE_PKMN_EP_OFFSET As Integer = &H84F4 * 8 + 2
        Private Const ACTIVE_PKMN_EP_LENGTH As Integer = 4 * 546
        Private Shared ReadOnly ADVENTURES_HAD_OFFSET As Integer = &H8B70 * 8
        Private Const ADVENTURES_HAD_LENGTH As Integer = 32
        Private Shared ReadOnly HELD_ITEM_STORAGE_OFFSET As Integer = &H8BA2 * 8
        Private Const HELD_ITEM_STORAGE_LENGTH As Integer = 50 * 33
        Private Shared ReadOnly HELD_ITEM_EP_STORAGE_OFFSET As Integer = &H8C70 * 8 + 2 'HELD_ITEM_STORAGE_OFFSET+HELD_ITEM_STORAGE_LENGTH
        Private Const HELD_ITEM_EP_STORAGE_LENGTH As Integer = 50 * 33
        Private Shared ReadOnly ITEM_STORAGE_OFFSET As Integer = &H8E0C * 8 + 6
        Private Const ITEM_STORAGE_LENGTH As Integer = 1000 * 2 * 11
        Private Shared ReadOnly ITEM_SHOP1_OFFSET As Integer = &H98CA * 8 + 6
        Private Const ITEM_SHOP1_LENGTH As Integer = 8 * 22
        Private Shared ReadOnly ITEM_SHOP2_OFFSET As Integer = &H98E0 * 8 + 6 'ITEM_SHOP1_OFFSET+ITEM_SHOP1_LENGTH
        Private Const ITEM_SHOP2_LENGTH As Integer = 4 * 22
        Private Shared ReadOnly HELD_MONEY_OFFSET As Integer = &H990C * 8 + 6
        Private Const HELD_MONEY_LENGTH As Integer = 17
        Private Shared ReadOnly HELD_MONEY_EP_OFFSET As Integer = &H990E * 8 + 7 'HELD_MONEY_OFFSET+HELD_MONEY_LENGTH
        Private Const HELD_MONEY_EP_LENGTH As Integer = 17 'HELD_MONEY_LENGTH
        Private Shared ReadOnly STORED_MONEY_OFFSET As Integer = &H9915 * 8 + 6
        Private Const STORED_MONEY_LENGTH As Integer = 24
        Private Shared ReadOnly TEAMNAME_OFFSET As Integer = &H994E * 8
        Private Const TEAMNAME_BYTELENGTH As Integer = 10
        Private Shared ReadOnly ADVENTURE_LOG_OFFSET As Integer = &H9958 * 8
        Private Const ADVENTURE_LOG_LENGTH As Integer = 447 ' TODO length
        Private Shared ReadOnly CROAGUNK_SHOP_OFFSET As Integer = &HB475 * 8
        Private Const CROAGUNK_SHOP_LENGTH As Integer = 8 * 11

        ' private (public for debugging)
        Public split_data() As Boolean
        Public buf As BooleanBuffer

        Public pkmnStorage As PkmnStorage
        Public activePkmn As ActivePkmn
        Public activePkmnEP As ActivePkmn
        Public adventuresHad As Integer ' signed
        Public heldItemStorage As SkyHeldItemStorage
        Public heldItemEPStorage As SkyHeldItemStorage
        Public itemStorage As ItemStorage
        Public itemShop1 As ItemShop
        Public itemShop2 As ItemShop
        Public heldMoney As Integer
        Public heldMoneyEP As Integer
        Public storedMoney As Integer
        Public teamName As String 'SkyStringTrash
        Public adventureLog As AdventureLog
        'Public croagunkShop As ItemShopNoParams

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

            pkmnStorage = New PkmnStorage(buf.seek(PKMN_STORAGE_OFFSET).view(PKMN_STORAGE_LENGTH))
            activePkmn = New ActivePkmn(buf.seek(ACTIVE_PKMN_OFFSET).view(ACTIVE_PKMN_LENGTH))
            activePkmnEP = New ActivePkmn(buf.seek(ACTIVE_PKMN_EP_OFFSET).view(ACTIVE_PKMN_EP_LENGTH))
            adventuresHad = buf.seek(ADVENTURES_HAD_OFFSET).getInt(ADVENTURES_HAD_LENGTH)
            heldItemStorage = New SkyHeldItemStorage(buf.seek(HELD_ITEM_STORAGE_OFFSET).view(HELD_ITEM_STORAGE_LENGTH))
            heldItemEPStorage = New SkyHeldItemStorage(buf.seek(HELD_ITEM_EP_STORAGE_OFFSET).view(HELD_ITEM_EP_STORAGE_LENGTH))
            itemStorage = New ItemStorage(buf.seek(ITEM_STORAGE_OFFSET).view(ITEM_STORAGE_LENGTH))
            itemShop1 = New ItemShop(buf.seek(ITEM_SHOP1_OFFSET).view(ITEM_SHOP1_LENGTH))
            itemShop2 = New ItemShop(buf.seek(ITEM_SHOP2_OFFSET).view(ITEM_SHOP2_LENGTH))
            heldMoney = buf.seek(HELD_MONEY_OFFSET).getInt(HELD_MONEY_LENGTH)
            heldMoneyEP = buf.seek(HELD_MONEY_EP_OFFSET).getInt(HELD_MONEY_EP_LENGTH)
            storedMoney = buf.seek(STORED_MONEY_OFFSET).getInt(STORED_MONEY_LENGTH)
            teamName = SkyCharConv.decode(buf.seek(TEAMNAME_OFFSET).getBytes(TEAMNAME_BYTELENGTH))
            adventureLog = New AdventureLog(buf.seek(ADVENTURE_LOG_OFFSET).view(ADVENTURE_LOG_LENGTH))
            'croagunkShop = New ItemShopNoParams(buf.seek(CROAGUNK_SHOP_OFFSET).view(CROAGUNK_SHOP_LENGTH))
        End Sub

        Public Overridable Function toByteA() As Byte()
            buf.seek(MAGIC_OFFSET).putBytes(MAGIC_DATA)

            pkmnStorage.store(buf.seek(PKMN_STORAGE_OFFSET).view(PKMN_STORAGE_LENGTH))
            activePkmn.store(buf.seek(ACTIVE_PKMN_OFFSET).view(ACTIVE_PKMN_LENGTH))
            activePkmnEP.store(buf.seek(ACTIVE_PKMN_EP_OFFSET).view(ACTIVE_PKMN_EP_LENGTH))
            buf.seek(ADVENTURES_HAD_OFFSET).putInt(adventuresHad, ADVENTURES_HAD_LENGTH)
            heldItemStorage.store(buf.seek(HELD_ITEM_STORAGE_OFFSET).view(HELD_ITEM_STORAGE_LENGTH))
            heldItemEPStorage.store(buf.seek(HELD_ITEM_EP_STORAGE_OFFSET).view(HELD_ITEM_EP_STORAGE_LENGTH))
            itemStorage.store(buf.seek(ITEM_STORAGE_OFFSET).view(ITEM_STORAGE_LENGTH))
            itemShop1.store(buf.seek(ITEM_SHOP1_OFFSET).view(ITEM_SHOP1_LENGTH))
            itemShop2.store(buf.seek(ITEM_SHOP2_OFFSET).view(ITEM_SHOP2_LENGTH))
            buf.seek(HELD_MONEY_OFFSET).putInt(heldMoney, HELD_MONEY_LENGTH)
            buf.seek(HELD_MONEY_EP_OFFSET).putInt(heldMoneyEP, HELD_MONEY_EP_LENGTH)
            buf.seek(STORED_MONEY_OFFSET).putInt(storedMoney, STORED_MONEY_LENGTH)
            buf.seek(TEAMNAME_OFFSET).putBytes(SkyCharConv.encode(teamName))
            adventureLog.store(buf.seek(ADVENTURE_LOG_OFFSET).view(ADVENTURE_LOG_LENGTH))
            'croagunkShop.store(buf.seek(CROAGUNK_SHOP_OFFSET).view(CROAGUNK_SHOP_LENGTH))

            Dim data() As Byte = BitConverterLE.packBits(split_data)
            'Will be handled by Sky Editor... BitConverterLE.writeInt32(calcChecksum(data), data, 0)
            Return data
        End Function

        Private Shared Function calcChecksum(ByVal data() As Byte) As Integer
            Dim chksum As Integer = 0
            For i As Integer = 4 To data.Length - 1 Step 4
                chksum += BitConverterLE.readInt32(data, i)
            Next i
            Return chksum
        End Function

    End Class

End Namespace