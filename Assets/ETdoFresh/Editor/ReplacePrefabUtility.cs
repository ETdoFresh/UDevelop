using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ETdoFresh.Editor
{
    public class ReplacePrefabUtility : EditorWindow
    {
        private enum NameCopyOptions { KeepOriginal, CopyFromPrefab, ReplaceOldPrefabNameWithNewPrefabName }
        
        private Object _prefab;
        private Object _newPrefab;
        private NameCopyOptions _copyName = NameCopyOptions.KeepOriginal;
        private bool _copyPosition = true;
        private bool _copyRotation = true;
        private bool _copyScale = true;
        
        [MenuItem("ETdoFresh/Replace Prefab Utility")]
        public static void ShowWindow()
        {
            GetWindow<ReplacePrefabUtility>("Replace Prefab Utility");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("This utility will help you replace your selection with a prefab.", EditorStyles.wordWrappedLabel);
            
            // Selection Helper
            EditorGUILayout.LabelField("Selection Helper", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Select the prefab below, and then press the button to select all instances of that prefab in the scene.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();
            _prefab = EditorGUILayout.ObjectField("Prefab", _prefab, typeof(Object), false);
            EditorGUILayout.Space();
            EditorGUI.BeginDisabledGroup(!_prefab || _prefab is not GameObject);
            if (GUILayout.Button("Select All Instances"))
            {
                var prefabPath = AssetDatabase.GetAssetPath(_prefab);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                var instances = FindObjectsOfType<GameObject>(true);
                var selection = new List<Object>();
                foreach (var instance in instances)
                {
                    if (PrefabUtility.GetCorrespondingObjectFromSource(instance) != prefab) continue;
                    selection.Add(instance);
                }
                Selection.objects = selection.ToArray();
                Debug.Log($"Selected {selection.Count} instances of {_prefab.name}.");
            }
            EditorGUI.EndDisabledGroup();
            if (!_prefab)
            {
                EditorGUILayout.HelpBox("Select a prefab to enable this button.", MessageType.Info);
            } else if (_prefab is not GameObject)
            {
                EditorGUILayout.HelpBox("Object is not a Prefab GameObject. Please select a prefab to enable this button.", MessageType.Error);
            }
            EditorGUILayout.Space();
            
            // Replace Prefab
            EditorGUILayout.LabelField("Replace Prefab", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Select your options below, and then press the button to replace your selection with the prefab.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();
            _newPrefab = EditorGUILayout.ObjectField("New Prefab", _newPrefab, typeof(Object), false);
            _copyName = (NameCopyOptions)EditorGUILayout.EnumPopup("Copy Name", _copyName);
            _copyPosition = EditorGUILayout.Toggle("Copy Position", _copyPosition);
            _copyRotation = EditorGUILayout.Toggle("Copy Rotation", _copyRotation);
            _copyScale = EditorGUILayout.Toggle("Copy Scale", _copyScale);
            EditorGUILayout.Space();
            EditorGUI.BeginDisabledGroup(!_newPrefab || _newPrefab is not GameObject);
            if (GUILayout.Button("Replace Prefab"))
            {
                var newPrefabPath = AssetDatabase.GetAssetPath(_newPrefab);
                var newPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(newPrefabPath);
                var selection = Selection.gameObjects;
                var replaced = 0;
                var created = new List<Object>();
                foreach (var selected in selection)
                {
                    var instance = PrefabUtility.InstantiatePrefab(newPrefab, selected.transform.parent) as GameObject;
                    if (!instance) continue;
                    created.Add(instance);
                    Undo.RegisterCreatedObjectUndo(instance, "Replace Prefab");
                    
                    var originalPrefab = PrefabUtility.GetCorrespondingObjectFromSource(selected);
                    var originalPrefabName = originalPrefab ? originalPrefab.name : selected.name;
                    
                    instance.name = _copyName switch
                    {
                        NameCopyOptions.KeepOriginal => instance.name,
                        NameCopyOptions.CopyFromPrefab => newPrefab.name,
                        NameCopyOptions.ReplaceOldPrefabNameWithNewPrefabName => selected.name.Replace(originalPrefabName, newPrefab.name),
                        _ => instance.name
                    };
                    instance.transform.position = _copyPosition ? selected.transform.position : instance.transform.position;
                    instance.transform.rotation = _copyRotation ? selected.transform.rotation : instance.transform.rotation;
                    instance.transform.localScale = _copyScale ? selected.transform.localScale : instance.transform.localScale;
                    Undo.DestroyObjectImmediate(selected);
                    replaced++;
                }
                Selection.objects = created.ToArray();
                Debug.Log($"Replaced {replaced} instances of {_newPrefab.name}.");
            }
            EditorGUI.EndDisabledGroup();
            if (!_newPrefab)
            {
                EditorGUILayout.HelpBox("Select a prefab to enable this button.", MessageType.Info);
            } else if (_newPrefab is not GameObject)
            {
                EditorGUILayout.HelpBox("Object is not a Prefab GameObject. Please select a prefab to enable this button.", MessageType.Error);
            }
        }
    }
}