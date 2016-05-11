Imports System.Reflection
Imports ROMEditor.FileFormats.Explorers.Script.Commands

Namespace FileFormats.Explorers.Script
    Partial Class SSB
        Public Class CommandInfo
            Public Property IsSky As Boolean
            Public Property CommandID As UInt16
            Public Property ParameterCount As UInt16
            Public Property CommandType As TypeInfo
            Public Sub New()

            End Sub
            Public Sub New(IsSky As Boolean, CommandID As UInt16, ParameterCount As UInt16, CommandType As Type)
                Me.IsSky = IsSky
                Me.CommandID = CommandID
                Me.ParameterCount = ParameterCount
                Me.CommandType = CommandType
            End Sub
        End Class

        'Todo: make readonly to public only
        Public Property CurrentCommandInfo As List(Of CommandInfo)

        Private Function GetSkyCommandInfo() As List(Of CommandInfo)
            Dim out As New List(Of CommandInfo)

            out.Add(New CommandInfo(True, &H0, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H1, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H2, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H3, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H4, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H5, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H6, 6, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H7, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H8, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H9, 1, GetType(LoadBottomPic)))
            out.Add(New CommandInfo(True, &HA, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &HB, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HC, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HD, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HE, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HF, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H10, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H11, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H12, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H13, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H14, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H15, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H16, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H17, 1, GetType(LoadTopPic)))
            out.Add(New CommandInfo(True, &H18, 1, GetType(LoadTopPic)))
            out.Add(New CommandInfo(True, &H19, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H1A, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H1B, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H1C, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H1D, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H1E, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H1F, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H20, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H21, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H22, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H23, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H24, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H25, 3, GetType(BgmFadeIn)))
            out.Add(New CommandInfo(True, &H26, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H27, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H28, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H29, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H2A, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H2B, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H2C, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H2D, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H2E, 4, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H2F, 4, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H30, 4, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H31, 4, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H32, 4, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H33, 4, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H34, 4, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H35, 4, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H36, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H37, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H38, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H39, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H3A, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H3B, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H3C, 0, GetType(NoParamCommand))) 'Unknown
            out.Add(New CommandInfo(True, &H3D, 5, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H3E, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H3F, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H40, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H41, 0, GetType(NoParamCommand))) 'Unknown
            out.Add(New CommandInfo(True, &H42, 5, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H43, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H44, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H45, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H46, 0, GetType(NoParamCommand))) 'Unknown
            out.Add(New CommandInfo(True, &H47, 5, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H48, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H49, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H4A, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H4B, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H4C, 4, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H4D, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H4E, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H4F, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H50, 0, GetType(NoParamCommand))) 'Unknown
            out.Add(New CommandInfo(True, &H51, 5, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H52, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H53, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H54, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H55, 0, GetType(NoParamCommand))) 'Unknown
            out.Add(New CommandInfo(True, &H56, 5, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H57, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H58, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H59, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H5A, 0, GetType(NoParamCommand))) 'Unknown
            out.Add(New CommandInfo(True, &H5B, 5, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H5C, 0, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H5D, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H5E, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H5F, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H60, 4, GetType(ImagePos)))
            out.Add(New CommandInfo(True, &H61, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H62, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H63, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H64, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H65, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H66, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H67, 2, GetType(CaseText)))
            out.Add(New CommandInfo(True, &H68, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H69, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H6A, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H6B, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H6C, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H6D, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H6E, 1, GetType(CaseTextDefault)))
            out.Add(New CommandInfo(True, &H6F, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H70, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H71, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H72, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H73, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H74, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H75, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H76, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H77, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H78, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H79, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H7A, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H7B, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H7C, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H7D, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H7E, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H7F, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H80, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H81, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H82, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H83, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H84, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H85, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H86, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H87, 1, GetType(GotoCommandRaw)))
            out.Add(New CommandInfo(True, &H88, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H89, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H8A, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H8B, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H8C, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H8D, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H8E, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H8F, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H90, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H91, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H92, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H93, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H94, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H95, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H96, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H97, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H98, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H99, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H9A, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H9B, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H9C, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H9D, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H9E, 1, GetType(MonologueCommand)))
            out.Add(New CommandInfo(True, &H9F, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HA0, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HA1, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &HA2, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &HA3, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HA4, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HA5, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HA6, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HA7, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HA8, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HA9, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HAA, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HAB, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HAC, 1, GetType(SwitchTalk)))
            out.Add(New CommandInfo(True, &HAD, 1, GetType(SwitchTalk)))
            out.Add(New CommandInfo(True, &HAE, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HAF, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HB0, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HB1, 0, GetType(NoParamCommand))) 'Unknown
            out.Add(New CommandInfo(True, &HB2, 5, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HB3, 0, GetType(NoParamCommand))) 'Unknown
            out.Add(New CommandInfo(True, &HB4, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HB5, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HB6, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HB7, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HB8, 0, GetType(NoParamCommand))) 'Unknown
            out.Add(New CommandInfo(True, &HB9, 5, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HBA, 0, GetType(NoParamCommand))) 'Unknown
            out.Add(New CommandInfo(True, &HBB, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HBC, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HBD, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HBE, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HBF, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HC0, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HC1, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HC2, 10, GetType(RawCommand))) 'Unknown
            out.Add(New CommandInfo(True, &HC3, 5, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HC4, 6, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HC5, 0, GetType(NoParamCommand))) 'Unknown
            out.Add(New CommandInfo(True, &HC6, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HC7, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HC8, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HC9, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HCA, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HCB, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HCC, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HCD, 4, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HCE, 4, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HCF, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HD0, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HD1, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HD2, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HD3, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HD4, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HD5, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &HD6, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HD7, 4, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HD8, 4, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HD9, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HDA, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HDB, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HDC, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HDD, 8, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HDE, 6, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HDF, 6, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HE0, 4, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HE1, 4, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HE2, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HE3, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HE4, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HE5, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HE6, 4, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HE7, 4, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HE8, 2, GetType(FadeTopBg)))
            out.Add(New CommandInfo(True, &HE9, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HEA, 2, GetType(FadeTopBg)))
            out.Add(New CommandInfo(True, &HEB, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HEC, 8, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HED, 6, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HEE, 6, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HEF, 4, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HF0, 4, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HF1, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HF2, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HF3, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HF4, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HF5, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HF6, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HF7, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HF8, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HF9, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HFA, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HFB, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HFC, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HFD, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HFE, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &HFF, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H100, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H101, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H102, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H103, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H104, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H105, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H106, 6, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H107, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H108, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H109, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H10A, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H10B, 4, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H10C, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H10D, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H10E, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H10F, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H110, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H111, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H112, 0, GetType(NoParamCommand))) 'Unknown
            out.Add(New CommandInfo(True, &H113, 5, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H114, 0, GetType(NoParamCommand))) 'Unknown
            out.Add(New CommandInfo(True, &H115, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H116, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H117, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H118, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H119, 0, GetType(NoParamCommand))) 'Unknown
            out.Add(New CommandInfo(True, &H11A, 5, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H11B, 0, GetType(NoParamCommand))) 'Unknown
            out.Add(New CommandInfo(True, &H11C, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H11D, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H11E, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H11F, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H120, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H121, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H122, 0, GetType(NoParamCommand))) 'Unknown
            out.Add(New CommandInfo(True, &H123, 5, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H124, 6, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H125, 0, GetType(NoParamCommand))) 'Unknown
            out.Add(New CommandInfo(True, &H126, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H127, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H128, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H129, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H12A, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H12B, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H12C, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H12D, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H12E, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H12F, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H130, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H131, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H132, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H133, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H134, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H135, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H136, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H137, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H138, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H139, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H13A, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H13B, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H13C, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H13D, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H13E, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H13F, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H140, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H141, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H142, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H143, 4, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H144, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H145, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H146, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H147, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H148, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H149, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H14A, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H14B, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H14C, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H14D, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H14E, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H14F, 8, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H150, 3, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H151, 4, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H152, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H153, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H154, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H155, 5, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H156, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H157, 1, GetType(Delay)))
            out.Add(New CommandInfo(True, &H158, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H159, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H15A, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H15B, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H15C, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H15D, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H15E, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H15F, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H160, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H161, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H162, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H163, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H164, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H165, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H166, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H167, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H168, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H169, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H16A, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H16B, 2, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H16C, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H16D, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H16E, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H16F, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H170, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H171, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H172, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H173, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H174, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H175, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H176, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H177, 0, GetType(NoParamCommand)))
            out.Add(New CommandInfo(True, &H178, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H179, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H17A, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H17B, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H17C, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H17D, 1, GetType(RawCommand)))
            out.Add(New CommandInfo(True, &H17E, 1, GetType(RawCommand)))

            Return out
        End Function

        Public Function CreateCommand(Info As CommandInfo) As RawCommand
            Dim instance As RawCommand = SkyEditor.Core.Utilities.ReflectionHelpers.CreateInstance(Info.CommandType)

            instance.CommandID = Info.CommandID
            instance.Params = New List(Of UShort)
            For count = 0 To Info.ParameterCount - 1
                instance.Params.Add(0)
            Next

            'Initialize relevant properties
            Dim paramAttributeType = GetType(CommandParameterAttribute)
            For Each item In Info.CommandType.GetProperties
                For Each attribute In item.GetCustomAttributes(True)
                    If TypeOf attribute Is CommandParameterAttribute Then
                        Dim paramInfo = DirectCast(attribute, CommandParameterAttribute)

                        Select Case item.PropertyType
                            Case GetType(StringCommandParameter)
                                If isMultiLang Then
                                    Dim stringParam As New MultiLangStringCommandParameter
                                    item.SetValue(instance, stringParam)
                                Else
                                    Dim stringParam As New StringCommandParameter
                                    item.SetValue(instance, stringParam)
                                End If
                            Case GetType(ConstantCommandParameter)
                                Dim stringParam As New ConstantCommandParameter
                                item.SetValue(instance, stringParam)
                            Case GetType(GotoTarget)
                                Dim target As New GotoTarget
                                item.SetValue(instance, target)

                        End Select

                    End If
                Next
            Next

            Return instance
        End Function

        ''' <summary>
        ''' Creates a Command for Explorers of Sky using the given data.
        ''' </summary>
        ''' <param name="CommandID">ID of the Command</param>
        ''' <param name="Params">Parameters of the Command</param>
        ''' <returns></returns>
        Private Function CreateCommand(CommandID As UInt16, Params As IEnumerable(Of UInt16)) As RawCommand
            'Select the command type
            Dim commandType As Type = (From i In CurrentCommandInfo Where i.CommandID = CommandID Select i).First.CommandType

            'Initialize the command
            Dim cmd As RawCommand = commandType.GetConstructor({}).Invoke({})
            'Set the default data
            cmd.CommandID = CommandID
            cmd.Params = Params
            cmd.IsEoS = True

            'Use attributes on Properties to make parameter interaction more natural.
            Dim paramAttributeType = GetType(CommandParameterAttribute)
            For Each item In commandType.GetProperties
                For Each attribute In item.GetCustomAttributes(True)
                    If TypeOf attribute Is CommandParameterAttribute Then
                        Dim paramInfo = DirectCast(attribute, CommandParameterAttribute)

                        Select Case item.PropertyType
                            Case GetType(UInt16), GetType(UInt32), GetType(UInt64), GetType(UShort), GetType(UInteger), GetType(ULong)
                                item.SetValue(cmd, Params(paramInfo.Index))
                            Case GetType(Int16), GetType(Short)
                                item.SetValue(cmd, BitConverter.ToInt16(BitConverter.GetBytes(Params(paramInfo.Index)), 0))
                            'Case GetType(Int32), GetType(Integer)
                            '    item.SetValue(cmd, BitConverter.ToInt32(BitConverter.GetBytes(Params(paramInfo.Index)), 0))
                            'Case GetType(Int64), GetType(Long)
                            '    item.SetValue(cmd, BitConverter.ToInt64(BitConverter.GetBytes(Params(paramInfo.Index)), 0))
                            Case GetType(StringCommandParameter)
                                If isMultiLang Then
                                    Dim stringParam As New MultiLangStringCommandParameter
                                    stringParam.English = English(Params(paramInfo.Index))
                                    stringParam.French = French(Params(paramInfo.Index))
                                    stringParam.German = German(Params(paramInfo.Index))
                                    stringParam.Italian = Italian(Params(paramInfo.Index))
                                    stringParam.Spanish = Spanish(Params(paramInfo.Index))
                                    item.SetValue(cmd, stringParam)
                                Else
                                    Dim stringParam As New StringCommandParameter
                                    stringParam.English = English(Params(paramInfo.Index))
                                    item.SetValue(cmd, stringParam)
                                End If
                                UnreferencedStringIndexes.Remove(Params(paramInfo.Index))
                            Case GetType(ConstantCommandParameter)
                                Dim stringParam As New ConstantCommandParameter
                                stringParam.Constant = Constants(Params(paramInfo.Index))
                                UnreferencedConstantIndexes.Remove(Params(paramInfo.Index))
                                item.SetValue(cmd, stringParam)
                            Case GetType(GotoTarget)
                                'This will be processed at a later time.  For now, we'll just keep track of this command.
                                'After all commands have been created, its Goto Target will be set to a particular label, using the raw value stored in cmd.Params
                                If Not GotoTargetCommands.Contains(cmd) Then
                                    GotoTargetCommands.Add(cmd)
                                End If
                            Case Else
                                Throw New InvalidCastException(My.Resources.Language.ErrorScriptCommandAttributeInvalidType)
                        End Select

                    End If
                Next
            Next

            Return cmd
        End Function

        ''' <summary>
        ''' Gets a byte array representing the given command.
        ''' 
        ''' </summary>
        ''' <param name="Command"></param>
        ''' <returns></returns>
        Private Function GetCommandBytes(Command As RawCommand) As Byte()
            Dim commandType = Command.GetType
            Dim params As List(Of UInt16) = Command.Params
            'Check the properties with attributes to make things go smoother
            Dim paramAttributeType = GetType(CommandParameterAttribute)
            For Each item In commandType.GetProperties
                For Each attribute In item.GetCustomAttributes(True)
                    If TypeOf attribute Is CommandParameterAttribute Then
                        Dim index = DirectCast(attribute, CommandParameterAttribute).Index

                        Select Case item.PropertyType
                            Case GetType(UInt16), GetType(UInt32), GetType(UInt64), GetType(UShort), GetType(UInteger), GetType(ULong)
                                params(index) = item.GetValue(Command)
                            Case GetType(Int16), GetType(Short)
                                'We don't want any overflow due to negatives, or numbers that are over 2 bytes, so we'll allow all overflow
                                params(index) = BitConverter.ToUInt16(BitConverter.GetBytes(item.GetValue(Command)), 0)
                            'Case GetType(Int32), GetType(Integer)
                            '    'We don't want any overflow due to negatives, or numbers that are over 2 bytes, so we'll allow all overflow
                            '    params(index) = BitConverter.ToUInt16(BitConverter.GetBytes(item.GetValue(Command)), 0)
                            'Case GetType(Int64), GetType(Long)
                            '    'We don't want any overflow due to negatives, or numbers that are over 2 bytes, so we'll allow all overflow
                            '    params(index) = BitConverter.ToUInt16(BitConverter.GetBytes(item.GetValue(Command)), 0)
                            Case GetType(StringCommandParameter)
                                Dim stringParam As StringCommandParameter = item.GetValue(Command)
                                If AvailableStringIndexes.Count > 0 Then
                                    'Overwrite available entries in the string table if we can
                                    Dim stringIndex = AvailableStringIndexes(0)
                                    AvailableStringIndexes.RemoveAt(0)

                                    English(stringIndex) = stringParam.English
                                    If isMultiLang Then
                                        If TypeOf stringParam Is MultiLangStringCommandParameter Then
                                            With DirectCast(stringParam, MultiLangStringCommandParameter)
                                                French(stringIndex) = .French
                                                German(stringIndex) = .German
                                                Italian(stringIndex) = .Italian
                                                Spanish(stringIndex) = .Spanish
                                            End With
                                        Else
                                            'That's odd.  We're in multi-language mode, but don't have a multi-language parameter.  Someone must have tampered with something.
                                            'In this case, we'll just use the English for the other languages
                                            French(stringIndex) = stringParam.English
                                            German(stringIndex) = stringParam.English
                                            Italian(stringIndex) = stringParam.English
                                            Spanish(stringIndex) = stringParam.English
                                        End If
                                    End If

                                    params(index) = stringIndex
                                Else
                                    'Otherwise, add new ones
                                    English.Add(stringParam.English)
                                    If isMultiLang Then
                                        If TypeOf stringParam Is MultiLangStringCommandParameter Then
                                            With DirectCast(stringParam, MultiLangStringCommandParameter)
                                                French.Add(.French)
                                                German.Add(.German)
                                                Italian.Add(.Italian)
                                                Spanish.Add(.Spanish)
                                            End With
                                        Else
                                            'That's odd.  We're in multi-language mode, but don't have a multi-language parameter.  Someone must have tampered with something.
                                            'In this case, we'll just use the English for the other languages
                                            French.Add(stringParam.English)
                                            German.Add(stringParam.English)
                                            Italian.Add(stringParam.English)
                                            Spanish.Add(stringParam.English)
                                        End If
                                    End If

                                    params(index) = English.Count - 1
                                End If
                            Case GetType(ConstantCommandParameter)
                                Dim constParam As ConstantCommandParameter = item.GetValue(Command)
                                If AvailableStringIndexes.Count > 0 Then
                                    'Overwrite available entries in the string table if we can
                                    Dim constIndex = AvailableConstantIndexes(0)
                                    AvailableConstantIndexes.RemoveAt(0)
                                    English(constIndex) = constParam.Constant
                                    params(index) = constIndex
                                Else
                                    'Otherwise, add new ones
                                    Constants.Add(constParam.Constant)
                                    params(index) = Constants.Count - 1
                                End If
                            Case GetType(GotoTarget)
                                'At this point in the saving cycle, we've already updated the appropriate parameter.
                                'Take no action
                            Case Else
                                Throw New InvalidCastException(My.Resources.Language.ErrorScriptCommandAttributeInvalidType)
                        End Select

                    End If
                Next
            Next

            'The hard part is over.  All the parameters are already in the list declared above.
            'Time to convert to binary!
            Dim buffer As New List(Of Byte)
            buffer.AddRange(BitConverter.GetBytes(Command.CommandID))
            For Each item In params
                buffer.AddRange(BitConverter.GetBytes(item))
            Next

            Return buffer.ToArray
        End Function

    End Class
End Namespace

