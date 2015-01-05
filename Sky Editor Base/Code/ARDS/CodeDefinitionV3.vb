Namespace ARDS
    ''' <summary>
    ''' Interface a class must implement in order to be loaded as an ARDS plugin.
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface CodeDefinitionV3
        ''' <summary>
        ''' Generates an Action Replay DS code for the given save and region.
        ''' </summary>
        ''' <param name="Save">The save that contains the data to generate the code.  Use Save.IsSkySave to determine if it's for Explorers of Sky.  If so, DirectCast it to SkySave.  If not, DirectCast it to TimeDarknessSave or use generic properties.</param>
        ''' <param name="TargetRegion">The region to generate the code for.</param>
        ''' <param name="ButtonActivator">AND combination of ARDS.DSButton for use in activating the code.</param>
        ''' <param name="CodeType">Single CheatFormat that the user wants to be generated.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GenerateCode(ByVal Save As GenericSave, TargetRegion As Region, ButtonActivator As UInt16, CodeType As CheatFormat) As String 'CodeGeneratorHelper.Code

        ''' <summary>
        ''' Category this code definition belongs to.  CodeDefinitions of the same category will be grouped together.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Category As String
        ''' <summary>
        ''' Identifying name of this code definition.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Name As String
        ''' <summary>
        ''' Used when adding CodeDefinitions to GUI.
        ''' Should return Name.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function ToString() As String
        ''' <summary>
        ''' List of regions that GenerateCode() supports.
        ''' One or more ARDS.Region values joined by the OR bitwise operation.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property SupportedRegions As UInt16
        ''' <summary>
        ''' Author of this plugin.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Author As String
        ''' <summary>
        ''' An array of GameIDs this plugin is aware of.  Must be what is defined in SkyEditorPlugin.GameTypes, because while saves can have the same format across two or more games, chances are cheat codes only work for one.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property SupportedGames As String()
        ''' <summary>
        ''' Array containing what ARDS.CheatFormats this plugin knows how to output.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property SupportedCheatFormats As CheatFormat()
    End Interface
End Namespace