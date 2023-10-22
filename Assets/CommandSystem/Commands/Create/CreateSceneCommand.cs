using System;

namespace CommandSystem.Commands.Create
{
    [Serializable]
    public class CreateSceneCommand : Command
    {
        private string _sceneName;
        private string _scenePath;

        public CreateSceneCommand(string commandInput) : base(commandInput) { }

        public override bool AddToHistory => true;
        public override string CommandOutput => $"Created Scene {_sceneName}";

        public override string[] CommandNames => new[] { "create-scene", "createscene", "c-s" };
        public override string CommandUsage => $"{CommandNames[0]} [SCENE_NAME/PATH]";
        public override string CommandDescription => "Creates an empty .unity object in project.";

        public override void OnRun(params string[] args)
        {
#if UNITY_EDITOR
            var sceneNameOrPath =  args.Length < 3 ? null : string.Join("_", args[2..]);
            _scenePath = CommandAsset.ResolvePath(sceneNameOrPath, "Scene", ".unity");
            _sceneName = CommandAsset.GetNameFromPath(_scenePath);
            var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(
                UnityEditor.SceneManagement.NewSceneSetup.EmptyScene, UnityEditor.SceneManagement.NewSceneMode.Additive);
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, _scenePath);
            UnityEditor.SceneManagement.EditorSceneManager.CloseScene(scene, true);
            var sceneAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.SceneAsset>(_scenePath);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.Selection.activeObject = sceneAsset;
#endif
        }

        public override void OnUndo()
        {
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.DeleteAsset(_scenePath);
#endif
        }

        public override void OnRedo()
        {
#if UNITY_EDITOR
            var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(
                UnityEditor.SceneManagement.NewSceneSetup.EmptyScene, UnityEditor.SceneManagement.NewSceneMode.Single);
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, _scenePath);
            var sceneAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.SceneAsset>(_scenePath);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.Selection.activeObject = sceneAsset;
#endif
        }
    }
}