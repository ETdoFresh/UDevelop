using System.Linq;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ETdoFresh.Editor
{
    [Overlay(typeof(SceneView), "Scene Loader")]
    public class SceneLoaderToolbarOverlay : ToolbarOverlay
    {
        private SceneLoaderToolbarOverlay() : base(SceneLoaderLoadSceneToolbarDropdown.ID) { }
    }

    [EditorToolbarElement(ID, typeof(SceneView))]
    public class SceneLoaderLoadSceneToolbarDropdown : EditorToolbarDropdown
    {
        public const string ID = "SceneLoaderToolbar/LoadScene";

        private static string dropChoice;

        public SceneLoaderLoadSceneToolbarDropdown()
        {
            var sceneName = SceneManager.GetActiveScene().name;
            var niceSceneName = ObjectNames.NicifyVariableName(sceneName);
            text = niceSceneName;
            dropChoice = sceneName;
            icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/ETdoFresh/Editor/unity-white.png");
            clicked += ShowDropdown;
        }

        void ShowDropdown()
        {
            var menu = new GenericMenu();
            foreach (var scenePath in EditorBuildSettings.scenes.Select(x => x.path))
            {
                var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                var niceSceneName = ObjectNames.NicifyVariableName(scene.name);
                menu.AddItem(new GUIContent(niceSceneName), dropChoice == scenePath, () =>
                {
                    text = niceSceneName;
                    dropChoice = scene.name;
                    EditorSceneManager.OpenScene(scenePath);
                });
            }
            menu.ShowAsContext();
        }
    }
}