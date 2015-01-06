Imports SkyEditorBase.ARDS
Public Class RedRescuePoints
    Implements SkyEditorBase.ARDS.CodeDefinitionV3

    Public ReadOnly Property Author As String Implements SkyEditorBase.ARDS.CodeDefinitionV3.Author
        Get
            Return "Demonic722"
        End Get
    End Property

    Public ReadOnly Property Category As String Implements SkyEditorBase.ARDS.CodeDefinitionV3.Category
        Get
            Return "Misc."
        End Get
    End Property

    Public ReadOnly Property Name As String Implements SkyEditorBase.ARDS.CodeDefinitionV3.Name
        Get
            Return "Rescue Points"
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
        Dim s = DirectCast(Save, RBSave)
        Dim moneyHex As String = Conversion.Hex(Math.Min(s.RescuePoints, &HFFFF))
        Dim code As New SkyEditorBase.ARDS.CBAHelper.Code
        code.Add(CBAHelper.Line.IfButtonDown(ButtonActivator))
        code.Add(New CBAHelper.Line(String.Format("82038C1C {0}", moneyHex.PadLeft(4, "0"))))
        Return code.ToString
    End Function

    Public ReadOnly Property SupportedCheatFormats As CheatFormat() Implements CodeDefinitionV3.SupportedCheatFormats
        Get
            Return {CheatFormat.CBA}
        End Get
    End Property
End Class
