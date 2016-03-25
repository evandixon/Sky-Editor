Imports SkyEditorBase

Namespace ARDS

    Public Class Button
        Public Property Name As String
        Public Property ButtonValue As Integer
        Public Overrides Function ToString() As String
            Return PluginHelper.GetLanguageItem(Name)
        End Function
        Public Overrides Function Equals(obj As Object) As Boolean
            If TypeOf obj Is Button Then
                Return Name.Equals(obj.name)
            Else
                Return False
            End If
        End Function
        Public Shared Operator =(ByVal obj1 As Object, ByVal obj2 As Button) As Boolean
            If TypeOf obj1 Is Button Then
                Return obj1.Name = obj2.Name
            Else
                Return False
            End If
        End Operator
        Public Shared Operator <>(ByVal obj1 As Object, ByVal obj2 As Button) As Boolean
            If TypeOf obj1 Is Button Then
                Return obj1.Name <> obj2.Name
            Else
                Return False
            End If
        End Operator
        Private Sub New(Name As String, ButtonValue As Integer)
            Me.Name = Name
            Me.ButtonValue = ButtonValue
        End Sub
        Public Shared Function A() As Button
            Return New Button("A", 1)
        End Function
        Public Shared Function B() As Button
            Return New Button("B", 2)
        End Function
        Public Shared Function X() As Button
            Return New Button("X", &H400)
        End Function
        Public Shared Function Y() As Button
            Return New Button("Y", &H800)
        End Function
        Public Shared Function Up() As Button
            Return New Button("Up", &H40)
        End Function
        Public Shared Function Down() As Button
            Return New Button("Down", &H80)
        End Function
        Public Shared Function Left() As Button
            Return New Button("Left", &H20)
        End Function
        Public Shared Function Right() As Button
            Return New Button("Right", &H10)
        End Function
        Public Shared Function L() As Button
            Return New Button("L", &H200)
        End Function
        Public Shared Function R() As Button
            Return New Button("R", &H100)
        End Function
        Public Shared Function Start() As Button
            Return New Button("Start", &H4)
        End Function
        Public Shared Function [Select]() As Button
            Return New Button("Select", &H8)
        End Function
        Public Shared Function NDSButtons() As Button()
            Return {Button.A, Button.B, Button.X, Button.Y, Button.Up, Button.Down, Button.Left, Button.Right, Button.L, Button.R, Button.Start, Button.Select}
        End Function
        Public Shared Function GBAButtons() As Button()
            Return {Button.A, Button.B, Button.Up, Button.Down, Button.Left, Button.Right, Button.L, Button.R, Button.Start, Button.Select}
        End Function
        Public Shared Function NoButtons() As Button()
            Return {}
        End Function
    End Class
End Namespace