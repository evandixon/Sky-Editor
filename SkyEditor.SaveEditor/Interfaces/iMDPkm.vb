Namespace Interfaces
    Public Interface iMDPkm
        ReadOnly Property IsValid As Boolean
        Property Level As Byte
        Property ID As Integer
        Property MetAt As Integer
        Property MaxHP As Integer
        Property StatAttack As Integer
        Property StatDefense As Integer
        Property StatSpAttack As Integer
        Property StatSpDefense As Integer
        Property Exp As Integer
        Property Name As String

        Function GetPokemonDictionary() As IDictionary(Of Integer, String)
        Function GetMetAtDictionary() As IDictionary(Of Integer, String)
    End Interface

End Namespace