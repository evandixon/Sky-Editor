Imports System

Namespace skyjed.util


	Public Class JNumberSpinner
		Inherits JSpinner

		Public Sub New(ByVal max As Integer)
			Me.Model = New SpinnerNumberModel(0, 0, max, 1) ' val min max step
			Dim txt As JFormattedTextField = CType(Me.Editor, JSpinner.NumberEditor).TextField
			Dim formatter As NumberFormatter = (CType(txt.Formatter, NumberFormatter))
			formatter.AllowsInvalid = False
			formatter.CommitsOnValidEdit = True
		End Sub

		Public Overridable Property ValueInt As Integer
			Get
				Return CInt(Math.Truncate(Me.Value))
			End Get
		End Property

	End Class

End Namespace