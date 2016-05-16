Imports System.Text
Imports SkyEditor.Core.IO

Namespace FileFormats.Explorers
    Public Class LanguageString
        Inherits GenericFile
        Implements IOpenableFile
        Public Enum Region
            US
            Europe
        End Enum

        Public Sub New()
            Items = New List(Of String)
        End Sub

        Public Shared Function ConvertEUToUS(Offset As Integer) As Integer
            If Offset = 3938 OrElse Offset = 3939 Then
                Throw New IndexOutOfRangeException("Indexes 3938 and 3939 do not have a US equivalent.")
            ElseIf Offset < 3938 Then
                Return Offset
            ElseIf 3938 < Offset AndAlso Offset < 17600 Then
                Return Offset - 2
            ElseIf 17600 <= Offset AndAlso Offset < 17812 Then
                Throw New NotImplementedException("Conversion in the Staff Credits is currently not supported.")
            Else 'If   17812<=Offset
                Return Offset - 31
            End If
        End Function
        Public Shared Function ConvertUSToEU(Offset As Integer) As Integer
            If Offset < 3938 Then
                Return Offset
            ElseIf 3938 <= Offset AndAlso Offset < 17598 Then
                Return Offset + 2
            ElseIf 17598 <= Offset AndAlso Offset < 17781 Then
                Throw New NotImplementedException("Conversion in the Staff Credits is currently not supported.")
            Else ' offset <= 17781
                Return Offset + 31
            End If
        End Function

        Public ReadOnly Property FileRegion As Region
            Get
                If Items.Count = 18451 Then
                    Return Region.US
                ElseIf Items.Count = 18482 Then
                    Return Region.Europe
                Else
                    Return Nothing
                End If
            End Get
        End Property

        Public Property Items As List(Of String)

        Public Overrides Async Function OpenFile(Filename As String, Provider As IOProvider) As Task Implements IOpenableFile.OpenFile
            Await MyBase.OpenFile(Filename, Provider)
            Dim bytes = IO.File.ReadAllBytes(Filename)

            Items = New List(Of String)

            Dim offset1 As UInt32 = BitConverter.ToUInt32(bytes, 0)
            Dim e = Encoding.GetEncoding("Windows-1252")
            'Loop through each entry
            For count As Integer = 0 To offset1 - 5 Step 4
                Dim startOffset As UInteger = BitConverter.ToUInt32(bytes, count)
                Items.Add("")
                Dim endOffset As UInteger = startOffset
                Dim s As New StringBuilder
                'Read the null-terminated string
                While bytes(endOffset) <> 0
                    s.Append(e.GetString({RawData(endOffset)}))
                    endOffset += 1
                End While
                Items(count / 4) = s.ToString
            Next
        End Function
        Public Overrides Sub Save(Destination As String, provider As IOProvider)
            'Generate File
            Dim e = Encoding.GetEncoding("Windows-1252")
            Dim offsets As New List(Of UInt32)
            For i As UInt32 = 0 To Items.Count - 1
                offsets.Add(0)
            Next
            offsets.Add(0) 'The file format contains an additional pointer to 1 byte after the last entry
            Dim stringdataBytes As New List(Of Byte)
            For count As Integer = 0 To Items.Count - 1
                Dim offset As UInt32 = offsets.Count * 4 + stringdataBytes.Count
                offsets(count) = offset
                Dim strBytes = e.GetBytes(Item(count).Replace(vbCrLf, vbCr))
                For Each s In strBytes
                    stringdataBytes.Add(s)
                Next
                stringdataBytes.Add(0)
            Next
            offsets(offsets.Count - 1) = offsets.Count * 4 + stringdataBytes.Count 'The file format contains an additional pointer to 1 byte after the last entry
            Dim offsetBytes As New List(Of Byte)
            For Each offset In offsets
                Dim t = BitConverter.GetBytes(offset)
                offsetBytes.Add(t(0))
                offsetBytes.Add(t(1))
                offsetBytes.Add(t(2))
                offsetBytes.Add(t(3))
            Next

            Dim totalData As New List(Of Byte)
            For Each b In offsetBytes
                totalData.Add(b)
            Next
            For Each b In stringdataBytes
                totalData.Add(b)
            Next
            'Write buffer to stream
            Length = totalData.Count
            RawData(0, totalData.Count) = totalData.ToArray
            MyBase.Save(Destination, provider)
        End Sub
        Default Public Property Item(Index As UInteger) As String
            Get
                Return Items(Index)
            End Get
            Set(value As String)
                Items(Index) = value
            End Set
        End Property
        Public Const ItemNameStartUS As Integer = 6773
        Public Const HardyMessage As Integer = 1660
        Public Const HardyMale As Integer = 1661
        Public Const HardyFemale As Integer = 1662
        Public Const DocileMessage As Integer = 1663
        Public Const DocileMale As Integer = 1664
        Public Const DocileFemale As Integer = 1665
        Public Const BraveMessage As Integer = 1666
        Public Const BraveMale As Integer = 1667
        Public Const BraveFemale As Integer = 1668
        Public Const JollyMessage As Integer = 1669
        Public Const JollyMale As Integer = 1670
        Public Const JollyFemale As Integer = 1671
        Public Const ImpishMessage As Integer = 1672
        Public Const ImpishMale As Integer = 1673
        Public Const ImpishFemale As Integer = 1674
        Public Const NaïveMessage As Integer = 1675
        Public Const NaïveMale As Integer = 1676
        Public Const NaïveFemale As Integer = 1677
        Public Const TimidMessage As Integer = 1678
        Public Const TimidMale As Integer = 1679
        Public Const TimidFemale As Integer = 1680
        Public Const HastyMessage As Integer = 1681
        Public Const HastyMale As Integer = 1682
        Public Const HastyFemale As Integer = 1683
        Public Const SassyMessage As Integer = 1684
        Public Const SassyMale As Integer = 1685
        Public Const SassyFemale As Integer = 1686
        Public Const CalmMessage As Integer = 1687
        Public Const CalmMale As Integer = 1688
        Public Const CalmFemale As Integer = 1689
        Public Const RelaxedMessage As Integer = 1690
        Public Const RelaxedMale As Integer = 1691
        Public Const RelaxedFemale As Integer = 1692
        Public Const LonelyMessage As Integer = 1693
        Public Const LonelyMale As Integer = 1694
        Public Const LonelyFemale As Integer = 1695
        Public Const QuirkyMessage As Integer = 1696
        Public Const QuirkyMale As Integer = 1697
        Public Const QuirkyFemale As Integer = 1698
        Public Const QuietMessage As Integer = 1699
        Public Const QuietMale As Integer = 1700
        Public Const QuietFemale As Integer = 1701
        Public Const RashMessage As Integer = 1702
        Public Const RashMale As Integer = 1703
        Public Const RashFemale As Integer = 1704
        Public Const BoldMessage As Integer = 1705
        Public Const BoldMale As Integer = 1706
        Public Const BoldFemale As Integer = 1707

        Public Const PokemonNameLength As Integer = 553
        Public Const ItemLength As Integer = 1352
        Public Const LocationLength As Integer = 255
        Public Const MoveLength As Integer = 562

        Public Function GetPokemonName(PokemonID As Integer) As String
            If FileRegion = Region.Europe Then
                Return Item(8736 + PokemonID)
            ElseIf FileRegion = Region.US Then
                Return Item(8734 + PokemonID)
            Else
                Return Item(8736 + PokemonID)
            End If
        End Function

        Public Function GetItemName(ItemID As Integer) As String
            If FileRegion = Region.Europe Then
                Return Item(6775 + ItemID)
            ElseIf FileRegion = Region.US Then
                Return Item(6773 + ItemID)
            Else
                Return Item(6775 + ItemID)
            End If
        End Function

        Public Function GetLocationName(LocationID As Integer) As String
            If FileRegion = Region.Europe Then
                Return Item(16566 + LocationID)
            ElseIf FileRegion = Region.US Then
                Return Item(16564 + LocationID)
            Else
                Return Item(16566 + LocationID)
            End If
        End Function

        Public Function GetMoveName(MoveID As Integer) As String
            If FileRegion = Region.Europe Then
                Return Item(8175 + MoveID)
            ElseIf FileRegion = Region.US Then
                Return Item(8173 + MoveID)
            Else
                Return Item(8175 + MoveID)
            End If
        End Function

        Public Sub UpdatePersonalityTestResult(ResultIndex As Integer, PokemonID As Integer)
            Dim r As New Text.RegularExpressions.Regex("(\[CS\:K\])(.*)(\[CR\])")
            If PokemonID > 600 Then 'In case a female Pokemon was passed in.
                PokemonID -= 600
            End If
            Item(ResultIndex) = r.Replace(Item(ResultIndex), "[CS:K]" & GetPokemonName(PokemonID) & "[CR]")
        End Sub
        Public Sub UpdatePersonalityTestResult(Results As PersonalityTestContainer)
            UpdatePersonalityTestResult(HardyMale, Results.HardyMale)
            UpdatePersonalityTestResult(HardyFemale, Results.HardyFemale)
            UpdatePersonalityTestResult(DocileMale, Results.DocileMale)
            UpdatePersonalityTestResult(DocileFemale, Results.DocileFemale)
            UpdatePersonalityTestResult(BraveMale, Results.BraveMale)
            UpdatePersonalityTestResult(BraveFemale, Results.BraveFemale)
            UpdatePersonalityTestResult(JollyMale, Results.JollyMale)
            UpdatePersonalityTestResult(JollyFemale, Results.JollyFemale)
            UpdatePersonalityTestResult(ImpishMale, Results.ImpishMale)
            UpdatePersonalityTestResult(ImpishFemale, Results.ImpishFemale)
            UpdatePersonalityTestResult(NaïveMale, Results.NaiveMale)
            UpdatePersonalityTestResult(NaïveFemale, Results.NaiveFemale)
            UpdatePersonalityTestResult(TimidMale, Results.TimidMale)
            UpdatePersonalityTestResult(TimidFemale, Results.TimidFemale)
            UpdatePersonalityTestResult(HastyMale, Results.HastyMale)
            UpdatePersonalityTestResult(HastyFemale, Results.HastyFemale)
            UpdatePersonalityTestResult(SassyMale, Results.SassyMale)
            UpdatePersonalityTestResult(SassyFemale, Results.SassyFemale)
            UpdatePersonalityTestResult(CalmMale, Results.CalmMale)
            UpdatePersonalityTestResult(CalmFemale, Results.CalmFemale)
            UpdatePersonalityTestResult(RelaxedMale, Results.RelaxedMale)
            UpdatePersonalityTestResult(RelaxedFemale, Results.RelaxedFemale)
            UpdatePersonalityTestResult(LonelyMale, Results.LonelyMale)
            UpdatePersonalityTestResult(LonelyFemale, Results.LonelyFemale)
            UpdatePersonalityTestResult(QuirkyMale, Results.QuirkyMale)
            UpdatePersonalityTestResult(QuirkyFemale, Results.QuirkyFemale)
            UpdatePersonalityTestResult(QuietMale, Results.QuietMale)
            UpdatePersonalityTestResult(QuietFemale, Results.QuietFemale)
            UpdatePersonalityTestResult(RashMale, Results.RashMale)
            UpdatePersonalityTestResult(RashFemale, Results.RashFemale)
            UpdatePersonalityTestResult(BoldMale, Results.BoldMale)
            UpdatePersonalityTestResult(BoldFemale, Results.BoldFemale)
        End Sub


        'Default Public Property Item(Index As UInteger) As String
        '    Get
        '        Dim startOffset As UInteger = BitConverter.ToUInt32(RawData, Index * 4)
        '        Dim endOffset As UInteger = BitConverter.ToUInt32(RawData, (Index + 1) * 4) - 2

        '        Return ""
        '    End Get
        '    Set(value As String)
        '        Dim startOffset As UInteger = BitConverter.ToUInt32(RawData, Index * 4)
        '        Dim endOffset As UInteger = BitConverter.ToUInt32(RawData, (Index + 1) * 4) - 2
        '        Dim e As New Text.UTF7Encoding
        '        Dim valueBytes = e.GetBytes(value)
        '        For count As Integer = 0 To Math.Min(valueBytes.Length - 1, endOffset - startOffset - 1)
        '            RawData(startOffset + count) = valueBytes(count)
        '        Next
        '    End Set
        'End Property

    End Class
End Namespace