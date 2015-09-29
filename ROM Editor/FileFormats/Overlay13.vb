Namespace FileFormats
    ''' <summary>
    ''' The overlay that controls the personality test.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Overlay13
        Public Property Filename As String
        Public Property RawData As Byte()
#Region "RawIDs"
        Public Property Partner01 As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F4C)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F4C + 0) = bytes(0)
                RawData(&H1F4C + 1) = bytes(1)
            End Set
        End Property
        Public Property Partner02 As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F4E)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F4E + 0) = bytes(0)
                RawData(&H1F4E + 1) = bytes(1)
            End Set
        End Property
        Public Property Partner03 As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F50)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F50 + 0) = bytes(0)
                RawData(&H1F50 + 1) = bytes(1)
            End Set
        End Property
        Public Property Partner04 As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F52)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F52 + 0) = bytes(0)
                RawData(&H1F52 + 1) = bytes(1)
            End Set
        End Property
        Public Property Partner05 As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F54)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F54 + 0) = bytes(0)
                RawData(&H1F54 + 1) = bytes(1)
            End Set
        End Property
        Public Property Partner06 As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F56) '&H1F56
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F56 + 0) = bytes(0)
                RawData(&H1F56 + 1) = bytes(1)
            End Set
        End Property
        Public Property Partner07 As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F58)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F58 + 0) = bytes(0)
                RawData(&H1F58 + 1) = bytes(1)
            End Set
        End Property
        Public Property Partner08 As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F5A)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F5A + 0) = bytes(0)
                RawData(&H1F5A + 1) = bytes(1)
            End Set
        End Property
        Public Property Partner09 As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F5C)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F5C + 0) = bytes(0)
                RawData(&H1F5C + 1) = bytes(1)
            End Set
        End Property
        Public Property Partner10 As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F5E)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F5E + 0) = bytes(0)
                RawData(&H1F5E + 1) = bytes(1)
            End Set
        End Property
        Public Property Partner11 As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F60)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F60 + 0) = bytes(0)
                RawData(&H1F60 + 1) = bytes(1)
            End Set
        End Property
        Public Property Partner12 As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F62)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F62 + 0) = bytes(0)
                RawData(&H1F62 + 1) = bytes(1)
            End Set
        End Property
        Public Property Partner13 As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F64)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F64 + 0) = bytes(0)
                RawData(&H1F64 + 1) = bytes(1)
            End Set
        End Property
        Public Property Partner14 As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F66)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F66 + 0) = bytes(0)
                RawData(&H1F66 + 1) = bytes(1)
            End Set
        End Property
        Public Property Partner15 As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F68)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F68 + 0) = bytes(0)
                RawData(&H1F68 + 1) = bytes(1)
            End Set
        End Property
        Public Property Partner16 As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F6A)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F6A + 0) = bytes(0)
                RawData(&H1F6A + 1) = bytes(1)
            End Set
        End Property
        Public Property Partner17 As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F6C)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F6C + 0) = bytes(0)
                RawData(&H1F6C + 1) = bytes(1)
            End Set
        End Property
        Public Property Partner18 As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F6E)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F6E + 0) = bytes(0)
                RawData(&H1F6E + 1) = bytes(1)
            End Set
        End Property
        Public Property Partner19 As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F70)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F70 + 0) = bytes(0)
                RawData(&H1F70 + 1) = bytes(1)
            End Set
        End Property
        Public Property Partner20 As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F72)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F72 + 0) = bytes(0)
                RawData(&H1F72 + 1) = bytes(1)
            End Set
        End Property
        Public Property Partner21 As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F74)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F74 + 0) = bytes(0)
                RawData(&H1F74 + 1) = bytes(1)
            End Set
        End Property
        Public Property Partner22 As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F76)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F76 + 0) = bytes(0)
                RawData(&H1F76 + 1) = bytes(1)
            End Set
        End Property
        Public Property HardyMale As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F78)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F78 + 0) = bytes(0)
                RawData(&H1F78 + 1) = bytes(1)
            End Set
        End Property
        Public Property HardyFemale As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F7A)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F7A + 0) = bytes(0)
                RawData(&H1F7A + 1) = bytes(1)
            End Set
        End Property
        Public Property DocileMale As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F7C)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F7C + 0) = bytes(0)
                RawData(&H1F7C + 1) = bytes(1)
            End Set
        End Property
        Public Property DocileFemale As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F7E)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F7E + 0) = bytes(0)
                RawData(&H1F7E + 1) = bytes(1)
            End Set
        End Property
        Public Property BraveMale As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F80)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F80 + 0) = bytes(0)
                RawData(&H1F80 + 1) = bytes(1)
            End Set
        End Property
        Public Property BraveFemale As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F82)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F82 + 0) = bytes(0)
                RawData(&H1F82 + 1) = bytes(1)
            End Set
        End Property
        Public Property JollyMale As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F84)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F84 + 0) = bytes(0)
                RawData(&H1F84 + 1) = bytes(1)
            End Set
        End Property
        Public Property JollyFemale As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F86)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F86 + 0) = bytes(0)
                RawData(&H1F86 + 1) = bytes(1)
            End Set
        End Property
        Public Property ImpishMale As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F88)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F88 + 0) = bytes(0)
                RawData(&H1F88 + 1) = bytes(1)
            End Set
        End Property
        Public Property ImpishFemale As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F8A)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F8A + 0) = bytes(0)
                RawData(&H1F8A + 1) = bytes(1)
            End Set
        End Property
        Public Property NaiveMale As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F8C)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F8C + 0) = bytes(0)
                RawData(&H1F8C + 1) = bytes(1)
            End Set
        End Property
        Public Property NaiveFemale As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F8E)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F8E + 0) = bytes(0)
                RawData(&H1F8E + 1) = bytes(1)
            End Set
        End Property
        Public Property TimidMale As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F90)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F90 + 0) = bytes(0)
                RawData(&H1F90 + 1) = bytes(1)
            End Set
        End Property
        Public Property TimidFemale As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F92)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F92 + 0) = bytes(0)
                RawData(&H1F92 + 1) = bytes(1)
            End Set
        End Property
        Public Property HastyMale As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F94)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F94 + 0) = bytes(0)
                RawData(&H1F94 + 1) = bytes(1)
            End Set
        End Property
        Public Property HastyFemale As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F96)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F96 + 0) = bytes(0)
                RawData(&H1F96 + 1) = bytes(1)
            End Set
        End Property
        Public Property SassyMale As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F98)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F98 + 0) = bytes(0)
                RawData(&H1F98 + 1) = bytes(1)
            End Set
        End Property
        Public Property SassyFemale As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F9A)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F9A + 0) = bytes(0)
                RawData(&H1F9A + 1) = bytes(1)
            End Set
        End Property
        Public Property CalmMale As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F9C)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F9C + 0) = bytes(0)
                RawData(&H1F9C + 1) = bytes(1)
            End Set
        End Property
        Public Property CalmFemale As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1F9E)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1F9E + 0) = bytes(0)
                RawData(&H1F9E + 1) = bytes(1)
            End Set
        End Property
        Public Property RelaxedMale As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1FA0)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1FA0 + 0) = bytes(0)
                RawData(&H1FA0 + 1) = bytes(1)
            End Set
        End Property
        Public Property RelaxedFemale As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1FA2)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1FA2 + 0) = bytes(0)
                RawData(&H1FA2 + 1) = bytes(1)
            End Set
        End Property
        Public Property LonelyMale As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1FA4)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1FA4 + 0) = bytes(0)
                RawData(&H1FA4 + 1) = bytes(1)
            End Set
        End Property
        Public Property LonelyFemale As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1FA6)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1FA6 + 0) = bytes(0)
                RawData(&H1FA6 + 1) = bytes(1)
            End Set
        End Property
        Public Property QuirkyMale As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1FA8)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1FA8 + 0) = bytes(0)
                RawData(&H1FA8 + 1) = bytes(1)
            End Set
        End Property
        Public Property QuirkyFemale As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1FAA)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1FAA + 0) = bytes(0)
                RawData(&H1FAA + 1) = bytes(1)
            End Set
        End Property
        Public Property QuietMale As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1FAC)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1FAC + 0) = bytes(0)
                RawData(&H1FAC + 1) = bytes(1)
            End Set
        End Property
        Public Property QuietFemale As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1FAE)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1FAE + 0) = bytes(0)
                RawData(&H1FAE + 1) = bytes(1)
            End Set
        End Property
        Public Property RashMale As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1FB0)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1FB0 + 0) = bytes(0)
                RawData(&H1FB0 + 1) = bytes(1)
            End Set
        End Property
        Public Property RashFemale As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1FB2)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1FB2 + 0) = bytes(0)
                RawData(&H1FB2 + 1) = bytes(1)
            End Set
        End Property
        Public Property BoldMale As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1FB4)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1FB4 + 0) = bytes(0)
                RawData(&H1FB4 + 1) = bytes(1)
            End Set
        End Property
        Public Property BoldFemale As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1FB6)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1FB6 + 0) = bytes(0)
                RawData(&H1FB6 + 1) = bytes(1)
            End Set
        End Property
        Public Property Unk1 As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1FB8)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1FB8 + 0) = bytes(0)
                RawData(&H1FB8 + 1) = bytes(1)
            End Set
        End Property
        Public Property Unk2 As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, &H1FBA)
            End Get
            Set(value As UInt16)
                Dim bytes As Byte() = BitConverter.GetBytes(value)
                RawData(&H1FBA + 0) = bytes(0)
                RawData(&H1FBA + 1) = bytes(1)
            End Set
        End Property

