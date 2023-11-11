using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem.Commands.Select;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using static System.Reflection.BindingFlags;

namespace CommandSystem.Commands
{
    public static class Utility
    {
        public static string GetGameObjectScenePath(GameObject gameObject)
        {
            var path = gameObject.name;
            var parent = gameObject.transform.parent;
            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }

            return path; 
        }
        
        public static string GetObjectsScenePath(Object[] objects)
        {
            var scenePaths = "";
            foreach (var obj in objects)
            {
                if (obj is GameObject go) scenePaths += GetGameObjectScenePath(go) + "\n";
                else if (obj is Component c) scenePaths += GetGameObjectScenePath(c.gameObject) + "\n";
                else scenePaths += obj.name + "\n";
            }
            if (scenePaths.EndsWith("\n")) scenePaths = scenePaths[..^1];
            return scenePaths;
        }
        
        public static GameObject FindGameObjectByName(string gameObjectName)
        {
            var objectNameWithoutIndex = SelectionUtil.RemoveIndexFromName(gameObjectName);
            const StringComparison ignoreCase = StringComparison.CurrentCultureIgnoreCase;
            var objectsByName = Object
                .FindObjectsOfType<GameObject>(true)
                .Where(x => string.Equals(x.name, objectNameWithoutIndex, ignoreCase))
                .OrderBy(SelectionUtil.GetGameObjectOrder)
                .Cast<Object>();
            var selectedObjects = SelectionUtil.ParseAndSelectIndex(objectsByName, gameObjectName);
            return selectedObjects.FirstOrDefault() as GameObject;
        }
        
        public static object GetValue(object obj, string name)
        {
            var type = obj.GetType();
            var field = type.GetField(name, Public | NonPublic | Instance);
            if (field != null)
            {
                return field.GetValue(obj);
            }

            var property = type.GetProperty(name, Public | NonPublic | Instance);
            if (property != null)
            {
                return property.GetValue(obj);
            }

            return null;
        }
        
        public static void SetValue(object obj, string name, object value)
        {
            var type = obj.GetType();
            var field = type.GetField(name, Public | NonPublic | Instance);
            if (field != null)
            {
                field.SetValue(obj, value);
                return;
            }

            var property = type.GetProperty(name, Public | NonPublic | Instance);
            if (property != null)
            {
                property.SetValue(obj, value);
                return;
            }
            
            throw new Exception($"Could not find field or property {name} on type {type}");
        }
        
        public static Type FindSystemTypeByName(string typeNameString)
        {
            return StringToTypeUtility.Get(typeNameString);
        }

        public static Object[] FindObjectsByName(string objectName)
        {
            return Object.FindObjectsOfType<GameObject>().Where(x => x.name == objectName).Cast<Object>().ToArray();
        }

        public static Object[] FindObjectsByName(string objectName, Object[] objectArray)
        {
            return objectArray.Where(x => x.name == objectName).ToArray();
        }
        
        public static string Help(string commandAlias)
        {
            var aliasMap = CommandJsonRunner.AliasMap;

            {
                if (string.IsNullOrEmpty(commandAlias))
                {
                    var seenCommandJObjects = new HashSet<JObject>();
                    var output = "";
                    foreach (var jObject in aliasMap.Values)
                    {
                        if (seenCommandJObjects.Contains(jObject))
                        {
                            continue;
                        }

                        var commandName = jObject["Name"]?.ToString();
                        var commandDescription = jObject["Description"]?.ToString();
                        output += $"{commandName}\n{commandDescription}\n\n";
                        seenCommandJObjects.Add(jObject);
                    }
                    if (output.EndsWith("\n\n")) output = output[..^2];
                    return output;
                }
            }
            {
                if (aliasMap.TryGetValue(commandAlias, out var jObject))
                {
                    var commandName = jObject["Name"]?.ToString();
                    var commandDescription = jObject["Description"]?.ToString();
                    var commandAliases = string.Join(", ", jObject["Aliases"]?.Select(x => x.ToString()).ToArray() ?? Array.Empty<string>());
                    var commandOverloads = jObject["Overloads"] as JArray;
                    var usages = "";
                    foreach (var overload in commandOverloads)
                    {
                        var inputs = overload["Input"] as JArray;
                        var inputNames = inputs.Select(x => x["Name"].ToString()).ToArray();
                        var outputName = overload["Output"]?["Name"]?.ToString();
                        var usage = $"{commandName} {string.Join(" ", inputNames)}\nOutput: {outputName}";
                        usages += $"{usage}\n";
                    }
                    if (usages.EndsWith("\n")) usages = usages[..^1];
                    return $"\n{commandName}\n{commandDescription}\nAliases: {commandAliases}\n\n{usages}";
                }
            }
            
            throw new Exception($"Could not find command with alias {commandAlias}");
        }
    }
}