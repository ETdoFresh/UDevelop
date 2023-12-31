﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandSystem.Commands.Select;
using UnityEngine;
using Object = UnityEngine.Object;
using static System.Reflection.BindingFlags;

namespace CommandSystem
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

        public static string GetGameObjectScenePathWithIndex(GameObject gameObject)
        {
            var path = gameObject.name;
            var parent = gameObject.transform.parent;
            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }

            return $"{path}[{GetPathIndex(gameObject)}]";
        }

        private static int GetPathIndex(Object obj)
        {
            Transform transform;

            if (obj is GameObject gameObject) transform = gameObject.transform;
            else if (obj is Component component) transform = component.transform;
            else return -1;

            var parent = transform.parent;
            if (parent != null)
            {
                var pathIndex = 0;
                for (var i = 0; i < parent.childCount; i++)
                {
                    var child = parent.GetChild(i);
                    if (child == transform) return pathIndex;
                    if (child.name == transform.name) pathIndex++;
                }
            }
            else
            {
                var rootSceneObjects = Object.FindObjectsOfType<GameObject>().Where(x => !x.transform.parent)
                    .Select(x => x.transform);
                var pathIndex = 0;
                foreach (var rootSceneObject in rootSceneObjects)
                {
                    if (rootSceneObject == transform) return pathIndex;
                    if (rootSceneObject.name == transform.name) pathIndex++;
                }
            }

            return -1;
        }

        public static string GetObjectsScenePath(object[] objects)
        {
            var scenePaths = "";
            foreach (var obj in objects)
            {
                if (obj is GameObject go) scenePaths += GetGameObjectScenePath(go) + "\n";
                else if (obj is Component c) scenePaths += GetGameObjectScenePath(c.gameObject) + "\n";
                else if (obj is Object o) scenePaths += o.name + "\n";
                else scenePaths += obj + "\n";
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
            var commandMap = CommandRunner.CommandMap;
            var helpCommand = commandMap["help"][0];

            {
                if (string.IsNullOrEmpty(commandAlias))
                {
                    var seenCommandObjects = new HashSet<CommandObject[]>();
                    var output = $"----------------------------\n";
                    output += $"{helpCommand.Name} v{helpCommand.Version} - List of Commands\n";
                    output += "----------------------------\n";
                    foreach (var commandObjects in commandMap.Values)
                    {
                        if (seenCommandObjects.Contains(commandObjects)) continue;
                        var commandName = commandObjects[0].Name;
                        var commandVersion = commandObjects[0].Version;
                        var aliases = string.Join(", ", commandObjects[0].Aliases);
                        var commandDescription = commandObjects[0].Description;
                        output += $"{commandName} v{commandVersion} \n{commandDescription}\nAliases: {aliases}\n\n";
                        seenCommandObjects.Add(commandObjects);
                    }

                    if (output.EndsWith("\n\n")) output = output[..^2];
                    return output;
                }
            }
            {
                if (commandMap.TryGetValue(commandAlias, out var commandObjects))
                {
                    var commandName = commandObjects[0].Name;
                    var dashes = new string('-', (int)(commandName.Length * 1.6f)) + "\n";
                    var commandVersion = commandObjects[0].Version;
                    var commandDescription = commandObjects[0].Description;
                    var commandAliases = string.Join(", ", commandObjects[0].Aliases);
                    var usages = "";
                    foreach (var overload in commandObjects)
                    {
                        var inputs = overload.Input;
                        var inputNames = inputs.Select(x => x.Name).ToArray();
                        var outputName = overload.Output.Name;
                        var usage = $"\n{commandName} {string.Join(" ", inputNames)}\nOutput: {outputName}";
                        usages += $"{usage}\n";
                    }

                    if (usages.EndsWith("\n")) usages = usages[..^1];
                    return
                        $"{dashes}{commandName} v{commandVersion}\n{dashes}{commandDescription}\nAliases: {commandAliases}\n\nUsages:{usages}";
                }
            }

            throw new Exception($"Could not find command with alias {commandAlias}");
        }

        public static object GetCSharpStaticObject(string fullName)
        {
            var typeName = fullName[..fullName.LastIndexOf('.')];
            var fieldName = fullName[(fullName.LastIndexOf('.') + 1)..];
            var type = FindSystemTypeByName(typeName);
            var fieldOrProperty = GetFieldInfoOrPropertyInfo(type, fieldName);
            var getValueMethod = fieldOrProperty?.GetType().GetMethod("GetValue", Public | NonPublic | Instance);
            if (getValueMethod != null)
                return getValueMethod.Invoke(fieldOrProperty, new object[] { null });

            throw new Exception($"Could not find field or property {fieldName} on type {type}");
        }

        public static object GetFieldInfoOrPropertyInfo(Type type, string fieldOrPropertyName)
        {
            var field = type.GetField(fieldOrPropertyName, Public | NonPublic | Instance);
            if (field != null) return field;
            var property = type.GetProperty(fieldOrPropertyName, Public | NonPublic | Instance);
            if (property != null) return property;
            throw new Exception($"Could not find field or property {fieldOrPropertyName} on type {type.FullName}");
        }

        public static object GetFieldInfo(Type type, string fieldName)
        {
            var field = type.GetField(fieldName, Public | NonPublic | Instance);
            if (field != null) return field;
            throw new Exception($"Could not find field {fieldName} on type {type.FullName}");
        }

        public static object Or(object obj1, object obj2)
        {
            if (obj1 is bool b1 && obj2 is bool b2)
                return b1 || b2;
            else
                return obj1 ?? obj2;
        }

        public static bool IsNull(object obj)
        {
            return obj == null;
        }

        public static object Cast(object obj, Type type)
        {
            return Convert.ChangeType(obj, type);
        }

        public static object[] Filter(FieldInfo fieldInfo, object value, object[] array)
        {
            return array.Where(x => IsEqual(fieldInfo?.GetValue(x), value)).ToArray();
        }

        public static object[] Filter(PropertyInfo propertyInfo, object value, object[] array)
        {
            return array.Where(x => IsEqual(propertyInfo?.GetValue(x), value)).ToArray();
        }

        public static object[] Filter(MethodInfo methodInfo, object value, object[] array)
        {
            return array.Where(x => IsEqual(methodInfo?.Invoke(x, null), value)).ToArray();
        }

        public static object[] Filter(CommandReference commandReference, object value, object[] array)
        {
            return array.Where(x => IsEqual(commandReference
                .Run(new ArgData("{FilterByArg}", x.GetType(), x, true)), value)).ToArray();
        }

        public static object Filter(bool[] keepArray, object[] array)
        {
            if (keepArray.Length != array.Length)
                throw new Exception(
                    $"Length of keepArray ({keepArray.Length}) must match length of array ({array.Length})");
            return array.Where((x, i) => keepArray[i]).ToArray();
        }

        public static object Map(FieldInfo fieldInfo, object[] array)
        {
            return array.Select(fieldInfo.GetValue).ToArray();
        }

        public static object Map(PropertyInfo propertyInfo, object[] array)
        {
            return array.Select(propertyInfo.GetValue).ToArray();
        }

        public static object Map(MethodInfo methodInfo, object[] array)
        {
            return array.Select(x => methodInfo.Invoke(x, null)).ToArray();
        }

        public static object Map(CommandReference commandReference, object[] array)
        {
            return array.Select(x => commandReference
                .Run(new ArgData("{MapByArg}", x.GetType(), x, true))).ToArray();
        }

        public static object Reduce(MethodInfo methodInfo, object initialValue, object[] array)
        {
            return array.Aggregate(initialValue, (x, y) => methodInfo.Invoke(x, new[] { y }));
        }

        public static object Reduce(CommandReference commandReference, object initialValue, object[] array)
        {
            return array.Aggregate(initialValue,
                (x, y) => commandReference.Run(
                    new ArgData("{ReduceByArg1}", x.GetType(), x, true),
                    new ArgData("{ReduceByArg2}", y.GetType(), y, true)));
        }

        public static object[] SortBy(FieldInfo fieldInfo, object[] array)
        {
            return array.OrderBy(fieldInfo.GetValue).ToArray();
        }

        public static object[] SortBy(PropertyInfo propertyInfo, object[] array)
        {
            return array.OrderBy(propertyInfo.GetValue).ToArray();
        }

        public static object ToArray(object obj)
        {
            if (obj == null) return Array.Empty<object>();
            if (obj is Array array) return array;
            if (obj is string) return new object[] { obj };
            if (obj is IEnumerable enumerable) return enumerable.Cast<object>().ToArray();
            return new[] { obj };
        }

        public static string AddIndexIfMissing(string path)
        {
            string[] paths = Array.Empty<string>();
            System.Linq.Enumerable.OrderBy(paths, x => x);
            return path.Contains("[") && path.Contains("]") ? path : $"{path.TrimEnd()}[0]";
        }

        public static string[] AddIndexIfMissing(string[] paths)
        {
            return paths.Select(AddIndexIfMissing).ToArray();
        }

        public static object ParseEnum(Type enumType, string enumString)
        {
            if (enumType == null) throw new ArgumentNullException(nameof(enumType));
            if (enumString == null) throw new ArgumentNullException(nameof(enumString));
            if (!enumType.IsEnum) throw new ArgumentException("Type provided must be an Enum.", nameof(enumType));
            if (!enumString.Contains("|")) return Enum.Parse(enumType, enumString);
            var enumStrings = enumString.Split('|');
            var enumValues = enumStrings.Select(x => Enum.Parse(enumType, x)).ToArray();
            var enumValue = enumValues.Aggregate(0, (current, value) => current | (int)value);
            return Enum.ToObject(enumType, enumValue);
        }

        public static object CastArray(object arrayObject, Type newArrayType)
        {
            if (!newArrayType.IsArray)
                throw new ArgumentException("Type provided must be an array.", nameof(newArrayType));
            var arrayType = arrayObject.GetType();
            if (!arrayType.IsArray)
                throw new ArgumentException("Object provided must be an array.", nameof(arrayObject));
            var newElementType = newArrayType.GetElementType();
            if (newElementType == null)
                throw new ArgumentException("Type provided must be an array.", nameof(newArrayType));
            var array = (Array)arrayObject;
            var newArray = Array.CreateInstance(newElementType, array.Length);
            for (var i = 0; i < array.Length; i++)
            {
                var element = array.GetValue(i);
                if (newElementType.IsInstanceOfType(element))
                {
                    newArray.SetValue(element, i);
                }
                else
                {
                    try
                    {
                        newArray.SetValue(Convert.ChangeType(element, newElementType), i);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            $"Could not cast element {i} of array {arrayObject} to type {newElementType}", e);
                    }
                }
            }

            return newArray;
        }

        public static string[] AssetDatabaseFindAssets(string filter)
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.FindAssets(filter, new[] { "Assets" }).ToArray();
#else
            return Array.Empty<string>();
