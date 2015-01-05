Namespace skyjed.util



	Public Class JBooleanLabel
		Inherits JLabel

		Public Sub New()
			Me.New(False)
			'this.addMouseListener(new ZeroOneMouseAdapter());
		End Sub

		Public Sub New(ByVal b As Boolean)
			Me.Bool = b
			Me.addMouseListener(New ZeroOneMouseAdapter(Me))
		End Sub

		Public Overridable Property Bool As Boolean
			Get
				Return Me.Text.Equals("1")
			End Get
			Set(ByVal b As Boolean)
				Me.Text = If(b, "1", "0")
			End Set
		End Property


		' byte can be converted to int without cast so this can be used as getByte() too
		Public Overridable Property Int As SByte
			Get
				Return CSByte(If(Me.Bool, 1, 0))
			End Get
			Set(ByVal i As Integer)
				Me.Bool = i <> 0
			End Set
		End Property


		Private Class ZeroOneMouseAdapter
			Inherits MouseAdapter

			Private ReadOnly outerInstance As JBooleanLabel

			Public Sub New(ByVal outerInstance As JBooleanLabel)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub mouseReleased(ByVal e As MouseEvent)
				Dim lbl As JBooleanLabel = CType(e.Component, JBooleanLabel)
				If lbl.Enabled AndAlso e.Button = MouseEvent.BUTTON1 Then
					lbl.Bool = (Not lbl.Bool)
				End If
			End Sub
		End Class

	End Class

End Namespace