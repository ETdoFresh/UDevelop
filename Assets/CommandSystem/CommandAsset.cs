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

        public static string GetUniquePath(string path, bool generateUniqueName)
        {
            if (!System.IO.File.Exists(path)) return path;
#if UNITY_EDITOR
            if (generateUniqueName) return UnityEditor.AssetDatabase.GenerateUniqueAssetPath(path);
            throw new Exception($"Asset already exists at path: {path}");
#else
            throw new NotImplementedException();
#endif
        }

        public static string HandleNullOrEmptyPath(string path, string defaultFilename)
        {
            return string.IsNullOrEmpty(path) ? defaultFilename : path;
        }

        public static string HandleMissingExtension(string path, string defaultExtension)
        {
            var hasExtension = path.ToLower().EndsWith(defaultExtension);
            return hasExtension ? path : path + defaultExtension;
        }

        private static string HandleMissingAssetPath(string path)
        {
            return path.StartsWith(ASSET_PATH) ? path : System.IO.Path.Combine(ASSET_PATH, path);
        }

        public static string HandleMissingDirectory(string path)
        {
            var noAssetsPath = path.StartsWith("Assets") ? path[6..] : path;
            var combinedPath = System.IO.Path.Combine(Application.dataPath, noAssetsPath);
            var directory = System.IO.Path.GetDirectoryName(combinedPath);
            if (!string.IsNullOrEmpty(directory) && !System.IO.Directory.Exists(directory))
                System.IO.Directory.CreateDirectory(directory);
            return path;
        }

        public static string GetNameFromPath(string path)
        {
            return System.IO.Path.GetFileNameWithoutExtension(path);
        }

        public static string ResolvePath(string nameOrPath, string defaultFilename, string defaultExtension, bool generateUniqueName = true)
        {
            var path = nameOrPath;
            path = HandleNullOrEmptyPath(path, $"{defaultFilename}{defaultExtension}");
            path = HandleMissingExtension(path, defaultExtension);
            path = HandleMissingAssetPath(path);
            path = HandleMissingDirectory(path);
            path = GetUniquePath(path, generateUniqueName);
            return path;
        }
    }
}