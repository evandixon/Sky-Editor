Imports System.IO
Imports System.Windows
Imports System.Windows.Input
Imports Xceed.Wpf.AvalonDock
Imports Xceed.Wpf.AvalonDock.Layout.Serialization

Namespace AvalonHelpers

    ''' <remarks>
    ''' Class implements an attached behavior to load/save a layout for AvalonDock manager.
    ''' This layout defines the position and shape of each document and tool window
    ''' displayed in the application.
    ''' 
    ''' Load/Save is triggered through command binding
    ''' On application start (AvalonDock.Load event results in LoadLayoutCommand) and
    '''    application shutdown (AvalonDock.Unload event results in SaveLayoutCommand).
    ''' 
    ''' This implementation of layout save/load is MVVM compliant, robust, and simple to use.
    ''' Just add the following code into your XAML:
    ''' 
    ''' xmlns:AVBehav="clr-namespace:Edi.View.Behavior"
    ''' ...
    ''' 
    ''' avalonDock:DockingManager AnchorablesSource="{Binding Tools}" 
    '''                           DocumentsSource="{Binding Files}"
    '''                           ActiveContent="{Binding ActiveDocument, Mode=TwoWay, Converter={StaticResource ActiveDocumentConverter}}"
    '''                           Grid.Row="3"
    '''                           SnapsToDevicePixels="True"
    '''                AVBehav:AvalonDockLayoutSerializer.LoadLayoutCommand="{Binding LoadLayoutCommand}"
    '''                AVBehav:AvalonDockLayoutSerializer.SaveLayoutCommand="{Binding SaveLayoutCommand}"
    '''                
    ''' The LoadLayoutCommand passes a reference of the AvalonDock Manager instance to load the XML layout.
    ''' The SaveLayoutCommand passes a string of the XML Layout which can be persisted by the viewmodel/model.
    ''' 
    ''' Both command bindings work with RoutedCommands or delegate commands (RelayCommand).
    ''' 
    ''' Credit to Dirk Bahle for implementation
    ''' http://www.codeproject.com/Articles/719143/AvalonDock-Tutorial-Part-Load-Save-Layout
    ''' </remarks>
    Public NotInheritable Class AvalonDockLayoutSerializer
        Private Sub New()
        End Sub
#Region "fields"
        ''' <summary>
        ''' Backing store for LoadLayoutCommand dependency property
        ''' </summary>
        Private Shared ReadOnly LoadLayoutCommandProperty As DependencyProperty = DependencyProperty.RegisterAttached("LoadLayoutCommand", GetType(ICommand), GetType(AvalonDockLayoutSerializer), New PropertyMetadata(Nothing, AddressOf OnLoadLayoutCommandChanged))

        ''' <summary>
        ''' Backing store for SaveLayoutCommand dependency property
        ''' </summary>
        Private Shared ReadOnly SaveLayoutCommandProperty As DependencyProperty = DependencyProperty.RegisterAttached("SaveLayoutCommand", GetType(ICommand), GetType(AvalonDockLayoutSerializer), New PropertyMetadata(Nothing, AddressOf OnSaveLayoutCommandChanged))
#End Region

#Region "methods"
#Region "Load Layout"
        ''' <summary>
        ''' Standard get method of <seealso cref="LoadLayoutCommandProperty"/> dependency property.
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        Public Shared Function GetLoadLayoutCommand(obj As DependencyObject) As ICommand
            Return DirectCast(obj.GetValue(LoadLayoutCommandProperty), ICommand)
        End Function

        ''' <summary>
        ''' Standard set method of <seealso cref="LoadLayoutCommandProperty"/> dependency property.
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <param name="value"></param>
        Public Shared Sub SetLoadLayoutCommand(obj As DependencyObject, value As ICommand)
            obj.SetValue(LoadLayoutCommandProperty, value)
        End Sub

        ''' <summary>
        ''' This method is executed if a <seealso cref="LoadLayoutCommandProperty"/> dependency property
        ''' is about to change its value (eg: The framewark assigns bindings).
        ''' </summary>
        ''' <param name="d"></param>
        ''' <param name="e"></param>
        Private Shared Sub OnLoadLayoutCommandChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
            Dim framworkElement As FrameworkElement = TryCast(d, FrameworkElement)
            ' Remove the handler if it exist to avoid memory leaks
            RemoveHandler framworkElement.Loaded, AddressOf OnFrameworkElement_Loaded

            Dim command = TryCast(e.NewValue, ICommand)
            If command IsNot Nothing Then
                ' the property is attached so we attach the Drop event handler
                AddHandler framworkElement.Loaded, AddressOf OnFrameworkElement_Loaded
            End If
        End Sub

        ''' <summary>
        ''' This method is executed when a AvalonDock <seealso cref="DockingManager"/> instance fires the
        ''' Load standard (FrameworkElement) event.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Shared Sub OnFrameworkElement_Loaded(sender As Object, e As RoutedEventArgs)
            Dim frameworkElement As FrameworkElement = TryCast(sender, FrameworkElement)

            ' Sanity check just in case this was somehow send by something else
            If frameworkElement Is Nothing Then
                Return
            End If

            Dim loadLayoutCommand As ICommand = AvalonDockLayoutSerializer.GetLoadLayoutCommand(frameworkElement)

            ' There may not be a command bound to this after all
            If loadLayoutCommand Is Nothing Then
                Return
            End If

            ' Check whether this attached behaviour is bound to a RoutedCommand
            If TypeOf loadLayoutCommand Is RoutedCommand Then
                ' Execute the routed command
                TryCast(loadLayoutCommand, RoutedCommand).Execute(frameworkElement, frameworkElement)
            Else
                ' Execute the Command as bound delegate
                loadLayoutCommand.Execute(frameworkElement)
            End If
        End Sub
