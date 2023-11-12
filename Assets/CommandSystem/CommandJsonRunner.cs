using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using static System.Reflection.BindingFlags;

namespace CommandSystem
{
    public static class CommandJsonRunner
    {
        private const float UpdateRate = 0;
        private static float _nextUpdate;
        private static Dictionary<string, JObject> aliasMap = new();

        public static Dictionary<string, JObject> AliasMap => aliasMap;

        public static string ProcessCommandInputString(string commandString)
        {
            AttemptUpdateAliasMap();
            var split = commandString.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (!aliasMap.TryGetValue(split[0].ToLower(), out var commandJson))
                throw new ArgumentException($"Command {split[0]} not found!");

            var argRegEx = new System.Text.RegularExpressions.Regex(@"[\""].+?[\""]|[^ ]+");
            var argMatches = argRegEx.Matches(commandString);
            var args = argMatches.Select(x => x.Value).Skip(1).ToArray();
            args = args.Select(x => x.StartsWith("\"") && x.EndsWith("\"") ? x[1..^1] : x).ToArray();

            var stringArgs = new ArgData[args.Length];
            for (var i = 0; i < args.Length; i++)
                stringArgs[i] = new ArgData(args[i], typeof(string), args[i], false);

            var version = commandJson["Version"]?.Value<int>();
            var name = commandJson["Name"]?.ToString();
            var description = commandJson["Description"]?.ToString();
            var aliases = commandJson["Aliases"] as JArray;
            var overloads = commandJson["Overloads"] as JArray;
            var overload = FindBestOverload(overloads, stringArgs);
            if (overload == null) throw new ArgumentException("No overload found!");
            var input = overload["Input"] as JArray;
            var calls = overload["Calls"] as JArray;
            var output = overload["Output"]?.ToString();
            var commandLineOutput = overload["CommandLineOutput"]?.ToString();

            var localArgs = new Dictionary<string, ArgData>();
            for (var i = 0; i < (input?.Count ?? 0); i++)
            {
                var arg = i < args.Length ? (object)args[i] : null;
                var argName = input[i]["Name"]?.ToString();
                var argTypeString = input[i]["Type"]?.ToString();
                var argRequired = input[i]["Required"]?.Value<bool>() == true;
                var argType = StringToTypeUtility.Get(argTypeString);
                arg = ConvertType(arg, argType);
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
                        AttemptToRunCSharp(csharp, localArgs, callType, out var callOutput);
                        callOutput = ConvertType(callOutput, callType);
                        localArgs[callName] = new ArgData(callName, callType, callOutput, false);
                    }
                    else if (command != null)
                    {
                        AttemptToRunCommand(command, localArgs, out var callOutput);
                        callOutput = ConvertType(callOutput, callType);
                        localArgs[callName] = new ArgData(callName, callType, callOutput, false);
                    }
                    else
                    {
                        throw new ArgumentException("No CSharp or Command found!");
                    }
                }
                else
                {
                    if (csharp != null)
                        AttemptToRunCSharp(csharp, localArgs, null, out _);
                    else if (command != null)
                        AttemptToRunCommand(command, localArgs, out _);
                    else
                        throw new ArgumentException("No CSharp or Command found!");
                }
            }

