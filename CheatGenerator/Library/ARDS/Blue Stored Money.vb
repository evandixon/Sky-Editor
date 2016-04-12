Imports CheatGenerator.ARDS

Public Class BlueStoredMoney
    Implements CodeDefinition

    Public ReadOnly Property Author As String Implements CodeDefinition.Author
        Get
            Return "Demonic722"
        End Get
    End Property

    Public ReadOnly Property Category As String Implements CodeDefinition.Category
        Get
            Return "Money"
        End Get
    End Property

    Public ReadOnly Property Name As String Implements CodeDefinition.Name
        Get
            Return "Stored Money"
        End Get
    End Property

    Public ReadOnly Property SupportedGames As String() Implements CodeDefinition.SupportedGames
        Get
            Return {} 'SaveEditor.GameStrings.BlueGame}
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
        'Dim s = DirectCast(Save, RBSave)
        'Dim moneyHex As String = Conversion.Hex(s.StoredMoney)
        'Dim code As New SkyEditorBase.ARDS.CodeGeneratorHelper.Code
        'code.Add(CodeGeneratorHelper.Line.IfButtonDown(ButtonActivator))
        'code.Add(New CodeGeneratorHelper.Line(String.Format("0213C130 {0}", moneyHex.PadLeft(8, "0"))))
        'code.Add(CodeGeneratorHelper.Line.MasterEnd)
        'Return code.ToString
        Return ""
    End Function

    Public ReadOnly Property SupportedCheatFormats As CheatFormat() Implements CodeDefinition.SupportedCheatFormats
        Get
            Return {CheatFormat.ARDS}
        End Get
    End Property

End Class