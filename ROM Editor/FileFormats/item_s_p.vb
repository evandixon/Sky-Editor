Imports SkyEditorBase

Namespace FileFormats
    Public Class item_s_p
        Inherits GenericFile
        Public Structure Item
            ''' <summary>
            ''' Rarity of the item (ex. *, **, ***).  Also controls whether the item affects a Pokemon or a Type.
            ''' </summary>
            ''' <returns></returns>
            Public Property Rarity As ItemRarity
            ''' <summary>
            ''' If the rarity is 1-4, the ID of the Type the item affects.
            ''' If the rarity is 5-10, the ID of the Pokemon the item affects.
            ''' </summary>
            ''' <returns></returns>
            Public Property TargetID As UInt16
            Public Enum ItemRarity As UInt16
                None = 0
                OneStar1_Type = 1 'Dungeon with a key
                OneStar2_Type = 2 'Job payment
                TwoStar_Type = 3 'Trade the 1 star items for the same type
                ThreeStar_Type = 4 'Trade the 1 star items and 2 star item for the same type

                OneStar1_Pokémon = 5 'Box appraisal
                OneStar2_Pokémon = 6 'Box appraisal
                TwoStar_Pokémon = 7 'Trade the 1 star items for the same Pokemon.
                ThreeStar_Pokémon = 8 'Trade the 1 star items and the two star item for the same Pokemon.
                ThreeStar_HatchItem = 9 'The Pokemon may hatch holding the item
                ThreeStar_Evolution_Pokemon = 10 'Trade the 1 star items and the two star item for the previous Pokemon.
            End Enum
            Public Sub New(Rarity As ItemRarity, TargetID As UInt16)
                Me.Rarity = Rarity
                Me.TargetID = TargetID
            End Sub
        End Structure

        Public Property Items As List(Of Item)

        Public Sub New(Filename As String)
            MyBase.New(Filename)
            InitItems()
        End Sub
        Public Sub New(RawData As Byte())
            MyBase.New(RawData)
            InitItems()
        End Sub
        Private Sub InitItems()
            Items = New List(Of Item)
            If Length >= 16 Then
                Dim index = &H10
                While index + 3 < Length
                    Dim parameter = BitConverter.ToUInt16(RawData(index, 2), 0)
                    If parameter = &H404 Then 'End of the file
                        Exit While
                    Else
                        Dim id = BitConverter.ToUInt16(RawData(index + 2, 2), 0)
                        Items.Add(New Item(parameter, id))
                    End If
                    index += 4
                End While
            End If
        End Sub
        Public Overrides Sub PreSave()
            MyBase.PreSave()
            Length = &HF0F '4 * Items.Count + 32

            'Write header
            RawData(0) = &H53 'S
            RawData(1) = &H49 'I
            RawData(2) = &H52 'R
            RawData(3) = &H30 '0
            RawData(4) = &H0
            RawData(5) = &H0
            RawData(6) = &H0
            RawData(7) = &H0
            RawData(8) = &H0
            RawData(9) = &HF 'Probably the last byte of the header, probably.
            RawData(10) = &H0
            RawData(11) = &H0
            RawData(12) = &H0
            RawData(13) = &H0
            RawData(14) = &H0
            RawData(15) = &H0

            'Write body
            For count = 0 To Math.Min(Items.Count - 1, 956)
                RawData(15 + count, 2) = BitConverter.GetBytes(Items(count).Rarity)
                RawData(15 + count + 2, 2) = BitConverter.GetBytes(Items(count).TargetID)
            Next

            'Write footer
            RawData(Length - 16) = &H53
            RawData(Length - 15) = &H49
            RawData(Length - 14) = &H52
            RawData(Length - 13) = &H30
            RawData(Length - 12) = &H0
            RawData(Length - 11) = &H0
            RawData(Length - 10) = &H0
            RawData(Length - 9) = &H0
            RawData(Length - 8) = &H0
            RawData(Length - 7) = &HF
            RawData(Length - 6) = &H0
            RawData(Length - 5) = &H0
            RawData(Length - 4) = &H0
            RawData(Length - 3) = &H0
            RawData(Length - 2) = &H0
            RawData(Length - 1) = &H0

        End Sub
    End Class
End Namespace
