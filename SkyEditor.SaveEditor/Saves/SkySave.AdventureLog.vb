Namespace Saves
    Partial Class SkySave
        ''' <summary>
        ''' Gets or sets the number of adventures the team has had.
        ''' If set to a negative, it will display as negative in-game.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Adventures As Integer
            Get
                Return Bits.Int(0, Offsets.Adventures, 32)
            End Get
            Set(value As Integer)
                Bits.Int(0, Offsets.Adventures, 32) = value
            End Set
        End Property
    End Class

End Namespace