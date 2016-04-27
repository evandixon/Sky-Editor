Namespace FileFormats.Explorers.Script
    Public Class CommandParameterAttribute
        Inherits Attribute

        Public ReadOnly Property Index As Integer

        ''' <summary>
        ''' Marks a property as a Command Parameter.
        ''' </summary>
        ''' <param name="Index">Index of the parameter to store in this property.</param>
        Public Sub New(Index As Integer)
            Me.Index = Index
        End Sub
    End Class
End Namespace

