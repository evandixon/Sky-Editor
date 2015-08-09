Namespace FileFormats
    Namespace Animations
        Public Class AnimData
            Public Property AnimGroupData As New List(Of AnimGroup)
            Public Property AnimSequenceData As New List(Of AnimSequence)
            Public Shared Function FromXml(XmlString As String) As AnimData
                Dim a As New AnimData
                Dim d As New Xml.XmlDocument
                d.LoadXml(XmlString)
                For Each item As Xml.XmlNode In d.SelectNodes("AnimData/AnimGroupTable/AnimGroup")
                    a.AnimGroupData.Add(AnimGroup.FromXml(item))
                Next
                For Each item As Xml.XmlNode In d.SelectSingleNode("AnimData/AnimSequenceTable")
                    If Not item.OuterXml.StartsWith("<!") Then
                        a.AnimSequenceData.Add(AnimSequence.FromXml(item))
                    End If
                Next
                Return a
            End Function
        End Class
        Public Class AnimGroup
            Public Property Name As String
            Public Property Frames As New List(Of Integer)
            Public Function ToXml() As String
                Dim d As New Text.StringBuilder
                Using w As Xml.XmlWriter = Xml.XmlWriter.Create(d)
                    w.WriteStartElement("AnimGroup")
                    w.WriteAttributeString("name", Name)
                    For Each item In Frames
                        w.WriteStartElement("AnimSequenceIndex")
                        w.WriteValue(item)
                        w.WriteEndElement()
                    Next
                    w.WriteEndElement()
                End Using
                Return d.ToString
            End Function
            Public Shared Function FromXml(Node As Xml.XmlNode) As AnimGroup
                Dim a As New AnimGroup
                a.Name = Node.Attributes("name").Value
                For Each item As Xml.XmlNode In Node.SelectNodes("AnimSequenceIndex")
                    a.Frames.Add(CInt(item.InnerText))
                Next
                Return a
            End Function
        End Class
        Public Class AnimSequence
            Public Property Frames As New List(Of AnimFrame)
            Public Shared Function FromXml(XmlNode As Xml.XmlNode) As AnimSequence
                Dim a As New AnimSequence
                For Each item As Xml.XmlNode In XmlNode.SelectNodes("AnimFrame")
                    a.Frames.Add(AnimFrame.FromXml(item))
                Next
                Return a
            End Function
        End Class
        Public Class AnimFrame
            Public Property frameDuration As UInt16
            Public Property metaFrmGrpIndex As UInt16
            Public Property sprOffsetX As Int16
            Public Property sprOffsetY As Int16
            Public Property shadowOffsetX As Int16
            Public Property shadowOffsetY As Int16
            Public Function ToXml() As String
                Dim d As New Text.StringBuilder
                Using w As Xml.XmlWriter = Xml.XmlWriter.Create(d)
                    w.WriteStartElement("AnimFrame")

                    w.WriteStartElement("Duration")
                    w.WriteValue(frameDuration)
                    w.WriteEndElement()

                    w.WriteStartElement("MetaFrameGroupIndex")
                    w.WriteValue(metaFrmGrpIndex)
                    w.WriteEndElement()

                    w.WriteStartElement("Sprite")

                    w.WriteStartElement("XOffset")
                    w.WriteValue(sprOffsetX)
                    w.WriteEndElement()

                    w.WriteStartElement("YOffset")
                    w.WriteValue(sprOffsetY)
                    w.WriteEndElement()

                    w.WriteEndElement()

                    w.WriteStartElement("Shadow")

                    w.WriteStartElement("XOffset")
                    w.WriteValue(shadowOffsetX)
                    w.WriteEndElement()

                    w.WriteStartElement("YOffset")
                    w.WriteValue(shadowOffsetY)
                    w.WriteEndElement()

                    w.WriteEndElement()

                    w.WriteEndElement()
                End Using
                Return d.ToString
            End Function
            Public Shared Function FromXml(Node As Xml.XmlNode) As AnimFrame
                Dim a As New AnimFrame
                a.frameDuration = Node.SelectSingleNode("Duration").InnerText
                a.metaFrmGrpIndex = Node.SelectSingleNode("MetaFrameGroupIndex").InnerText
                a.sprOffsetX = Node.SelectSingleNode("Sprite/XOffset").InnerText
                a.sprOffsetY = Node.SelectSingleNode("Sprite/YOffset").InnerText
                a.shadowOffsetX = Node.SelectSingleNode("Shadow/XOffset").InnerText
                a.shadowOffsetY = Node.SelectSingleNode("Shadow/YOffset").InnerText
                Return a
            End Function
        End Class
    End Namespace
End Namespace