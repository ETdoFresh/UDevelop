using ETdoFresh.ReadonlyInspectorAttribute;
using UnityEngine;

namespace ETdoFresh.SceneReferences
{
    public class SceneReference : ScriptableObject
    {
#if UNITY_EDITOR
        [Readonly] public UnityEditor.SceneAsset sceneAsset;
#endif
        public string sceneName;
        public int sceneIndex;
    }
}