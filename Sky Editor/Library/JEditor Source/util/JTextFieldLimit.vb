Namespace skyjed.util


	Public Class JTextFieldLimit
		Inherits JTextField

		Private limit As Integer

		Public Sub New(ByVal limit As Integer)
			MyBase.New()
			Me.limit = limit
		End Sub

		Protected Friend Overridable Function createDefaultModel() As Document
			Return New LimitDocument(Me)
		End Function

		Private Class LimitDocument
			Inherits PlainDocument

			Private ReadOnly outerInstance As JTextFieldLimit

			Public Sub New(ByVal outerInstance As JTextFieldLimit)
				Me.outerInstance = outerInstance
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
'ORIGINAL LINE: public void insertString(int offset, String str, javax.swing.text.AttributeSet attr) throws javax.swing.text.BadLocationException
			Public Overridable Sub insertString(ByVal offset As Integer, ByVal str As String, ByVal attr As AttributeSet)
				If str Is Nothing Then
					Return
				End If
				If Me.Length + str.Length <= outerInstance.limit Then
					MyBase.insertString(offset, str, attr)
				End If
			End Sub
		End Class

	End Class

End Namespace