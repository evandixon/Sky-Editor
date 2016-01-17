Imports SkyEditorBase.ARDS
Imports SkyEditorBase.Interfaces

Public Class RedStoredMoney
    Implements SkyEditorBase.ARDS.CodeDefinition

    Public ReadOnly Property Author As String Implements SkyEditorBase.ARDS.CodeDefinition.Author
        Get
            Return "Demonic722"
        End Get
    End Property

    Public ReadOnly Property Category As String Implements SkyEditorBase.ARDS.CodeDefinition.Category
        Get
            Return "Money"
        End Get
    End Property

    Public ReadOnly Property Name As String Implements SkyEditorBase.ARDS.CodeDefinition.Name
        Get
            Return "Stored Money"
        End Get
    End Property

    Public ReadOnly Property SupportedGames As String() Implements SkyEditorBase.ARDS.CodeDefinition.SupportedGames
        Get
            Return {SaveEditor.GameStrings.RedGame}
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

    Public Function GenerateCode(Save As Object, TargetRegion As Region, ButtonActivator As UShort, CodeType As CheatFormat) As String Implements CodeDefinition.GenerateCode
        'Dim s = DirectCast(Save, RBSave)
        'Dim moneyHex As String = Conversion.Hex(Math.Min(s.StoredMoney, &HFFFFFF)).PadLeft(8, "0")
        'Dim code As New SkyEditorBase.ARDS.CBAHelper.Code
        'code.Add(CBAHelper.Line.IfButtonDown(ButtonActivator))
        'code.Add(New CBAHelper.Line(String.Format("82038C0C {0}", moneyHex.Substring(4, 4))))
        'code.Add(CBAHelper.Line.IfButtonDown(ButtonActivator))
        'code.Add(New CBAHelper.Line(String.Format("32038C0F {0}", moneyHex.Substring(0, 4))))
        'Return code.ToString
        Return ""
    End Function

    Public ReadOnly Property SupportedCheatFormats As CheatFormat() Implements CodeDefinition.SupportedCheatFormats
        Get
            Return {CheatFormat.CBA}
        End Get
    End Property
End Class