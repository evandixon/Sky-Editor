Namespace MysteryDungeon.Explorers
    Public Class SkyActiveAttack
        Implements IExplorersActiveAttack
        Implements INotifyPropertyChanged

        Public Const Length = 29

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Public Sub New()

        End Sub
        Public Sub New(bits As Binary)
            With bits
                IsValid = .Bit(0)
                IsLinked = .Bit(1)
                IsSwitched = .Bit(2)
                IsSet = .Bit(3)
                IsSealed = .Bit(4)
                ID = .Int(0, 5, 10)
                PP = .Int(0, 15, 7)
                Ginseng = .Int(0, 22, 7)
            End With
        End Sub
        Public Function GetAttackBits() As Binary
            Dim out As New Binary(Length)
            With out
                .Bit(0) = IsValid
                .Bit(1) = IsLinked
                .Bit(2) = IsSwitched
                .Bit(3) = IsSet
                .Bit(4) = IsSealed
                .Int(0, 5, 10) = ID
                .Int(0, 15, 7) = PP
                .Int(0, 22, 7) = Ginseng
            End With
            Return out
        End Function

        Public Property IsValid As Boolean
            Get
                Return _isValid
            End Get
            Set(value As Boolean)
                If Not _isValid = value Then
                    _isValid = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(IsValid)))
                End If
            End Set
        End Property
        Dim _isValid As Boolean

        Public Property IsLinked As Boolean Implements IMDAttack.IsLinked
            Get
                Return _isLinked
            End Get
            Set(value As Boolean)
                If Not _isLinked = value Then
                    _isLinked = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(IsLinked)))
                End If
            End Set
        End Property
        Dim _isLinked As Boolean

        Public Property IsSwitched As Boolean Implements IMDAttack.IsSwitched
            Get
                Return _isSwitched
            End Get
            Set(value As Boolean)
                If Not _isSwitched = value Then
                    _isSwitched = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(IsSwitched)))
                End If
            End Set
        End Property
        Dim _isSwitched As Boolean

        Public Property IsSet As Boolean Implements IMDAttack.IsSet
            Get
                Return _isSet
            End Get
            Set(value As Boolean)
                If Not _isSet = value Then
                    _isSet = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(IsSet)))
                End If
            End Set
        End Property
        Dim _isSet As Boolean

        Public Property IsSealed As Boolean Implements IExplorersActiveAttack.IsSealed
            Get
                Return _isSealed
            End Get
            Set(value As Boolean)
                If Not _isSealed = value Then
                    _isSealed = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(IsSealed)))
                End If
            End Set
        End Property
        Dim _isSealed As Boolean

        Public Property ID As Integer Implements IMDAttack.ID
            Get
                Return _id
            End Get
            Set(value As Integer)
                If Not _id = value Then
                    _id = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ID)))
                End If
            End Set
        End Property
        Dim _id As Integer

        Public Property PP As Integer Implements IExplorersActiveAttack.PP
            Get
                Return _pp
            End Get
            Set(value As Integer)
                If Not _pp = value Then
                    _pp = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(PP)))
                End If
            End Set
        End Property
        Dim _pp As Integer

        Public Property Ginseng As Integer Implements IMDAttack.Ginseng
            Get
                Return _ginseng
            End Get
            Set(value As Integer)
                If Not _ginseng = value Then
                    _ginseng = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Ginseng)))
                End If
            End Set
        End Property
        Dim _ginseng As Integer

        Private ReadOnly Property MoveNames As Dictionary(Of Integer, String) Implements IMDAttack.MoveNames
            Get
                Return Lists.ExplorersMoves
            End Get
        End Property

    End Class

End Namespace
