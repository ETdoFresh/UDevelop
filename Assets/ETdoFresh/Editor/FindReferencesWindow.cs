using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ETdoFresh.Editor
{
    public class FindReferencesWindow : EditorWindow
    {
        private Dictionary<string, string[]> _projectDependencyCache;

        private Object _previousObjectToFind;
        private Object _objectToFind;
        private string _objectToFindGuid;
        private Object[] _results;
        private Vector2 _scrollPosition;

        [MenuItem("ETdoFresh/Find References")]
        public static void ShowWindow()
        {
            GetWindow(typeof(FindReferencesWindow), false, "Find References");
        }

        private void OnGUI()
        {
            if (_previousObjectToFind != _objectToFind)
            {
                _previousObjectToFind = _objectToFind;
                _results = null;
                _objectToFindGuid = null;
            }

            if (_objectToFind == null && _results != null)
            {
                _results = null;
            }

            if (_objectToFind == null && _objectToFindGuid != null)
            {
                _objectToFindGuid = null;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(_projectDependencyCache != null
                ? $" Project Dependency Cache: {_projectDependencyCache.Count}"
                : "Project Dependency Cache: null");

            if (_projectDependencyCache != null)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Reload"))
                {
                    if (!EditorUtility.DisplayDialog("Reload Project Dependency Cache",
                            "This can take a long time and may freeze Unity. Are you sure you want to search the whole project?",
                            "Yes", "No"))
                        return;
                    LoadProjectDependencyCache();
                }
                if (GUILayout.Button("Reload [Recursive]"))
                {
                    if (!EditorUtility.DisplayDialog("Reload Project Dependency Cache",
                            "This can take a long time and may freeze Unity. Are you sure you want to search the whole project?",
                            "Yes", "No"))
                        return;
                    LoadProjectDependencyCache(true);
                }
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();

            _objectToFind = EditorGUILayout.ObjectField("Object to find", _objectToFind, typeof(Object), true);

            if (_objectToFind != null && _objectToFindGuid == null)
            {
                var instanceIDs = new NativeArray<int>(new[] { _objectToFind.GetInstanceID() }, Allocator.Temp);
                var guidsOut = new NativeArray<GUID>(new GUID[] { default }, Allocator.Temp);
                AssetDatabase.InstanceIDsToGUIDs(instanceIDs, guidsOut);
                _objectToFindGuid = guidsOut[0].ToString();
            }

            EditorGUILayout.LabelField("GUID", _objectToFindGuid ?? "No GUID found");
            EditorGUILayout.Space();

            EditorGUI.BeginDisabledGroup(_objectToFind == null || _objectToFindGuid == null);
            if (GUILayout.Button("Find References in Project"))
            {
                // Confirm that the user wants to search the whole project
                if (_projectDependencyCache == null && !EditorUtility.DisplayDialog("Find References in Project",
                        "This can take a long time and may freeze Unity. Are you sure you want to search the whole project?",
                        "Yes", "No"))
                    return;

                var path = AssetDatabase.GetAssetPath(_objectToFind);
                _objectToFindGuid = AssetDatabase.AssetPathToGUID(path);

                if (_projectDependencyCache == null)
                    LoadProjectDependencyCache();
                
                var results = new List<Object>();
                foreach (var assetPath in _projectDependencyCache.Keys)
                {
                    if (assetPath == path) continue;  // Skip the object we're searching for
                    var dependencies = _projectDependencyCache[assetPath];
                    if (dependencies.All(dependency => dependency != path)) continue;
                    var asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                    results.Add(asset);
                }
                
                foreach (var prefabGuid in AssetDatabase.FindAssets("t:Prefab"))
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(prefabGuid);
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                    foreach(var prefabComponent in prefab.GetComponentsInChildren<Component>(true))
                    {
                        var component = prefabComponent;
                        if (!component) continue;
                        var fields = component.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        foreach (var field in fields)
                        {
                            if (field.FieldType == _objectToFind.GetType())
                            {
                                var value = field.GetValue(component);
                                if (value == _objectToFind)
                                {
                                    results.Add(component);
                                    break;
                                }
                            }
                        }
                        
                        var properties = component.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        foreach (var property in properties)
                        {
                            try
                            {
                                if (property.PropertyType == _objectToFind.GetType())
                                {
                                    if (property.DeclaringType == typeof(Renderer) && property.Name == "material")
                                        continue;
                                    if (property.DeclaringType == typeof(Renderer) && property.Name == "materials")
                                        continue;
                                    var value = property.GetValue(component);
                                    if (value == _objectToFind)
                                    {
                                        results.Add(component);
                                        break;
                                    }
                                }
                            }
                            catch
                            {
                                // ignore
                            }
                        }
                        
                        // Check field nested classes/structs
                        foreach (var field in fields)
                        {
                            try
                            {
                                if (field.FieldType.IsClass || field.FieldType.IsValueType)
                                {
                                    var nestedFields = field.FieldType.GetFields(System.Reflection.BindingFlags.Public |
                                        System.Reflection.BindingFlags.NonPublic |
                                        System.Reflection.BindingFlags.Instance);
                                    foreach (var nestedField in nestedFields)
                                    {
                                        if (nestedField.FieldType == _objectToFind.GetType())
                                        {
                                            var value = nestedField.GetValue(field.GetValue(component));
                                            if (value == _objectToFind)
                                            {
                                                results.Add(component);
                                                break;
                                            }
                                        }
                                    }

                                    var nestedProperties = field.FieldType.GetProperties(
                                        System.Reflection.BindingFlags.Public |
                                        System.Reflection.BindingFlags.NonPublic |
                                        System.Reflection.BindingFlags.Instance);
                                    foreach (var nestedProperty in nestedProperties)
                                    {
                                        try
                                        {
                                            if (nestedProperty.PropertyType == _objectToFind.GetType())
                                            {
                                                if (nestedProperty.DeclaringType == typeof(Renderer) && nestedProperty.Name == "material") continue;
                                                if (nestedProperty.DeclaringType == typeof(Renderer) && nestedProperty.Name == "materials") continue;
                                                var value = nestedProperty.GetValue(field.GetValue(component));
                                                if (value == _objectToFind)
                                                {
                                                    results.Add(component);
                                                    break;
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            // ignore
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                // ignore
                            }
                        }

                        // Check property nested classes/structs
                        foreach (var property in properties)
                        {
                            try
                            {
                                if (property.PropertyType.IsClass || property.PropertyType.IsValueType)
                                {
                                    var nestedFields = property.PropertyType.GetFields(System.Reflection.BindingFlags.Public |
                                        System.Reflection.BindingFlags.NonPublic |
                                        System.Reflection.BindingFlags.Instance);
                                    foreach (var nestedField in nestedFields)
                                    {
                                        if (nestedField.FieldType == _objectToFind.GetType())
                                        {
                                            if (property.DeclaringType == typeof(Renderer) && property.Name == "material") continue;
                                            if (property.DeclaringType == typeof(Renderer) && property.Name == "materials") continue;
                                            var value = nestedField.GetValue(property.GetValue(component));
                                            if (value == _objectToFind)
                                            {
                                                results.Add(component);
                                                break;
                                            }
                                        }
                                    }

                                    var nestedProperties = property.PropertyType.GetProperties(
                                        System.Reflection.BindingFlags.Public |
                                        System.Reflection.BindingFlags.NonPublic |
                                        System.Reflection.BindingFlags.Instance);
                                    foreach (var nestedProperty in nestedProperties)
                                    {
                                        try
                                        {
                                            if (nestedProperty.PropertyType == _objectToFind.GetType())
                                            {
                                                if (nestedProperty.DeclaringType == typeof(Renderer) && nestedProperty.Name == "material") continue;
                                                if (nestedProperty.DeclaringType == typeof(Renderer) && nestedProperty.Name == "materials") continue;
                                                if (property.DeclaringType == typeof(Renderer) && property.Name == "material") continue;
                                                if (property.DeclaringType == typeof(Renderer) && property.Name == "materials") continue;
                                                var value = nestedProperty.GetValue(property.GetValue(component));
                                                if (value == _objectToFind)
                                                {
                                                    results.Add(component);
                                                    break;
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            // ignore
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                // ignore
                            }
                        }
                        
                        // Check field arrays/enumerables
                        foreach (var field in fields)
                        {
                            try
                            {
                                if (field.FieldType.IsArray && field.FieldType.GetElementType() == _objectToFind.GetType())
                                {
                                    if (field.GetValue(component) is not Array array) continue;
                                    foreach (var item in array)
                                    {
                                        if (item == _objectToFind)
                                        {
                                            results.Add(component);
                                            break;
                                        }
                                    }
                                }
                                else if (field.FieldType.IsGenericType && field.FieldType.GetGenericArguments()[0] == _objectToFind.GetType())
                                {
                                    var enumerable = field.GetValue(component) as IEnumerable<object>;
                                    if (enumerable == null) continue;
                                    foreach (var item in enumerable)
                                    {
                                        if (item == _objectToFind)
                                        {
                                            results.Add(component);
                                            break;
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                // ignore
                            }
                        }
                        
                        // Check property arrays/enumerables
                        foreach (var property in properties)
                        {
                            try
                            {
                                if (property.PropertyType.IsArray && property.PropertyType.GetElementType() == _objectToFind.GetType())
                                {
                                    if (property.DeclaringType == typeof(Renderer) && property.Name == "material") continue;
                                    if (property.DeclaringType == typeof(Renderer) && property.Name == "materials") continue;
                                    if (property.GetValue(component) is not Array array) continue;
                                    foreach (var item in array)
                                    {
                                        if (item == _objectToFind)
                                        {
                                            results.Add(component);
                                            break;
                                        }
                                    }
                                }
                                else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericArguments()[0] == _objectToFind.GetType())
                                {
                                    var enumerable = property.GetValue(component) as IEnumerable<object>;
                                    if (enumerable == null) continue;
                                    foreach (var item in enumerable)
                                    {
                                        if (item == _objectToFind)
                                        {
                                            results.Add(component);
                                            break;
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                // ignore
                            }
                        }
                    }
                }
                
                _results = results.Distinct().ToArray();
            }
            
            if (GUILayout.Button("Find References in Scene"))
            {
                var results = new List<Object>();
                if (_objectToFind is GameObject)
                {
                    foreach (var sceneGameObject in FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                    {
                        if (PrefabUtility.GetPrefabInstanceStatus(sceneGameObject) != PrefabInstanceStatus.NotAPrefab)
                        {
                            var prefab = PrefabUtility.GetCorrespondingObjectFromSource(sceneGameObject);
                            if (prefab == _objectToFind)
                            {
                                results.Add(sceneGameObject);
                                continue;
                            }
                            prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(sceneGameObject);
                            if (prefab == _objectToFind)
                            {
                                results.Add(sceneGameObject);
                                continue;
                            }
                        }

                        if (sceneGameObject == _objectToFind)
                        {
                            results.Add(sceneGameObject);
                        }
                    }
                    
                    // Find references in MonoBehaviours
                    foreach(var sceneComponent in FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                    {
                        // Get All Scene Component References
                        var component = sceneComponent;
                        
                        var fields = component.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        foreach (var field in fields)
                        {
                            if (field.FieldType == _objectToFind.GetType())
                            {
                                var value = field.GetValue(component);
                                if (value == _objectToFind)
                                {
                                    results.Add(component);
                                    break;
                                }
                            }
                        }
                        
                        var properties = component.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        foreach (var property in properties)
                        {
                            if (property.PropertyType == _objectToFind.GetType())
                            {
                                if (property.DeclaringType == typeof(Renderer) && property.Name == "material") continue;
                                if (property.DeclaringType == typeof(Renderer) && property.Name == "materials") continue;
                                var value = property.GetValue(component);
                                if (value == _objectToFind)
                                {
                                    results.Add(component);
                                    break;
                                }
                            }
                        }
                        
                        // Check field nested classes/structs
                        foreach (var field in fields)
                        {
                            try
                            {
                                if (field.FieldType.IsClass || field.FieldType.IsValueType)
                                {
                                    var nestedFields = field.FieldType.GetFields(System.Reflection.BindingFlags.Public |
                                        System.Reflection.BindingFlags.NonPublic |
                                        System.Reflection.BindingFlags.Instance);
                                    foreach (var nestedField in nestedFields)
                                    {
                                        if (nestedField.FieldType == _objectToFind.GetType())
                                        {
                                            var value = nestedField.GetValue(field.GetValue(component));
                                            if (value == _objectToFind)
                                            {
                                                results.Add(component);
                                                break;
                                            }
                                        }
                                    }

                                    var nestedProperties = field.FieldType.GetProperties(
                                        System.Reflection.BindingFlags.Public |
                                        System.Reflection.BindingFlags.NonPublic |
                                        System.Reflection.BindingFlags.Instance);
                                    foreach (var nestedProperty in nestedProperties)
                                    {
                                        try
                                        {
                                            if (nestedProperty.PropertyType == _objectToFind.GetType())
                                            {
                                                if (nestedProperty.DeclaringType == typeof(Renderer) && nestedProperty.Name == "material") continue;
                                                if (nestedProperty.DeclaringType == typeof(Renderer) && nestedProperty.Name == "materials") continue;
                                                var value = nestedProperty.GetValue(field.GetValue(component));
                                                if (value == _objectToFind)
                                                {
                                                    results.Add(component);
                                                    break;
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            // ignore
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                // ignore
                            }
                        }

                        // Check property nested classes/structs
                        foreach (var property in properties)
                        {
                            try
                            {
                                if (property.PropertyType.IsClass || property.PropertyType.IsValueType)
                                {
                                    var nestedFields = property.PropertyType.GetFields(System.Reflection.BindingFlags.Public |
                                        System.Reflection.BindingFlags.NonPublic |
                                        System.Reflection.BindingFlags.Instance);
                                    foreach (var nestedField in nestedFields)
                                    {
                                        if (nestedField.FieldType == _objectToFind.GetType())
                                        {
                                            if (property.DeclaringType == typeof(Renderer) && property.Name == "material") continue;
                                            if (property.DeclaringType == typeof(Renderer) && property.Name == "materials") continue;
                                            var value = nestedField.GetValue(property.GetValue(component));
                                            if (value == _objectToFind)
                                            {
                                                results.Add(component);
                                                break;
                                            }
                                        }
                                    }

                                    var nestedProperties = property.PropertyType.GetProperties(
                                        System.Reflection.BindingFlags.Public |
                                        System.Reflection.BindingFlags.NonPublic |
                                        System.Reflection.BindingFlags.Instance);
                                    foreach (var nestedProperty in nestedProperties)
                                    {
                                        try
                                        {
                                            if (nestedProperty.PropertyType == _objectToFind.GetType())
                                            {
                                                if (nestedProperty.DeclaringType == typeof(Renderer) && nestedProperty.Name == "material") continue;
                                                if (nestedProperty.DeclaringType == typeof(Renderer) && nestedProperty.Name == "materials") continue;
                                                if (property.DeclaringType == typeof(Renderer) && property.Name == "material") continue;
                                                if (property.DeclaringType == typeof(Renderer) && property.Name == "materials") continue;
                                                var value = nestedProperty.GetValue(property.GetValue(component));
                                                if (value == _objectToFind)
                                                {
                                                    results.Add(component);
                                                    break;
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            // ignore
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                // ignore
                            }
                        }
                        
                        // Check field arrays/enumerables
                        foreach (var field in fields)
                        {
                            try
                            {
                                if (field.FieldType.IsArray && field.FieldType.GetElementType() == _objectToFind.GetType())
                                {
                                    if (field.GetValue(component) is not Array array) continue;
                                    foreach (var item in array)
                                    {
                                        if (item == _objectToFind)
                                        {
                                            results.Add(component);
                                            break;
                                        }
                                    }
                                }
                                else if (field.FieldType.IsGenericType && field.FieldType.GetGenericArguments()[0] == _objectToFind.GetType())
                                {
                                    var enumerable = field.GetValue(component) as IEnumerable<object>;
                                    if (enumerable == null) continue;
                                    foreach (var item in enumerable)
                                    {
                                        if (item == _objectToFind)
                                        {
                                            results.Add(component);
                                            break;
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                // ignore
                            }
                        }
                        
                        // Check property arrays/enumerables
                        foreach (var property in properties)
                        {
                            try
                            {
                                if (property.PropertyType.IsArray && property.PropertyType.GetElementType() == _objectToFind.GetType())
                                {
                                    if (property.DeclaringType == typeof(Renderer) && property.Name == "material") continue;
                                    if (property.DeclaringType == typeof(Renderer) && property.Name == "materials") continue;
                                    if (property.GetValue(component) is not Array array) continue;
                                    foreach (var item in array)
                                    {
                                        if (item == _objectToFind)
                                        {
                                            results.Add(component);
                                            break;
                                        }
                                    }
                                }
                                else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericArguments()[0] == _objectToFind.GetType())
                                {
                                    var enumerable = property.GetValue(component) as IEnumerable<object>;
                                    if (enumerable == null) continue;
                                    foreach (var item in enumerable)
                                    {
                                        if (item == _objectToFind)
                                        {
                                            results.Add(component);
                                            break;
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                // ignore
                            }
                        }
                    }
                }
                else
                {
                    foreach(var sceneComponent in FindObjectsByType<Component>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                    {
                        // Get All Scene Component References
                        var component = sceneComponent;
                        
                        var fields = component.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        foreach (var field in fields)
                        {
                            if (field.FieldType == _objectToFind.GetType())
                            {
                                var value = field.GetValue(component);
                                if (value == _objectToFind)
                                {
                                    results.Add(component);
                                    break;
                                }
                            }
                        }
                        
                        var properties = component.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        foreach (var property in properties)
                        {
                            if (property.PropertyType == _objectToFind.GetType())
                            {
                                if (property.DeclaringType == typeof(Renderer) && property.Name == "material") continue;
                                if (property.DeclaringType == typeof(Renderer) && property.Name == "materials") continue;
                                var value = property.GetValue(component);
                                if (value == _objectToFind)
                                {
                                    results.Add(component);
                                    break;
                                }
                            }
                        }
                        
                        // Check field nested classes/structs
                        foreach (var field in fields)
                        {
                            try
                            {
                                if (field.FieldType.IsClass || field.FieldType.IsValueType)
                                {
                                    var nestedFields = field.FieldType.GetFields(System.Reflection.BindingFlags.Public |
                                        System.Reflection.BindingFlags.NonPublic |
                                        System.Reflection.BindingFlags.Instance);
                                    foreach (var nestedField in nestedFields)
                                    {
                                        if (nestedField.FieldType == _objectToFind.GetType())
                                        {
                                            var value = nestedField.GetValue(field.GetValue(component));
                                            if (value == _objectToFind)
                                            {
                                                results.Add(component);
                                                break;
                                            }
                                        }
                                    }

                                    var nestedProperties = field.FieldType.GetProperties(
                                        System.Reflection.BindingFlags.Public |
                                        System.Reflection.BindingFlags.NonPublic |
                                        System.Reflection.BindingFlags.Instance);
                                    foreach (var nestedProperty in nestedProperties)
                                    {
                                        try
                                        {
                                            if (nestedProperty.PropertyType == _objectToFind.GetType())
                                            {
                                                if (nestedProperty.DeclaringType == typeof(Renderer) && nestedProperty.Name == "material") continue;
                                                if (nestedProperty.DeclaringType == typeof(Renderer) && nestedProperty.Name == "materials") continue;
                                                var value = nestedProperty.GetValue(field.GetValue(component));
                                                if (value == _objectToFind)
                                                {
                                                    results.Add(component);
                                                    break;
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            // ignore
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                // ignore
                            }
                        }

                        // Check property nested classes/structs
                        foreach (var property in properties)
                        {
                            try
                            {
                                if (property.PropertyType.IsClass || property.PropertyType.IsValueType)
                                {
                                    var nestedFields = property.PropertyType.GetFields(System.Reflection.BindingFlags.Public |
                                        System.Reflection.BindingFlags.NonPublic |
                                        System.Reflection.BindingFlags.Instance);
                                    foreach (var nestedField in nestedFields)
                                    {
                                        if (nestedField.FieldType == _objectToFind.GetType())
                                        {
                                            if (property.DeclaringType == typeof(Renderer) && property.Name == "material") continue;
                                            if (property.DeclaringType == typeof(Renderer) && property.Name == "materials") continue;
                                            var value = nestedField.GetValue(property.GetValue(component));
                                            if (value == _objectToFind)
                                            {
                                                results.Add(component);
                                                break;
                                            }
                                        }
                                    }

                                    var nestedProperties = property.PropertyType.GetProperties(
                                        System.Reflection.BindingFlags.Public |
                                        System.Reflection.BindingFlags.NonPublic |
                                        System.Reflection.BindingFlags.Instance);
                                    foreach (var nestedProperty in nestedProperties)
                                    {
                                        try
                                        {
                                            if (nestedProperty.PropertyType == _objectToFind.GetType())
                                            {
                                                if (nestedProperty.DeclaringType == typeof(Renderer) && nestedProperty.Name == "material") continue;
                                                if (nestedProperty.DeclaringType == typeof(Renderer) && nestedProperty.Name == "materials") continue;
                                                if (property.DeclaringType == typeof(Renderer) && property.Name == "material") continue;
                                                if (property.DeclaringType == typeof(Renderer) && property.Name == "materials") continue;
                                                var value = nestedProperty.GetValue(property.GetValue(component));
                                                if (value == _objectToFind)
                                                {
                                                    results.Add(component);
                                                    break;
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            // ignore
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                // ignore
                            }
                        }
                        
                        // Check field arrays/enumerables
                        foreach (var field in fields)
                        {
                            try
                            {
                                if (field.FieldType.IsArray && field.FieldType.GetElementType() == _objectToFind.GetType())
                                {
                                    if (field.GetValue(component) is not Array array) continue;
                                    foreach (var item in array)
                                    {
                                        if (item == _objectToFind)
                                        {
                                            results.Add(component);
                                            break;
                                        }
                                    }
                                }
                                else if (field.FieldType.IsGenericType && field.FieldType.GetGenericArguments()[0] == _objectToFind.GetType())
                                {
                                    var enumerable = field.GetValue(component) as IEnumerable<object>;
                                    if (enumerable == null) continue;
                                    foreach (var item in enumerable)
                                    {
                                        if (item == _objectToFind)
                                        {
                                            results.Add(component);
                                            break;
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                // ignore
                            }
                        }
                        
                        // Check property arrays/enumerables
                        foreach (var property in properties)
                        {
                            try
                            {
                                if (property.PropertyType.IsArray && property.PropertyType.GetElementType() == _objectToFind.GetType())
                                {
                                    if (property.DeclaringType == typeof(Renderer) && property.Name == "material") continue;
                                    if (property.DeclaringType == typeof(Renderer) && property.Name == "materials") continue;
                                    if (property.GetValue(component) is not Array array) continue;
                                    foreach (var item in array)
                                    {
                                        if (item == _objectToFind)
                                        {
                                            results.Add(component);
                                            break;
                                        }
                                    }
                                }
                                else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericArguments()[0] == _objectToFind.GetType())
                                {
                                    var enumerable = property.GetValue(component) as IEnumerable<object>;
                                    if (enumerable == null) continue;
                                    foreach (var item in enumerable)
                                    {
                                        if (item == _objectToFind)
                                        {
                                            results.Add(component);
                                            break;
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                // ignore
                            }
                        }
                    }
                }
                
                if (_objectToFind is GameObject)
                    foreach(var component in results.OfType<Component>().ToArray())
                        if (component.gameObject == _objectToFind)
                            results.Remove(component);
                
                _results = results.Distinct().ToArray();
            }

            EditorGUI.EndDisabledGroup();
            
            if (_results == null)
            {
                return;
            }

            if (_results.Length == 0)
            {
                EditorGUILayout.LabelField("No results");
            }
            else
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Results");
                
                // Place the results in a scrollable view using remaining space
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, false, false);
                EditorGUI.BeginDisabledGroup(true);
                foreach (var result in _results)
                    EditorGUILayout.ObjectField(result, typeof(Object), true);
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndScrollView();
            }
        }

        private void LoadProjectDependencyCache(bool recursive = false)
        {
            _projectDependencyCache = new Dictionary<string, string[]>();
            var assetPaths = AssetDatabase.GetAllAssetPaths();
            foreach (var assetPath in assetPaths)
            {
                var dependencies = AssetDatabase.GetDependencies(assetPath, recursive);
                _projectDependencyCache.Add(assetPath, dependencies);
            }
        }
    }
}