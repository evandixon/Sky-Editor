Namespace skyjed.util


	Public MustInherit Class PopupMouseAdapter
		Inherits MouseAdapter

		Protected Friend MustOverride Sub popUp(ByVal e As MouseEvent)

		Public Overridable Sub mousePressed(ByVal e As MouseEvent)
			If e.PopupTrigger Then
				popUp(e)
			End If
		End Sub

		Public Overridable Sub mouseReleased(ByVal e As MouseEvent)
			If e.PopupTrigger Then
				popUp(e)
			End If
		End Sub

	End Class

End Namespace