            var outputRegEx = new System.Text.RegularExpressions.Regex(@"\{.*?\}");
            commandLineOutput = commandLineOutput ?? output;
            return outputRegEx.Replace(commandLineOutput, m => localArgs[m.Value].Value.ToString());
        }

        private static void AttemptToRunCSharp(string csharpCode, Dictionary<string, ArgData> localArgs,
            Type callType, out object output)
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

                output = Activator.CreateInstance(type, argObjects.ToArray());
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

                        output = enumValue;
                        return;
                    }

                    // Else return c# code as string
                    {
                        output = csharpCode;
                        return;
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

                var self = (object)null;
                if (argStrings.Length > 0 && argStrings[0].StartsWith("this"))
                {
                    var selfString = argStrings[0][4..].Trim();
                    self = localArgs[selfString].Value;
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

                if (method == null) throw new ArgumentException("Method not found!");

                output = method.Invoke(self, argObjects);
                // Function Call Example
                // UnityEngine.GameObject.Find({GameObject Name})
            }
        }

        private static void AttemptToRunCommand(string formattedCommandString,
            Dictionary<string, ArgData> localArgs, out object output)
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
                    args[i] = new ArgData(argString, typeof(string), argString, false);
            }

            output = Run(alias, localArgs, args);
        }

        private static object Run(string alias, Dictionary<string, ArgData> localArgs, params ArgData[] args)
        {
            AttemptUpdateAliasMap();
            var argTypes = args.Select(x => x.Type).ToArray();
            var commandJson = aliasMap[alias.ToLower()];
            var version = commandJson["Version"]?.Value<int>();
            var name = commandJson["Name"]?.ToString();
            var description = commandJson["Description"]?.ToString();
            var aliases = commandJson["Aliases"] as JArray;
            var overloads = commandJson["Overloads"] as JArray;
            var overload = FindBestOverload(overloads, args);
            if (overload == null) throw new ArgumentException("No overload found!");
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
                if (argRequired && argValue == null) throw new ArgumentException($"Argument {argName} is required!");
                if (!argRequired && argValue == null) continue;
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
                        AttemptToRunCSharp(csharp, localArgs, callType, out var callOutput);
                        callOutput = ConvertType(callOutput, callType);
                        localArgs[callName] = new ArgData(callName, callType, callOutput, false);
                    }
                    else if (command != null)
                    {
                        AttemptToRunCommand(command, localArgs, out var callOutput);
                        callOutput = ConvertType(callOutput, callType);
                        localArgs[callName] = new ArgData(callName, callType, callOutput, false);
                    }
                    else
                    {
                        throw new ArgumentException("No CSharp or Command found!");
                    }
                }
                else
                {
                    if (csharp != null)
                        AttemptToRunCSharp(csharp, localArgs, null, out _);
                    else if (command != null)
                        AttemptToRunCommand(command, localArgs, out _);
                    else
                        throw new ArgumentException("No CSharp or Command found!");
                }
            }

            var outputName = output?["Name"]?.ToString();
            return outputName != null && outputName != "void" ? localArgs[outputName].Value : null;
        }

        private static void AttemptUpdateAliasMap()
        {
            if (Application.isPlaying)
            {
                if (Time.time > _nextUpdate)
                {
                    _nextUpdate = Time.time + UpdateRate;
                    UpdateAliasMap();
                }
            }
            else
            {
#if UNITY_EDITOR
                if (UnityEditor.EditorApplication.timeSinceStartup > _nextUpdate)
                {
                    _nextUpdate = (float)UnityEditor.EditorApplication.timeSinceStartup + UpdateRate;
                    UpdateAliasMap();
                }
#endif
            }
        }

        private static void UpdateAliasMap()
        {
            aliasMap.Clear();
#if UNITY_EDITOR
            var jsonFiles =
                UnityEditor.AssetDatabase.FindAssets("t:TextAsset", new[] { "Assets/CommandSystem/Commands/User" });
            foreach (var jsonFile in jsonFiles)
            {
                var jsonFilePath = UnityEditor.AssetDatabase.GUIDToAssetPath(jsonFile);
                var jsonFileText = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(jsonFilePath).text;
                var jsonFileJObject = JObject.Parse(jsonFileText);
                var jsonFileAlias = jsonFileJObject["Aliases"];
                if (jsonFileAlias == null) continue;
                foreach (var alias in jsonFileAlias)
                    aliasMap[alias.ToString().ToLower()] = jsonFileJObject;
            }
#endif
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
                    if (i >= args.Length) throw new ArgumentException("Not enough arguments!");
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
                foreach(var argElement in (Array)argData.Value)
                    if (!IsConvertible(new ArgData(argData.Name, argElementType, argElement, argData.Required), inputElementType))
                        return false;
                return true;
            }
            return false;
        }

        private static object ConvertType(object obj, Type toType)
        {
            if (obj == null || toType == null) return obj;
            var fromType = obj.GetType();
            if (fromType == toType) return obj;
            if (toType.IsAssignableFrom(fromType)) return obj;
            if (toType.IsEnum && fromType == typeof(int))
                return obj;
            if (toType.IsEnum && fromType == typeof(string))
                return Enum.Parse(toType, obj.ToString());
            if (toType == typeof(int) && fromType == typeof(string))
                return int.Parse(obj.ToString());
            if (toType == typeof(float) && fromType == typeof(string))
                return float.Parse(obj.ToString());
            if (toType == typeof(double) && fromType == typeof(string))
                return double.Parse(obj.ToString());
            if (toType == typeof(bool) && fromType == typeof(string))
                return bool.Parse(obj.ToString());

            if (toType.IsArray && fromType.IsArray)
            {
                // Convert array types
                var fromElementType = fromType.GetElementType();
                var toElementType = toType.GetElementType();
                if (fromElementType == toElementType) return obj;
                var fromArray = (Array)obj;
                var toArray = Array.CreateInstance(toElementType, fromArray.Length);
                for (var i = 0; i < fromArray.Length; i++)
                {
                    var fromElement = fromArray.GetValue(i);
                    var toElement = ConvertType(fromElement, toElementType);
                    toArray.SetValue(toElement, i);
                }
                return toArray;
            }
            
            return obj is IConvertible ? Convert.ChangeType(obj, toType) : obj;
        }
    }
}