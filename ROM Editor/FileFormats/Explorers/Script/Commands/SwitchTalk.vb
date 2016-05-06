Namespace FileFormats.Explorers.Script.Commands
    Public Class SwitchTalk
        Inherits RawCommand

        <CommandParameter(0)> Public Property CharacterID As UInt16

        Public Property IsMonologue As Boolean
            Get
                If IsEoS Then
                    Return CommandID = &HAC
                Else
                    Return CommandID = &H99
                End If
            End Get
            Set(value As Boolean)
                If value Then
                    If IsEoS Then
                        CommandID = &HAC
                    Else
                        CommandID = &H99
                    End If
                Else
                    If IsEoS Then
                        CommandID = &HAD
                    Else
                        CommandID = &H100
                    End If
                End If
            End Set
        End Property

        Public Overrides Function ToString() As String
            Dim output As String
            If IsMonologue Then
                output = My.Resources.SsbCommandNames.SwitchMonologue
            Else
                output = My.Resources.SsbCommandNames.SwitchTalk
            End If
            Return String.Format(output, CharacterID)
        End Function

    End Class

End Namespace
