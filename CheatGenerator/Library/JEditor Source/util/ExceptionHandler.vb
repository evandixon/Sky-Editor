Imports System

Namespace skyjed.util



	Public Class ExceptionHandler

		Public Shared Sub handleException(ByVal e As Exception, ByVal component As Component)
            'Console.WriteLine(e.ToString())
            'Console.Write(e.StackTrace)
            'JOptionPane.showMessageDialog(component, e.Message, e.GetType().Name, JOptionPane.ERROR_MESSAGE)
            ExceptionManager.LogException(e, component.tostring)
		End Sub

	End Class

End Namespace