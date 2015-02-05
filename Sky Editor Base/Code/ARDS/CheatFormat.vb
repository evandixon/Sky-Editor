Namespace ARDS
    Public Class CheatFormat
        Public Property Name As String
        Public Property SupportedButtons As Button()
        Public Overrides Function ToString() As String
            Return PluginHelper.GetLanguageItem(Name)
        End Function
        Public Overrides Function Equals(obj As Object) As Boolean
            If TypeOf obj Is CheatFormat Then
                Return Name.Equals(obj.name)
            Else
                Return False
            End If
        End Function
        Public Shared Operator =(ByVal obj1 As Object, ByVal obj2 As CheatFormat) As Boolean
            If TypeOf obj1 Is CheatFormat Then
                Return obj1.Name = obj2.Name
            Else
                Return False
            End If
        End Operator
        Public Shared Operator <>(ByVal obj1 As Object, ByVal obj2 As CheatFormat) As Boolean
            If TypeOf obj1 Is CheatFormat Then
                Return obj1.Name <> obj2.Name
            Else
                Return False
            End If
        End Operator
        Public Sub New(Name As String, SupportedButtons As Button())
            Me.Name = Name
            Me.SupportedButtons = SupportedButtons
        End Sub
        Public Shared Function ARDS() As CheatFormat
            Return New CheatFormat("Action Replay DS", Button.NDSButtons)
        End Function
        Public Shared Function ARGBA() As CheatFormat
            Return New CheatFormat("Action Replay GBA", Button.GBAButtons)
        End Function
        Public Shared Function GamesharkGBA() As CheatFormat
            Return New CheatFormat("Gameshark GBA", Button.GBAButtons)
        End Function
        Public Shared Function CBA() As CheatFormat
            Return New CheatFormat("Codebreaker GBA", Button.GBAButtons)
        End Function
    End Class
End Namespace