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
    Public Shared Function NDSModSourceFile() As String
        Return PluginHelper.GetLanguageItem("NDSModSrc", "NDS Mod")
    End Function
End Class