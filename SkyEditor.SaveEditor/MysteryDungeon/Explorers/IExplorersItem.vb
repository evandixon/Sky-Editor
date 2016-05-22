Namespace MysteryDungeon.Explorers
    Public Interface IExplorersItem
        Property ID As Integer

        ''' <summary>
        ''' The ID of the item inside this one, if this item is a box.
        ''' </summary>
        ''' <returns></returns>
        Property ContainedItemID As Integer?
        Property Quantity As Integer
        Property HeldBy As Byte
        ReadOnly Property IsBox As Boolean
    End Interface

End Namespace