#End Region

#Region "Save Layout"
        ''' <summary>
        ''' Standard get method of <seealso cref="SaveLayoutCommandProperty"/> dependency property.
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        Public Shared Function GetSaveLayoutCommand(obj As DependencyObject) As ICommand
            Return DirectCast(obj.GetValue(SaveLayoutCommandProperty), ICommand)
        End Function

        ''' <summary>
        ''' Standard get method of <seealso cref="SaveLayoutCommandProperty"/> dependency property.
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <param name="value"></param>
        Public Shared Sub SetSaveLayoutCommand(obj As DependencyObject, value As ICommand)
            obj.SetValue(SaveLayoutCommandProperty, value)
        End Sub

        ''' <summary>
        ''' This method is executed if a <seealso cref="SaveLayoutCommandProperty"/> dependency property
        ''' is about to change its value (eg: The framewark assigns bindings).
        ''' </summary>
        ''' <param name="d"></param>
        ''' <param name="e"></param>
        Private Shared Sub OnSaveLayoutCommandChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
            Dim framworkElement As FrameworkElement = TryCast(d, FrameworkElement)
            ' Remove the handler if it exist to avoid memory leaks
            RemoveHandler framworkElement.Unloaded, AddressOf OnFrameworkElement_Saveed

            Dim command = TryCast(e.NewValue, ICommand)
            If command IsNot Nothing Then
                ' the property is attached so we attach the Drop event handler
                AddHandler framworkElement.Unloaded, AddressOf OnFrameworkElement_Saveed
            End If
        End Sub

        ''' <summary>
        ''' This method is executed when a AvalonDock <seealso cref="DockingManager"/> instance fires the
        ''' Unload standard (FrameworkElement) event.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Shared Sub OnFrameworkElement_Saveed(sender As Object, e As RoutedEventArgs)
            Dim frameworkElement As DockingManager = TryCast(sender, DockingManager)

            ' Sanity check just in case this was somehow send by something else
            If frameworkElement Is Nothing Then
                Return
            End If

            Dim SaveLayoutCommand As ICommand = AvalonDockLayoutSerializer.GetSaveLayoutCommand(frameworkElement)

            ' There may not be a command bound to this after all
            If SaveLayoutCommand Is Nothing Then
                Return
            End If

            Dim xmlLayoutString As String = String.Empty

            Using fs As New StringWriter()
                Dim xmlLayout As New XmlLayoutSerializer(frameworkElement)

                xmlLayout.Serialize(fs)

                xmlLayoutString = fs.ToString()
            End Using

            ' Check whether this attached behaviour is bound to a RoutedCommand
            If TypeOf SaveLayoutCommand Is RoutedCommand Then
                ' Execute the routed command
                TryCast(SaveLayoutCommand, RoutedCommand).Execute(xmlLayoutString, frameworkElement)
            Else
                ' Execute the Command as bound delegate
                SaveLayoutCommand.Execute(xmlLayoutString)
            End If
        End Sub
#End Region
#End Region
    End Class
End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================
