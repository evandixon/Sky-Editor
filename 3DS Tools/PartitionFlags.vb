Public Structure PartitionFlags
    Public Property RawData As Byte()

    ''' <summary>
    ''' Number of seconds to wait until writing the save to the cartridge.
    ''' Only applicable starting with firmware version 6.0.0-11
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BackupWaitTime As Byte
        Get
            Return RawData(0)
        End Get
        Set(value As Byte)
            RawData(0) = value
        End Set
    End Property
    Public Property KeyYMethod As KeyYMethod
        Get
            Return RawData(1)
        End Get
        Set(value As KeyYMethod)
            RawData(1) = value
        End Set
    End Property
    Public Property Byte2 As Byte
        Get
            Return RawData(2)
        End Get
        Set(value As Byte)
            RawData(2) = value
        End Set
    End Property
    ''' <summary>
    '''
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>SDK 3.X+</remarks>
    Public Property MediaCardDevice As CardDevice
        Get
            Return RawData(3)
        End Get
        Set(value As CardDevice)
            RawData(3) = value
        End Set
    End Property
    Public Property MediaPlatform As Platform
        Get
            Return RawData(4)
        End Get
        Set(value As Platform)
            RawData(4) = value
        End Set
    End Property
    Public Property MediaType As MediaType
        Get
            Return RawData(5)
        End Get
        Set(value As MediaType)
            RawData(5) = value
        End Set
    End Property
    Public Property Byte6 As Byte
        Get
            Return RawData(6)
        End Get
        Set(value As Byte)
            RawData(6) = value
        End Set
    End Property
    Public Property Byte7 As Byte
        Get
            Return RawData(7)
        End Get
        Set(value As Byte)
            RawData(7) = value
        End Set
    End Property

    Public Sub SetRepeatingCTR()
        RawData = {0, 0, 0, 0, 0, 1, 0, 0}
    End Sub

    Public Sub New(RawData As IEnumerable(Of Byte), Optional Index As Integer = 0)
        ReDim Me.RawData(7)
        For count As Byte = 0 To 7
            Me.RawData(count) = RawData(count + Index)
        Next
    End Sub
End Structure

Public Enum KeyYMethod
    Sys2 = 0
    Sys6 = 1
    Sys9 = &HA
End Enum
Public Enum MediaType
    InnerDevice = 0
    Card1 = 1
    Card2 = 2
    ExtendedDevice = 3
End Enum
Public Enum CardDevice
    NotSet = 0
    NORFlash = 1
    None = 2
    BT = 3
End Enum
Public Enum Platform
    CTR = 1
End Enum
'00 00 00 00 00 01 00 00 - Repeating CTR Fail
'00 00 00 01 01 01 00 00 - 2.2.0-4 KeyY Method
'00 01 00 02 01 01 00 00 - 6.0.0-11 KeyY Method