#End Region
        ''' <summary>
        ''' Returns the rawID, subtracted by 600 if applicable
        ''' </summary>
        ''' <param name="RawID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetPokemonID(RawID As UInt16) As UInt16
            If RawID > 600 Then
                Return RawID - 600
            Else
                Return RawID
            End If
        End Function
        ''' <summary>
        ''' Returns the gender of the given raw ID
        ''' </summary>
        ''' <param name="RawID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetPokemonGender(RawID As UInt16) As UInt16
            Return (RawID > 600)
        End Function
        ''' <summary>
        ''' Sets the PokemonID to be the given gender
        ''' </summary>
        ''' <param name="RawID"></param>
        ''' <param name="IsFemale"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function SetPokemonIDGender(RawID As UInt16, IsFemale As Boolean) As UInt16
            If IsFemale Then
                Return RawID + 600
            Else
                Return RawID
            End If
        End Function

        Public Sub New(Filename As String)
            Me.Filename = Filename
            RawData = IO.File.ReadAllBytes(Filename)
        End Sub
        Public Sub Save()
            IO.File.WriteAllBytes(Filename, RawData)
        End Sub

    End Class
    ''' <summary>
    ''' Class that can store data from Overlay13.  Used for json serialization.
    ''' </summary>
    Public Class PersonalityTestContainer
        Public Property Partner01 As UInt16
        Public Property Partner02 As UInt16
        Public Property Partner03 As UInt16
        Public Property Partner04 As UInt16
        Public Property Partner05 As UInt16
        Public Property Partner06 As UInt16
        Public Property Partner07 As UInt16
        Public Property Partner08 As UInt16
        Public Property Partner09 As UInt16
        Public Property Partner10 As UInt16
        Public Property Partner11 As UInt16
        Public Property Partner12 As UInt16
        Public Property Partner13 As UInt16
        Public Property Partner14 As UInt16
        Public Property Partner15 As UInt16
        Public Property Partner16 As UInt16
        Public Property Partner17 As UInt16
        Public Property Partner18 As UInt16
        Public Property Partner19 As UInt16
        Public Property Partner20 As UInt16
        Public Property Partner21 As UInt16
        Public Property Partner22 As UInt16
        Public Property HardyMale As UInt16
        Public Property HardyFemale As UInt16
        Public Property DocileMale As UInt16
        Public Property DocileFemale As UInt16
        Public Property BraveMale As UInt16
        Public Property BraveFemale As UInt16
        Public Property JollyMale As UInt16
        Public Property JollyFemale As UInt16
        Public Property ImpishMale As UInt16
        Public Property ImpishFemale As UInt16
        Public Property NaiveMale As UInt16
        Public Property NaiveFemale As UInt16
        Public Property TimidMale As UInt16
        Public Property TimidFemale As UInt16
        Public Property HastyMale As UInt16
        Public Property HastyFemale As UInt16
        Public Property SassyMale As UInt16
        Public Property SassyFemale As UInt16
        Public Property CalmMale As UInt16
        Public Property CalmFemale As UInt16
        Public Property RelaxedMale As UInt16
        Public Property RelaxedFemale As UInt16
        Public Property LonelyMale As UInt16
        Public Property LonelyFemale As UInt16
        Public Property QuirkyMale As UInt16
        Public Property QuirkyFemale As UInt16
        Public Property QuietMale As UInt16
        Public Property QuietFemale As UInt16
        Public Property RashMale As UInt16
        Public Property RashFemale As UInt16
        Public Property BoldMale As UInt16
        Public Property BoldFemale As UInt16
        Public Sub UpdateOverlay(Overlay As Overlay13)
            Overlay.Partner01 = Me.Partner01
            Overlay.Partner02 = Me.Partner02
            Overlay.Partner03 = Me.Partner03
            Overlay.Partner04 = Me.Partner04
            Overlay.Partner05 = Me.Partner05
            Overlay.Partner06 = Me.Partner06
            Overlay.Partner07 = Me.Partner07
            Overlay.Partner08 = Me.Partner08
            Overlay.Partner09 = Me.Partner09
            Overlay.Partner10 = Me.Partner10
            Overlay.Partner11 = Me.Partner11
            Overlay.Partner12 = Me.Partner12
            Overlay.Partner13 = Me.Partner13
            Overlay.Partner14 = Me.Partner14
            Overlay.Partner15 = Me.Partner15
            Overlay.Partner16 = Me.Partner16
            Overlay.Partner17 = Me.Partner17
            Overlay.Partner18 = Me.Partner18
            Overlay.Partner19 = Me.Partner19
            Overlay.Partner20 = Me.Partner20
            Overlay.Partner21 = Me.Partner21
            Overlay.Partner22 = Me.Partner22
            Overlay.HardyMale = Me.HardyMale
            Overlay.HardyFemale = Me.HardyFemale
            Overlay.DocileMale = Me.DocileMale
            Overlay.DocileFemale = Me.DocileFemale
            Overlay.BraveMale = Me.BraveMale
            Overlay.BraveFemale = Me.BraveFemale
            Overlay.JollyMale = Me.JollyMale
            Overlay.JollyFemale = Me.JollyFemale
            Overlay.ImpishMale = Me.ImpishMale
            Overlay.ImpishFemale = Me.ImpishFemale
            Overlay.NaiveMale = Me.NaiveMale
            Overlay.NaiveFemale = Me.NaiveFemale
            Overlay.TimidMale = Me.TimidMale
            Overlay.TimidFemale = Me.TimidFemale
            Overlay.HastyMale = Me.HastyMale
            Overlay.HastyFemale = Me.HastyFemale
            Overlay.SassyMale = Me.SassyMale
            Overlay.SassyFemale = Me.SassyFemale
            Overlay.CalmMale = Me.CalmMale
            Overlay.CalmFemale = Me.CalmFemale
            Overlay.RelaxedMale = Me.RelaxedMale
            Overlay.RelaxedFemale = Me.RelaxedFemale
            Overlay.LonelyMale = Me.LonelyMale
            Overlay.LonelyFemale = Me.LonelyFemale
            Overlay.QuirkyMale = Me.QuirkyMale
            Overlay.QuirkyFemale = Me.QuirkyFemale
            Overlay.QuietMale = Me.QuietMale
            Overlay.QuietFemale = Me.QuietFemale
            Overlay.RashMale = Me.RashMale
            Overlay.RashFemale = Me.RashFemale
            Overlay.BoldMale = Me.BoldMale
            Overlay.BoldFemale = Me.BoldFemale
        End Sub
        Public Sub New()

        End Sub
        Public Sub New(Overlay As Overlay13)
            Me.Partner01 = Overlay.Partner01
            Me.Partner02 = Overlay.Partner02
            Me.Partner03 = Overlay.Partner03
            Me.Partner04 = Overlay.Partner04
            Me.Partner05 = Overlay.Partner05
            Me.Partner06 = Overlay.Partner06
            Me.Partner07 = Overlay.Partner07
            Me.Partner08 = Overlay.Partner08
            Me.Partner09 = Overlay.Partner09
            Me.Partner10 = Overlay.Partner10
            Me.Partner11 = Overlay.Partner11
            Me.Partner12 = Overlay.Partner12
            Me.Partner13 = Overlay.Partner13
            Me.Partner14 = Overlay.Partner14
            Me.Partner15 = Overlay.Partner15
            Me.Partner16 = Overlay.Partner16
            Me.Partner17 = Overlay.Partner17
            Me.Partner18 = Overlay.Partner18
            Me.Partner19 = Overlay.Partner19
            Me.Partner20 = Overlay.Partner20
            Me.Partner21 = Overlay.Partner21
            Me.Partner22 = Overlay.Partner22
            Me.HardyMale = Overlay.HardyMale
            Me.HardyFemale = Overlay.HardyFemale
            Me.DocileMale = Overlay.DocileMale
            Me.DocileFemale = Overlay.DocileFemale
            Me.BraveMale = Overlay.BraveMale
            Me.BraveFemale = Overlay.BraveFemale
            Me.JollyMale = Overlay.JollyMale
            Me.JollyFemale = Overlay.JollyFemale
            Me.ImpishMale = Overlay.ImpishMale
            Me.ImpishFemale = Overlay.ImpishFemale
            Me.NaiveMale = Overlay.NaiveMale
            Me.NaiveFemale = Overlay.NaiveFemale
            Me.TimidMale = Overlay.TimidMale
            Me.TimidFemale = Overlay.TimidFemale
            Me.HastyMale = Overlay.HastyMale
            Me.HastyFemale = Overlay.HastyFemale
            Me.SassyMale = Overlay.SassyMale
            Me.SassyFemale = Overlay.SassyFemale
            Me.CalmMale = Overlay.CalmMale
            Me.CalmFemale = Overlay.CalmFemale
            Me.RelaxedMale = Overlay.RelaxedMale
            Me.RelaxedFemale = Overlay.RelaxedFemale
            Me.LonelyMale = Overlay.LonelyMale
            Me.LonelyFemale = Overlay.LonelyFemale
            Me.QuirkyMale = Overlay.QuirkyMale
            Me.QuirkyFemale = Overlay.QuirkyFemale
            Me.QuietMale = Overlay.QuietMale
            Me.QuietFemale = Overlay.QuietFemale
            Me.RashMale = Overlay.RashMale
            Me.RashFemale = Overlay.RashFemale
            Me.BoldMale = Overlay.BoldMale
            Me.BoldFemale = Overlay.BoldFemale
        End Sub
    End Class

End Namespace