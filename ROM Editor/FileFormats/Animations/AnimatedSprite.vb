Imports System.Drawing
Namespace FileFormats
    Namespace Animations
        Public Class AnimatedSprite
            Private Property Path As String
            Private Property FrameData As FrameList
            Public Property AnimData As AnimData
            Public Function GetFrameGroup(FrameIndex As Integer) As System.Drawing.Bitmap
                Return GetFrame(FrameData.FrameGroups(FrameIndex))
            End Function
            Public Function GetFrame(FrameGroup As FrameGroup) As Bitmap
                Dim minX As Integer = FrameGroup.Frames(0).XOffset
                Dim minY As Integer = FrameGroup.Frames(0).YOffset
                For Each item In FrameGroup.Frames
                    If item.XOffset < minX Then
                        minX = item.XOffset
                    End If
                    If item.YOffset < minY Then
                        minY = item.YOffset
                    End If
                Next
                Dim offsetX As Integer = minX
                Dim offsetY As Integer = minY
                Dim width As Integer = FrameGroup.Frames(0).ResolutionWidth
                Dim height As Integer = FrameGroup.Frames(0).ResolutionHeight
                For Each item In FrameGroup.Frames
                    Dim w = item.ResolutionWidth
                    Dim h = item.ResolutionWidth
                    Dim x = item.XOffset
                    Dim y = item.YOffset
                    If x - offsetX > 0 Then
                        width += w
                        offsetX = x
                    End If
                    If y - offsetY > 0 Then
                        height += h
                        offsetY = y
                    End If
                Next
                Dim out As New Bitmap(width, height)
                Dim gOut As Graphics = Graphics.FromImage(out)
                For Each item In FrameGroup.Frames
                    Dim filePath = String.Format(IO.Path.Combine(Path, "imgs", "{0}.png"), item.ImageIndex.ToString.PadLeft(4, "0"))
                    If IO.File.Exists(filePath) Then
                        Dim b = Drawing.Bitmap.FromFile(filePath)
                        If item.HFlip Then
                            b.RotateFlip(RotateFlipType.RotateNoneFlipX)
                        End If
                        If item.VFlip Then
                            b.RotateFlip(RotateFlipType.RotateNoneFlipY)
                        End If
                        gOut.DrawImage(b, New Point(item.XOffset - minX, item.YOffset - minY))
                    End If
                Next
                out.MakeTransparent(out.GetPixel(0, 0))
                Return out
            End Function
            Public Function GetAnimationFrames(AnimationIndex As Integer) As System.Drawing.Bitmap()
                Dim out As New List(Of Bitmap)
                For Each item In AnimData.AnimSequenceData(AnimationIndex).Frames
                    out.Add(GetFrame(FrameData.FrameGroups(item.metaFrmGrpIndex)))
                Next
                Return out.ToArray
            End Function
            Public Sub New(Path As String)
                FrameData = FrameList.FromXml(IO.File.ReadAllText(IO.Path.Combine(Path, "frames.xml")))
                AnimData = AnimData.FromXml(IO.File.ReadAllText(IO.Path.Combine(Path, "animations.xml")))
                Me.Path = Path
            End Sub
        End Class
    End Namespace
End Namespace