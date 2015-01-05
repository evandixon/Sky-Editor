Public Class SaveHackerToolkitInterface
    Implements Save_Hacker_Toolkit_2_Interfaces.ChecksumFixer
#Region "Checksum Fixer Interface Methods"
    Public Function FixChecksum(RawData As Byte()) As Byte() Implements Save_Hacker_Toolkit_2_Interfaces.ChecksumFixer.FixChecksum
        Dim save As New GenericSave(RawData)
        Return save.GetBytes
    End Function
    Public ReadOnly Property GameName As String Implements Save_Hacker_Toolkit_2_Interfaces.ChecksumFixer.GameName
        Get
            Return "Pokemon Mystery Dungeon: Red/Blue Rescue Team or Explorers of Time/Darkness/Sky Save"
        End Get
    End Property
    Public Overrides Function ToString() As String Implements Save_Hacker_Toolkit_2_Interfaces.ChecksumFixer.ToString
        Return GameName
    End Function
    Public Sub New()

    End Sub
#End Region
End Class
