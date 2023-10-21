using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommandSystem
{
    public static class CommandAsset
    {
        public const string ASSET_PATH = "Assets/CommandLineAssets";

        public static T FindAsset<T>(string path = "") where T : Object
        {
#if UNITY_EDITOR
            string[] guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T).Name} {path}");
            if (guids.Length > 0)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
            }
#endif
            return null;
        }

        public static T CreateAsset<T>(string path = "") where T : Object
        {
#if UNITY_EDITOR
            if (typeof(T) == typeof(GameObject))
            {
                var prefab = new GameObject(path);
                var prefabPath = $"{ASSET_PATH}/{path}";
                UnityEditor.PrefabUtility.SaveAsPrefabAsset(prefab, prefabPath);
                Object.DestroyImmediate(prefab);
                return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(prefabPath);
            }

            if (typeof(T) == typeof(ScriptableObject))
            {
                var asset = ScriptableObject.CreateInstance(typeof(T));
                var assetPath = $"{ASSET_PATH}/{path}";
                UnityEditor.AssetDatabase.CreateAsset(asset, assetPath);
                return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
            }

            if (typeof(T) == typeof(Material))
            {
                var asset = new Material(Shader.Find("Standard"));
                var assetPath = $"{ASSET_PATH}/{path}";
                UnityEditor.AssetDatabase.CreateAsset(asset, assetPath);
                return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
            }

            if (typeof(T) == typeof(UnityEditor.SceneAsset))
            {
                var emptySceneAsset = FindAsset<UnityEditor.SceneAsset>("EmptyScene");
                var asset = Object.Instantiate(emptySceneAsset);
                var assetPath = $"{ASSET_PATH}/{path}";
                UnityEditor.AssetDatabase.CreateAsset(asset, assetPath);
                return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
            }
#endif
            throw new NotImplementedException();
        }

        public static string GetUniquePath(string path)
        {
#if UNITY_EDITOR
            var assetPath = $"{ASSET_PATH}/{path}";
            return UnityEditor.AssetDatabase.GenerateUniqueAssetPath(assetPath);
#endif
            throw new NotImplementedException();
        }

        public static bool Exists(string path)
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(Object)) != null;
#endif
            throw new NotImplementedException();
        }

        private static string GetSceneNameFromPath(string path)
        {
            var split = path.Split('/');
            var filename = split[^1];
            split = filename.Split('.');
            return split[0];
        }
    }
}