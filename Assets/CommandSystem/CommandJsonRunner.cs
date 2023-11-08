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

            var argTypes = new Type[args.Length];
            argTypes = argTypes.Select(x => typeof(string)).ToArray();

            var version = commandJson["Version"]?.Value<int>();
            var typeFullName = commandJson["TypeFullName"]?.ToString();
            var description = commandJson["Description"]?.ToString();
            var aliases = commandJson["Aliases"] as JArray;
            var overloads = commandJson["Overloads"] as JArray;
            var overload = FindBestOverload(overloads, args, argTypes);
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
                arg = argType.IsEnum ? Enum.Parse(argType, arg.ToString()) : arg;
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
                    if (csharp != null)
                    {
                        AttemptToRunCSharp(csharp, localArgs, out var callOutput);
                        localArgs[callName] = new ArgData(callName, StringToTypeUtility.Get(callTypeString), callOutput, false);
                    }
                    else if (command != null)
                    {
                        AttemptToRunCommand(command, localArgs, out var callOutput);
                        localArgs[callName] = new ArgData(callName, StringToTypeUtility.Get(callTypeString), callOutput, false);
                    }
                    else
                    {
                        throw new ArgumentException("No CSharp or Command found!");
                    }
                }
                else
                {
                    if (csharp != null)
                        AttemptToRunCSharp(csharp, localArgs, out _);
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
            out object output)
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
                var methodSplit = split[0].Split('.');
                var methodName = methodSplit[^1];
                var fullTypeName = methodSplit[..^1];
                var fullTypeString = string.Join('.', fullTypeName);
                var type = StringToTypeUtility.Get(fullTypeString);
                var argsString = split[1][..^1];
                var argStrings = argsString.Split(',').Select(x => x.Trim()).ToArray();

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
                    var arg = localArgs[argString];
                    args[i] = arg;
                }

                var argTypes = args.Select(x => x.Type).ToArray();
                var argObjects = args.Select(x => x.Value).ToArray();
                var method = type.GetMethod(methodName, Public | NonPublic | Instance | Static, null, argTypes, null);
                output = method.Invoke(self, argObjects);
                // Function Call Example
                // UnityEngine.GameObject.Find({GameObject Name})
            }
        }

        private static void AttemptToRunCommand(string formattedCommandString,
            Dictionary<string, ArgData> localArgs, out object p3)
        {
            // getscenepath {New GameObject}
            // alias = getscenepath
            // argStrings = {New GameObject}

            var alias = formattedCommandString.Split(' ')[0];

            // Use RegEx to get each argument between { }. Name between { } can have spaces. Include { } in RegEx.
            // RegEx should also include arguments with no { } and separate by spaces.
            var regex = new System.Text.RegularExpressions.Regex(@"[\{].+?[\}]|[^ ]+");
            var argStrings = regex.Matches(formattedCommandString).Select(x => x.Value).Skip(1).ToArray();

            var args = new object[argStrings.Length];
            for (var i = 0; i < argStrings.Length; i++)
            {
                var argString = argStrings[i];
                if (localArgs.TryGetValue(argString, out var arg))
                    args[i] = arg.Value;
                else
                    args[i] = argString;
            }

            var output = Run(alias, localArgs, args);
            p3 = output;
        }

        private static object Run(string alias, Dictionary<string, ArgData> localArgs, params object[] args)
        {
            AttemptUpdateAliasMap();
            var argTypes = args.Select(x => x?.GetType()).ToArray();
            var commandJson = aliasMap[alias.ToLower()];
            var version = commandJson["Version"]?.Value<int>();
            var typeFullName = commandJson["TypeFullName"]?.ToString();
            var description = commandJson["Description"]?.ToString();
            var aliases = commandJson["Aliases"] as JArray;
            var overloads = commandJson["Overloads"] as JArray;
            var overload = FindBestOverload(overloads, args, argTypes);
            if (overload == null) throw new ArgumentException("No overload found!");
            var input = overload["Input"] as JArray;
            var calls = overload["Calls"] as JArray;
            var output = overload["Output"] as JObject;
            var commandLineOutput = overload["CommandLineOutput"]?.ToString();

            localArgs = new Dictionary<string, ArgData>(localArgs);
            for (var i = 0; i < (input?.Count ?? 0); i++)
            {
                var arg = i < args.Length ? args[i] : null;
                var argName = input[i]["Name"]?.ToString();
                var argRequired = input[i]["Required"]?.Value<bool>() == true;
                var argTypeString = input[i]["Type"]?.ToString();
                var argType = StringToTypeUtility.Get(argTypeString);
                if (argRequired && arg == null) throw new ArgumentException($"Argument {argName} is required!");
                if (!argRequired && arg == null) continue;
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
                    if (csharp != null)
                    {
                        AttemptToRunCSharp(csharp, localArgs, out var callOutput);
                        localArgs[callName] = new ArgData(callName, StringToTypeUtility.Get(callTypeString), callOutput, false);
                    }
                    else if (command != null)
                    {
                        AttemptToRunCommand(command, localArgs, out var callOutput);
                        localArgs[callName] = new ArgData(callName, StringToTypeUtility.Get(callTypeString), callOutput, false);
                    }
                    else
                    {
                        throw new ArgumentException("No CSharp or Command found!");
                    }
                }
                else
                {
                    if (csharp != null)
                        AttemptToRunCSharp(csharp, localArgs, out _);
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

        private static JObject FindBestOverload(JArray overloads, object[] args, Type[] argTypes)
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
                    if (i >= argTypes.Length) throw new ArgumentException("Not enough arguments!");
                    var inputType = inputTypes[i];
                    var argType = argTypes[i];
                    var maybeConvertibleToEnum = inputType.IsEnum && (argType == typeof(string) || argType == typeof(int));
                    if (inputType.IsAssignableFrom(argType) || maybeConvertibleToEnum) continue;
                    isMatch = false;
                    break;
                }

                if (isMatch) return overload as JObject;
            }

            return null;
        }
    }
}