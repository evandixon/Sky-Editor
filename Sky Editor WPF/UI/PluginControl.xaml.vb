Imports System.Reflection
Imports SkyEditorBase.Interfaces

Namespace UI
    Public Class PluginControl
        Implements iObjectControl
        Public Sub RefreshDisplay()
            Dim uiElements As New List(Of PluginUiElement)
            With GetEditingObject(Of PluginManager)()
                Dim supportedAssemblyPaths = Utilities.ReflectionHelpers.GetSupportedPlugins(PluginHelper.GetPluginAssemblies, .CoreAssemblyName)
                For Each item In .Plugins
                    Dim assemblyPath As String = item.GetType.Assembly.Location
                    uiElements.Add(New PluginUiElement With {.IsEnabled = True, .Author = item.PluginAuthor, .Name = item.PluginName, .Credits = item.Credits, .Filename = assemblyPath})
                    supportedAssemblyPaths.Remove(assemblyPath)
                Next
                'Now to look at the plugins that aren't loaded
                Using manager As New Utilities.AssemblyReflectionManager
                    For Each item In supportedAssemblyPaths
                        manager.LoadAssembly(item, "PluginControl")
                        Dim elements = manager.Reflect(item, Function(CurrentAssembly As Assembly, Args() As Object) As List(Of PluginUiElement)
                                                                 Dim out As New List(Of PluginUiElement)
                                                                 For Each result In From t In CurrentAssembly.GetTypes Where Utilities.ReflectionHelpers.IsOfType(t, GetType(iSkyEditorPlugin)) AndAlso t.GetConstructor({}) IsNot Nothing
                                                                     Dim info As iSkyEditorPlugin = result.GetConstructor({}).Invoke({})
                                                                     out.Add(New PluginUiElement With {.IsEnabled = False, .Author = info.PluginAuthor, .Name = info.PluginName, .Credits = info.Credits, .Filename = Args(0)})
                                                                 Next
                                                                 Return out
                                                             End Function, item)
                        For Each element In elements
                            uiElements.Add(New PluginUiElement With {.IsEnabled = False, .Author = element.Author, .Name = element.Name, .Credits = element.Credits, .Filename = element.Filename})
                        Next
                    Next
                End Using

            End With

            For Each item In uiElements
                AddHandler item.Modified, AddressOf OnItemModified
            Next
            gridPlugins.ItemsSource = uiElements
            IsModified = False
        End Sub

        Private Sub OnItemModified(sender As Object, e As EventArgs)
            IsModified = True
        End Sub

        Private Sub btnApply_Click(sender As Object, e As RoutedEventArgs) Handles btnApply.Click
            If gridPlugins.ItemsSource IsNot Nothing Then 'Just in case
                Dim plugins = SettingsManager.Instance.Settings.Plugins
                For Each item As PluginUiElement In gridPlugins.ItemsSource
                    Dim filenamePart = item.Filename.Replace(PluginManager.GetInstance.PluginFolder, "").TrimStart("\")
                    If item.IsEnabled Then
                        If Not plugins.Contains(filenamePart) Then
                            plugins.Add(filenamePart)
                        End If
                    Else
                        plugins.Remove(filenamePart)
                    End If
                Next
                SettingsManager.Instance.Settings.Plugins = plugins
                SettingsManager.Instance.Save()
                If MessageBox.Show(PluginHelper.GetLanguageItem("You need to restart the program to save your changes.  Do you want to restart now?"), PluginHelper.GetLanguageItem("Sky Editor"), MessageBoxButton.YesNo) = MessageBoxResult.Yes Then
                    Redistribution.RedistributionHelpers.RequestRestartProgram()
                End If
            End If
            IsModified = False
        End Sub

        Private Sub SettingsEditor_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Header = PluginHelper.GetLanguageItem("Plugins")
        End Sub

        Public Function GetSupportedTypes() As IEnumerable(Of Type) Implements iObjectControl.GetSupportedTypes
            Return {GetType(PluginManager)}
        End Function

        Public Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer Implements iObjectControl.GetSortOrder
            Return 0
        End Function

#Region "IObjectControl Support"
        Public Function SupportsObject(Obj As Object) As Boolean Implements iObjectControl.SupportsObject
            Return True
        End Function

        Public Function IsBackupControl(Obj As Object) As Boolean Implements iObjectControl.IsBackupControl
            Return False
        End Function

        ''' <summary>
        ''' Called when Header is changed.
        ''' </summary>
        Public Event HeaderUpdated As iObjectControl.HeaderUpdatedEventHandler Implements iObjectControl.HeaderUpdated

        ''' <summary>
        ''' Called when IsModified is changed.
        ''' </summary>
        Public Event IsModifiedChanged As iObjectControl.IsModifiedChangedEventHandler Implements iObjectControl.IsModifiedChanged

        ''' <summary>
        ''' Returns the value of the Header.  Only used when the iObjectControl is behaving as a tab.
        ''' </summary>
        ''' <returns></returns>
        Public Property Header As String Implements iObjectControl.Header
            Get
                Return _header
            End Get
            Set(value As String)
                Dim oldValue = _header
                _header = value
                RaiseEvent HeaderUpdated(Me, New EventArguments.HeaderUpdatedEventArgs(oldValue, value))
            End Set
        End Property
        Dim _header As String

        ''' <summary>
        ''' Returns the current EditingObject, after casting it to type T.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <returns></returns>
        Protected Function GetEditingObject(Of T)() As T
            Return PluginHelper.Cast(Of T)(_editingObject)
        End Function

        ''' <summary>
        ''' Returns the current EditingObject.
        ''' It is recommended to use GetEditingObject(Of T), since it returns iContainter(Of T).Item if the EditingObject implements that interface.
        ''' </summary>
        ''' <returns></returns>
        Protected Function GetEditingObject() As Object
            Return _editingObject
        End Function

        ''' <summary>
        ''' The way to get the EditingObject from outside this class.  Refreshes the display on set, and updates the object on get.
        ''' Calling this from inside this class could result in a stack overflow, especially if called from UpdateObject, so use GetEditingObject or GetEditingObject(Of T) instead.
        ''' </summary>
        ''' <returns></returns>
        Public Property EditingObject As Object Implements iObjectControl.EditingObject
            Get
                'UpdateObject()
                Return _editingObject
            End Get
            Set(value As Object)
                _editingObject = value
                RefreshDisplay()
            End Set
        End Property
        Dim _editingObject As Object

        ''' <summary>
        ''' Whether or not the EditingObject has been modified without saving.
        ''' Set to true when the user changes anything in the GUI.
        ''' Set to false when the object is saved, or if the user undoes every change.
        ''' </summary>
        ''' <returns></returns>
        Public Property IsModified As Boolean Implements iObjectControl.IsModified
            Get
                Return _isModified
            End Get
            Set(value As Boolean)
                Dim oldValue As Boolean = _isModified
                _isModified = value
                If Not oldValue = _isModified Then
                    RaiseEvent IsModifiedChanged(Me, New EventArgs)
                End If
            End Set
        End Property
        Dim _isModified As Boolean
#End Region
    End Class

End Namespace
