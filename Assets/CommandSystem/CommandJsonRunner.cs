using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using static System.Reflection.BindingFlags;

namespace CommandSystem
{
    public static class CommandJsonRunner
    {
        private static Dictionary<string, JObject> AliasMap = CommandRunner.AliasMap;
        
        
        public static bool TryRun(string commandString, out OutputData outputData)
        {
            outputData = ProcessCommandInputString(commandString);
            return true;
        }
        
        
        public static bool TryRun(string commandString, Dictionary<string, ArgData> args, out OutputData outputData)
        {
            throw new NotImplementedException();
        }
        
        public static OutputData ProcessCommandInputString(string commandString)
        {
            var split = commandString.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (!AliasMap.TryGetValue(split[0].ToLower(), out var commandJson))
                ThrowException("Command not found!", commandString);

            var argRegEx = new System.Text.RegularExpressions.Regex(@"[\""].+?[\""]|[^ ]+");
            var argMatches = argRegEx.Matches(commandString);
            var args = argMatches.Select(x => x.Value).Skip(1).ToArray();
            args = args.Select(x => x.StartsWith("\"") && x.EndsWith("\"") ? x[1..^1] : x).ToArray();

            var typedArgs = new ArgData[args.Length];
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] == "null")
                    typedArgs[i] = new ArgData(args[i], typeof(object), null, false);
                else if (bool.TryParse(args[i], out var boolValue))
                    typedArgs[i] = new ArgData(args[i], typeof(bool), boolValue, false);
                else if (int.TryParse(args[i], out var intValue))
                    typedArgs[i] = new ArgData(args[i], typeof(int), intValue, false);
                else if (float.TryParse(args[i], out var floatValue))
                    typedArgs[i] = new ArgData(args[i], typeof(float), floatValue, false);
                else if (double.TryParse(args[i], out var doubleValue))
                    typedArgs[i] = new ArgData(args[i], typeof(double), doubleValue, false);
                else
                    typedArgs[i] = new ArgData(args[i], typeof(string), args[i], false);
            }

            var version = commandJson["Version"]?.Value<int>();
            var name = commandJson["Name"]?.ToString();
            var description = commandJson["Description"]?.ToString();
            var aliases = commandJson["Aliases"] as JArray;
            var overloads = commandJson["Overloads"] as JArray;
            var overload = FindBestOverload(overloads, typedArgs);
            if (overload == null) ThrowException("No overload found!", commandString);
            var input = overload["Input"] as JArray;
            var calls = overload["Calls"] as JArray;
            var output = overload["Output"] as JObject;
            var commandLineOutput = overload["CommandLineOutput"]?.ToString();

            var localArgs = new Dictionary<string, ArgData>();
            for (var i = 0; i < (input?.Count ?? 0); i++)
            {
                var arg = i < typedArgs.Length ? (object)typedArgs[i].Value : null;
                var argName = input[i]["Name"]?.ToString();
                var argTypeString = input[i]["Type"]?.ToString();
                var argRequired = input[i]["Required"]?.Value<bool>() == true;
                var argType = StringToTypeUtility.Get(argTypeString);
                arg = ConvertType(new OutputData { Value = arg }, argType).Value;
                localArgs[argName] = new ArgData(argName, argType, arg, argRequired);
            }

            for (var i = 0; i < (calls?.Count ?? 0); i++)
            {
                var call = calls[i] as JObject;
                var callTypeString = call["Type"]?.ToString();
                var callName = call["Name"]?.ToString();
                var csharp = call["CSharp"]?.ToString();
                var command = call["Command"]?.ToString();
                if (callTypeString != "void")
                {
                    var callType = StringToTypeUtility.Get(callTypeString);
                    if (csharp != null)
                    {
                        var callOutput = AttemptToRunCSharp(csharp, localArgs, callType);
                        callOutput = ConvertType(callOutput, callType);
                        localArgs[callName] = new ArgData(callName, callType, callOutput?.Value, false);
                    }
                    else if (command != null)
                    {
                        var callOutput = AttemptToRunCommand(command, localArgs);
                        callOutput = ConvertType(callOutput, callType);
                        localArgs[callName] = new ArgData(callName, callType, callOutput?.Value, false);
                    }
                    else
                    {
                        ThrowException("No CSharp or Command found!", commandString, localArgs);
                    }
                }
                else
                {
                    if (csharp != null)
                        AttemptToRunCSharp(csharp, localArgs, null);
                    else if (command != null)
                        AttemptToRunCommand(command, localArgs);
                    else
                        ThrowException("No CSharp or Command found!", commandString, localArgs);
                }
            }

            var outputName = output?["Name"]?.ToString();
            var outputRegEx = new System.Text.RegularExpressions.Regex(@"\{.*?\}");
            var outputValue = outputName != null && outputName != "void" ? localArgs[outputName].Value : null;
            commandLineOutput ??= outputName;
            commandLineOutput = outputRegEx.Replace(commandLineOutput,
                m => localArgs.TryGetValue(m.Value, out var arg) ? arg.Value?.ToString() ?? "null" : "not found");
            return new OutputData { Value = outputValue, CommandLineOutput = commandLineOutput };
        }

        private static OutputData AttemptToRunCSharp(string csharpCode, Dictionary<string, ArgData> localArgs,
            Type callType)
        {
            // New Example
            // new UnityEngine.GameObject({GameObject Name}, {Component Types})
            // Looks up {GameObject Name} in localRunValues and cast as localRunType lookup
            // Looks up {Component Types} in localRunValues and cast as localRunType lookup
            if (csharpCode.StartsWith("new"))
            {
                var withoutNew = csharpCode[3..];
                var split = withoutNew.Split('(');
                var typeString = split[0];
                var argsString = split[1][..^1];
                var argStrings = argsString.Split(',').Select(x => x.Trim()).ToArray();
                var type = StringToTypeUtility.Get(typeString);
                var argObjects = new List<object>();
                for (var i = 0; i < argStrings.Length; i++)
                {
                    var argString = argStrings[i];
                    if (!localArgs.TryGetValue(argString, out var arg)) continue;
                    if (arg.Required || arg.Value != null)
                        argObjects.Add(arg.Value);
                }

                var outputValue = Activator.CreateInstance(type, argObjects.ToArray());
                return new OutputData { Value = outputValue, CommandLineOutput = csharpCode };
            }
            else
            {
                // Function Call Example
                // UnityEngine.GameObject.Find({GameObject Name})
                // Type: UnityEngine.GameObject
                // Function: Find
                // Args: {GameObject Name}
                var split = csharpCode.Split('(');

                if (split.Length == 1)
                {
                    if (callType.IsEnum)
                    {
                        var flagSplit = csharpCode.Split('|');
                        var enumValue = 0;
                        foreach (var flag in flagSplit)
                        {
                            var flagString = flag.Trim();
                            var flagValue = (int)Enum.Parse(callType, flagString);
                            enumValue |= flagValue;
                        }

                        var outputValue = enumValue;
                        return new OutputData { Value = outputValue, CommandLineOutput = csharpCode };
                    }

                    // Else return c# code as string
                    {
                        var outputValue = csharpCode;
                        return new OutputData { Value = outputValue, CommandLineOutput = csharpCode };
                    }
                }

                var methodSplit = split[0].Split('.');
                var methodName = methodSplit[^1];
                var fullTypeName = methodSplit[..^1];
                var fullTypeString = string.Join('.', fullTypeName);
                var type = StringToTypeUtility.Get(fullTypeString);

                // Else run method
                var argsString = split[1][..^1];
                var argStrings = string.IsNullOrEmpty(argsString)
                    ? Array.Empty<string>()
                    : argsString.Split(',').Select(x => x.Trim()).ToArray();

                var self = (ArgData)null;
                if (argStrings.Length > 0 && argStrings[0].StartsWith("this"))
                {
                    var selfString = argStrings[0][4..].Trim();
                    self = localArgs[selfString];
                    argStrings = argStrings.Skip(1).ToArray();
                }

                var args = new ArgData[argStrings.Length];
                for (var i = 0; i < argStrings.Length; i++)
                {
                    var argString = argStrings[i];
                    if (localArgs.TryGetValue(argString, out var arg))
                        args[i] = arg;
                }

                var argTypes = args.Select(x => x?.Type ?? typeof(object)).ToArray();
                var argObjects = args.Select(x => x?.Value ?? null).ToArray();
                var bindingFlags = Public | NonPublic | Instance | Static;
                // The following works pretty well, but does not work very well for generics
                // var method = type.GetMethod(methodName, bindingFlags, null, argTypes, null);

                // To make generics work, we first try above method, then try MakeGenericMethod
                var method = type.GetMethod(methodName, bindingFlags, null, argTypes, null);
                if (method == null)
                {
                    var isLinq = type.FullName.StartsWith("System.Linq");
                    if (isLinq)
                    {
                        var genericMethod = type.GetMethods(bindingFlags).FirstOrDefault(x =>
                            x.Name == methodName && x.GetParameters().Length == argTypes.Length);
                        var typeFromArray = (Type)null;
                        if (self != null)
                            typeFromArray = self.GetType().IsArray ? self.GetType().GetElementType() : self.GetType();
                        else if (argObjects.Length > 0)
                            typeFromArray = argTypes[0].IsArray ? argTypes[0].GetElementType() : argTypes[0];
                        method = genericMethod?.MakeGenericMethod(typeFromArray);
                    }
                    else
                    {
                        method = type.GetMethods(bindingFlags).FirstOrDefault(x =>
                            x.Name == methodName && x.GetParameters().Length == argTypes.Length);
                    }
                }

                if (method == null) ThrowException("Method not found!", csharpCode, localArgs);

                try
                {
                    var outputValue2 = method.Invoke(self?.Value, argObjects);
                    return new OutputData { Value = outputValue2, CommandLineOutput = csharpCode };
                }
                catch (Exception ex)
                {
                    ThrowException(ex, csharpCode, localArgs);
                    return null;
                }
            }
        }

        public static OutputData AttemptToRunCommand(string formattedCommandString,
            Dictionary<string, ArgData> localArgs)
        {
            // getscenepath {New GameObject}
            // alias = getscenepath
            // argStrings = {New GameObject}

            var alias = formattedCommandString.Split(' ')[0];

            // Use RegEx to get each argument between { }. Name between { } can have spaces. Include { } in RegEx.
            // RegEx should also include arguments with no { } and separate by spaces.
            var regex = new System.Text.RegularExpressions.Regex(@"[\{].+?[\}]|[^ ]+");
            var argStrings = regex.Matches(formattedCommandString).Select(x => x.Value).Skip(1).ToArray();

            var args = new ArgData[argStrings.Length];
            for (var i = 0; i < argStrings.Length; i++)
            {
                var argString = argStrings[i];
                if (localArgs.TryGetValue(argString, out var arg))
                    args[i] = arg;
                else
                {
                    if (argString == "null")
                        args[i] = new ArgData(argString, typeof(object), null, false);
                    else if (bool.TryParse(argString, out var boolValue))
                        args[i] = new ArgData(argString, typeof(bool), boolValue, false);
                    else if (int.TryParse(argString, out var intValue))
                        args[i] = new ArgData(argString, typeof(int), intValue, false);
                    else if (float.TryParse(argString, out var floatValue))
                        args[i] = new ArgData(argString, typeof(float), floatValue, false);
                    else if (double.TryParse(argString, out var doubleValue))
                        args[i] = new ArgData(argString, typeof(double), doubleValue, false);
                    else
                        args[i] = new ArgData(argString, typeof(string), argString, false);
                }
            }

            return Run(alias, localArgs, args);
        }

        private static OutputData Run(string alias, Dictionary<string, ArgData> localArgs, params ArgData[] args)
        {
            var argTypes = args.Select(x => x.Type).ToArray();
            var commandJson = AliasMap[alias.ToLower()];
            var version = commandJson["Version"]?.Value<int>();
            var name = commandJson["Name"]?.ToString();
            var description = commandJson["Description"]?.ToString();
            var aliases = commandJson["Aliases"] as JArray;
            var overloads = commandJson["Overloads"] as JArray;
            var overload = FindBestOverload(overloads, args);
            if (overload == null) ThrowException("No overload found!", alias, localArgs, args);
            var input = overload["Input"] as JArray;
            var calls = overload["Calls"] as JArray;
            var output = overload["Output"] as JObject;
            var commandLineOutput = overload["CommandLineOutput"]?.ToString();

            localArgs = new Dictionary<string, ArgData>(localArgs);
            for (var i = 0; i < (input?.Count ?? 0); i++)
            {
                var argValue = i < args.Length ? args[i].Value : null;
                var argName = input[i]["Name"]?.ToString();
                var argRequired = input[i]["Required"]?.Value<bool>() == true;
                var argTypeString = input[i]["Type"]?.ToString();
                var argType = StringToTypeUtility.Get(argTypeString);
                localArgs[argName] = new ArgData(argName, argType, argValue, argRequired);
            }

            for (var i = 0; i < (calls?.Count ?? 0); i++)
            {
                var call = calls[i] as JObject;
                var callTypeString = call["Type"]?.ToString();
                var callName = call["Name"]?.ToString();
                var csharp = call["CSharp"]?.ToString();
                var command = call["Command"]?.ToString();
                if (callTypeString != "void")
                {
                    var callType = StringToTypeUtility.Get(callTypeString);
                    if (csharp != null)
                    {
                        var callOutput = AttemptToRunCSharp(csharp, localArgs, callType);
                        callOutput = ConvertType(callOutput, callType);
                        localArgs[callName] = new ArgData(callName, callType, callOutput?.Value, false);
                    }
                    else if (command != null)
                    {
                        var callOutput = AttemptToRunCommand(command, localArgs);
                        callOutput = ConvertType(callOutput, callType);
                        localArgs[callName] = new ArgData(callName, callType, callOutput?.Value, false);
                    }
                    else
                    {
                        ThrowException("No CSharp or Command found!", alias, localArgs, args);
                    }
                }
                else
                {
                    if (csharp != null)
                        AttemptToRunCSharp(csharp, localArgs, null);
                    else if (command != null)
                        AttemptToRunCommand(command, localArgs);
                    else
                        ThrowException("No CSharp or Command found!", alias, localArgs, args);
                }
            }

            var outputName = output?["Name"]?.ToString();
            var outputValue = outputName != null && outputName != "void" ? localArgs[outputName].Value : null;
            commandLineOutput ??= outputName;
            var outputRegEx = new System.Text.RegularExpressions.Regex(@"\{.*?\}");
            commandLineOutput = outputRegEx.Replace(commandLineOutput, m => outputValue?.ToString() ?? "null");
            return new OutputData { Value = outputValue, CommandLineOutput = commandLineOutput };
        }

        private static JObject FindBestOverload(JArray overloads, ArgData[] args)
        {
            var argLength = args.Length;
            foreach (var overload in overloads)
            {
                var inputMinLength = overload["Input"]?.Where(x => x["Required"]?.Value<bool>() == true).Count() ?? 0;
                var inputMaxLength = overload["Input"]?.Count() ?? 0;
                if (argLength < inputMinLength || argLength > inputMaxLength) continue;
                var input = overload["Input"] as JArray;
                var inputTypes = input.Select(x => StringToTypeUtility.Get(x["Type"]?.ToString())).ToArray();
                var inputIsRequired = input.Select(x => x["Required"]?.Value<bool>() == true).ToArray();
                var isMatch = true;
                for (var i = 0; i < inputTypes.Length; i++)
                {
                    if (inputIsRequired[i] == false) continue;
                    if (i >= args.Length) ThrowException("Not enough arguments!");
                    var inputType = inputTypes[i];
                    if (IsConvertible(args[i], inputType)) continue;
                    isMatch = false;
                    break;
                }

                if (isMatch) return overload as JObject;
            }

            return null;
        }

        private static bool IsConvertible(ArgData argData, Type inputType)
        {
            if (argData == null) return false;
            if (argData.Type == inputType) return true;
            if (inputType.IsAssignableFrom(argData.Type)) return true;
            var argTypeAttempt2 = argData.Value?.GetType();
            if (argTypeAttempt2 != null && inputType.IsAssignableFrom(argTypeAttempt2)) return true;
            if (inputType.IsEnum && argData.Type == typeof(int)) return true;
            if (argData.Type == typeof(string) && inputType.IsEnum)
                return Enum.TryParse(inputType, argData.Value.ToString(), out _);
            if (argData.Type == typeof(string) && inputType == typeof(int))
                return int.TryParse(argData.Value.ToString(), out _);
            if (argData.Type == typeof(string) && inputType == typeof(float))
                return float.TryParse(argData.Value.ToString(), out _);
            if (argData.Type == typeof(string) && inputType == typeof(double))
                return double.TryParse(argData.Value.ToString(), out _);
            if (argData.Type == typeof(string) && inputType == typeof(bool))
                return bool.TryParse(argData.Value.ToString(), out _);
            if (inputType.IsArray && argData.Type.IsArray)
            {
                var inputElementType = inputType.GetElementType();
                var argElementType = argData.Type.GetElementType();
                foreach (var argElement in (Array)argData.Value)
                    if (!IsConvertible(new ArgData(argData.Name, argElementType, argElement, argData.Required),
                            inputElementType))
                        return false;
                return true;
            }

            return false;
        }

        private static OutputData ConvertType(OutputData outputData, Type toType)
        {
            var obj = outputData.Value;
            if (obj == null || toType == null) return outputData;
            var fromType = obj.GetType();
            if (fromType == toType) return outputData;
            if (toType.IsAssignableFrom(fromType)) return outputData;
            if (toType.IsEnum && fromType == typeof(int)) return outputData;
            if (toType.IsEnum && fromType == typeof(string))
                return outputData.Replace(Enum.Parse(toType, obj.ToString()));
            if (toType == typeof(int) && fromType == typeof(string))
                return outputData.Replace(int.Parse(obj.ToString()));
            if (toType == typeof(float) && fromType == typeof(string))
                return outputData.Replace(float.Parse(obj.ToString()));
            if (toType == typeof(double) && fromType == typeof(string))
                return outputData.Replace(double.Parse(obj.ToString()));
            if (toType == typeof(bool) && fromType == typeof(string))
                return outputData.Replace(bool.Parse(obj.ToString()));

            if (toType.IsArray && fromType.IsArray)
            {
                // Convert array types
                var fromElementType = fromType.GetElementType();
                var toElementType = toType.GetElementType();
                if (fromElementType == toElementType) return outputData;
                var fromArray = (Array)obj;
                var toArray = Array.CreateInstance(toElementType, fromArray.Length);
                for (var i = 0; i < fromArray.Length; i++)
                {
                    var fromElement = fromArray.GetValue(i);
                    var fromElementOutput = new OutputData { Value = fromElement };
                    var toElement = ConvertType(fromElementOutput, toElementType);
                    toArray.SetValue(toElement.Value, i);
                }

                return outputData.Replace(toArray);
            }

            return obj is IConvertible ? outputData.Replace(Convert.ChangeType(obj, toType)) : outputData;
        }

        private static void ThrowException(Exception ex, string commandString = null,
            Dictionary<string, ArgData> localArgs = null, ArgData[] args = null)
        {
            var message = ex.ToString();
            if (commandString != null) message += $"\nCommand: {commandString}";

            if (localArgs != null)
            {
                message += "\nLocal Args:";
                foreach (var localArg in localArgs)
                    message += $"\n{localArg.Value}";
            }

            if (args != null)
            {
                message += "\nArgs:";
                foreach (var arg in args)
                    message += $"\n{arg}";
            }

            throw new ArgumentException(message);
        }
        
        private static void ThrowException(string message, string commandString = null,
            Dictionary<string, ArgData> localArgs = null, ArgData[] args = null)
        {
            ThrowException(new ArgumentException(message), commandString, localArgs, args);
        }
    }
}