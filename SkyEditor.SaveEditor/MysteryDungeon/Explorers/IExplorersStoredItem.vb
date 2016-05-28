Namespace MysteryDungeon.Explorers
    Public Interface IExplorersStoredItem
        Property ID As Integer
        Property ContainedItemID As Integer
        Property Quantity As Integer
        ReadOnly Property IsBox As Boolean
    End Interface
End Namespace

