Imports SkyEditorBase.ARDS
Public Class BlueHeldMoney
    Implements SkyEditorBase.ARDS.CodeDefinition

    Public ReadOnly Property Author As String Implements SkyEditorBase.ARDS.CodeDefinition.Author
        Get
            Return "CodeJunkies"
        End Get
    End Property

    Public ReadOnly Property Category As String Implements SkyEditorBase.ARDS.CodeDefinition.Category
        Get
            Return "Money"
        End Get
    End Property

    Public ReadOnly Property Name As String Implements SkyEditorBase.ARDS.CodeDefinition.Name
        Get
            Return "Held Money"
        End Get
    End Property

    Public ReadOnly Property SupportedGames As String() Implements SkyEditorBase.ARDS.CodeDefinition.SupportedGames
        Get
            Return {SkyEditor.GameConstants.BlueGame}
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
        Dim moneyHex As String = Conversion.Hex(s.HeldMoney)
        Dim code As New SkyEditorBase.ARDS.CodeGeneratorHelper.Code
        code.Add(CodeGeneratorHelper.Line.IfButtonDown(ButtonActivator))
        code.Add(New CodeGeneratorHelper.Line(String.Format("0213C12C {0}", moneyHex.PadLeft(8, "0"))))
        code.Add(CodeGeneratorHelper.Line.MasterEnd)
        Return code.ToString
    End Function

    Public ReadOnly Property SupportedCheatFormats As CheatFormat() Implements CodeDefinition.SupportedCheatFormats
        Get
            Return {CheatFormat.ARDS}
        End Get
    End Property
End Class
