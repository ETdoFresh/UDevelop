using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace CommandSystem
{
    public static class CommandJsonRunner
    {
        private const float UpdateRate = 3600;
        private static float _nextUpdate;
        private static Dictionary<string, JObject> aliasMap = new();

        public static T ProcessCommandInputString<T>(string commandString)
        {
            AttemptUpdateAliasMap();
            var commandJson = aliasMap["newgameobject"];
            var args = Array.Empty<object>();
            var version = commandJson["Version"]?.Value<int>();
            var typeFullName = commandJson["TypeFullName"]?.ToString();
            var description = commandJson["Description"]?.ToString();
            var aliases = commandJson["Aliases"] as JArray;
            var overloads = commandJson["Overloads"] as JArray;
            var overload = FindBestOverload(overloads, args);
            if (overload == null) throw new ArgumentException("No overload found!");
            var input = overload["Input"] as JArray;
            var calls = overload["Calls"] as JArray;
            var output = overload["Output"]?.ToString();
            var commandLineOutput = overload["CommandLineOutput"]?.ToString();
            
            var localRunValues = new Dictionary<string, object>();
            var localRunTypes = new Dictionary<string, Type>();
            for (var i = 0; i < (input?.Count ?? 0); i++)
            {
                var arg = i < args.Length ? args[i] : null;
                var argName = input[i]["Name"]?.ToString();
                var argTypeString = input[i]["Type"]?.ToString();
                var argType = StringToTypeUtility.Get(argTypeString);
                localRunValues.Add(argName, arg);
                localRunTypes.Add(argName, argType);
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
                        AttemptToRunCSharp(csharp, localRunValues, localRunTypes, out var callOutput);
                        localRunValues.Add(callName, callOutput);
                        localRunTypes.Add(callName, StringToTypeUtility.Get(callTypeString));
                    }
                    else if (command != null)
                    {
                        AttemptToRunCommand(command, localRunValues, localRunTypes, out var callOutput);
                        localRunValues.Add(callName, callOutput);
                        localRunTypes.Add(callName, StringToTypeUtility.Get(callTypeString));
                    }
                    else
                    {
                        throw new ArgumentException("No CSharp or Command found!");
                    }
                }
                else
                {
                    if (csharp != null)
                        AttemptToRunCSharp(csharp, localRunValues, localRunTypes, out _);
                    else if (command != null)
                        AttemptToRunCommand(command, localRunValues, localRunTypes, out _);
                    else
                        throw new ArgumentException("No CSharp or Command found!");
                }
            }
            
            return default;
        }

        private static void AttemptToRunCSharp(string csharpCode, Dictionary<string, object> localRunValues, Dictionary<string, Type> localRunTypes, out object output)
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
                var argStrings = argsString.Split(',');
                var type = StringToTypeUtility.Get(typeString);
                var argObjects = new object[argStrings.Length];
                for (var i = 0; i < argStrings.Length; i++)
                {
                    var argString = argStrings[i];
                    var arg = localRunValues[argString];
                    argObjects[i] = arg;
                }
                output = Activator.CreateInstance(type, argObjects);
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
                var method = type.GetMethod(methodName);
                var argsString = split[1][..^1];
                var argStrings = argsString.Split(',');
                var argObjects = new object[argStrings.Length];
                for (var i = 0; i < argStrings.Length; i++)
                {
                    var argString = argStrings[i];
                    var arg = localRunValues[argString];
                    argObjects[i] = arg;
                }
                output = method.Invoke(null, argObjects);
                // Function Call Example
                // UnityEngine.GameObject.Find({GameObject Name})
            }
        }

        private static void AttemptToRunCommand(string command, Dictionary<string, object> localRunValues, Dictionary<string, Type> localRunTypes, out object p3)
        {
            throw new NotImplementedException();
        }

        private static object Run(string alias, params object[] args)
        {
            throw new System.NotImplementedException();
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
            var jsonFiles = UnityEditor.AssetDatabase.FindAssets("t:TextAsset", new[] { "Assets/CommandSystem/Commands/User" });
            foreach (var jsonFile in jsonFiles)
            {
                var jsonFilePath = UnityEditor.AssetDatabase.GUIDToAssetPath(jsonFile);
                var jsonFileText = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(jsonFilePath).text;
                var jsonFileJObject = JObject.Parse(jsonFileText);
                var jsonFileAlias = jsonFileJObject["Aliases"];
                if (jsonFileAlias == null) continue;
                foreach (var alias in jsonFileAlias) 
                    aliasMap.Add(alias.ToString(), jsonFileJObject);
            }
            #endif
        }

        private static JObject FindBestOverload(JArray overloads, object[] args)
        {
            var argLength = args.Length;
            foreach (var overload in overloads)
            {
                var inputMinLength = overload["Input"]?.Where(x => x["Required"]?.Value<bool>() == true).Count() ?? 0;
                var inputMaxLength = overload["Input"]?.Count() ?? 0;
                if (argLength < inputMinLength || argLength > inputMaxLength) continue;
                return overload as JObject;
            }
            return null;
        }
    }
}