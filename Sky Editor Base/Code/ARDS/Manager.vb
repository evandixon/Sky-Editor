Imports SkyEditorBase.Utilities

Namespace ARDS
    Public Class Manager
        Public Property CodeDefinitions As List(Of CodeDefinition)
        Public Property GameIDs As Dictionary(Of String, String)

        Public Function GetCodeTypes() As List(Of CheatFormat)
            Dim _codetypes As New List(Of CheatFormat)
            For Each item In CodeDefinitions
                For Each code In item.SupportedCheatFormats
                    If Not _codetypes.Contains(code) Then
                        _codetypes.Add(code)
                    End If
                Next
            Next
            Return _codetypes
        End Function
        Public Function GetRegions(CodeType As CheatFormat) As List(Of Region)
            Dim out As New List(Of Region)
            For Each item In CodeDefinitions
                If item.SupportedCheatFormats.Contains(CodeType) Then
                    Dim bytes() As Byte = BitConverter.GetBytes(item.SupportedRegions)
                    If CType(bytes(0), Bits8).Bit1 Then
                        If Not out.Contains(Region.Japan) Then
                            out.Add(Region.Japan)
                        End If
                    End If
                    If CType(bytes(0), Bits8).Bit2 Then
                        If Not out.Contains(Region.US) Then
                            out.Add(Region.US)
                        End If
                    End If
                    If CType(bytes(0), Bits8).Bit3 Then
                        If Not out.Contains(Region.UK) Then
                            out.Add(Region.UK)
                        End If
                    End If
                    If CType(bytes(0), Bits8).Bit4 Then
                        If Not out.Contains(Region.Germany) Then
                            out.Add(Region.Germany)
                        End If
                    End If
                    If CType(bytes(0), Bits8).Bit5 Then
                        If Not out.Contains(Region.Korea) Then
                            out.Add(Region.Korea)
                        End If
                    End If
                    If CType(bytes(0), Bits8).Bit6 Then
                        If Not out.Contains(Region.Australia) Then
                            out.Add(Region.Australia)
                        End If
                    End If
                    If CType(bytes(0), Bits8).Bit7 Then
                        If Not out.Contains(Region.Spain) Then
                            out.Add(Region.Spain)
                        End If
                    End If
                    If CType(bytes(0), Bits8).Bit8 Then
                        If Not out.Contains(Region.France) Then
                            out.Add(Region.France)
                        End If
                    End If
                    If CType(bytes(1), Bits8).Bit1 Then
                        If Not out.Contains(Region.Netherlands) Then
                            out.Add(Region.Netherlands)
                        End If
                    End If
                    If CType(bytes(1), Bits8).Bit2 Then
                        If Not out.Contains(Region.Italy) Then
                            out.Add(Region.Italy)
                        End If
                    End If
                    If CType(bytes(1), Bits8).Bit3 Then
                        If Not out.Contains(Region.Denmark) Then
                            out.Add(Region.Denmark)
                        End If
                    End If
                End If
            Next
            Return out
        End Function
        Public Function GetGames(CodeType As CheatFormat, Region As Region) As List(Of String)
            Dim out As New List(Of String)
            For Each item In CodeDefinitions
                '(after the AndAlso) If adding the region doesn't yield any change, it must have already been there
                If item.SupportedCheatFormats.Contains(CodeType) AndAlso (item.SupportedRegions Or Region = item.SupportedRegions) Then
                    For Each g In item.SupportedGames
                        If Not out.Contains(g) Then
                            out.Add(g)
                        End If
                    Next
                End If
            Next
            Return out
        End Function
        Public Function GetGames(CodeType As CheatFormat, Region As Region, SaveFormatID As String) As List(Of String)
            Dim out As New List(Of String)
            Dim games As List(Of String) = GetGames(CodeType, Region)
            For Each item In games
                If GameIDs.Keys.Contains(item) Then
                    If GameIDs(item) = SaveFormatID Then
                        out.Add(item)
                    End If
                End If
            Next
            Return out
        End Function
        Public Function GetCategories(CodeType As CheatFormat, Region As Region, GameID As String) As List(Of String)
            Dim out As New List(Of String)
            For Each item In CodeDefinitions
                '(after the AndAlso) If adding the region doesn't yield any change, it must have already been there
                If item.SupportedCheatFormats.Contains(CodeType) AndAlso (item.SupportedRegions Or Region = item.SupportedRegions) AndAlso item.SupportedGames.Contains(GameID) Then
                    If Not out.Contains(item.Category) Then
                        out.Add(item.Category)
                    End If
                End If
            Next
            Return out
        End Function

        Public Function GetCode(CodeType As CheatFormat, Region As Region, GameID As String, Category As String) As List(Of CodeDefinition)
            Dim out As New List(Of CodeDefinition)
            For Each item In CodeDefinitions
                '(after the AndAlso) If adding the region doesn't yield any change, it must have already been there
                If item.SupportedCheatFormats.Contains(CodeType) AndAlso (item.SupportedRegions Or Region = item.SupportedRegions) AndAlso item.Category = Category AndAlso item.SupportedGames.Contains(GameID) Then
                    If Not out.Contains(item) Then
                        out.Add(item)
                    End If
                End If
            Next
            Return out
        End Function
        Public Sub New(Definitions As List(Of CodeDefinition))
            MyBase.New()
            Me.CodeDefinitions = Definitions
        End Sub
        Public Sub New()
            MyBase.New()
            Me.CodeDefinitions = New List(Of CodeDefinition)
            Me.GameIDs = New Dictionary(Of String, String)
        End Sub
    End Class
End Namespace