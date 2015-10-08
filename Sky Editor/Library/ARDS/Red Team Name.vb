Imports SkyEditorBase.ARDS
Imports SkyEditorBase.Interfaces

Public Class RedTeamName
    Implements SkyEditorBase.ARDS.CodeDefinition

    Public ReadOnly Property Author As String Implements SkyEditorBase.ARDS.CodeDefinition.Author
        Get
            Return "Demonic722"
        End Get
    End Property

    Public ReadOnly Property Category As String Implements SkyEditorBase.ARDS.CodeDefinition.Category
        Get
            Return "Misc."
        End Get
    End Property

    Public ReadOnly Property Name As String Implements SkyEditorBase.ARDS.CodeDefinition.Name
        Get
            Return "Team Name"
        End Get
    End Property

    Public ReadOnly Property SupportedGames As String() Implements SkyEditorBase.ARDS.CodeDefinition.SupportedGames
        Get
            Return {SkyEditor.GameStrings.RedGame}
        End Get
    End Property

    Public ReadOnly Property SupportedRegions As UShort Implements SkyEditorBase.ARDS.CodeDefinition.SupportedRegions
        Get
            Return SkyEditorBase.ARDS.Region.US
        End Get
    End Property

    Public Overrides Function ToString() As String Implements SkyEditorBase.ARDS.CodeDefinition.ToString
        Return Name
    End Function

    Public Function GenerateCode(Save As iGenericFile, TargetRegion As Region, ButtonActivator As UShort, CodeType As CheatFormat) As String Implements CodeDefinition.GenerateCode
        'Dim s = DirectCast(Save, RBSave)
        'Dim n = s.TeamName.PadRight(10, vbNullChar)
        'Dim Hex0 As String = Conversion.Hex(SkyEditor.Lists.StringEncodingInverse(n(0))).PadLeft(2, "0")
        'Dim Hex1 As String = Conversion.Hex(SkyEditor.Lists.StringEncodingInverse(n(1))).PadLeft(2, "0")
        'Dim Hex2 As String = Conversion.Hex(SkyEditor.Lists.StringEncodingInverse(n(2))).PadLeft(2, "0")
        'Dim Hex3 As String = Conversion.Hex(SkyEditor.Lists.StringEncodingInverse(n(3))).PadLeft(2, "0")
        'Dim Hex4 As String = Conversion.Hex(SkyEditor.Lists.StringEncodingInverse(n(4))).PadLeft(2, "0")
        'Dim Hex5 As String = Conversion.Hex(SkyEditor.Lists.StringEncodingInverse(n(5))).PadLeft(2, "0")
        'Dim Hex6 As String = Conversion.Hex(SkyEditor.Lists.StringEncodingInverse(n(6))).PadLeft(2, "0")
        'Dim Hex7 As String = Conversion.Hex(SkyEditor.Lists.StringEncodingInverse(n(7))).PadLeft(2, "0")
        'Dim Hex8 As String = Conversion.Hex(SkyEditor.Lists.StringEncodingInverse(n(8))).PadLeft(2, "0")
        'Dim Hex9 As String = Conversion.Hex(SkyEditor.Lists.StringEncodingInverse(n(9))).PadLeft(2, "0")
        'Dim code As New SkyEditorBase.ARDS.CBAHelper.Code
        'code.Add(CBAHelper.Line.IfButtonDown(ButtonActivator))
        'code.Add(New CBAHelper.Line(String.Format("82038C10 {0}", Hex1 & Hex0)))
        'code.Add(CBAHelper.Line.IfButtonDown(ButtonActivator))
        'code.Add(New CBAHelper.Line(String.Format("82038C12 {0}", Hex3 & Hex2)))
        'code.Add(CBAHelper.Line.IfButtonDown(ButtonActivator))
        'code.Add(New CBAHelper.Line(String.Format("82038C14 {0}", Hex5 & Hex4)))
        'code.Add(CBAHelper.Line.IfButtonDown(ButtonActivator))
        'code.Add(New CBAHelper.Line(String.Format("82038C16 {0}", Hex7 & Hex6)))
        'code.Add(CBAHelper.Line.IfButtonDown(ButtonActivator))
        'code.Add(New CBAHelper.Line(String.Format("82038C18 {0}", Hex9 & Hex8)))
        'Return code.ToString
        Return ""
    End Function

    Public ReadOnly Property SupportedCheatFormats As CheatFormat() Implements CodeDefinition.SupportedCheatFormats
        Get
            Return {CheatFormat.CBA}
        End Get
    End Property
End Class