Namespace Roms
    Public Interface iPackedRom
        Function Unpack(DestinationDirectory As String) As Task
        Function RePack(NewFileName As String) As Task
        ReadOnly Property GameCode As String
    End Interface
End Namespace

