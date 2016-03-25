Public Class PokePRNG

    Public Sub New()
        m_seed = 0UI
    End Sub

    Public Sub New(ByVal _SEED As UInt32)
        m_seed = _SEED
    End Sub

    Private m_seed As UInt32

    Public Property Seed() As UInt32
        Get
            Return m_seed
        End Get
        Set(ByVal value As UInt32)
            m_seed = value
        End Set
    End Property

    Public Function Previous() As UInt32
        Return &HEEB9EB65 * m_seed + &HA3561A1
    End Function

    Public Function PreviousNum() As UInt32
        m_seed = Previous()
        Return m_seed
    End Function

    Public Function [Next]() As UInt32
        Return (&H41C64E6D * m_seed) + &H6073
    End Function

    Public Function NextNum() As UInt32
        m_seed = [Next]()
        Return m_seed
    End Function

End Class