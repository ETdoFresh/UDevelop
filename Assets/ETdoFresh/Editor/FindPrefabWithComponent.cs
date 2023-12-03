#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ETdoFresh.Editor
{
    public class FindPrefabWithComponent : EditorWindow
    {
        private string _componentName;
        private string _componentNameLower;
        private List<string> _prefabPaths = new();

        [MenuItem("ETdoFresh/Find Prefab With Component")]
        public static void ShowWindow()
        {
            GetWindow(typeof(FindPrefabWithComponent));
        }

        public FindPrefabWithComponent()
        {
            titleContent = new GUIContent("Find Prefab With Component");
        }

        private void OnGUI()
        {
            GUILayout.Label("Find Prefab With Component", EditorStyles.boldLabel);
            _componentName = EditorGUILayout.TextField("Component Name", _componentName);
            _componentNameLower = _componentName.ToLower();
            if (GUILayout.Button("Find Prefabs"))
            {
                _prefabPaths = new List<string>();
                var guids = AssetDatabase.FindAssets("t:Prefab");
                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    var components = prefab.GetComponentsInChildren<Component>();
                    foreach (var component in components)
                    {
                        if (!component) continue;
                        var componentName = component.GetType().Name;
                        if (componentName.ToLower().Contains(_componentNameLower))
                        {
                            _prefabPaths.Add(path);
                            break;
                        }
                    }
                }
            }

            if (_prefabPaths.Count > 0)
            {
                GUILayout.Label("Prefabs with " + _componentName + " component", EditorStyles.boldLabel);
                foreach (var prefabPath in _prefabPaths)
                {
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                    EditorGUILayout.ObjectField(prefab, typeof(GameObject), false);
                }
            }
        }
    }
}
#endif