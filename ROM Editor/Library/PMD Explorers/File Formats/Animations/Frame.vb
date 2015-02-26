Namespace FileFormats
    Namespace Animations
        Public Class FrameList
            Public Property FrameGroups As New List(Of FrameGroup)
            Public Shared Function FromXml(XmlString As String) As FrameList
                Dim f As New FrameList
                Dim d As New Xml.XmlDocument
                d.LoadXml(XmlString)
                For Each item As Xml.XmlNode In d.SelectNodes("FrameList/FrameGroup")
                    f.FrameGroups.Add(FrameGroup.FromXml(item))
                Next
                Return f
            End Function
        End Class
        Public Class FrameGroup
            Public Property Frames As New List(Of Frame)
            Public Shared Function FromXml(XmlNode As Xml.XmlNode) As FrameGroup
                Dim f As New FrameGroup
                For Each item As Xml.XmlNode In XmlNode.SelectNodes("Frame")
                    f.Frames.Add(Frame.FromXml(item))
                Next
                Return f
            End Function
        End Class
        Public Class Frame
            Public Property ImageIndex As Integer
            Public Property Unk0 As Integer
            Public Property YOffset As Integer
            Public Property XOffset As Integer
            Public Property Unk1 As Integer
            Public Property ResolutionWidth As Integer
            Public Property ResolutionHeight As Integer
            Public Property VFlip As Boolean
            Public Property HFlip As Boolean
            Public Property Mosaic As Integer
            Public Property XOffsetBit6 As Boolean
            Public Property XOffsetBit7 As Boolean
            Public Property YOffsetBit3 As Boolean
            Public Property YOffsetBit5 As Boolean
            Public Property YOffsetBit6 As Boolean
            Public Shared Function FromXml(XmlNode As Xml.XmlNode) As Frame
                Dim f As New Frame
                f.ImageIndex = CInt(XmlNode.SelectSingleNode("ImageIndex").InnerText)
                f.Unk0 = Convert.ToInt32(XmlNode.SelectSingleNode("Unk0").InnerText.Replace("0x", ""), 16)
                f.YOffset = CInt(XmlNode.SelectSingleNode("YOffset").InnerText)
                f.XOffset = CInt(XmlNode.SelectSingleNode("XOffset").InnerText)
                f.Unk1 = Convert.ToInt32(XmlNode.SelectSingleNode("Unk1").InnerText.Replace("0x", ""), 16)
                f.ResolutionWidth = CInt(XmlNode.SelectSingleNode("Resolution/Width").InnerText)
                f.ResolutionHeight = CInt(XmlNode.SelectSingleNode("Resolution/Heigth").InnerText)
                f.VFlip = CInt(XmlNode.SelectSingleNode("VFlip").InnerText)
                f.HFlip = CInt(XmlNode.SelectSingleNode("HFlip").InnerText)
                f.Mosaic = CInt(XmlNode.SelectSingleNode("Mosaic").InnerText)
                f.XOffsetBit6 = CInt(XmlNode.SelectSingleNode("XOffsetBit6").InnerText)
                f.XOffsetBit7 = CInt(XmlNode.SelectSingleNode("XOffsetBit7").InnerText)
                f.YOffsetBit3 = CInt(XmlNode.SelectSingleNode("YOffsetBit3").InnerText)
                f.YOffsetBit5 = CInt(XmlNode.SelectSingleNode("YOffsetBit5").InnerText)
                f.YOffsetBit6 = CInt(XmlNode.SelectSingleNode("YOffsetBit6").InnerText)
                Return f
            End Function
        End Class
    End Namespace
End Namespace