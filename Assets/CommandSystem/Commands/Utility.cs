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

                        var commandName = jObject["TypeFullName"]?.ToString();
                        var commandDescription = jObject["Description"]?.ToString();
                        output += $"{commandName}\n{commandDescription}\n\n";
                        seenCommandJObjects.Add(jObject);
                    }
                    return output;
                }
            }
            {
                if (aliasMap.TryGetValue(commandAlias, out var jObject))
                {
                    var commandName = jObject["TypeFullName"]?.ToString();
                    var commandDescription = jObject["Description"]?.ToString();
                    return $"{commandName}\n{commandDescription}";
                }
            }
            
            throw new Exception($"Could not find command with alias {commandAlias}");
        }
    }
}