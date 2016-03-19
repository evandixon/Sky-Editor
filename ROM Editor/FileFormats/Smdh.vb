Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.IO
Imports System.Drawing

Namespace FileFormats
    Public Class smdhHeader
        Public magic As UInt32
        Public version As UInt16, reserved As UInt16
    End Class
    Public Class smdhTitle
        Public Property ShortDescription As String
        Public Property LongDescription As String
        Public Property Publisher As String
    End Class
    Class smdhSettings
        Public gameRatings As Byte() = New Byte(15) {}
        Public regionLock As UInt32
        Public matchMakerId As Byte() = New Byte(11) {}
        Public flags As UInt32
        Public eulaVersion As UInt16, reserved As UInt16
        Public defaultFrame As UInt32, cecId As UInt32
    End Class
    Public Enum smdhTitleID
        Japanese = 0
        English = 1
        French = 2
        German = 3
        Italian = 4
        Spanish = 5
        SimplifiedChinese = 6
        Korean = 7
        Dutch = 8
        Portuguese = 9
        Russian = 10
        TraditionalChinese = 11
        Unused1 = 12
        Unused2 = 13
        Unused3 = 14
        Unused4 = 15
    End Enum
    Public Class SMDH
        Implements IDisposable
        Private file As FileStream

        Private header As New smdhHeader()
        Private applicationTitles As smdhTitle() = New smdhTitle(15) {}
        Private settings As New smdhSettings()
        Private reserved As Byte() = New Byte(7) {}
        Private bigIconData As UInt16() = New UInt16(2303) {}, smallIconData As UInt16() = New UInt16(575) {}

        Private m_smallIcon As New Bitmap(24, 24), m_bigIcon As New Bitmap(48, 48)

        Public ReadOnly Property Valid() As Boolean
            Get
                Return header.magic = &H48444D53
            End Get
        End Property

        Public ReadOnly Property Version() As UInt32
            Get
                Return Me.header.version
            End Get
        End Property

        Public Property ShortDescription(appTitleID As Integer) As String
            Get
                If Me.applicationTitles(appTitleID) Is Nothing Then
                    Me.applicationTitles(appTitleID) = New smdhTitle()
                End If
                Return Me.applicationTitles(appTitleID).ShortDescription
            End Get
            Set(value As String)
                If Me.applicationTitles(appTitleID) Is Nothing Then
                    Me.applicationTitles(appTitleID) = New smdhTitle()
                End If
                Me.applicationTitles(appTitleID).ShortDescription = value
            End Set
        End Property

        Public Property LongDescription(appTitleID As Integer) As String
            Get
                If Me.applicationTitles(appTitleID) Is Nothing Then
                    Me.applicationTitles(appTitleID) = New smdhTitle()
                End If
                Return Me.applicationTitles(appTitleID).LongDescription
            End Get
            Set(value As String)
                If Me.applicationTitles(appTitleID) Is Nothing Then
                    Me.applicationTitles(appTitleID) = New smdhTitle()
                End If
                Me.applicationTitles(appTitleID).LongDescription = value
            End Set
        End Property

        Public Property Publisher(appTitleID As Integer) As String
            Get
                If Me.applicationTitles(appTitleID) Is Nothing Then
                    Me.applicationTitles(appTitleID) = New smdhTitle()
                End If
                Return Me.applicationTitles(appTitleID).Publisher
            End Get
            Set(value As String)
                If Me.applicationTitles(appTitleID) Is Nothing Then
                    Me.applicationTitles(appTitleID) = New smdhTitle()
                End If
                Me.applicationTitles(appTitleID).Publisher = value
            End Set
        End Property

        Public Property SmallIcon() As Bitmap
            Get
                Return Me.m_smallIcon
            End Get
            Set
                Me.m_smallIcon = Value
                Me.convertSmallIcon(False)
            End Set
        End Property

        Public Property BigIcon() As Bitmap
            Get
                Return Me.m_bigIcon
            End Get
            Set
                Me.m_bigIcon = Value
                Me.convertBigIcon(False)
            End Set
        End Property

        Private Function getU8() As Byte
            Dim temp As Byte() = New Byte(0) {}
            file.Read(temp, 0, 1)
            Return temp(0)
        End Function

        Private Function getU16() As UInt16
            Dim temp As Byte() = New Byte(1) {}
            file.Read(temp, 0, 2)
            Return BitConverter.ToUInt16(temp, 0)
        End Function

        Private Function getU32() As UInt32
            Dim temp As Byte() = New Byte(3) {}
            file.Read(temp, 0, 4)
            Return BitConverter.ToUInt32(temp, 0)
        End Function

        Private Sub readHeader()
            Me.header.magic = Me.getU32()
            Me.header.version = Me.getU16()
            Me.header.reserved = Me.getU16()
        End Sub

        Private Sub readTitle(titleId As Integer)
            Me.applicationTitles(titleId) = New smdhTitle()
            Dim shortDesc As New System.Text.StringBuilder
            Dim longDesc As New System.Text.StringBuilder
            Dim pub As New System.Text.StringBuilder

            For i As Integer = 0 To 63
                shortDesc.Append(Convert.ToChar(Me.getU16))
            Next
            Me.applicationTitles(titleId).ShortDescription = shortDesc.ToString

            For i As Integer = 0 To 127
                longDesc.Append(Convert.ToChar(Me.getU16))
            Next
            Me.applicationTitles(titleId).LongDescription = longDesc.ToString

            For i As Integer = 0 To 63
                pub.Append(Convert.ToChar(Me.getU16))

            Next
            Me.applicationTitles(titleId).Publisher = pub.ToString
        End Sub

        Private Sub readTitles()
            For i As Integer = 0 To applicationTitles.Length - 1
                Me.readTitle(i)
            Next
        End Sub

        Private Sub readSettings()
            For i As Integer = 0 To 15
                Me.settings.gameRatings(i) = Me.getU8()
            Next
            Me.settings.regionLock = Me.getU32()
            For i As Integer = 0 To 11
                Me.settings.matchMakerId(i) = Me.getU8()
            Next
            Me.settings.flags = Me.getU32()
            Me.settings.eulaVersion = Me.getU16()
            Me.settings.reserved = Me.getU16()
            Me.settings.defaultFrame = Me.getU32()
            Me.settings.cecId = Me.getU32()
        End Sub

        Private Sub readReserved()
            For i As Integer = 0 To 7
                Me.reserved(i) = Me.getU8()
            Next
        End Sub

        Private Sub readSmallIcon()
            For i As Integer = 0 To Me.smallIconData.Length - 1
                Me.smallIconData(i) = Me.getU16()
            Next
        End Sub

        Private Sub readBigIcon()
            For i As Integer = 0 To Me.bigIconData.Length - 1
                Me.bigIconData(i) = Me.getU16()
            Next
        End Sub
        Private tileOrder As Byte() = {0, 1, 8, 9, 2, 3,
            10, 11, 16, 17, 24, 25,
            18, 19, 26, 27, 4, 5,
            12, 13, 6, 7, 14, 15,
            20, 21, 28, 29, 22, 23,
            30, 31, 32, 33, 40, 41,
            34, 35, 42, 43, 48, 49,
            56, 57, 50, 51, 58, 59,
            36, 37, 44, 45, 38, 39,
            46, 47, 52, 53, 60, 61,
            54, 55, 62, 63}

        Private Sub convertBigIcon(toBitmap As Boolean)
            If toBitmap Then
                Dim i As Integer = 0
                For tile_y As Integer = 0 To 47 Step 8
                    For tile_x As Integer = 0 To 47 Step 8
                        For k As Integer = 0 To 8 * 8 - 1
                            Dim x As Integer = tileOrder(k) And &H7
                            Dim y As Integer = tileOrder(k) >> 3
                            Dim color__1 As Integer = Me.bigIconData(i)
                            i += 1

                            Dim b As Integer = (color__1 And &H1F) << 3
                            Dim g As Integer = ((color__1 >> 5) And &H3F) << 2
                            Dim r As Integer = ((color__1 >> 11) And &H1F) << 3

                            'this.smallIcon.SetPixel(x, y, Color.FromArgb(255, 0, 0));
                            Me.m_bigIcon.SetPixel(x + tile_x, y + tile_y, Color.FromArgb(r, g, b))
                        Next
                    Next
                Next
            Else
                Dim i As Integer = 0
                For tile_y As Integer = 0 To 47 Step 8
                    For tile_x As Integer = 0 To 47 Step 8
                        For k As Integer = 0 To 8 * 8 - 1
                            Dim x As Integer = tileOrder(k) And &H7
                            Dim y As Integer = tileOrder(k) >> 3

                            Dim r As Integer = Me.m_bigIcon.GetPixel(x + tile_x, y + tile_y).R >> 3
                            Dim g As Integer = Me.m_bigIcon.GetPixel(x + tile_x, y + tile_y).G >> 2
                            Dim b As Integer = Me.m_bigIcon.GetPixel(x + tile_x, y + tile_y).B >> 3

                            Me.bigIconData(i) = Convert.ToUInt16((r << 11) Or (g << 5) Or b)
                            i += 1
                        Next
                    Next
                Next
            End If
        End Sub
        Private Sub convertSmallIcon(toBitmap As Boolean)
            If toBitmap Then
                Dim i As Integer = 0
                For tile_y As Integer = 0 To 23 Step 8
                    For tile_x As Integer = 0 To 23 Step 8
                        For k As Integer = 0 To 8 * 8 - 1
                            Dim x As Integer = tileOrder(k) And &H7
                            Dim y As Integer = tileOrder(k) >> 3
                            Dim color__1 As Integer = Me.smallIconData(i)
                            i += 1

                            Dim b As Integer = (color__1 And &H1F) << 3
                            Dim g As Integer = ((color__1 >> 5) And &H3F) << 2
                            Dim r As Integer = ((color__1 >> 11) And &H1F) << 3

                            'this.smallIcon.SetPixel(x, y, Color.FromArgb(255, 0, 0));
                            Me.m_smallIcon.SetPixel(x + tile_x, y + tile_y, Color.FromArgb(r, g, b))
                        Next
                    Next
                Next
            Else
                Dim i As Integer = 0
                For tile_y As Integer = 0 To 23 Step 8
                    For tile_x As Integer = 0 To 23 Step 8
                        For k As Integer = 0 To 8 * 8 - 1
                            Dim x As Integer = tileOrder(k) And &H7
                            Dim y As Integer = tileOrder(k) >> 3

                            Dim r As Integer = Me.m_smallIcon.GetPixel(x + tile_x, y + tile_y).R >> 3
                            Dim g As Integer = Me.m_smallIcon.GetPixel(x + tile_x, y + tile_y).G >> 2
                            Dim b As Integer = Me.m_smallIcon.GetPixel(x + tile_x, y + tile_y).B >> 3

                            Me.smallIconData(i) = Convert.ToUInt16((r << 11) Or (g << 5) Or b)
                            i += 1
                        Next
                    Next
                Next
            End If
        End Sub

        Public Sub Load(fileName As [String])
            file = IO.File.OpenRead(fileName)
            Dim temp As Byte() = New Byte(3) {}

            Me.readHeader()
            If Me.Valid Then
                Me.readTitles()
                Me.readSettings()
                Me.readReserved()
                Me.readSmallIcon()
                Me.readBigIcon()

                Me.convertSmallIcon(True)
                Me.convertBigIcon(True)
            End If
            file.Close()
        End Sub

        Public Sub Save(fileName As [String])
            file = IO.File.OpenWrite(fileName)
            Dim writer As New BinaryWriter(file)
            Dim u = System.Text.UnicodeEncoding.Unicode

            Me.header.magic = &H48444D53
            writer.Write(Me.header.magic)
            writer.Write(Me.header.version)
            writer.Write(Me.header.reserved)

            For i As Integer = 0 To applicationTitles.Length - 1
                If Me.applicationTitles(i) Is Nothing Then
                    Me.applicationTitles(i) = New smdhTitle()
                End If

                Dim shortDesc = u.GetBytes(Me.applicationTitles(i).ShortDescription)
                writer.Write(shortDesc, 0, shortDesc.Length)

                Dim longDesc = u.GetBytes(Me.applicationTitles(i).LongDescription)
                writer.Write(longDesc, 0, longDesc.Length)

                Dim pub = u.GetBytes(Me.applicationTitles(i).Publisher)
                writer.Write(pub, 0, pub.Length)
            Next

            For i As Integer = 0 To Me.settings.gameRatings.Length - 1
                writer.Write(Me.settings.gameRatings(i))
            Next
            writer.Write(Me.settings.regionLock)
            For i As Integer = 0 To Me.settings.matchMakerId.Length - 1
                writer.Write(Me.settings.matchMakerId(i))
            Next
            writer.Write(Me.settings.flags)
            writer.Write(Me.settings.eulaVersion)
            writer.Write(Me.settings.reserved)
            writer.Write(Me.settings.defaultFrame)
            writer.Write(Me.settings.cecId)

            For i As Integer = 0 To Me.reserved.Length - 1
                writer.Write(Me.reserved(i))
            Next
            For i As Integer = 0 To Me.smallIconData.Length - 1
                writer.Write(Me.smallIconData(i))
            Next
            For i As Integer = 0 To Me.bigIconData.Length - 1
                writer.Write(Me.bigIconData(i))
            Next

            writer.Close()
            file.Close()
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    If m_bigIcon IsNot Nothing Then m_bigIcon.Dispose()
                    If m_smallIcon IsNot Nothing Then m_smallIcon.Dispose()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            ' TODO: uncomment the following line if Finalize() is overridden above.
            ' GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class
    '=======================================================
    'Service provided by Telerik (www.telerik.com)
    'Conversion powered by NRefactory.
    'Twitter: @telerik
    'Facebook: facebook.com/telerik
    '=======================================================
End Namespace