Namespace MysteryDungeon.Rescue
    Public Class RBAttack
        'Implements IExplorersAttack
        Implements INotifyPropertyChanged
        Implements IMDAttack

        Public Const Length = 20

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Public Sub New()

        End Sub
        Public Sub New(bits As Binary)
            With bits
                IsValid = .Bit(0)
                IsLinked = .Bit(1)
                IsSwitched = .Bit(2)
                IsSet = .Bit(3)
                ID = .Int(0, 4, 9)
                Ginseng = .Int(0, 13, 7)
            End With
        End Sub
        Public Function GetAttackBits() As Binary
            Dim out As New Binary(Length)
            With out
                .Bit(0) = IsValid
                .Bit(1) = IsLinked
                .Bit(2) = IsSwitched
                .Bit(3) = IsSet
                .Int(0, 4, 9) = ID
                .Int(0, 13, 7) = Ginseng
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

        Public ReadOnly Property MoveNames As Dictionary(Of Integer, String) Implements IMDAttack.MoveNames
            Get
                Return Lists.RBMoves
            End Get
        End Property
    End Class
End Namespace

