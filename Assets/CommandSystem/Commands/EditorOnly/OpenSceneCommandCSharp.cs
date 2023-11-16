using System;
using System.Linq;

namespace CommandSystem.Commands.EditorOnly
{
    [Serializable]
    public class OpenSceneCommandCSharp : CommandCSharp
    {
        public OpenSceneCommandCSharp(string commandInput) : base(commandInput) { }

        public override void OnRun(params string[] args)
        {
#if UNITY_EDITOR
            string scenePath = null;
            if (args.Length < 2)
            {
                scenePath = UnityEditor.Selection.objects
                    .Where(x => x is UnityEditor.SceneAsset)
                    .Select(UnityEditor.AssetDatabase.GetAssetPath)
                    .FirstOrDefault();
                if (scenePath == null)
                    throw new ArgumentException("No Scene selected!");
            }
            var sceneName = string.Join("_", args[1..]);
            // var sceneAsset = CommandAsset.FindAsset<UnityEditor.SceneAsset>(sceneName);
            // if (sceneAsset) scenePath = UnityEditor.AssetDatabase.GetAssetPath(sceneAsset);
            //
            // if (scenePath == null)
            //     throw new ArgumentException($"Scene not found! {sceneName}");
            //
            // UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath);
#endif
        }
    }
}