Imports SkyEditor.skyjed.buffer
Imports SkyEditor.skyjed.util

Namespace skyjed.save

    Public Class SkyItem

        Public Const LENGTH As Integer = 33
        Public Shared ReadOnly MIME_TYPE As String = "application/x-sky-item; class=""[Z"""

        Public Const LEN_FLAGS As Integer = 7
        Private Const LEN_PARAM As Integer = 11
        Private Const LEN_ID As Integer = 11
        Private Const LEN_HELDBY As Integer = 3

        Public isvalid As Boolean
        Public flags() As Boolean
        Public param As Integer
        Public id As Integer
        Public heldby As Integer

        Public Sub New()
            flags = New Boolean(LEN_FLAGS - 1) {}
        End Sub

        Public Sub New(ByVal id As Integer, ByVal param As Integer)
            Me.New()
            Me.isvalid = (Not id = 0)
            Me.param = param
            Me.id = id
        End Sub

        Public Sub load(ByVal buf As BooleanBuffer)
            isvalid = buf.get()
            flags = buf.get(LEN_FLAGS)
            param = buf.getInt(LEN_PARAM)
            id = buf.getInt(LEN_ID)
            heldby = buf.getInt(LEN_HELDBY)
        End Sub

        Public Sub store(ByVal buf As BooleanBuffer)
            buf.put(isvalid)
            buf.put(flags)
            buf.putInt(param, LEN_PARAM)
            buf.putInt(id, LEN_ID)
            buf.putInt(heldby, LEN_HELDBY)
        End Sub

        Public Sub clear()
            Dim arr(LENGTH - 1) As Boolean
            Dim buf As BooleanBuffer = New BooleanBufferArray(arr)
            load(buf)
        End Sub

        'Public Overridable Sub copy()
        '	Dim arr(LENGTH - 1) As Boolean
        '	Dim buf As BooleanBuffer = New BooleanBufferArray(arr)
        '	store(buf)
        '          ClipboardHelper.setClipboardContents(arr, MIME_TYPE)
        '      End Sub

        'Public Overridable Sub paste()
        '	Dim arr() As Boolean = CType(ClipboardHelper.getClipboardContents(MIME_TYPE), Boolean())
        '	Dim buf As BooleanBuffer = New BooleanBufferArray(arr)
        '	load(buf)
        'End Sub

        Public Overrides Function ToString() As String
            If Not isvalid Then
                Return "----------"
            End If
            Dim desc As String = Lists.SkyItemNames(id) 'SkyItemRes.getItemName(id)
            If param > 0 Then
                If Me.Box Then
                    desc &= " (" & Lists.SkyItemNames(param) & ")"
                Else
                    desc &= " (" & param & ")"
                End If
            End If
            If heldby > 0 Then
                desc &= " [" & heldby & "]"
            End If
            Return desc
        End Function

        Public ReadOnly Property Box As Boolean
            Get
                Return id > 363 AndAlso id < 400
            End Get
        End Property

    End Class
    Public Class TDItem

        Public Const LENGTH As Integer = 31
        Public Shared ReadOnly MIME_TYPE As String = "application/x-sky-item; class=""[Z"""

        Public Const LEN_FLAGS As Integer = 6
        Private Const LEN_PARAM As Integer = 11
        Private Const LEN_ID As Integer = 10
        Private Const LEN_HELDBY As Integer = 3

        Public isvalid As Boolean
        Public flags() As Boolean
        Public param As Integer
        Public id As Integer
        Public heldby As Integer

        Public Sub New()
            flags = New Boolean(LEN_FLAGS - 1) {}
            isvalid = False
        End Sub

        Public Sub New(ByVal id As Integer, ByVal param As Integer)
            Me.New()
            Me.isvalid = True
            Me.param = param
            Me.id = id
        End Sub

        Public Overridable Sub load(ByVal buf As BooleanBuffer)
            isvalid = buf.get()
            flags = buf.get(LEN_FLAGS)
            param = buf.getInt(LEN_PARAM)
            id = buf.getInt(LEN_ID)
            heldby = buf.getInt(LEN_HELDBY)
        End Sub

        Public Overridable Sub store(ByVal buf As BooleanBuffer)
            buf.put(isvalid)
            buf.put(flags)
            buf.putInt(param, LEN_PARAM)
            buf.putInt(id, LEN_ID)
            buf.putInt(heldby, LEN_HELDBY)
        End Sub

        Public Sub clear()
            Dim arr(LENGTH - 1) As Boolean
            Dim buf As BooleanBuffer = New BooleanBufferArray(arr)
            load(buf)
        End Sub

        'Public Overridable Sub copy()
        '	Dim arr(LENGTH - 1) As Boolean
        '	Dim buf As BooleanBuffer = New BooleanBufferArray(arr)
        '	store(buf)
        '          ClipboardHelper.setClipboardContents(arr, MIME_TYPE)
        '      End Sub

        'Public Overridable Sub paste()
        '	Dim arr() As Boolean = CType(ClipboardHelper.getClipboardContents(MIME_TYPE), Boolean())
        '	Dim buf As BooleanBuffer = New BooleanBufferArray(arr)
        '	load(buf)
        'End Sub

        Public Overrides Function ToString() As String
            'If Not isvalid Then
            '    Return "----------"
            'End If
            Dim desc As String = Lists.SkyItemNames(id) 'SkyItemRes.getItemName(id)
            If param > 0 Then
                If Me.Box Then
                    'desc &= " (" & Lists.SkyItemNames(param) & ")"
                Else
                    desc &= " (" & param & ")"
                End If
            End If
            If heldby > 0 Then
                desc &= " [" & heldby & "]"
            End If
            Return desc
        End Function

        Private ReadOnly Property Box As Boolean
            Get
                Return id > 363 AndAlso id < 400
            End Get
        End Property

    End Class
    Public Class RBItem

        Public Const LENGTH As Integer = 23
        Public Shared ReadOnly MIME_TYPE As String = "application/x-sky-item; class=""[Z"""

        Public Const LEN_FLAGS As Integer = 7 '6
        Private Const LEN_PARAM As Integer = 7 '8
        Private Const LEN_ID As Integer = 8
        'Private Const LEN_HELDBY As Integer = 3

        Public isvalid As Boolean
        Public flags() As Boolean
        Public param As Integer
        Public id As Integer
        'Public heldby As Integer

        Public Sub New()
            flags = New Boolean(LEN_FLAGS - 1) {}
            isvalid = False
        End Sub

        Public Sub New(ByVal id As Integer, ByVal param As Integer)
            Me.New()
            Me.isvalid = True
            Me.param = param
            Me.id = id
        End Sub

        Public Sub load(ByVal buf As BooleanBuffer)
            isvalid = buf.get()
            flags = buf.get(LEN_FLAGS)
            param = buf.getInt(LEN_PARAM)
            id = buf.getInt(LEN_ID)
            'heldby = buf.getInt(LEN_HELDBY)
        End Sub

        Public Sub store(ByVal buf As BooleanBuffer)
            buf.put(isvalid)
            buf.put(flags)
            buf.putInt(param, LEN_PARAM)
            buf.putInt(id, LEN_ID)
            'buf.putInt(heldby, LEN_HELDBY)
        End Sub

        Public Sub clear()
            Dim arr(LENGTH - 1) As Boolean
            Dim buf As BooleanBuffer = New BooleanBufferArray(arr)
            load(buf)
        End Sub

        'Public Overridable Sub copy()
        '	Dim arr(LENGTH - 1) As Boolean
        '	Dim buf As BooleanBuffer = New BooleanBufferArray(arr)
        '	store(buf)
        '          ClipboardHelper.setClipboardContents(arr, MIME_TYPE)
        '      End Sub

        'Public Overridable Sub paste()
        '	Dim arr() As Boolean = CType(ClipboardHelper.getClipboardContents(MIME_TYPE), Boolean())
        '	Dim buf As BooleanBuffer = New BooleanBufferArray(arr)
        '	load(buf)
        'End Sub

        Public Overrides Function ToString() As String
            'If Not isvalid Then
            '    Return "----------"
            'End If
            Dim desc As String = Lists.RBItemNames(id) 'SkyItemRes.getItemName(id)
            If param > 0 Then
                desc &= " (" & param & ")"
            End If
            'If heldby > 0 Then
            '    desc &= " [" & heldby & "]"
            'End If
            Return desc
        End Function

        Private ReadOnly Property Box As Boolean
            Get
                Return False
            End Get
        End Property

    End Class
End Namespace