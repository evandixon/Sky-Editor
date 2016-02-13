Imports SkyEditorBase
Public Class GameStrings
    Public Shared Function GenericNDSRom() As String
        Return PluginHelper.GetLanguageItem("Generic NDS ROM")
    End Function
    Public Shared Function SkyNDSRom() As String
        Return PluginHelper.GetLanguageItem("NDS ROM: Pokemon Mystery Dungeon: Explorers of Sky")
    End Function
    Public Shared Function SkyROMHeader() As String
        Return PluginHelper.GetLanguageItem("POKEDUN SORA")
    End Function
    Public Shared Function BGPFile() As String
        Return PluginHelper.GetLanguageItem("BGP File")
    End Function
    Public Shared Function item_p_File() As String
        Return PluginHelper.GetLanguageItem("item_p_File", "Item Definitions (item_p.bin)")
    End Function
    Public Shared Function item_s_p_File() As String
        Return PluginHelper.GetLanguageItem("item_s_p_File", "Exclusive Item Rarity (item_s_p.bin)")
    End Function
    Public Shared Function waza_p_File() As String
        Return PluginHelper.GetLanguageItem("waza_p_File", "Pokémon Move Assignments (waza_p.bin)")
    End Function
    Public Shared Function LanguageStringFile() As String
        Return PluginHelper.GetLanguageItem("LanguageStringFile", "NDS Mod")
    End Function
    Public Shared Function NDSModSourceFile() As String
        Return PluginHelper.GetLanguageItem("NDSModSrc", "NDS Mod")
    End Function
    Public Shared Function KaomadoFixNDSModSourceFile() As String
        Return PluginHelper.GetLanguageItem("KaomadoFixNDSModSrc", "Insert Missing Pokemon Portraits")
    End Function
    Public Shared Function PersonalityTest() As String
        Return PluginHelper.GetLanguageItem("PersonalityTest", "Personality Test")
    End Function

    Public Const PokemonXCode = "0004000000055D00"
    Public Const PokemonYCode = "0004000000055E00"
    Public Const ORCode = "000400000011C400"
    Public Const ASCode = "000400000011C500"
    Public Const GTICode = "00040000000BA(8|9)00"
    Public Const PSMDCode = "0004000000(1746|149B)00"

    Public Const SkyCode = "C2S."
End Class