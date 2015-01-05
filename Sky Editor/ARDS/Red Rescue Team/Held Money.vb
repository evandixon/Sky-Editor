Imports SkyEditorBase.ARDS
Public Class RedHeldMoney
    Implements SkyEditorBase.ARDS.CodeDefinitionV3

    Public ReadOnly Property Author As String Implements SkyEditorBase.ARDS.CodeDefinitionV3.Author
        Get
            Return "CodeJunkies"
        End Get
    End Property

    Public ReadOnly Property Category As String Implements SkyEditorBase.ARDS.CodeDefinitionV3.Category
        Get
            Return "Money"
        End Get
    End Property

    Public ReadOnly Property Name As String Implements SkyEditorBase.ARDS.CodeDefinitionV3.Name
        Get
            Return "Held Money"
        End Get
    End Property

    Public ReadOnly Property SupportedGames As String() Implements SkyEditorBase.ARDS.CodeDefinitionV3.SupportedGames
        Get
            Return {SkyEditor.GameConstants.RedGame}
        End Get
    End Property

    Public ReadOnly Property SupportedRegions As UShort Implements SkyEditorBase.ARDS.CodeDefinitionV3.SupportedRegions
        Get
            Return SkyEditorBase.ARDS.Region.US
        End Get
    End Property

    Public Overrides Function ToString() As String Implements SkyEditorBase.ARDS.CodeDefinitionV3.ToString
        Return Name
    End Function

    Public Function GenerateCode(Save As SkyEditorBase.GenericSave, TargetRegion As Region, ButtonActivator As UShort, CodeType As CheatFormat) As String Implements CodeDefinitionV3.GenerateCode
        Dim s = SkyEditor.RBSave.FromBase(Save)
        Dim moneyHex As String = Conversion.Hex(Math.Min(s.HeldMoney, &HFFFFFF)).PadLeft(8, "0")
        Dim code As New SkyEditorBase.ARDS.CBAHelper.Code
        code.Add(CBAHelper.Line.IfButtonDown(ButtonActivator))
        code.Add(New CBAHelper.Line(String.Format("82038C08 {0}", moneyHex.Substring(4, 4))))
        code.Add(CBAHelper.Line.IfButtonDown(ButtonActivator))
        code.Add(New CBAHelper.Line(String.Format("32038C0A {0}", moneyHex.Substring(0, 4))))
        Return code.ToString
    End Function

    Public ReadOnly Property SupportedCheatFormats As CheatFormat() Implements CodeDefinitionV3.SupportedCheatFormats
        Get
            Return {CheatFormat.CBA}
        End Get
    End Property
End Class
