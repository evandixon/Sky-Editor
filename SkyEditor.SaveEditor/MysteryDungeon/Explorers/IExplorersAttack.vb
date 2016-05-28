Namespace MysteryDungeon.Explorers
    Public Interface IExplorersAttack
        Property IsLinked As Boolean
        Property IsSwitched As Boolean
        Property IsSet As Boolean
        Property ID As Integer
        Property Ginseng As Integer
        ReadOnly Property MoveNames As Dictionary(Of Integer, String)
    End Interface
End Namespace

