Namespace FileFormats.Explorers.Script.Commands
    Public Class FadeTopBg
        Inherits RawCommand

        ''' <summary>
        ''' Gets or sets whether or not this is a fade in.
        ''' If True, it's a fade in.  If False, it's a fade out.
        ''' </summary>
        ''' <returns></returns>
        Public Property IsFadeIn As Boolean
            Get
                If IsEoS Then
                    Return CommandID = &HE8
                Else
                    Return CommandID = &HCB
                End If
            End Get
            Set(value As Boolean)
                If IsEoS Then
                    CommandID = &HE8
                Else
                    CommandID = &HCB
                End If
            End Set
        End Property

        <CommandParameter(0)> Public Property Block As UInt16

        <CommandParameter(1)> Public Property Frames As UInt16

        Public Overrides Function ToString() As String
            Dim output As String
            If IsFadeIn Then
                output = My.Resources.SsbCommandNames.FadeInTopBg
            Else
                output = My.Resources.SsbCommandNames.FadeOutTopBg
            End If
            Return String.Format(output, Block, Frames)
        End Function

    End Class

End Namespace
