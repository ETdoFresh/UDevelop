using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CommandSystem.Commands
{
    [Serializable]
    public class CreateAssetCommand : Command
    {
        private enum AssetType
        {
            GameObject,
            ScriptableObject,
            Material,
            SceneAsset
        }

        [SerializeField] private AssetType type;
        [SerializeField] private string path;

        public CreateAssetCommand(string commandInput) : base(commandInput) { }

        public override void OnRun(params string[] args)
        {
            var typeArg = args[1];
            var pathArg = args.Length > 2 ? args[2] : "";

            type = GetTypeFromArg(typeArg);
            path = GetPathFromArg(pathArg, type);

            switch (type)
            {
                case AssetType.GameObject:
                    CommandAsset.CreateAsset<GameObject>(path);
                    break;
                case AssetType.ScriptableObject:
                    CommandAsset.CreateAsset<ScriptableObject>(path);
                    break;
                case AssetType.Material:
                    CommandAsset.CreateAsset<Material>(path);
                    break;
                case AssetType.SceneAsset:
                    CommandAsset.CreateAsset<SceneAsset>(path);
                    break;
            }
        }

        private AssetType GetTypeFromArg(string arg)
        {
            arg = arg.ToLower();
            if (new[] { "gameobject", "prefab" }.Contains(arg)) return AssetType.GameObject;
            if (new[] { "scriptableobject", "scriptable" }.Contains(arg)) return AssetType.ScriptableObject;
            if (new[] { "material" }.Contains(arg)) return AssetType.Material;
            if (new[] { "sceneasset", "scene" }.Contains(arg)) return AssetType.SceneAsset;
            throw new Exception($"Invalid asset type: {arg}");
        }

        private string GetPathFromArg(string path, AssetType assetType, bool generateUniqueName = true)
        {
            if (string.IsNullOrEmpty(path))
                path = $"{assetType.ToString().ToLower()}.{GetExtFromType(assetType)}";

            if (!HasExt(path, assetType))
                path += $".{GetExtFromType(assetType)}";

            if (CommandAsset.Exists(path))
                if (generateUniqueName)
                    path = CommandAsset.GetUniquePath(path);
                else
                    throw new Exception($"Asset already exists at path: {path}");

            return path;
        }

        private string GetExtFromType(AssetType assetType)
        {
            return assetType switch
            {
                AssetType.GameObject => "prefab",
                AssetType.ScriptableObject => "asset",
                AssetType.Material => "mat",
                AssetType.SceneAsset => "unity",
                _ => throw new ArgumentOutOfRangeException(nameof(assetType), assetType, null)
            };
        }

        private bool HasExt(string path, AssetType assetType)
        {
            return path.ToLower().EndsWith(GetExtFromType(assetType));
        }
    }

    [Serializable]
    public class OpenSceneCommand : Command
    {
        [SerializeField] private string path;

        public OpenSceneCommand(string commandInput) : base(commandInput) { }

        public override void OnRun(params string[] args)
        {
            path = args[1];
            var sceneAsset = CommandAsset.FindAsset<SceneAsset>(path);
            if (sceneAsset == null)
                throw new Exception($"Scene asset not found at path: {path}");
#if UNITY_EDITOR
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(sceneAsset));
#else
            SceneManager.LoadScene(sceneAsset.name);
#endif
        }
    }
}