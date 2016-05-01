Namespace FileFormats.Explorers.Script.Commands
    ''' <summary>
    ''' A simple command without any parameters.
    ''' </summary>
    Public Class NoParamCommand
        Inherits RawCommand

        Public Overrides Function ToString() As String
            If IsEoS Then
                Select Case Me.CommandID
                    Case 0
                        Return My.Resources.SsbCommandNames.NOP
                    Case &H21
                        Return My.Resources.SsbCommandNames.BgmStop
                    Case &H26
                        Return My.Resources.SsbCommandNames.Bgm2Stop
                    Case &H6F
                        Return My.Resources.SsbCommandNames.DigAnim
                    Case &H70
                        Return My.Resources.SsbCommandNames.EndScript
                    Case &H82
                        Return My.Resources.SsbCommandNames.ReturnCommand
                    Case &H96
                        Return My.Resources.SsbCommandNames.PauseScript
                    Case &HA2
                        Return My.Resources.SsbCommandNames.DeletePicSpeak
                    Case &H128
                        Return My.Resources.SsbCommandNames.SoundStop
                    Case &H149
                        Return My.Resources.SsbCommandNames.SwitchParam
                    Case &H158
                        Return My.Resources.SsbCommandNames.ActRaiseHand
                    Case &H15E
                        Return My.Resources.SsbCommandNames.NoRep
                    Case &H171
                        Return My.Resources.SsbCommandNames.MapWaitAnim
                    Case &H175
                        Return My.Resources.SsbCommandNames.MapArrowClear
                    Case &H177
                        Return My.Resources.SsbCommandNames.MapLabelClear
                    Case Else
                        Return MyBase.ToString
                End Select
            Else
                Select Case Me.CommandID
                    Case 0
                        Return My.Resources.SsbCommandNames.NOP
                    Case &H1D
                        Return My.Resources.SsbCommandNames.BgmStop
                    Case &H22
                        Return My.Resources.SsbCommandNames.Bgm2Stop
                    Case &H62
                        Return My.Resources.SsbCommandNames.DigAnim
                    Case &H63
                        Return My.Resources.SsbCommandNames.EndScript
                    Case &H74
                        Return My.Resources.SsbCommandNames.ReturnCommand
                    Case &H84
                        Return My.Resources.SsbCommandNames.PauseScript
                    Case &H90
                        Return My.Resources.SsbCommandNames.DeletePicSpeak
                    Case &HFE
                        Return My.Resources.SsbCommandNames.SoundStop
                    Case &H11C
                        Return My.Resources.SsbCommandNames.SwitchParam
                    Case &H12A
                        Return My.Resources.SsbCommandNames.ActRaiseHand
                    Case &H130
                        Return My.Resources.SsbCommandNames.NoRep
                    Case &H142
                        Return My.Resources.SsbCommandNames.MapWaitAnim
                    Case &H146
                        Return My.Resources.SsbCommandNames.MapArrowClear
                    Case &H148
                        Return My.Resources.SsbCommandNames.MapLabelClear
                    Case Else
                        Return MyBase.ToString
                End Select
            End If
        End Function

    End Class

End Namespace
