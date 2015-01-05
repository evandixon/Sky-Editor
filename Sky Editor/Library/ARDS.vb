Imports System.IO
Namespace ARDS
    ''' <summary>
    ''' Contains methods that help code makers make codes.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class CodeGeneratorHelper
        Public Class Line
            Dim _rawData As String
            Public Property RawData As String
                Get
                    Return _rawData.Trim.Substring(0, 17)
                End Get
                Set(value As String)
                    _rawData = value.Trim.Substring(0, 17)
                End Set
            End Property
            Private Property Section(Index As Byte) As String
                Get
                    Return RawData.Split(" ")(Index And 1)
                End Get
                Set(value As String)
                    If Index = 0 Then
                        _rawData = value.Trim.Substring(0, 8) & " " & RawData.Split(" ")(1)
                    Else
                        _rawData = RawData.Split(" ")(0) & " " & value.Trim.Substring(0, 8)
                    End If
                End Set
            End Property
            Public Property Part1 As String
                Get
                    Return Section(0)
                End Get
                Set(value As String)
                    Section(1) = value
                End Set
            End Property
            Public Property Part2 As String
                Get
                    Return Section(1)
                End Get
                Set(value As String)
                    Section(1) = value
                End Set
            End Property
            Public Shared Narrowing Operator CType(ByVal x As String) As Line
                Return New Line(x)
            End Operator
            Public Shared Widening Operator CType(ByVal x As Line) As String
                Return x.RawData
            End Operator
            Public Sub New(RawData As String)
                Me.RawData = RawData
            End Sub
            Public Overrides Function ToString() As String
                Return RawData
            End Function

            ''' <summary>
            ''' Ends the previous If statement
            ''' </summary>
            ''' <remarks></remarks>
            Public Const LocalEndIf As String = "D0000000 00000000"

            ''' <summary>
            ''' Ends the previous repeat
            ''' </summary>
            ''' <remarks></remarks>
            Public Const LocalEndRepeat As String = "D1000000 00000000"

            ''' <summary>
            ''' Ends all previous If statements and repeats
            ''' </summary>
            ''' <remarks></remarks>
            Public Const MasterEnd As String = "D2000000 00000000"

            ''' <summary>
            ''' Gets the If Button Down line of an ARDS code.
            ''' </summary>
            ''' <param name="Buttons">AND combination of ARDS.DSButton</param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Shared Function IfButtonDown(Buttons As UInt16) As String
                Dim b As Byte() = BitConverter.GetBytes(Buttons)
                Return "92FFFFA8 " & Conversion.Hex(b(1)).PadLeft(2, "0") & Conversion.Hex(b(0)).PadLeft(2, "0") & "0000"
            End Function

            ''' <summary>
            ''' Gets the If Button Not Down line of an ARDS code.
            ''' </summary>
            ''' <param name="Buttons">AND combination of ARDS.DSButton</param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Shared Function IfNotButtonDown(Buttons As UInt16) As String
                Dim b As Byte() = BitConverter.GetBytes(Buttons)
                Return "A2FFFFA8 " & Conversion.Hex(b(1)).PadLeft(2, "0") & Conversion.Hex(b(0)).PadLeft(2, "0") & "0000"
            End Function

            ''' <summary>
            ''' Gets the If Button Down line of an ARDS code using the GBA compatible offset.
            ''' X, Y, and NDS_Not_Folded not supported.
            ''' </summary>
            ''' <param name="Buttons">AND combination of ARDS.DSButton</param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Shared Function IfGBAButtonDown(Buttons As UInt16) As String
                Dim b As Byte() = BitConverter.GetBytes(Buttons)
                Return "92FFFFA8 " & Conversion.Hex(b(1)).PadLeft(2, "0") & Conversion.Hex(b(0)).PadLeft(2, "0") & "0000"
            End Function

            ''' <summary>
            ''' Gets the If Button Not Down line of an ARDS code using the GBA compatible offset.
            ''' X, Y, and NDS_Not_Folded not supported.
            ''' </summary>
            ''' <param name="Buttons">AND combination of ARDS.DSButton</param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Shared Function IfGBAButtonNotDown(Buttons As UInt16) As String
                Dim b As Byte() = BitConverter.GetBytes(Buttons)
                Return "A2FFFFA8 " & Conversion.Hex(b(1)).PadLeft(2, "0") & Conversion.Hex(b(0)).PadLeft(2, "0") & "0000"
            End Function
        End Class
        Public Class Code
            Inherits Generic.List(Of Line)
            Public Overrides Function ToString() As String
                Dim out As String = ""
                For Each Line In Me
                    out = out & Line.ToString & vbCrLf
                Next
                Return out.Trim
            End Function
            Public Sub New()
                MyBase.New()
            End Sub
            Public Sub New(Code As String)
                MyBase.New()
                Code = Code.Trim.Replace(" ", "").Replace(vbCr, "").Replace(vbLf, "")
                If Count Mod 8 = 0 Then
                    For Count As Integer = 0 To Code.Length - 1 Step 16
                        Me.Add(New Line(Code.Substring(Count, Count + 7) & " " & Code.Substring(Count + 8, Count + 15)))
                    Next
                End If
            End Sub
        End Class
    End Class

    ''' <summary>
    ''' Depricated; use CodeDefinitionV2
    ''' Interface a class must implement in order to be loaded as an ARDS plugin.
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface CodeDefinition
        ''' <summary>
        ''' Generates an Action Replay DS code for the given save and region.
        ''' </summary>
        ''' <param name="Save">The save that contains the data to generate the code.  Use Save.IsSkySave to determine if it's for Explorers of Sky.  If so, DirectCast it to SkySave.  If not, DirectCast it to TimeDarknessSave or use generic properties.</param>
        ''' <param name="TargetRegion">The region to generate the code for.</param>
        ''' <param name="ButtonActivator">AND combination of ARDS.DSButton for use in activating the code.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GenerateCode(ByVal Save As GenericSave, TargetRegion As Region, ButtonActivator As UInt16) As CodeGeneratorHelper.Code

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
    End Interface

    ''' <summary>
    ''' Interface a class must implement in order to be loaded as an ARDS plugin.
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface CodeDefinitionV2
        ''' <summary>
        ''' Generates an Action Replay DS code for the given save and region.
        ''' </summary>
        ''' <param name="Save">The save that contains the data to generate the code.  Use Save.IsSkySave to determine if it's for Explorers of Sky.  If so, DirectCast it to SkySave.  If not, DirectCast it to TimeDarknessSave or use generic properties.</param>
        ''' <param name="TargetRegion">The region to generate the code for.</param>
        ''' <param name="ButtonActivator">AND combination of ARDS.DSButton for use in activating the code.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GenerateCode(ByVal Save As GenericSave, TargetRegion As Region, ButtonActivator As UInt16) As CodeGeneratorHelper.Code

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
        ''' Specifies whether or not this plugin supports Explorers of Sky
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property SupportsSky As Boolean
        ''' <summary>
        ''' Specifies whether or not this plugin supports Explorers of Time/Darkness
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property SupportsTD As Boolean
    End Interface

    ''' <summary>
    ''' List of most if not all game regions that could yield a differnt code.
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum Region
        Japan = 1
        US = 2
        UK = 4
        Germany = 8
        Korea = 16
        Australia = 32
        Spain = 64
        France = 128
        Netherlands = 256
        Italy = 512
        Denmark = 1024
    End Enum

    ''' <summary>
    ''' List of buttons on the DS
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum DSButton
        Y = &HF7FF
        X = &HFBFF
        L = &HFDFF
        R = &HFEFF
        Down = &HFF7F
        Up = &HFFBF
        Right = &HFFDF
        Left = &HFFEF
        Start = &HFFF7
        [Select] = &HFFFB
        B = &HFFFD
        A = &HFFFE
    End Enum

    ''' <summary>
    ''' Manages ARDS plugins, retaining compatibility with any plugins that may or may not have been made yet
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ManagerV1
        Private Shared _codeDefinitions As List(Of CodeDefinition)
        ''' <summary>
        ''' Gets the cached list of CodeDefinitions.  If null, they will be loaded.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared ReadOnly Property CodeDefinitions As List(Of CodeDefinition)
            Get
                If CodeDefinitionsLoaded Then
                    Return _codeDefinitions
                Else
                    Return LoadCodeDefinitions()
                End If
            End Get
        End Property

        ''' <summary>
        ''' Returns whether or not CodeDefinitions have been loaded.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared ReadOnly Property CodeDefinitionsLoaded As Boolean
            Get
                Return _codeDefinitions IsNot Nothing
            End Get
        End Property

        ''' <summary>
        ''' Reloads the CodeDefinition cache and returns it.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function LoadCodeDefinitions() As List(Of CodeDefinition)
            Const PluginFolder = "Resources/ARDS"
            Dim out As New List(Of CodeDefinition)
            If IO.Directory.Exists(Path.Combine(Environment.CurrentDirectory, PluginFolder)) Then
                Dim plugins As String() = IO.Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, PluginFolder))
                For Each plugin In plugins
                    Try
                        Dim a As System.Reflection.Assembly = System.Reflection.Assembly.LoadFrom(plugin)
                        Dim types As Type() = a.GetTypes
                        For Each item In types
                            Dim IsDefinition As Boolean = False
                            For Each intface As Type In item.GetInterfaces
                                If intface Is GetType(CodeDefinition) Then
                                    IsDefinition = True
                                    Exit For
                                End If
                            Next
                            If IsDefinition Then
                                Dim CurrentCollection As CodeDefinition = a.CreateInstance(item.ToString)
                                out.Add(CurrentCollection)
                            End If
                        Next
                    Catch ex As Exception
                        If Settings.DebugMode Then
                            MessageBox.Show("Error loading plugin """ & plugin & """:" & vbCrLf & ex.ToString)
                            'Else swallow error and just not load the plugin
                        End If
                    End Try
                Next
            End If
            Return out
        End Function

        Private Shared _categories As List(Of String)
        ''' <summary>
        ''' Gets a list of the categories of plugins
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared ReadOnly Property Categories As List(Of String)
            Get
                If _categories Is Nothing Then
                    _categories = New List(Of String)
                    For Each item In CodeDefinitions
                        If Not _categories.Contains(item.Category) Then
                            _categories.Add(item.Category)
                        End If
                    Next
                End If
                Return _categories
            End Get
        End Property
        Public Shared Function GetDefinitionsForCategory(CategoryName As String) As List(Of CodeDefinition)
            Dim out As New List(Of CodeDefinition)
            For Each item In CodeDefinitions
                If item.Category = CategoryName Then out.Add(item)
            Next
            Return out
        End Function
        Public Shared Function GetDefinitionsForCategory(CategoryName As String, Region As Region) As List(Of CodeDefinition)
            Dim out As New List(Of CodeDefinition)
            For Each item In CodeDefinitions
                If item.Category = CategoryName AndAlso (item.SupportedRegions Or Region = item.SupportedRegions) Then out.Add(item)
            Next
            Return out
        End Function

        Private Shared _regions As List(Of Region)
        Public Shared ReadOnly Property Regions As List(Of Region)
            Get
                If _regions Is Nothing Then
                    _regions = New List(Of Region)
                    For Each item In CodeDefinitions
                        Dim bytes() As Byte = BitConverter.GetBytes(item.SupportedRegions)
                        If CType(bytes(0), Bit8).Bit1 Then
                            If Not _regions.Contains(Region.Japan) Then
                                _regions.Add(Region.Japan)
                            End If
                        End If
                        If CType(bytes(0), Bit8).Bit2 Then
                            If Not _regions.Contains(Region.US) Then
                                _regions.Add(Region.US)
                            End If
                        End If
                        If CType(bytes(0), Bit8).Bit3 Then
                            If Not _regions.Contains(Region.UK) Then
                                _regions.Add(Region.UK)
                            End If
                        End If
                        If CType(bytes(0), Bit8).Bit4 Then
                            If Not _regions.Contains(Region.Germany) Then
                                _regions.Add(Region.Germany)
                            End If
                        End If
                        If CType(bytes(0), Bit8).Bit5 Then
                            If Not _regions.Contains(Region.Korea) Then
                                _regions.Add(Region.Korea)
                            End If
                        End If
                        If CType(bytes(0), Bit8).Bit6 Then
                            If Not _regions.Contains(Region.Australia) Then
                                _regions.Add(Region.Australia)
                            End If
                        End If
                        If CType(bytes(0), Bit8).Bit7 Then
                            If Not _regions.Contains(Region.Spain) Then
                                _regions.Add(Region.Spain)
                            End If
                        End If
                        If CType(bytes(0), Bit8).Bit8 Then
                            If Not _regions.Contains(Region.France) Then
                                _regions.Add(Region.France)
                            End If
                        End If
                        If CType(bytes(1), Bit8).Bit1 Then
                            If Not _regions.Contains(Region.Netherlands) Then
                                _regions.Add(Region.Netherlands)
                            End If
                        End If
                        If CType(bytes(1), Bit8).Bit2 Then
                            If Not _regions.Contains(Region.Italy) Then
                                _regions.Add(Region.Italy)
                            End If
                        End If
                        If CType(bytes(1), Bit8).Bit3 Then
                            If Not _regions.Contains(Region.Denmark) Then
                                _regions.Add(Region.Denmark)
                            End If
                        End If
                    Next
                End If
                Return _regions
            End Get
        End Property

        Public Shared Function GetCategoriesForRegion(Region As Region) As List(Of String)
            Dim out As New List(Of String)
            For Each item In CodeDefinitions
                'If adding the region doesn't yield any change, it must have already been there
                If item.SupportedRegions Or Region = item.SupportedRegions Then
                    If Not out.Contains(item.Category) Then
                        out.Add(item.Category)
                    End If
                End If
            Next
            Return out
        End Function
    End Class
    'Manages ARDS plugins that specify whether or not they support Sky or Time/Darkness
    Public Class ManagerV2
        Private Shared _codeDefinitions As List(Of CodeDefinition)
        ''' <summary>
        ''' Gets the cached list of CodeDefinitions.  If null, they will be loaded.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared ReadOnly Property CodeDefinitions As List(Of CodeDefinition)
            Get
                If CodeDefinitionsLoaded Then
                    Return _codeDefinitions
                Else
                    Return LoadCodeDefinitions()
                End If
            End Get
        End Property

        ''' <summary>
        ''' Returns whether or not CodeDefinitions have been loaded.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared ReadOnly Property CodeDefinitionsLoaded As Boolean
            Get
                Return _codeDefinitions IsNot Nothing
            End Get
        End Property

        ''' <summary>
        ''' Reloads the CodeDefinition cache and returns it.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function LoadCodeDefinitions() As List(Of CodeDefinition)
            Const PluginFolder = "Resources/ARDS2"
            Dim out As New List(Of CodeDefinition)
            If IO.Directory.Exists(Path.Combine(Environment.CurrentDirectory, PluginFolder)) Then
                Dim plugins As String() = IO.Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, PluginFolder))
                For Each plugin In plugins
                    Try
                        Dim a As System.Reflection.Assembly = System.Reflection.Assembly.LoadFrom(plugin)
                        Dim types As Type() = a.GetTypes
                        For Each item In types
                            Dim IsDefinition As Boolean = False
                            For Each intface As Type In item.GetInterfaces
                                If intface Is GetType(CodeDefinitionv2) Then
                                    IsDefinition = True
                                    Exit For
                                End If
                            Next
                            If IsDefinition Then
                                Dim CurrentCollection As CodeDefinition = a.CreateInstance(item.ToString)
                                out.Add(CurrentCollection)
                            End If
                        Next
                    Catch ex As Exception
                        If Settings.DebugMode Then
                            MessageBox.Show("Error loading plugin """ & plugin & """:" & vbCrLf & ex.ToString)
                            'Else swallow error and just not load the plugin
                        End If
                    End Try
                Next
            End If
            Return out
        End Function

        Private Shared _categories As List(Of String)
        ''' <summary>
        ''' Gets a list of the categories of plugins
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared ReadOnly Property Categories As List(Of String)
            Get
                If _categories Is Nothing Then
                    _categories = New List(Of String)
                    For Each item In CodeDefinitions
                        If Not _categories.Contains(item.Category) Then
                            _categories.Add(item.Category)
                        End If
                    Next
                End If
                Return _categories
            End Get
        End Property
        Public Shared Function GetDefinitionsForCategory(CategoryName As String) As List(Of CodeDefinition)
            Dim out As New List(Of CodeDefinition)
            For Each item In CodeDefinitions
                If item.Category = CategoryName Then out.Add(item)
            Next
            Return out
        End Function
        Public Shared Function GetDefinitionsForCategory(CategoryName As String, Region As Region) As List(Of CodeDefinition)
            Dim out As New List(Of CodeDefinition)
            For Each item In CodeDefinitions
                If item.Category = CategoryName AndAlso (item.SupportedRegions Or Region = item.SupportedRegions) Then out.Add(item)
            Next
            Return out
        End Function

        Private Shared _regions As List(Of Region)
        Public Shared ReadOnly Property Regions As List(Of Region)
            Get
                If _regions Is Nothing Then
                    _regions = New List(Of Region)
                    For Each item In CodeDefinitions
                        Dim bytes() As Byte = BitConverter.GetBytes(item.SupportedRegions)
                        If CType(bytes(0), Bit8).Bit1 Then
                            If Not _regions.Contains(Region.Japan) Then
                                _regions.Add(Region.Japan)
                            End If
                        End If
                        If CType(bytes(0), Bit8).Bit2 Then
                            If Not _regions.Contains(Region.US) Then
                                _regions.Add(Region.US)
                            End If
                        End If
                        If CType(bytes(0), Bit8).Bit3 Then
                            If Not _regions.Contains(Region.UK) Then
                                _regions.Add(Region.UK)
                            End If
                        End If
                        If CType(bytes(0), Bit8).Bit4 Then
                            If Not _regions.Contains(Region.Germany) Then
                                _regions.Add(Region.Germany)
                            End If
                        End If
                        If CType(bytes(0), Bit8).Bit5 Then
                            If Not _regions.Contains(Region.Korea) Then
                                _regions.Add(Region.Korea)
                            End If
                        End If
                        If CType(bytes(0), Bit8).Bit6 Then
                            If Not _regions.Contains(Region.Australia) Then
                                _regions.Add(Region.Australia)
                            End If
                        End If
                        If CType(bytes(0), Bit8).Bit7 Then
                            If Not _regions.Contains(Region.Spain) Then
                                _regions.Add(Region.Spain)
                            End If
                        End If
                        If CType(bytes(0), Bit8).Bit8 Then
                            If Not _regions.Contains(Region.France) Then
                                _regions.Add(Region.France)
                            End If
                        End If
                        If CType(bytes(1), Bit8).Bit1 Then
                            If Not _regions.Contains(Region.Netherlands) Then
                                _regions.Add(Region.Netherlands)
                            End If
                        End If
                        If CType(bytes(1), Bit8).Bit2 Then
                            If Not _regions.Contains(Region.Italy) Then
                                _regions.Add(Region.Italy)
                            End If
                        End If
                        If CType(bytes(1), Bit8).Bit3 Then
                            If Not _regions.Contains(Region.Denmark) Then
                                _regions.Add(Region.Denmark)
                            End If
                        End If
                    Next
                End If
                Return _regions
            End Get
        End Property

        Public Shared Function GetCategoriesForRegion(Region As Region) As List(Of String)
            Dim out As New List(Of String)
            For Each item In CodeDefinitions
                'If adding the region doesn't yield any change, it must have already been there
                If item.SupportedRegions Or Region = item.SupportedRegions Then
                    If Not out.Contains(item.Category) Then
                        out.Add(item.Category)
                    End If
                End If
            Next
            Return out
        End Function
    End Class
End Namespace

