using System;
using UnityEngine;

namespace CommandSystem.Commands.Create
{
    [Serializable]
    public class CreatePrefabCommandCSharp : CommandCSharp
    {
        private string _prefabName;
        private string _prefabPath;

        public CreatePrefabCommandCSharp(string commandInput) : base(commandInput) { }

        public override void OnRun(params string[] args)
        {
#if UNITY_EDITOR
            var prefabNameOrPath = args.Length < 2 ? null : string.Join("_", args[1..]);
            // _prefabPath = CommandAsset.ResolvePath(prefabNameOrPath, "Prefab", ".prefab");
            // _prefabName = CommandAsset.GetNameFromPath(_prefabPath);
            var emptyGameObject = new GameObject(_prefabName);
            var prefab = UnityEditor.PrefabUtility.SaveAsPrefabAsset(emptyGameObject, _prefabPath);
            UnityEngine.Object.DestroyImmediate(emptyGameObject);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.Selection.activeObject = prefab;
#else
            throw new InvalidOperationException("Create Object in Project Commands only work in Unity Editor!");
#endif
        }

        public override void OnUndo()
        {
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.DeleteAsset(_prefabPath);
#endif
        }

        public override void OnRedo()
        {
#if UNITY_EDITOR
            var emptyGameObject = new GameObject(_prefabName);
            var prefab = UnityEditor.PrefabUtility.SaveAsPrefabAsset(emptyGameObject, _prefabPath);
            UnityEngine.Object.DestroyImmediate(emptyGameObject);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.Selection.activeObject = prefab;
#endif
        }
    }
}