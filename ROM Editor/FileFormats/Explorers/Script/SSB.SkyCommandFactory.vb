Imports ROMEditor.FileFormats.Explorers.Script.Commands

Namespace FileFormats.Explorers.Script
    Partial Class SSB


        ''' <summary>
        ''' A list of the indexes of the constants that have not been referenced by a known Command type.
        ''' We need to leave these in their current index in order to keep things from breaking.
        ''' </summary>
        ''' <returns></returns>
        Private Property UnreferencedConstantIndexes As List(Of Integer)

        ''' <summary>
        ''' A list of the indexes of the strings that have not been referenced by a known Command type.
        ''' We need to leave these in their current index in order to keep things from breaking.
        ''' </summary>
        ''' <returns></returns>
        Private Property UnreferencedStringIndexes As List(Of Integer)

        ''' <summary>
        ''' A list of the indexes of constants that have been temporarily cleared and can be used for other strings.
        ''' </summary>
        ''' <returns></returns>
        Private Property AvailableConstantIndexes As List(Of Integer)

        ''' <summary>
        ''' A list of the indexes of strings that have been temporarily cleared and can be used for other strings.
        ''' </summary>
        ''' <returns></returns>
        Private Property AvailableStringIndexes As List(Of Integer)


        ''' <summary>
        ''' Creates a Command for Explorers of Sky using the given data.
        ''' </summary>
        ''' <param name="CommandID">ID of the Command</param>
        ''' <param name="Params">Parameters of the Command</param>
        ''' <returns></returns>
        Private Function CreateSkyCommand(CommandID As UInt16, Params As IEnumerable(Of UInt16)) As RawCommand
            'Select the command type
            Dim commandType As Type

            Select Case CommandID
                Case &H9E
                    commandType = GetType(MonologueCommand)
                Case &HAE
                    commandType = GetType(BasicTalkCommand)
                Case Else
                    commandType = GetType(RawCommand)
            End Select

            'Initialize the command
            Dim cmd As RawCommand = commandType.GetConstructor({}).Invoke({})
            'Set the default data
            cmd.CommandID = CommandID
            cmd.Params = Params

            'Use attributes on Properties to make parameter interaction more natural.
            Dim paramAttributeType = GetType(CommandParameterAttribute)
            For Each item In commandType.GetProperties
                For Each attribute In item.GetCustomAttributes(True)
                    If TypeOf attribute Is CommandParameterAttribute Then
                        Dim paramInfo = DirectCast(attribute, CommandParameterAttribute)

                        Select Case item.PropertyType
                            Case GetType(UInt16), GetType(UInt32), GetType(UInt64), GetType(UShort), GetType(UInteger), GetType(ULong)
                                item.SetValue(cmd, Params(paramInfo.Index))
                            Case GetType(Int16), GetType(Short)
                                item.SetValue(cmd, BitConverter.ToInt16(BitConverter.GetBytes(Params(paramInfo.Index)), 0))
                            Case GetType(Int32), GetType(Integer)
                                item.SetValue(cmd, BitConverter.ToInt32(BitConverter.GetBytes(Params(paramInfo.Index)), 0))
                            Case GetType(Int64), GetType(Long)
                                item.SetValue(cmd, BitConverter.ToInt64(BitConverter.GetBytes(Params(paramInfo.Index)), 0))
                            Case GetType(StringCommandParameter)
                                If isMultiLang Then
                                    Dim stringParam As New MultiLangStringCommandParameter
                                    stringParam.English = English(Params(paramInfo.Index))
                                    stringParam.French = French(Params(paramInfo.Index))
                                    stringParam.German = German(Params(paramInfo.Index))
                                    stringParam.Italian = Italian(Params(paramInfo.Index))
                                    stringParam.Spanish = Spanish(Params(paramInfo.Index))
                                    item.SetValue(cmd, stringParam)
                                Else
                                    Dim stringParam As New StringCommandParameter
                                    stringParam.English = English(Params(paramInfo.Index))
                                    item.SetValue(cmd, stringParam)
                                End If
                                UnreferencedStringIndexes.Remove(Params(paramInfo.Index))
                            Case GetType(ConstantCommandParameter)
                                Dim stringParam As New ConstantCommandParameter
                                stringParam.Constant = Constants(Params(paramInfo.Index))
                                UnreferencedConstantIndexes.Remove(Params(paramInfo.Index))
                                item.SetValue(cmd, stringParam)
                            Case Else
                                Throw New InvalidCastException(My.Resources.Language.ErrorScriptCommandAttributeInvalidType)
                        End Select

                    End If
                Next
            Next

            Return cmd
        End Function

        ''' <summary>
        ''' Gets a byte array representing the given command.
        ''' 
        ''' </summary>
        ''' <param name="Command"></param>
        ''' <returns></returns>
        Private Function GetSkyCommandBytes(Command As RawCommand) As Byte()
            Dim commandType = Command.GetType
            Dim params As List(Of UInt16) = Command.Params
            'Check the properties with attributes to make things go smoother
            Dim paramAttributeType = GetType(CommandParameterAttribute)
            For Each item In commandType.GetProperties
                For Each attribute In item.GetCustomAttributes(True)
                    If TypeOf attribute Is CommandParameterAttribute Then
                        Dim index = DirectCast(attribute, CommandParameterAttribute).Index

                        Select Case item.PropertyType
                            Case GetType(UInt16), GetType(UInt32), GetType(UInt64), GetType(UShort), GetType(UInteger), GetType(ULong)
                                params(index) = item.GetValue(Command)
                            Case GetType(Int16), GetType(Short)
                                'We don't want any overflow due to negatives, or numbers that are over 2 bytes, so we'll allow all overflow
                                params(index) = BitConverter.ToUInt16(BitConverter.GetBytes(item.GetValue(Command)), 0)
                            Case GetType(Int32), GetType(Integer)
                                'We don't want any overflow due to negatives, or numbers that are over 2 bytes, so we'll allow all overflow
                                params(index) = BitConverter.ToUInt16(BitConverter.GetBytes(item.GetValue(Command)), 0)
                            Case GetType(Int64), GetType(Long)
                                'We don't want any overflow due to negatives, or numbers that are over 2 bytes, so we'll allow all overflow
                                params(index) = BitConverter.ToUInt16(BitConverter.GetBytes(item.GetValue(Command)), 0)
                            Case GetType(StringCommandParameter)
                                Dim stringParam As StringCommandParameter = item.GetValue(Command)
                                If AvailableStringIndexes.Count > 0 Then
                                    'Overwrite available entries in the string table if we can
                                    Dim stringIndex = AvailableStringIndexes(0)
                                    AvailableStringIndexes.RemoveAt(0)

                                    English(stringIndex) = stringParam.English
                                    If isMultiLang Then
                                        If TypeOf stringParam Is MultiLangStringCommandParameter Then
                                            With DirectCast(stringParam, MultiLangStringCommandParameter)
                                                French(stringIndex) = .French
                                                German(stringIndex) = .German
                                                Italian(stringIndex) = .Italian
                                                Spanish(stringIndex) = .Spanish
                                            End With
                                        Else
                                            'That's odd.  We're in multi-language mode, but don't have a multi-language parameter.  Someone must have tampered with something.
                                            'In this case, we'll just use the English for the other languages
                                            French(stringIndex) = stringParam.English
                                            German(stringIndex) = stringParam.English
                                            Italian(stringIndex) = stringParam.English
                                            Spanish(stringIndex) = stringParam.English
                                        End If
                                    End If
                                Else
                                    'Otherwise, add new ones
                                    English.Add(stringParam.English)
                                    If isMultiLang Then
                                        If TypeOf stringParam Is MultiLangStringCommandParameter Then
                                            With DirectCast(stringParam, MultiLangStringCommandParameter)
                                                French.Add(.French)
                                                German.Add(.German)
                                                Italian.Add(.Italian)
                                                Spanish.Add(.Spanish)
                                            End With
                                        Else
                                            'That's odd.  We're in multi-language mode, but don't have a multi-language parameter.  Someone must have tampered with something.
                                            'In this case, we'll just use the English for the other languages
                                            French.Add(stringParam.English)
                                            German.Add(stringParam.English)
                                            Italian.Add(stringParam.English)
                                            Spanish.Add(stringParam.English)
                                        End If
                                    End If
                                End If
                            Case GetType(ConstantCommandParameter)
                                Dim constParam As ConstantCommandParameter = item.GetValue(Command)
                                If AvailableStringIndexes.Count > 0 Then
                                    'Overwrite available entries in the string table if we can
                                    Dim constIndex = AvailableConstantIndexes(0)
                                    AvailableConstantIndexes.RemoveAt(0)
                                    English(constIndex) = constParam.Constant
                                Else
                                    'Otherwise, add new ones
                                    Constants.Add(constParam.Constant)
                                End If
                            Case Else
                                Throw New InvalidCastException(My.Resources.Language.ErrorScriptCommandAttributeInvalidType)
                        End Select

                    End If
                Next
            Next

            'The hard part is over.  All the parameters are already in the list declared above.
            'Time to convert to binary!
            Dim buffer As New List(Of Byte)
            buffer.AddRange(BitConverter.GetBytes(Command.CommandID))
            For Each item In params
                buffer.AddRange(BitConverter.GetBytes(item))
            Next

            Return buffer.ToArray
        End Function

    End Class
End Namespace

