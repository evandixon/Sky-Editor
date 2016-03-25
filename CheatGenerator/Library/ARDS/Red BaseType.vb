Imports CheatGenerator.ARDS

Public Class RedBaseType
    Implements CodeDefinition

    Public ReadOnly Property Author As String Implements CodeDefinition.Author
        Get
            Return "Unknown"
        End Get
    End Property

    Public ReadOnly Property Category As String Implements CodeDefinition.Category
        Get
            Return "Misc."
        End Get
    End Property

    Public ReadOnly Property Name As String Implements CodeDefinition.Name
        Get
            Return "Base Type"
        End Get
    End Property

    Public ReadOnly Property SupportedGames As String() Implements CodeDefinition.SupportedGames
        Get
            Return {SaveEditor.GameStrings.RedGame}
        End Get
    End Property

    Public ReadOnly Property SupportedRegions As UShort Implements CodeDefinition.SupportedRegions
        Get
            Return Region.US
        End Get
    End Property

    Public Overrides Function ToString() As String Implements CodeDefinition.ToString
        Return Name
    End Function

    Public Function GenerateCode(Save As Object, TargetRegion As Region, ButtonActivator As UShort, CodeType As CheatFormat) As String Implements CodeDefinition.GenerateCode
        'Const FF As Byte = &HFF
        'Dim s = DirectCast(Save, RBSave)
        'Dim moneyHex As String = Conversion.Hex(Math.Min(s.BaseType, FF))
        'Dim code As New SkyEditorBase.ARDS.CBAHelper.Code
        'code.Add(CBAHelper.Line.IfButtonDown(ButtonActivator))
        'code.Add(New CBAHelper.Line(String.Format("32000AEB {0}", moneyHex.PadLeft(4, "0"))))
        'Return code.ToString
        Return ""
    End Function

    Public ReadOnly Property SupportedCheatFormats As CheatFormat() Implements CodeDefinition.SupportedCheatFormats
        Get
            Return {CheatFormat.CBA}
        End Get
    End Property
End Class