#endif
        }

        public static string[] AssetDatabaseGUIDToAssetPath(string[] guids)
        {
#if UNITY_EDITOR
            return guids.Select(UnityEditor.AssetDatabase.GUIDToAssetPath).ToArray();
#else
            return Array.Empty<string>();
#endif
        }

        public static Object[] AssetDatabaseLoadAssetAtPath(string[] assetPaths)
        {
#if UNITY_EDITOR
            return assetPaths.Select(UnityEditor.AssetDatabase.LoadAssetAtPath<Object>).ToArray();
#else
            return Array.Empty<Object>();
#endif
        }

        public static Object[] ResourcesLoadAll(string path)
        {
            var assetsViaResourcesLoadAll = Array.Empty<Object>();
            return Resources.LoadAll(path);
        }

        public static string AssetDatabaseGetAssetPath(Object obj)
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.GetAssetPath(obj);
#else
            return "{GetAssetPath is not supported in builds}";
#endif
        }

        public static string[] AssetDatabaseGetAssetPath(Object[] objects)
        {
            return objects.Select(AssetDatabaseGetAssetPath).ToArray();
        }

        public static object[] OrderBy(object[] array)
        {
            return array.OrderBy(x => x).ToArray();
        }

        public static object[] OrderByDescending(object[] array)
        {
            return array.OrderByDescending(x => x).ToArray();
        }

        public static object ThrowExceptionIfNull(object obj) => ThrowExceptionIfNull(obj, "Object is null");

        public static object ThrowExceptionIfNull(object obj, string message)
        {
            if (obj == null) throw new Exception(message);
            return obj;
        }

        private static bool IsEqual(object a, object b)
        {
            if (a == null && b == null) return true;
            if (a == null || b == null) return false;
            return a.Equals(b);
        }
    }
}