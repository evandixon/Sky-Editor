Imports SkyEditorBase.ARDS
Public Class RedBaseType
    Implements SkyEditorBase.ARDS.CodeDefinition

    Public ReadOnly Property Author As String Implements SkyEditorBase.ARDS.CodeDefinition.Author
        Get
            Return "Unknown"
        End Get
    End Property

    Public ReadOnly Property Category As String Implements SkyEditorBase.ARDS.CodeDefinition.Category
        Get
            Return "Misc."
        End Get
    End Property

    Public ReadOnly Property Name As String Implements SkyEditorBase.ARDS.CodeDefinition.Name
        Get
            Return "Base Type"
        End Get
    End Property

    Public ReadOnly Property SupportedGames As String() Implements SkyEditorBase.ARDS.CodeDefinition.SupportedGames
        Get
            Return {SkyEditor.GameConstants.RedGame}
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

    Public Function GenerateCode(Save As SkyEditorBase.GenericSave, TargetRegion As Region, ButtonActivator As UShort, CodeType As CheatFormat) As String Implements CodeDefinition.GenerateCode
        Dim s = DirectCast(Save, RBSave)
        Dim moneyHex As String = Conversion.Hex(Math.Min(s.BaseType, &HFF))
        Dim code As New SkyEditorBase.ARDS.CBAHelper.Code
        code.Add(CBAHelper.Line.IfButtonDown(ButtonActivator))
        code.Add(New CBAHelper.Line(String.Format("32000AEB {0}", moneyHex.PadLeft(4, "0"))))
        Return code.ToString
    End Function

    Public ReadOnly Property SupportedCheatFormats As CheatFormat() Implements CodeDefinition.SupportedCheatFormats
        Get
            Return {CheatFormat.CBA}
        End Get
    End Property
End Class
