Imports System.Threading.Tasks

Namespace MenuActions
    Public Class FileOpenAuto
        Inherits MenuAction

        Private WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog

        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            Dim _manager = PluginManager.GetInstance
            OpenFileDialog1.Filter = _manager.IOFiltersString
            If OpenFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                If OpenFileDialog1.FileName.ToLower.EndsWith(".skysln") Then
                    PluginManager.GetInstance.CurrentSolution = Solution.OpenSolutionFile(OpenFileDialog1.FileName)
                Else
                    PluginHelper.RequestFileOpen(_manager.OpenObject(OpenFileDialog1.FileName), True)
                End If
            End If
            Return Task.CompletedTask
        End Function

        Public Sub New()
            MyBase.New({PluginHelper.GetLanguageItem("_File"), PluginHelper.GetLanguageItem("_Open"), PluginHelper.GetLanguageItem("Open (_Auto-Detect Game)")})
            AlwaysVisible = True
            OpenFileDialog1 = New Forms.OpenFileDialog
            SortOrder = 3
        End Sub
    End Class
End Namespace

