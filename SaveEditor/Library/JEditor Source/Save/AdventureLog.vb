Imports SkyEditor.skyjed.buffer.BooleanBuffer

Namespace skyjed.save

    Public Class AdventureLog

        Private Const LEN_PLAY_TIME As Integer = 32
        Private Const LEN_DUNGEONS_CLEARED As Integer = 20
        Private Const LEN_SKIP1 As Integer = 53
        Private Const LEN_FLAGS As Integer = 59
        Private Const LEN_SKIP2 As Integer = 56
        Private Const LEN_POKEMON_JOINED As Integer = 14
        Private Const LEN_KINDS_OF_POKEMON_BATTLED As Integer = 10
        Private Const LEN_SKIP3 As Integer = 4
        Private Const LEN_MOVES_LEARNED As Integer = 9
        Private Const LEN_RECORD_NUM_OF_ONE_FLOOR_VICTORIES As Integer = 20
        Private Const LEN_FAINTED_IN_DUNGEONS As Integer = 20
        Private Const LEN_POKEMON_EGGS_HATCHED As Integer = 20
        Private Const LEN_WON_BIG_AT_BIG_TREASURE As Integer = 20
        Private Const LEN_RECYCLED As Integer = 20
        Private Const LEN_SKY_GIFTS_SENT As Integer = 20

        Public playTime As Long ' 32bit unsigned int would be sufficient
        Public dungeonsCleared As Integer
        Public flags() As Boolean ' 23 read-write & 36 read-only
        Public pokemonJoined As Integer ' read-only
        Public kindsOfPokemonBattled As Integer ' read-only
        Public movesLearned As Integer ' read-only
        Public recordNumOfOneFloorVictories As Integer
        Public faintedInDungeons As Integer
        Public pokemonEggsHatched As Integer
        Public wonBigAtBigTreasure As Integer
        Public recycled As Integer
        Public skyGiftsSent As Integer

        Public Sub New()
            flags = New Boolean(LEN_FLAGS - 1) {}
        End Sub

        Public Sub New(ByVal buf As buffer.BooleanBuffer)
            Me.New()
            load(buf)
        End Sub

        Public Sub load(ByVal buf As buffer.BooleanBuffer)
            playTime = buf.getLong(LEN_PLAY_TIME)
            dungeonsCleared = buf.getInt(LEN_DUNGEONS_CLEARED)
            buf.skip(LEN_SKIP1)
            For i As Integer = 0 To 58
                flags(i) = buf.get()
            Next i
            buf.skip(LEN_SKIP2)
            pokemonJoined = buf.getInt(LEN_POKEMON_JOINED)
            kindsOfPokemonBattled = buf.getInt(LEN_KINDS_OF_POKEMON_BATTLED)
            buf.skip(LEN_SKIP3)
            movesLearned = buf.getInt(LEN_MOVES_LEARNED)
            recordNumOfOneFloorVictories = buf.getInt(LEN_RECORD_NUM_OF_ONE_FLOOR_VICTORIES)
            faintedInDungeons = buf.getInt(LEN_FAINTED_IN_DUNGEONS)
            pokemonEggsHatched = buf.getInt(LEN_POKEMON_EGGS_HATCHED)
            wonBigAtBigTreasure = buf.getInt(LEN_WON_BIG_AT_BIG_TREASURE)
            recycled = buf.getInt(LEN_RECYCLED)
            skyGiftsSent = buf.getInt(LEN_SKY_GIFTS_SENT)
        End Sub

        Public Sub store(ByVal buf As buffer.BooleanBuffer)
            buf.putLong(playTime, LEN_PLAY_TIME)
            buf.putInt(dungeonsCleared, LEN_DUNGEONS_CLEARED)
            buf.skip(LEN_SKIP1)
            For i As Integer = 0 To 58
                buf.put(flags(i))
            Next i
            buf.skip(LEN_SKIP2)
            buf.putInt(pokemonJoined, LEN_POKEMON_JOINED)
            buf.putInt(kindsOfPokemonBattled, LEN_KINDS_OF_POKEMON_BATTLED)
            buf.skip(LEN_SKIP3)
            buf.putInt(movesLearned, LEN_MOVES_LEARNED)
            buf.putInt(recordNumOfOneFloorVictories, LEN_RECORD_NUM_OF_ONE_FLOOR_VICTORIES)
            buf.putInt(faintedInDungeons, LEN_FAINTED_IN_DUNGEONS)
            buf.putInt(pokemonEggsHatched, LEN_POKEMON_EGGS_HATCHED)
            buf.putInt(wonBigAtBigTreasure, LEN_WON_BIG_AT_BIG_TREASURE)
            buf.putInt(recycled, LEN_RECYCLED)
            buf.putInt(skyGiftsSent, LEN_SKY_GIFTS_SENT)
        End Sub

    End Class

End Namespace