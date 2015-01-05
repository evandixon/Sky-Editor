Imports SkyEditorBase.ARDS
Imports SkyEditor

Public Class MyCodeGenerator
    Implements SkyEditorBase.ARDS.CodeDefinitionV3

    ''' <summary>
    ''' The name you want mentioned in the code generator
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Author As String Implements SkyEditorBase.ARDS.CodeDefinitionV3.Author
        Get
            Return "CodeJunkies"
        End Get
    End Property

    ''' <summary>
    ''' Codes are grouped together by a category.  If you have multiple classes like this one that generate similar things, make sure they have the same category.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Category As String Implements SkyEditorBase.ARDS.CodeDefinitionV3.Category
        Get
            Return "Money"
        End Get
    End Property

    ''' <summary>
    ''' This should describe what you want the code to change
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Name As String Implements SkyEditorBase.ARDS.CodeDefinitionV3.Name
        Get
            Return "Held Money"
        End Get
    End Property

    ''' <summary>
    ''' A string array of GameID's that this plugin supports.
    ''' It may be easier to have one class per game,
    ''' but you can specify multiple games here if you want to detect the save type in GenerateCode()
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property SupportedGames As String() Implements SkyEditorBase.ARDS.CodeDefinitionV3.SupportedGames
        Get
            Return {SkyEditor.GameConstants.RBSave}
        End Get
    End Property
    ''' <summary>
    ''' Bit field of regions this plugin supports.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property SupportedRegions As UShort Implements SkyEditorBase.ARDS.CodeDefinitionV3.SupportedRegions
        Get
            'If you wanted to support multiple regions, use an OR combination
            'The following line will list your plugin in both the Japan and US regions.
            'Return SkyEditorBase.ARDS.Region.Japan Or SkyEditorBase.ARDS.Region.US

            'Sky Editor was developed by someone who lives in the US, so it's only natural that Sky Editor AND the starter kit both only support US.
            'Sorry, people who live outside the US.
            Return SkyEditorBase.ARDS.Region.US
        End Get
    End Property

    Public Overrides Function ToString() As String Implements SkyEditorBase.ARDS.CodeDefinitionV3.ToString
        'This class is going to be added directly to a dropdownlist, and will be displayed as whatever this function returns.
        'Change what it's shown as in Name, not here.
        'I see no need for you to change this function.
        Return Name
    End Function
    ''' <summary>
    ''' The code for generating, well, a code.
    ''' </summary>
    ''' <param name="Save">The GenericSave containing the data to make a code.</param>
    ''' <param name="TargetRegion">The region game to make a code for.</param>
    ''' <param name="ButtonActivator">Bit field containing which buttons to be pressed.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GenerateCode(Save As SkyEditorBase.GenericSave, TargetRegion As Region, ButtonActivator As UShort, CodeType As CheatFormat) As String Implements CodeDefinitionV3.GenerateCode
        'If you're making a code for Sky Editor, and not another plugin, you'll need to convert the GenericSave into one that contains the property you want
        'This line is for Blue Rescue Team
        Dim s As RBSave = DirectCast(Save, RBSave)

        'If you want to make a code for a different game, use one of these lines
        'Dim s As SkyEditor.RBSave = SkyEditor.TDSave.FromBase(Save)
        'Dim s As SkyEditor.RBSave = SkyEditor.SkySave.FromBase(Save)

        'We'll need to get a hex representation of the value for use in the code
        Dim moneyHex As String = Conversion.Hex(s.HeldMoney).PadLeft(8, "0")

        'For purposes of demonstration, I'm using a Select Case to show how to handle each supported Cheat Type.
        'More than likely, you will only use one, in which case, you can use the code for the appropriate code type in place of the entire Select Case
        Select Case CodeType
            Case CheatFormat.ARDS
                'The Best Part about using Sky Editor to make codes is that I have classes to make it easy.
                'This is how to make a code for Blue Rescue Team's held items
                Dim code As New SkyEditorBase.ARDS.CodeGeneratorHelper.Code 'Initialize the code

                code.Add(CodeGeneratorHelper.Line.IfButtonDown(ButtonActivator)) 'Add the IfButtonDown line.  You probably don't need to change this

                'Write the actual code here.  You may need additional code.Add() lines,
                'and you may find additional lines for your convenience in CodeGeneratorHelper.Line.
                'If not, you can make a new line as shown here.
                code.Add(New CodeGeneratorHelper.Line(String.Format("0213C12C {0}", moneyHex)))

                'The last line of all codes that have a button activator.
                'This is the equivilant of D2000000 00000000
                code.Add(CodeGeneratorHelper.Line.MasterEnd)
                Return code.ToString
            Case CheatFormat.CBA
                'Because of the different code format, there's a different helper class
                Dim code As New SkyEditorBase.ARDS.CBAHelper.Code
                'With Codebreaker cheats, only the line after an "If" is run, so it appears twice
                code.Add(CBAHelper.Line.IfButtonDown(ButtonActivator))
                code.Add(New CBAHelper.Line(String.Format("82038C08 {0}", moneyHex.Substring(4, 4)))) 'This is a 16 bit write
                'Codebreaker only supports up to 16 bit writes, so in this case, I split up the money value into two writes
                code.Add(CBAHelper.Line.IfButtonDown(ButtonActivator))
                code.Add(New CBAHelper.Line(String.Format("32038C0A {0}", moneyHex.Substring(0, 4)))) 'This is an 8 bit write
                'There is no end if, so nothing left to do but return the code
                Return code.ToString
            Case Else
                'There shouldn't be a case else because only ARDS and CBA are defined in SupportedCheatFormats below.
                'However, to get rid of the warning, I'm returning an empty string.
                Return ""
        End Select
    End Function

    ''' <summary>
    ''' This is an array of CheatFormats that this generator supports.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property SupportedCheatFormats As CheatFormat() Implements CodeDefinitionV3.SupportedCheatFormats
        Get
            Return {CheatFormat.ARDS, CheatFormat.CBA}
        End Get
    End Property
End Class
